using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UbiGamesBackupToolX.Bean
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Game: INotifyPropertyChanged
    {
        public string id { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public string Version { get; set; }
        public string Title { get; set; }
        public string AppName { get; set; }
        public string imgpath { get; set; } = "../Resources/gamepanelbackground.png";

        private bool _selected = false;
        [JsonIgnore]
        public bool selected { get {
                return _selected;
            } set {
                _selected = value;
                NotifyPropertyChanged(nameof(selected));
            }
        }

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
