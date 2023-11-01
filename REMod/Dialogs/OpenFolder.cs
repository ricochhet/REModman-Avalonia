using REMod.Core.Logger;
using REMod.Core.Configuration.Enums;
using System;
using System.Threading.Tasks;
using System.Windows;
using Avalonia.Controls;
using Avalonia;

namespace REMod.Dialogs
{
    public class OpenFolder
    {
        private readonly UserControl mainWindow;
        private readonly OpenFolderDialog dialogWindow;
        public TaskCompletionSource<bool> Confirmed = new();

        public OpenFolder(string title, GameType selectedGameType, string selectedGamePath, UserControl window)
        {
            dialogWindow = new OpenFolderDialog(selectedGameType, selectedGamePath)
            {
                Title = title
            };
            dialogWindow.Title = title;
            dialogWindow.OpenModFolder_CardAction.Click += OnClick;
            dialogWindow.OpenDownloadFolder_CardAction.Click += OnClick;
            dialogWindow.OpenGameFolder_CardAction.Click += OnClick;
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