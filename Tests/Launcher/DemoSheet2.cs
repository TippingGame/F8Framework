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
	internal class Sheet2Item
	{
		[Preserve]
		public System.Int32 id;
		[Preserve]
		public System.String name;
		[Preserve]
		public System.Single[] price;
		[Preserve]
		public System.Int32 fddfd;
		[Preserve]
		public System.Single aaasd;
		[Preserve]
		public System.Int32[] dfdfd;
		[Preserve]
		public System.String[] gggaa;
	}
	
	[Serializable]
	internal class Sheet2
	{
		[Preserve]
		public static void PreRegister()
		{
			TypeHandlerFactory.PreRegister<System.Single[]>(new F8Framework.Core.ArrayHandler<System.Single>());
			TypeHandlerFactory.PreRegister<System.Int32[]>(new F8Framework.Core.ArrayHandler<System.Int32>());
			TypeHandlerFactory.PreRegister<System.String[]>(new F8Framework.Core.ArrayHandler<System.String>());
			TypeHandlerFactory.PreRegister<F8Framework.Tests.Sheet2Item>(new F8Framework.Core.ObjectHandler<F8Framework.Tests.Sheet2Item>());
			TypeHandlerFactory.PreRegister<System.Collections.Generic.Dictionary<System.Int32, F8Framework.Tests.Sheet2Item>>(new F8Framework.Core.DictionaryHandler<System.Int32, F8Framework.Tests.Sheet2Item>());
			TypeHandlerFactory.PreRegister<F8Framework.Tests.Sheet2>(new F8Framework.Core.ObjectHandler<F8Framework.Tests.Sheet2>());
		}
		[Preserve]
		public Dictionary<System.Int32, F8Framework.Tests.Sheet2Item> Dict = new Dictionary<System.Int32, F8Framework.Tests.Sheet2Item>();
	}
}
