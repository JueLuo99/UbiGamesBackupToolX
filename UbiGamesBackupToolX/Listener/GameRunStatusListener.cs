using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UbiGamesBackupToolX.Bean;

namespace UbiGamesBackupToolX.Listener
{
    class GameRunStatusListener
    {
        public delegate void GameEventHandler(GameEventArgs gameEventArgs);
        public GameEventHandler RunGameEvent = null;
        public GameEventHandler ExitGameEvent = null;

        //public GameEventHandler RunGameEventCallback = null;
        //public GameEventHandler ExitGameEventCallback = null;

        private List<Game> RunGameList { get; set; } = new List<Game>();
        private List<Game> UnRunGameList { get; set; }

        public GameRunStatusListener()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "ListenerGameList.json";
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    String listenerJson = reader.ReadToEnd();
                    //Encoding defaultEncoding = Encoding.Default;
                    //Encoding windows1252 = Encoding.GetEncoding(1252);
                    ////Console.WriteLine("编码方式：" + defaultEncoding.WindowsCodePage);
                    //Encoding utf8 = Encoding.UTF8;
                    //byte[] utf8s = utf8.GetBytes(listenerJson);
                    ////Console.WriteLine(defaultEncoding.GetString(Encoding.Convert(defaultEncoding, utf8, utf8s)));
                    //Console.WriteLine(windows1252.GetString(Encoding.Convert(windows1252, utf8, utf8s)));
                    UnRunGameList = JsonConvert.DeserializeObject<List<Game>>(listenerJson);
                    //如果待监听列表为空，不进行监听
                    if (UnRunGameList == null)
                    {
                        Console.WriteLine("监听器初始化失败......");
                    }
                }
            }
        }

        public void GameRunning(string name)
        {
            if (UnRunGameList != null && UnRunGameList.Count > 0)
            {
                List<Game> gamelist = (from game in UnRunGameList
                                       where game.Title.Equals(name)
                                       select game).ToList();
                if (gamelist.Count > 0)
                {
                    GameEventArgs gameEvent = new GameEventArgs()
                    {
                        game = gamelist
                    };
                    RunGameList.AddRange(gamelist);
                    if (RunGameEvent != null)
                    {
                        RunGameEvent(gameEvent);
                    }
                    if (UnRunGameList != null)
                    {
                        foreach (Game gg in gamelist)
                        {
                            UnRunGameList.Remove(gg);
                        }
                    }
                }
            }
        }

        public void GameExit(string name)
        {
            if (RunGameList != null && RunGameList.Count >= 0)
            {
                List<Game> gamelist = (from game in RunGameList
                                       where game.Title.Equals(name)
                                       select game).ToList();
                if (gamelist.Count > 0)
                {
                    GameEventArgs gameEvent = new GameEventArgs()
                    {
                        game = gamelist
                    };
                    UnRunGameList.AddRange(gamelist);
                    if (ExitGameEvent != null)
                    {
                        ExitGameEvent(gameEvent);
                    }
                    if (RunGameList != null)
                    {
                        foreach (Game gg in gamelist)
                        {
                            RunGameList.Remove(gg);
                        }
                    }
                }
            }
        }

        public class GameEventArgs
        {
            public List<Game> game { get; set; }
        }
    }

}
