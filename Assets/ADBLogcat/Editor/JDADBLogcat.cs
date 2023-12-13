//-----------------------------------------------------------------------------
//            ADBLogcat
// Copyright © 2018-2020 Jellydot Inc
//-----------------------------------------------------------------------------



using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor.Callbacks;
//-----------------------------------------------------------------------------
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using System;
using System.Runtime.InteropServices;
#endif
//-----------------------------------------------------------------------------
using UnityEditor;

namespace JellyDot_ADBLogcat
{
    //-------------------------------------------------------------------------
    //                  Logcat
    //-------------------------------------------------------------------------
    public class JDADBLogcat : EditorWindow
    {
        public enum LogType
        {
            lt_Verbose,
            lt_Debug,
            lt_Info,
            lt_Warning,
            lt_Error,
            lt_Fatal,
            lt_Silent,
        }

        public static LogType StringToLogType(string aValue)
        {
            aValue = aValue.ToLower();
            if (aValue.Contains("v/"))
                return LogType.lt_Verbose;
            if (aValue.Contains("d/"))
                return LogType.lt_Debug;
            if (aValue.Contains("i/"))
                return LogType.lt_Info;
            if (aValue.Contains("w/"))
                return LogType.lt_Warning;
            if (aValue.Contains("e/"))
                return LogType.lt_Error;
            if (aValue.Contains("f/"))
                return LogType.lt_Fatal;
            if (aValue.Contains("s/"))
                return LogType.lt_Silent;
            return LogType.lt_Info;
        }

        public static int ms_LogDateView = 1;
        public static int ms_LogTagView = 1;
        public static int ms_LogEmptyLineView = 1;
        public static string ms_ADBSDKPath = string.Empty;

        public class LogData
        {
            public string m_OriginalLog = string.Empty;
            public LogType m_Logtype = LogType.lt_Info;

            public string m_Day = string.Empty;
            public string m_Time = string.Empty;
            public string m_Tag = string.Empty;
            public string m_Log = string.Empty;

            public string m_ViewData = string.Empty; 
        }

        public static LogData StringToLogData(string aValue)
        {
            LogData result = new LogData();
            result.m_OriginalLog = aValue;
            string szKey = "):";
            int iPos = aValue.IndexOf(szKey);
            if (iPos > 0)
            {
                string szInfo = aValue.Substring(0, iPos + szKey.Length);
                string szMsg = aValue.Substring(iPos + szKey.Length, aValue.Length - (iPos + szKey.Length));
                result.m_Log = szMsg.Replace(" \r", "");

                string[] Infobuf = szInfo.Split(' ');  
                if (Infobuf.Length > 3)
                {
                    string szDay = Infobuf[0];
                    result.m_Day = szDay;
                    string szTime = Infobuf[1];
                    result.m_Time = szTime;
                    string[] szTypes = Infobuf[2].Split('/');
                    if (szTypes.Length > 1)
                    {
                        LogType elogtype = StringToLogType(szTypes[0] + "/");   //  E/
                        result.m_Logtype = elogtype;
                        string szTag = szTypes[1];
                        result.m_Tag = szTag.Replace("(", "");
                    }
                    return result;
                }
            }
            else
            {
                result = null;
            }
            return null;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (null == ms_ADBWindow)
            {
                ms_ADBWindow = (JDADBLogcat)EditorWindow.GetWindow(typeof(JDADBLogcat));
                ms_ADBWindow.titleContent = new GUIContent("ADBLogcat", "Android Logcat Plugins For Unity");
            }
            if (null != ms_ADBWindow)
                ms_ADBWindow.Clear();
            InitGUIStyle();
        }

        public static JDADBLogcat ms_ADBWindow = null;

        [MenuItem("Tools/ADB Logcat")]
        public static void Init()
        {
            ms_ADBWindow = (JDADBLogcat)EditorWindow.GetWindow(typeof(JDADBLogcat));
            ms_ADBWindow.titleContent = new GUIContent("ADBLogcat", "Android Logcat Plugins For Unity");
            ms_ADBWindow.minSize = new Vector2(820, 454);
            ms_ADBWindow.Focus();
            ms_ADBWindow.Show();
        }

        void Awake()
        {
            string szRoot = Application.dataPath;
            //  Load Option
            m_CheckTagText = PlayerPrefs.GetString(szRoot + "_ADB_CheckTagText", m_CheckTagText);
            ms_LogDateView = PlayerPrefs.GetInt(szRoot + "_ADB_LogDateView");
            ms_LogTagView = PlayerPrefs.GetInt(szRoot + "_ADB_LogTagView");
            ms_LogEmptyLineView = PlayerPrefs.GetInt(szRoot + "_ADB_LogEmptyLineView");

            LoadSDKPath();

            ms_LogError_Color = StringToColor(PlayerPrefs.GetString(szRoot + "_ADB_LogError_Color", ColorToString(ms_LogError_Color)));
            ms_LogWarn_Color = StringToColor(PlayerPrefs.GetString(szRoot + "_ADB_LogWarn_Color", ColorToString(ms_LogWarn_Color)));
            ms_LogNormal_Color = StringToColor(PlayerPrefs.GetString(szRoot + "_ADB_LogNormal_Color", ColorToString(ms_LogNormal_Color)));
            ms_LogDebug_Color = StringToColor(PlayerPrefs.GetString(szRoot + "_ADB_LogDebug_Color", ColorToString(ms_LogDebug_Color)));
            ms_FindText_Color = StringToColor(PlayerPrefs.GetString(szRoot + "_ADB_FindText_Color", ColorToString(ms_FindText_Color)));
            ms_Text_Color = StringToColor(PlayerPrefs.GetString(szRoot + "_ADB_Text_Color", ColorToString(ms_Text_Color)));
            ms_ListSelect_Color = StringToColor(PlayerPrefs.GetString(szRoot + "_ADB_ListSelect_Color", ColorToString(ms_ListSelect_Color)));

            ms_LimitLogCount = PlayerPrefs.GetInt(szRoot + "_ADB_LimitLogCount", ms_LimitLogCount);
            ms_LimitViewLogCount = PlayerPrefs.GetInt(szRoot + "_ADB_LimitViewLogCount", ms_LimitViewLogCount);

            m_LogListInfo.m_ItemHeight = 16;

            InitGUIStyle();
        }

        void LoadSDKPath()
        {
            ms_ADBSDKPath = PlayerPrefs.GetString("_ADB_SDKPath", ms_ADBSDKPath);
            if (ms_WriteLog)
                UnityEngine.Debug.Log("LoadSDKPath>>    sdkpath: " + ms_ADBSDKPath);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (!File.Exists(ms_ADBSDKPath))
            {
                if (ms_WriteLog)
                    UnityEngine.Debug.LogError("file not find   " + ms_ADBSDKPath);

                ms_ADBSDKPath = string.Empty;
                PlayerPrefs.SetString("_ADB_SDKPath", ms_ADBSDKPath);
                PlayerPrefs.Save();
            }
#endif
        }

