using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UbiGamesBackupToolX.Bean;

namespace UbiGamesBackupToolX.Utils
{
    class UbiFileUtils
    {
        public static List<Game> SupportGameList = GetSupportGame();
        // public static string USERINFOLOCATION = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\AppData\\Local\\Ubisoft Game Launcher\\users.dat";
        public static string USERSETTINGLOCATION = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\AppData\\Local\\Ubisoft Game Launcher\\settings.yml";
        public static string UPLAYINSTALLLOCATION
        {
            get
            {
                string uplayPath = GetUplayPath();
                string[] dp = uplayPath.Split(new char[] { '"', });
                if (dp.Length > 2)
                {
                    return dp[1].Substring(0, uplayPath.LastIndexOf('\\'));
                }
                else
                {
                    return null;
                }

            }
        }
        public static string UPLAYSAVEGAME
        {
            get
            {
                if (UPLAYINSTALLLOCATION == null)
                {
                    return null;
                }
                else
                {
                    return UPLAYINSTALLLOCATION + "savegames";
                }
            }
        }
        public static string USERICONLOCATION
        {
            get
            {
                if (UPLAYINSTALLLOCATION == null)
                {
                    return null;
                }
                else
                {
                    return UPLAYINSTALLLOCATION + "cache\\avatars";
                }
            }
        }
        public static string GAMELOGOCACHE
        {
            get
            {
                if (UPLAYINSTALLLOCATION == null)
                {
                    return null;
                }
                else
                {
                    return UPLAYINSTALLLOCATION + "cache\\assets";
                }
            }
        }

        public const string GAMEICON = "Resources/gameicon/";
        public const string DEFAULTGAMEICON = "../Resources/gamepanelbackground.png";

