using REMod.Core.Configuration.Enums;
using REMod.Core.Providers;
using REMod.Core.Resolvers;
using REMod.Core.Resolvers.Enums;
using REMod.Core.Utils;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Avalonia.Controls;
using Avalonia.Interactivity;
using REMod.Models;
using REMod.Core.Manager;
using System;
using REMod.Core.Plugins;

namespace REMod.Dialogs
{
    public partial class ModSettingsDialog : Window
    {
        private static GameType selectedGameType = GameType.None;
        private static string selectedGamePath = string.Empty;
        private static ModItem item = new();

        public ModSettingsDialog() 
        {
            InitializeComponent();
        }

        public ModSettingsDialog(GameType type, string gamePath, ModItem mod)
        {
            selectedGameType = type;
            selectedGamePath = gamePath;
            item = mod;

            InitializeComponent();
        }

        private void PatchMod_Button_Initialized(object sender, EventArgs e)
        {
            if (sender is not Button button)
                return;

            if (selectedGameType != GameType.None)
            {
                if (!RisePakPatchExtension.IsPatchable(selectedGameType, item.Hash))
                {
                    button.IsEnabled = false;
                }
                else
                {
                    button.IsEnabled = true;
                }
            }
        }

        private async void PatchMod_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button)
                return;

            if (selectedGameType != GameType.None)
            {
                if (RisePakPatchExtension.IsPatchable(selectedGameType, item.Hash))
                {
                    BaseDialog confirmDialog = new("Mod Manager", $"{StringHelper.Truncate(item.Name, 38)} can be converted to a PAK mod, proceed?", this);
                    confirmDialog.Show();

                    if (await confirmDialog.Confirmed.Task)
                    {
                        RisePakPatchExtension.Patch(selectedGameType, item.Hash);
                    }
                }
            }
        }

        private void LoadOrder_NumberBox_Initialized(object sender, EventArgs e)
        {
            if (sender is not NumericUpDown numberBox)
                return;

            if (selectedGameType != GameType.None)
            {
                numberBox.Value = ModInstaller.GetLoadOrder(selectedGameType, item.Hash);
            }
        }

        private void LoadOrder_NumberBox_ValueChanged(object sender, NumericUpDownValueChangedEventArgs e)
        {
            if (sender is not NumericUpDown numberBox)
                return;

            if (selectedGameType != GameType.None)
            {
                if (numberBox.Value != null)
                {
                    ModInstaller.SetLoadOrder(selectedGameType, item.Hash, (int)numberBox.Value);
                }
            }
        }

        private async void DeleteMod_Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button)
                return;

            if (selectedGameType != GameType.None)
            {
                if (Directory.Exists(selectedGamePath))
                {
                    BaseDialog confirmDialog = new("Mod Manager", $"Do you want to delete mod {StringHelper.Truncate(item.Name, 38)} for {ManagerSettings.GetLastSelectedGame()}?", this);
                    confirmDialog.Show();

                    if (await confirmDialog.Confirmed.Task)
                    {
                        ModInstaller.Delete(selectedGameType, item.Hash);
                    }
                }
                else
                {
                    BaseDialog dialog = new("Mod Manager", $"{selectedGameType} has not been correctly configured.", this);
                    dialog.Show();
                }
            }
        }
    }
}