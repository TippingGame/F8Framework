/*Auto create
Don't Edit it*/

using System;
using System.Collections.Generic;

namespace F8Framework.F8ExcelDataClass
{
	[Serializable]
	public class LocalizedStringsItem
	{
	public int id;
	public string TextID;
	public string ChineseSimplified;
	public string English;
	}
	
	[Serializable]
	public class LocalizedStrings
	{
		public Dictionary<int, LocalizedStringsItem> Dict = new Dictionary<int, LocalizedStringsItem>();
	}
}
