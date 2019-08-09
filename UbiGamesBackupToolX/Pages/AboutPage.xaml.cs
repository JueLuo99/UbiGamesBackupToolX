using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UbiGamesBackupToolX.Pages
{
    /// <summary>
    /// AboutPage.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPage : Page
    {
        public SettingPage settingPage;
        public AboutPage()
        {
            InitializeComponent();
        }

        private void AboutfeedbackBtn_Clicked(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://jq.qq.com/?_wv=1027&k=50pui0f");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.ToolVersion.Text = "当前版本：" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