        static bool ms_InitGUIStyle = false;
        public static void InitGUIStyle()
        {
            ms_ListSelect_GUIStyle.normal.textColor = ms_ListSelect_Color;
            ms_ListSelect_GUIStyle.alignment = TextAnchor.MiddleLeft;
            ms_ListSelect_GUIStyle.clipping = TextClipping.Clip;
            ms_ListSelect_GUIStyle.padding = new RectOffset(2, 0, -2, 0);

            ms_LogError_GUIStyle.normal.textColor = Color.red;
            ms_LogError_GUIStyle.alignment = TextAnchor.MiddleLeft;
            ms_LogError_GUIStyle.clipping = TextClipping.Clip;
            ms_LogError_GUIStyle.padding = new RectOffset(2, 0, -2, 0);

            ms_LogWarn_GUIStyle.normal.textColor = new Color(1.0f, 0.4f, 0.1f);
            ms_LogWarn_GUIStyle.alignment = TextAnchor.MiddleLeft;
            ms_LogWarn_GUIStyle.clipping = TextClipping.Clip;
            ms_LogWarn_GUIStyle.padding = new RectOffset(2, 0, -2, 0);

            ms_LogDebug_GUIStyle.normal.textColor = ms_LogDebug_Color;
            ms_LogDebug_GUIStyle.alignment = TextAnchor.MiddleLeft;
            ms_LogDebug_GUIStyle.clipping = TextClipping.Clip;
            ms_LogDebug_GUIStyle.padding = new RectOffset(2, 0, -2, 0);

            ms_LogNormal_GUIStyle.normal.textColor = ms_LogNormal_Color;
            ms_LogNormal_GUIStyle.alignment = TextAnchor.MiddleLeft;
            ms_LogNormal_GUIStyle.clipping = TextClipping.Clip;
            ms_LogNormal_GUIStyle.padding = new RectOffset(2, 0, -2, 0);

            ms_Text_GUIStyle.clipping = TextClipping.Clip;
            ms_Text_GUIStyle.wordWrap = true;
            ms_Text_GUIStyle.richText = true;
            ms_Text_GUIStyle.fontSize = 12;
            ms_Text_GUIStyle.border = new RectOffset(2, 17, 2, 17);
            ms_Text_GUIStyle.normal.textColor = ms_Text_Color;
            ms_Text_GUIStyle.padding = new RectOffset(10, 10, 3, 3);


            ms_Find_GUIStyle.clipping = TextClipping.Clip;
            ms_Find_GUIStyle.wordWrap = true;
            ms_Find_GUIStyle.richText = true;
            ms_Find_GUIStyle.fontSize = 12;
            ms_Find_GUIStyle.border = new RectOffset(2, 17, 2, 17);
            ms_Find_GUIStyle.normal.textColor = ms_FindText_Color;
            ms_Find_GUIStyle.padding = new RectOffset(10, 10, 3, 3);

            ms_InitGUIStyle = true;
        }

        public void Clear()
        {
            m_ListSelectIndex = -1;
            m_LogListIndex = 0;
            m_LogAllList.Clear();
            m_LogViewList.Clear();
            m_ViewText = string.Empty;
        }

        void OnEnable()
        {
            string szRoot = Application.dataPath;
            m_ViewVerbose = PlayerPrefs.GetInt(szRoot + "_ADB_ViewVerbose", m_ViewVerbose);
            m_ViewDebug = PlayerPrefs.GetInt(szRoot + "_ADB_ViewDebug", m_ViewDebug);
            m_ViewInfo = PlayerPrefs.GetInt(szRoot + "_ADB_ViewInfo", m_ViewInfo);
            m_ViewWarn = PlayerPrefs.GetInt(szRoot + "_ADB_ViewWarn", m_ViewWarn);
            m_ViewError = PlayerPrefs.GetInt(szRoot + "_ADB_ViewError", m_ViewError);
            m_ViewAssert = PlayerPrefs.GetInt(szRoot + "_ADB_ViewAssert", m_ViewAssert);
        }
        void OnFirst()
        {
            LoadSDKPath();
            ProcessADBDevice(CallbackFinishDeviceEvent_OnFirst);
        }

        bool m_First = false;

        void OnDisable()
        {
            m_First = false;
        }

        void OnDestroy()
        {
            m_First = false;
            ProcessADBDone();
            PlayerPrefs.Save();
            ms_ADBWindow = null;
        }

        bool m_CompliingReset = false;

        void Update()
        {
            //-----------------------------------------------------------------
            if (EditorApplication.isCompiling && !m_CompliingReset)
            {

                ms_ADBSDKPath = PlayerPrefs.GetString("_ADB_SDKPath", ms_ADBSDKPath);
                ProcessADBDone();
                m_CompliingReset = true;

            }
            if (!EditorApplication.isCompiling)
            {
                m_CompliingReset = false;
            }

            //-----------------------------------------------------------------
            if (m_Repaint)
            {
                Repaint();
                m_Repaint = false;
            }

            if (m_EventList.Count > 0)
            {
                CallbackEvent call = m_EventList[0];
                if (null != call.m_Call)
                    call.m_Call(call.m_Error);
                m_EventList.RemoveAt(0);
            }
        }

        public enum DebugOption
        {
            Verbose,
            Debug,
            Info,
            Warn,
            Error,
            Assert,
        }

        public int m_ViewVerbose = 1;
        public int m_ViewDebug = 1;
        public int m_ViewInfo = 1;
        public int m_ViewWarn = 1;
        public int m_ViewError = 1;
        public int m_ViewAssert = 1;

        string m_CheckTagText = string.Empty;

        public static int ms_LimitLogCount = 50000;
        public static int ms_LimitViewLogCount = 20000;
        List<LogData> m_LogAllList = new List<LogData>();
        List<LogData> m_LogViewList = new List<LogData>();

        GUILayoutUtil.ListInfo m_LogListInfo = new GUILayoutUtil.ListInfo();
        float m_LogListIndex = 0;
        string m_ViewText = string.Empty;

        void CallbackFinishDeviceEvent_OnFirst(string aError)
        {
            if (!string.IsNullOrEmpty(aError))
            {
                UnityEngine.Debug.Log(aError);
                if (aError.Contains("daemon started successfully"))
                {
                    ProcessADBDevice(CallbackFinishDeviceEvent_OnFirst);
                }
                else
                {
                    ShowSDKSetup();
                }

            }
        }

        public class CallbackEvent
        {
            public CallbackFinishDeviceEvent m_Call = null;
            public string m_Error = string.Empty;

            public CallbackEvent(CallbackFinishDeviceEvent aCall, string aError)
            {
                m_Call = aCall;
                m_Error = aError;
            }
        }

        List<CallbackEvent> m_EventList = new List<CallbackEvent>();

        void ShowSDKSetup()
        {
            JDADBLogcatSDKSetup.ShowSDKSetup();
        }

        void CallbackFinishDeviceEvent_ADBStart(string aError)
        {
            UnityEngine.Debug.Log(aError);
            if (!string.IsNullOrEmpty(aError))
            {
                if (aError.Contains("daemon started successfully"))
                {
                    ProcessADBDevice(CallbackFinishDeviceEvent_ADBStart);
                }
                else
                {
                    ShowSDKSetup();
                }


            }
            else
            {
                if (m_DeviceList.Count > 0)
                {
                    string szDeviceSN = GetCurrentDeviceSerialNumber();
                    ProcessADBLogcat(szDeviceSN);
                }
            }
        }


