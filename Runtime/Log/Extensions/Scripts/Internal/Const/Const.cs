namespace F8Framework.Core
{
    public static class LogConst
    {
        public const string DEFAULT_CATEGORY_NAME                   = "All";
        public const string CATEGORY_DELIMITER                      = "$(category)";
        public const string LOGFILE_NAME                            = "LogViewerLogForMail.txt";
        public const string LOGMAIL_SUBJECT_FORMAT                  = "[LogViewer][{0}] {1}";
        public const string LOGMAIL_BODY                            = "Log from LogViewer. Please refer to the attached file.";

        public const string MAX_SHOW_LOG_COUNT_FORMAT               = "{0}+";

        public const int    MAX_SHOW_LOG_COUNT                      = 10000;
    }

    public static class SystemViewConst
    {
        public const string MEMORY_SIZE_MB_FORMAT                   = "{0} MB";
        public const string MEMORY_SIZE_GB_FORMAT                   = "{0} GB";
        public const string RESOLUTION_FORMAT                       = "{0} x {1}";
        public const string INFROMATION_HEADING_OS                  = "Operating System";
        public const string INFROMATION_HEADING_DEVICE_MODEL        = "Device Model";
        public const string INFROMATION_HEADING_PROCESSOR_TYPE      = "Processor Type";
        public const string INFROMATION_HEADING_PROCESSOR_COUNT     = "Processor Count";
        public const string INFROMATION_HEADING_MEMORY_SIZE         = "Memory Size";
        public const string INFROMATION_HEADING_GC_MEMORY           = "GC Memory";
        public const string INFROMATION_HEADING_PLATFORM            = "Platform";
        public const string INFROMATION_HEADING_VERSION             = "Version";
        public const string INFROMATION_HEADING_SYSTEMLANGUAGE      = "System Language";
        public const string INFROMATION_HEADING_RESOLUTION          = "Resolution";
        public const string INFROMATION_HEADING_ORIENTATION         = "Orientation";
        public const string INFROMATION_HEADING_PLAYTIME            = "Play Time";
        public const string INFROMATION_HEADING_LEVELPLAYTIME       = "Level Play Time";
        public const string INFROMATION_HEADING_LEVELNAME           = "Level Name";
        public const string INFROMATION_HEADING_LOCATION            = "Location";
        public const string INFROMATION_HEADING_ACCELEROMETER       = "Accelerometer";
        public const string INFROMATION_HEADING_GYROSCOPE           = "Gyroscope";
        public const string INFROMATION_HEADING_VIBRATION           = "Vibration";
        public const string INFROMATION_HEADING_AUDIO               = "Audio";
        public const string INFROMATION_HEADING_DEVICE_NAME         = "Device Name";
        public const string INFROMATION_HEADING_DEVICE_VENDOR       = "Device Vendor";
        public const string INFROMATION_HEADING_DEVICE_VERSION      = "Device Version";
        public const string INFROMATION_HEADING_MAX_TEXTURE         = "Max Texture";
        public const string INFROMATION_HEADING_NPOT                = "NPOT";
    }

    public static class ConsoleViewConst
    {
        public const string SEND_MAIL_SENDING                       = "Sending mail...";
        public const string SEND_MAIL_CANCELED                      = "SendMail canceled";
        public const string SEND_MAIL_SUCCEEDED                     = "SendMail succeeded";
        public const string SEND_MAIL_FAILED                        = "SendMail failed : {0}";
        public const string STACKTRACE_COPY_TITLE                   = "";
        public const string STACKTRACE_COPY_MESSAGE                 = "Copy success";
    }

    public static class ViewerConst
    {
        public const int GESTURE_TOUCH_COUNT                        = 5;
        public const float GESTURE_TOUCH_TIME_INTERVAL              = 1.0f;
    }
}
