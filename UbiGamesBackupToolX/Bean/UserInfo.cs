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
    class UserInfo: INotifyPropertyChanged
    {
        public string UID { get; set; }
        public string UNAME { get; set; }
        public string BackupTime { get; set; }
        public string USERSAVEGAME { get; set; }
        public string USERIMAGE { get; set; }
        private bool _selected = false;
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
