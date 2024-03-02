#if RECORD_CORUTINE_STACK
namespace F8Framework.Core
{
    public class ManagedCoroutineException : System.Exception
    {
        ManagedCoroutine coroutine;
        System.Exception ex;
        public ManagedCoroutineException(ManagedCoroutine coroutine, System.Exception ex)
        {
            this.coroutine = coroutine;
            this.ex = ex;
        }
        public override string StackTrace
        {
            get
            {
                string stackText = ex.StackTrace;

                stackText += "\n";
                stackText += "===================================\n";

                stackText += ToCoroutineStackString(coroutine);

                return stackText;
            }
        }

        public override string Message
        {
            get
            {
                return ex.Message;
            }
        }

        public string ToCoroutineStackString(ManagedCoroutine coroutine, bool bEnumeratorStack = true)
        {
            string stackText = "";
            if(bEnumeratorStack)
            {
                foreach (var enumeratorStack in coroutine.enumeratorStack)
                {
                    stackText += string.Format("({0}) - {1}", enumeratorStack.count, enumeratorStack.enumerator);
                    stackText += "\n";
                }
            }

            int hideStack = 0;
            if (coroutine.parent != null)
            {
                hideStack = 2;
            }

            for (int i = 0; i < coroutine.stackTrace.GetFrames().Length - hideStack; i++)
            {
                var frame = coroutine.stackTrace.GetFrame(i);
                var method = frame.GetMethod();
                if (method != null)
                {
                    var filePath = frame.GetFileName();
                    if (string.IsNullOrEmpty(filePath))
                    {
                        stackText += string.Format("{0}:{1}()", method.DeclaringType, method.Name);
                    }
                    else
                    {
                        var projectPath = System.IO.Directory.GetCurrentDirectory();
                        if (filePath.StartsWith(projectPath))
                        {
                            filePath =
 filePath.Substring((projectPath.Length + 1), filePath.Length - (projectPath.Length + 1));
                        }

                        int line = frame.GetFileLineNumber();
                        stackText +=
 string.Format("{0}:{1}() - ( {2} : {3} )", method.DeclaringType, method.Name, filePath, line);
                    }

                    stackText += "\n";
                }
            }

            if (coroutine.parent != null)
            {
                stackText += ToCoroutineStackString(coroutine.parent, false);
            }

            return stackText;
        }
    }
}
#endif