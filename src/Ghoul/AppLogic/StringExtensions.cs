using System;
using System.Globalization;
using PeanutButter.Utils;

namespace Ghoul.AppLogic
{
    public static class StringExtensions
    {
        public static bool IsSimilarTo(this string str, string other)
        {
            var sourceParts = str.ToWords();
            var otherParts = other.ToWords();
            var maxScore = CalculateMaxScoreFor(sourceParts.Length);
            var finalScore = 0M;
            sourceParts.ForEach(
                (part, idx) =>
                {
                    var max = sourceParts.Length - idx;
                    var matchIndex = otherParts.IndexOf(part);
                    finalScore += matchIndex == idx
                        ? max
                        : max / 2;
                });
            return (finalScore / maxScore) > 0.5M;
        }

        private static decimal CalculateMaxScoreFor(int itemCount)
        {
            var result = 0;
            while (itemCount > 0)
            {
                result += itemCount;
                itemCount--;
            }

            return result;
        }

        private static int IndexOf(
            this string[] haystack,
            string needle,
            StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
        {
            var result = -1;
            haystack.ForEach(
                (item, idx) =>
                {
                    if (result == -1 && item.Equals(needle, comparison))
                        result = idx;
                });
            return result;
        }

        private static string[] ToWords(this string str)
        {
            return (str ?? "")
                .ToLower(CultureInfo.CurrentCulture)
                .Trim()
                .Split(' ', '\t');
        }
    }
}