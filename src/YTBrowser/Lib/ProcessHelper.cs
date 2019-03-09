using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefWebkit.Lib
{
    /// <summary>
    /// 进程相关助手类
    /// </summary>
    public class ProcessHelper
    {
        /// <summary>
        /// 获取进程名称的PID，未获取到返回0
        /// </summary>
        /// <param name="processName">进程名称</param>
        /// <returns></returns>
        public static int GetPidByProcessName(string processName)
        {
            Process[] arrayProcess = Process.GetProcessesByName(processName);

            foreach (Process p in arrayProcess)
            {
                return p.Id;
            }
            return 0;
        }

        /// <summary>
        /// 通过进程名称结束进程
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static bool KillProcessByName(string processName)
        {
            bool bRes = false;
            try
            {
                Process[] arrayProcess = Process.GetProcessesByName(processName);
                foreach (Process p in arrayProcess)
                {
                    p.Kill();
                    p.WaitForExit();
                    bRes = true;
                    break;
                }
            }
            catch
            {

            }
            return bRes;
        }

        /// <summary>
        /// 通过程序路径启动程序
        /// </summary>
        /// <param name="strExeFullPath">应用程序的完整路径</param>
        public static void ProcessStartByName(string strExeFullPath)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = strExeFullPath;
                info.Arguments = "";
                info.WindowStyle = ProcessWindowStyle.Maximized;
                Process pro = Process.Start(info);
                pro.WaitForExit();
            }
            catch(Exception ex)
            {
                throw new Exception("启动应用程序：" + strExeFullPath +"失败！" + ex.ToString());
            }
        }
    }
}
