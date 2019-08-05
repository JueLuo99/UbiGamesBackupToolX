using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UbiGamesBackupToolX.Bean;

namespace UbiGamesBackupToolX.Listener
{
    class GameStatusListener
    {
        private const int MAX_PATH = 260;
        public const int PROCESS_ALL_ACCESS = 0x000F0000 | 0x00100000 | 0xFFF;

        public Thread thread;
        private bool isrun = true;

        public delegate void Event_Handler(object sender, GameEventArgs gameEventArgs);
        //用委托声明两个事件
        public event Event_Handler Process_Event = null;
        public event Event_Handler Process_Exit = null;

        public List<GameEventArgs> RunGameList = new List<GameEventArgs>();
        public List<Game> UnRunGameList;


        [DllImport("coredll.dll")]
        public extern static int GetWindowThreadProcessId(IntPtr hWnd, ref int lpdwProcessId);
        [DllImport("coredll.dll")]
        public extern static IntPtr OpenProcess(int fdwAccess, int fInherit, int IDProcess);
        [DllImport("coredll.dll")]
        public extern static bool CloseHandle(IntPtr hObject);
        [DllImport("Coredll.dll", EntryPoint = "GetModuleFileName")]
        private static extern uint GetModuleFileName(IntPtr hModule, [Out] StringBuilder lpszFileName, int nSize);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public GameStatusListener()
        {
            Console.WriteLine("监听器初始化......");
            string path = System.AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "ListenerGameList.json";
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    String listenerJson = reader.ReadToEnd();
                    Console.WriteLine(listenerJson);
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
                        isrun = false;
                        Console.WriteLine("监听器初始化失败......");
                    }
                }
            }
        }
        public void start()
        {
            thread = new Thread(new ThreadStart(this.run));
            thread.IsBackground = true;
            thread.Start();
        }
        /// <summary>
        /// 监听器运行主体
        /// </summary>
        public void run()
        {
            Console.WriteLine("监听器运行......");
            int i = 0;
            while (isrun)
            {
                i++;
                Console.WriteLine("监听器监听第" + i + "次");
                //IntPtr windowHandle = FindWindow(null, "Assassin's Creed Origins");

                //启动
                for (int pos = 0; pos < UnRunGameList.Count; pos++)
                {
                    Game g = UnRunGameList[pos];
                    IntPtr title;
                    List<Game> gamelist = (from game in UnRunGameList
                                           where game.Title == g.Title
                                           select game).ToList();
                    Console.WriteLine(g.Title);
                    if ((title = FindWindow(null, g.Title)) != IntPtr.Zero)
                    {
                        GameEventArgs gameEvent = new GameEventArgs(gamelist, title);
                        Process_Event(this, gameEvent);
                        RunGameList.Add(gameEvent);
                        foreach (Game gg in gamelist)
                        {
                            UnRunGameList.Remove(gg);
                        }
                        i--;
                    }
                }
                //关闭
                for (int pos = 0; pos < RunGameList.Count; pos++)
                {
                    GameEventArgs g = RunGameList[pos];
                    List<Game> gamelist = (from game in g.game
                                           where game.Title == g.game[0].Title
                                           select game).ToList();
                    if (FindWindow(null, g.game[0].Title) == IntPtr.Zero)
                    {
                        Process_Exit(this, g);
                        RunGameList.Remove(g);
                        UnRunGameList.AddRange(gamelist);
                        i--;
                    }
                }
                Thread.Sleep(5000);
            }
        }
        /// <summary>
        /// 监听器运行时更新游戏监测列表
        /// </summary>
        public void UpdateGameList()
        {
            //code
        }
        /// <summary>
        /// 监听器重新启动
        /// </summary>
        public void restart()
        {
            Console.WriteLine("监听器停止......");
            isrun = false;
            stop();
            Console.WriteLine("监听器初始化......");
            string path = System.AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "ListenerGameList.json";
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    UnRunGameList = JsonConvert.DeserializeObject<List<Game>>(reader.ReadToEnd());
                    RunGameList = new List<GameEventArgs>();
                    isrun = true;
                    //如果待监听列表为空，不进行监听
                    if (UnRunGameList == null)
                    {
                        isrun = false;
                        Console.WriteLine("监听器初始化失败......");
                    }
                }
            }
            try
            {
                start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void stop()
        {
            if (thread != null)
            {
                Console.WriteLine("监听器停止......");
                isrun = false;
                thread.Abort();
            }
        }
        /// <summary>
        /// 通过窗体句柄获取应用程序运行路径
        /// </summary>
        /// <param name="hwnd">句柄</param>
        /// <returns>运行路径</returns>
        public static String GetAppRunPathFromHandle(IntPtr hwnd)
        {
            int pId = 0;
            IntPtr pHandle = IntPtr.Zero;
            GetWindowThreadProcessId(hwnd, ref pId);
            pHandle = OpenProcess(PROCESS_ALL_ACCESS, 0, pId);
            StringBuilder sb = new StringBuilder(MAX_PATH);
            GetModuleFileName(pHandle, sb, sb.Capacity);
            CloseHandle(pHandle);
            return sb.ToString();
        }

        /// <summary>
        /// 通过窗体句柄获取应用程序名称
        /// </summary>
        /// <param name="hwnd">句柄</param>
        /// <returns>应用程序名称</returns>
        public static string GetAppRunFileNameFromHandle(IntPtr hwnd)
        {
            return Path.GetFileNameWithoutExtension(GetAppRunFileNameFromHandle(hwnd));
        }
    }
    class GameEventArgs : EventArgs
    {
        public List<Game> game { get; set; }
        public IntPtr FormTile;
        public GameEventArgs(List<Game> game, IntPtr title)
        {
            this.game = game;
            this.FormTile = title;
        }
    }
}
