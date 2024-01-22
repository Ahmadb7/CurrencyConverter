using EnsureThat;
using QuikGraph;
using QuikGraph.Algorithms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TechTest.ConsoleApp
{
    internal class SimpleCurrencyConverter
    {
        private Dictionary<string, Dictionary<string, decimal>> data = new Dictionary<string, Dictionary<string, decimal>>();
        private AdjacencyGraph<string, Edge<string>> adjacencyGraph = new AdjacencyGraph<string, Edge<string>>();
        private Dictionary<Edge<string>, double> edgeAmountDictionary = new Dictionary<Edge<string>, double>();

        private Func<Edge<string>, double> edgeCost = edge => 1;
        private Func<Edge<string>, double> edgeRate = null;

        public SimpleCurrencyConverter()
        {
            edgeRate = AlgorithmExtensions.GetIndexer(edgeAmountDictionary);
        }

        public decimal Convert(string from, string to)
        {
            Ensure.That(from).IsNotNull();
            Ensure.That(to).IsNotNull();

            decimal rate = 0;
            if (!data.ContainsKey(from))
            {
                if (!FindRount(from, to, out rate))
                    throw new ArgumentOutOfRangeException(nameof(from));
                return rate;
            }
            else
            {

                if (!data[from].ContainsKey(to))
                {
                    if (!FindRount(from, to, out rate))
                        throw new ArgumentOutOfRangeException(nameof(to));
                    return rate;
                }
                else
                {
                    rate = data[from][to];
                }
            }

            return rate;
        }

        private bool FindRount(string from, string to, out decimal rate)
        {
            rate = 0;
            // Compute shortest paths
            TryFunc<string, IEnumerable<Edge<string>>> tryGetPaths = adjacencyGraph.ShortestPathsDijkstra(edgeCost, from);

            // Query path for given vertices
            string target = to;
            if (tryGetPaths(target, out IEnumerable<Edge<string>> path))
            {
                var edgeRateResult = 1d;
                foreach (var edge in path)
                {
                    var curEdgeAmount = edgeRate(edge);
                    edgeRateResult *= curEdgeAmount;
                    Debug.WriteLine($"{edge}:{curEdgeAmount}");
                }
                rate = (decimal)edgeRateResult;

                return true;
            }

            return false;
        }

        public void InitializeData()
        {
            var initData = "" +
                "(USD => CAD) 1.34\r\n" +
                "(CAD => GBP) 0.58\r\n" +
                "(USD => EUR) 0.86\r\n";

            string[] convertedString = NormalizeInputData(initData);
            ConvertInputData(convertedString);

        }

        private void ConvertInputData(string[] convertedString)
        {
            // each line contains: 'CAD=>GBP:0.58'
            convertedString.ToList().ForEach(line =>
            {
                var lineSplit = line.Split(new[] { "=>", ":" }, StringSplitOptions.RemoveEmptyEntries);
                var from = lineSplit[0];
                var to = lineSplit[1];
                var amount = decimal.Parse(lineSplit[2]);

                AddVertex(from);
                AddVertex(to);
                AddEdge(from, to, (double)amount);
                AddEdge(to, from, (double)(1/amount));

                if (data.ContainsKey(from))
                {
                    data[from][to] = amount;
                }
                else
                {
                    data[from] = new Dictionary<string, decimal>
                    {
                        [to] = amount
                    };
                }
            });
        }

        private void AddVertex(string vertex)
        {
            if (!adjacencyGraph.Vertices.Contains(vertex))
                adjacencyGraph.AddVertex(vertex);
        }

        private void AddEdge(string from, string to, double amount = 0)
        {
            var edge = new Edge<string>(from, to);
            var result = adjacencyGraph.AddEdge(edge);
            edgeAmountDictionary.Add(edge, amount);
        }

        private string[] NormalizeInputData(string initData)
        {
            return initData
                            .Replace("(", "")
                            .Replace(")", ":")
                            .Replace(" ", "")
                            .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
