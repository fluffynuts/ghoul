﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ghoul.AppLogic.Events;
using Ghoul.Native;
using Ghoul.Ui;
using Ghoul.Utils;
using PeanutButter.INIFile;
using PeanutButter.TinyEventAggregator;
using PeanutButter.Utils;
using Keys = Ghoul.Utils.Constants.Keys;

namespace Ghoul.AppLogic
{
    public interface ILayoutSaver
    {
        void SaveCurrentLayout();
    }

    internal class LayoutSaver
        : ILayoutSaver
    {
        private readonly IINIFile _config;
        private readonly IUserInput _userInput;
        private readonly ISectionNameHelper _sectionNameHelper;
        private readonly IDesktopWindowUtil _desktopWindowUtil;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICheckListDialogFactory _dialogFactory;

        public LayoutSaver(
            IINIFile config,
            IUserInput userInput,
            ISectionNameHelper sectionNameHelper,
            IDesktopWindowUtil desktopWindowUtil,
            IEventAggregator eventAggregator,
            ICheckListDialogFactory dialogFactory
        )
        {
            _config = config;
            _userInput = userInput;
            _sectionNameHelper = sectionNameHelper;
            _desktopWindowUtil = desktopWindowUtil;
            _eventAggregator = eventAggregator;
            _dialogFactory = dialogFactory;
        }

        public void SaveCurrentLayout()
        {
            _eventAggregator.GetEvent<LayoutSaveStartedEvent>().Publish(true);
            try
            {
                var layoutName = _userInput.Prompt(
                    "Enter name for layout",
                    "Please enter a name for the layout to be saved",
                    _config.EnumerateLayouts().ToArray()).UserInput;
                if (string.IsNullOrWhiteSpace(layoutName))
                    return;

                SaveCurrentToConfig(layoutName);
                _eventAggregator
                    .GetEvent<LayoutAddedEvent>()
                    .Publish(layoutName);
            }
            finally
            {
                _eventAggregator.GetEvent<LayoutSaveCompletedEvent>().Publish(true);
            }
        }

        private void SaveCurrentToConfig(string layoutName)
        {
            // TODO: better input of layout names
            // TODO: verify with user when re-using a layout name: it will overwrite the existing settings

            var processWindows = _desktopWindowUtil.ListWindows();
            var selector = _dialogFactory.Create(
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
                    var proc = w?.Process?.MainModule.FileName;
                    if (proc == null)
                        return;
                    var procFileName = Path.GetFileName(proc);
                    var sectionName = MakeAppLayoutSectionNameFor(
                        layoutName,
                        procFileName
                    );
                    _config.AddSection(sectionName);
                    var pos = w.Position;
                    if (string.IsNullOrWhiteSpace(proc) || pos == null)
                        return;
                    var section = _config[sectionName];
                    section[Keys.POSITION] = pos.ToString();
                    section[Keys.TITLE] = w.WindowTitle;
                    section[Keys.EXECUTABLE] = proc;
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
            var prefix = _sectionNameHelper.CreateAppLayoutSectionNameFor(layoutName, appName);
            while (true)
            {
                var attempt = $"{prefix} #{idx++}";
                if (!allSections.Contains(attempt))
                    return attempt;
            }
        }

        private void RemoveAllAppLayoutSectionsFor(
            string layoutName
        )
        {
            var search = _sectionNameHelper.CreateAppLayoutSectionNameFor(layoutName, "");
            _config.Sections
                .Where(s => s.StartsWith(search))
                .ToArray()
                // FIXME: remove this when RemoveSection is in the IINIFile interface
                .ForEach(_config.RemoveSection);
        }
    }
}