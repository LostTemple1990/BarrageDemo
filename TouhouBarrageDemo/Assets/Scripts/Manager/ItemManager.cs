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

    private const float ItemSpWidth = 32f;
    private const float ItemSpHeight = 32f;
    private const float ItemSpHalfWidth = ItemSpWidth / 2;
    private const float ItemSpHalfHeight = ItemSpHeight / 2;

    private ItemManager()
    {
        _itemList = new List<ItemBase>();
        _clearItemList = new List<ItemBase>();
        _itemsPool = new Dictionary<ItemType, Stack<ItemBase>>();
        //_itemTypeList = new List<ItemType>() { ItemType.PPointNormal, ItemType.PPointBig };
        // 初始化最大的缓存数目
        _maxCacheCountDatas = new Dictionary<ItemType, int>();
        _maxCacheCountDatas.Add(ItemType.PowerNormal, 100); // 小P点
    }

    private List<ItemBase> _itemList;
    private int _itemCount;

    private List<ItemBase> _clearItemList;
    private int _clearListCount;

    private Dictionary<ItemType, Stack<ItemBase>> _itemsPool;
    /// <summary>
    /// 对应最大的缓存数目
    /// </summary>
    private Dictionary<ItemType, int> _maxCacheCountDatas;
    /// <summary>
    /// item对应图像的uv数据
    /// uv按照左上，左下，右上，右下来排列
    /// </summary>
    private Vector2[][] _itemSpriteUVData;


    private GameObject _itemMeshGo;
    private Mesh _itemMesh;

    /// <summary>
    /// 道具顶点list
    /// </summary>
    private List<Vector3> _itemVerList;
    /// <summary>
    /// 道具uvlist
    /// </summary>
    private List<Vector2> _itemUVList;
    /// <summary>
    /// 道具三角形索引list
    /// </summary>
    private List<int> _itemTriList;
    /// <summary>
    /// 道具颜色list
    /// </summary>
    private List<Color> _itemColorList;

    //private List<ItemType> _itemTypeList;

    public void Init()
    {
        _itemCount = 0;
        _clearListCount = 0;
        _itemSpriteUVData = new Vector2[(int)ItemBase.eItemSpriteType.Total][];
        // PowerNormal
        _itemSpriteUVData[(int)ItemBase.eItemSpriteType.PowerNormal] =
            new Vector2[4] { new Vector2(0, 1), new Vector2(0, 0.8f), new Vector2(0.25f, 1), new Vector2(0.25f, 0.8f) };
        _itemSpriteUVData[(int)ItemBase.eItemSpriteType.PowerNormalUp] =
            new Vector2[4] { new Vector2(0.5f, 1), new Vector2(0.5f, 0.8f), new Vector2(0.75f, 1), new Vector2(0.75f, 0.8f) };
        // PowerBig
        _itemSpriteUVData[(int)ItemBase.eItemSpriteType.PowerBig] =
            new Vector2[4] { new Vector2(0.25f, 0.6f), new Vector2(0.25f, 0.4f), new Vector2(0.5f, 0.6f), new Vector2(0.5f, 0.4f) };
        _itemSpriteUVData[(int)ItemBase.eItemSpriteType.PowerBigUp] =
            new Vector2[4] { new Vector2(0.75f, 0.6f), new Vector2(0.75f, 0.4f), new Vector2(1f, 0.6f), new Vector2(1f, 0.4f) };
        // PowerFull
        _itemSpriteUVData[(int)ItemBase.eItemSpriteType.PowerFull] =
           new Vector2[4] { new Vector2(0.25f, 0.8f), new Vector2(0.25f, 0.6f), new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.6f) };
        _itemSpriteUVData[(int)ItemBase.eItemSpriteType.PowerFullUp] =
            new Vector2[4] { new Vector2(0.75f, 0.8f), new Vector2(0.75f, 0.6f), new Vector2(1f, 0.8f), new Vector2(1f, 0.6f) };
        // BombFragment
        _itemSpriteUVData[(int)ItemBase.eItemSpriteType.BombFragment] =
            new Vector2[4] { new Vector2(0, 0.2f), new Vector2(0, 0f), new Vector2(0.25f, 0.2f), new Vector2(0.25f, 0f) };
        _itemSpriteUVData[(int)ItemBase.eItemSpriteType.BombFragmentUp] =
            new Vector2[4] { new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0f), new Vector2(0.75f, 0.2f), new Vector2(0.75f, 0f) };
        // Bomb
        _itemSpriteUVData[(int)ItemBase.eItemSpriteType.Bomb] =
            new Vector2[4] { new Vector2(0.25f, 0.2f), new Vector2(0.25f, 0f), new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0f) };
        _itemSpriteUVData[(int)ItemBase.eItemSpriteType.BombUp] =
            new Vector2[4] { new Vector2(0.75f, 0.2f), new Vector2(0.75f, 0f), new Vector2(1f, 0.2f), new Vector2(1f, 0f) };
        // LifeFragment
        _itemSpriteUVData[(int)ItemBase.eItemSpriteType.LifeFragment] =
            new Vector2[4] { new Vector2(0, 0.8f), new Vector2(0, 0.6f), new Vector2(0.25f, 0.8f), new Vector2(0.25f, 0.6f) };
        _itemSpriteUVData[(int)ItemBase.eItemSpriteType.LifeFragmentUp] =
            new Vector2[4] { new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.6f), new Vector2(0.75f, 0.8f), new Vector2(0.75f, 0.6f) };
        // Life
        _itemSpriteUVData[(int)ItemBase.eItemSpriteType.Life] =
            new Vector2[4] { new Vector2(0, 0.4f), new Vector2(0, 0.2f), new Vector2(0.25f, 0.4f), new Vector2(0.25f, 0.2f) };
        _itemSpriteUVData[(int)ItemBase.eItemSpriteType.LifeUp] =
            new Vector2[4] { new Vector2(0.5f, 0.4f), new Vector2(0.5f, 0.2f), new Vector2(0.75f, 0.4f), new Vector2(0.75f, 0.2f) };

        // mesh
        _itemMeshGo = ResourceManager.GetInstance().GetPrefab("Prefab/Extra", "ItemMeshGo");
        _itemMesh = _itemMeshGo.GetComponent<MeshFilter>().mesh;
        UIManager.GetInstance().AddGoToLayer(_itemMeshGo, LayerId.Item);
        _itemVerList = new List<Vector3>();
        _itemUVList = new List<Vector2>();
        _itemTriList = new List<int>();
        _itemColorList = new List<Color>();
    }

    public void Update()
    {
        UpdateItems();
        //ClearItems();
    }

    private void UpdateItems()
    {
        int i, j;
        ItemBase item;
        bool findFlag;
        for (i = 0, j = 1; i < _itemCount; i++)
        {
            findFlag = false;
            if (_itemList[i] == null)
            {
                for (; j < _itemCount; j++)
                {
                    if (_itemList[j] != null)
                    {
                        findFlag = true;
                        _itemList[i] = _itemList[j];
                        _itemList[j] = null;
                        j++;
                        break;
                    }
                }
            }
            else
            {
                findFlag = true;
            }
            if (findFlag)
            {
                item = _itemList[i];
                item.Update();
                if (item.clearFlag == 1)
                {
                    RestoreToPool(item);
                    _itemList[i] = null;
                }
            }
            else
            {
                break;
            }
        }
        _itemList.RemoveRange(i, _itemCount - i);
        _itemCount = i;
    }

    public void Render()
    {
        _itemMesh.Clear();
        _itemVerList.Clear();
        _itemUVList.Clear();
        _itemTriList.Clear();
        _itemColorList.Clear();
        int verIndex = 0;
        for (int i = 0; i < _itemCount; i++)
        {
            if (_itemList[i] != null)
            {
                ItemBase.STGItemRenderData data = _itemList[i].GetRenderData();
                BuildItemMesh(data.spType, data.pos, ref verIndex);
            }
        }
        _itemMesh.vertices = _itemVerList.ToArray();
        _itemMesh.uv = _itemUVList.ToArray();
        _itemMesh.triangles = _itemTriList.ToArray();
    }

    private void BuildItemMesh(ItemBase.eItemSpriteType spType,Vector2 centerPos, ref int verIndex)
    {
        _itemVerList.Add(new Vector3(centerPos.x - ItemSpHalfWidth, centerPos.y + ItemSpHalfHeight));
        _itemVerList.Add(new Vector3(centerPos.x - ItemSpHalfWidth, centerPos.y - ItemSpHalfHeight));
        _itemVerList.Add(new Vector3(centerPos.x + ItemSpHalfWidth, centerPos.y + ItemSpHalfHeight));
        _itemVerList.Add(new Vector3(centerPos.x + ItemSpHalfWidth, centerPos.y - ItemSpHalfHeight));
        Vector2[] spUV = _itemSpriteUVData[(int)spType];
        for (int i = 0; i < spUV.Length; i++)
        {
            _itemUVList.Add(spUV[i]);
        }
        _itemTriList.Add(verIndex);
        _itemTriList.Add(verIndex + 3);
        _itemTriList.Add(verIndex + 1);
        _itemTriList.Add(verIndex);
        _itemTriList.Add(verIndex + 2);
        _itemTriList.Add(verIndex + 3);
        verIndex += 4;
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

    /// <summary>
    /// 掉落物品
    /// </summary>
    /// <param name="itemDatas">结构 type,count</param>
    /// <para>举例： 1,5,2,3,5,6</para>
    /// <para>即生成type=1的5个，type=2的3个,type=5的6个</para>
    /// <param name="centerX"></param>
    /// <param name="centerY"></param>
    /// <param name="halfWidth"></param>
    /// <param name="halfHeight"></param>
    public void DropItems(List<int> itemDatas, float centerX, float centerY, float halfWidth, float halfHeight)
    {
        float beginX = centerX - halfWidth;
        float endX = centerX + halfWidth;
        float beginY = centerY - halfHeight;
        float endY = centerY + halfHeight;
        ItemType itemType;
        int itemCount;
        for (int i = 0, len = itemDatas.Count; i < len; i += 2)
        {
            itemType = (ItemType)itemDatas[i];
            itemCount = itemDatas[i + 1];
            for (int j = 0; j < itemCount; j++)
            {
                CreateItemByItemType(itemType, MTRandom.GetNextFloat(beginX, endX), MTRandom.GetNextFloat(beginY, endY));
            }
        }
    }

    public void CreateItemByItemType(ItemType itemType,float posX,float posY)
    {
        ItemBase item = GetAtPool(itemType);
        if ( item != null )
        {
            item.Init();
            item.SetToPosition(posX, posY);
            _itemList.Add(item);
            _itemCount++;
        }
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
            case ItemType.PowerNormal:
                item = new ItemPowerNormal();
                break;
            case ItemType.PowerBig:
                item = new ItemPowerBig();
                break;
            case ItemType.PowerFull:
                item = new ItemPowerFull();
                break;
            case ItemType.LifeFragment:
                item = new ItemLifeFragment();
                break;
            case ItemType.Life:
                item = new ItemLife();
                break;
            case ItemType.BombFragment:
                item = new ItemBombFragment();
                break;
            case ItemType.Bomb:
                item = new ItemBomb();
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
        _itemMesh.Clear();
        _itemVerList.Clear();
        _itemUVList.Clear();
        _itemTriList.Clear();
        _itemColorList.Clear();
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
            if (!_maxCacheCountDatas.TryGetValue(type, out maxCacheCount))
                maxCacheCount = 0;
            if (_itemsPool.TryGetValue(type, out stack))
            {
                int stackCount = stack.Count;
                if (stackCount > maxCacheCount)
                {
                    int count = 0;
                    while (count <= Consts.MaxDestroyItemCountPerFrame && stackCount > maxCacheCount)
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
        return false;
    }
}
