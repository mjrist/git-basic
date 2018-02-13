namespace GitBasic
{
    public static class StringExtensions
    {
        public static string SubstringFromLast(this string value, char character)
        {
            int i = value.LastIndexOf(character);
            return (i < 0) ? string.Empty : value.Substring(i + 1);
        }       
    }
}
