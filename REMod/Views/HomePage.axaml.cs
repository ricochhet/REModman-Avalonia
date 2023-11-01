using Avalonia.Controls;
using Avalonia.Interactivity;

namespace REMod.Views
{
    public partial class HomePage : UserControl
    {
        private Window m_Window;
        public HomePage() {
            InitializeComponent();
        }

        public HomePage(Window window) {
            InitializeComponent();
            m_Window = window;
        }

        private void GotoCollectionPage_Click(object sender, RoutedEventArgs e)
        {
            m_Window.Content = new CollectionPage();
        }
    }
}