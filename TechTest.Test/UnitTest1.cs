using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTest.ConsoleApp;
using TechTest.ConsoleApp.Exceptions;

namespace TechTest.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestDirectRoute()
        {
            CustomCurrencyConverter.Instance.ClearConfiguration();

            CustomCurrencyConverter.Instance.UpdateConfiguration(InitData());

            var result = CustomCurrencyConverter.Instance.Convert("USD", "CAD", 2000);

            Assert.IsTrue(Math.Round(result, 2) == 2680.00);
        }

        [TestMethod]
        public void TestIndirectRoute()
        {
            CustomCurrencyConverter.Instance.ClearConfiguration();

            CustomCurrencyConverter.Instance.UpdateConfiguration(InitData());

            var result = CustomCurrencyConverter.Instance.Convert("CAD", "EUR", 2000);

            Assert.IsTrue(Math.Round(result, 2) == 1283.58);
        }

        [TestMethod]
        public void TestNoConfigurationData()
        {
            CustomCurrencyConverter.Instance.ClearConfiguration();
            //CustomCurrencyConverter.Instance.UpdateConfiguration(InitData());

            Assert.ThrowsException<NoConfigurationDataException>(() => CustomCurrencyConverter.Instance.Convert("USD", "CAD", 2000));

        }

        private IEnumerable<Tuple<string, string, double>> InitData()
        {
            var initData = "" +
                "(USD => CAD) 1.34\r\n" +
                "(CAD => GBP) 0.58\r\n" +
                "(USD => EUR) 0.86\r\n";

            string[] convertedString = NormalizeInputData(initData);
            return ConvertInputData(convertedString);
        }

        private IEnumerable<Tuple<string, string, double>> ConvertInputData(string[] convertedString)
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
