using UnityEngine;
using System.Collections;

public class ItemBombFragment : ItemBase
{
    public override void Init()
    {
        _itemType = ItemType.BombFragment;
        _itemGO = ResourceManager.GetInstance().GetPrefab("Item", "ItemBombFragment");
        UIManager.GetInstance().AddGoToLayer(_itemGO, LayerId.Item);
        _collisionHalfWidth = _collisionHalfHeight = 11;
        _halfWidth = _halfHeight = 11;
        _aboveY = Consts.ItemTopBorderY + _halfHeight;
        _defaultSp = "BombFragment";
        _upSp = "BombFragment_Up";
        _sr = _itemGO.transform.Find("Sprite").GetComponent<SpriteRenderer>();
        base.Init();
    }

    protected override void DoEffect()
    {
        PlayerInterface.GetInstance().AddSpellCardFragmentCount(1);
    }

    public override void Clear()
    {
        UIManager.GetInstance().RemoveGoFromLayer(_itemGO);
        base.Clear();
    }
}
