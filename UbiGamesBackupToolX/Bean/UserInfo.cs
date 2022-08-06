using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbiGamesBackupToolX.Bean
{
    [JsonObject(MemberSerialization.OptOut)]
    public class UserInfo: INotifyPropertyChanged
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UID { get; set; }
        /// <summary>
        /// 备份时间
        /// </summary>
        public string BackupTime { get; set; }
        /// <summary>
        /// 用户存档位置
        /// </summary>
        public string USERSAVEGAME { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string USERIMAGE { get; set; }

        private bool _selected = false;

        /// <summary>
        /// 是否被选中
        /// </summary>
        [JsonIgnore]
        public bool selected { get {
                return _selected;
            } set {
                _selected = value;
                NotifyPropertyChanged(nameof(selected));
            } } //使用JsonIgnore在序列化时忽略该属性 selected仅用于标记当前选中的用户
        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
