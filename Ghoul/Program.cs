using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using PeanutButter.INIFile;
using PeanutButter.TrayIcon;
using PeanutButter.Utils;

namespace Ghoul
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var trayIcon = new TrayIcon(Resources.Ghoul);
            var config = LoadConfig();
            AddSaveLayoutItem(trayIcon, config);
            var menu = AddRestoreLayoutMenuTo(trayIcon);
            AddRestoreMenusTo(menu, config, trayIcon);
            AddExitMenuItemTo(trayIcon);
            config.Persist();
            trayIcon.Show();

            Application.Run();
        }

        private static void AddExitMenuItemTo(
            TrayIcon trayIcon
        )
        {
            trayIcon.AddMenuSeparator();
            trayIcon.AddMenuItem("Exit", () =>
            {
                trayIcon.Hide();
                Application.Exit();
            });
        }

        private static void AddRestoreMenusTo(
            MenuItem menu,
            INIFile config,
            TrayIcon trayIcon
        )
        {
            config.Sections.Where(IsLayoutSection)
                .ForEach(s =>
                {
                    var layoutName = LayoutNameFor(s);
                    trayIcon.AddMenuItem(layoutName, () =>
                    {
                        // TODO
                        RestartApplicationsForLayout(layoutName, config);
                    }, menu);
                });
        }

        private static void RestartApplicationsForLayout(
            string layoutName,
            INIFile config
        )
        {
            var sectionName = RestoreAppsSectionFor(layoutName);
            if (!config.HasSection(sectionName))
                return;
            var running = Process.GetProcesses();
            config[sectionName].Keys.ForEach(k =>
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
            return processes.FirstOrDefault(p =>
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


        private static string RestoreAppsSectionFor(string layoutName)
        {
            return $"{RESTORE_SECTION_PREFIX}{layoutName}";
        }

        private static void AddSaveLayoutItem(TrayIcon trayIcon, INIFile config)
        {
            trayIcon.AddMenuItem("Save current layout...", () =>
            {
                var name = GetUserInput("Enter name for layout", "Please enter a name for the layout to be saved");
                if (string.IsNullOrWhiteSpace(name))
                    return;
                var util = new DesktopWindowUtil();
                var processWindows = util.ListWindows();
                var sectionName = MakeLayoutSectionName(name);
                config.AddSection(sectionName);
                RemoveAllAppLayoutSectionsFor(name, config);
                processWindows.ForEach(w =>
                {
                    var pos = w?.Position;
                    var proc = w?.Process?.MainModule?.FileName;
                    if (string.IsNullOrWhiteSpace(proc) || pos == null)
                        return;
                    config[sectionName][proc] = $"{pos.Top} {pos.Left} {pos.Width} {pos.Height}";
                });
                config.Persist();
            });
        }

        private static void RemoveAllAppLayoutSectionsFor(
            string layoutName,
            INIFile config
        )
        {
            // TODO: continue from here with updated PB
//            var search = MakeAppLayoutSectionName(layoutName, "");
//            config.Sections
//                .Where(s => s.StartsWith(search))
//                .ToArray()
//                .ForEach(s => config.RemoveSection(s));
        }

        private static string MakeAppLayoutSectionName(string layoutName, string appName)
        {
            return $"{APP_LAYOUT_SECTION_PREFIX}{layoutName} : {appName}";
        }

        private static MenuItem AddRestoreLayoutMenuTo(TrayIcon trayIcon)
        {
            return trayIcon.AddSubMenu("Restore layout");
        }

        private static bool IsLayoutSection(string sectionName)
        {
            return sectionName?.StartsWith(LAYOUT_SECTION_PREFIX) ?? false;
        }

        private static string LayoutNameFor(string sectionName)
        {
            return sectionName.Substring(LAYOUT_SECTION_PREFIX.Length);
        }

        private const string LAYOUT_SECTION_PREFIX = "layout: ";
        private const string RESTORE_SECTION_PREFIX = "restore: ";
        private const string APP_LAYOUT_SECTION_PREFIX = "app-layout: ";

        private static string MakeLayoutSectionName(string layoutName)
        {
            return $"{LAYOUT_SECTION_PREFIX}{layoutName}";
        }

        private static string GetUserInput(
            string caption,
            string message
        )
        {
            var prompt = new PromptForm(caption, message);
            var result = prompt.Prompt();
            return result.UserInput.Trim();
        }

        private static INIFile LoadConfig()
        {
            var configLocation = FindConfig();
            return new INIFile(configLocation);
        }

        private static string FindConfig()
        {
            var programPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var programFolder = Path.GetDirectoryName(programPath);
            return Path.Combine(programFolder, "ghoul.ini");
        }
    }
}