using UnityEngine;
using System.Collections.Generic;

public class AnimationManager 
{
    private static AnimationManager _instance;

    public static AnimationManager GetInstance()
    {
        if ( _instance == null )
        {
            _instance = new AnimationManager();
        }
        return _instance;
    }

    private Dictionary<string, AnimationCache> _aniCacheDic;
    /// <summary>
    /// 记录着对应textures里面所有sprites的map
    /// </summary>
    private Dictionary<string, Sprite[]> _texturesMap;
    /// <summary>
    /// key - texture里面的aniName
    /// value - 对应aniName里面的所有sprite的map
    /// </summary>
    private Dictionary<string, Dictionary<string, Sprite>> _textureSpsMap;

    private List<AnimationBase> _aniList;
    private int _aniCount;

    private Dictionary<string, IParser> _unitAniDatas;

    private AnimationManager()
    {
        _aniCacheDic = new Dictionary<string, AnimationCache>();
        _texturesMap = new Dictionary<string, Sprite[]>();
        _textureSpsMap = new Dictionary<string, Dictionary<string, Sprite>>();
        _aniList = new List<AnimationBase>();
        _aniCount = 0;
    }

    public void Init()
    {
        _unitAniDatas = DataManager.GetInstance().GetDatasByName("AnimationCfgs") as Dictionary<string, IParser>;
        foreach(string key in _unitAniDatas.Keys)
        {
            LoadAniCache(key);
        }
    }

    public void Update()
    {
        int i;
        AnimationBase ani;
        for (i=0;i<_aniCount;i++)
        {
            ani = _aniList[i];
            if ( ani != null )
            {
                _aniList[i].Update();
            }
        }
        _aniCount = CommonUtils.RemoveNullElementsInList<AnimationBase>(_aniList,_aniCount);
    }

    //public void LoadAniCache(string aniId)
    //{
    //    UnitAniCfg cfg = GetUnitAniCfgById(aniId);
    //    string aniName = cfg.aniName;
    //    string textureName = cfg.textureName;
    //    string path = "Animation/" + textureName;
    //    Sprite[] sprites;
    //    int len, i,j;
    //    Dictionary<string, Sprite> aniSpsMap;
    //    // 判断是否已经将该贴图的所有sprites存入map中
    //    if (!_texturesMap.TryGetValue(textureName, out sprites))
    //    {
    //        sprites = Resources.LoadAll<Sprite>(path);
    //        _texturesMap.Add(textureName, sprites);
    //    }
    //    // 提取texture中名字带有aniName的所有sprite，存入aniSpsMap中
    //    aniSpsMap = new Dictionary<string, Sprite>();
    //    _textureSpsMap.Add(aniName, aniSpsMap);
    //    len = sprites.Length;
    //    for (i=0;i<len;i++)
    //    {
    //        if ( sprites[i].name.IndexOf(aniName) != -1 )
    //        {
    //            aniSpsMap.Add(sprites[i].name, sprites[i]);
    //        }
    //    }
    //    // 将对应的sp数组放入aniCache中
    //    Sprite sp;
    //    Dictionary<string, Sprite[]> cacheSps = new Dictionary<string, Sprite[]>();
    //    string[] sliceStrs = cfg.aniData.Split(',');
    //    for (i=0,len=sliceStrs.Length;i<len;i=i+3)
    //    {
    //        string actionName = sliceStrs[i];
    //        int aniLen = int.Parse(sliceStrs[i + 1]);
    //        sprites = new Sprite[aniLen];
    //        for (j=0;j<aniLen;j++)
    //        {
    //            if (aniSpsMap.TryGetValue(aniName + "_" + actionName + "_" + j, out sp))
    //            {
    //                sprites[j] = sp;
    //            }
    //        }
    //        cacheSps.Add(actionName, sprites);
    //    }
    //    // 创建AnimationCache
    //    AnimationCache aniCache = new AnimationCache();
    //    aniCache.AniName = aniName;
    //    aniCache.SetActionSpritesMap(cacheSps);
    //    _aniCacheDic.Add(aniName,aniCache);
    //}

    public void LoadAniCache(string aniId)
    {
        UnitAniCfg cfg = GetUnitAniCfgById(aniId);
        string aniName = cfg.aniName;
        string textureName = cfg.textureName;
        Sprite[] sprites;
        int len, i, j;
        Dictionary<string, Sprite[]> cacheSps = new Dictionary<string, Sprite[]>();
        string[] sliceStrs = cfg.aniData.Split(',');
        string actionAniFrameName;
        for (i = 0, len = sliceStrs.Length; i < len; i = i + 3)
        {
            string actionName = sliceStrs[i];
            int aniLen = int.Parse(sliceStrs[i + 1]);
            sprites = new Sprite[aniLen];
            for (j = 0; j < aniLen; j++)
            {
                actionAniFrameName = aniName + "_" + actionName + "_" + j;
                sprites[j] = ResourceManager.GetInstance().GetSprite(textureName, actionAniFrameName);
            }
            cacheSps.Add(actionName, sprites);
        }
        // 创建AnimationCache
        AnimationCache aniCache = new AnimationCache();
        aniCache.AniName = aniName;
        aniCache.SetActionSpritesMap(cacheSps);
        _aniCacheDic.Add(aniName, aniCache);
    }

    public AnimationCache GetAnimationCache(string aniName)
    {
        AnimationCache cache;
        if ( _aniCacheDic.TryGetValue(aniName,out cache) )
        {
            return cache;
        }
        return null;
    }

    public T CreateAnimation<T>(LayerId id)
        where T : AnimationBase,new()
    {
        T ani = new T();
        ani.Init(id);
        _aniList.Add(ani);
        _aniCount++;
        return ani;
    }

    public T CreateAnimation<T>(GameObject aniParent,string rendererName,LayerId id)
        where T : AnimationBase, new()
    {
        T ani = new T();
        ani.Init(aniParent,rendererName,id);
        _aniList.Add(ani);
        _aniCount++;
        return ani;
    }

    public UnitAniCfg GetUnitAniCfgById(string id)
    {
        IParser cfg;
        if ( !_unitAniDatas.TryGetValue(id,out cfg) )
        {
            return null;
        }
        return cfg as UnitAniCfg;
    }

    public void RemoveAnimation(AnimationBase ani)
    {
        if ( ani != null )
        {
            for (int i=0;i<_aniCount;i++)
            {
                if ( _aniList[i] == ani )
                {
                    ani.Clear();
                    _aniList[i] = null;
                }
            }
        }
    }
}