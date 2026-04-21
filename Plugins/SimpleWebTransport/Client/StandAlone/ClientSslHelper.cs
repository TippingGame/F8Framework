using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace JamesFrowen.SimpleWeb
{
    class ClientSslHelper
    {
        readonly bool allowErrors;

        public ClientSslHelper(bool allowErrors)
        {
            this.allowErrors = allowErrors;
        }

        internal bool TryCreateStream(Connection conn, Uri uri)
        {
            NetworkStream stream = conn.client.GetStream();
            if (uri.Scheme != "wss")
            {
                conn.stream = stream;
                return true;
            }

            try
            {
                conn.stream = CreateStream(stream, uri);
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Create SSLStream Failed: {e}", false);
                return false;
            }
        }

        Stream CreateStream(NetworkStream stream, Uri uri)
        {
            SslStream sslStream = new SslStream(stream, true, ValidateServerCertificate);
            sslStream.AuthenticateAsClient(uri.Host);
            return sslStream;
        }

        bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // only accept if no errors
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            if (allowErrors)
            {
                Log.Error($"Cert had Errors {sslPolicyErrors}, but allowErrors is true");
                return true;
            }

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }
    }
}
