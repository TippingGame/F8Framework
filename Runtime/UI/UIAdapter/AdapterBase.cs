using UnityEngine;

namespace F8Framework.Core
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public abstract class AdapterBase : MonoBehaviour
    {
        public abstract void Adapt();
    }
}