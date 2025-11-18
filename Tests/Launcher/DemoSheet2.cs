/*Auto create
Don't Edit it*/

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;
using UnityEngine;

namespace F8Framework.Tests
{
	[Serializable]
	internal class DemoSheet2Item
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
	internal class DemoSheet2
	{
		[Preserve]
		public Dictionary<System.Int32, DemoSheet2Item> Dict = new Dictionary<System.Int32, DemoSheet2Item>();
	}
}
