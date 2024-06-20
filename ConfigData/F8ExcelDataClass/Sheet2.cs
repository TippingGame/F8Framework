/*Auto create
Don't Edit it*/

using System;
using System.Collections.Generic;

namespace F8Framework.F8ExcelDataClass
{
	[Serializable]
	public class Sheet2Item
	{
	public int id;
	public string name;
	public float[] price;
	public int fddfd;
	public float aaasd;
	public int[] dfdfd;
	public string[] gggaa;
	}
	
	[Serializable]
	public class Sheet2
	{
		public Dictionary<int, Sheet2Item> Dict = new Dictionary<int, Sheet2Item>();
	}
}
