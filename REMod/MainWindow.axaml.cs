using REMod.Core.Configuration.Enums;
using REMod.Core.Configuration.Structs;
using REMod.Core.Manager;
using REMod.Core.Plugins;
using REMod.Core.Providers;
using REMod.Core.Resolvers.Enums;
using REMod.Core.Utils;
using REMod.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Avalonia.Controls;
using Avalonia.Interactivity;
using REMod.Dialogs;
using REMod.Core.Logger;
using Avalonia.Logging;

namespace REMod
{
    public partial class MainWindow : Window
    {
        private GameType selectedGameType = GameType.None;
        private string selectedGamePath = string.Empty;

        public ObservableCollection<ModItem> ModCollection = new();

        public MainWindow()
        {
            if (!DataProvider.Exists(FileType.Settings))
            {
                DataProvider.Create(FileType.Settings);
            }

            ILogger logger = new NativeLogger();
            LogBase.Add(logger);
            LogBase.Info("REMod stdout has been initialized.");

            InitializeComponent();
        }

        private ItemsControl? itemsControl;
        private void PopulateItemsControl() 
        {
            if (itemsControl == null)
                return;

            ModCollection.Clear();

            if (selectedGameType != GameType.None)
            {
                if (DataProvider.Exists(FolderType.Data, selectedGameType) && DataProvider.Exists(FolderType.Mods, selectedGameType))
                {
                    List<ModData> index = ManagerCache.Build(selectedGameType);
                    ManagerCache.SaveHashChanges(selectedGameType, index);

                    foreach (ModData mod in index)
                    {
                        ModCollection.Add(new ModItem
                        {
                            Name = mod.Name,
                            Hash = mod.Hash,
                            IsEnabled = mod.IsEnabled,
                        });
                    }

                    itemsControl.ItemsSource = ModCollection;
                }
                else
                {
                    BaseDialog dialog = new("Configuration Error", $"{ManagerSettings.GetLastSelectedGame()} has not been correctly configured.", this);
                    dialog.Show();
                }
            }
        }

        private void CheckSelectedGameState()
        {
            if (selectedGameType != GameType.None)
            {
                if (!DataProvider.Exists(FolderType.Mods, selectedGameType))
                {
                    DataProvider.Create(FolderType.Mods, selectedGameType);
                }

                if (!DataProvider.Exists(FolderType.Downloads, selectedGameType))
                {
                    DataProvider.Create(FolderType.Downloads, selectedGameType);
                }

                if (!DataProvider.Exists(FileType.Cache, selectedGameType))
                {
                    DataProvider.Create(FileType.Cache, selectedGameType);
                }
            }
        }

        private Grid? toolBar_Grid_Grid;
        private void ToolBar_Grid_Visibility()
        {
            if (toolBar_Grid_Grid == null)
                return;


            if (selectedGameType != GameType.None)
            {
                if (selectedGamePath != "")
                {
                    toolBar_Grid_Grid.IsEnabled = true;
                    toolBar_Grid_Grid.IsVisible = true;
                }
                else
                {
                    toolBar_Grid_Grid.IsEnabled = false;
                    toolBar_Grid_Grid.IsVisible = false;
                }
            }
            else
            {
                ToolBar_Grid.IsEnabled = false;
                ToolBar_Grid.IsVisible = false;
            }
        }

        private Button? setupGame_CardAction_Button;
        private void SetupGame_CardAction_Visibility()
        {
            if (setupGame_CardAction_Button == null)
                return;

            if (selectedGameType != GameType.None)
            {
                if (selectedGamePath != "")
                {
                    setupGame_CardAction_Button.IsEnabled = false;
                    setupGame_CardAction_Button.IsVisible = false;
                }
                else
                {
                    setupGame_CardAction_Button.IsEnabled = true;
                    setupGame_CardAction_Button.IsVisible = true;
                }
            }
            else
            {
                setupGame_CardAction_Button.IsEnabled = false;
                setupGame_CardAction_Button.IsVisible = false;
            }
        }

        private void ModsItemsControl_Initialized(object sender, EventArgs e)
        {
            if (sender is ItemsControl ic) {
                itemsControl = ic;
                if (selectedGameType != GameType.None)
                {
                    PopulateItemsControl();
                }
            }
        }

        private void GameSelector_ComboBox_Initialize(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox) {
                comboBox.Items.Clear();
                comboBox.ItemsSource = Enum.GetValues(typeof(GameType));
                comboBox.SelectedIndex = (int)ManagerSettings.GetLastSelectedGame();
                selectedGameType = ManagerSettings.GetLastSelectedGame();
                selectedGamePath = ManagerSettings.GetGamePath(selectedGameType);

                CheckSelectedGameState();
            }
        }

        private void ToolBar_Grid_Initialize(object sender, EventArgs e)
        {
            if (sender is Grid grid) {
                toolBar_Grid_Grid = grid;
                ToolBar_Grid_Visibility();
            }
        }

