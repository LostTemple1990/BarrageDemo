using UnityEngine;
using System.Collections;

public class ItemLife : ItemBase
{
    public override void Init()
    {
        _itemType = ItemType.Life;
        _itemGO = ResourceManager.GetInstance().GetPrefab("Item", "ItemLife");
        UIManager.GetInstance().AddGoToLayer(_itemGO, LayerId.Item);
        _collisionHalfWidth = _collisionHalfHeight = 11;
        _halfWidth = _halfHeight = 11;
        _aboveY = Consts.ItemTopBorderY + _halfHeight;
        _defaultSp = "ItemLife";
        _upSp = "ItemLife_Up";
        _sr = _itemGO.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        base.Init();
    }

    protected override void DoEffect()
    {
        PlayerInterface.GetInstance().AddLifeCount(1);
    }

    public override void Clear()
    {
        UIManager.GetInstance().RemoveGoFromLayer(_itemGO);
        base.Clear();
    }
}
