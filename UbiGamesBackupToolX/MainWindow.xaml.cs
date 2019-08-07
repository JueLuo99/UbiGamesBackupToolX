﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UbiGamesBackupToolX.Bean;
using UbiGamesBackupToolX.Pages;
using UbiGamesBackupToolX.Utils;
using static UbiGamesBackupToolX.Utils.Win32;

namespace UbiGamesBackupToolX
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool RegisterShellHookWindow(IntPtr hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string Message);
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool DeregisterShellHookWindow(IntPtr hHandle);
        [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int cch);
        [DllImport("kernel32.dll")]
        public static extern Boolean AllocConsole();
        [DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);


        /// <summary>
        /// 用以存储Hook到的被创建窗体名称
        /// </summary>
        StringBuilder createWT = new StringBuilder(512);

        /// <summary>
        ///  用以获取窗体句柄
        /// </summary>
        WindowInteropHelper wih = null;

        public enum ShellEvents
        {
            HSHELL_WINDOWCREATED = 1,
            HSHELL_WINDOWDESTROYED = 2,
            HSHELL_ACTIVATESHELLWINDOW = 3,

            HSHELL_WINDOWACTIVATED = 4,
            HSHELL_GETMINRECT = 5,
            HSHELL_REDRAW = 6,
            HSHELL_TASKMAN = 7,
            HSHELL_LANGUAGE = 8,
            HSHELL_SYSMENU = 9,
            HSHELL_ENDTASK = 10,
            HSHELL_ACCESSIBILITYSTATE = 11,
            HSHELL_APPCOMMAND = 12,
            HSHELL_WINDOWREPLACED = 13,
            HSHELL_WINDOWREPLACING = 14,
            HSHELL_HIGHBIT = 0x8000,
            HSHELL_FLASH = (HSHELL_REDRAW | HSHELL_HIGHBIT),
            HSHELL_RUDEAPPACTIVATED = (HSHELL_WINDOWACTIVATED | HSHELL_HIGHBIT)
        }
        uint WM_ShellHook;

        MainPage mainPage = null;
        SettingPage settingPage = null;
        public MainWindow()
        {
            InitializeComponent();
            wih = new WindowInteropHelper(this);//获取当前窗体句柄
            initPage();
        }

        private void initPage()
        {
            mainPage = new MainPage()
            {
                mainWindow = this
            };
            settingPage = new SettingPage()
            {
                mainWindow = this
            };
        }


        public void showSettingPage()
        {
            this.PageBox.Content = settingPage;
        }

        public void showMainPage()
        {
            PageBox.Content = mainPage;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            showMainPage();
            //if (Config.Instance.AllowBackup)
            //{
            //    SetShellHook();
            //}
            //TODO 托盘图标等相关 主页面初始化优化
            SetShellHook();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UnSetShellHook();
        }
        private void MaxBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }
        private void Min_Click(object sender, RoutedEventArgs e)
        {
            //WindowState = WindowState.Minimized;
            this.Hide();
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        //-----------------------------------窗体移动-----------------------------------------

        private void TitlePanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SendMessage(wih.Handle, Win32.WM_NCLBUTTONDOWN, (int)Win32.HitTest.HTCAPTION, 0);
            return;
        }

        private void TitlePanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void TitlePanel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
        //------------------------------------------------------------------------------------


        //-------------------------------------最大化-----------------------------------

        /// <summary>
        /// 设置窗体最大大小（不遮挡任务栏）
        /// </summary>
        /// <param name="frm"></param>
        public virtual void SetFormMax(Window window)
        {
            window.MaxWidth = Screen.PrimaryScreen.WorkingArea.Width;
            window.MaxHeight = Screen.PrimaryScreen.WorkingArea.Height;
        }
        //------------------------------------------------------------------------------------

        //-------------------------------窗体大小更改-----------------------------------------
        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MaxBtn.Background = (ImageBrush)FindResource("MaxImageBrush");
            }
            else if (WindowState == WindowState.Normal)
            {
                MaxBtn.Background = (ImageBrush)FindResource("NormalImageBrush");
            }
        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(new HwndSourceHook(this.WndProc));
            }
        }

        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32.WM_GETMINMAXINFO: // WM_GETMINMAXINFO message  
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
                case Win32.WM_NCHITTEST: // WM_NCHITTEST message  
                    return WmNCHitTest(lParam, ref handled);
            }
            if (msg == WM_ShellHook)
            {
                switch ((ShellEvents)wParam)
                {
                    case ShellEvents.HSHELL_WINDOWCREATED:
                        GetWindowText(lParam, createWT, createWT.Capacity);
#if DEBUG
                        ConsoleLogHelper.WriteLineInfo("窗体" + EncodingConvert.GetUtf8ByUnicodeString(createWT.ToString())+ "被创建");
                        if (createWT.ToString().Equals("Ghost Recon® Wildlands"))
                        {
                            ConsoleLogHelper.WriteLineInfo("游戏幽灵行动启动");
                        }
                        else
                        {

                        }
#endif
                        break;
                    case ShellEvents.HSHELL_WINDOWDESTROYED:
                        GetWindowText(lParam, createWT, createWT.Capacity);
#if DEBUG
                        ConsoleLogHelper.WriteLineInfo("窗体" + createWT.ToString() + "被销毁");
#endif
                        break;
                }
            }
            return IntPtr.Zero;
        }
        private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            // MINMAXINFO structure  
            Win32.MINMAXINFO mmi = (Win32.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(Win32.MINMAXINFO));

            // Get handle for nearest monitor to this window  
            WindowInteropHelper wih = new WindowInteropHelper(this);
            IntPtr hMonitor = Win32.MonitorFromWindow(wih.Handle, Win32.MONITOR_DEFAULTTONEAREST);

            // Get monitor info  
            Win32.MONITORINFOEX monitorInfo = new Win32.MONITORINFOEX();
            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
            Win32.GetMonitorInfo(new HandleRef(this, hMonitor), monitorInfo);

            // Get HwndSource  
            HwndSource source = HwndSource.FromHwnd(wih.Handle);
            if (source == null)
                // Should never be null  
                throw new Exception("Cannot get HwndSource instance.");
            if (source.CompositionTarget == null)
                // Should never be null  
                throw new Exception("Cannot get HwndTarget instance.");

            // Get transformation matrix  
            System.Windows.Media.Matrix matrix = source.CompositionTarget.TransformFromDevice;

            // Convert working area  
            Win32.RECT workingArea = monitorInfo.rcWork;
            System.Windows.Point dpiIndependentSize =
                matrix.Transform(new System.Windows.Point(
                        workingArea.Right - workingArea.Left,
                        workingArea.Bottom - workingArea.Top
                        ));

            // Convert minimum size  
            System.Windows.Point dpiIndenpendentTrackingSize = matrix.Transform(new System.Windows.Point(
                this.MinWidth,
                this.MinHeight
                ));

            // Set the maximized size of the window  
            mmi.ptMaxSize.x = (int)dpiIndependentSize.X;
            mmi.ptMaxSize.y = (int)dpiIndependentSize.Y;

            // Set the position of the maximized window  
            mmi.ptMaxPosition.x = 0;
            mmi.ptMaxPosition.y = 0;

            // Set the minimum tracking size  
            mmi.ptMinTrackSize.x = (int)dpiIndenpendentTrackingSize.X;
            mmi.ptMinTrackSize.y = (int)dpiIndenpendentTrackingSize.Y;

            Marshal.StructureToPtr(mmi, lParam, true);
        }
        private readonly int customBorderThickness = 7;
        private readonly int cornerWidth = 8;
        /// <summary>  
        /// Mouse point used by HitTest  
        /// </summary>  
        private System.Windows.Point mousePoint = new System.Windows.Point();
        private IntPtr WmNCHitTest(IntPtr lParam, ref bool handled)
        {
            // Update cursor point  
            // The low-order word specifies the x-coordinate of the cursor.  
            // #define GET_X_LPARAM(lp) ((int)(short)LOWORD(lp))  
            this.mousePoint.X = (int)(short)(lParam.ToInt32() & 0xFFFF);
            // The high-order word specifies the y-coordinate of the cursor.  
            // #define GET_Y_LPARAM(lp) ((int)(short)HIWORD(lp))  
            this.mousePoint.Y = (int)(short)(lParam.ToInt32() >> 16);

            // Do hit test  
            handled = true;
            if (Math.Abs(this.mousePoint.Y - this.Top) <= this.cornerWidth
        && Math.Abs(this.mousePoint.X - this.Left) <= this.cornerWidth)
            { // Top-Left  
                return new IntPtr((int)Win32.HitTest.HTTOPLEFT);
            }
            else if (Math.Abs(this.ActualHeight + this.Top - this.mousePoint.Y) <= this.cornerWidth
                && Math.Abs(this.mousePoint.X - this.Left) <= this.cornerWidth)
            { // Bottom-Left  
                return new IntPtr((int)Win32.HitTest.HTBOTTOMLEFT);
            }
            else if (Math.Abs(this.mousePoint.Y - this.Top) <= this.cornerWidth
                && Math.Abs(this.ActualWidth + this.Left - this.mousePoint.X) <= this.cornerWidth)
            { // Top-Right  
                return new IntPtr((int)Win32.HitTest.HTTOPRIGHT);
            }
            else if (Math.Abs(this.ActualWidth + this.Left - this.mousePoint.X) <= this.cornerWidth
                && Math.Abs(this.ActualHeight + this.Top - this.mousePoint.Y) <= this.cornerWidth)
            { // Bottom-Right  
                return new IntPtr((int)Win32.HitTest.HTBOTTOMRIGHT);
            }
            else if (Math.Abs(this.mousePoint.X - this.Left) <= this.customBorderThickness)
            { // Left  
                return new IntPtr((int)Win32.HitTest.HTLEFT);
            }
            else if (Math.Abs(this.ActualWidth + this.Left - this.mousePoint.X) <= this.customBorderThickness)
            { // Right  
                return new IntPtr((int)Win32.HitTest.HTRIGHT);
            }
            else if (Math.Abs(this.mousePoint.Y - this.Top) <= this.customBorderThickness)
            { // Top  
                return new IntPtr((int)Win32.HitTest.HTTOP);
            }
            else if (Math.Abs(this.ActualHeight + this.Top - this.mousePoint.Y) <= this.customBorderThickness)
            { // Bottom  
                return new IntPtr((int)Win32.HitTest.HTBOTTOM);
            }
            else
            {
                handled = false;
                return IntPtr.Zero;
            }
        }
        //------------------------------------------------结束-------------------------------
        /// <summary>
        /// 安装钩子
        /// </summary>
        public void SetShellHook()
        {
            if (RegisterShellHookWindow(wih.Handle))
            {
                WM_ShellHook = RegisterWindowMessage("SHELLHOOK");
            }
        }
        /// <summary>
        /// 卸载钩子
        /// </summary>
        public void UnSetShellHook()
        {
            DeregisterShellHookWindow(wih.Handle);
        }

        public void initListenerRes()
        {

        }
    }
}
