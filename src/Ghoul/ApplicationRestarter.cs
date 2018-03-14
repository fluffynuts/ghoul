using System;
using System.Diagnostics;
using System.Linq;
using PeanutButter.INIFile;
using PeanutButter.Utils;
using Sections = Ghoul.Constants.Sections;

namespace Ghoul
{
    internal interface IApplicationRestarter
    {
        void RestartApplicationsForLayout(
            string layoutName
        );
    }

    internal class ApplicationRestarter
        : IApplicationRestarter
    {
        private readonly IINIFile _config;

        public ApplicationRestarter(IINIFile config)
        {
            _config = config;
        }

        public void RestartApplicationsForLayout(
            string layoutName
        )
        {
            var sectionName = RestoreAppsSectionFor(layoutName);
            if (!_config.HasSection(sectionName))
                return;
            var running = Process.GetProcesses();
            _config[sectionName].Keys.ForEach(
                k =>
                {
                    var match = MatchProcess(k, running);
                    match?.Kill();
                    StartProcess(k);
                });
        }

        private static void StartProcess(string path)
        {
            var proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = path,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    UseShellExecute = true
                }
            };
            proc.Start();
        }

        private static Process MatchProcess(
            string name,
            Process[] running
        )
        {
            return ProcessFinders
                .FirstOrDefault(t => t.Item1(name))
                ?.Item2(name, running);
        }

        private static string RestoreAppsSectionFor(string layoutName)
        {
            return $"{Sections.RESTORE_PREFIX}{layoutName}";
        }

        private static readonly Tuple<Func<string, bool>, Func<string, Process[], Process>>[] ProcessFinders =
        {
            CreateTuple(FullPathMatch, FindProcessByFullPath)
        };

        private static Tuple<Func<string, bool>, Func<string, Process[], Process>> CreateTuple(
            Func<string, bool> matcher,
            Func<string, Process[], Process> processFinder
        )
        {
            return Tuple.Create(matcher, processFinder);
        }

        private static bool FullPathMatch(string path)
        {
            return path.Contains("\\") || path.Contains("/");
        }

        private static Process FindProcessByFullPath(string path, Process[] processes)
        {
            return processes.FirstOrDefault(
                p =>
                {
                    try
                    {
                        return p.MainModule.FileName.Equals(path, StringComparison.OrdinalIgnoreCase);
                    }
                    catch
                    {
                        return false;
                    }
                });
        }

    }
}
