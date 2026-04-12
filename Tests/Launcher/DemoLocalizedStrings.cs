/*Auto create
Don't Edit it*/

using System;
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine.Scripting;
using UnityEngine;

namespace F8Framework.Tests
{
	[Serializable]
	internal class LocalizedStringsItem
	{
		[Preserve]
		public System.Int32 id;
		[Preserve]
		public System.String TextID;
		[Preserve]
		public System.String ChineseSimplified;
		[Preserve]
		public System.String English;
	}
	
	[Serializable]
	internal class LocalizedStrings
	{
		[Preserve]
		public static void PreRegister()
		{
			TypeHandlerFactory.PreRegister<F8Framework.Tests.LocalizedStringsItem>(new F8Framework.Core.ObjectHandler<F8Framework.Tests.LocalizedStringsItem>());
			TypeHandlerFactory.PreRegister<System.Collections.Generic.Dictionary<System.Int32, F8Framework.Tests.LocalizedStringsItem>>(new F8Framework.Core.DictionaryHandler<System.Int32, F8Framework.Tests.LocalizedStringsItem>());
			TypeHandlerFactory.PreRegister<F8Framework.Tests.LocalizedStrings>(new F8Framework.Core.ObjectHandler<F8Framework.Tests.LocalizedStrings>());
		}
		[Preserve]
		public Dictionary<System.Int32, F8Framework.Tests.LocalizedStringsItem> Dict = new Dictionary<System.Int32, F8Framework.Tests.LocalizedStringsItem>();
	}
}
