using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CreatePrefabTool : EditorWindow
{
    private static List<string> ValidExtensions;

    [MenuItem("YKTools/STGTools/CreateBulletPrefabs")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CreatePrefabTool));
    }

    public CreatePrefabTool()
    {
        // 支持的文件扩展名
        ValidExtensions = new List<string>();
        ValidExtensions.Add(".png");
        titleContent = new GUIContent("CreatePrefabTool");
        _toggleGroupEnable = true;
        _createBulletPrefab = true;
        _createItemPrefab = false;
        _createType = ECreatePrefabType.Bullet;
    }

    private bool _toggleGroupEnable;
    private bool _createBulletPrefab;
    private bool _createItemPrefab;
    private ECreatePrefabType _createType;

    private string _imageDirectory = "";
    private bool _selectImageDirBtn;
    private string _prefabDirectory = "";
    private bool _selectPrefabDirBtn;
    /// <summary>
    /// 生成prefab的按钮
    /// </summary>
    private bool _createPrefabsBtn;

    private void OnGUI()
    {
        EditorGUILayout.Space();
        _createType = (ECreatePrefabType)EditorGUILayout.EnumPopup("CreateType : ", _createType);
        GUILayout.Space(5);
        EditorGUILayout.LabelField("ImageDirectory", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        _imageDirectory = EditorGUILayout.TextField(_imageDirectory, GUILayout.Width(320));
        _selectImageDirBtn = GUILayout.Button("Browse");
        if (_selectImageDirBtn)
        {
            _imageDirectory = EditorUtility.OpenFolderPanel("SelectImageFolder", _imageDirectory, "");
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        // 输出位置
        EditorGUILayout.LabelField("OutputDirectory", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        _prefabDirectory = EditorGUILayout.TextField(_prefabDirectory, GUILayout.Width(320));
        _selectPrefabDirBtn = GUILayout.Button("Browse");
        if (_selectPrefabDirBtn)
        {
            _prefabDirectory = EditorUtility.OpenFolderPanel("SelectOutputPrefabFolder", _prefabDirectory, "");
        }
        EditorGUILayout.EndHorizontal();
        // 创建按钮
        EditorGUILayout.Space();
        _createPrefabsBtn = GUILayout.Button("CreatePrefabs");
        if (_createPrefabsBtn)
        {
            // 检测输入以及输出目录的合法性
            if (!System.IO.Directory.Exists(_imageDirectory))
            {
                EditorUtility.DisplayDialog("Error!", "InputDir is not valid!", "confirm");
                return;
            }
            if (!System.IO.Directory.Exists(_prefabDirectory))
            {
                EditorUtility.DisplayDialog("Error!", "OutputDir is not valid!", "confirm");
                return;
            }
            //List<string> paths = GetAllFilesAtPath(_inputDir);
            if (_createType == ECreatePrefabType.Bullet)
            {
                CreateBulletPrefab(_imageDirectory, _prefabDirectory);
            }
            else if (_createType == ECreatePrefabType.Item)
            {
                CreateItemPrefab(_imageDirectory, _prefabDirectory);
            }
        }

        //_toggleGroupEnable = EditorGUILayout.BeginToggleGroup("Select which prefab to create",_toggleGroupEnable);
        //_createBulletPrefab = EditorGUILayout.Toggle("BulletPrefab", _createBulletPrefab);
        //if ( _createItemPrefab )
        //{

        //}
        //_createItemPrefab = EditorGUILayout.Toggle("ItemPrefab", _createBulletPrefab);
        //if (_createItemPrefab)
        //{

        //}
        //EditorGUILayout.EndToggleGroup();
    }

    private void CreateBulletPrefab(string inputPath, string outputPath)
    {
        List<string> _filePaths = GetAllFilesAtPath(inputPath);
        // 为了转化成相对路径，需要截取掉前面的绝对路径部分
        int removeCount = Application.dataPath.Length - 6;
        // 将outputPath转化成相对路径
        outputPath = outputPath.Remove(0, removeCount);
        // 创建item模板
        RectTransform rectTf;
        GameObject go = new GameObject();
        rectTf = go.AddComponent<RectTransform>();
        rectTf.sizeDelta = new Vector2(0, 0);
        rectTf.localPosition = new Vector3(0, 300, 0);
        //子对象
        GameObject child = new GameObject("BulletSprite");
        rectTf = child.AddComponent<RectTransform>();
        rectTf.SetParent(go.transform, false);
        rectTf.localPosition = Vector3.zero;
        rectTf.localScale = new Vector3(100, 100, 1);
        // sp
        SpriteRenderer spRenderer = child.AddComponent<SpriteRenderer>();
        // 设置sortingLayer
        spRenderer.sortingLayerName = "STG";
        int count = _filePaths.Count;
        Sprite sp;
        string relativePath;
        // 循环遍历所有的图像
        for (int i = 0; i < count; i++)
        {
            // 获取相对路径
            relativePath = _filePaths[i].Remove(0, removeCount);
            sp = AssetDatabase.LoadAssetAtPath<Sprite>(relativePath);
            // 赋值
            spRenderer.sprite = sp;
            // 设置prefab名称
            go.name = sp.name;
            // 设置大小
            rectTf.sizeDelta = new Vector2(sp.texture.width, sp.texture.height);
            // 创建prefab
            PrefabUtility.CreatePrefab(outputPath + "/" + sp.name + ".prefab", go);
        }
        DestroyImmediate(go);
    }

    private void CreateItemPrefab(string inputPath, string outputPath)
    {
        List<string> _filePaths = GetAllFilesAtPath(inputPath);
        // 为了转化成相对路径，需要截取掉前面的绝对路径部分
        int removeCount = Application.dataPath.Length - 6;
        // 将outputPath转化成相对路径
        outputPath = outputPath.Remove(0, removeCount);
        // 创建item模板
        RectTransform rectTf;
        GameObject go = new GameObject();
        rectTf = go.AddComponent<RectTransform>();
        rectTf.sizeDelta = new Vector2(0, 0);
        rectTf.localPosition = new Vector3(0, 300, 0);
        //子对象
        GameObject child = new GameObject("Sprite");
        rectTf = child.AddComponent<RectTransform>();
        rectTf.SetParent(go.transform, false);
        rectTf.localPosition = Vector3.zero;
        rectTf.localScale = new Vector3(100, 100, 1);
        // sp
        SpriteRenderer spRenderer = child.AddComponent<SpriteRenderer>();
        // 设置sortingLayer
        spRenderer.sortingLayerName = "STG";
        int count = _filePaths.Count;
        Sprite sp;
        string relativePath;
        // 循环遍历所有的图像
        for (int i = 0; i < count; i++)
        {
            // 获取相对路径
            relativePath = _filePaths[i].Remove(0, removeCount);
            sp = AssetDatabase.LoadAssetAtPath<Sprite>(relativePath);
            // 赋值
            spRenderer.sprite = sp;
            // 设置prefab名称
            go.name = sp.name;
            // 设置大小
            rectTf.sizeDelta = new Vector2(sp.texture.width, sp.texture.height);
            // 创建prefab
            PrefabUtility.CreatePrefab(outputPath + "/" + sp.name + ".prefab", go);
        }
        DestroyImmediate(go);
    }

    public List<string> GetAllFilesAtPath(string path)
    {
        List<string> paths = new List<string>();
        if (System.IO.Directory.Exists(path))
        {
            int len;
            string[] filePaths = System.IO.Directory.GetFiles(path);
            len = filePaths.Length;
            string ext;
            for (int i = 0; i < len; i++)
            {
                // 检测文件是否符合要求
                ext = System.IO.Path.GetExtension(filePaths[i]).ToLower();
                if (ValidExtensions.IndexOf(ext) != -1)
                {
                    paths.Add(filePaths[i]);
                }
            }
            // 遍历子文件夹
            string[] subDirPaths = System.IO.Directory.GetDirectories(path);
            len = subDirPaths.Length;
            List<string> retPaths;
            for (int i = 0; i < len; i++)
            {
                retPaths = GetAllFilesAtPath(subDirPaths[i]);
                paths.AddRange(retPaths);
            }
        }
        return paths;
    }
}

public enum ECreatePrefabType : byte
{
    Bullet = 1,
    Item = 2,
}