        void OnGUI()
        {
            if (!m_First)
            {
                m_First = true;
                OnFirst();
            }

            if (ms_InitGUIStyle)
            {
                GUIStyle gs = GUI.skin.FindStyle("AnimationKeyframeBackground");
                if (null != gs)
                {
                    ms_ListSelect_GUIStyle.border = gs.border;
                    ms_ListSelect_GUIStyle.normal.background = gs.normal.background;
                }

                gs = GUI.skin.FindStyle("CN Box");
                if (null != gs)
                {
                    ms_Find_GUIStyle.border = gs.border;
                    ms_Find_GUIStyle.normal.background = gs.normal.background;
                }

                gs = GUI.skin.FindStyle("AnimationKeyframeBackground");
                if (null != gs)
                {
                    ms_Text_GUIStyle.border = gs.border;
                    ms_Text_GUIStyle.normal.background = gs.normal.background;
                }

                ms_InitGUIStyle = false;
            }

            EditorGUILayout.BeginHorizontal();
            if (null == m_ADBProc)
            {
                Color oldCol = GUI.color;
                GUI.color = Color.green;

                if (GUILayout.Button("ADB Start", GUILayout.ExpandWidth(false), GUILayout.Width(90)))
                {
                    ProcessADBDevice(CallbackFinishDeviceEvent_ADBStart);
                }
                GUI.color = oldCol;
            }
            else
            {
                Color oldCol = GUI.color;
                GUI.color = Color.red;

                if (GUILayout.Button("ADB Stop", GUILayout.ExpandWidth(false), GUILayout.Width(90)))
                {
                    ProcessADBDone();
                }
                GUI.color = oldCol;
            }
            if (GUILayout.Button("Log Clear", GUILayout.ExpandWidth(false), GUILayout.Width(70)))
            {
                Clear();
            }

            if (m_LastLog)
            {
                Color oldCol = GUI.color;
                GUI.color = Color.red;
                if (GUILayout.Button("Auto Scroll", GUILayout.ExpandWidth(false), GUILayout.Width(90)))
                {
                    m_LastLog = false;
                }
                GUI.color = oldCol;
            }
            else
            {

                if (GUILayout.Button("Auto Scroll", GUILayout.ExpandWidth(false), GUILayout.Width(90)))
                {
                    m_LastLog = true;
                }


            }

            if (GUILayout.Button("File Save", GUILayout.ExpandWidth(false), GUILayout.Width(70)))
            {
                string szPath = EditorUtility.SaveFilePanel(
                        "Save Text File",
                        Application.dataPath + "../ADBLog",
                        "adblog.txt",
                        "txt");
                if (szPath.Length != 0)
                {
                    SaveToTextFile(szPath);
                }
                else
                {
                    //  error
                }
            }
            if (GUILayout.Button("File Load", GUILayout.ExpandWidth(false), GUILayout.Width(70)))
            {
                string szPath = EditorUtility.OpenFilePanel(
                        "Load Text File",
                        Application.dataPath + "../ADBLog",
                        "txt");
                if (szPath.Length != 0)
                {
                    LoadFromTextFile(szPath);
                }
                else
                {
                    //  error
                }
            }
            if (m_FindWindow)
            {
                Color oldCol = GUI.color;
                GUI.color = Color.red;
                if (GUILayout.Button("Find", GUILayout.ExpandWidth(false), GUILayout.Width(60)))
                {
                    m_FindWindow = false;
                }
                GUI.color = oldCol;
            }
            else
            {
                if (GUILayout.Button("Find", GUILayout.ExpandWidth(false), GUILayout.Width(60)))
                {
                    m_FindWindow = true;
                }
            }
            //---------------------------------------------------------------------
            bool bViewVerbose = EditorGUILayout.ToggleLeft("/V", (m_ViewVerbose == 1), GUILayout.ExpandWidth(false), GUILayout.Width(30));
            bool bViewDebug = EditorGUILayout.ToggleLeft("/D", (m_ViewDebug == 1), ms_LogDebug_GUIStyle, GUILayout.ExpandWidth(false), GUILayout.Width(30));
            bool bViewInfo = EditorGUILayout.ToggleLeft("/I", (m_ViewInfo == 1), ms_LogNormal_GUIStyle, GUILayout.ExpandWidth(false), GUILayout.Width(28));
            bool bViewWarn = EditorGUILayout.ToggleLeft("/W", (m_ViewWarn == 1), ms_LogWarn_GUIStyle, GUILayout.ExpandWidth(false), GUILayout.Width(32));
            bool bViewError = EditorGUILayout.ToggleLeft("/E", (m_ViewError == 1), ms_LogError_GUIStyle, GUILayout.ExpandWidth(false), GUILayout.Width(28));
            bool bViewAssert = EditorGUILayout.ToggleLeft("/A", (m_ViewAssert == 1), GUILayout.ExpandWidth(false), GUILayout.Width(30));

            if (bViewVerbose != (m_ViewVerbose == 1))
            {
                if (bViewVerbose)
                    m_ViewVerbose = 1;
                else
                    m_ViewVerbose = 0;
                PlayerPrefs.SetInt(Application.dataPath + "_ADB_ViewVerbose", m_ViewVerbose);
                ChanageViewList();
            }
            if (bViewDebug != (m_ViewDebug == 1))
            {
                if (bViewDebug)
                    m_ViewDebug = 1;
                else
                    m_ViewDebug = 0;
                PlayerPrefs.SetInt(Application.dataPath + "_ADB_ViewDebug", m_ViewDebug);
                ChanageViewList();
            }
            if (bViewInfo != (m_ViewInfo == 1))
            {
                if (bViewInfo)
                    m_ViewInfo = 1;
                else
                    m_ViewInfo = 0;
                PlayerPrefs.SetInt(Application.dataPath + "_ADB_ViewInfo", m_ViewInfo);
                ChanageViewList();
            }
            if (bViewWarn != (m_ViewWarn == 1))
            {
                if (bViewWarn)
                    m_ViewWarn = 1;
                else
                    m_ViewWarn = 0;
                PlayerPrefs.SetInt(Application.dataPath + "_ADB_ViewWarn", m_ViewWarn);
                ChanageViewList();
            }
            if (bViewError != (m_ViewError == 1))
            {
                if (bViewError)
                    m_ViewError = 1;
                else
                    m_ViewError = 0;
                PlayerPrefs.SetInt(Application.dataPath + "_ADB_ViewError", m_ViewError);
                ChanageViewList();
            }
            if (bViewAssert != (m_ViewAssert == 1))
            {
                if (bViewAssert)
                    m_ViewAssert = 1;
                else
                    m_ViewAssert = 0;
                PlayerPrefs.SetInt(Application.dataPath + "_ADB_ViewAssert", m_ViewAssert);
                ChanageViewList();
            }
            //---------------------------------------------------------------------
            if (GUILayout.Button("Option", GUILayout.ExpandWidth(false), GUILayout.Width(60)))
            {
                JDADBLogcatOption.ShowADBOption();
            }
            if (GUILayout.Button("Help", GUILayout.ExpandWidth(false), GUILayout.Width(50)))
            {
                EditorUtility.DisplayDialog("", "It will be supported.", "OK");
            }

            EditorGUILayout.EndHorizontal();
            //---------------------------------------------------------------------
            EditorGUILayout.BeginHorizontal();
            //---------------------------------------------------------------------
            //  Devices info
            switch (m_ProcessADBDeviceState)
            {
                case ProcessADBDeviceState.Loading:
                    EditorGUILayout.LabelField("Loading...", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false), GUILayout.Width(200), GUILayout.Height(20));
                    break;
                default:
                    {
                        if (m_DeviceList.Count > 0)
                        {
                            List<string> devices = new List<string>();
                            foreach (DeviceData dd in m_DeviceList)
                            {
                                devices.Add(dd.m_Model + "  " + dd.m_SerialNumber);
                            }

                            int iDeviceIndex = EditorGUILayout.Popup(
                                m_DeviceIndex, devices.ToArray(),
                                GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false), GUILayout.Width(200), GUILayout.Height(20));
                            if (iDeviceIndex != m_DeviceIndex)
                            {
                                m_DeviceIndex = iDeviceIndex;
                                if (null != m_ADBProc)
                                {
                                    ProcessADBDone();
                                    string szDeviceSN = GetCurrentDeviceSerialNumber();
                                    Clear();
                                    ProcessADBLogcat(szDeviceSN);
                                }
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("No devices found.", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false), GUILayout.Width(200), GUILayout.Height(20));
                        }
                    }
                    break;
            }

