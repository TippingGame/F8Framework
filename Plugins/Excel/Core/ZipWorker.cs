namespace Excel.Core
{
    using Excel.Log;
    using ICSharpCode.SharpZipLib.Zip;
    using System;
    using System.IO;

    public class ZipWorker : IDisposable
    {
        private const string TMP = "TMP_Z";
        private const string FOLDER_xl = "xl";
        private const string FOLDER_worksheets = "worksheets";
        private const string FILE_sharedStrings = "sharedStrings.{0}";
        private const string FILE_styles = "styles.{0}";
        private const string FILE_workbook = "workbook.{0}";
        private const string FILE_sheet = "sheet{0}.{1}";
        private const string FOLDER_rels = "_rels";
        private const string FILE_rels = "workbook.{0}.rels";
        private byte[] buffer;
        private bool disposed;
        private bool _isCleaned;
        private string _tempPath;
        private string _tempEnv = Path.GetTempPath();
        private string _exceptionMessage;
        private string _xlPath;
        private string _format = "xml";
        private bool _isValid;

        private bool CheckFolderTree()
        {
            this._xlPath = Path.Combine(this._tempPath, "xl");
            return (Directory.Exists(this._xlPath) && (Directory.Exists(Path.Combine(this._xlPath, "worksheets")) && (File.Exists(Path.Combine(this._xlPath, "workbook.{0}")) && File.Exists(Path.Combine(this._xlPath, "styles.{0}")))));
        }

        private void CleanFromTemp(bool catchIoError)
        {
            if (!string.IsNullOrEmpty(this._tempPath))
            {
                this._isCleaned = true;
                try
                {
                    if (Directory.Exists(this._tempPath))
                    {
                        Directory.Delete(this._tempPath, true);
                    }
                }
                catch (IOException)
                {
                    if (!catchIoError)
                    {
                        throw;
                    }
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing && !this._isCleaned)
                {
                    this.CleanFromTemp(false);
                }
                this.buffer = null;
                this.disposed = true;
            }
        }

        public bool Extract(Stream fileStream)
        {
            if (fileStream == null)
            {
                return false;
            }
            this.CleanFromTemp(false);
            this.NewTempPath();
            this._isValid = true;
            ZipFile zipFile = null;
            try
            {
                zipFile = new ZipFile(fileStream);
                foreach (ZipEntry entry in zipFile)
                {
                    this.ExtractZipEntry(zipFile, entry);
                }
            }
            catch (Exception exception)
            {
                this._isValid = false;
                this._exceptionMessage = exception.Message;
                this.CleanFromTemp(true);
            }
            finally
            {
                fileStream.Close();
                if (zipFile != null)
                {
                    zipFile.Close();
                }
            }
            return (this._isValid ? this.CheckFolderTree() : false);
        }

        private void ExtractZipEntry(ZipFile zipFile, ZipEntry entry)
        {
            if (entry.IsCompressionMethodSupported() && !string.IsNullOrEmpty(entry.Name))
            {
                string path = Path.Combine(this._tempPath, entry.Name);
                string str2 = entry.IsDirectory ? path : Path.GetDirectoryName(Path.GetFullPath(path));
                if (!Directory.Exists(str2))
                {
                    Directory.CreateDirectory(str2);
                }
                if (entry.IsFile)
                {
                    using (FileStream stream = File.Create(path))
                    {
                        this.buffer ??= new byte[0x1000];
                        using (Stream stream2 = zipFile.GetInputStream(entry))
                        {
                            int num;
                            while ((num = stream2.Read(this.buffer, 0, this.buffer.Length)) > 0)
                            {
                                stream.Write(this.buffer, 0, num);
                            }
                        }
                        stream.Flush();
                    }
                }
            }
        }

        ~ZipWorker()
        {
            this.Dispose(false);
        }

        public Stream GetSharedStringsStream() => 
            GetStream(Path.Combine(this._xlPath, $"sharedStrings.{this._format}"));

        private static Stream GetStream(string filePath) => 
            !File.Exists(filePath) ? null : File.Open(filePath, FileMode.Open, FileAccess.Read);

        public Stream GetStylesStream() => 
            GetStream(Path.Combine(this._xlPath, $"styles.{this._format}"));

        public Stream GetWorkbookRelsStream() => 
            GetStream(Path.Combine(this._xlPath, Path.Combine("_rels", $"workbook.{this._format}.rels")));

        public Stream GetWorkbookStream() => 
            GetStream(Path.Combine(this._xlPath, $"workbook.{this._format}"));

        public Stream GetWorksheetStream(int sheetId) => 
            GetStream(Path.Combine(Path.Combine(this._xlPath, "worksheets"), $"sheet{sheetId}.{this._format}"));

        public Stream GetWorksheetStream(string sheetPath)
        {
            if (sheetPath.StartsWith("/xl/"))
            {
                sheetPath = sheetPath.Substring(4);
            }
            return GetStream(Path.Combine(this._xlPath, sheetPath));
        }

        private void NewTempPath()
        {
            this._tempPath = Path.Combine(UnityEngine.Application.persistentDataPath, "TMP_Z" + "_ExcelCache");
            this._isCleaned = false;
            object[] formatting = new object[] { this._tempPath };
            LogManager.Log<ZipWorker>(this).Debug("Using temp path {0}", formatting);
            Directory.CreateDirectory(this._tempPath);
        }

        public bool IsValid =>
            this._isValid;

        public string TempPath =>
            this._tempPath;

        public string ExceptionMessage =>
            this._exceptionMessage;
    }
}

