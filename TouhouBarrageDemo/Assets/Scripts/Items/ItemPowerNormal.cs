using UnityEngine;
using System.Collections;

public class ItemPowerNormal : ItemBase
{
    public override void Init()
    {
        _itemType = ItemType.PowerNormal;
        _collisionHalfWidth = _collisionHalfHeight = 6;
        _halfWidth = _halfHeight = 6;
        _upPosY = Consts.ItemTopBorderY - 8;
        //_itemGO = ResourceManager.GetInstance().GetPrefab("Item", "ItemPowerNormal");
        //UIManager.GetInstance().AddGoToLayer(_itemGO, LayerId.Item);
        //_defaultSp = "PowerNormal";
        //_upSp = "PowerNormal_Up";
        //_sr = _itemGO.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        _defaultSpType = eItemSpriteType.PowerNormal;
        _upSpType = eItemSpriteType.PowerNormalUp;
        base.Init();
    }

    protected override void DoEffect()
    {
        PlayerInterface.GetInstance().AddPower(2);
    }

    public override void Clear()
    {
        //UIManager.GetInstance().RemoveGoFromLayer(_itemGO);
        base.Clear();
    }
}
