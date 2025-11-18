/*Auto create
Don't Edit it*/

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;
using UnityEngine;

namespace F8Framework.Tests
{
	[Serializable]
	internal class DemoLocalizedStringsItem
	{
		[Preserve]
		public System.Int32 id;
		[Preserve]
		public System.String TextID;
		[Preserve]
		public System.String ChineseSimplified;
		[Preserve]
		public System.String English;
	}
	
	[Serializable]
	internal class DemoLocalizedStrings
	{
		[Preserve]
		public Dictionary<System.Int32, DemoLocalizedStringsItem> Dict = new Dictionary<System.Int32, DemoLocalizedStringsItem>();
	}
}
