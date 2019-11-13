using UnityEngine;
using System.Collections;

public class ItemPowerFull : ItemBase
{
    public override void Init()
    {
        _itemType = ItemType.PowerFull;
        _itemGO = ResourceManager.GetInstance().GetPrefab("Item", "ItemPowerFull");
        UIManager.GetInstance().AddGoToLayer(_itemGO, LayerId.Item);
        _collisionHalfWidth = _collisionHalfHeight = 11;
        _halfWidth = _halfHeight = 11;
        _aboveY = Consts.ItemTopBorderY + _halfHeight;
        _defaultSp = "PowerFull";
        _upSp = "PowerFull_Up";
        _sr = _itemGO.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        base.Init();
    }

    protected override void DoEffect()
    {
        PlayerInterface.GetInstance().AddPower(400);
    }

    public override void Clear()
    {
        UIManager.GetInstance().RemoveGoFromLayer(_itemGO);
        base.Clear();
    }
}
