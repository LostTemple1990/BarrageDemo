using UnityEngine;
using System.Collections;

public class ItemPowerFull : ItemBase
{
    public override void Init()
    {
        _itemType = ItemType.PowerFull;
        _collisionHalfWidth = _collisionHalfHeight = 11;
        _halfWidth = _halfHeight = 11;
        _upPosY = Consts.ItemTopBorderY - 16;
        //_itemGO = ResourceManager.GetInstance().GetPrefab("Item", "ItemPowerFull");
        //UIManager.GetInstance().AddGoToLayer(_itemGO, LayerId.Item);
        //_defaultSp = "PowerFull";
        //_upSp = "PowerFull_Up";
        //_sr = _itemGO.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        _defaultSpType = eItemSpriteType.PowerFull;
        _upSpType = eItemSpriteType.PowerFullUp;
        base.Init();
    }

    protected override void DoEffect()
    {
        PlayerInterface.GetInstance().AddPower(400);
    }

    public override void Clear()
    {
        //UIManager.GetInstance().RemoveGoFromLayer(_itemGO);
        base.Clear();
    }
}
