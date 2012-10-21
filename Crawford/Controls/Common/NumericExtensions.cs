namespace System.Windows.Controls
{
    internal static class NumericExtensions
    {
        public static bool IsNumeric(this double value)
        {
            return (!double.IsInfinity(value) && !double.IsNaN(value));
        }
    }
}