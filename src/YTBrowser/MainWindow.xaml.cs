using CefSharp;
using CefSharp.Wpf;
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

namespace TYBrowser
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ChromiumWebBrowser chrome=null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var settings = new CefSettings();
            //settings.BrowserSubprocessPath = @"x86\CefSharp.BrowserSubprocess.exe";
            //Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);

            config = GetConfig();
            chrome = new CefSharp.Wpf.ChromiumWebBrowser();
            chrome.LoadError += Chrome_LoadError;
            chrome.Loaded += Chrome_Loaded;
            if (!string.IsNullOrEmpty(config.Url))
            {
                this.Content = chrome;
                chrome.Address = config.Url;
                txtUrl.Text = config.Url;
            }
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
                case Key.F11:
                    if (this.WindowState == WindowState.Normal)
                        this.WindowState = WindowState.Maximized;
                    else
                        this.WindowState = WindowState.Normal;
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
            public string Url { get; set; }
        }
    }
}
