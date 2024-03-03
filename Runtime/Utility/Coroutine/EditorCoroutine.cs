using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace F8Framework.Core
{
    public class EditorCoroutine
    {
        private readonly IEnumerator routine;

        object current;

        bool IsDone;

        public static EditorCoroutine Start(IEnumerator enumerator)
        {
            EditorCoroutine coroutine = new EditorCoroutine(enumerator);
            coroutine.Start();
            return coroutine;
        }

        public EditorCoroutine(IEnumerator enumerator)
        {
            routine = enumerator;
        }

        private void Start()
        {
            IsDone = false;
#if UNITY_EDITOR
            EditorApplication.update += Update;
#endif
        }

        public void Stop()
        {
            IsDone = true;
#if UNITY_EDITOR
            EditorApplication.update -= Update;
#endif
        }

        private void Update()
        {
            if (MoveNext() == false)
            {
                Stop();
            }
        }

        static Stack<IEnumerator> enumeratorStack = new Stack<IEnumerator>(32);

        private bool MoveNext()
        {
            IEnumerator enumerator = routine;

            var root = enumerator;

            while (enumerator.Current as IEnumerator != null)
            {
                enumeratorStack.Push(enumerator);
                enumerator = enumerator.Current as IEnumerator;
            }

            current = enumerator.Current;
            var result = CheckMoveNext(enumerator);

            while (enumeratorStack.Count > 1)
            {
                if (result == false)
                {
                    result = enumeratorStack.Pop().MoveNext();
                }
                else
                {
                    enumeratorStack.Clear();
                }
            }

            if (enumeratorStack.Count > 0 && !result && root == enumeratorStack.Pop())
            {
                result = root.MoveNext();
            }

            return result;
        }

        private bool CheckMoveNext(IEnumerator enumerator)
        {
            if (current is EditorCoroutine)
            {
                if ((current as EditorCoroutine).IsDone == true)
                {
                    return enumerator.MoveNext();
                }
            }
            else if (current is UnityEngine.AsyncOperation)
            {
                if ((current as UnityEngine.AsyncOperation).isDone == true)
                {
                    return enumerator.MoveNext();
                }
            }
            else
            {
                return enumerator.MoveNext();
            }

            return true;
        }
    }
}