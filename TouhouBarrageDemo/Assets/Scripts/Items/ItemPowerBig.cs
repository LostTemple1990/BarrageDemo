﻿using UnityEngine;
using System.Collections;

public class ItemPowerBig : ItemBase
{
    public override void Init()
    {
        _itemType = ItemType.PowerBig;
        _collisionHalfWidth = _collisionHalfHeight = 11;
        _halfWidth = _halfHeight = 11;
        _upPosY = Consts.ItemTopBorderY - 16;
        //_itemGO = ResourceManager.GetInstance().GetPrefab("Item", "ItemPowerBig");
        //UIManager.GetInstance().AddGoToLayer(_itemGO, LayerId.Item);
        //_defaultSp = "PowerBig";
        //_upSp = "PowerBig_Up";
        //_sr = _itemGO.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        _defaultSpType = eItemSpriteType.PowerBig;
        _upSpType = eItemSpriteType.PowerBigUp;
        base.Init();
    }

    protected override void DoEffect()
    {
        PlayerInterface.GetInstance().AddPower(100);
    }

    public override void Clear()
    {
        //UIManager.GetInstance().RemoveGoFromLayer(_itemGO);
        base.Clear();
    }
}
