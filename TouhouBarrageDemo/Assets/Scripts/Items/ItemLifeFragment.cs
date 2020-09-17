using UnityEngine;
using System.Collections;

public class ItemLifeFragment : ItemBase
{
    public override void Init()
    {
        _itemType = ItemType.LifeFragment;
        _collisionHalfWidth = _collisionHalfHeight = 11;
        _halfWidth = _halfHeight = 11;
        _upPosY = Consts.ItemTopBorderY - 16;
        //_itemGO = ResourceManager.GetInstance().GetPrefab("Item", "ItemLifeFragment");
        //UIManager.GetInstance().AddGoToLayer(_itemGO, LayerId.Item);
        //_defaultSp = "LifeFragment";
        //_upSp = "LifeFragment_Up";
        //_sr = _itemGO.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        _defaultSpType = eItemSpriteType.LifeFragment;
        _upSpType = eItemSpriteType.LifeFragmentUp;
        base.Init();
    }

    protected override void DoEffect()
    {
        PlayerInterface.GetInstance().AddLifeFragmentCount(1);
    }

    public override void Clear()
    {
        //UIManager.GetInstance().RemoveGoFromLayer(_itemGO);
        base.Clear();
    }
}
