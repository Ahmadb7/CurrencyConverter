namespace TechTest.ConsoleApp
{

    internal class CustomCurrency
    {
        public string Name { get; set; }
        public CurrencyCode FromCurrency { get; set; }
        public CurrencyCode ToCurrency { get; set; }
        public decimal Amount { get; set; }

        public CustomCurrency()
        {
            
        }
    }
}
