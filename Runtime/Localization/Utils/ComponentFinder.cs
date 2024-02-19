using UnityEngine;

namespace F8Framework.Core
{
	public static class ComponentFinder
	{
		public static Component Find<T1>(MonoBehaviour behaviour)
			where T1 : Component
		{
			if (behaviour.GetComponent<T1>())
			{
				return behaviour.GetComponent<T1>();
			}

			return null;
		}

		public static Component Find<T1, T2>(MonoBehaviour behaviour)
			where T1 : Component
			where T2 : Component
		{
			if (behaviour.GetComponent<T1>())
			{
				return behaviour.GetComponent<T1>();
			}

			if (behaviour.GetComponent<T2>())
			{
				return behaviour.GetComponent<T2>();
			}

			return null;
		}

		public static Component Find<T1, T2, T3>(MonoBehaviour behaviour)
			where T1 : Component
			where T2 : Component
			where T3 : Component
		{
			if (behaviour.GetComponent<T1>())
			{
				return behaviour.GetComponent<T1>();
			}

			if (behaviour.GetComponent<T2>())
			{
				return behaviour.GetComponent<T2>();
			}

			if (behaviour.GetComponent<T3>())
			{
				return behaviour.GetComponent<T3>();
			}

			return null;
		}

		public static Component Find<T1, T2, T3, T4>(MonoBehaviour behaviour)
			where T1 : Component
			where T2 : Component
			where T3 : Component
			where T4 : Component
		{
			if (behaviour.GetComponent<T1>())
			{
				return behaviour.GetComponent<T1>();
			}

			if (behaviour.GetComponent<T2>())
			{
				return behaviour.GetComponent<T2>();
			}

			if (behaviour.GetComponent<T3>())
			{
				return behaviour.GetComponent<T3>();
			}

			if (behaviour.GetComponent<T4>())
			{
				return behaviour.GetComponent<T4>();
			}

			return null;
		}
	}
}
