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
            _isVisible = true;
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
            string ret = string.Format("{0}{1}{2}{3}{4}{5}{6}",
                "local lib = require \"LuaLib\"\n",
                "local consts = Constants\n",
                "local Stage = {}\n",
                "local CustomizedTable = {}\n",
                "local CustomizedEnemyTable = {}\n",
                "local BossTable = {}\n",
                "local CustomizedSTGObjectTable = {}\n");
            if (BarrageProject.IsDebugStage)
            {
                string debugStageName = BarrageProject.DebugStageNode.GetAttrByIndex(0).GetValueString();
                ret = ret + string.Format("SetDebugStageName(\"{0}\")\n", debugStageName);
            }
            ret = ret + "\n";
            return ret;
        }

        public override string ToLuaFoot()
        {
            string ret = "";
            if (BarrageProject.IsDebugSpellCard)
            {
                string scName = BarrageProject.DebugSpellCardNode.GetAttrByIndex(0).GetValueString();
                ret += string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                    "\nSetDebugStageName(\"__TestSCStage\")\n",
                    string.Format("BossTable[\"{0}\"] = {{}}\n","__TestSCBoss"),
                    string.Format("BossTable[\"{0}\"].Init = function(self)\n    self:SetAni(2001)\n    self:SetPos(0,280)\n    self:SetCollisionSize(32,32)\nend\n", "__TestSCBoss"),
                    "Stage[\"__TestSCStage\"] = function()\n    local boss = CreateBoss(\"__TestSCBoss\",0,280)\n",
                    "    boss:MoveTo(0,170,90,IntModeEaseInQuad)\n    if Wait(100)==false then return end\n",
                    "    boss:SetPhaseData(1,true)\n",
                    string.Format("    StartSpellCard(SpellCard[\"{0}\"],boss)\n",scName),
                    "    if WaitForSpellCardFinish() == false then return end\n    FinishStage()\nend\n\n"
                    );
            }
            ret += string.Format("{0}{1}{2}{3}{4}{5}{6}",
                "return\n{\n",
                "   CustomizedBulletTable = CustomizedTable,\n",
                "   CustomizedEnemyTable = CustomizedEnemyTable,\n",
                "   BossTable = BossTable,\n",
                "   CustomizedSTGObjectTable = CustomizedSTGObjectTable,\n",
                "   Stage = Stage,\n",
                "}");
            return ret;
        }
    }
}
