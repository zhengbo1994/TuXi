using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
public class RarHelper
{
    public RarHelper()
    {

    }

    static RarHelper()
    {
        //判断是否安装了WinRar.exe
        RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
        _existSetupWinRar = !string.IsNullOrEmpty(key.GetValue(string.Empty).ToString());

        //获取WinRar.exe路径
        _winRarPath = key.GetValue(string.Empty).ToString();
    }

    static bool _existSetupWinRar;
    /// <summary>
    /// 获取是否安装了WinRar的标识
    /// </summary>
    public static bool ExistSetupWinRar
    {
        get { return _existSetupWinRar; }
    }

    static string _winRarPath;
    /// <summary>
    /// 获取WinRar.exe路径
    /// </summary>
    public static string WinRarPath
    {
        get { return _winRarPath; }
    }

    #region 压缩到.rar,这个方法针对目录压缩
    /// <summary>
    /// 压缩到.rar,这个方法针对目录压缩
    /// </summary>
    /// <param name="intputPath">输入目录</param>
    /// <param name="outputPath">输出目录</param>
    /// <param name="outputFileName">输出文件名</param>
    public static void CompressRar(string intputPath, string outputPath, string outputFileName)
    {
        //rar 执行时的命令、参数
        string rarCmd;
        //启动进程的参数
        ProcessStartInfo processStartInfo = new ProcessStartInfo();
        //进程对象
        Process process = new Process();
        try
        {
            if (!ExistSetupWinRar)
            {
                throw new ArgumentException("请确认服务器上已安装WinRar应用！");
            }
            //判断输入目录是否存在
            if (!Directory.Exists(intputPath))
            {
                throw new ArgumentException("指定的要压缩目录不存在！");
            }
            //命令参数  uxinxin修正参数压缩文件到当前目录，而不是从盘符开始
            rarCmd = " a " + outputFileName + " " + "./" + " -r";
            //rarCmd = " a " + outputFileName + " " + outputPath + " -r";
            //创建启动进程的参数
            //指定启动文件名
            processStartInfo.FileName = WinRarPath;
            //指定启动该文件时的命令、参数
            processStartInfo.Arguments = rarCmd;
            //指定启动窗口模式：隐藏
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //指定压缩后到达路径
            processStartInfo.WorkingDirectory = outputPath;
            //创建进程对象

            //指定进程对象启动信息对象
            process.StartInfo = processStartInfo;
            //启动进程
            process.Start();
            //指定进程自行退行为止
            process.WaitForExit();
            //Uxinxin增加的清理关闭，不知道是否有效
            process.Close();
            process.Dispose();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            process.Close();
            process.Dispose();

        }
    }
    #endregion

    #region 解压.rar
    /// <summary>
    /// 解压.rar
    /// </summary>
    /// <param name="inputRarFileName">输入.rar</param>
    /// <param name="outputPath">输出目录</param>
    public static void UnCompressRar(string inputRarFileName, string outputPath)
    {
        //rar 执行时的命令、参数
        string rarCmd;
        //启动进程的参数
        ProcessStartInfo processStartInfo = new ProcessStartInfo();
        //进程对象
        Process process = new Process();
        try
        {
            if (!ExistSetupWinRar)
            {
                throw new ArgumentException("请确认服务器上已安装WinRar应用！");
            }
            //如果压缩到目标路径不存在
            if (!Directory.Exists(outputPath))
            {
                //创建压缩到目标路径
                Directory.CreateDirectory(outputPath);
            }
            rarCmd = "x " + inputRarFileName + " " + outputPath + " -y";


            processStartInfo.FileName = WinRarPath;
            processStartInfo.Arguments = rarCmd;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.WorkingDirectory = outputPath;


            process.StartInfo = processStartInfo;
            process.Start();
            process.WaitForExit();
            process.Close();
            process.Dispose();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            process.Close();
            process.Dispose();
        }
    }
    #endregion

    #region 将传入的文件列表压缩到指定的目录下
    /// <summary>
    /// 将传入的文件列表压缩到指定的目录下
    /// </summary>
    /// <param name="sourceFilesPaths">要压缩的文件路径列表</param>
    /// <param name="compressFileSavePath">压缩文件存放路径</param>
    /// <param name="compressFileName">压缩文件名（全名）</param>
    public static void CompressFilesToRar(List<string> sourceFilesPaths, string compressFileSavePath, string compressFileName)
    {
        //rar 执行时的命令、参数
        string rarCmd;
        //启动进程的参数
        ProcessStartInfo processStartInfo = new ProcessStartInfo();
        //创建进程对象
        //进程对象
        Process process = new Process();
        try
        {
            if (!ExistSetupWinRar)
            {
                throw new ArgumentException("Not setuping the winRar, you can Compress.make sure setuped winRar.");
            }
            //判断输入文件列表是否为空
            if (sourceFilesPaths == null || sourceFilesPaths.Count < 1)
            {
                throw new ArgumentException("CompressRar'arge : sourceFilesPaths cannot be null.");
            }
            rarCmd = " a -ep1 -ap " + compressFileName;
            //-ep1 -ap表示压缩时不保留原有文件的路径，都压缩到压缩包中即可,调用winrar命令内容可以参考我转载的另一篇文章：教你如何在DOS(cmd)下使用WinRAR压缩文件
            foreach (object filePath in sourceFilesPaths)
            {
                rarCmd += " " + filePath.ToString(); //每个文件路径要与其他的文件用空格隔开
            }
            //rarCmd += " -r";
            //创建启动进程的参数

            //指定启动文件名
            processStartInfo.FileName = WinRarPath;
            //指定启动该文件时的命令、参数
            processStartInfo.Arguments = rarCmd;
            //指定启动窗口模式：隐藏
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //指定压缩后到达路径
            processStartInfo.WorkingDirectory = compressFileSavePath;

            //指定进程对象启动信息对象
            process.StartInfo = processStartInfo;
            //启动进程
            process.Start();
            //指定进程自行退行为止
            process.WaitForExit();
            process.Close();
            process.Dispose();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            process.Close();
            process.Dispose();
        }
    }
    #endregion
}