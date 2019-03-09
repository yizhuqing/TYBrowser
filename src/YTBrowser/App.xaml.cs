using CefSharp;
using CefWebkit.Lib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace TYBrowser
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            try
            {
                //Add Custom assembly resolver
                AppDomain.CurrentDomain.AssemblyResolve += Resolver;

                //Any CefSharp references have to be in another method with NonInlining
                // attribute so the assembly rolver has time to do it's thing.
                InitializeCefSharp();
            }
            catch (Exception ex)
            {
                MessageBox.Show("程序框架级错误：" + ex.ToString(), "错误！", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void InitializeCefSharp()
        {
            //Perform dependency check to make sure all relevant resources are in our output directory.
            var settings = new CefSettings();
            settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                   Environment.Is64BitProcess ? "x64" : "x86",
                                                   "CefSharp.BrowserSubprocess.exe");


            string strCurrentAppDir =AppDomain.CurrentDomain.BaseDirectory;//当前应用的根路径
            settings.Locale = "zh-CN";
            //Cookie支持
            settings.CachePath = strCurrentAppDir + "\\Cache";
            settings.PersistSessionCookies = true;
            //flash支持
            settings.CefCommandLineArgs["enable-system-flash"] = "1";
            settings.CefCommandLineArgs.Add("ppapi-flash-path", strCurrentAppDir + "\\Plugins\\pepflashplayer64_29_0_0_171.dll"); //指定flash的版本，不使用系统安装的flash版本
            settings.CefCommandLineArgs.Add("ppapi-flash-version", "64.29.0.0.171");
            //开启Media的命令参数
            //settings.CefCommandLineArgs.Add("enable-media-stream", "enable-media-stream");
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");
            //忽略证书错误
            settings.IgnoreCertificateErrors = true;

            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
        }


        // Will attempt to load missing assembly from either x86 or x64 subdir
        // Required by CefSharp to load the unmanaged dependencies when running using AnyCPU
        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }
    }
}
