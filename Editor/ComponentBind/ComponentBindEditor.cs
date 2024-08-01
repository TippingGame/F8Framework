using UnityEditor;
using UnityEngine;

namespace F8Framework.Core.Editor
{
    [CustomEditor(typeof(ComponentBind), true)]  // 添加 true 参数以启用绘制基类的属性
    public class ComponentBindEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            
            foreach (var o in targets)
            {
                var targetObject = (ComponentBind)o;
                if (GUILayout.Button("组件绑定（可能需要点击两次）", GUILayout.Height(50)))
                {
                    (targetObject).Bind();
                }
            }
        }
    }
}