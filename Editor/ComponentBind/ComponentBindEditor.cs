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
                if (GUILayout.Button("按照约定名称UI组件绑定（需要点击两次）", GUILayout.Height(50)))
                {
                    (targetObject).Bind();
                }

                if (GUILayout.Button("搜索全部UI组件绑定（需要点击两次）", GUILayout.Height(50)))
                {
                    targetObject.BindAllUIComponents();
                }
            }
        }
    }
}