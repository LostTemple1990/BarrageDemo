using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundBase
{
    private static int ClearDuration = 60;

    protected GameObject _bgLayer;
    protected Transform _bgTf;
    protected Camera _bgCamera;
    protected Transform _bgCameraTf;
    // 前景层的tf组件
    protected Transform _foregroundLayerTf;

    protected List<BgBlockContainer> _containerList;
    /// <summary>
    /// 背景精灵对象
    /// </summary>
    protected List<BgSpriteObject> _spriteObjList;
    /// <summary>
    /// 背景精灵对象的数目
    /// </summary>
    protected int _spriteObjCount;

    protected int _clearTime;

    protected Vector3 _curPos;

    public BackgroundBase()
    {

    }

    public virtual void Init(Transform rootTf)
    {
        _bgTf = rootTf.transform.Find("BgLayer");
        _bgCameraTf = rootTf.transform.Find("BgCamera");
        _bgCamera = _bgCameraTf.GetComponent<Camera>();
        // 初始化前景层
        _foregroundLayerTf = _bgCameraTf.Find("ForegroundLayer");

        _containerList = new List<BgBlockContainer>();
        _spriteObjList = new List<BgSpriteObject>();
        _spriteObjCount = 0;
    }

    public BgBlockContainer CreateBgBlockContainer(string prefabName, int count)
    {
        GameObject blockObj = ResourceManager.GetInstance().GetPrefab("Prefab/Background", prefabName);
        BgBlockContainer container = new BgBlockContainer(_bgTf);
        container.Init();
        container.SetBlockObject(blockObj, count);
        //添加到列表中
        _containerList.Add(container);
        return container;
    }

    /// <summary>
    /// 创建单个背景精灵
    /// </summary>
    /// <param name="spName"></param>
    public BgSpriteObject CreateBgSpriteObject(string spName)
    {
        BgSpriteObject spObj = ObjectsPool.GetInstance().GetPoolClassAtPool<BgSpriteObject>();
        spObj.Init(_foregroundLayerTf);
        spObj.SetSprite(spName);
        AddBgSpriteObject(spObj);
        return spObj;
    }

    /// <summary>
    /// 将背景精灵添加到列表中
    /// </summary>
    /// <param name="spObj"></param>
    public void AddBgSpriteObject(BgSpriteObject spObj)
    {
        _spriteObjList.Add(spObj);
        _spriteObjCount++;
    }

    public virtual void Update(int curFrame)
    {
        UpdateContainers();
        UpdateSpriteObjects();
    }

    protected void UpdateContainers()
    {
        foreach (BgBlockContainer container in _containerList)
        {
            container.Update();
        }
    }

    protected void UpdateSpriteObjects()
    {
        BgSpriteObject spObj;
        for (int i = 0; i < _spriteObjCount; i++)
        {
            spObj = _spriteObjList[i];
            if (spObj != null)
            {
                spObj.Update();
                if (!spObj.IsActive)
                {
                    ObjectsPool.GetInstance().RestorePoolClassToPool(spObj);
                    _spriteObjList[i] = null;
                }
            }
        }
        _clearTime++;
        if (_clearTime >= ClearDuration)
        {
            _clearTime = 0;
            _spriteObjCount = CommonUtils.RemoveNullElementsInList<BgSpriteObject>(_spriteObjList, _spriteObjCount);
        }
    }

    protected void UpdatePos()
    {
        _bgTf.localPosition = _curPos;
    }

    public virtual void Clear()
    {
        int i;
        foreach (BgBlockContainer container in _containerList)
        {
            container.Clear();
        }
        _containerList.Clear();
        BgSpriteObject spObj;
        for (i = 0; i < _spriteObjCount; i++)
        {
            spObj = _spriteObjList[i];
            if (spObj != null)
            {
                ObjectsPool.GetInstance().RestorePoolClassToPool(spObj);
            }
        }
        _spriteObjList.Clear();
        _spriteObjCount = 0;
    }
}
