using System;
using System.Collections.Generic;
using System.Linq;

namespace TechTest.ConsoleApp
{
    public class Program
    {

        static void Main(string[] args)
        {
            //CustomCurrencyConverter.Instance.UpdateConfiguration(InitData());

            var from = "USD";
            var to = "EUR";
            var result = -1d;

            try
            {
                result = CustomCurrencyConverter.Instance.Convert(from, to, 1000);
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadLine();
        }

        private static IEnumerable<Tuple<string, string, double>> InitData()
        {
            var initData = "" +
                "(USD => CAD) 1.34\r\n" +
                "(CAD => GBP) 0.58\r\n" +
                "(USD => EUR) 0.86\r\n";

            string[] convertedString = NormalizeInputData(initData);
            return ConvertInputData(convertedString);
        }

        private static IEnumerable<Tuple<string, string, double>> ConvertInputData(string[] convertedString)
        {
            // each line contains: 'CAD=>GBP:0.58'
            return convertedString
                .ToList()
                .Select(line => line.Split(new[] { "=>", ":" }, StringSplitOptions.RemoveEmptyEntries))
                .Select(item =>
                {
                    var from = item[0];
                    var to = item[1];
                    var amount = double.Parse(item[2]);

                    return new Tuple<string, string, double>(from, to, amount);
                });
        }

        private static string[] NormalizeInputData(string initData)
        {
            return initData
                            .Replace("(", "")
                            .Replace(")", ":")
                            .Replace(" ", "")
                            .Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

    }
}
