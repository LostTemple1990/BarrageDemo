using UnityEngine;
using System.Collections;

public class ItemPPointNormal : ItemBase
{
    public override void Init()
    {
        _itemType = ItemType.PPointNormal;
        _itemGO = ResourceManager.GetInstance().GetPrefab("item", "PPointNormal");
        UIManager.GetInstance().AddGoToLayer(_itemGO, LayerId.Item);
        base.Init();
    }

    protected override void DoEffect()
    {
        PlayerService.GetInstance().AddPower(0.1f);
    }

    public override void Clear()
    {
        base.Clear();
        UIManager.GetInstance().RemoveGoFromLayer(_itemGO);
    }
}
