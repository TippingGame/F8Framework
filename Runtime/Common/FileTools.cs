using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace F8Framework.Core
{
    public class FileTools
    {
        public static void IndexNext(ref int pIndex, int pMin, int pMax)
        {
            pIndex += 1;
            if (pIndex > pMax)
                pIndex = pMin;
        }
        
        // 生成文件的MD5
        public static string CreateMd5ForFile(string filename)
        {
            try
            {
                using (FileStream file = new FileStream(filename, FileMode.Open))
                {
                    using (MD5 md5 = MD5.Create())
                    {
                        byte[] retVal = md5.ComputeHash(file);

                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < retVal.Length; i++)
                        {
                            sb.Append(retVal[i].ToString("x2"));
                        }

                        return sb.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogF8.LogError(ex);
                return "";
            }
        }
        
        /// <summary>
        /// 获取文件夹下所有文件大小
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static int GetAllFileSize(string filePath)
        {
            int sum = 0;
            if (!Directory.Exists(filePath))
            {
                return 0;
            }

            DirectoryInfo dti = new DirectoryInfo(filePath);

            FileInfo[] fi = dti.GetFiles();

            for (int i = 0; i < fi.Length; ++i )
            {
                sum += Convert.ToInt32(fi[i].Length / 1024);
            }

            DirectoryInfo[] di = dti.GetDirectories();

            if (di.Length > 0)
            {
                for (int i = 0; i < di.Length; i++)
                {
                    sum += GetAllFileSize(di[i].FullName);
                }
            }
            return sum;
        }

        /// <summary>
        /// 获取指定文件大小
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static long GetFileSize(string filePath)
        {
            long sum = 0;
            if (!File.Exists(filePath))
            {
                return 0;
            }
            else
            {
                FileInfo files = new FileInfo(filePath);
                sum += files.Length;
            }
            return sum;
        }
        
        /// <summary>
        /// 从路径的末尾向前截取指定级别的目录
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="levels"></param>
        /// <returns></returns>
        public static string TruncatePath(string fullPath, int levels)
        {
            for (int i = 0; i < levels; i++)
            {
                fullPath = Path.GetDirectoryName(fullPath);
                if (string.IsNullOrEmpty(fullPath))
                    break;
            }

            return fullPath;
        }
        
        public static bool IsLegalURI(string uri)
        {
            return !string.IsNullOrEmpty(uri) && uri.Contains("://");
        }
        
        public static bool IsLegalHTTPURI(string uri)
        {
            return !string.IsNullOrEmpty(uri) && (uri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || uri.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
        }
        
        public static string FormatToUnityPath(string path)
        {
            return path.Replace("\\", "/");
        }

        public static string FormatToSysFilePath(string path)
        {
            return path.Replace("/", "\\");
        }

        public static string GetFileExtension(string path)
        {
            return Path.GetExtension(path).ToLower();
        }

        public static string[] GetSpecifyFilesInFolder(string path, string[] extensions = null, bool exclude = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            if (extensions == null)
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            }
            else if (exclude)
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => !extensions.Contains(GetFileExtension(f))).ToArray();
            }
            else
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => extensions.Contains(GetFileExtension(f))).ToArray();
            }
        }

        public static string[] GetSpecifyFilesInFolder(string path, string pattern)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
        }

        public static string[] GetAllFilesInFolder(string path)
        {
            return GetSpecifyFilesInFolder(path);
        }

        public static string[] GetAllDirsInFolder(string path)
        {
            return Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
        }

        public static List<string> GetAllFilesName(string path, List<string> FileList)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] fil = dir.GetFiles();
            DirectoryInfo[] dii = dir.GetDirectories();
            foreach (FileInfo f in fil)
            {
                if (!f.FullName.EndsWith(".meta"))
                {
                    LogF8.Log(f.Name);
                    FileList.Add(f.Name);
                }
            }

            //获取子文件夹内的文件列表，递归遍历
            foreach (DirectoryInfo d in dii)
            {
                GetAllFilesName(d.FullName, FileList);
            }

            return FileList;
        }

        public static void CheckFileAndCreateDirWhenNeeded(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            FileInfo file_info = new FileInfo(filePath);
            DirectoryInfo dir_info = file_info.Directory;
            if (!dir_info.Exists)
            {
                Directory.CreateDirectory(dir_info.FullName);
            }
        }

        public static void CheckDirAndCreateWhenNeeded(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        public static bool SafeWriteAllBytes(string outFile, byte[] outBytes)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }

                File.WriteAllBytes(outFile, outBytes);
                return true;
            }
            catch (System.Exception ex)
            {
                LogF8.LogError(
                    string.Format("SafeWriteAllBytes failed! path = {0} with err = {1}", outFile, ex.Message));
                return false;
            }
        }

        public static bool SafeWriteAllLines(string outFile, string[] outLines)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }

                File.WriteAllLines(outFile, outLines);
                return true;
            }
            catch (System.Exception ex)
            {
                LogF8.LogError(
                    string.Format("SafeWriteAllLines failed! path = {0} with err = {1}", outFile, ex.Message));
                return false;
            }
        }

        public static bool SafeWriteAllText(string outFile, string text)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }

                File.WriteAllText(outFile, text);
                return true;
            }
            catch (System.Exception ex)
            {
                LogF8.LogError(string.Format("SafeWriteAllText failed! path = {0} with err = {1}", outFile,
                    ex.Message));
                return false;
            }
        }

        public static byte[] SafeReadAllBytes(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllBytes(inFile);
            }
            catch (System.Exception ex)
            {
                LogF8.LogError(string.Format("SafeReadAllBytes failed! path = {0} with err = {1}", inFile, ex.Message));
                return null;
            }
        }

        public static string[] SafeReadAllLines(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllLines(inFile);
            }
            catch (System.Exception ex)
            {
                LogF8.LogError(string.Format("SafeReadAllLines failed! path = {0} with err = {1}", inFile, ex.Message));
                return null;
            }
        }

        public static string SafeReadAllText(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllText(inFile);
            }
            catch (System.Exception ex)
            {
                LogF8.LogError(string.Format("SafeReadAllText failed! path = {0} with err = {1}", inFile, ex.Message));
                return null;
            }
        }

        private static void DeleteDirectory(string dirPath, string[] excludeName = null)
        {
            if (!Directory.Exists(dirPath))
            {
                return;
            }

            string[] files = Directory.GetFiles(dirPath);
            string[] dirs = Directory.GetDirectories(dirPath);

            foreach (string file in files)
            {
                bool delete = true;
                if (excludeName != null)
                {
                    foreach (string s in excludeName)
                    {
                        if (file.EndsWith(s))
                        {
                            delete = false;
                        }
                    }
                }

                if (delete)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir, excludeName);
            }

            string[] filesAfter = Directory.GetFiles(dirPath);
            string[] dirsAfter = Directory.GetDirectories(dirPath);
            if (filesAfter.Length == 0 && dirsAfter.Length == 0)
            {
                Directory.Delete(dirPath, false);
            }
        }

        public static bool SafeClearDir(string folderPath, string[] excludeName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return true;
                }

                if (Directory.Exists(folderPath))
                {
                    DeleteDirectory(folderPath, excludeName);
                }

                Directory.CreateDirectory(folderPath);
                return true;
            }
            catch (System.Exception ex)
            {
                LogF8.LogError(string.Format("SafeClearDir failed! path = {0} with err = {1}", folderPath, ex.Message));
                return false;
            }
        }

        public static bool SafeDeleteDir(string folderPath, string[] excludeName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return true;
                }

                if (Directory.Exists(folderPath))
                {
                    DeleteDirectory(folderPath, excludeName);
                }

                return true;
            }
            catch (System.Exception ex)
            {
                LogF8.LogError(string.Format("SafeDeleteDir failed! path = {0} with err: {1}", folderPath, ex.Message));
                return false;
            }
        }

        public static bool SafeDeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return true;
                }

                if (!File.Exists(filePath))
                {
                    return true;
                }

                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
                return true;
            }
            catch (System.Exception ex)
            {
                LogF8.LogError(string.Format("SafeDeleteFile failed! path = {0} with err: {1}", filePath, ex.Message));
                return false;
            }
        }

        public static bool SafeRenameFile(string sourceFileName, string destFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceFileName))
                {
                    return false;
                }

                if (!File.Exists(sourceFileName))
                {
                    return true;
                }

                SafeDeleteFile(destFileName);
                File.SetAttributes(sourceFileName, FileAttributes.Normal);
                File.Move(sourceFileName, destFileName);
                return true;
            }
            catch (System.Exception ex)
            {
                LogF8.LogError(string.Format("SafeRenameFile failed! path = {0} with err: {1}", sourceFileName,
                    ex.Message));
                return false;
            }
        }

        public static bool SafeCopyFile(string fromFile, string toFile)
        {
            try
            {
                if (string.IsNullOrEmpty(fromFile))
                {
                    return false;
                }

                if (!File.Exists(fromFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(toFile);
                SafeDeleteFile(toFile);
                File.Copy(fromFile, toFile, true);
                return true;
            }
            catch (System.Exception ex)
            {
                LogF8.LogError(string.Format("SafeCopyFile failed! formFile = {0}, toFile = {1}, with err = {2}",
                    fromFile, toFile, ex.Message));
                return false;
            }
        }

        public static bool SafeCopyDirectory(string sourceDirName, string destDirName, bool copySubDirs, string[] excludeName = null)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);
                if (dir.Exists == false)
                {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourceDirName);
                }

                DirectoryInfo[] dirs = dir.GetDirectories();
                if (Directory.Exists(destDirName) == false)
                {
                    Directory.CreateDirectory(destDirName);
                }

                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    bool copy = true;
                    if (excludeName != null)
                    {
                        foreach (string s in excludeName)
                        {
                            if (file.Name.EndsWith(s))
                            {
                                copy = false;
                            }
                        }
                    }
                    if (copy)
                    {
                        string temppath = Path.Combine(destDirName, file.Name);
                        file.CopyTo(temppath, true);
                    }
                }

                if (copySubDirs == true)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        SafeCopyDirectory(subdir.FullName, temppath, copySubDirs);
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                LogF8.LogError(string.Format("SafeCopyDirectory failed! sourceDirName = {0}, destDirName = {1}, with err = {2}",
                    sourceDirName, destDirName, ex.Message));
                return false;
            }
        }
        
        public static void MoveFile(string source, string dest, bool overwrite = true)
        {
            var directoryInfo = new FileInfo(dest).Directory;
            if (directoryInfo != null)
            {
                var targetPath = directoryInfo.FullName;

                if (Directory.Exists(targetPath) == false)
                {
                    Directory.CreateDirectory(targetPath);
                }
            }

            if (File.Exists(source) == true)
            {
                if (overwrite == true)
                {
                    if (File.Exists(dest) == true)
                    {
                        File.SetAttributes(dest, FileAttributes.Normal);
                        File.Delete(dest);
                    }
                }
                File.Move(source, dest);
            }
        }
        
        public static byte[] Encypt(byte[] targetData, byte key)
        {
            var result = new byte[targetData.Length];
            //key异或
            int dataLength = targetData.Length;
            for (int i = 0; i < dataLength; ++i)
            {
                result[i] = (byte)(targetData[i] ^ key);
            }

            return result;
        }
    }
}