﻿using GitCommands;
using GitUI.CommandsDialogs.BrowseDialog;
using ResourceManager.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GitUI.Hotkey;

namespace GitUI.CommandsDialogs
{
    class FormBrowseNavigateCommands : ITranslate
    {
        private readonly TranslationString _noRevisionFoundError =
            new TranslationString("No revision found.");

        GitUICommands UICommands;
        GitModule Module;
        RevisionGrid RevisionGrid;
        FormBrowse _formBrowse;

        public FormBrowseNavigateCommands(FormBrowse formBrowse, GitUICommands uiCommands, GitModule module, RevisionGrid revisionGrid)
        {
            _formBrowse = formBrowse;
            UICommands = uiCommands;
            Module = module;
            RevisionGrid = revisionGrid;
        }

        public void Translate()
        {
            Translator.Translate(this, Settings.CurrentTranslation);
        }

        public virtual void AddTranslationItems(Translation translation)
        {
            TranslationUtl.AddTranslationItemsFromFields("FormBrowse", this, translation);
        }

        public virtual void TranslateItems(Translation translation)
        {
            TranslationUtl.TranslateItemsFromFields("FormBrowse", this, translation);
        }

        public void SelectCurrentRevisionExecute()
        {
            _formBrowse.ExecuteCommand(GitUI.CommandsDialogs.FormBrowse.Commands.SelectCurrentRevision);
        }

        public void GotoCommitExcecute()
        {
            using (FormGoToCommit formGoToCommit = new FormGoToCommit(UICommands))
            {
                if (formGoToCommit.ShowDialog(_formBrowse) != DialogResult.OK)
                    return;

                string revisionGuid = formGoToCommit.GetRevision();
                if (!string.IsNullOrEmpty(revisionGuid))
                {
                    RevisionGrid.SetSelectedRevision(new GitRevision(Module, revisionGuid));
                }
                else
                {
                    MessageBox.Show(_formBrowse, _noRevisionFoundError.Text);
                }
            }
        }

        public IEnumerable<MenuCommand> GetMenuCommands()
        {
            var resultList = new List<MenuCommand>();

            {
                var menuCommand = new MenuCommand();
                menuCommand.Name = "GotoCurrentRevision";
                menuCommand.Text = "Go to current revision";
                //// menuCommand.Image = global::GitUI.Properties.Resources.IconGotoCommit;
                menuCommand.ShortcutKeyDisplayString = _formBrowse.GetShortcutKeys(GitUI.CommandsDialogs.FormBrowse.Commands.SelectCurrentRevision).ToShortcutKeyDisplayString();
                menuCommand.ExecuteAction = SelectCurrentRevisionExecute;

                resultList.Add(menuCommand);
            }

            {
                var menuCommand = new MenuCommand();
                menuCommand.Name = "GotoCommit";
                menuCommand.Text = "Go to commit...";
                menuCommand.Image = global::GitUI.Properties.Resources.IconGotoCommit;
                menuCommand.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.G)));
                menuCommand.ExecuteAction = GotoCommitExcecute;

                resultList.Add(menuCommand);
            }

            return resultList;
        }
    }
}
