using System.Collections.Generic;
using System.Linq;
using PeanutButter.INIFile;

namespace Ghoul
{
    internal interface ISectionNameHelper
    {
        string[] ListLayoutNames();

        string CreateAppLayoutSectionNameFor(
            string layoutName,
            string appName);

        string[] ListAppLayoutSectionsFor(string layout);
    }

    internal class SectionNameHelper
        : ISectionNameHelper
    {
        private readonly INIFile _config;

        public SectionNameHelper(INIFile config)
        {
            _config = config;
        }

        public string[] ListLayoutNames()
        {
            return _config.Sections.Aggregate(
                new List<string>(),
                (acc, cur) =>
                {
                    if (!cur.StartsWith(Constants.APP_LAYOUT_SECTION_PREFIX))
                        return acc;
                    var layoutName = GrokLayoutNameFrom(cur);
                    if (layoutName == null)
                        return acc;
                    if (!acc.Contains(layoutName))
                        acc.Add(layoutName);
                    return acc;
                }).ToArray();
        }

        public string CreateAppLayoutSectionNameFor(
            string layoutName,
            string appName)
        {
            return $"{Constants.APP_LAYOUT_SECTION_PREFIX}{layoutName} : {appName}";
        }

        public string[] ListAppLayoutSectionsFor(string layout)
        {
            var search = $"{Constants.APP_LAYOUT_SECTION_PREFIX}{layout} :";
            return _config.Sections.Where(
                s => s.StartsWith(search)
                ).ToArray();
        }

        private string GrokLayoutNameFrom(string cur)
        {
            var parts = cur.Split(':');
            return parts.Length < 3
                ? null
                : parts[1]?.Trim();
        }
    }
}