using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager {
    private static ResourceManager _instance;

    public static ResourceManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new ResourceManager();
        }
        return _instance;
    }

    private ResourceManager()
    {

    }

    private Dictionary<string, ResourceData> _dic;
    private Material _spriteDefaultMat;

    public void Init()
    {
        _dic = new Dictionary<string, ResourceData>();
        ParseResource("pl00", 28);
        ParseResource("etama2", 54);
        ParseResource("etama", 250);
        ParseResource("etama9", 16);
        ParseSpriteAtlas("Effects", "Effects");
    }

    /// <summary>
    /// 根据配置解析资源，临时使用resource.load
    /// </summary>
    private void ParseResource(string packName,int count)
    {
        ResourceData resData = new ResourceData();
        resData.Init(new string[]{ packName });
        Dictionary<string, Object> resMap = new Dictionary<string, Object>();
        Texture2D texture;
        Sprite sp;
        for (int i=0;i<count;i++)
        {
            texture = Resources.Load(packName + "/" + packName + "_" + i) as Texture2D;
            if ( texture != null )
            {
                sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                resMap.Add(packName + "_" + i, sp);
            }
        }
        resData.datasDic = resMap;
        _dic.Add(packName, resData);
    }

    private void ParseSpriteAtlas(string packName,string atlasName)
    {
        ResourceData resData = new ResourceData();
        resData.Init(new string[] { packName });
        Dictionary<string, Object> resMap = new Dictionary<string, Object>();
        resData.datasDic = resMap;
        _dic.Add(atlasName, resData);

        string path = packName == "" ? atlasName : packName + "/" + atlasName;
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        int len = sprites.Length;
        for (int i=0;i<len;i++)
        {
            resMap.Add(sprites[i].name, sprites[i]);
        }
    }

    public Sprite GetSprite(string packName,string resName)
    {
        ResourceData resData;
        if ( _dic.TryGetValue(packName,out resData) )
        {
            if ( resData != null )
            {
                Sprite sp = resData.GetObject(resName) as Sprite;
                return sp;
            }
        }
        //Resources.Load<Sprite>()
        //Sprite sp = Resources.Load<Sprite>(packName + "/" + resName);
        return null;
    }

    public T GetResource<T>(string packName,string resName)
    {
        ResourceData resData;
        if (_dic.TryGetValue(packName, out resData))
        {
            return (T)resData.GetObject(resName);
        }
        return default(T);
    }

    public GameObject GetPrefab(string packName,string resName)
    {
        GameObject prefab = ObjectsPool.GetInstance().GetPrefabAtPool(resName);
        if ( prefab == null )
        {
            Object obj = Resources.Load(packName + "/" + resName);
            prefab = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
        }
        return prefab;
    }

    /// <summary>
    /// 获取精灵默认的材质
    /// </summary>
    /// <returns></returns>
    public Material GetSpriteDefualtMaterial()
    {
        if ( _spriteDefaultMat == null )
        {
            _spriteDefaultMat = Resources.Load<Material>("materials/SpriteDefaultMat");
        }
        return _spriteDefaultMat;
    }
}
