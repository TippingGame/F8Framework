using System;
using System.Linq;

#if UNITY_EDITOR_WIN
using Microsoft.Win32;
using System.Text;
#elif UNITY_EDITOR_OSX
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
#elif UNITY_EDITOR_LINUX
using System.IO;
using System.Xml;
using System.Xml.Linq;
#endif

namespace F8Framework.Core.Editor
{
    public abstract class PreferanceStorageAccessor
    {
        protected string prefPath;
        protected string[] cachedData = new string[0];

        protected abstract void FetchKeysFromSystem();

        protected PreferanceStorageAccessor(string pathToPrefs)
        {
            prefPath = pathToPrefs;
        }

        public string[] GetKeys(bool reloadData = true)
        {
            if (reloadData || cachedData.Length == 0)
            {
                FetchKeysFromSystem();
            }

            return cachedData;
        }

        public Action PrefEntryChangedDelegate;
        protected bool ignoreNextChange = false;

        public void IgnoreNextChange()
        {
            ignoreNextChange = true;
        }

        protected virtual void OnPrefEntryChanged()
        {
            if (ignoreNextChange)
            {
                ignoreNextChange = false;
                return;
            }

            PrefEntryChangedDelegate();
        }

        public Action StartLoadingDelegate;
        public Action StopLoadingDelegate;

        public abstract void StartMonitoring();
        public abstract void StopMonitoring();
        public abstract bool IsMonitoring();
    }

#if UNITY_EDITOR_WIN

    public class WindowsPrefStorage : PreferanceStorageAccessor
    {
        RegistryMonitor monitor;

        public WindowsPrefStorage(string pathToPrefs) : base(pathToPrefs)
        {
            monitor = new RegistryMonitor(RegistryHive.CurrentUser, prefPath);
            monitor.RegChanged += new EventHandler(OnRegChanged);
        }

        private void OnRegChanged(object sender, EventArgs e)
        {
            OnPrefEntryChanged();
        }

        protected override void FetchKeysFromSystem()
        {
            cachedData = new string[0];

            using (RegistryKey rootKey = Registry.CurrentUser.OpenSubKey(prefPath))
            {
                if (rootKey != null)
                {
                    cachedData = rootKey.GetValueNames();
                    rootKey.Close();
                }
            }

            // Clean <key>_h3320113488 nameing
            cachedData = cachedData.Select((key) => { return key.Substring(0, key.LastIndexOf("_h", StringComparison.Ordinal)); }).ToArray();

            EncodeAnsiInPlace();
        }

        public override void StartMonitoring()
        {
            monitor.Start();
        }

        public override void StopMonitoring()
        {
            monitor.Stop();
        }

        public override bool IsMonitoring()
        {
            return monitor.IsMonitoring;
        }

        private void EncodeAnsiInPlace()
        {
            Encoding utf8 = Encoding.UTF8;
            Encoding ansi = Encoding.GetEncoding(1252);

            for (int i = 0; i < cachedData.Length; i++)
            {
                cachedData[i] = utf8.GetString(ansi.GetBytes(cachedData[i]));
            }
        }
    }

#elif UNITY_EDITOR_LINUX

    public class LinuxPrefStorage : PreferanceStorageAccessor
    {
        FileSystemWatcher fileWatcher;

        public LinuxPrefStorage(string pathToPrefs) : base(Path.Combine(Environment.GetEnvironmentVariable("HOME"), pathToPrefs))
        {
            fileWatcher = new FileSystemWatcher();
            fileWatcher.Path = Path.GetDirectoryName(prefPath);
            fileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            fileWatcher.Filter = "prefs";

            fileWatcher.Changed += OnWatchedFileChanged;
        }

        protected override void FetchKeysFromSystem()
        {
            cachedData = new string[0];

            if (File.Exists(prefPath))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                XmlReader reader = XmlReader.Create(prefPath, settings);

                XDocument doc = XDocument.Load(reader);

                cachedData = doc.Element("unity_prefs").Elements().Select((e) => e.Attribute("name").Value).ToArray();
            }
        }

        public override void StartMonitoring()
        {
            fileWatcher.EnableRaisingEvents = true;
        }

        public override void StopMonitoring()
        {
            fileWatcher.EnableRaisingEvents = false;
        }

        public override bool IsMonitoring()
        {
            return fileWatcher.EnableRaisingEvents;
        }

        private void OnWatchedFileChanged(object source, FileSystemEventArgs e)
        {
            OnPrefEntryChanged();
        }
    }

#elif UNITY_EDITOR_OSX

    public class MacPrefStorage : PreferanceStorageAccessor
    {
        private FileSystemWatcher fileWatcher;
        private DirectoryInfo prefsDirInfo;
        private String prefsFileNameWithoutExtension;

        public MacPrefStorage(string pathToPrefs) : base(Path.Combine(Environment.GetEnvironmentVariable("HOME"), pathToPrefs))
        {
            prefsDirInfo = new DirectoryInfo(Path.GetDirectoryName(prefPath));
            prefsFileNameWithoutExtension = Path.GetFileNameWithoutExtension(prefPath);

            fileWatcher = new FileSystemWatcher();
            fileWatcher.Path = Path.GetDirectoryName(prefPath);
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileWatcher.Filter = Path.GetFileName(prefPath);

            // MAC delete the old and create a new file instead of updating
            fileWatcher.Created += OnWatchedFileChanged;
        }

        protected override void FetchKeysFromSystem()
        {
            // Workaround to avoid incomplete tmp phase from MAC OS
            foreach (FileInfo info in prefsDirInfo.GetFiles())
            {
                // Check if tmp PlayerPrefs file exist
                if (info.FullName.Contains(prefsFileNameWithoutExtension) && !info.FullName.EndsWith(".plist"))
                {
                    StartLoadingDelegate();
                    return;
                }
            }
            StopLoadingDelegate();

            cachedData = new string[0];

            if (File.Exists(prefPath))
            {
                string fixedPrefsPath = prefPath.Replace("\"", "\\\"").Replace("'", "\\'").Replace("`", "\\`");
                var cmdStr = string.Format(@"-p '{0}'", fixedPrefsPath);

                string stdOut = String.Empty;
                string errOut = String.Empty;

                var process = new System.Diagnostics.Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = "plutil";
                process.StartInfo.Arguments = cmdStr;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.OutputDataReceived += new DataReceivedEventHandler((sender, evt) => { stdOut += evt.Data + "\n"; });
                process.ErrorDataReceived += new DataReceivedEventHandler((sender, evt) => { errOut += evt.Data + "\n"; });

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                MatchCollection matches = Regex.Matches(stdOut, @"(?: "")(.*)(?:"" =>.*)");
                cachedData = matches.Cast<Match>().Select((e) => e.Groups[1].Value).ToArray();
            }
        }

        public override void StartMonitoring()
        {
            fileWatcher.EnableRaisingEvents = true;
        }

        public override void StopMonitoring()
        {
            fileWatcher.EnableRaisingEvents = false;
        }

        public override bool IsMonitoring()
        {
            return fileWatcher.EnableRaisingEvents;
        }

        private void OnWatchedFileChanged(object source, FileSystemEventArgs e)
        {
            OnPrefEntryChanged();
        }

    }
#endif
}
