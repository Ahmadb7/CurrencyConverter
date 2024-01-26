using EnsureThat;
using QuikGraph;
using QuikGraph.Algorithms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TechTest.ConsoleApp.Exceptions;

namespace TechTest.ConsoleApp
{
    public class CustomCurrencyConverter : ICurrencyConverter
    {
        private ConcurrentDictionary<string, Dictionary<string, double>> data = new ConcurrentDictionary<string, Dictionary<string, double>>();
        private AdjacencyGraph<string, Edge<string>> adjacencyGraph = new AdjacencyGraph<string, Edge<string>>();
        private ConcurrentDictionary<Edge<string>, double> edgeRateDictionary = new ConcurrentDictionary<Edge<string>, double>();

        private Func<Edge<string>, double> edgeCost = edge => 1;
        private Func<Edge<string>, double> edgeRate = null;

        private static readonly Lazy<CustomCurrencyConverter> _instance =
        new Lazy<CustomCurrencyConverter>(() => new CustomCurrencyConverter());

        public static CustomCurrencyConverter Instance
        {
            get { return _instance.Value; }
        }

        private CustomCurrencyConverter()
        {
            edgeRate = AlgorithmExtensions.GetIndexer(edgeRateDictionary);
        }

        public void ClearConfiguration()
        {
            data.Clear();
            adjacencyGraph.Clear();
            edgeRateDictionary.Clear();
        }

        public double Convert(string fromCurrency, string toCurrency, double amount)
        {
            var rate = Convert(fromCurrency, toCurrency);

            return amount * rate;
        }

        public void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates)
        {
            conversionRates.ToList().ForEach(rate =>
            {
                var from = rate.Item1;
                var to = rate.Item2;
                var amount = rate.Item3;

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
                    data[from] = new Dictionary<string, double>
                    {
                        [to] = amount
                    };
                }

            });
        }

        private double Convert(string from, string to)
        {
            Ensure.That(from).IsNotNull();
            Ensure.That(to).IsNotNull();

            double rate = 0;
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

        private bool FindRount(string from, string to, out double rate)
        {
            rate = 0;

            if(adjacencyGraph == null || adjacencyGraph.EdgeCount == 0 || adjacencyGraph.VertexCount == 0)
            {
                throw new NoConfigurationDataException("No configuration data is imported.");
            }

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
                rate = edgeRateResult;

                return true;
            }

            return false;
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
            edgeRateDictionary.AddOrUpdate(edge, amount, (k, v) => v);
        }

    }
}
