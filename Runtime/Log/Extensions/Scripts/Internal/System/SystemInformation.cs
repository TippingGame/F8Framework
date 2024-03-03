using System;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace F8Framework.Core
{
    public class SystemInformation : Singleton<SystemInformation>
    {
        public class System
        {
            public string operatingSystem = SystemInfo.operatingSystem;
            public string deviceModel = SystemInfo.deviceModel;
            public string processorType = SystemInfo.processorType;
            public int processorCount = SystemInfo.processorCount;
            public float memorySize = (float)(SystemInfo.systemMemorySize) / 1024;
            public float gcTotalMemory = 0.0f;
        }

        public class App
        {
            public RuntimePlatform platform = Application.platform;
            public string applicationVersion = Application.version;
            public SystemLanguage systemLanguage = Application.systemLanguage;
        }

        public class Display
        {
            public int width = Screen.width;
            public int height = Screen.height;
            public ScreenOrientation orientation = Screen.orientation;

            public string GetOrientationToString()
            {
                string orientationString = "-";
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
                orientationString = orientation.ToString();
#else
#endif
                return orientationString;
            }
        }

        public class Play
        {
            public float playTime = 0.0f;
            public float levelPlayTime = 0.0f;
            public string levelName = string.Empty;
        }

        public class Features
        {
            public bool supportsLocationService = SystemInfo.supportsLocationService;
            public bool supportsAccelerometer = SystemInfo.supportsAccelerometer;
            public bool supportsGyroscope = SystemInfo.supportsGyroscope;
            public bool supportsVibration = SystemInfo.supportsVibration;
            public bool supportsAudio = SystemInfo.supportsAudio;
        }

        public class Graphics
        {
            public string deviceName = SystemInfo.graphicsDeviceName;
            public string deviceVendor = SystemInfo.graphicsDeviceVendor;
            public string deviceVersion = SystemInfo.graphicsDeviceVersion;
            public float memorySize = (float)SystemInfo.graphicsMemorySize / 1024;
            public int maxTextureSize = SystemInfo.maxTextureSize;
            public NPOTSupport npotSupport = SystemInfo.npotSupport;
        }

        public class Information
        {
            public System system = new System();
            public App app = new App();
            public Display display = new Display();
            public Play play = new Play();
            public Features features = new Features();
            public Graphics graphics = new Graphics();
        }

        private Information information = new Information();

        public SystemInformation()
        {
        }

        public override string ToString()
        {
            RefreshInformation();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("=============== System Information ==============").AppendLine();

            sb.AppendLine("System");
            sb.AppendFormat("-> Operating System : {0}", information.system.operatingSystem).AppendLine();
            sb.AppendFormat("-> Device Model : {0}", information.system.deviceModel).AppendLine();
            sb.AppendFormat("-> Processor Type : {0}", information.system.processorType).AppendLine();
            sb.AppendFormat("-> Processor Count : {0}", information.system.processorCount.ToString()).AppendLine();
            sb.AppendFormat("-> Memory : {0} GB", information.system.memorySize.ToString()).AppendLine();
            sb.AppendFormat("-> GC Memory : {0} MB", information.system.gcTotalMemory.ToString()).AppendLine();
            sb.AppendLine();

            sb.AppendLine("App");
            sb.AppendFormat("-> Platform : {0}", information.app.platform.ToString()).AppendLine();
            sb.AppendFormat("-> Version : {0}", information.app.applicationVersion).AppendLine();
            sb.AppendFormat("-> System Language : {0}", information.app.systemLanguage.ToString()).AppendLine();
            sb.AppendLine();

            sb.AppendLine("Display");
            sb.AppendFormat("-> Resolution : {0} x {1}", information.display.width.ToString(),
                information.display.height.ToString()).AppendLine();
            sb.AppendFormat("-> Orientation : {0}", information.display.GetOrientationToString()).AppendLine();
            sb.AppendLine();

            sb.AppendLine("Play");
            sb.AppendFormat("-> Play Time : {0} seconds", information.play.playTime.ToString()).AppendLine();
            sb.AppendFormat("-> Level Play Time : {0} seconds", information.play.levelPlayTime.ToString()).AppendLine();
            sb.AppendFormat("-> Level Name : {0}", information.play.levelName).AppendLine();
            sb.AppendLine();

            sb.AppendLine("Features");
            sb.AppendFormat("-> Location Service : {0}", information.features.supportsLocationService.ToString())
                .AppendLine();
            sb.AppendFormat("-> Gyroscope : {0}", information.features.supportsGyroscope.ToString()).AppendLine();
            sb.AppendFormat("-> Vibration : {0}", information.features.supportsVibration.ToString()).AppendLine();
            sb.AppendFormat("-> Accelerometer : {0}", information.features.supportsAccelerometer.ToString())
                .AppendLine();
            sb.AppendFormat("-> Audio : {0}", information.features.supportsAudio.ToString()).AppendLine();
            sb.AppendLine();

            sb.AppendLine("Graphics");
            sb.AppendFormat("-> Device Name : {0}", information.graphics.deviceName).AppendLine();
            sb.AppendFormat("-> Device Vendor : {0}", information.graphics.deviceVendor).AppendLine();
            sb.AppendFormat("-> Device Version : {0}", information.graphics.deviceVersion).AppendLine();
            sb.AppendFormat("-> Memory : {0} GB", information.graphics.memorySize).AppendLine();
            sb.AppendFormat("-> Max Texture Size : {0}", information.graphics.maxTextureSize.ToString()).AppendLine();
            sb.AppendFormat("-> NPOT Support : {0}", information.graphics.npotSupport.ToString()).AppendLine();
            sb.AppendLine();

            sb.AppendLine("=================================================").AppendLine();

            return sb.ToString();
        }

        public Information GetInformation()
        {
            RefreshInformation();

            return information;
        }

        public void RefreshInformation()
        {
            information.system.gcTotalMemory = ((float)GC.GetTotalMemory(false)) / 1024 / 1024;

            information.play.playTime = Time.unscaledTime;
            information.play.levelPlayTime = Time.timeSinceLevelLoad;
            information.play.levelName = SceneManager.GetActiveScene().name;

            information.display.orientation = Screen.orientation;
            information.display.width = Screen.width;
            information.display.height = Screen.height;
        }
    }
}