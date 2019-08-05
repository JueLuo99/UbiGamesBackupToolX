using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace UbiGamesBackupToolX.Utils
{
    class ConsoleLogHelper
    {
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32", CharSet = CharSet.Auto)]
        internal static extern bool AllocConsole();

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32", CharSet = CharSet.Auto)]
        internal static extern bool FreeConsole();
        [DllImport("kernel32")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32")]
        private static extern int GetConsoleOutputCP();
        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }
        /// <summary>
        /// 在程序启动时，执行一次即可
        /// </summary>
        public static void OpenConsole()
        {
            if (!HasConsole)
            {
                try
                {
                    var consoleTitle = "App Runtime Log";
                    AllocConsole();
                    InvalidateOutAndError();
                    //Console.BackgroundColor = ConsoleColor.Black;
                    //Console.CursorVisible = false;
                    //Console.Title = consoleTitle;
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                }
            }
        }


        /// <summary>
        /// 该方法只在退出程序时，调用
        /// </summary>
        public static void CloseConsole()
        {
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
        }

        public static void WriteLine(string msg)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void WriteLineError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(msg);
        }

        public static void WriteLineError(Exception ex)
        {
            WriteLineError(ex.Message);
        }

        public static void WriteLineInfo(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            WriteLine(msg);
        }
        static void InvalidateOutAndError()
        {
            Type type = typeof(System.Console);

            System.Reflection.FieldInfo _out = type.GetField("_out",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.FieldInfo _error = type.GetField("_error",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            System.Reflection.MethodInfo _InitializeStdOutError = type.GetMethod("InitializeStdOutError",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            Debug.Assert(_out != null);
            Debug.Assert(_error != null);

            Debug.Assert(_InitializeStdOutError != null);

            _out.SetValue(null, null);
            _error.SetValue(null, null);

            _InitializeStdOutError.Invoke(null, new object[] { true });
        }

        static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
    }
}
