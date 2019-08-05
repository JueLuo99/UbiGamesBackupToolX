using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGamesBackupToolX.Bean
{
    [JsonObject(MemberSerialization.OptIn)]
    class Config
    {
        private static Config _instance;
        public static Config Instance {
            get{
                if (_instance == null)
                {
                    if (File.Exists(Config.ConfigFilePath))
                    {
                        using (Stream stream = new FileStream(ConfigFilePath, FileMode.Open))
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                _instance = JsonConvert.DeserializeObject<Config>(reader.ReadToEnd());
                            }
                            if (_instance == null)
                            {
                                _instance = new Config();
                                _instance.Save();
                            }
                        }
                    }
                    else
                    {
                        _instance = new Config();
                        _instance.Save();
                    }
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// Config存储路径
        /// </summary>
        public static string ConfigFilePath = System.AppDomain.CurrentDomain.BaseDirectory + System.IO.Path.DirectorySeparatorChar + "config.json";

        /// <summary>
        /// 是否开启自动备份
        /// </summary>
        private bool allowbackup = false;
        /// <summary>
        /// 是否开启自动备份Get、Set
        /// </summary>
        public bool AllowBackup { get { return allowbackup; } set { allowbackup = value; } }
        /// <summary>
        /// 自动备份文件夹路径
        /// 默认应为当前工作目录下AllowBackup文件夹
        /// </summary>
        private string allowbackuppath = System.AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "AllowBackup";
        /// <summary>
        /// 自动备份路径GET、SET
        /// </summary>
        public string AllowBackupPath
        {
            get { return allowbackuppath; }
            set
            {
                allowbackuppath = value;
            }
        }

        //自动备份提示显示位置---------已弃用
        //private string allowbackupshowlocation = "TopLeft";
        //public string AllowBackupShowLocation
        //{
        //    get { return allowbackupshowlocation; }
        //    set
        //    {
        //        allowbackupshowlocation = value;
        //    }
        //}

        /// <summary>
        /// 启动游戏提示状态
        /// </summary>
        private bool rungametipstatus = true;
        /// <summary>
        /// 启动游戏提示状态GET、SET
        /// </summary>
        public bool RunGameTipStatus
        {
            get
            {
                return rungametipstatus;
            }
            set
            {
                rungametipstatus = value;
            }
        }
        /// <summary>
        /// 退出游戏提示状态
        /// </summary>
        private bool exitgametipstatus = true;
        /// <summary>
        /// 退出游戏提示状态GET、SET
        /// </summary>
        public bool ExitGameTipStatus
        {
            get
            {
                return exitgametipstatus;
            }
            set
            {
                exitgametipstatus = value;
            }
        }

        /// <summary>
        /// 隐藏的构造方法
        /// </summary>
        private Config()
        {
            
        }



        /// <summary>
        /// 存储当前配置
        /// 调用此方法后，才会持久化到本地文件中
        /// </summary>
        public void Save()
        {
            if (this != null)
            {
                using (Stream stream = new FileStream(ConfigFilePath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(JsonConvert.SerializeObject(this));
                    }
                }
            }
        }
    }
}
