/*Auto create
Don't Edit it*/

using System;
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine.Scripting;
using UnityEngine;

namespace F8Framework.F8ExcelDataClass
{
	[Serializable]
	public class LocalizedStringsItem
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
	public class LocalizedStrings
	{
		[Preserve]
		public static void PreRegister()
		{
			TypeHandlerFactory.PreRegister<F8Framework.F8ExcelDataClass.LocalizedStringsItem>(new F8Framework.Core.ObjectHandler<F8Framework.F8ExcelDataClass.LocalizedStringsItem>());
			TypeHandlerFactory.PreRegister<System.Collections.Generic.Dictionary<System.Int32, F8Framework.F8ExcelDataClass.LocalizedStringsItem>>(new F8Framework.Core.DictionaryHandler<System.Int32, F8Framework.F8ExcelDataClass.LocalizedStringsItem>());
			TypeHandlerFactory.PreRegister<F8Framework.F8ExcelDataClass.LocalizedStrings>(new F8Framework.Core.ObjectHandler<F8Framework.F8ExcelDataClass.LocalizedStrings>());
		}
		[Preserve]
		public Dictionary<System.Int32, F8Framework.F8ExcelDataClass.LocalizedStringsItem> Dict = new Dictionary<System.Int32, F8Framework.F8ExcelDataClass.LocalizedStringsItem>();
	}
}
