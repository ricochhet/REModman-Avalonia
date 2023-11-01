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
using REMod.Core.Manager;
using REMod.Core.Logger;

namespace REMod.Dialogs
{
    public partial class SettingsDialog : Window
    {
        public SettingsDialog() 
        {
            InitializeComponent();
        }

        private async void DeleteData_CardAction_Click(object sender, RoutedEventArgs e)
        {
            BaseDialog confirmDialog = new("Mod Manager", $"This action is irreversible, are you sure?", this);
            confirmDialog.Show();

            if (await confirmDialog.Confirmed.Task)
            {
                DataProvider.Delete(FolderType.Data, ManagerSettings.GetLastSelectedGame());
            }
        }
    }
}