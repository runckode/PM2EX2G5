using PM2EX2G5.Views;

namespace PM2EX2G5
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
            MainPage = new NavigationPage(new PageSitiosList());
        }
    }
}
