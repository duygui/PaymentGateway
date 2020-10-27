namespace PaymentGatewayAPI.Helpers
{
    public static class StringHelper
    {
        public static string Masked(this string source)
            => source.Masked('*', 4, 8);

        public static string Masked(this string source, char maskValue, int start, int count)
            => $"{source.Substring(0, start)}{new string(maskValue, count)}{source.Substring(start + count)}";
    }
}
