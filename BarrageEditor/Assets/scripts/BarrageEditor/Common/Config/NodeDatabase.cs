using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarrageEditor
{
    public class NodeConfig
    {
        public NodeType type;
        public List<object> defaultAttrValues;
        /// <summary>
        /// 快捷按钮的图标位置
        /// </summary>
        public string shortcutPath;
        /// <summary>
        /// 提示信息
        /// </summary>
        public string shortcutTip;
        /// <summary>
        /// 创建的时候是否编辑
        /// </summary>
        public bool editOnCreated;
        /// <summary>
        /// 是否可以被直接删除
        /// </summary>
        public bool isDeletable;

        public List<NodeType> allowParents;
        public List<NodeType> allowChilds;
        public List<NodeType> forbidParents;
        public List<NodeType> needAncestors;

        public NodeConfig()
        {
            editOnCreated = false;
            isDeletable = true;
            allowParents = null;
            allowChilds = null;
        }
    }

    public enum NodeType : int
    {
        Root = 0,
        ProjectSetting = 1,
        Folder = 1001,
        CodeBlock = 1002,
        Code = 1003,
        If = 1004,
        IfThen = 1005,
        IfElse = 1006,
        DefVar = 1007,
        Repeat = 1008,
        Comment = 1009,

        StageGroup = 1101,
        Stage = 1102,

        AddTask = 1201,
        TaskWait = 1202,

        DefineEnemy = 1301,
        OnEnemyCreate = 1302,
        CreateCustomizedEnemy = 1303,
        CreateSimpleEnemy = 1304,
        SetDropItems = 1305,

        DefineBoss = 1401,
        OnBossCreate = 1042,
        CreateBoss = 1403,
        SetBossInvincible = 1410,
        ShowBossBloodBar = 1411,
        DefineSpellCard = 1451,
        SpellCardInit = 1452,
        SpellCardFinish = 1453,
        StartSpellCard = 1454,
        SetBossPhaseData = 1455,

        DefineBullet = 1501,
        OnBulletCreate = 1502,
        CreateCustomizedBullet = 1503,
        CreateSimpleBullet = 1504,
        SetBulletStyle = 1505,
        SetBulletIgnoreCollisionGroup = 1506,
        SetBulletResistEliminatedTypes = 1507,
        ChangeBulletProperty = 1508,

        UnitSetV = 1601,
        UnitSetAcce = 1602,
        UnitMoveTo = 1603,
        UnitMoveTowards = 1604,
        UnitSetStraightParas = 1605,
        UnitSetPolarParas = 1606,
        UnitSetIgnoreCollisionGroups = 1621,
        UnitSetResistEliminatedTypes = 1622,
        KillUnit = 1691,
        DelUnit = 1692,

        DefineSTGObject = 1701,
        OnSTGObjectCreate = 1702,
        CreateCusomizedSTGObject = 1703,
        SetSpriteForSTGObject = 1704,
    }

    public class NodeDatabase
    {
        private Dictionary<NodeType, NodeConfig> _nodeCfgDic;

        public NodeDatabase()
        {
            _nodeCfgDic = new Dictionary<NodeType, NodeConfig>();
            NodeConfig cfg;
            #region root
            cfg = new NodeConfig
            {
                type = NodeType.Root,
                isDeletable = false,
                defaultAttrValues = new List<object>(),
            };
            _nodeCfgDic.Add(NodeType.Root, cfg);
            #endregion
            #region projectSetting
            cfg = new NodeConfig
            {
                type = NodeType.ProjectSetting,
                defaultAttrValues = new List<object> { "unnamed", "YK", "true" },
                allowParents = new List<NodeType> { NodeType.Root },
                allowChilds = new List<NodeType>(),
                isDeletable = false,
            };
            _nodeCfgDic.Add(NodeType.ProjectSetting, cfg);
            #endregion
            InitGeneralNodeCfgs();
            InitStageNodeCfgs();
            InitTaskNodeCfgs();
            InitEnemyNodeCfgs();
            InitBossNodeCfgs();
            InitBulletNodeCfgs();
            InitObjectNodeCfgs();
            InitUnitNodeCfgs();
        }

        private void InitGeneralNodeCfgs()
        {
            NodeConfig cfg;
            List<object> values;
            int i;
            #region general
            #region folder
            cfg = new NodeConfig
            {
                type = NodeType.Folder,
                shortcutPath = "folder",
                shortcutTip = "folder",
                allowParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                editOnCreated = true,
            };
            cfg.defaultAttrValues = new List<object> { "" };
            _nodeCfgDic.Add(NodeType.Folder, cfg);
            #endregion
            #region codeblock
            cfg = new NodeConfig
            {
                type = NodeType.CodeBlock,
                shortcutPath = "codeblock",
                shortcutTip = "code block",
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                editOnCreated = true,
            };
            cfg.defaultAttrValues = new List<object> { "" };
            _nodeCfgDic.Add(NodeType.CodeBlock, cfg);
            #endregion
            #region if
            cfg = new NodeConfig
            {
                type = NodeType.If,
                shortcutPath = "if",
                shortcutTip = "if",
                editOnCreated = true,
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            cfg.defaultAttrValues = new List<object> { "" };
            _nodeCfgDic.Add(NodeType.If, cfg);
            #endregion
            #region then
            cfg = new NodeConfig
            {
                type = NodeType.IfThen,
                isDeletable = false,
                allowParents = new List<NodeType>(),
            };
            cfg.defaultAttrValues = new List<object>();
            _nodeCfgDic.Add(NodeType.IfThen, cfg);
            #endregion
            #region else
            cfg = new NodeConfig
            {
                type = NodeType.IfElse,
                isDeletable = false,
                allowParents = new List<NodeType>(),
            };
            cfg.defaultAttrValues = new List<object>();
            _nodeCfgDic.Add(NodeType.IfElse, cfg);
            #endregion
            #region DefVar
            cfg = new NodeConfig
            {
                type = NodeType.DefVar,
                shortcutPath = "variable",
                shortcutTip = "define variable",
                editOnCreated = true,
                allowChilds = new List<NodeType>(),
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
            };
            cfg.defaultAttrValues = new List<object> { "", "" };
            _nodeCfgDic.Add(NodeType.DefVar, cfg);
            #endregion
            #region Repeat
            cfg = new NodeConfig
            {
                type = NodeType.Repeat,
                shortcutPath = "repeat",
                shortcutTip = "repeat",
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
            };
            values = new List<object> { "Infinite" };
            for (i=0;i<6;i++)
            {
                values.Add("");
            }
            cfg.defaultAttrValues = values;
            _nodeCfgDic.Add(NodeType.Repeat, cfg);
            #endregion
            #region Code
            cfg = new NodeConfig
            {
                type = NodeType.Code,
                shortcutPath = "code",
                shortcutTip = "code",
                editOnCreated = true,
                allowChilds = new List<NodeType>(),
            };
            cfg.defaultAttrValues = new List<object> { "" };
            _nodeCfgDic.Add(NodeType.Code, cfg);
            #endregion
            #region Comment
            cfg = new NodeConfig
            {
                type = NodeType.Comment,
                shortcutPath = "comment",
                shortcutTip = "comment",
                editOnCreated = true,
                allowChilds = new List<NodeType>(),
            };
            cfg.defaultAttrValues = new List<object> { "" };
            _nodeCfgDic.Add(NodeType.Comment, cfg);
            #endregion
            #endregion
        }

        private void InitStageNodeCfgs()
        {
            NodeConfig cfg;
            // StageGroup
            cfg = new NodeConfig
            {
                type = NodeType.StageGroup,
                shortcutPath = "stagegroup",
                shortcutTip = "stage group",
                defaultAttrValues = new List<object> { "" },
                allowChilds = new List<NodeType> { NodeType.Stage },
                allowParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.StageGroup, cfg);
            // Stage
            cfg = new NodeConfig
            {
                type = NodeType.Stage,
                shortcutPath = "stage",
                shortcutTip = "stage",
                defaultAttrValues = new List<object> { "" },
                allowParents = new List<NodeType> { NodeType.StageGroup },
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.Stage, cfg);
        }

        private void InitTaskNodeCfgs()
        {
            NodeConfig cfg;
            // AddTask
            cfg = new NodeConfig
            {
                type = NodeType.AddTask,
                shortcutPath = "task",
                shortcutTip = "unit add task",
                defaultAttrValues = new List<object> { "self" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                //needAncestors = new List<NodeType> { NodeType.Stage, NodeType.OnEnemyCreate, NodeType.OnBulletCreate, NodeType.Spel },
            };
            _nodeCfgDic.Add(NodeType.AddTask, cfg);
            // TaskWait
            cfg = new NodeConfig
            {
                type = NodeType.TaskWait,
                shortcutPath = "taskwait",
                shortcutTip = "wait",
                defaultAttrValues = new List<object> { "60" },
                needAncestors = new List<NodeType> { NodeType.Stage, NodeType.AddTask },
            };
            _nodeCfgDic.Add(NodeType.TaskWait, cfg);
        }

        private void InitEnemyNodeCfgs()
        {
            NodeConfig cfg;
            cfg = new NodeConfig
            {
                type = NodeType.DefineEnemy,
                shortcutPath = "enemydefine",
                shortcutTip = "define enemy",
                defaultAttrValues = new List<object> { "" },
                allowParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.DefineEnemy, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.OnEnemyCreate,
                shortcutPath = "enemyinit",
                defaultAttrValues = new List<object> { "10", "" },
                editOnCreated = false,
                isDeletable = false,
            };
            _nodeCfgDic.Add(NodeType.OnEnemyCreate, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.CreateCustomizedEnemy,
                shortcutPath = "enemycreate",
                shortcutTip = "create enemy",
                defaultAttrValues = new List<object> { "", "100000", "0", "0", "" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.CreateCustomizedEnemy, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.CreateSimpleEnemy,
                shortcutPath = "enemysimple",
                shortcutTip = "create simple enemy",
                defaultAttrValues = new List<object> { "100000", "10", "0", "0" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.CreateSimpleEnemy, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.SetDropItems,
                shortcutPath = "dropitem",
                shortcutTip = "set drop items",
                defaultAttrValues = new List<object> { "self", "32", "32", "PPointNormal", "3", "", "", "" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.SetDropItems, cfg);
        }

        private void InitBossNodeCfgs()
        {
            NodeConfig cfg;
            cfg = new NodeConfig
            {
                type = NodeType.DefineBoss,
                shortcutPath = "bossdefine",
                shortcutTip = "define boss",
                defaultAttrValues = new List<object> { "" },
                allowParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.DefineBoss, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.OnBossCreate,
                shortcutPath = "bossinit",
                defaultAttrValues = new List<object> { "2001", "0", "0", "32,32" },
                editOnCreated = false,
                isDeletable = false,
            };
            _nodeCfgDic.Add(NodeType.OnBossCreate, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.CreateBoss,
                shortcutPath = "bosscreate",
                shortcutTip = "create boss",
                defaultAttrValues = new List<object> { "" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.CreateBoss, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.SetBossInvincible,
                shortcutPath = "unknow",
                shortcutTip = "set invincible",
                defaultAttrValues = new List<object> { "boss", "5" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.SetBossInvincible, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.ShowBossBloodBar,
                shortcutPath = "bossshowbloodbar",
                shortcutTip = "show blood bar",
                defaultAttrValues = new List<object> { "boss", "true" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.ShowBossBloodBar, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.DefineSpellCard,
                shortcutPath = "scdefine",
                shortcutTip = "define spell card",
                defaultAttrValues = new List<object> { "", "unknown spell card", "1", "60", "ConditionEliminateAll", "true" },
                allowParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.DefineSpellCard, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.SpellCardInit,
                defaultAttrValues = new List<object>(),
                allowParents = new List<NodeType> { },
            };
            _nodeCfgDic.Add(NodeType.SpellCardInit, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.SpellCardFinish,
                defaultAttrValues = new List<object>(),
                allowParents = new List<NodeType> { },
            };
            _nodeCfgDic.Add(NodeType.SpellCardFinish, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.StartSpellCard,
                shortcutPath = "startsc",
                shortcutTip = "start spell card",
                defaultAttrValues = new List<object> { "", "boss", "true" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.StartSpellCard, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.SetBossPhaseData,
                shortcutPath = "bossphasedata",
                shortcutTip = "set phase data",
                defaultAttrValues = new List<object> { "boss", "3,1", "true" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.SetBossPhaseData, cfg);
        }

        private void InitBulletNodeCfgs()
        {
            NodeConfig cfg;
            #region define bullet
            cfg = new NodeConfig
            {
                type = NodeType.DefineBullet,
                shortcutPath = "bulletdefine",
                shortcutTip = "define bullet",
                defaultAttrValues = new List<object> { "" },
                allowParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.DefineBullet, cfg);
            #endregion
            #region onBulletCreate
            cfg = new NodeConfig
            {
                type = NodeType.OnBulletCreate,
                shortcutPath = "bulletdefine",
                shortcutTip = "define bullet",
                defaultAttrValues = new List<object> { "v,angle" },
                editOnCreated = false,
                isDeletable = false,
            };
            _nodeCfgDic.Add(NodeType.OnBulletCreate, cfg);
            #endregion
            #region createBullet
            cfg = new NodeConfig
            {
                type = NodeType.CreateCustomizedBullet,
                shortcutPath = "bulletcreate",
                shortcutTip = "create bullet",
                defaultAttrValues = new List<object> { "", "107010", "0", "0", "" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.CreateCustomizedBullet, cfg);
            #endregion
            cfg = new NodeConfig
            {
                type = NodeType.CreateSimpleBullet,
                shortcutPath = "bulletcreatestraight",
                shortcutTip = "create simple bullet",
                defaultAttrValues = new List<object> { "107010", "0", "0", "3", "0", "false", "0", "UseVelocityAngle", "", "" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.CreateSimpleBullet, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.SetBulletStyle,
                shortcutPath = "bulletchangestyle",
                shortcutTip = "set bullet style",
                defaultAttrValues = new List<object> { "self", "107010" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.SetBulletStyle, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.SetBulletIgnoreCollisionGroup,
                shortcutPath = "ignorecollisiongroup",
                shortcutTip = "set ignore collision groups",
                defaultAttrValues = new List<object> { "self", "0" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.SetBulletIgnoreCollisionGroup, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.ChangeBulletProperty,
                shortcutPath = "bulletchangeprops",
                shortcutTip = "change property",
                defaultAttrValues = new List<object> { "self", "Prop_Velocity", "ChangeModeChangeTo", "0", "0", "0", "0", "0", "30", "IntModeLinear", "1", "0" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.ChangeBulletProperty, cfg);
        }

        private void InitObjectNodeCfgs()
        {
            NodeConfig cfg;

            cfg = new NodeConfig
            {
                type = NodeType.DefineSTGObject,
                shortcutPath = "objectdefine",
                shortcutTip = "define object",
                defaultAttrValues = new List<object> { "" },
                allowParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.DefineSTGObject, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.OnSTGObjectCreate,
                shortcutPath = "objectinit",
                shortcutTip = "init object",
                defaultAttrValues = new List<object> { "", "STGEffectAtlas", "MapleLeaf1", "BlendMode_Normal", "LayerEffectNormal", "false" },
                isDeletable = false,
            };
            _nodeCfgDic.Add(NodeType.OnSTGObjectCreate, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.CreateCusomizedSTGObject,
                shortcutPath = "objectcreate",
                shortcutTip = "create object",
                defaultAttrValues = new List<object> { "", "" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.CreateCusomizedSTGObject, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.SetSpriteForSTGObject,
                shortcutPath = "objectsetimg",
                shortcutTip = "set sprite",
                defaultAttrValues = new List<object> { "self", "STGEffectAtlas", "MapleLeaf1", "BlendMode_Normal", "LayerEffectNormal", "false" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.SetSpriteForSTGObject, cfg);
        }

        private void InitUnitNodeCfgs()
        {
            NodeConfig cfg;
            // SetV
            cfg = new NodeConfig
            {
                type = NodeType.UnitSetV,
                shortcutPath = "setv",
                shortcutTip = "set velocity",
                defaultAttrValues = new List<object> { "self", "3", "0", "false" },
                allowChilds = new List<NodeType>(),
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
            };
            _nodeCfgDic.Add(NodeType.UnitSetV, cfg);
            // SetAcce
            cfg = new NodeConfig
            {
                type = NodeType.UnitSetAcce,
                shortcutPath = "setaccel",
                shortcutTip = "set acceleration",
                defaultAttrValues = new List<object> { "self", "0.05", "0", "false" },
                allowChilds = new List<NodeType>(),
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
            };
            _nodeCfgDic.Add(NodeType.UnitSetAcce, cfg);
            // MoveTo
            cfg = new NodeConfig
            {
                type = NodeType.UnitMoveTo,
                shortcutPath = "moveto",
                shortcutTip = "move to",
                defaultAttrValues = new List<object> { "self", "0", "0", "60", "IntModeLinear" },
                allowChilds = new List<NodeType>(),
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
            };
            _nodeCfgDic.Add(NodeType.UnitMoveTo, cfg);
            // MoveTowards
            cfg = new NodeConfig
            {
                type = NodeType.UnitMoveTowards,
                shortcutPath = "moveto",
                shortcutTip = "move towards",
                defaultAttrValues = new List<object> { "self", "3", "0", "false", "60" },
                allowChilds = new List<NodeType>(),
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
            };
            _nodeCfgDic.Add(NodeType.UnitMoveTowards, cfg);
            // SetStraightParas
            cfg = new NodeConfig
            {
                type = NodeType.UnitSetStraightParas,
                shortcutPath = "setstraightparas",
                shortcutTip = "set straight paras",
                defaultAttrValues = new List<object> { "self", "3", "0", "false", "UseVelocityAngle", "0" },
                allowChilds = new List<NodeType>(),
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
            };
            _nodeCfgDic.Add(NodeType.UnitSetStraightParas, cfg);
            // SetPolarParas
            cfg = new NodeConfig
            {
                type = NodeType.UnitSetPolarParas,
                shortcutPath = "setpolarparas",
                shortcutTip = "set polar paras",
                defaultAttrValues = new List<object> { "self", "3", "0", "1", "60" },
                allowChilds = new List<NodeType>(),
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
            };
            _nodeCfgDic.Add(NodeType.UnitSetPolarParas, cfg);
            // IgnoreCollisionGroups
            cfg = new NodeConfig
            {
                type = NodeType.UnitSetIgnoreCollisionGroups,
                shortcutPath = "ignorecollisiongroup",
                shortcutTip = "set ignore collision groups",
                defaultAttrValues = new List<object> { "self", "0" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.UnitSetIgnoreCollisionGroups, cfg);
            // ResistEliminatedTypes
            cfg = new NodeConfig
            {
                type = NodeType.UnitSetResistEliminatedTypes,
                shortcutPath = "ignorecollisiongroup",
                shortcutTip = "set ignore collision groups",
                defaultAttrValues = new List<object> { "self", "0" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.UnitSetResistEliminatedTypes, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.KillUnit,
                shortcutPath = "unitkill",
                shortcutTip = "kill unit",
                defaultAttrValues = new List<object> { "self", "true" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.KillUnit, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.DelUnit,
                shortcutPath = "unitdel",
                shortcutTip = "delete unit",
                defaultAttrValues = new List<object> { "self" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.DelUnit, cfg);
        }

        /// <summary>
        /// 获取单个节点的配置信息
        /// </summary>
        /// <param name="styleId"></param>
        /// <returns></returns>
        public NodeConfig GetNodeCfgByNodeType(NodeType type)
        {
            NodeConfig cfg;
            if ( _nodeCfgDic.TryGetValue(type,out cfg) )
            {
                return cfg;
            }
            return null;
        }
    }
}
