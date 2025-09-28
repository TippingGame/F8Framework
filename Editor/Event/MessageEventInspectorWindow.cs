using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace F8Framework.Core.Editor
{
    public class MessageEventInspectorWindow : EditorWindow
    {
        private MessageManager messageManager;
        private Vector2 scrollPosition;
        private float lastRefreshTime;
        [SerializeField] private float refreshInterval = 0.5f;
        // 折叠状态
        private Dictionary<int, bool> eventFoldoutStates = new Dictionary<int, bool>();
        private bool showEventsWithListeners = true;
        private bool showEventsWithoutListeners = false;
        private string searchFilter = "";
        private bool showValidOnly = false;
        private int displayedCount = 0;
        
        [MenuItem("开发工具/事件系统监视器", false, 106)]
        public static void ShowWindow()
        {
            if (HasOpenInstances<MessageEventInspectorWindow>())
            {
                GetWindow<MessageEventInspectorWindow>("事件系统监视器").Close();
            }
            else
            {
                var window = GetWindow<MessageEventInspectorWindow>("事件系统监视器");
                // 加载保存的设置
                if (F8EditorPrefs.HasKey("MessageEventInspectorWindow_RefreshInterval"))
                {
                    window.refreshInterval = F8EditorPrefs.GetFloat("MessageEventInspectorWindow_RefreshInterval");
                }
                window.minSize = new Vector2(800f, 600f);
            }
        }

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
            lastRefreshTime = Time.realtimeSinceStartup;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
            F8EditorPrefs.SetFloat("MessageEventInspectorWindow_RefreshInterval", refreshInterval);
        }

        private void OnEditorUpdate()
        {
            if (Time.realtimeSinceStartup - lastRefreshTime > refreshInterval)
            {
                Repaint();
                lastRefreshTime = Time.realtimeSinceStartup;
            }
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("此工具仅在运行模式下可用", MessageType.Info);
                return;
            }
            
            DrawToolbar();
            
            var events = MessageManager.Instance.events;
            if (events == null || events.Count == 0)
            {
                EditorGUILayout.HelpBox("MessageManager 没有事件监听", MessageType.Info);
                return;
            }
            messageManager = MessageManager.Instance;
                
            DrawStatistics();
            DrawEventsList();
        }

        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                // 搜索框
                GUILayout.Label("搜索:", GUILayout.Width(30));
                searchFilter = GUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField, GUILayout.Width(200));
                
                // 清空按钮
                if (GUILayout.Button("清空所有", EditorStyles.toolbarButton, GUILayout.Width(60)))
                {
                    if (EditorUtility.DisplayDialog("确认清空", "确定要清空所有事件监听器吗？", "确定", "取消"))
                    {
                        messageManager.Clear();
                    }
                }
                GUILayout.FlexibleSpace();
                
                // 显示选项
                showEventsWithListeners = GUILayout.Toggle(showEventsWithListeners, "有监听", EditorStyles.toolbarButton, GUILayout.Width(60));
                showEventsWithoutListeners = GUILayout.Toggle(showEventsWithoutListeners, "无监听", EditorStyles.toolbarButton, GUILayout.Width(60));
                showValidOnly = GUILayout.Toggle(showValidOnly, "仅有效", EditorStyles.toolbarButton, GUILayout.Width(60));
                
                GUILayout.FlexibleSpace();
                
                GUILayout.Space(20);
                
                EditorGUIUtility.labelWidth = 80;
                // 刷新间隔
                refreshInterval = EditorGUILayout.FloatField("刷新间隔(秒)", refreshInterval, GUILayout.Width(150));
                refreshInterval = Mathf.Clamp(refreshInterval, 0.1f, 5f); // 限制在0.1-5秒之间
                EditorGUIUtility.labelWidth = 0; // 重置为默认值
                
                // 刷新按钮
                if (GUILayout.Button("刷新", EditorStyles.toolbarButton, GUILayout.Width(50)))
                {
                    RefreshData();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawStatistics()
        {
            int totalEvents = messageManager.events.Count;
            int totalListeners = messageManager.events.Sum(e => e.Value.Count);
            int validListeners = 0;
            int invalidListeners = 0;

            // 统计有效/无效监听器
            foreach (var eventPair in messageManager.events)
            {
                foreach (var listener in eventPair.Value)
                {
                    if (listener.EventDataShouldBeInvoked())
                        validListeners++;
                    else
                        invalidListeners++;
                }
            }

            GUILayout.BeginVertical("Box");
            {
                GUILayout.Label("事件系统统计", EditorStyles.boldLabel);
                
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label($"总事件类型: {totalEvents}", GUILayout.Width(120));
                    GUILayout.Label($"总监听器: {totalListeners}", GUILayout.Width(100));
                    GUILayout.Label($"有效: {validListeners}", GUILayout.Width(80));
                    GUILayout.Label($"无效: {invalidListeners}", GUILayout.Width(80));
                    GUILayout.Label($"调用栈: {messageManager.callStack.Count}", GUILayout.Width(80));
                    GUILayout.Label($"待触发: {messageManager.dispatchInvokes.Sum(e => e.Value.Count)}", GUILayout.Width(80));
                }
                GUILayout.EndHorizontal();

                // 显示警告信息
                if (invalidListeners > 0)
                {
                    EditorGUILayout.HelpBox($"发现 {invalidListeners} 个无效监听器（Handle为null）", MessageType.Warning);
                }

                if (messageManager.callStack.Count > 0)
                {
                    EditorGUILayout.HelpBox($"调用栈中有 {messageManager.callStack.Count} 个事件正在处理", MessageType.Info);
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawEventsList()
        {
            GUILayout.Label($"事件列表 ({GetFilteredEventsCount()} 个)", EditorStyles.boldLabel);
            
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            {
                // 按事件ID排序
                var sortedEvents = messageManager.events.OrderBy(e => e.Key).ToList();
                
                displayedCount = 0;
                
                foreach (var eventPair in sortedEvents)
                {
                    int eventId = eventPair.Key;
                    var listeners = eventPair.Value;
                    
                    // 过滤条件
                    if (!ShouldDisplayEvent(eventId, listeners))
                        continue;

                    displayedCount++;
                    DrawEventItem(eventId, listeners, displayedCount);
                }

                if (displayedCount == 0)
                {
                    GUILayout.Label("没有找到匹配的事件", EditorStyles.centeredGreyMiniLabel);
                }
            }
            GUILayout.EndScrollView();
        }

        private bool ShouldDisplayEvent(int eventId, List<IEventDataBase> listeners)
        {
            bool hasListeners = listeners.Count > 0;
            bool hasValidListeners = listeners.Any(l => l.EventDataShouldBeInvoked());
            
            // 基本显示条件
            if ((hasListeners && !showEventsWithListeners) || 
                (!hasListeners && !showEventsWithoutListeners))
                return false;
            
            // 仅显示有效条件
            if (showValidOnly && !hasValidListeners)
                return false;
            
            // 搜索过滤
            if (!string.IsNullOrEmpty(searchFilter))
            {
                string filter = searchFilter.ToLower();
                if (!eventId.ToString().Contains(filter) &&
                    !listeners.Any(l => l.LogDebugInfo().ToLower().Contains(filter)))
                    return false;
            }
            
            return true;
        }

        private int GetFilteredEventsCount()
        {
            return messageManager.events.Count(e => ShouldDisplayEvent(e.Key, e.Value));
        }

        private void DrawEventItem(int eventId, List<IEventDataBase> listeners, int itemIndex)
        {
            if (!eventFoldoutStates.ContainsKey(eventId))
            {
                eventFoldoutStates[eventId] = false;
            }

            int validCount = listeners.Count(l => l.EventDataShouldBeInvoked());
            int invalidCount = listeners.Count - validCount;

            // 为每一行添加背景色
            Rect bgRect = EditorGUILayout.BeginVertical();
            Color bgColor = itemIndex % 2 == 0
                ? new Color(0.3f, 0.3f, 0.3f, 0.2f)
                : new Color(0.4f, 0.4f, 0.4f, 0.2f);
            EditorGUI.DrawRect(bgRect, bgColor);

            GUILayout.BeginVertical("Box");
            {
                // 事件头
                GUILayout.BeginHorizontal();
                {
                    // 折叠箭头和事件ID
                    string eventTitle = $"事件 ID: {eventId}";

                    // 创建可点击的折叠区域 - 只在标题区域处理点击
                    Rect foldoutRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(false));

                    // 计算标题文本宽度
                    float textWidth = GUI.skin.label.CalcSize(new GUIContent(eventTitle)).x + 25; // 加25是为了包含箭头
                    Rect clickableRect = new Rect(foldoutRect.x, foldoutRect.y, textWidth, foldoutRect.height);

                    // 检测折叠区域点击（只在标题区域，不影响按钮）
                    if (Event.current.type == EventType.MouseDown &&
                        clickableRect.Contains(Event.current.mousePosition))
                    {
                        eventFoldoutStates[eventId] = !eventFoldoutStates[eventId];
                        Event.current.Use();
                        Repaint();
                    }

                    eventFoldoutStates[eventId] =
                        EditorGUI.Foldout(foldoutRect, eventFoldoutStates[eventId], eventTitle, true);

                    GUILayout.FlexibleSpace();

                    // 无效（红色）
                    if (invalidCount > 0)
                    {
                        GUILayout.Label($"{invalidCount} 个无效",
                            new GUIStyle(GUI.skin.label) { normal = { textColor = Color.red } });
                    }

                    // 监听器统计
                    GUILayout.Label($"{listeners.Count} 个监听器", GUILayout.Width(100));

                    // 操作按钮 - 现在这些按钮可以正常点击了
                    if (GUILayout.Button("移除所有", GUILayout.Width(80)))
                    {
                        if (EditorUtility.DisplayDialog("确认移除",
                                $"确定要移除事件 {eventId} 的所有 {listeners.Count} 个监听器吗？", "确定", "取消"))
                        {
                            messageManager.RemoveEventListener(eventId);
                        }
                    }

                    if (GUILayout.Button("触发事件", GUILayout.Width(80)))
                    {
                        messageManager.DispatchEvent(eventId);
                        Debug.Log($"触发事件: {eventId}");
                    }
                }
                GUILayout.EndHorizontal();

                // 监听器列表
                if (eventFoldoutStates[eventId] && listeners.Count > 0)
                {
                    GUILayout.BeginVertical(GUI.skin.box);
                    {
                        int displayedListeners = 0;
                        for (int i = 0; i < listeners.Count; i++)
                        {
                            if (DrawListener(listeners[i], i, eventId))
                                displayedListeners++;
                        }

                        if (displayedListeners == 0)
                        {
                            GUILayout.Label("没有匹配的监听器", EditorStyles.centeredGreyMiniLabel);
                        }
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private bool DrawListener(IEventDataBase eventData, int index, int eventId)
        {
            // 过滤无效监听器
            if (showValidOnly && !eventData.EventDataShouldBeInvoked())
                return false;

            // 搜索过滤
            if (!string.IsNullOrEmpty(searchFilter) && 
                !eventData.LogDebugInfo().ToLower().Contains(searchFilter.ToLower()))
                return false;

            bool isValid = eventData.EventDataShouldBeInvoked();
            
            // 为监听器行添加背景色
            Rect listenerRect = EditorGUILayout.BeginVertical();
            Color listenerColor = index % 2 == 0
                ? new Color(0.25f, 0.25f, 0.35f, 0.2f)
                : new Color(0.35f, 0.35f, 0.45f, 0.2f);
            EditorGUI.DrawRect(listenerRect, listenerColor);
            
            GUILayout.BeginHorizontal();
            {
                // 序号 - 保持固定宽度
                GUILayout.Label($"{index + 1}.", GUILayout.Width(20));
    
                // 监听器信息 - 移除宽度限制
                string debugInfo = eventData.LogDebugInfo();
                GUILayout.Label(debugInfo); // 移除了 GUILayout.Width(300)
    
                // Handle信息 - 已经是不限制宽度的
                string handleInfo = GetHandleInfo(eventData);
                GUILayout.Label($"    Handle: {handleInfo}");
                
                GUILayout.FlexibleSpace();
                
                // 状态标签
                GUI.color = isValid ? Color.green : Color.red;
                GUILayout.Label(isValid ? "有效" : "无效", EditorStyles.label, GUILayout.Width(40));
                GUI.color = Color.white;
                
                // 移除按钮
                if (GUILayout.Button("移除", GUILayout.Width(50)))
                {
                    RemoveListener(eventData, eventId);
                }
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
            return true;
        }

        private string GetHandleInfo(IEventDataBase eventData)
        {
            // 直接访问public字段
            if (eventData is EventData simpleEvent)
            {
                return simpleEvent.Handle == null ? "null" : simpleEvent.Handle.ToString();
            }
            else if (eventData is EventData<object[]> complexEvent)
            {
                return complexEvent.Handle == null ? "null" : complexEvent.Handle.ToString();
            }
            
            return "未知类型";
        }

        private void RemoveListener(IEventDataBase eventData, int eventId)
        {
            // 由于无法直接知道具体的Action类型，这里提供通用移除方法
            if (eventData is EventData simpleEvent && simpleEvent.Listener != null)
            {
                messageManager.RemoveEventListener(eventId, simpleEvent.Listener, simpleEvent.Handle);
                Debug.Log($"移除监听器: {simpleEvent.Listener.Method.Name}");
            }
            else if (eventData is EventData<object[]> complexEvent && complexEvent.Listener != null)
            {
                messageManager.RemoveEventListener(eventId, complexEvent.Listener, complexEvent.Handle);
                Debug.Log($"移除监听器: {complexEvent.Listener.Method.Name}");
            }
            else
            {
                Debug.LogWarning("无法移除该监听器：类型不匹配");
            }
        }

        private void RefreshData()
        {
            messageManager = MessageManager.Instance;
            if (messageManager != null)
            {
                lastRefreshTime = Time.realtimeSinceStartup;
            }
        }
    }
}