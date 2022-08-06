using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using UbiGamesBackupToolX.Bean;
using UbiGamesBackupToolX.Utils;
using MessageBox = System.Windows.MessageBox;

namespace UbiGamesBackupToolX.Pages
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {

        private enum ToolMode
        {
            Backup, Restore
        }
        ToolMode toolMode = ToolMode.Backup;
        List<string> SelectGameList = new List<string>();
        public UserInfo SelectUser { get; set; } = null;

        private List<Game> GamePanelItemList;

        public MainWindow mainWindow;

        public MainPage()
        {
            InitializeComponent();
        }

        private void BackReBtn_Click(object sender, RoutedEventArgs e)
        {
            if (toolMode == ToolMode.Restore)
            {
                if (SelectGameList.Count > 0)
                {
                    if (MessageBoxResult.OK == System.Windows.MessageBox.Show("确定要还原这些游戏？", "确定", MessageBoxButton.OKCancel, MessageBoxImage.Question))
                    {
                        String ReStorePath = UbiFileUtils.UPLAYSAVEGAME + System.IO.Path.DirectorySeparatorChar + SelectUser.UID;

                        foreach (string GID in SelectGameList)
                        {
                            UbiFileUtils.CopyDirectory(SelectUser.USERSAVEGAME + System.IO.Path.DirectorySeparatorChar + GID, ReStorePath);
                        }
                        System.Windows.MessageBox.Show("还原完成！");
                        GC.Collect();
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("还未选择存档！");
                }
            }
            else
            {
                if (SelectGameList.Count > 0)
                {
                    if (MessageBoxResult.OK == System.Windows.MessageBox.Show("确定要备份这些游戏？", "确定", MessageBoxButton.OKCancel, MessageBoxImage.Question))
                    {
                        FolderBrowserDialog folderBrowserDialogBackupTo = new FolderBrowserDialog();
                        if (folderBrowserDialogBackupTo.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            SelectUser.BackupTime = string.Format("{0:yyyy-MM-dd HH.mm.ss}", DateTime.Now);
                            String backupPath = folderBrowserDialogBackupTo.SelectedPath + System.IO.Path.DirectorySeparatorChar + SelectUser.UID + " " + SelectUser.BackupTime;

                            foreach (string path in SelectGameList)
                            {
                               UbiFileUtils.CopyDirectory(UbiFileUtils.UPLAYSAVEGAME + System.IO.Path.DirectorySeparatorChar + SelectUser.UID + System.IO.Path.DirectorySeparatorChar + path, backupPath);
                            }
                            UbiFileUtils.OutBackupInfo(backupPath,SelectUser);
                            System.Windows.MessageBox.Show("备份完成！");
                            GC.Collect();
                        }
                        folderBrowserDialogBackupTo.Dispose();
                    }

                }
                else
                {
                    System.Windows.MessageBox.Show("还未选择游戏！");
                }
            }
        }


        

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.showSettingPage();
        }

        
        

        /// <summary>
        /// 切换备份或还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeModeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.toolMode == ToolMode.Backup)
            {
                FolderBrowserDialog folderBrowserDialogBackupTo = new FolderBrowserDialog();
                if (Config.Instance.AllowBackup)
                {
                    folderBrowserDialogBackupTo.SelectedPath = Config.Instance.AllowBackupPath;
                }
                System.Windows.MessageBox.Show("请选择备份的位置以开始还原");
                if (folderBrowserDialogBackupTo.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //button3.Visible = false;
                    //button5.Visible = true;
                    toolMode = ToolMode.Restore;
                    try
                    {
                        InitUserList(folderBrowserDialogBackupTo.SelectedPath);
                        InitGameListPanel();
                        toolstatus.Content = "由 " + SelectUser.UID + " 备份于" + SelectUser.BackupTime;
                        BackReBtn.Background = (ImageBrush)FindResource("RestoreImageBrush");
                        BackReBtn.ToolTip = "还原";
                    }
                    catch (Exception)
                    {

                        toolMode = ToolMode.Backup;
                        //button3.Visible = true;
                        //button5.Visible = false;
                        //throw;
                        InitUserList(UbiFileUtils.UPLAYSAVEGAME);
                        InitGameListPanel();
                        toolstatus.Content = "选择将要备份的游戏存档";
                        BackReBtn.Background = (ImageBrush)FindResource("BackupImageBrush");
                        System.Windows.MessageBox.Show("这个路径里并没有备份好的存档！回到备份模式");
                        BackReBtn.ToolTip = "备份";
                    }

                }
                folderBrowserDialogBackupTo.Dispose();
            }
            else
            {
                //button3.Visible = true;
                //button5.Visible = false;
                toolMode = ToolMode.Backup;

                InitUserList(UbiFileUtils.UPLAYSAVEGAME);
                InitGameListPanel();
                toolstatus.Content = "选择将要备份的游戏存档";
                BackReBtn.Background = (ImageBrush)FindResource("BackupImageBrush");
                BackReBtn.ToolTip = "备份";
            }
        }

        public void ALLClick()
        {
            foreach(Game game in GamePanelItemList)
            {
                game.selected = !game.selected;
                if (game.selected)
                {
                    if(SelectGameList.Contains(game.id))
                    {
                        SelectGameList.Remove(game.id);
                    }
                    else
                    {
                        SelectGameList.Add(game.id);
                    }
                }
            }
        }

        private void ALLPanelSelect_Click(object sender, RoutedEventArgs e)
        {
            ALLClick();
        }

        public void InitUserList(string path)
        {
            if (path == null) {
                MessageBox.Show("找不到Uplay安装位置，请确保已安装Uplay!");
                return;
            }
            List<UserInfo> userlist = null;
            SelectGameList = new List<string>();
            switch (toolMode)
            {
                case ToolMode.Backup:
                    userlist = UbiFileUtils.GetBackupAllUserInfo();
                    break;
                case ToolMode.Restore:
                    userlist = UbiFileUtils.GetReStoreUserList(path);
                    break;
            }
            this.UserListView.ItemsSource = userlist;
            SelectUser = userlist[0];
            userlist[0].selected = true;
            InitGameListPanel();
        }

        /// <summary>
        /// 初始化游戏列表
        /// </summary>
        public void InitGameListPanel()
        {
            GamePanelItemList = UbiFileUtils.CheckGameSaveDirectory(SelectUser);
            this.GameListView.ItemsSource = GamePanelItemList;
            GC.Collect();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitUserList(UbiFileUtils.UPLAYSAVEGAME);
        }

        private void GamePanelMouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                WrapPanel gamepanel = (WrapPanel)sender;
                string gid = gamepanel.Tag.ToString();
                IEnumerable<Game> itemCollection = (IEnumerable<Game>)GameListView.Items.SourceCollection;
                itemCollection = from game in itemCollection where game.id.Equals(gid) select game;
                Game selectgame = itemCollection.First();
                selectgame.selected = !selectgame.selected;
                if (SelectGameList.Contains(gid))
                {
                    SelectGameList.Remove(gid);
                }
                else
                {
                    SelectGameList.Add(gid);
                }
            }
        }

        private void UserImgPanelMouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            UserInfo userInfo = (UserInfo)((Ellipse)sender).Tag;
            if (SelectUser != userInfo)
            {
                SelectUser.selected = false;
                SelectUser = userInfo;
                SelectUser.selected = true;
                SelectGameList = new List<string>();
                if (toolMode == ToolMode.Restore)
                {
                    toolstatus.Content = "由 " + SelectUser.UID + " 备份于" + SelectUser.BackupTime;
                }
                InitGameListPanel();
                GC.Collect();
            }
        }
    }
}
