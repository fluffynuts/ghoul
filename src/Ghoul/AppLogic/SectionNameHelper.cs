using System.Collections.Generic;
using System.Linq;
using PeanutButter.INIFile;
using Sections = Ghoul.Utils.Constants.Sections;

namespace Ghoul.AppLogic
{
    public interface ISectionNameHelper
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
        private readonly IINIFile _config;

        public SectionNameHelper(IINIFile config)
        {
            _config = config;
        }

        public string[] ListLayoutNames()
        {
            return _config.Sections.Aggregate(
                new List<string>(),
                (acc, cur) =>
                {
                    if (!cur.StartsWith(Sections.APP_LAYOUT_PREFIX))
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
            return $"{Sections.APP_LAYOUT_PREFIX}{layoutName} : {appName}";
        }

        public string[] ListAppLayoutSectionsFor(string layout)
        {
            var search = $"{Sections.APP_LAYOUT_PREFIX}{layout} :";
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