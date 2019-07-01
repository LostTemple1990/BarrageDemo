using UnityEngine;
using UnityEngine.UI;
using YKEngine;

namespace BarrageEditor
{
    public class NodeSpellCardFinish : BaseNode
    {
        public override void Init(RectTransform parentTf)
        {
            _nodeType = NodeType.SpellCardFinish;
            base.Init(parentTf);
            _functionImg.sprite = ResourceManager.GetInstance().GetSprite("NodeIcon", "onscfinish");
        }

        public override string GetNodeName()
        {
            return "spell card finish";
        }

        public override string ToDesc()
        {
            return string.Format("onFinish");
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
            string ret = string.Format("SpellCard[\"{0}\"].OnFinish = function({1})\n", scTypeName, paramStr);
            return ret;
        }

        public override string ToLuaFoot()
        {
            return string.Format("end\n");
        }
    }
}
