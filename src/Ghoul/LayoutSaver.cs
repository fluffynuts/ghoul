﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PeanutButter.INIFile;
using PeanutButter.Utils;

namespace Ghoul
{
    internal class LayoutRestorer
    {
        private readonly INIFile _config;
        private readonly IApplicationRestarter _applicationRestarter;

        public LayoutRestorer(
            INIFile config,
            IApplicationRestarter applicationRestarter
        )
        {
            _config = config;
            _applicationRestarter = applicationRestarter;
        }

        public void RestoreLayout(string name)
        {
            _applicationRestarter.RestartApplicationsForLayout(name);
            RestoreWindowPositionsForLayout(name);
        }

        private void RestoreWindowPositionsForLayout(string name)
        {
            var section = GetSectionNameForLayoutName(name);
        }

        private object GetSectionNameForLayoutName(string name)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class LayoutSaver
    {
        private readonly INIFile _config;
        private readonly IUserInput _userInput;

        public LayoutSaver(
            INIFile config,
            IUserInput userInput
        )
        {
            _config = config;
            _userInput = userInput;
        }

        public void SaveCurrentLayout()
        {
            var layoutName = _userInput.Prompt(
                "Enter name for layout",
                "Please enter a name for the layout to be saved").UserInput;
            if (string.IsNullOrWhiteSpace(layoutName))
                return;

            // TODO: better input of layout names
            // TODO: verify with user when re-using a layout name: it will overwrite the existing settings

            var util = new DesktopWindowUtil();
            var processWindows = util.ListWindows();
            var selector = new CheckListDialog<ProcessWindow>(
                processWindows,
                new[]
                {
                    nameof(ProcessWindow.ProcessName),
                    nameof(ProcessWindow.WindowTitle),
                    nameof(ProcessWindow.Position)
                });
            var selectorResult = selector.Prompt();
            if (selectorResult.Result == DialogResult.Cancel)
                return;
            processWindows = selectorResult.SelectedItems;

            RemoveAllAppLayoutSectionsFor(layoutName);
            AddAppLayoutSectionsFor(layoutName, processWindows);
            _config.Persist();
        }

        private void AddAppLayoutSectionsFor(string layoutName, ProcessWindow[] processWindows)
        {
            processWindows.ForEach(
                w =>
                {
                    var proc = w?.Process?.MainModule?.FileName;
                    if (proc == null)
                        return;
                    var procFileName = Path.GetFileName(proc);
                    var sectionName = MakeAppLayoutSectionNameFor(
                        layoutName,
                        procFileName
                    );
                    _config.AddSection(sectionName);
                    var pos = w?.Position;
                    if (string.IsNullOrWhiteSpace(proc) || pos == null)
                        return;
                    var section = _config[sectionName];
                    section["position"] = pos.ToString();
                    section["title"] = w.WindowTitle;
                    section["executable"] = procFileName;
                });
        }

        private string MakeAppLayoutSectionNameFor(
            string layoutName,
            string appName
        )
        {
            var allSections = new HashSet<string>(
                _config.Sections.ToArray(),
                new CaseInsensitiveStringComparer()
            );
            var idx = 1;
            while (true)
            {
                var attempt = $"{MakeAppLayoutSectionName(layoutName, appName)} #{idx++}";
                if (!allSections.Contains(attempt))
                    return attempt;
            }
        }

        private static string MakeAppLayoutSectionName(string layoutName, string appName)
        {
            return $"{Constants.APP_LAYOUT_SECTION_PREFIX}{layoutName} : {appName}";
        }

        private void RemoveAllAppLayoutSectionsFor(
            string layoutName
        )
        {
            var search = MakeAppLayoutSectionName(layoutName, "");
            _config.Sections
                .Where(s => s.StartsWith(search))
                .ToArray()
                .ForEach(_config.RemoveSection);
        }
    }

    internal class UserInput: IUserInput
    {
        public IPromptResult Prompt(string caption, string message)
        {
            var prompt = new PromptForm(caption, message);
            return prompt.Prompt();
        }
    }

    internal interface IUserInput
    {
        IPromptResult Prompt(string caption, string message);
    }
}