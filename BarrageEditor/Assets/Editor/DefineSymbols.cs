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
        _symbolsType = symbols.IndexOf("Debug") == -1 ? eSymbolsType.Debug : eSymbolsType.Release;
    }

    private eSymbolsType _symbolsType;

    private void OnGUI()
    {
        EditorGUILayout.Space();
        _symbolsType = (eSymbolsType)EditorGUILayout.EnumPopup("SymbolVersion : ", _symbolsType);
        GUILayout.Space(5);
        if (_symbolsType == eSymbolsType.Debug)
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("DefineSymbols"))
            {
                List<string> newSymbols = new List<string>();
                newSymbols.Add("Debug");

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
            EditorGUILayout.Space();
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