        /// <summary>
        /// 获取Uplay路径
        /// </summary>
        /// <returns>Uplay路径或是错误提示</returns>
        private static string GetUplayPath()
        {
            try
            {
                string strKeyName = string.Empty;
                string softPath = @"SOFTWARE\Classes\uplay\Shell\Open\Command";
                RegistryKey regKey = Registry.LocalMachine;
                RegistryKey regSubKey = regKey.OpenSubKey(softPath, false);
                if (regSubKey == null) { return ""; }
                object objResult = regSubKey.GetValue(strKeyName);
                RegistryValueKind regValueKind = regSubKey.GetValueKind(strKeyName);
                if (regValueKind == Microsoft.Win32.RegistryValueKind.String)
                {
                    return objResult.ToString();
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 复制存档目录
        /// </summary>
        /// <param name="srcdir">游戏存档源文件夹</param>
        /// <param name="desdir">目标存档文件夹</param>
        public static void CopyDirectory(string srcdir, string desdir)
        {
            string folderName = srcdir.Substring(srcdir.LastIndexOf("\\") + 1);
            string desfolderdir = desdir + "\\" + folderName;
            if (desdir.LastIndexOf("\\") == (desdir.Length - 1))
            {
                desfolderdir = desdir + folderName;
            }
            string[] filenames = Directory.GetFileSystemEntries(srcdir);
            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理 如果存在这个目录就递归Copy该目录下面的文件
                {
                    string currentdir = desfolderdir + "\\" + file.Substring(file.LastIndexOf("\\") + 1);
                    if (!Directory.Exists(currentdir))
                    {
                        Directory.CreateDirectory(currentdir);
                    }
                    CopyDirectory(file, desfolderdir);
                }
                else // 否则直接copy文件
                {
                    string srcfileName = file.Substring(file.LastIndexOf("\\") + 1);
                    srcfileName = desfolderdir + "\\" + srcfileName;
                    if (!Directory.Exists(desfolderdir))
                    {
                        Directory.CreateDirectory(desfolderdir);
                    }
                    File.Copy(file, srcfileName, true);
                }
            }
        }

        /// <summary>
        /// 输出备份信息
        /// </summary>
        /// <param name="path">备份地址</param>
        public static void OutBackupInfo(string path, UserInfo userInfo)
        {
            using (Stream stream = new FileStream(path + System.IO.Path.DirectorySeparatorChar + "userinfo.json", FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(userInfo));
                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// 获取需要还原存档的用户列表
        /// </summary>
        /// <param name="path">备份路径</param>
        /// <returns>用户列表</returns>
        public static List<UserInfo> GetReStoreUserList(string path)
        {
            List<UserInfo> UserList = new List<UserInfo>();
            string[] userpath = Directory.GetDirectories(path);
            foreach (string dir in userpath)
            {
                string userjson = dir + System.IO.Path.DirectorySeparatorChar + "userinfo.json";
                if (File.Exists(userjson))
                {
                    using (StreamReader reader = new StreamReader(new FileStream(userjson, FileMode.Open)))
                    {
                        UserInfo user = JsonConvert.DeserializeObject<UserInfo>(reader.ReadToEnd());
                        user.USERSAVEGAME = dir;
                        if (!File.Exists(user.USERIMAGE) || !File.Exists(UbiFileUtils.USERICONLOCATION + "\\" + user.UID + "_64.png"))
                        {
                            user.USERIMAGE = "../Resources/user.png";
                        }
                        UserList.Add(user);
                    }
                }
                else
                {
                    continue;
                }
            }
            return UserList;
        }

        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <returns>返回所有用户的集合</returns>
        public static List<UserInfo> GetBackupAllUserInfo()
        {
            List<UserInfo> UserList = new List<UserInfo>();
            GetUserIdList().ForEach(userId =>
            {
                string imgpath = USERICONLOCATION + "\\" + userId + "_64.png";
                UserList.Add(new UserInfo()
                {
                    UID = userId,
                    USERIMAGE = File.Exists(imgpath) ? imgpath : "../Resources/user.png",
                    USERSAVEGAME = UPLAYSAVEGAME + System.IO.Path.DirectorySeparatorChar + userId
                });
            });
            return UserList;
        }

        public static List<string> GetUserIdList()
        {
            DirectoryInfo folder = new DirectoryInfo(UPLAYSAVEGAME);
            if (folder.Exists)
            {
                List<string> list = new List<string>();
                foreach (DirectoryInfo directory in folder.GetDirectories())
                {
                    list.Add(directory.Name);
                }
                return list;
            }
            return null;
        }

        /// <summary>
        /// 检查并获取已存在的存档的游戏
        /// </summary>
        /// <returns>检测到的已存在存档的游戏列表</returns>
        public static List<Game> CheckGameSaveDirectory(UserInfo selectuser)
        {
            List<Game> gamelist = SupportGameList;
            List<Game> Ugamelist = new List<Game>();
            if (selectuser != null)
            {
                string[] GameSaveDirectory = GetGameSaveDirectory(selectuser);
                foreach (string id in GameSaveDirectory)
                {
                    foreach (Game g in gamelist)
                    {
                        if (g.id.Equals(id))
                        {
                            //string imgpath = GAMELOGOCACHE+"\\" + g.img;
                            //if (File.Exists(imgpath))
                            //{
                            //    g.imgpath = imgpath;
                            //}

                            // 上方为使用Uplay缓存图片 已弃用
                            if (g.gameicon.Trim() != "")
                            {
                                g.imgpath = GAMEICON + g.gameicon;
                            }
                            Ugamelist.Add(g);
                            break;
                        }
                    }
                }
            }
            return Ugamelist;
        }

        /// <summary>
        /// 获取支持的游戏列表
        /// </summary>
        /// <returns></returns>
        public static List<Game> GetSupportGame()
        {
            List<Game> gamelist = JsonConvert.DeserializeObject<List<Game>>(Properties.Resources.game);
            return gamelist;
        }

        /// <summary>
        /// 获取用户所有存档文件夹
        /// </summary>
        /// <returns>返回存档文件夹名称，不包括路径</returns>
        public static String[] GetGameSaveDirectory(UserInfo selectuser)
        {
            if (Directory.Exists(selectuser.USERSAVEGAME))
            {
                String[] SaveDirectorys = Directory.GetDirectories(selectuser.USERSAVEGAME);
                for (int i = 0; i < SaveDirectorys.Length; i++)
                {
                    SaveDirectorys[i] = SaveDirectorys[i].Substring(SaveDirectorys[i].LastIndexOf('\\') + 1, SaveDirectorys[i].Length - SaveDirectorys[i].LastIndexOf('\\') - 1);
                }
                return SaveDirectorys;
            }
            return new string[0];
        }
    }
}
