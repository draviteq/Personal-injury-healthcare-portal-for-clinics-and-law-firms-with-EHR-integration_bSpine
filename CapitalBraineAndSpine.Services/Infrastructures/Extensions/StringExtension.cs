namespace CapitalBraineAndSpine.Service.Infrastructures.Extensions
{
    public static class StringExtension
    {
        public static string GetName(this string member) => nameof(member);

        public static string ReplacePipeWithSlash(this string str)
        {
            return str.Replace("|", "/");
        }

        public static string ReplaceSlashWithPipe(this string str)
        {
            return str.Replace("/", "|");
        }

        public static string ConvertToX(this string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= 2)
                return input;

            // Keep the first two characters and replace the rest with 'X'
            string firstTwo = input.Substring(0, 2);
            string converted = new string('X', input.Length - 2);

            return firstTwo + converted;
        }

        /// <summary>
        /// Returns a dash ("--") if the string is null or empty.
        /// Otherwise, returns the original string.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <returns>The original string or "--" if null or empty.</returns>
        public static string ToDashIfNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) ? "--" : value;
        }
    }
}