            if (GUILayout.Button("Devices Update", GUILayout.ExpandWidth(false), GUILayout.Width(140)))
            {
                ProcessADBDevice(CallbackFinishDeviceEvent_OnFirst);
            }
            //---------------------------------------------------------------------
            GUI.SetNextControlName("EditTag");
            string szCheckTagText = EditorGUILayout.TextField(m_CheckTagText);
            if (!szCheckTagText.Equals(m_CheckTagText))
            {
                m_CheckTagText = szCheckTagText;
                PlayerPrefs.SetString(Application.dataPath + "_ADB_CheckTagText", m_CheckTagText);
                ChanageViewList();
            }
            //---------------------------------------------------------------------
            EditorGUILayout.EndHorizontal();
            m_LogListInfo.m_ItemHeight = 18;
            float fH = position.height - 50 - 106;
            m_LogListInfo.m_ListHeight = fH;
            m_LogListInfo.m_TotalCount = m_LogViewList.Count;
            m_ListViewCount = (int)(fH / m_LogListInfo.m_ItemHeight);
            m_LogListInfo.m_ViewCount = m_ListViewCount;

            if (!m_LastLog)
            {
                if ((int)m_LogListIndex + m_LogListInfo.m_ViewCount >= m_LogViewList.Count)
                {
                    m_LogListIndex = m_LogViewList.Count;
                }
            }

            if (m_LastLog)
                m_LogListIndex = m_LogViewList.Count;
            EditorGUILayout.BeginHorizontal();
            bool bRepaint = false;
            Rect listrect = new Rect();

            GUILayoutUtil.List(
                m_LogListInfo,
                ref m_LogListIndex,
                OnDrawItemEvent_Log,
                ref bRepaint, ref listrect);
            if (bRepaint)
            {
                m_Repaint = true;
                m_LastLog = false;
            }

            Rect rr = GUILayoutUtility.GetLastRect();
            if (GUILayoutUtil.InArea(rr, Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    GUI.FocusControl("");
                }

            }

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            {
                if (m_FindWindow)
                {
                    EditorGUILayout.BeginHorizontal();
                    //-------------------------------------------------------------
                    GUI.SetNextControlName("EditFind");
                    m_FindText = EditorGUILayout.TextField(m_FindText, ms_Find_GUIStyle, GUILayout.ExpandHeight(false), GUILayout.Height(20));
                    if (Event.current.isKey && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "EditFind")
                    {
                        FindNext();
                    }
                    //-------------------------------------------------------------
                    if (GUILayout.Button("Next", GUILayout.ExpandWidth(false), GUILayout.Width(60)))
                    {
                        FindNext();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            float fTextH = 106;
            if (m_FindWindow)
                fTextH -= 20;
            EditorGUILayout.TextArea(m_ViewText, ms_Text_GUIStyle, GUILayout.ExpandHeight(false), GUILayout.Height(fTextH));

            EditorGUILayout.EndVertical();

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.isKey)
                {
                    if (Event.current.keyCode == KeyCode.DownArrow)
                    {
                        m_LogListIndex += 1;
                        Repaint();
                    }
                    if (Event.current.keyCode == KeyCode.UpArrow)
                    {
                        m_LogListIndex -= 1;
                        Repaint();
                    }
                    if (Event.current.keyCode == KeyCode.PageDown)
                    {
                        m_LogListIndex += (m_ListViewCount - 1);
                        Repaint();
                    }
                    if (Event.current.keyCode == KeyCode.PageUp)
                    {
                        m_LogListIndex -= (m_ListViewCount - 1);
                        Repaint();
                    }
                    if (Event.current.keyCode == KeyCode.Home)
                    {
                        m_LogListIndex = 0;
                        Repaint();
                    }
                    if (Event.current.keyCode == KeyCode.End)
                    {
                        m_LogListIndex = m_LogViewList.Count;
                        Repaint();
                    }
                    if (Event.current.keyCode == KeyCode.F3)
                    {
                        FindNext();
                    }
                }
            }

        }

        string m_FindText = string.Empty;
        bool m_FindWindow = false;
        bool m_LastLog = false;
        int m_ListViewCount = 0;
        int m_ListSelectIndex = -1;

        public static GUIStyle ms_ListSelect_GUIStyle = new GUIStyle();
        public static GUIStyle ms_LogError_GUIStyle = new GUIStyle();
        public static GUIStyle ms_LogWarn_GUIStyle = new GUIStyle();
        public static GUIStyle ms_LogNormal_GUIStyle = new GUIStyle();
        public static GUIStyle ms_LogDebug_GUIStyle = new GUIStyle();
        public static GUIStyle ms_Text_GUIStyle = new GUIStyle();
        public static GUIStyle ms_Find_GUIStyle = new GUIStyle();

        public static Color ms_LogError_Color = Color.red;
        public static Color ms_LogWarn_Color = new Color(1.0f, 0.4f, 0.1f);
        public static Color ms_LogNormal_Color = Color.white;
        public static Color ms_LogDebug_Color = Color.gray;

        public static Color ms_FindText_Color = Color.white;
        public static Color ms_Text_Color = Color.white;

        public static Color ms_ListSelect_Color = new Color(0.3f, 0.5f, 0.3f);

        void FindNext()
        {
            if (string.IsNullOrEmpty(m_FindText))
                return;
            int iStart = m_ListSelectIndex;

            bool bFind = false;
            iStart++;
            if (iStart < m_LogViewList.Count - 1)
            {
                for (int i = iStart; i < m_LogViewList.Count; ++i)
                {
                    if (null == m_LogViewList[i])
                        continue;

                    string szValue = m_LogViewList[i].m_OriginalLog;
                    if (szValue.Contains(m_FindText))
                    {
                        m_ListSelectIndex = i;

                        if ((m_ListSelectIndex < m_LogListIndex) ||
                            (m_ListSelectIndex >= (m_LogListIndex + m_ListViewCount)))
                        {
                            m_LogListIndex = m_ListSelectIndex - 3;
                        }
                        bFind = true;
                        break;
                    }
                }

                if (!bFind)
                {
                    iStart = 0;
                    for (int i = iStart; i < m_LogViewList.Count; ++i)
                    {
                        if (null == m_LogViewList[i])
                            continue;

                        string szValue = m_LogViewList[i].m_OriginalLog;
                        if (szValue.Contains(m_FindText))
                        {
                            m_ListSelectIndex = i;

                            if ((m_ListSelectIndex < m_LogListIndex) ||
                                (m_ListSelectIndex >= (m_LogListIndex + m_ListViewCount)))
                            {
                                m_LogListIndex = m_ListSelectIndex - 3;
                            }
                            bFind = true;
                            break;
                        }
                    }
                    if (bFind)
                    {
                        Repaint();
                    }
                    else
                    {
                        //  not find
                    }

                }
                else
                {
                    Repaint();
                }
            }

        }


