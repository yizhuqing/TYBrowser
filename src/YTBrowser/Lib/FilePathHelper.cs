using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefWebkit.Lib
{
    /// <summary>
    /// 文件路径助手类
    /// </summary>
    public class FilePathHelper
    {      
        #region 默认参数值获取
        /// <summary>
        /// 获取桌面路径
        /// </summary>
        /// <returns></returns>
        /// 
        public static string GetDesktopDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }


        /// <summary>
        /// 获取当前应用文件夹路径
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentAppDirectory()
        {
            return System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }

        /// <summary>
        /// 获取当前引用的根路径（若为开发模式，返回项目的根路径；若为安装模式，返回安装目录的根路径）
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentAppRootPath()
        {
            string strRes = GetCurrentAppDirectory();
            if (strRes.ToLower().Contains(@"bin\debug") || strRes.ToLower().Contains(@"bin\release"))
            {
                //调试模式，返回调试模式下，项目的根路径
                strRes = Path.GetFullPath("../..");
            }
            return strRes;
        }

        /// <summary>
        /// 获取默认文件筛选器文件类型的字符串 
        /// </summary>
        /// <returns>所有文件|*.*</returns>
        public static string GetDefaultFilterStr()
        {
            return "所有文件|*.*";
        }
        #endregion

        #region 获取目录文件路径操作
        /// <summary>
        /// 获取目录的文件路径（不对指定需要获取的目录的文件扩展名过滤）
        /// </summary>
        /// <param name="strDirectoryPath"></param>
        /// <param name="isRecursion"></param>
        /// <param name="listFilePaths"></param>
        public static void GetDirectoryFilePaths(string strDirectoryPath, bool isRecursion, ref List<string> listFilePaths)
        {
            GetDirectoryFilePaths(strDirectoryPath, "", isRecursion, ref listFilePaths);
        }

        /// <summary>
        /// 获取目录的文件路径（不对指定需要获取的目录进行递归和文件扩展名过滤）
        /// </summary>
        /// <param name="strDirectoryPath"></param>
        /// <param name="listFilePaths"></param>
        public static void GetDirectoryFilePaths(string strDirectoryPath, ref List<string> listFilePaths)
        {
            GetDirectoryFilePaths(strDirectoryPath, "", false, ref listFilePaths);
        }

        /// <summary>
        /// 获取目录的文件路径（不对指定需要获取的目录进行递归）
        /// </summary>
        /// <param name="strDirectoryPath">指定需要获取的目录</param>
        /// <param name="strExtFilter">文件扩展名过滤</param>
        /// <param name="listFilePaths">引用值返回结果</param>
        public static void GetDirectoryFilePaths(string strDirectoryPath, string strExtFilter, ref List<string> listFilePaths)
        {
            GetDirectoryFilePaths(strDirectoryPath, strExtFilter, false, ref listFilePaths);
        }

        /// <summary>
        /// 获取目录的文件路径
        /// </summary>
        /// <param name="strDirectoryPath">指定需要获取的目录</param>
        /// <param name="strExtFilter">文件扩展名过滤</param>
        /// <param name="isRecursion">是否需要递归查找执行目录下的文件</param>
        /// <param name="listFilePaths">引用值返回结果</param>
        public static void GetDirectoryFilePaths(string strDirectoryPath, string strExtFilter, bool isRecursion, ref List<string> listFilePaths)
        {
            try
            {
                //Thread.Sleep(50);
                if (isRecursion)
                {
                    string[] subPaths = System.IO.Directory.GetDirectories(strDirectoryPath);//得到所有子目录
                    foreach (string path in subPaths)
                    {
                        GetDirectoryFilePaths(path, strExtFilter, isRecursion, ref listFilePaths);//对每一个字目录做与根目录相同的操作：即找到子目录并将当前目录的文件名存入List  
                    }
                }


                string[] files = System.IO.Directory.GetFiles(strDirectoryPath);
                foreach (string file in files)
                {
                    //判断是否过滤文件扩展名,为空则不过滤
                    if (!string.IsNullOrWhiteSpace(strExtFilter))
                    {
                        //文件扩展名
                        string fileExtension = System.IO.Path.GetExtension(file);
                        if (fileExtension.ToLower().Equals(strExtFilter.ToLower()))
                        {
                            listFilePaths.Add(file);
                        }
                    }
                    else
                    {

                        listFilePaths.Add(file);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("获取指定目录的文件路径出错！" + ex.ToString());
            }
        }
        #endregion

        #region 获取系统盘符
        /// <summary>
        /// 获取系统盘符
        /// </summary>
        /// <returns></returns>
        public static string GetSystemDrive()
        {
            string strRes = GetCurrentAppRootPath();
            try
            {
                strRes = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
            }
            catch
            {

            }
            return strRes;
        }
        #endregion
    }
}
