using REMod.Core.Logger;
using REMod.Core.Configuration.Enums;
using System;
using System.Threading.Tasks;
using System.Windows;
using Avalonia.Controls;
using Avalonia;

namespace REMod.Dialogs
{
    public class Settings
    {
        private readonly Window m_Window;
        private readonly SettingsDialog m_Dialog;
        public TaskCompletionSource<bool> Confirmed = new();

        public Settings(string title, Window window)
        {
            m_Dialog = new SettingsDialog()
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
            m_Dialog.Close();
        }
    }
}