using UnityEngine;
using UnityEditor;

public class CreatePrefabTool : EditorWindow
{
    [MenuItem("YKTools/STGTools/CreateBulletPrefabs")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CreatePrefabTool));
    }

    public CreatePrefabTool()
    {
        titleContent = new GUIContent("CreatePrefabTool");
        _toggleGroupEnable = true;
        _createBulletPrefab = true;
        _createItemPrefab = false;
    }

    private bool _toggleGroupEnable;
    private bool _createBulletPrefab;
    private bool _createItemPrefab;

    private void OnGUI()
    {
        _toggleGroupEnable = EditorGUILayout.BeginToggleGroup("Select which prefab to create",_toggleGroupEnable);
        _createBulletPrefab = EditorGUILayout.Toggle("BulletPrefab", _createBulletPrefab);
        if ( _createItemPrefab )
        {

        }
        _createItemPrefab = EditorGUILayout.Toggle("ItemPrefab", _createBulletPrefab);
        if (_createItemPrefab)
        {

        }
        EditorGUILayout.EndToggleGroup();
    }
}