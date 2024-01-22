using System;

namespace TechTest.ConsoleApp
{
    public class Program
    {

        static void Main(string[] args)
        {
            
            var from = "USD";
            var to = "EUR";
            var result = CustomCurrencyConverter.Instance.Convert(from, to, 1000);

            Console.WriteLine("");
        }

    }
}
