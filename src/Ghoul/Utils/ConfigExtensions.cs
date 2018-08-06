using System.Collections.Generic;
using System.Linq;
using PeanutButter.INIFile;
using PeanutButter.Utils;

namespace Ghoul.Utils
{
    public static class ConfigExtensions
    {
        public static IEnumerable<string> EnumerateLayouts(this IINIFile config)
        { 
            return config.Sections
                .Where(s => s.StartsWith(Constants.Sections.APP_LAYOUT_PREFIX))
                .Select(GetLayoutName)
                .Where(s => !s.IsNullOrEmpty())
                .Distinct();
        }

        private static string GetLayoutName(string str)
        {
            return str.Split(':')
                .Skip(1)
                .FirstOrDefault()
                ?.Trim();
        }
    }
}