using System;
using System.Collections;
using System.Collections.Generic;
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
            private const int BufferSize = 65536; // 64KB 缓冲区

            // 压缩回调接口
            public interface IZipCallback
            {
                bool OnPreZip(ZipEntry entry); // true 继续，false 跳过
                void OnPostZip(ZipEntry entry);
                void OnFinished(string result);
            }

            // 默认回调实现（过滤 .meta 和 .ds_store）
            public class ZipResult : IZipCallback
            {
                public bool OnPreZip(ZipEntry entry)
                {
                    if (entry.IsFile)
                    {
                        string ext = Path.GetExtension(entry.Name).ToLower();
                        if (ext == ".meta" || ext == ".ds_store")
                            return false;
                    }
                    return true;
                }

                public void OnPostZip(ZipEntry entry) { }
                public void OnFinished(string result) => LogF8.Log("Zip Finished : " + result);
            }

            #region 解压（同步版本）
            /// <summary>
            /// 解压缩 Zip 文件
            /// </summary>
            public static bool UnZipFile(string sourceFile, string destinationDirectory = null,
                string password = null, bool coverFile = false)
            {
                if (!File.Exists(sourceFile))
                {
                    LogF8.LogError("要解压的文件不存在：" + sourceFile);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(destinationDirectory))
                    destinationDirectory = Path.GetDirectoryName(sourceFile);

                Directory.CreateDirectory(destinationDirectory); // 自动创建目录

                try
                {
                    using (var zipStream = new ZipInputStream(File.Open(sourceFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                    {
                        zipStream.Password = password;
                        ZipEntry entry;
                        while ((entry = zipStream.GetNextEntry()) != null)
                        {
                            if (!ProcessEntry(entry, zipStream, destinationDirectory, coverFile))
                                continue; // 跳过（目录或已被过滤）
                        }
                    }
                    LogF8.Log("Zip解压完成：" + sourceFile);
                    return true;
                }
                catch (Exception ex)
                {
                    LogF8.LogException(ex);
                    return false;
                }
            }
            #endregion

            #region 解压（协程版本）
            public static IEnumerator UnZipFileCoroutine(string sourceFile, string destinationDirectory = null,
                string password = null, bool coverFile = false)
            {
                if (!File.Exists(sourceFile))
                {
                    LogF8.LogError("要解压的文件不存在：" + sourceFile);
                    yield break;
                }

                if (string.IsNullOrWhiteSpace(destinationDirectory))
                    destinationDirectory = Path.GetDirectoryName(sourceFile);

                Directory.CreateDirectory(destinationDirectory);

                using (var zipStream = new ZipInputStream(File.Open(sourceFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    zipStream.Password = password;
                    ZipEntry entry;
                    while ((entry = zipStream.GetNextEntry()) != null)
                    {
                        try
                        {
                            ProcessEntry(entry, zipStream, destinationDirectory, coverFile);
                        }
                        catch (Exception ex)
                        {
                            LogF8.LogException(ex);
                        }
                        yield return null; // 每处理一个文件等待一帧
                    }
                }
                LogF8.Log("Zip解压完成：" + sourceFile);
            }
            #endregion

            #region 解压（异步版本）
            public static async Task UnZipFileAsync(string sourceFile, string destinationDirectory = null,
                string password = null, bool coverFile = false)
            {
                // 使用 Task.Run 将同步解压放到线程池执行（保持非阻塞）
                await Task.Run(() => UnZipFile(sourceFile, destinationDirectory, password, coverFile));
            }
            #endregion

            #region 压缩
            /// <summary>
            /// 压缩文件和文件夹
            /// </summary>
            public static bool Zip(string[] fileOrDirArray, string zipFilePath,
                string password = null, IZipCallback callback = null, int zipLevel = 6)
            {
                if (fileOrDirArray == null || fileOrDirArray.Length == 0)
                {
                    callback?.OnFinished("输入路径为空");
                    return false;
                }
                if (string.IsNullOrEmpty(zipFilePath))
                {
                    callback?.OnFinished("输出路径为空");
                    return false;
                }

                FileTools.SafeDeleteFile(zipFilePath);
                using (var zipStream = new ZipOutputStream(File.Create(zipFilePath)))
                {
                    zipStream.SetLevel(zipLevel);
                    zipStream.Password = password;

                    foreach (string path in fileOrDirArray)
                    {
                        bool success;
                        if (Directory.Exists(path))
                            success = ZipDirectory(path, "", zipStream, callback);
                        else if (File.Exists(path))
                            success = ZipFile(path, "", zipStream, callback);
                        else
                            success = false;

                        if (!success)
                        {
                            callback?.OnFinished($"压缩失败：{path}");
                            return false;
                        }
                    }

                    zipStream.Finish();
                }

                callback?.OnFinished($"压缩完成：{zipFilePath}");
                return true;
            }

            // 压缩单个文件（流式读写）
            private static bool ZipFile(string fileName, string parentPath, ZipOutputStream zipStream, IZipCallback callback)
            {
                string entryName = parentPath + Path.GetFileName(fileName);
                var entry = new ZipEntry(entryName) { DateTime = DateTime.Now };

                if (callback != null && !callback.OnPreZip(entry))
                    return true; // 跳过

                try
                {
                    using (var fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        entry.Size = fs.Length;
                        zipStream.PutNextEntry(entry);

                        byte[] buffer = new byte[BufferSize];
                        int bytesRead;
                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            zipStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogF8.LogException(ex);
                    return false;
                }

                callback?.OnPostZip(entry);
                return true;
            }

            // 压缩文件夹（递归）
            private static bool ZipDirectory(string dirPath, string parentPath, ZipOutputStream zipStream, IZipCallback callback)
            {
                string entryName = Path.Combine(parentPath, GetDirName(dirPath)).Replace('\\', '/');
                var entry = new ZipEntry(entryName) { DateTime = DateTime.Now, Size = 0 };

                if (callback != null && !callback.OnPreZip(entry))
                    return true; // 跳过

                try
                {
                    zipStream.PutNextEntry(entry);
                    zipStream.Flush();

                    // 压缩文件夹内所有文件
                    foreach (string file in Directory.GetFiles(dirPath))
                        if (!ZipFile(file, Path.Combine(parentPath, GetDirName(dirPath)), zipStream, callback))
                            return false;

                    // 递归处理子文件夹
                    foreach (string subDir in Directory.GetDirectories(dirPath))
                        if (!ZipDirectory(subDir, Path.Combine(parentPath, GetDirName(dirPath)), zipStream, callback))
                            return false;
                }
                catch (Exception ex)
                {
                    LogF8.LogException(ex);
                    return false;
                }

                callback?.OnPostZip(entry);
                return true;
            }

            private static string GetDirName(string path)
            {
                if (!Directory.Exists(path)) return "";
                path = path.Replace("\\", "/").TrimEnd('/');
                return Path.GetFileName(path) + "/";
            }
            #endregion

            #region 私有辅助方法
            /// <summary>
            /// 处理单个 ZipEntry（解压逻辑核心）
            /// </summary>
            private static bool ProcessEntry(ZipEntry entry, ZipInputStream zipStream, string destDir, bool coverFile)
            {
                if (entry.IsDirectory)
                {
                    // 创建目录（注意防止 Zip Slip）
                    string dirPath = GetSafePath(destDir, entry.Name);
                    Directory.CreateDirectory(dirPath);
                    return false; // 目录不产生文件，返回 false 表示跳过后续文件写入
                }

                // 跳过空文件名
                string fileName = Path.GetFileName(entry.Name);
                if (string.IsNullOrEmpty(fileName))
                    return false;

                // 获取安全的最终路径
                string targetFile = GetSafePath(destDir, entry.Name);

                // 处理已存在文件
                if (File.Exists(targetFile))
                {
                    if (coverFile)
                    {
                        FileTools.SafeDeleteFile(targetFile);
                    }
                    else
                    {
                        return false;
                    }
                }

                // 确保目标目录存在
                Directory.CreateDirectory(Path.GetDirectoryName(targetFile));

                // 写入文件
                using (var fs = new FileStream(targetFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] buffer = new byte[BufferSize];
                    int bytesRead;
                    while ((bytesRead = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, bytesRead);
                    }
                }

                return true;
            }

            /// <summary>
            /// 防止 Zip Slip 漏洞：确保最终路径在目标目录内
            /// </summary>
            private static string GetSafePath(string destDir, string entryName)
            {
                string fullDest = Path.GetFullPath(Path.Combine(destDir, entryName));
                string fullDestDir = Path.GetFullPath(destDir + Path.DirectorySeparatorChar);
                if (!fullDest.StartsWith(fullDestDir, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("试图解压到目标目录之外，已阻止: " + entryName);
                return fullDest;
            }
            #endregion
        }
    }
}