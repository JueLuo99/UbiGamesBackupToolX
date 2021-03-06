﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UbiGamesBackupToolX.Bean;

namespace UbiGamesBackupToolX.Listener
{
    public class GameRunStatusListener
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
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "ListenerGameList.json";
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(stream,Encoding.Unicode))
                {
                    String listenerJson = reader.ReadToEnd();
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
        public void RefreshGameList()
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "ListenerGameList.json";
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(stream))
                {

                    List<Game> Temp = JsonConvert.DeserializeObject<List<Game>>(reader.ReadToEnd());
                    for (int i = 0; i < RunGameList.Count; i++)
                    {
                        Game g = (Game)RunGameList[i];
                        if (Temp.Contains(g))
                        {
                            Temp.Remove(g);
                        }
                        else
                        {
                            RunGameList.Remove(g);
                            i--;
                        }
                    }
                    UnRunGameList = Temp;
                }
            }
        }

        public class GameEventArgs
        {
            public List<Game> game { get; set; }
        }
    }

}
