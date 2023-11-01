using REMod.Core.Logger;
using REMod.Core.Configuration.Enums;
using System;
using System.Threading.Tasks;
using System.Windows;
using Avalonia.Controls;
using Avalonia;
using REMod.Models;

namespace REMod.Dialogs
{
    public class ModSettings
    {
        private readonly Window m_Window;
        private readonly ModSettingsDialog m_Dialog;
        public TaskCompletionSource<bool> Confirmed = new();

        public ModSettings(
            string title,
            GameType selectedGameType,
            string selectedGamePath,
            ModItem item,
            Window window
        )
        {
            m_Dialog = new ModSettingsDialog(
                selectedGameType,
                selectedGamePath,
                item
            )
            {
                Title = title
            };
            m_Dialog.Title = title;
            m_Dialog.Confirm_Button.Click += OnClick;
            m_Dialog.Cancel_Button.Click += OnClick;
            m_Dialog.Closed += Close;
            m_Window = window;
        }

        public void Show()
        {
            LogBase.Info($"Opening dialog box: {m_Dialog.Title}");
            m_Window.IsEnabled = false;
            m_Dialog.Show();
        }

        private void Close(object? sender, EventArgs e)
        {
            m_Window.IsEnabled = true;
            m_Dialog.Close();
        }

        private void OnClick(object? sender, EventArgs e)
        {
            if (sender == m_Dialog.Confirm_Button)
            {
                Confirmed.SetResult(true);
            }
            else
            {
                Confirmed.SetResult(false);
            }

            LogBase.Info($"Closing dialog box: {m_Dialog.Title}");
            m_Window.IsEnabled = true;
            m_Dialog.Close();
        }
    }
}
