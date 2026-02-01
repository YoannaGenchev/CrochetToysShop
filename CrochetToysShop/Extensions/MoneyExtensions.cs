namespace CrochetToysShop.Extensions
{
    public static class MoneyExtensions
    {
        public static string ToEuro(this decimal value)
            => $"{value:0.00} €";
    }
}
