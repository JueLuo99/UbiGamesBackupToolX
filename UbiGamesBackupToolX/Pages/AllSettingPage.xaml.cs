using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using UbiGamesBackupToolX.Utils;

namespace UbiGamesBackupToolX.Pages
{
    /// <summary>
    /// AllSettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class AllSettingPage : Page
    {
        public SettingPage settingPage;
        public AllSettingPage()
        {
            InitializeComponent();
        }

        private void ShortcutButton_Clicked(object sender, RoutedEventArgs e)
        {
            ShortcutCreator.CreateShortcutOnDesktop("UbiGamesBackupTool", this.GetType().Assembly.Location);
        }

        private void powerCkb_Checked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox checkBox = sender as System.Windows.Controls.CheckBox;
            if (checkBox.IsChecked == true)
            {
                try
                {
                    // 添加到 当前登陆用户的 注册表启动项
                    string startuppath = Process.GetCurrentProcess().MainModule.FileName;
                    RegistryKey RKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl);
                    if (RKey == null)
                    {
                        RKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    }
                    RKey.SetValue("UbiGamesBackupToolX", "\"" + startuppath + "\" " + "-b");
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            else if (checkBox.IsChecked == false)
            {
                try
                {
                    RegistryKey RKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl);
                    if (RKey == null)
                    {
                        RKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    }
                    RKey.DeleteValue("UbiGamesBackupToolX");
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
        }
    }
}
