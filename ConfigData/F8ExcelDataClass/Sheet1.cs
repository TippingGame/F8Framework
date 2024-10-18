/*Auto create
Don't Edit it*/

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace F8Framework.F8ExcelDataClass
{
	[Serializable]
	public class Sheet1Item
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
	public class Sheet1
	{
		public Dictionary<int, Sheet1Item> Dict = new Dictionary<int, Sheet1Item>();
	}
}
