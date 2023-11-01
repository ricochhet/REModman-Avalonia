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
        private readonly Window mainWindow;
        private readonly ModSettingsDialog dialogWindow;
        public TaskCompletionSource<bool> Confirmed = new();

        public ModSettings(string title, GameType selectedGameType, string selectedGamePath, ModItem item, Window window)
        {
            dialogWindow = new ModSettingsDialog(selectedGameType, selectedGamePath, item)
            {
                Title = title
            };
            dialogWindow.Title = title;
            dialogWindow.Confirm_Button.Click += OnClick;
            dialogWindow.Cancel_Button.Click += OnClick;
            dialogWindow.Closed += Close;
            mainWindow = window;
        }

        public void Show()
        {
            LogBase.Info($"Opening dialog box: {dialogWindow.Title}");
            mainWindow.IsEnabled = false;
            dialogWindow.Show();
        }

        private void Close(object? sender, EventArgs e) {
            mainWindow.IsEnabled = true;
            dialogWindow.Close();
        }

        private void OnClick(object? sender, EventArgs e)
        {
            if (sender == dialogWindow.Confirm_Button)
            {
                Confirmed.SetResult(true);
            }
            else
            {
                Confirmed.SetResult(false);
            }

            LogBase.Info($"Closing dialog box: {dialogWindow.Title}");
            mainWindow.IsEnabled = true;
            dialogWindow.Close();
        }
    }
}