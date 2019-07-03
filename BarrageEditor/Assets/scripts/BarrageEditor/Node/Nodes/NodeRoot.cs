using UnityEngine;
using UnityEngine.UI;

namespace BarrageEditor
{
    public class NodeRoot : BaseNode
    {
        public override void Init( RectTransform parentTf)
        {
            _extraDepth = 0;
            _nodeType = NodeType.Root;
            base.Init(parentTf);
            _expandImg.gameObject.SetActive(false);
            // 移动功能标识图像
            RectTransform rect = _functionImg.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, 0);
            // 移动描述文本的位置
            rect = _selectedImg.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(31, 0);
        }

        public override string GetNodeName()
        {
            return "project";
        }

        public override string ToDesc()
        {
            return "project";
        }

        public override string ToLuaHead()
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}",
                "local lib = require \"LuaLib\"\n",
                "local consts = Constants\n",
                "local Stage = {}\n",
                "local CustomizedTable = {}\n",
                "local CustomizedEnemyTable = {}\n",
                "local BossTable = {}\n",
                "local CustomizedSTGObjectTable = {}\n\n");
        }

        public override string ToLuaFoot()
        {
            return string.Format("{0}{1}{2}{3}{4}{5}{6}",
                "return\n{\n",
                "   CustomizedBulletTable = CustomizedTable,\n",
                "   CustomizedEnemyTable = CustomizedEnemyTable,\n",
                "   BossTable = BossTable,\n",
                "   CustomizedSTGObjectTable = CustomizedSTGObjectTable,\n",
                "   Stage = Stage,\n",
                "}");
        }
    }
}
