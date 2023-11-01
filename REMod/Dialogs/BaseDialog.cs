using REMod.Core.Logger;
using System;
using System.Threading.Tasks;
using System.Windows;
using Avalonia.Controls;
using Avalonia;

namespace REMod.Dialogs
{
    public class BaseDialog
    {
        private readonly Window mainWindow;
        private readonly BaseDialogWindow dialogWindow;
        public TaskCompletionSource<bool> Confirmed = new();

        public BaseDialog(string title, string content, Window window)
        {
            dialogWindow = new BaseDialogWindow
            {
                Title = title
            };
            dialogWindow.Title = title;
            dialogWindow.Content_TextBlock.Text = content;
            dialogWindow.Confirm_Button.Click += OnClick;
            dialogWindow.Cancel_Button.Click += OnClick;
            dialogWindow.Closed += Close;
            mainWindow = window;
        }

        public void Show()
        {
            LogBase.Info($"Opening dialog box: {dialogWindow.Title} - {dialogWindow.Content_TextBlock.Text}");
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

            LogBase.Info($"Closing dialog box: {dialogWindow.Title} - {dialogWindow.Content_TextBlock.Text}");
            mainWindow.IsEnabled = true;
            dialogWindow.Close();
        }
    }
}