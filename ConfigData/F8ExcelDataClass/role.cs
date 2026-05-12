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
	public class roleItem
	{
		[Preserve]
		public System.Int32 id;
		[Preserve]
		public System.String name;
	}
	
	[Serializable]
	public class role
	{
		[Preserve]
		public static void PreRegister()
		{
			TypeHandlerFactory.PreRegister<F8Framework.F8ExcelDataClass.roleItem>(new F8Framework.Core.ObjectHandler<F8Framework.F8ExcelDataClass.roleItem>());
			TypeHandlerFactory.PreRegister<System.Collections.Generic.Dictionary<System.Int32, F8Framework.F8ExcelDataClass.roleItem>>(new F8Framework.Core.DictionaryHandler<System.Int32, F8Framework.F8ExcelDataClass.roleItem>());
			TypeHandlerFactory.PreRegister<F8Framework.F8ExcelDataClass.role>(new F8Framework.Core.ObjectHandler<F8Framework.F8ExcelDataClass.role>());
		}
		[Preserve]
		public Dictionary<System.Int32, F8Framework.F8ExcelDataClass.roleItem> Dict = new Dictionary<System.Int32, F8Framework.F8ExcelDataClass.roleItem>();
	}
}
