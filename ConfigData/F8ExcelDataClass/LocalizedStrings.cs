/*Auto create
Don't Edit it*/

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace F8Framework.F8ExcelDataClass
{
	[Serializable]
	public class LocalizedStringsItem
	{
		[Preserve]
		public int id;
		[Preserve]
		public string TextID;
		[Preserve]
		public string ChineseSimplified;
		[Preserve]
		public string English;
	}
	
	[Serializable]
	public class LocalizedStrings
	{
		public Dictionary<int, LocalizedStringsItem> Dict = new Dictionary<int, LocalizedStringsItem>();
	}
}
