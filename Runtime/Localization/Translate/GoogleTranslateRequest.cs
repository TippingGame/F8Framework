// Copyright (c) H. Ibrahim Penekli. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace F8Framework.Core
{
    [Serializable]
    public class GoogleTranslateRequest
    {
        public Language Source;
        public Language Target;
        public string Text;

        public GoogleTranslateRequest()
        {
        }

        public GoogleTranslateRequest(Language source, Language target, string text)
        {
            Source = source;
            Target = target;
            Text = text;
        }
    }
}
