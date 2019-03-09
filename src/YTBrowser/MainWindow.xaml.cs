using CefSharp;
using CefSharp.Wpf;
using CefWebkit.CefSharpLib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using System.Deployment.Application;

namespace TYBrowser
{
    public class J {
        public void Shutdown() {
            Process.Start("shutdown.exe", "-s");//关机
        }
    }
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
            ChromiumWebBrowser chrome = null;
        public void Shutdown()
        {
            Environment.Exit(0);// 
            //Process.Start("shutdown.exe", "-s");//关机
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                string paras = ApplicationDeployment.CurrentDeployment.ActivationUri.Query;
                MessageBox.Show("启动参数：" + paras);
            }
            //var settings = new CefSettings();
            //settings.BrowserSubprocessPath = @"x86\CefSharp.BrowserSubprocess.exe";
            //Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);

            config = GetConfig();
            chrome = new CefSharp.Wpf.ChromiumWebBrowser();

            //右键菜单栏
            MenuHandler.mainWindow = this;
            chrome.MenuHandler = new MenuHandler();
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            chrome.RegisterJsObject("_host",  this);
            //在本窗体中打开连接
            chrome.LifeSpanHandler = new OpenPageSelf();
            //下载接口实现
            chrome.DownloadHandler = new DownloadHandler();

            chrome.LoadError += Chrome_LoadError;
            chrome.Loaded += Chrome_Loaded;
            if (!string.IsNullOrEmpty(config.Url))
            {
                this.Content = chrome;
                chrome.Address = config.Url;
                txtUrl.Text = config.Url;
            }
            if(config.FristLoad)
                SetOpenStart(config.IsSelfStart);
            config.FristLoad = false;
            cbxIsSelfStart.IsChecked = config.IsSelfStart;
            SaveConfig(config);
        }

        private void Chrome_Loaded(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("浏览器加载完成：" + e.Source);
        }

        private void Chrome_LoadError(object sender, LoadErrorEventArgs e)
        {
            //MessageBox.Show("浏览器加载失败：" + e.ErrorText);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    this.Content = chrome;
                    break;
                case Key.F2:
                    this.Content = gSetting;
                    break;
                case Key.F4:
                    this.Close();
                    break;
                case Key.F5:
                    chrome.Reload();
                    break;
                case Key.F6:
                case Key.F11:
                    if (this.WindowState == WindowState.Normal)
                    {
                        this.WindowStyle = WindowStyle.None;
                        this.WindowState = WindowState.Maximized;
                    }
                    else
                    {
                        this.WindowStyle = WindowStyle.SingleBorderWindow;
                        this.WindowState = WindowState.Normal;
                    }
                    break;
            }
        }
        Config config = null;
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            this.Content = chrome;
            chrome.Address = txtUrl.Text;
            config.Url = txtUrl.Text;
            SaveConfig(config);
        }
        public void SaveConfig( Config config)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "/config.xml";
            var xml= XmlSerialize.Serialize<Config>(config);
            System.IO.File.WriteAllText(fileName, xml);
        }
        public Config GetConfig()
        {
            Config config = null;
            string fileName = AppDomain.CurrentDomain.BaseDirectory+"/config.xml";
            if (System.IO.File.Exists(fileName))
            {
                config = XmlSerialize.Deserialize<Config>(System.IO.File.ReadAllText(fileName));
            }
            else
            {
                config = new Config();
            }
            return config;
        }
        public class Config {
            private bool _fristLoad = true;
            /// <summary>
            /// 第一次加载
            /// </summary>
            public bool FristLoad { get { return _fristLoad; } set { _fristLoad = value; } }
            public string Url { get; set; }
            private bool _isSelfStart = true;
            /// <summary>
            /// 是否开机启动
            /// </summary>
            public bool IsSelfStart { get { return _isSelfStart; }set { _isSelfStart = value; } }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetOpenStart(cbxIsSelfStart.IsChecked.Value);
        }

        private void SetOpenStart(bool isOpenStart)
        {
            string R_startPath = Assembly.GetExecutingAssembly().Location;
            if (isOpenStart)
            {
                RegistryKey R_local = Registry.LocalMachine;
                RegistryKey R_run = R_local.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                R_run.SetValue("TYBrowser", R_startPath);
                R_run.Close();
                R_local.Close();
                cbxIsSelfStart.IsChecked = true;
                config.IsSelfStart = true;
                SaveConfig(config);
            }
            else
            {
                try
                {
                    RegistryKey R_local = Registry.LocalMachine;
                    RegistryKey R_run = R_local.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                    R_run.DeleteValue("TYBrowser", false);
                    R_run.Close();
                    R_local.Close();
                    cbxIsSelfStart.IsChecked = false;
                    config.IsSelfStart = false;
                    SaveConfig(config);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("您需要管理员权限修改", "提示",MessageBoxButton.OK);
                }
            }
        }

        private void cbxIsSelfStart_Unchecked(object sender, RoutedEventArgs e)
        {
            SetOpenStart(cbxIsSelfStart.IsChecked.Value);
        }
    }
}
