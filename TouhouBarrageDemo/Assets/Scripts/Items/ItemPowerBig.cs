﻿using UnityEngine;
using System.Collections;

public class ItemPowerBig : ItemBase
{
    public override void Init()
    {
        _itemType = ItemType.PowerBig;
        _itemGO = ResourceManager.GetInstance().GetPrefab("item", "PowerBig");
        UIManager.GetInstance().AddGoToLayer(_itemGO, LayerId.Item);
        _collisionHalfWidth = _collisionHalfHeight = 11;
        _halfWidth = _halfHeight = 11;
        _aboveY = Consts.ItemTopBorderY + _halfHeight;
        _defaultSp = "PowerBig";
        _upSp = "PowerBig_Up";
        _sr = _itemGO.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        base.Init();
    }

    protected override void DoEffect()
    {
        PlayerInterface.GetInstance().AddPower(100);
    }

    public override void Clear()
    {
        UIManager.GetInstance().RemoveGoFromLayer(_itemGO);
        base.Clear();
    }
}
