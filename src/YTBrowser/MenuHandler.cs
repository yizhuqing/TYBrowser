using CefSharp;
using CefSharp.Wpf;
using CefWebkit.CefSharpLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TYBrowser
{
    public class MenuHandler : IContextMenuHandler
    {
        public static Window mainWindow { get; set; }
        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {

        }

        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            return true;
        }

        void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
            //隐藏菜单栏
            var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

            chromiumWebBrowser.Dispatcher.Invoke(() =>
            {
                chromiumWebBrowser.ContextMenu = null;
            });
        }

        bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {

            //绘制了一遍菜单栏  所以初始化的时候不必绘制菜单栏，再此处绘制即可
            var chromiumWebBrowser = (ChromiumWebBrowser)browserControl;

            chromiumWebBrowser.Dispatcher.Invoke(() =>
            {
                var menu = new ContextMenu
                {
                    IsOpen = true
                };

                RoutedEventHandler handler = null;

                handler = (s, e) =>
                {
                    menu.Closed -= handler;

                    //If the callback has been disposed then it's already been executed
                    //so don't call Cancel
                    if (!callback.IsDisposed)
                    {
                        callback.Cancel();
                    }
                };

                menu.Closed += handler;

                menu.Items.Add(new MenuItem
                {
                    Header = "最小化",
                    Command = new CustomCommand(MinWindow)
                });
                menu.Items.Add(new MenuItem
                {
                    Header = "关闭",
                    
                    Command = new CustomCommand(CloseWindow)
                });
                menu.Items.Add(new MenuItem
                {
                    Header = "关机",
                    Command = new CustomCommand(Shutdown)
                });
                chromiumWebBrowser.ContextMenu = menu;

            });

            return true;
        }
        /// <summary>
        /// 关机
        /// </summary>
        private void Shutdown()
        {
            //调用线程无法访问此对象,因为另一个线程拥有该对象
            var rt=MessageBox.Show("确定要关机吗？","提示",MessageBoxButton.YesNo);
            if (rt == MessageBoxResult.Yes)
            {
                //handler和window是两个线程，WPF做了线程安全。。。so以下
                mainWindow.Dispatcher.Invoke(
                    new Action(
                            delegate
                            {
                                Process.Start("shutdown.exe", "-s");//关机
                            }
                        ));
            }
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        private void CloseWindow()
        {
            //调用线程无法访问此对象,因为另一个线程拥有该对象
            //handler和window是两个线程，WPF做了线程安全。。。so以下
            mainWindow.Dispatcher.Invoke(
                new Action(
                        delegate
                        {
                            Application.Current.Shutdown();
                        }
                    ));
        }

        /// <summary>
        /// 最小化窗体
        /// </summary>
        private void MinWindow()
        {
            mainWindow.Dispatcher.Invoke(
                new Action(
                        delegate
                        {
                            mainWindow.WindowState = WindowState.Minimized;
                        }
                    ));
        }

        private static IEnumerable<Tuple<string, CefMenuCommand>> GetMenuItems(IMenuModel model)
        {
            var list = new List<Tuple<string, CefMenuCommand>>();
            for (var i = 0; i < model.Count; i++)
            {
                var header = model.GetLabelAt(i);
                var commandId = model.GetCommandIdAt(i);
                list.Add(new Tuple<string, CefMenuCommand>(header, commandId));
            }

            return list;
        }
    }
}
