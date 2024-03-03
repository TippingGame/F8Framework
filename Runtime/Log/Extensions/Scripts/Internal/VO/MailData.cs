using System;

namespace F8Framework.Core
{
    [Serializable]
    public class MailData
    {
        public string to = "to@mail.com";
        public string userName = "username@mail.com";
        public string userPassword = "password";
        public string smtpHost = "smtp.gmail.com";
        public int smtpPort = 587;
        public string[] cc = null;
    }
}