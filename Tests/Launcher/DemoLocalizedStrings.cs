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
	internal class DemoLocalizedStringsItem
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
	internal class DemoLocalizedStrings
	{
		[Preserve]
		public static void PreRegister()
		{
			TypeHandlerFactory.PreRegister<F8Framework.Tests.DemoLocalizedStringsItem>(new F8Framework.Core.ObjectHandler<F8Framework.Tests.DemoLocalizedStringsItem>());
			TypeHandlerFactory.PreRegister<System.Collections.Generic.Dictionary<System.Int32, F8Framework.Tests.DemoLocalizedStringsItem>>(new F8Framework.Core.DictionaryHandler<System.Int32, F8Framework.Tests.DemoLocalizedStringsItem>());
			TypeHandlerFactory.PreRegister<F8Framework.Tests.DemoLocalizedStrings>(new F8Framework.Core.ObjectHandler<F8Framework.Tests.DemoLocalizedStrings>());
		}
		[Preserve]
		public Dictionary<System.Int32, F8Framework.Tests.DemoLocalizedStringsItem> Dict = new Dictionary<System.Int32, F8Framework.Tests.DemoLocalizedStringsItem>();
	}
}
