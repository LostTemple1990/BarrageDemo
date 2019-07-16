using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeSpellCardInit : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.SpellCardInit;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "onscstart");
        }

        public override void CreateDefualtChilds()
        {
            // 设置boss血量
            BaseNode tmpNode = NodeManager.CreateNode(NodeType.Code);
            tmpNode.SetAttrsValues(new List<object> { "boss:SetMaxHp(500)" });
            InsertChildNode(tmpNode, -1);
            // 设置符卡无敌时间
            tmpNode = NodeManager.CreateNode(NodeType.SetBossInvincible);
            tmpNode.SetAttrsValues(new List<object> { "boss", "5" });
            InsertChildNode(tmpNode, -1);
            // 显示BOSS血条
            tmpNode = NodeManager.CreateNode(NodeType.ShowBossBloodBar);
            tmpNode.SetAttrsValues(new List<object> { "boss", "true" });
            InsertChildNode(tmpNode, -1);
        }

        public override string GetNodeName()
        {
            return "spell card init";
        }

        public override string ToDesc()
        {
            return string.Format("sc init");
        }

        public override string ToLuaHead()
        {
            string scTypeName = parentNode.GetAttrByIndex(0).GetValueString();
            int bossCount = int.Parse(parentNode.GetAttrByIndex(2).GetValueString());
            string paramStr = "";
            if (bossCount == 1)
            {
                paramStr = "boss";
            }
            else
            {
                for (int i = 0; i < bossCount; i++)
                {
                    if (i == 0)
                    {
                        paramStr += "boss" + i;
                    }
                    else
                    {
                        paramStr += ",boss" + i;
                    }
                }
            }
            string ret = string.Format("SpellCard[\"{0}\"].Init = function({1})\n", scTypeName, paramStr);
            ret += string.Format("    SetSpellCardProperties(\"{0}\",{1},{2},{3},nil)\n",
                parentNode.GetAttrByIndex(1).GetValueString(),//符卡名称
                parentNode.GetAttrByIndex(2).GetValueString(),//持续时间
                parentNode.GetAttrByIndex(3).GetValueString(),//击破条件
                parentNode.GetAttrByIndex(4).GetValueString());//是否符卡
            return ret;
        }

        public override string ToLuaFoot()
        {
            return string.Format("end\n");
        }
    }
}
