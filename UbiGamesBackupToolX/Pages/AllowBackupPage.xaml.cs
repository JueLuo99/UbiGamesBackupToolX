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
            System.Windows.Controls.CheckBox checkBox = sender as System.Windows.Controls.CheckBox;
            if (checkBox.IsChecked == true)
            {
                AllowBackupPathTextBox.IsEnabled = true;
                AllowBackupPathChooseBtn.IsEnabled = true;
                AddListenerGameBtn.IsEnabled = true;
                RunGameTipStatusCkb.IsEnabled = true;
                ExitGameTipStatusCkb.IsEnabled = true;

                //gameStatusListener.restart();
                //TODO 重启监听
            }
            else
            {
                AllowBackupPathTextBox.IsEnabled = false;
                AllowBackupPathChooseBtn.IsEnabled = false;
                AddListenerGameBtn.IsEnabled = false;
                RunGameTipStatusCkb.IsEnabled = false;
                ExitGameTipStatusCkb.IsEnabled = false;

                //gameStatusListener.stop();
                //TODO 停止监听
            }
            Config.Instance.AllowBackup = checkBox.IsChecked.Value;
            Config.Instance.Save();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AllowBackupCkb.IsChecked = Config.Instance.AllowBackup;
            AllowBackupPathTextBox.Text = Config.Instance.AllowBackupPath;
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
            //gameStatusListener.restart();
            //TODO 此处刷新监听列表
        }

        private void ExitGameTipStatusCkb_Checked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox checkBox = sender as System.Windows.Controls.CheckBox;
            if (checkBox.IsChecked == true)
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
            System.Windows.Controls.CheckBox checkBox = sender as System.Windows.Controls.CheckBox;
            if (checkBox.IsChecked == true)
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
