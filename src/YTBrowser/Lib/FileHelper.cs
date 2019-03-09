using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CefWebkit.Lib
{
    public class FileHelper
    {
        #region API函数声明

        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);
        #endregion

        #region 写log日志
        /// <summary>
        /// 输出日志，默认日志路径为：当前工程文件夹\Logs\DateTime.Now.Date.ToString("yyyyMMdd").log
        /// </summary>
        /// <param name="msg">日志输出的信息</param>
        public static void WriteLog(String msg)
        {

            string fileName = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Logs\\" + DateTime.Now.Date.ToString("yyyyMMdd") + ".log";
            WriteLog(fileName, msg);
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="strFileFullPath">输出日志的完整路径（包含文件夹及文件名）</param>
        /// <param name="msg">日志输出的信息</param>
        public static void WriteLog(string strFileFullPath, string msg)
        {
            try
            {
                //强制文件名后缀为.log
                if (!strFileFullPath.ToLower().Contains(".log"))
                {
                    strFileFullPath += ".log";
                }

                //判断路径文件夹是否存在
                if (!Directory.Exists(Path.GetDirectoryName(strFileFullPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(strFileFullPath));
                }

                //判断文件名称是否存在
                if (!File.Exists(strFileFullPath))
                {
                    //不存在文件
                    File.Create(strFileFullPath).Close();//创建该文件
                }
                StreamWriter writer = File.AppendText(strFileFullPath);//文件中添加文件流  
                try
                {
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + msg);

                }
                catch (Exception e)
                {
                    throw new Exception("写入日志错误！" + e.ToString());
                }
                finally
                {
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("日志输出失败！" + ex.ToString());
            }
        }
        #endregion

        #region 读log日志
        /// <summary>
        /// 读取默认路径下当前日期的日志文件
        /// </summary>
        /// <returns></returns>
        public static string ReadLog()
        {
            string fileName = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Logs\\" + DateTime.Now.Date.ToString("yyyyMMdd") + ".log";
            return ReadLog(fileName);
        }

        /// <summary>
        /// 读取指定路径的日志文件
        /// </summary>
        /// <param name="strFileFullPath">全路径</param>
        /// <returns></returns>
        public static string ReadLog(string strFileFullPath)
        {
            string strRes = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(strFileFullPath))
                {
                    throw new Exception("读取日志文件路径错误！");
                }

                //判断文件名称是否存在
                if (File.Exists(strFileFullPath))
                {
                    strRes = File.ReadAllText(strFileFullPath);//读取日志
                }
                else
                {
                    return "读取的日志文件【" + strFileFullPath + "】不存在！";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("读取日志失败！" + ex.ToString());
            }
            return strRes;
        }
        #endregion

        #region 获取Ini文件数据
        /// <summary>
        /// 获取Ini文件数据
        /// </summary>
        /// <param name="strIniFileFullPath">Ini文件的全路径（包含文件夹及文件名）</param>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string GetIniData(string strIniFileFullPath, string Section, string Key)
        {
            if (string.IsNullOrWhiteSpace(strIniFileFullPath))
            {
                return string.Empty;
            }

            if (File.Exists(strIniFileFullPath))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, null, temp, 1024, strIniFileFullPath);
                return temp.ToString();
            }
            else
            {
                return String.Empty;
            }
        }
        #endregion

        #region 设置Ini文件数据
        /// <summary>
        /// 设置ini文件
        /// </summary>
        /// <param name="strIniFileFullPath">Ini文件的全路径（包含文件夹及文件名）</param>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool WriteIniData(string strIniFileFullPath, string Section, string Key, string Value)
        {
            if (string.IsNullOrEmpty(strIniFileFullPath))
            {
                return false;
            }
            else if (!strIniFileFullPath.ToLower().Contains(".ini"))
            {
                strIniFileFullPath += ".ini";
            }
            try
            {
                if (Directory.Exists(Path.GetDirectoryName(strIniFileFullPath)) == false)//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(strIniFileFullPath));
                }

                if (File.Exists(strIniFileFullPath) == false)
                {
                    File.Create(strIniFileFullPath);
                }

                long OpStation = WritePrivateProfileString(Section, Key, Value, strIniFileFullPath);
                if (OpStation == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 文件读取
        /// <summary>
        /// 读取文件（编码使用默认编码）
        /// </summary>
        /// <param name="fileName">文件路径及名称</param>
        /// <returns></returns>
        public static string ReadFile(string fileName)
        {
            return ReadFile(fileName, Encoding.Default);
        }


        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="fileName">文件路径及名称</param>
        /// <param name="encoding">读取文件的编码</param>
        /// <returns></returns>
        public static string ReadFile(string fileName, Encoding encoding)
        {
            string strRes = string.Empty;
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader m_streamReader = new StreamReader(fs, encoding);
            try
            {
                //使用StreamReader类来读取文件
                m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                //从数据流中读取每一行，直到文件的最后一行，并在textBox中显示出内容，其中textBox为文本框，如果不用可以改为别的                
                string strLine = m_streamReader.ReadLine();
                while (strLine != null)
                {
                    strRes += strLine + "\n";
                    strLine = m_streamReader.ReadLine();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("读取文件错误！\r\n" + ex.ToString());
            }
            finally
            {
                //关闭此StreamReader对象
                m_streamReader.Close();
                fs.Close();
            }
            return strRes;
        }
        #endregion

        #region 当前应用程序路径
        /// <summary>
        /// 获取当前引用程序的路径
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentAppPath()
        {
            return System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }
        #endregion
    }
}
