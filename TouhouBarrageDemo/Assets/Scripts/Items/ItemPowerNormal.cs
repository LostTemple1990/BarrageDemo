using UnityEngine;
using System.Collections;

public class ItemPowerNormal : ItemBase
{
    public override void Init()
    {
        _itemType = ItemType.PowerNormal;
        _itemGO = ResourceManager.GetInstance().GetPrefab("Item", "ItemPowerNormal");
        UIManager.GetInstance().AddGoToLayer(_itemGO, LayerId.Item);
        _collisionHalfWidth = _collisionHalfHeight = 6;
        _halfWidth = _halfHeight = 6;
        _upPosY = Consts.ItemTopBorderY - 8;
        _defaultSp = "PowerNormal";
        _upSp = "PowerNormal_Up";
        _sr = _itemGO.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        base.Init();
    }

    protected override void DoEffect()
    {
        PlayerInterface.GetInstance().AddPower(10);
    }

    public override void Clear()
    {
        UIManager.GetInstance().RemoveGoFromLayer(_itemGO);
        base.Clear();
    }
}
