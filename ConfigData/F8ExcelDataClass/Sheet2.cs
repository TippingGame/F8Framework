/*Auto create
Don't Edit it*/

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace F8Framework.F8ExcelDataClass
{
	[Serializable]
	public class Sheet2Item
	{
		[Preserve]
		public int id;
		[Preserve]
		public string name;
		[Preserve]
		public float[] price;
		[Preserve]
		public int fddfd;
		[Preserve]
		public float aaasd;
		[Preserve]
		public int[] dfdfd;
		[Preserve]
		public string[] gggaa;
	}
	
	[Serializable]
	public class Sheet2
	{
		public Dictionary<int, Sheet2Item> Dict = new Dictionary<int, Sheet2Item>();
	}
}
