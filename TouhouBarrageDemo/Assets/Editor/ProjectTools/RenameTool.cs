using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace YKEditorTool
{
    public class RenameTool : EditorWindow
    {
        [MenuItem("YKTools/Project/Rename")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(RenameTool));
        }

        public RenameTool()
        {
            titleContent = new GUIContent("Rename");
        }

        /// <summary>
        /// 选择的路径
        /// </summary>
        private string _selPath;
        private string _previousString;
        private string _newString;

        public void OnEnable()
        {
            _selPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            Object[] list = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
            _previousString = _newString = "";
            
        }

        public void OnGUI()
        {
            EditorGUILayout.LabelField("CurrrentSelectedFolder:");
            EditorGUILayout.LabelField(_selPath);
            EditorGUILayout.Space();
            _previousString = EditorGUILayout.TextField("PreviousString :", _previousString);
            EditorGUILayout.Space();
            _newString = EditorGUILayout.TextField("newString :", _newString);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("Rename"))
            {
                if (_previousString == "" || _newString == "")
                {
                    UnityEditor.EditorUtility.DisplayDialog("Alert", "You should input both previous and new string first!", "ok");
                    return;
                }
                Object[] list = Selection.GetFiltered<Object>(SelectionMode.DeepAssets);
                foreach (var item in list)
                {
                    string name = item.name;
                    if (item.name.IndexOf(_previousString) != -1)
                    {
                        string newName = name.Replace(_previousString, _newString);
                        string path = AssetDatabase.GetAssetPath(item);
                        AssetDatabase.RenameAsset(path, newName);
                    }
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
