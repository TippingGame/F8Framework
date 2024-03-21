using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace F8Framework.Core
{
    public static partial class Util
    {
        /// <summary>
        /// Zip压缩帮助类
        /// </summary>
        public static class ZipHelper
        {
            private const int BufferSize = 2048;
            
            // 压缩回调接口
            public interface IZipCallback
            {
                bool OnPreZip(ZipEntry entry); //true表示继续执行
                void OnPostZip(ZipEntry entry);
                void OnFinished(string result);
            }
            
            public class ZipResult : IZipCallback
            {
                public bool OnPreZip(ZipEntry entry)
                {
                    if (entry.IsFile)
                    {
                        string extension = Path.GetExtension(entry.Name).ToLower();
                        if (extension == ".meta" || extension == ".ds_store")
                            return false;
                    }

                    return true;
                }

                public void OnPostZip(ZipEntry entry)
                {
                    // LogF8.LogUtil("OnPostZip : " + _entry.Name);
                }

                public void OnFinished(string result)
                {
                    LogF8.LogUtil("Zip Finished : " + result);
                }
            }
            
            /// <summary>
            /// 解压缩Zip文件
            /// </summary>
            public static bool UnZipFile(string sourceFile, string destinationDirectory = null,
                string password = null, bool coverFile = false)
            {
                bool result = false;

                if (!File.Exists(sourceFile))
                {
                    LogF8.LogError("要解压的文件不存在：" + sourceFile);
                    return result;
                }

                if (string.IsNullOrWhiteSpace(destinationDirectory))
                {
                    destinationDirectory = Path.GetDirectoryName(sourceFile);
                }

                FileTools.CheckDirAndCreateWhenNeeded(destinationDirectory);

                try
                {
                    using (ZipInputStream zipStream = new ZipInputStream(File.Open(sourceFile, FileMode.Open,
                               FileAccess.Read, FileShare.Read)))
                    {
                        zipStream.Password = password;
                        ZipEntry zipEntry = zipStream.GetNextEntry();

                        while (zipEntry != null)
                        {
                            if (zipEntry.IsDirectory) //如果是文件夹则创建
                            {
                                var path = Path.Combine(destinationDirectory, Path.GetDirectoryName(zipEntry.Name));

                                FileTools.CheckDirAndCreateWhenNeeded(path);
                            }
                            else
                            {
                                string fileName = Path.GetFileName(zipEntry.Name);
                                if (!string.IsNullOrEmpty(fileName) && fileName.Trim().Length > 0)
                                {
                                    var path = Path.Combine(destinationDirectory, zipEntry.Name);
                                    if (File.Exists(path))
                                    {
                                        if (coverFile)
                                        {
                                            FileTools.SafeDeleteFile(path);
                                        }
                                        else
                                        {
                                            zipEntry = zipStream.GetNextEntry();
                                            continue;
                                        }
                                    }

                                    if (!File.Exists(path))
                                    {
                                        FileInfo fileItem = new FileInfo(path);
                                        using (FileStream writeStream = fileItem.Create())
                                        {
                                            byte[] buffer = new byte[BufferSize];
                                            int readLength = 0;

                                            do
                                            {
                                                readLength = zipStream.Read(buffer, 0, BufferSize);
                                                writeStream.Write(buffer, 0, readLength);
                                            } while (readLength == BufferSize);

                                            writeStream.Flush();
                                            writeStream.Close();
                                        }
                                    }
                                }
                            }

                            zipEntry = zipStream.GetNextEntry(); //获取下一个文件
                        }

                        zipStream.Close();
                    }

                    result = true;
                }
                catch (System.Exception ex)
                {
                    GC.Collect();
                    LogF8.LogError("文件解压发生错误：" + ex);
                    return result;
                }
                LogF8.LogUtil("Zip解压完成：" + sourceFile);
                GC.Collect();
                return result;
            }

            public static IEnumerator UnZipFileCoroutine(string sourceFile, string destinationDirectory = null,
                string password = null, bool coverFile = false)
            {
                if (!File.Exists(sourceFile))
                {
                    LogF8.LogError("要解压的文件不存在：" + sourceFile);
                    yield break;
                }

                if (string.IsNullOrWhiteSpace(destinationDirectory))
                {
                    destinationDirectory = Path.GetDirectoryName(sourceFile);
                }

                FileTools.CheckDirAndCreateWhenNeeded(destinationDirectory);

                using (ZipInputStream zipStream = new ZipInputStream(File.Open(sourceFile, FileMode.Open,
                           FileAccess.Read, FileShare.Read)))
                {
                    zipStream.Password = password;
                    ZipEntry zipEntry = zipStream.GetNextEntry();

                    while (zipEntry != null)
                    {
                        if (zipEntry.IsDirectory) //如果是文件夹则创建
                        {
                            var path = Path.Combine(destinationDirectory, Path.GetDirectoryName(zipEntry.Name));

                            FileTools.CheckDirAndCreateWhenNeeded(path);
                        }
                        else
                        {
                            string fileName = Path.GetFileName(zipEntry.Name);
                            if (!string.IsNullOrEmpty(fileName) && fileName.Trim().Length > 0)
                            {
                                var path = Path.Combine(destinationDirectory, zipEntry.Name);
                                if (File.Exists(path))
                                {
                                    if (coverFile)
                                    {
                                        FileTools.SafeDeleteFile(path);
                                    }
                                    else
                                    {
                                        zipEntry = zipStream.GetNextEntry();
                                        continue;
                                    }
                                }

                                if (!File.Exists(path))
                                {
                                    try
                                    {
                                        FileInfo fileItem = new FileInfo(path);
                                        using (FileStream writeStream = fileItem.Create())
                                        {
                                            byte[] buffer = new byte[BufferSize];
                                            int readLength = 0;

                                            do
                                            {
                                                readLength = zipStream.Read(buffer, 0, BufferSize);
                                                writeStream.Write(buffer, 0, readLength);
                                            } while (readLength == BufferSize);

                                            writeStream.Flush();
                                            writeStream.Close();
                                        }
                                    }
                                    catch (System.Exception ex)
                                    {
                                        LogF8.LogError("文件解压发生错误：" + ex);
                                    }
                                    // 解压一个等待一帧，注意耗时
                                    yield return null;
                                }
                            }
                        }

                        zipEntry = zipStream.GetNextEntry(); //获取下一个文件
                    }

                    zipStream.Close();
                }
                LogF8.LogUtil("Zip解压完成：" + sourceFile);
                GC.Collect();
            }
            
            public static async Task UnZipFileAsync(string sourceFile, string destinationDirectory = null,
                string password = null, bool coverFile = false)
            {
                await Task.Run(() =>
                {
                    UnZipFile(sourceFile, destinationDirectory, password, coverFile);
                });
            }

            //压缩文件和文件夹
            public static bool Zip(string[] pFileOrDirArray, //需要压缩的文件和文件夹
                string pZipFilePath, //输出的zip文件完整路径
                string pPassword = null, //密码
                IZipCallback callback = null, //回调
                int pZipLevel = 6) //压缩等级
            {
                if (null == pFileOrDirArray)
                {
                    callback?.OnFinished("输入路径为空");
                    return false;
                }

                if (string.IsNullOrEmpty(pZipFilePath))
                {
                    callback?.OnFinished("输出路径为空");
                    return false;
                }
                
                var zipOutputStream = new ZipOutputStream(File.Create(pZipFilePath));
                zipOutputStream.SetLevel(pZipLevel); // 6 压缩质量和压缩速度的平衡点
                zipOutputStream.Password = pPassword;

                foreach (string fileOrDirectory in pFileOrDirArray)
                {
                    var result = false;

                    if (Directory.Exists(fileOrDirectory))
                        result = ZipDirectory(fileOrDirectory, string.Empty, zipOutputStream, callback);
                    else if (File.Exists(fileOrDirectory))
                        result = ZipFile(fileOrDirectory, string.Empty, zipOutputStream, callback);

                    if (!result)
                    {
                        GC.Collect();
                        callback?.OnFinished($"压缩失败：{fileOrDirectory}");
                        return false;
                    }
                }

                zipOutputStream.Finish();
                zipOutputStream.Close();
                zipOutputStream = null;

                GC.Collect();
                callback?.OnFinished($"压缩完成：{string.Join("，", pFileOrDirArray)}");
                return true;
            }

            //压缩文件
            private static bool ZipFile(string pFileName, //需要压缩的文件名
                string pParentPath, //相对路径
                ZipOutputStream pZipOutputStream, //压缩输出流
                IZipCallback callback = null) //回调
            {
                ZipEntry entry = null;
                FileStream fileStream = null;
                try
                {
                    string path = pParentPath + Path.GetFileName(pFileName);
                    entry = new ZipEntry(path) { DateTime = DateTime.Now };

                    if (null != callback && !callback.OnPreZip(entry))
                        return true; // 过滤

                    fileStream = File.OpenRead(pFileName);
                    var buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    fileStream.Close();

                    entry.Size = buffer.Length;

                    pZipOutputStream.PutNextEntry(entry);
                    pZipOutputStream.Write(buffer, 0, buffer.Length);
                }
                catch (Exception e)
                {
                    LogF8.LogError("压缩文件夹失败：" + e);
                    return false;
                }
                finally
                {
                    if (null != fileStream)
                    {
                        fileStream.Close();
                        fileStream.Dispose();
                    }
                }

                callback?.OnPostZip(entry);

                return true;
            }

            //压缩文件夹
            private static bool ZipDirectory(string pDirPath, //文件夹路径
                string pParentPath, //相对路径
                ZipOutputStream pZipOutputStream, //压缩输出流
                IZipCallback callback = null) //回调
            {
                ZipEntry entry = null;
                string path = Path.Combine(pParentPath, GetDirName(pDirPath));
                try
                {
                    entry = new ZipEntry(path)
                    {
                        DateTime = DateTime.Now,
                        Size = 0
                    };

                    if (null != callback && !callback.OnPreZip(entry))
                        return true; // 过滤

                    pZipOutputStream.PutNextEntry(entry);
                    pZipOutputStream.Flush();

                    var files = Directory.GetFiles(pDirPath);
                    foreach (string file in files)
                        ZipFile(file, Path.Combine(pParentPath, GetDirName(pDirPath)), pZipOutputStream, callback);
                }
                catch (Exception e)
                {
                    LogF8.LogError("压缩文件夹失败：" + e);
                    return false;
                }

                var directories = Directory.GetDirectories(pDirPath);
                foreach (string dir in directories)
                    if (!ZipDirectory(dir, Path.Combine(pParentPath, GetDirName(pDirPath)), pZipOutputStream, callback))
                        return false;

                callback?.OnPostZip(entry);

                return true;
            }

            private static string GetDirName(string pPath)
            {
                if (!Directory.Exists(pPath))
                    return string.Empty;

                pPath = pPath.Replace("\\", "/");
                var ss = pPath.Split('/');
                if (string.IsNullOrEmpty(ss[ss.Length - 1]))
                    return ss[ss.Length - 2] + "/";
                return ss[ss.Length - 1] + "/";
            }
        }
    }
}