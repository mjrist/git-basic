namespace GitBasic
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a substring starting after the last instance of the specified character.
        /// If the specified character does not occur in the string, the result is empty.
        /// </summary>
        /// <param name="value">The string to get the substring from.</param>
        /// <param name="character">The character after whose last instance the substring will be taken.</param>
        /// <returns></returns>
        public static string SubstringFromLast(this string value, char character)
        {
            int i = value.LastIndexOf(character);
            return (i < 0) ? string.Empty : value.Substring(i + 1);
        }       
    }
}
