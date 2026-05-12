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
	public class itemItem
	{
		[Preserve]
		public System.Int32 id;
		[Preserve]
		public System.String name;
	}
	
	[Serializable]
	public class item
	{
		[Preserve]
		public static void PreRegister()
		{
			TypeHandlerFactory.PreRegister<F8Framework.F8ExcelDataClass.itemItem>(new F8Framework.Core.ObjectHandler<F8Framework.F8ExcelDataClass.itemItem>());
			TypeHandlerFactory.PreRegister<System.Collections.Generic.Dictionary<System.Int32, F8Framework.F8ExcelDataClass.itemItem>>(new F8Framework.Core.DictionaryHandler<System.Int32, F8Framework.F8ExcelDataClass.itemItem>());
			TypeHandlerFactory.PreRegister<F8Framework.F8ExcelDataClass.item>(new F8Framework.Core.ObjectHandler<F8Framework.F8ExcelDataClass.item>());
		}
		[Preserve]
		public Dictionary<System.Int32, F8Framework.F8ExcelDataClass.itemItem> Dict = new Dictionary<System.Int32, F8Framework.F8ExcelDataClass.itemItem>();
	}
}
