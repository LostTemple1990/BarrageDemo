using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManager
{
    private static ItemManager _instance = new ItemManager();

    public static ItemManager GetInstance()
    {
        return _instance;
    }

    private ItemManager()
    {
        _itemList = new List<ItemBase>();
        _clearItemList = new List<ItemBase>();
        _dropDatabase = DataManager.GetInstance().GetDatasByName("DropItemsCfgs") as Dictionary<string, IParser>;
        _itemsPool = new Dictionary<ItemType, Stack<ItemBase>>();
        //_itemTypeList = new List<ItemType>() { ItemType.PPointNormal, ItemType.PPointBig };
        // 初始化最大的缓存数目
        _maxCacheCountDatas = new Dictionary<ItemType, int>();
        _maxCacheCountDatas.Add(ItemType.PPointNormal, 100); // 小P点
    }

    private Dictionary<string, IParser> _dropDatabase;

    private List<ItemBase> _itemList;
    private int _itemCount;

    private List<ItemBase> _clearItemList;
    private int _clearListCount;

    private Dictionary<ItemType, Stack<ItemBase>> _itemsPool;
    /// <summary>
    /// 对应最大的缓存数目
    /// </summary>
    private Dictionary<ItemType, int> _maxCacheCountDatas;

    //private List<ItemType> _itemTypeList;

    public void Init()
    {
        _itemCount = 0;
        _clearListCount = 0;
    }

    public void Update()
    {
        UpdateItems();
        ClearItems();
    }
    
    private void UpdateItems()
    {
        int i;
        ItemBase item;
        for (i=0;i<_itemCount;i++)
        {
            item = _itemList[i];
            _itemList[i].Update();
            if ( item.clearFlag == 1 )
            {
                _clearItemList.Add(_itemList[i]);
                _clearListCount++;
                _itemList[i] = null;
            }
        }
    }

    /// <summary>
    /// 将clearList里面的item放回pool
    /// <para>todo 每隔一段时间再检测一次</para>
    /// </summary>
    private void ClearItems()
    {
        int i,j;
        int findFlag;
        for (i = 0; i < _clearListCount; i++)
        {
            RestoreToPool(_clearItemList[i]);
        }
        _clearItemList.Clear();
        _clearListCount = 0;
        for (i=0,j=1;i<_itemCount;i++)
        {
            if ( _itemList[i] == null )
            {
                findFlag = 0;
                j = j == 1 ? i + 1 : j;
                for ( ;j<_itemCount;j++)
                {
                    if ( _itemList[j] != null )
                    {
                        _itemList[i] = _itemList[j];
                        _itemList[j] = null;
                        findFlag = 1;
                        j++;
                        break;
                    }
                }
                if ( findFlag == 0 )
                {
                    break;
                }
            }
        }
        _itemList.RemoveRange(i, _itemCount - i);
        _itemCount = i;
    }

    public void DropItems(string id,float centerX,float centerY,float halfWidth,float halfHeight)
    {
        float beginX = centerX - halfWidth;
        float endX = centerX + halfWidth;
        float beginY = centerY - halfHeight;
        float endY = centerY + halfHeight;
        ItemType itemType;
        int i;
        DropItemsCfg cfg = GetDropCfgById(id);
        if (cfg != null)
        {
            itemType = ItemType.PPointNormal;
            for (i=0;i<cfg.pPointN;i++)
            {
                CreateItemByItemType(itemType, MTRandom.GetNextFloat(beginX, endX), MTRandom.GetNextFloat(beginY, endY));
            }
            itemType = ItemType.PPointBig;
            for (i = 0; i < cfg.pPointB; i++)
            {
                CreateItemByItemType(itemType, MTRandom.GetNextFloat(beginX, endX), MTRandom.GetNextFloat(beginY, endY));
            }
        }
    }

    public DropItemsCfg GetDropCfgById(string id)
    {
        IParser parser;
        if ( !_dropDatabase.TryGetValue(id,out parser) )
        {
            return null;
        }
        return parser as DropItemsCfg;
    }

    public void CreateItemByItemType(ItemType itemType,float posX,float posY)
    {
        ItemBase item;
        item = GetAtPool(itemType);
        item.Init();
        item.SetToPosition(posX, posY);
        _itemList.Add(item);
        _itemCount++;
        //Logger.Log("CreateItem at pos " + posX + " , " + posY);
    }

    public ItemBase GetAtPool(ItemType itemType)
    {
        Stack<ItemBase> stack;
        if ( _itemsPool.TryGetValue(itemType,out stack) )
        {
            if (stack.Count > 0)
            {
                return stack.Pop();
            }
        }
        else
        {
            stack = new Stack<ItemBase>();
            _itemsPool.Add(itemType, stack);
        }
        ItemBase item;
        switch (itemType)
        {
            case ItemType.PPointNormal:
                item = new ItemPPointNormal();
                break;
            default:
                item = null;
                break;
        }
        return item;
    }

    public void RestoreToPool(ItemBase item)
    {
        Stack<ItemBase> stack;
        if ( _itemsPool.TryGetValue(item.itemType, out stack) )
        {
            item.Clear();
            stack.Push(item);
        }
    }

    /// <summary>
    /// Clear函数
    /// <para>ItemManager执行clear不清除对象池</para>
    /// <para>会针对每个item设置一个最大数目</para>
    /// <para>会定期清除高于这个最大数目的部分</para>
    /// </summary>
    public void Clear()
    {
        int i;
        ItemBase item;
        for (i = 0; i < _itemCount; i++)
        {
            item = _itemList[i];
            if ( item != null )
            {
                RestoreToPool(item);
            }
        }
        _itemList.Clear();
        _itemCount = 0;
    }

    /// <summary>
    /// 清除额外的item的cache
    /// </summary>
    /// <returns>返回true说明已经执行过销毁</returns>
    public bool DestroyExtraCache()
    {
        Dictionary<ItemType,Stack<ItemBase>>.KeyCollection keys = _itemsPool.Keys;
        Stack<ItemBase> stack;
        ItemBase item;
        foreach (ItemType type in keys)
        {
            int maxCacheCount;
            if ( _maxCacheCountDatas.TryGetValue(type,out maxCacheCount) )
            {
                if ( _itemsPool.TryGetValue(type, out stack) )
                {
                    int stackCount = stack.Count;
                    if ( stack.Count > maxCacheCount )
                    {
                        int count = 0;
                        while ( count <= Consts.MaxDestroyCountPerFrame && stackCount > maxCacheCount )
                        {
                            // 销毁对象
                            item = stack.Pop();
                            item.Destroy();
                            // 更新计数
                            count++;
                            stackCount--;
                        }
                    }
                    return true;
                }
            }
        }
        return false;
    }
}

public enum ItemType
{
    PPointNormal = 1,
    PPointBig = 2,
}
