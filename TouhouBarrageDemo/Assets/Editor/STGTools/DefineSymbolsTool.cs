using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class DefineSymbolsTool : EditorWindow
{
    enum eSymbolsType : byte
    {
        Debug,
        Release,
    }

    enum eStartGameType : byte
    {
        /// <summary>
        /// 从主界面启动
        /// </summary>
        StartFromTitle,
        /// <summary>
        /// 直接启动游戏
        /// </summary>
        StartFromGame,
    }

    enum eStartCharacter : byte
    {
        Reimu,
        Marisa,
    }

    [MenuItem("YKTools/STGTools/DefineSymbols")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DefineSymbolsTool));
    }

    public DefineSymbolsTool()
    {
        titleContent = new GUIContent("DefineSymbols");
    }

    public void OnEnable()
    {
        // Get DefineSymbols
        var list = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        var symbols = new List<string>(list.Split(';'));
        _symbolsType = symbols.IndexOf("Debug") != -1 ? eSymbolsType.Debug : eSymbolsType.Release;
        _logProtoMsg = symbols.IndexOf("LogCreateBulletProto") != -1;
        _showDebugInfoInMainView = symbols.IndexOf("ShowDebugInfo") != -1;
        _isStartedFromGame = symbols.IndexOf("StartFromGame") != -1;
        _showCollisionViewer = symbols.IndexOf("ShowCollisionViewer") != -1;
        _showLuaDebugStackTrace = symbols.IndexOf("ShowLuaDebugStackTrace") != -1;
        if (_isStartedFromGame)
        {
            if (symbols.IndexOf("StartWithReimu") != -1)
                _char = eStartCharacter.Reimu;
            else if (symbols.IndexOf("StartWithMarisa") != -1)
                _char = eStartCharacter.Marisa;
        }
        else
        {
            _char = eStartCharacter.Reimu;
        }
    }

    private eSymbolsType _symbolsType;
    /// <summary>
    /// 创建子弹原型信息
    /// </summary>
    private bool _logProtoMsg;
    /// <summary>
    /// 在主界面显示调试信息
    /// </summary>
    private bool _showDebugInfoInMainView;
    /// <summary>
    /// 是否直接进入游戏（debug模式专属)
    /// </summary>
    private bool _isStartedFromGame;
    private eStartCharacter _char;
    private bool _showCollisionViewer;
    /// <summary>
    /// 显示lua报错堆栈信息
    /// </summary>
    private bool _showLuaDebugStackTrace;

    private void OnGUI()
    {
        EditorGUILayout.Space();
        _symbolsType = (eSymbolsType)EditorGUILayout.EnumPopup("SymbolVersion : ", _symbolsType);
        GUILayout.Space(5);
        if (_symbolsType == eSymbolsType.Debug)
        {
            _showLuaDebugStackTrace = EditorGUILayout.Toggle("显示LUA堆栈信息", _showLuaDebugStackTrace);
            _logProtoMsg = EditorGUILayout.Toggle("创建子弹原型信息", _logProtoMsg);
            _showDebugInfoInMainView = EditorGUILayout.Toggle("主界面显示调试信息", _showDebugInfoInMainView);
            _showCollisionViewer = EditorGUILayout.Toggle("显示判定图形", _showCollisionViewer);
            GUILayout.Space(1);
            _isStartedFromGame = EditorGUILayout.BeginToggleGroup("StartFromGame", _isStartedFromGame);
            _char = (eStartCharacter)EditorGUILayout.EnumPopup("Character", _char);
            EditorGUILayout.EndToggleGroup();
            GUILayout.Space(5);
            if (GUILayout.Button("DefineSymbols"))
            {
                List<string> newSymbols = new List<string>();
                newSymbols.Add("Debug");
                if (_showLuaDebugStackTrace)
                    newSymbols.Add("ShowLuaDebugStackTrace");
                if (_logProtoMsg)
                    newSymbols.Add("LogCreateBulletProto");
                if (_showDebugInfoInMainView)
                    newSymbols.Add("ShowDebugInfo");
                if (_showCollisionViewer)
                    newSymbols.Add("ShowCollisionViewer");
                if (_isStartedFromGame)
                {
                    newSymbols.Add("StartFromGame");
                    if (_char == eStartCharacter.Reimu)
                        newSymbols.Add("StartWithReimu");
                    else if (_char == eStartCharacter.Marisa)
                        newSymbols.Add("StartWithMarisa");
                }

                string symbolsStr = "";
                if (newSymbols.Count > 0)
                {
                    symbolsStr += newSymbols[0];
                    for (int i = 1; i < newSymbols.Count; i++)
                    {
                        symbolsStr += ";" + newSymbols[i];
                    }
                }
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbolsStr);
                UnityEditor.EditorUtility.DisplayDialog("DefineSymbols", "Define new symbols complete!", "ok");
            }
        }
        else
        {
            if (GUILayout.Button("DefineSymbols"))
            {
                List<string> newSymbols = new List<string>();
                newSymbols.Add("Release");

                string symbolsStr = "";
                if (newSymbols.Count > 0)
                {
                    symbolsStr += newSymbols[0];
                    for (int i = 1; i < newSymbols.Count; i++)
                    {
                        symbolsStr += ";" + newSymbols[i];
                    }
                }
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, symbolsStr);
                UnityEditor.EditorUtility.DisplayDialog("DefineSymbols", "Define new symbols complete!", "ok");
            }
        }
    }
}