        void OnDrawItemEvent_Log(int iIndex)
        {
            if (m_LogViewList.Count > 0)
            {
                if (iIndex < m_LogViewList.Count)
                {
                    switch (Event.current.type)
                    {
                        default:
                            //case EventType.MouseDrag:
                            //case EventType.MouseDown:
                            //case EventType.MouseUp:
                            //case EventType.MouseMove:
                            //case EventType.Repaint:
                            //case EventType.Layout:
                            {
                                LogData ld = m_LogViewList[iIndex];
                                string szValue = ld.m_ViewData;
                                if (m_ListSelectIndex == iIndex)
                                {
                                    EditorGUILayout.LabelField(szValue, ms_ListSelect_GUIStyle);
                                }
                                else
                                {
                                    switch (ld.m_Logtype)
                                    {
                                        case LogType.lt_Warning:
                                            EditorGUILayout.LabelField(szValue, ms_LogWarn_GUIStyle);
                                            break;
                                        case LogType.lt_Error:
                                            EditorGUILayout.LabelField(szValue, ms_LogError_GUIStyle);
                                            break;
                                        case LogType.lt_Debug:
                                            EditorGUILayout.LabelField(szValue, ms_LogDebug_GUIStyle);
                                            break;
                                        default:
                                            EditorGUILayout.LabelField(szValue, ms_LogNormal_GUIStyle);
                                            break;
                                    }

                                }

                                //-------------------------------------------------------------
                                Rect rr = GUILayoutUtility.GetLastRect();
                                if (GUILayoutUtil.InArea(rr, Event.current.mousePosition))
                                {
                                    if (Event.current.type == EventType.MouseDown)
                                    {
                                        m_ListSelectIndex = iIndex;
                                        m_Repaint = true;
                                        OnSelectLogData(m_ListSelectIndex);
                                    }
                                }
                                //-------------------------------------------------------------

                            }
                            break;
                    }
                }
            }
        }


        void OnSelectLogData(int aIndex)
        {
            GUI.FocusControl("");

            if (aIndex < (m_LogViewList.Count))
            {
                LogData ld = m_LogViewList[aIndex];
                if (null != ld)
                {
                    m_ViewText = ld.m_OriginalLog;
                }
            }
        }

        bool m_Repaint = false;
        void OnLog(string aLog)
        {
            if (!string.IsNullOrEmpty(aLog))
            {
                LogData ld = StringToLogData(aLog);
                if (null != ld)
                {
                    ld.m_ViewData = GetViewString(ld);

                    if (m_LogAllList.Count > ms_LimitLogCount - 1)
                    {
                        m_LogAllList.RemoveAt(0);
                    }

                    AddViewLog(ld);

                    m_LogAllList.Add(ld);
                    m_Repaint = true;


                }
            }
        }

        static bool ms_WriteLog = false;



        void AddViewLog(LogData aLD)
        {
            if (m_LogViewList.Count > ms_LimitViewLogCount - 1)
            {
                m_LogViewList.RemoveAt(0);
                m_LogListIndex--;
                if (m_LogListIndex < 0)
                    m_LogListIndex = 0;
                m_ListSelectIndex--;
                if (m_ListSelectIndex < -1)
                    m_ListSelectIndex = -1;
            }

            bool bAdd = true;
            if (!string.IsNullOrEmpty(m_CheckTagText))
            {
                string szCheckTarget = aLD.m_OriginalLog.ToLower();
                string szCheckStr = m_CheckTagText.ToLower();
                if (szCheckTarget.Contains(szCheckStr))
                    bAdd = true;
                else
                    bAdd = false;
            }

            if (bAdd)
            {
                if (ms_LogEmptyLineView == 1)
                {
                    if (string.IsNullOrEmpty(aLD.m_Log))
                        bAdd = false;
                }
            }

            if (bAdd)
            {
                if ((m_ViewVerbose == 1) && (aLD.m_Logtype == LogType.lt_Verbose))
                    m_LogViewList.Add(aLD);
                if ((m_ViewDebug == 1) && (aLD.m_Logtype == LogType.lt_Debug))
                    m_LogViewList.Add(aLD);
                if ((m_ViewInfo == 1) && (aLD.m_Logtype == LogType.lt_Info))
                    m_LogViewList.Add(aLD);
                if ((m_ViewWarn == 1) && (aLD.m_Logtype == LogType.lt_Warning))
                    m_LogViewList.Add(aLD);
                if ((m_ViewError == 1) && (aLD.m_Logtype == LogType.lt_Error))
                    m_LogViewList.Add(aLD);
                if ((m_ViewAssert == 1) && (aLD.m_Logtype == LogType.lt_Silent))
                    m_LogViewList.Add(aLD);
            }

        }

        public string GetViewString(LogData ald)
        {
            if (null == ald)
                return string.Empty;

            string szDate = string.Empty;
            if (ms_LogDateView == 1)
            {
                szDate = string.Format("{0} {1}\t", ald.m_Day, ald.m_Time);
            }
            string szTag = string.Empty;
            if (ms_LogTagView == 1)
            {
                szTag = string.Format("{0}\t", ald.m_Tag);
            }
            return string.Format("{0}{1}{2}", szDate, szTag, ald.m_Log);
        }

        public void ChangeViewOpton()
        {
            foreach (LogData ld in m_LogViewList)
            {
                ld.m_ViewData = GetViewString(ld);
            }
            Repaint();
        }

        public void ChanageViewList()
        {
            m_LogViewList.Clear();

            for (int i = 0; i < m_LogAllList.Count; ++i)
            {
                LogData ld = m_LogAllList[i];
                if (null == ld)
                    continue;

                AddViewLog(ld);
            }
        }

        public class DeviceData
        {
            public string m_SerialNumber = string.Empty;
            public string m_Product = string.Empty;
            public string m_Model = string.Empty;
            public string m_TransportID = string.Empty;
            public string m_Device = string.Empty;
        }

        string GetCurrentDeviceSerialNumber()
        {
            if (m_DeviceList.Count > 0)
            {
                return m_DeviceList[m_DeviceIndex].m_SerialNumber;
            }
            return string.Empty;
        }

        string GetSDKPath()
        {
            if (!File.Exists(ms_ADBSDKPath))
            {
                ms_ADBSDKPath = string.Empty;
            }
            return ms_ADBSDKPath;
        }

        List<DeviceData> m_DeviceList = new List<DeviceData>();
        int m_DeviceIndex = 0;

        public delegate void CallbackFinishDeviceEvent(string aError);
        CallbackFinishDeviceEvent m_CallbackFinishDevice = null;

