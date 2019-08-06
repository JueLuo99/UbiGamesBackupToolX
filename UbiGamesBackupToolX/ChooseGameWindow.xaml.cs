using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UbiGamesBackupToolX.Bean;
using UbiGamesBackupToolX.Utils;

namespace UbiGamesBackupToolX
{
    /// <summary>
    /// ChooseGameWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseGameWindow : Window
    {
        public ChooseGameWindow()
        {
            InitializeComponent();
        }

        List<Game> SelectGameList = new List<Game>();
        List<Game> gamelist;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gamelist = new List<Game>(UbiFileUtils.SupportGameList);
            List<Game> havegamelist = GetHavedGameList();
            foreach(Game g in gamelist)
            {
                if((from game in havegamelist where game.id == g.id select game).Count() == 1)
                {
                    g.selected = true;
                    SelectGameList.Add(g);
                }
            }
            this.GameListView.ItemsSource = gamelist;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectGameList.Clear();
            Close();
        }

        private void GamePanelMouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            WrapPanel wrapPanel = (WrapPanel)sender;
            Game g = wrapPanel.Tag as Game;
            if (SelectGameList.Contains(g))
            {
                SelectGameList.Remove(g);
                g.selected = false;
            }
            else
            {
                SelectGameList.Add(g);
                g.selected = true;
            }
        }

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            using (Stream stream = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + System.IO.Path.DirectorySeparatorChar + "ListenerGameList.json", FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(SelectGameList));
                    writer.Flush();
                    writer.Close();
                    stream.Close();
                }
            }
            SelectGameList.Clear();
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            SelectGameList.Clear();
            Close();
        }

        private List<Game> GetHavedGameList()
        {
            List<Game> HavedGamelist;
            string path = System.AppDomain.CurrentDomain.BaseDirectory + System.IO.Path.DirectorySeparatorChar + "ListenerGameList.json";
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    HavedGamelist = JsonConvert.DeserializeObject<List<Game>>(reader.ReadToEnd());
                    //如果待监听列表为空，不进行监听
                    if (HavedGamelist != null)
                    {
                        return HavedGamelist;
                    }
                }
            }
            return new List<Game>();
        }
    }
}
