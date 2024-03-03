using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public class SystemLogView : LogViewBase
    {
        public SystemViewInfoGroup systemInfoGroup = null;
        public SystemViewInfoGroup appInfoGroup = null;
        public SystemViewInfoGroup displayInfoGroup = null;
        public SystemViewInfoGroup playInfoGroup = null;
        public SystemViewInfoGroup featureInfoGroup = null;
        public SystemViewInfoGroup graphicsInfoGroup = null;

        private MultiLayout multiLayout = null;
        private SystemInformation.Information information = null;

        public override void SetOrientation(ScreenOrientation orientation)
        {
            if (orientation == ScreenOrientation.Portrait || orientation == ScreenOrientation.PortraitUpsideDown)
            {
                multiLayout.SelectLayout(1);
            }
            else
            {
                multiLayout.SelectLayout(0);
            }

            Refresh();
        }

        public override void InitializeView()
        {
            multiLayout = GetComponent<MultiLayout>();
            information = SystemInformation.Instance.GetInformation();

            SetSystemInfo();
            SetAppInfo();
            SetDisplayInfo();
            SetPlayInfo();
            SetFeatureInfo();
            SetGraphicsInfo();
        }

        public void Refresh()
        {
            SystemInformation.Instance.RefreshInformation();

            UpdateRefresh();
        }

        private void UpdateRefresh()
        {
            SetSystemInfo();
            SetDisplayInfo();
            SetPlayInfo();
        }

        private void SetSystemInfo()
        {
            systemInfoGroup.ClearInfo();
            systemInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_OS, information.system.operatingSystem);
            systemInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_DEVICE_MODEL, information.system.deviceModel);
            systemInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_PROCESSOR_TYPE,
                information.system.processorType);
            systemInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_PROCESSOR_COUNT,
                information.system.processorCount.ToString());
            systemInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_MEMORY_SIZE,
                string.Format(SystemViewConst.MEMORY_SIZE_GB_FORMAT, information.system.memorySize.ToString()));
            systemInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_GC_MEMORY,
                string.Format(SystemViewConst.MEMORY_SIZE_MB_FORMAT, information.system.gcTotalMemory.ToString()));
        }

        private void SetAppInfo()
        {
            appInfoGroup.ClearInfo();
            appInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_PLATFORM, information.app.platform.ToString());
            appInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_VERSION,
                information.app.applicationVersion.ToString());
            appInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_SYSTEMLANGUAGE,
                information.app.systemLanguage.ToString());
        }

        private void SetDisplayInfo()
        {
            information = SystemInformation.Instance.GetInformation();

            displayInfoGroup.ClearInfo();
            displayInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_RESOLUTION,
                string.Format(SystemViewConst.RESOLUTION_FORMAT, information.display.width,
                    information.display.height));
            displayInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_ORIENTATION,
                information.display.GetOrientationToString());
        }

        private void SetPlayInfo()
        {
            playInfoGroup.ClearInfo();
            playInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_PLAYTIME,
                information.play.playTime.ToString("F"));
            playInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_LEVELPLAYTIME,
                information.play.levelPlayTime.ToString("F"));
            playInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_LEVELNAME, information.play.levelName);
        }

        private void SetFeatureInfo()
        {
            featureInfoGroup.ClearInfo();
            featureInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_LOCATION,
                information.features.supportsLocationService == true ? "✓" : "✗");
            featureInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_ACCELEROMETER,
                information.features.supportsAccelerometer == true ? "✓" : "✗");
            featureInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_GYROSCOPE,
                information.features.supportsGyroscope == true ? "✓" : "✗");
            featureInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_VIBRATION,
                information.features.supportsVibration == true ? "✓" : "✗");
            featureInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_AUDIO,
                information.features.supportsAudio == true ? "✓" : "✗");
        }

        private void SetGraphicsInfo()
        {
            graphicsInfoGroup.ClearInfo();
            graphicsInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_DEVICE_NAME, information.graphics.deviceName);
            graphicsInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_DEVICE_VENDOR,
                information.graphics.deviceVendor);
            graphicsInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_DEVICE_VERSION,
                information.graphics.deviceVersion);
            graphicsInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_MEMORY_SIZE,
                string.Format(SystemViewConst.MEMORY_SIZE_GB_FORMAT, information.graphics.memorySize.ToString()));
            graphicsInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_MAX_TEXTURE,
                information.graphics.maxTextureSize.ToString());
            graphicsInfoGroup.AddInfo(SystemViewConst.INFROMATION_HEADING_NPOT,
                information.graphics.npotSupport.ToString());
        }
    }
}