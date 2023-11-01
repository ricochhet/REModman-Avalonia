using Avalonia.Controls;
using Avalonia.Interactivity;
using REMod.Core.Logger;
using REMod.Views;

namespace REMod
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ILogger logger = new NativeLogger();
            LogBase.Add(logger);
            LogBase.Info("REMod stdout has been initialized.");

            InitializeComponent();
            mainContentControl.Content = new CollectionPage(this);
        }
    }
}