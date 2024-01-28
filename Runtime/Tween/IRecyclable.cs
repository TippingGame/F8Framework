using System;

namespace F8Framework.Core
{
	public interface IRecyclable <T>
	{
		Action<T> Recycle { get; set; }
	}

}

