﻿using UnityEngine;
using System.Collections;

public class ItemLife : ItemBase
{
    public override void Init()
    {
        _itemType = ItemType.Life;
        _collisionHalfWidth = _collisionHalfHeight = 11;
        _halfWidth = _halfHeight = 11;
        _upPosY = Consts.ItemTopBorderY - 16;
        //_itemGO = ResourceManager.GetInstance().GetPrefab("Item", "ItemLife");
        //UIManager.GetInstance().AddGoToLayer(_itemGO, LayerId.Item);
        //_defaultSp = "Life";
        //_upSp = "Life_Up";
        //_sr = _itemGO.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        _defaultSpType = eItemSpriteType.Life;
        _upSpType = eItemSpriteType.LifeUp;
        base.Init();
    }

    protected override void DoEffect()
    {
        PlayerInterface.GetInstance().AddLifeCount(1);
    }

    public override void Clear()
    {
        //UIManager.GetInstance().RemoveGoFromLayer(_itemGO);
        base.Clear();
    }
}