        public enum ProcessADBDeviceState
        {
            None,
            Loading,
            Finish,
        }
        ProcessADBDeviceState m_ProcessADBDeviceState = ProcessADBDeviceState.None;
        public void ProcessADBDevice(CallbackFinishDeviceEvent aCallback)
        {
            m_ProcessADBDeviceState = ProcessADBDeviceState.Loading;
            m_CallbackFinishDevice = aCallback;
            System.Threading.ThreadPool.QueueUserWorkItem(delegate (object state)
            {
                try
                {
                    string szSDKPath = GetSDKPath();
                    System.Diagnostics.ProcessStartInfo procStartInfo =
                        new System.Diagnostics.ProcessStartInfo("cmd", "/c \"" + szSDKPath + "\" devices -l");

                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.UseShellExecute = false;
                    procStartInfo.CreateNoWindow = true;
                    procStartInfo.RedirectStandardError = true;
                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.RedirectStandardInput = false;
                    int iCodePage = System.Console.OutputEncoding.CodePage;
                    procStartInfo.StandardOutputEncoding = System.Text.Encoding.GetEncoding(iCodePage);
                    procStartInfo.StandardErrorEncoding = System.Text.Encoding.GetEncoding(iCodePage);

                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo = procStartInfo;
                    proc.Start();

                    string stdError = proc.StandardError.ReadToEnd();
                    string result = proc.StandardOutput.ReadToEnd();
                    result = result.Replace(" ", "!");
                    result = result.Replace("\r\n", "|");

                    if (!string.IsNullOrEmpty(stdError))
                    {
                        UnityEngine.Debug.LogError(stdError);
                    }

                    //-----------------------------------------------------------------
                    m_DeviceList.Clear();
                    string[] bufs = result.Split('|');
                    foreach (string buf in bufs)
                    {
                        if (buf.Contains("List!of!devices!attached"))
                            continue;
                        if (buf.Contains("product:"))
                        {
                            DeviceData dd = new DeviceData();
                            string[] dinfos = buf.Split('!');
                            dd.m_SerialNumber = dinfos[0];
                            foreach (string dinfo in dinfos)
                            {
                                if (dinfo.Contains("product:"))
                                {
                                    string szValue = dinfo.Replace("product:", "");
                                    dd.m_Product = szValue;
                                }
                                if (dinfo.Contains("model:"))
                                {
                                    string szValue = dinfo.Replace("model:", "");
                                    dd.m_Model = szValue;
                                }
                                if (dinfo.Contains("device:"))
                                {
                                    string szValue = dinfo.Replace("device:", "");
                                    dd.m_Device = szValue;
                                }
                                if (dinfo.Contains("transport_id:"))
                                {
                                    string szValue = dinfo.Replace("transport_id:", "");
                                    dd.m_TransportID = szValue;
                                }
                            }
                            m_DeviceList.Add(dd);
                        }

                    }
                    //-----------------------------------------------------------------

                    OnLog(result);

                    m_ProcessADBDeviceState = ProcessADBDeviceState.Finish;
                    if (null != aCallback)
                    {
                        m_EventList.Add(new CallbackEvent(aCallback, stdError));
                    }

                    proc.WaitForExit();
                    proc.Kill();
                }
                catch (System.Exception e)
                {
                    if (ms_WriteLog)
                        UnityEngine.Debug.LogException(e);

                }
            });
        }

        Process m_ADBProc = null;
        StreamReader m_OutputReader = null;

        void ProcessADBLogcat(string aSerialNumber)
        {
            if (null != m_ADBProc)
                return;
            System.Threading.ThreadPool.QueueUserWorkItem(delegate (object state)
            {
                try
                {
                    string szSDKPath = GetSDKPath();
                    m_ADBProc = new Process();
                    m_ADBProc.StartInfo.FileName = "cmd.exe";
                    m_ADBProc.StartInfo.Arguments = string.Format("/c " + szSDKPath + " -s {0} logcat -v time", aSerialNumber);

                    m_ADBProc.StartInfo.UseShellExecute = false;
                    m_ADBProc.StartInfo.CreateNoWindow = true;
                    m_ADBProc.StartInfo.RedirectStandardError = true;
                    m_ADBProc.StartInfo.RedirectStandardInput = true;
                    m_ADBProc.StartInfo.RedirectStandardOutput = true;
                    m_ADBProc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    m_ADBProc.Start();
                    System.Threading.Thread.Sleep(1000);
                    m_OutputReader = m_ADBProc.StandardOutput;

                    while (true)
                    {
                        if (null != m_OutputReader)
                        {
                            if (m_OutputReader.EndOfStream)
                                break;

                            if (null != m_OutputReader)
                            {
                                string text = m_OutputReader.ReadLine();
                                OnLog(text);
                            }
                            System.Threading.Thread.Sleep(1);
                        }
                        else
                        {
                            break;
                        }
                    }

                    m_ADBProc.WaitForExit();
                }
                catch (System.Exception e)
                {
                    if (ms_WriteLog)
                        UnityEngine.Debug.LogException(e);
                    ProcessADBDone();
                }
                finally
                {
                    ProcessADBDone();
                }

            });

        }
        public void ProcessADBDone()
        {
            if (m_ADBProc != null)
            {
                m_OutputReader = null;
                try
                {
                    m_ADBProc.Kill();
                    m_ADBProc = null;
                }
                catch (System.Exception e)
                {
                    if (ms_WriteLog)
                        UnityEngine.Debug.LogException(e);
                }


                m_ADBProc = null;
            }
        }

        public class GUILayoutUtil
        {
            public delegate void OnDrawItemEvent(int iIndex);

            [System.Serializable]
            public class ListInfo
            {
                public int m_TotalCount = 0;
                public int m_ViewCount = 4;
                public float m_ListHeight = 100;
                public float m_ItemHeight = 25;
            }

            public static bool InArea(Rect aRect, Vector2 aPos)
            {
                if (aRect.x > aPos.x)
                    return false;
                if (aRect.y > aPos.y)
                    return false;
                if (aRect.x + aRect.width < aPos.x)
                    return false;
                if (aRect.y + aRect.height < aPos.y)
                    return false;
                return true;
            }

            public static void List(
                ListInfo aInfo,
                ref float aCurrentSelectIndex,
                OnDrawItemEvent aOnDrawItem, ref bool aRepaint, ref Rect aRect)
            {
                List(
                    aInfo.m_TotalCount, aInfo.m_ViewCount,
                    aInfo.m_ListHeight, aInfo.m_ItemHeight,
                    ref aCurrentSelectIndex,
                    aOnDrawItem, ref aRepaint, ref aRect);
            }

            public static void List(
                int aTotalCount, int aViewCount,
                float aListHeight, float aItemHeight,
                ref float aCurrentSelectIndex,
                OnDrawItemEvent aOnDrawItem, ref bool aRepaint, ref Rect aRect)
            {
                int iViewCount = aViewCount;
                int iCount = aTotalCount - iViewCount + 1;
                GUILayout.BeginVertical(GUILayout.Height(aListHeight));
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.BeginVertical();
                        if (aCurrentSelectIndex > iCount - 1)
                            aCurrentSelectIndex = iCount - 1;
                        if (aCurrentSelectIndex < 0)
                            aCurrentSelectIndex = 0;

                        for (int iRow = 0; iRow < iViewCount; ++iRow)
                        {
                            int iIndex = iRow + (int)(aCurrentSelectIndex);

                            if (iIndex < 0)
                                continue;

                            if (iIndex >= aTotalCount)
                                continue;

                            if (null != aOnDrawItem)
                                aOnDrawItem(iIndex);
                        }


                        GUILayout.EndVertical();
                    }

