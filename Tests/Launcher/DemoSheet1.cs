/*Auto create
Don't Edit it*/

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;
using UnityEngine;

namespace F8Framework.Tests
{
	[Serializable]
	internal class DemoSheet1Item
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
		[Preserve]
		public DemoSheet1.MyEnum meiju;
	}
	
	[Serializable]
	internal class DemoSheet1
	{
		[Preserve]
		public Dictionary<System.Int32, DemoSheet1Item> Dict = new Dictionary<System.Int32, DemoSheet1Item>();
		[Preserve]
		public enum MyEnum : System.Byte
		{
			Value1 = 1,
			Value2 = 2,
			Value3 = 4,
			Value4 = 8,
		}
	}
}
