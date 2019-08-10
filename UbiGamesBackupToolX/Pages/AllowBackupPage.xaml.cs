using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UbiGamesBackupToolX.Bean;

namespace UbiGamesBackupToolX.Pages
{
    /// <summary>
    /// AllowBackupPage.xaml 的交互逻辑
    /// </summary>
    public partial class AllowBackupPage : Page
    {
        public SettingPage settingPage;
        public AllowBackupPage()
        {
            InitializeComponent();
        }


        private void AllowBackupSettingCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (AllowBackupCkb.IsChecked??false)
            {
                settingPage.mainWindow.SetShellHook();
                Config.Instance.AllowBackup = true;
            }
            else
            {
                Config.Instance.AllowBackup = false;
                settingPage.mainWindow.UnSetShellHook();
            }
            AllowBackupPathTextBox.IsEnabled = Config.Instance.AllowBackup;
            AllowBackupPathChooseBtn.IsEnabled = Config.Instance.AllowBackup;
            AddListenerGameBtn.IsEnabled = Config.Instance.AllowBackup;
            RunGameTipStatusCkb.IsEnabled = Config.Instance.AllowBackup;
            ExitGameTipStatusCkb.IsEnabled = Config.Instance.AllowBackup;
            Config.Instance.Save();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AllowBackupCkb.IsChecked = Config.Instance.AllowBackup;
            if (File.Exists(Config.Instance.AllowBackupPath))
            {
                AllowBackupPathTextBox.Text = Config.Instance.AllowBackupPath;
            }
            else
            {
                if (!File.Exists(Config.Instance.DefaultAllowBackup))
                {
                    try
                    {
                        Directory.CreateDirectory(Config.Instance.DefaultAllowBackup);
                    }
                    catch(Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
                AllowBackupPathTextBox.Text = Config.Instance.DefaultAllowBackup;
                Config.Instance.AllowBackupPath = Config.Instance.DefaultAllowBackup;
                Config.Instance.Save();
            }
            RunGameTipStatusCkb.IsChecked = Config.Instance.RunGameTipStatus;
            ExitGameTipStatusCkb.IsChecked = Config.Instance.ExitGameTipStatus;

            AllowBackupPathTextBox.IsEnabled = Config.Instance.AllowBackup;
            AllowBackupPathChooseBtn.IsEnabled = Config.Instance.AllowBackup;
            AddListenerGameBtn.IsEnabled = Config.Instance.AllowBackup;
            RunGameTipStatusCkb.IsEnabled = Config.Instance.AllowBackup;
            ExitGameTipStatusCkb.IsEnabled = Config.Instance.AllowBackup;
        }

        private void AllowBackupChoosePathBtn_OnClicked(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AllowBackupPathTextBox.Text = folderBrowserDialog.SelectedPath;
            }
            folderBrowserDialog.Dispose();
        }

        private void AllowBackupPathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Controls.TextBox textbox = (System.Windows.Controls.TextBox)sender;
            if (Directory.Exists(textbox.Text))
            {
                Config.Instance.AllowBackupPath = textbox.Text;
                Config.Instance.Save();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("文件夹不存在，请重新输入!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void AddListenerGameBtn_OnClicked(object sender, RoutedEventArgs e)
        {
            ChooseGameWindow chooseGameWindow = new ChooseGameWindow
            {
                Owner = settingPage.mainWindow
            };
            chooseGameWindow.ShowDialog();
            GC.Collect();
            settingPage.mainWindow.gameRunStatusListener.RefreshGameList();
        }

        private void ExitGameTipStatusCkb_Checked(object sender, RoutedEventArgs e)
        {
            if (ExitGameTipStatusCkb.IsChecked??false)
            {
                Config.Instance.ExitGameTipStatus = true;
            }
            else
            {
                Config.Instance.ExitGameTipStatus = false;
            }
            Config.Instance.Save();
        }
        private void RunGameTipStatusCkb_Checked(object sender, RoutedEventArgs e)
        {
            if (RunGameTipStatusCkb.IsChecked??false)
            {
                Config.Instance.RunGameTipStatus = true;
            }
            else
            {
                Config.Instance.RunGameTipStatus = false;
            }
            Config.Instance.Save();
        }
    }
}
