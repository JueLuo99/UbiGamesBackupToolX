using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UbiGamesBackupToolX.Pages
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();
            aboutPage = new AboutPage()
            {
                settingPage = this
            };
            allowBackupPage = new AllowBackupPage()
            {
                settingPage = this
            };
            allSettingPage = new AllSettingPage()
            {
                settingPage = this
            };
        }

        public MainWindow mainWindow;

        AboutPage aboutPage;
        AllowBackupPage allowBackupPage;
        AllSettingPage allSettingPage;

        private void Settingokbtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.PageBox.GoBack();

        }

        private void AllSetting_Click(object sender, RoutedEventArgs e)
        {
            this.SettingContent.Content = allSettingPage;
        }

        private void AllowBackup_Click(object sender, RoutedEventArgs e)
        {
            this.SettingContent.Content = allowBackupPage;
        }
        private void BackupSetting_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            this.SettingContent.Content = aboutPage;
        }
    }
}
