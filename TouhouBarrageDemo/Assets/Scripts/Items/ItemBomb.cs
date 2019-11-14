using UnityEngine;
using System.Collections;

public class ItemBomb : ItemBase
{
    public override void Init()
    {
        _itemType = ItemType.Bomb;
        _itemGO = ResourceManager.GetInstance().GetPrefab("Item", "ItemBomb");
        UIManager.GetInstance().AddGoToLayer(_itemGO, LayerId.Item);
        _collisionHalfWidth = _collisionHalfHeight = 11;
        _halfWidth = _halfHeight = 11;
        _upPosY = Consts.ItemTopBorderY - 16;
        _defaultSp = "Bomb";
        _upSp = "Bomb_Up";
        _sr = _itemGO.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        base.Init();
    }

    protected override void DoEffect()
    {
        PlayerInterface.GetInstance().AddSpellCardCount(1);
    }

    public override void Clear()
    {
        UIManager.GetInstance().RemoveGoFromLayer(_itemGO);
        base.Clear();
    }
}
