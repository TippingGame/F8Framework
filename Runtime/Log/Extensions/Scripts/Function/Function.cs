using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace F8Framework.Core
{
    public class Function : Singleton<Function>
    {
        public interface IAdapter
        {
            void Initialize();

            void Clear();
        }

        public class CommandData
        {
            public string       methodName  = string.Empty;
            public object       instance    = null;
            public Type         type        = null;
            public MethodInfo   methodInfo  = null;
            public object[]     parameters  = null;
        }
        
        private List<IAdapter>              adapters            = null;
        private List<CommandData>           commands            = null;
        private Action<string>              cheatKeyCallback    = null;
        private MailData                    mailData            = null;

        public Function()
        {
            adapters = new List<IAdapter>();
            commands = new List<CommandData>();
        }
        
        public void Initialize()
        {
            foreach(var adapter in adapters)
            {
                adapter.Initialize();
            }
        }

        public void Clear()
        {
            foreach (var adapter in adapters)
            {
                adapter.Clear();
            }
        }

        public void AddAdapter(IAdapter adapter)
        {
            adapters.Add(adapter);
        }

        public List<IAdapter> GetAdapters()
        {
            return adapters;
        }

        public void AddCommand(object instance, string methodName, object[] parameters = null)
        {
            CommandData command = new CommandData
            {
                instance    = instance,
                methodName  = methodName,
                type        = instance.GetType(),
                parameters  = parameters
            };
            command.methodInfo  = command.type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);

            commands.Add(command);            
        }

        public List<CommandData> GetCommandDatas()
        {
            return commands;
        }

        public void AddCheatKeyCallback(Action<string> callback)
        {
            cheatKeyCallback += callback;
        }

        public void RemoveCheatKeyCallback(Action<string> callback)
        {
            cheatKeyCallback -= callback;
        }

        public void InvokeCheatKey(string cheatKey)
        {
            if(cheatKeyCallback != null)
            {
                cheatKeyCallback(cheatKey);
            }
        }

        public void CopyToClipboard(string message)
        {
            TextEditor textEditor   = new TextEditor();
            textEditor.text         = message;
            textEditor.OnFocus();
            textEditor.Copy();
        }

        public void SetMailData(MailData data)
        {
            mailData = data;
        }

        public void SendMail(string subject, string body, FileStream attachment, string attachmentName, SendCompletedEventHandler callback)
        {
            try
            {
                using (MailMessage mailMessage = new MailMessage(mailData.userName, mailData.to))
                {
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;

                    for (int index = 0; index < mailData.cc.Length; ++index)
                    {
                        mailMessage.CC.Add(mailData.cc[index]);
                    }

                    if (attachment != null)
                    {
                        mailMessage.Attachments.Add(new Attachment(attachment, attachmentName));
                    }

                    SmtpClient smtpClient = new SmtpClient(mailData.smtpHost, mailData.smtpPort);
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(mailData.userName, mailData.userPassword, mailData.smtpHost) as ICredentialsByHost;
                    ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };                    
                    smtpClient.SendCompleted += callback;
                    smtpClient.SendAsync(mailMessage, string.Empty);
                }
            }
            catch (Exception exception)
            {
                callback(null, new AsyncCompletedEventArgs(exception, true, string.Empty));
            }
        }
    }
}