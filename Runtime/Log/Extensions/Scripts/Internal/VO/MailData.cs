using System;

namespace F8Framework.Core
{
    [Serializable]
    public class MailData
    {
        // 不清楚参数含义可上网学习：Unity如何发送Email邮件
        public string to = "to@mail.com";
        public string userName = "username@mail.com";
        public string userPassword = "password";
        public string smtpHost = "smtp.gmail.com";
        public int smtpPort = 587;
        public string[] cc = null;
    }
}