        private void GameSelector_ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (GameSelector_ComboBox.SelectedItem != null)
            {
                selectedGameType = (GameType)Enum.Parse(typeof(GameType), GameSelector_ComboBox.SelectedItem.ToString() ?? GameType.None.ToString());
                ManagerSettings.SaveLastSelectedGame(selectedGameType);

                GameSelector_ComboBox.SelectedIndex = (int)ManagerSettings.GetLastSelectedGame();
                selectedGamePath = ManagerSettings.GetGamePath(selectedGameType);

                ToolBar_Grid_Visibility();
                SetupGame_CardAction_Visibility();
                CheckSelectedGameState();
                PopulateItemsControl();
            }
        }

        private void OpenFolder_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                OpenFolder confirmDialog = new("Open Folder", selectedGameType, selectedGamePath, this);
                confirmDialog.Show();
            }
        }

        private void Refresh_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                ToolBar_Grid_Visibility();
                SetupGame_CardAction_Visibility();
                CheckSelectedGameState();
                PopulateItemsControl();
            }
        }

        private void SetupGame_CardAction_Initialized(object sender, EventArgs e)
        {
            if (sender is Button button) {
                setupGame_CardAction_Button = button;
                SetupGame_CardAction_Visibility();
            }
        }

        private void SetupGame_CardAction_Click(object sender, RoutedEventArgs e)
        {
            if (selectedGameType != GameType.None)
            {
                if (!ProcessHelper.IsProcRunning(selectedGameType))
                {
                    BaseDialog dialog = new("Mod Manager", $"{selectedGameType} must be running to start the setup process.", this);
                    dialog.Show();

                    ToolBar_Grid_Visibility();
                    SetupGame_CardAction_Visibility();
                    CheckSelectedGameState();
                    PopulateItemsControl();
                }
                else
                {
                    ManagerSettings.SaveGamePath(selectedGameType);
                    BaseDialog dialog = new("Mod Manager", $"Setup has been completed for {selectedGameType}.", this);
                    dialog.Show();

                    ToolBar_Grid_Visibility();
                    SetupGame_CardAction_Visibility();
                    CheckSelectedGameState();
                    PopulateItemsControl();
                }
            }
        }

        private void EnableMod_ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch? toggle = sender as ToggleSwitch;

            if (toggle?.Tag is ModItem item && selectedGameType != GameType.None)
            {
                if (Directory.Exists(selectedGamePath))
                {
                    ModInstaller.Enable(selectedGameType, item.Hash, true);
                }
                else
                {
                    BaseDialog dialog = new("Mod Manager", $"{selectedGameType} has not been correctly configured.", this);
                    dialog.Show();
                }
            }
        }

        private void EnableMod_ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch? toggle = sender as ToggleSwitch;

            if (toggle?.Tag is ModItem item && selectedGameType != GameType.None)
            {
                if (Directory.Exists(selectedGamePath))
                {
                    ModInstaller.Enable(selectedGameType, item.Hash, false);
                }
                else
                {
                    BaseDialog dialog = new("Mod Manager", $"{selectedGameType} has not been correctly configured.", this);
                    dialog.Show();
                }
            }
        }

        private void PatchMod_Button_Initialized(object sender, EventArgs e)
        {
            Button? button = sender as Button;

            if (button?.Tag is ModItem item && selectedGameType != GameType.None)
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
            Button? button = sender as Button;

            if (button?.Tag is ModItem item && selectedGameType != GameType.None)
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
            NumericUpDown? numberBox = sender as NumericUpDown;

            if (numberBox?.Tag is ModItem item && selectedGameType != GameType.None)
            {
                numberBox.Value = ModInstaller.GetLoadOrder(selectedGameType, item.Hash);
            }
        }

        private void LoadOrder_NumberBox_ValueChanged(object sender, NumericUpDownValueChangedEventArgs e)
        {
            NumericUpDown? numberBox = sender as NumericUpDown;

            if (numberBox?.Tag is ModItem item && selectedGameType != GameType.None)
            {
                if (numberBox.Value != null)
                {
                    ModInstaller.SetLoadOrder(selectedGameType, item.Hash, (int)numberBox.Value);
                }
            }
        }

        private async void DeleteMod_Button_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;

            if (button?.Tag is ModItem item && selectedGameType != GameType.None)
            {
                if (Directory.Exists(selectedGamePath))
                {
                    BaseDialog confirmDialog = new("Mod Manager", $"Do you want to delete mod {StringHelper.Truncate(item.Name, 38)} for {ManagerSettings.GetLastSelectedGame()}?", this);
                    confirmDialog.Show();

                    if (await confirmDialog.Confirmed.Task)
                    {
                        ModInstaller.Delete(selectedGameType, item.Hash);

                        ToolBar_Grid_Visibility();
                        SetupGame_CardAction_Visibility();
                        CheckSelectedGameState();
                        PopulateItemsControl();
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