                    aCurrentSelectIndex = GUILayout.VerticalScrollbar(
                        aCurrentSelectIndex, 1, 0, iCount, GUILayout.ExpandHeight(false), GUILayout.Height(aListHeight));

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
                //-----------------------------------------------------------------
                aRepaint = false;
                Rect rr = GUILayoutUtility.GetLastRect();
                aRect = rr;
                if (InArea(rr, Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.ScrollWheel)
                    {
                        if (Event.current.delta.y > 0)
                        {
                            aCurrentSelectIndex += 3;
                            aRepaint = true;
                        }
                        else
                        {
                            aCurrentSelectIndex -= 3;
                            aRepaint = true;
                        }
                    }
                }
                //-----------------------------------------------------------------

            }
        }

        public void LoadFromTextFile(string aPath)
        {
            Clear();

            StreamReader sr = new StreamReader(aPath);
            string szBuf = sr.ReadToEnd();
            sr.Close();

            string[] szLines = szBuf.Split("\n"[0]);
            foreach (string szValue in szLines)
            {
                OnLog(szValue);
            }
        }

        public void SaveToTextFile(string aPath)
        {
            StreamWriter sw = new StreamWriter(aPath, false, System.Text.Encoding.UTF8);
            foreach (LogData ld in m_LogAllList)
            {
                sw.WriteLine(ld.m_OriginalLog);
            }
            sw.Close();
        }

        public static Rect ContractRect(Rect aTarget, float aValue)
        {
            Rect Result = new Rect(aTarget);
            Result.x = Result.x + aValue;
            Result.y = Result.y + aValue;
            Result.width = Result.width - (aValue * 2);
            Result.height = Result.height - (aValue * 2);
            return Result;
        }

        public static string ColorToString(Color aValue)
        {
            string result = string.Format("{0}#{1}#{2}#{3}", aValue.r, aValue.g, aValue.b, aValue.a);
            return result;
        }

        public static Color StringToColor(string aValue)
        {
            Color result = Color.white;
            string[] bufs = aValue.Split('#');
            if (bufs.Length == 4)
            {
                float fValue = 1;
                if (float.TryParse(bufs[0], out fValue))
                    result.r = fValue;
                if (float.TryParse(bufs[1], out fValue))
                    result.g = fValue;
                if (float.TryParse(bufs[2], out fValue))
                    result.b = fValue;
                if (float.TryParse(bufs[3], out fValue))
                    result.a = fValue;
            }

            return result;
        }
    }   //  class JDADBLogcat

    //-------------------------------------------------------------------------
    //                  Option
    //-------------------------------------------------------------------------

    public class JDADBLogcatSDKSetup : EditorWindow
    {
        public static GUIStyle ms_LabelPath_GUIStyle = new GUIStyle();

        public static void ShowSDKSetup()
        {
            JDADBLogcatSDKSetup window = (JDADBLogcatSDKSetup)EditorWindow.GetWindow(typeof(JDADBLogcatSDKSetup));
            window.titleContent = new GUIContent("adb Setup", "ADB Logcat Option");
            window.autoRepaintOnSceneChange = false;
            window.minSize = new Vector2(620, 160);
            window.Focus();
            window.ShowPopup();
        }

        bool m_Done = false;

        public void InitGUIStyle()
        {
            ms_LabelPath_GUIStyle.fontStyle = FontStyle.Bold;
            ms_LabelPath_GUIStyle.normal.textColor = new Color(255 / 91, 255 / 175, 255 / 227);
            ms_LabelPath_GUIStyle.alignment = TextAnchor.MiddleLeft;
            ms_LabelPath_GUIStyle.clipping = TextClipping.Clip;
            ms_LabelPath_GUIStyle.padding = new RectOffset(2, 0, -2, 0);
            ms_LabelPath_GUIStyle.richText = true;
        }

        void OnEnable()
        {
            InitGUIStyle();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Android adb path");
            EditorGUILayout.BeginHorizontal();
            string szOutput = JDADBLogcat.ms_ADBSDKPath;
            if (string.IsNullOrEmpty(szOutput))
            {
                szOutput = "Please put adb.exe.";
            }


            EditorGUILayout.LabelField(szOutput, ms_LabelPath_GUIStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Separator();
            if (m_Done)
            {
                if (GUILayout.Button("Done"))
                {
                    this.Close();
                }
            }
            if (GUILayout.Button("Browse"))
            {
                string szPath = EditorUtility.OpenFilePanel(
                        "Default Setup Location  (%USERPROFILE%/AppData/Local/Android/sdk/platform-tools/adb.exe)",
                        Application.dataPath,
                        "exe");
                if (szPath.Length != 0)
                {
                    JDADBLogcat.ms_ADBSDKPath = szPath;
                    PlayerPrefs.SetString("_ADB_SDKPath", JDADBLogcat.ms_ADBSDKPath);
                    PlayerPrefs.Save();
                    UnityEngine.Debug.Log(szPath);
                    this.Repaint();
                    this.Focus();
                    m_Done = true;
                }
                else
                {
                    //  error
                }
            }
            if (GUILayout.Button("Download"))
            {
                Application.OpenURL("https://developer.android.com/studio/#Other");
            }
            EditorGUILayout.EndHorizontal();
        }
    }


    public class JDADBLogcatOption : EditorWindow
    {
        public static GUIStyle ms_Caption_GUIStyle = new GUIStyle();

        public static void ShowADBOption()
        {
            JDADBLogcatOption window = (JDADBLogcatOption)EditorWindow.GetWindow(typeof(JDADBLogcatOption));
            window.titleContent = new GUIContent("Option", "ADB Logcat Option");
            window.autoRepaintOnSceneChange = false;
            window.minSize = new Vector2(420, 454);
            window.Focus();
            window.Show();
        }

        void Awake()
        {
            JDADBLogcatOption.InitGUIStyle();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            JDADBLogcatOption.InitGUIStyle();
        }
        public static void InitGUIStyle()
        {
            ms_Caption_GUIStyle.fontSize = 20;
            ms_Caption_GUIStyle.normal.textColor = Color.white;
            ms_Caption_GUIStyle.fontStyle = FontStyle.Bold;
        }

        void OnGUI()
        {
            JDADBLogcat adb_window = JellyDot_ADBLogcat.JDADBLogcat.ms_ADBWindow;
            //---------------------------------------------------------------------
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Line Color", ms_Caption_GUIStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            Color Col = EditorGUILayout.ColorField("List Select", JellyDot_ADBLogcat.JDADBLogcat.ms_ListSelect_Color);
            if (Col != JellyDot_ADBLogcat.JDADBLogcat.ms_ListSelect_Color)
            {
                JellyDot_ADBLogcat.JDADBLogcat.ms_ListSelect_Color = Col;
                string szColor = JDADBLogcat.ColorToString(JellyDot_ADBLogcat.JDADBLogcat.ms_ListSelect_Color);
                PlayerPrefs.GetString(Application.dataPath + "_ADB_LogError_Color", szColor);
                JellyDot_ADBLogcat.JDADBLogcat.InitGUIStyle();
            }
            Col = EditorGUILayout.ColorField("Log Error_Color", JellyDot_ADBLogcat.JDADBLogcat.ms_LogError_Color);
            if (Col != JellyDot_ADBLogcat.JDADBLogcat.ms_LogError_Color)
            {
                JellyDot_ADBLogcat.JDADBLogcat.ms_LogError_Color = Col;
                string szColor = JDADBLogcat.ColorToString(JellyDot_ADBLogcat.JDADBLogcat.ms_LogError_Color);
                PlayerPrefs.GetString(Application.dataPath + "_ADB_LogError_Color", szColor);
                JellyDot_ADBLogcat.JDADBLogcat.InitGUIStyle();
            }
            Col = EditorGUILayout.ColorField("Log Warn Color", JellyDot_ADBLogcat.JDADBLogcat.ms_LogWarn_Color);
            if (Col != JellyDot_ADBLogcat.JDADBLogcat.ms_LogWarn_Color)
            {
                JellyDot_ADBLogcat.JDADBLogcat.ms_LogWarn_Color = Col;
                string szColor = JDADBLogcat.ColorToString(JellyDot_ADBLogcat.JDADBLogcat.ms_LogWarn_Color);
                PlayerPrefs.GetString(Application.dataPath + "_ADB_LogWarn_Color", szColor);
                JellyDot_ADBLogcat.JDADBLogcat.InitGUIStyle();
            }
            Col = EditorGUILayout.ColorField("Log Normal Color", JellyDot_ADBLogcat.JDADBLogcat.ms_LogNormal_Color);
            if (Col != JellyDot_ADBLogcat.JDADBLogcat.ms_LogNormal_Color)
            {
                JellyDot_ADBLogcat.JDADBLogcat.ms_LogNormal_Color = Col;
                string szColor = JDADBLogcat.ColorToString(JellyDot_ADBLogcat.JDADBLogcat.ms_LogNormal_Color);
                PlayerPrefs.GetString(Application.dataPath + "_ADB_LogNormal_Color", szColor);
                JellyDot_ADBLogcat.JDADBLogcat.InitGUIStyle();
            }
            Col = EditorGUILayout.ColorField("Log Debug Color", JellyDot_ADBLogcat.JDADBLogcat.ms_LogDebug_Color);
            if (Col != JellyDot_ADBLogcat.JDADBLogcat.ms_LogDebug_Color)
            {
                JellyDot_ADBLogcat.JDADBLogcat.ms_LogDebug_Color = Col;
                string szColor = JDADBLogcat.ColorToString(JellyDot_ADBLogcat.JDADBLogcat.ms_LogDebug_Color);
                PlayerPrefs.GetString(Application.dataPath + "_ADB_LogDebug_Color", szColor);
                JellyDot_ADBLogcat.JDADBLogcat.InitGUIStyle();
            }
            Col = EditorGUILayout.ColorField("Find Text Color", JellyDot_ADBLogcat.JDADBLogcat.ms_FindText_Color);
            if (Col != JellyDot_ADBLogcat.JDADBLogcat.ms_FindText_Color)
            {
                JellyDot_ADBLogcat.JDADBLogcat.ms_FindText_Color = Col;
                string szColor = JDADBLogcat.ColorToString(JellyDot_ADBLogcat.JDADBLogcat.ms_FindText_Color);
                PlayerPrefs.GetString(Application.dataPath + "_ADB_FindText_Color", szColor);
                JellyDot_ADBLogcat.JDADBLogcat.InitGUIStyle();
            }
            Col = EditorGUILayout.ColorField("Text Color", JellyDot_ADBLogcat.JDADBLogcat.ms_Text_Color);
            if (Col != JellyDot_ADBLogcat.JDADBLogcat.ms_Text_Color)
            {
                JellyDot_ADBLogcat.JDADBLogcat.ms_Text_Color = Col;
                string szColor = JDADBLogcat.ColorToString(JellyDot_ADBLogcat.JDADBLogcat.ms_Text_Color);
                PlayerPrefs.GetString(Application.dataPath + "_ADB_Text_Color", szColor);
                JellyDot_ADBLogcat.JDADBLogcat.InitGUIStyle();
            }
            EditorGUILayout.Space();
            //---------------------------------------------------------------------
            EditorGUILayout.LabelField("Buffer Setting", ms_Caption_GUIStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            int iValue = EditorGUILayout.IntField("Limit Log Count", JellyDot_ADBLogcat.JDADBLogcat.ms_LimitLogCount);
            if (iValue != JellyDot_ADBLogcat.JDADBLogcat.ms_LimitLogCount)
            {
                JellyDot_ADBLogcat.JDADBLogcat.ms_LimitLogCount = iValue;
                PlayerPrefs.SetInt(Application.dataPath + "_ADB_LimitLogCount", JellyDot_ADBLogcat.JDADBLogcat.ms_LimitLogCount);
            }
            //---------------------------------------------------------------------
            iValue = EditorGUILayout.IntField("Limit ViewLog Count", JellyDot_ADBLogcat.JDADBLogcat.ms_LimitViewLogCount);
            if (iValue != JellyDot_ADBLogcat.JDADBLogcat.ms_LimitViewLogCount)
            {
                JellyDot_ADBLogcat.JDADBLogcat.ms_LimitViewLogCount = iValue;
                PlayerPrefs.SetInt(Application.dataPath + "_ADB_LimitViewLogCount", JellyDot_ADBLogcat.JDADBLogcat.ms_LimitViewLogCount);
            }
            EditorGUILayout.Space();
            //---------------------------------------------------------------------
            EditorGUILayout.LabelField("Log View", ms_Caption_GUIStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            //---------------------------------------------------------------------
            bool bValue = EditorGUILayout.Toggle("View Log Date", JellyDot_ADBLogcat.JDADBLogcat.ms_LogDateView == 1);
            if (bValue != (JellyDot_ADBLogcat.JDADBLogcat.ms_LogDateView == 1))
            {
                if (bValue)
                    JellyDot_ADBLogcat.JDADBLogcat.ms_LogDateView = 1;
                else
                    JellyDot_ADBLogcat.JDADBLogcat.ms_LogDateView = 0;

                PlayerPrefs.SetInt(Application.dataPath + "_ADB_LogDateView", JellyDot_ADBLogcat.JDADBLogcat.ms_LogDateView);
                if (null != adb_window)
                {
                    adb_window.ChangeViewOpton();
                }

            }
            bValue = EditorGUILayout.Toggle("View Log Tag", JellyDot_ADBLogcat.JDADBLogcat.ms_LogTagView == 1);
            if (bValue != (JellyDot_ADBLogcat.JDADBLogcat.ms_LogTagView == 1))
            {
                if (bValue)
                    JellyDot_ADBLogcat.JDADBLogcat.ms_LogTagView = 1;
                else
                    JellyDot_ADBLogcat.JDADBLogcat.ms_LogTagView = 0;
                PlayerPrefs.SetInt(Application.dataPath + "_ADB_LogTagView", JellyDot_ADBLogcat.JDADBLogcat.ms_LogTagView);
                if (null != adb_window)
                {
                    adb_window.ChangeViewOpton();
                }
            }
            //---------------------------------------------------------------------
            bValue = EditorGUILayout.Toggle("Not Visible LogEmptyLine", JellyDot_ADBLogcat.JDADBLogcat.ms_LogEmptyLineView == 1);
            if (bValue != (JellyDot_ADBLogcat.JDADBLogcat.ms_LogEmptyLineView == 1))
            {
                if (bValue)
                    JellyDot_ADBLogcat.JDADBLogcat.ms_LogEmptyLineView = 1;
                else
                    JellyDot_ADBLogcat.JDADBLogcat.ms_LogEmptyLineView = 0;
                PlayerPrefs.SetInt(Application.dataPath + "_ADB_LogEmptyLineView", JellyDot_ADBLogcat.JDADBLogcat.ms_LogEmptyLineView);
                if (null != adb_window)
                {
                    adb_window.ChanageViewList();
                }
            }
            //---------------------------------------------------------------------
            EditorGUILayout.Space();
            //---------------------------------------------------------------------
            EditorGUILayout.LabelField("adb Setting", ms_Caption_GUIStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(JDADBLogcat.ms_ADBSDKPath);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Separator();

            if (GUILayout.Button("Change"))
            {
                JDADBLogcatSDKSetup.ShowSDKSetup();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

}


#endif





/*-----------------------------------------------------------------------------
###	version 1.0

    First Release

###	version 1.1

	- Bug Fixes

        * Fixed a bug that did not exit when logging.
		* Fixed bug that started when device was not selected.

        * adb.exe setup.
		* Status display at device update.

        * Change Option.
###	version 1.2

	- Bug Fixes

        * Fixed a bug that did not exit when logging.
-----------------------------------------------------------------------------*/

