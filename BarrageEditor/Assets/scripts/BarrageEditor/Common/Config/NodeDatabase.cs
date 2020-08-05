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
        StartDialog = 1121,
        DialogWait = 1122,
        CreateDialogCG = 1123,
        HighlightDialogCG = 1124,
        FadeoutDialogCG = 1125,
        CreateSentence = 1131,
        DelSentence = 1132,

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
        SetInvincible = 1410,
        ShowBossBloodBar = 1411,
        ShowBossPosHint = 1412,
        ShowBossAura = 1413,
        ShowBossSpellCardHpAura = 1414,
        BossSetWanderProps = 1421,
        BossWander = 1422,
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

        DefineLaser = 1901,
        OnLaserCreate = 1902,
        CreateCustomizedLaser = 1903,
        CreateLaser = 1904,
        SetLaserStyle = 1905,
        LaserTurnHalfOn = 1911,
        LaserTurnOn = 1912,
        LaserTurnOff = 1913,
        LaserChangeLengthTo = 1915,

        DefineLinearLaser = 2001,
        OnLinearLaserCreate = 2002,
        CreateCustomizedLinearLaser = 2003,
        DefineCurveLaser = 2101,
        OnCurveLaserCreate = 2102,
        CreateCustomizedCurveLaser = 2103,

        UnitSetV = 1601,
        UnitSetAcce = 1602,
        UnitMoveTo = 1603,
        UnitMoveTowards = 1604,
        UnitSetStraightParas = 1605,
        UnitSetPolarParas = 1606,
        UnitSetIgnoreCollisionGroups = 1621,
        UnitSetResistEliminatedTypes = 1622,
        UnitAttachTo = 1626,
        UnitSetRelativePos = 1627,
        UnitEventTrigger = 1681,
        KillUnit = 1691,
        DelUnit = 1692,

        DefineSTGObject = 1701,
        OnSTGObjectCreate = 1702,
        CreateCusomizedSTGObject = 1703,
        SetSpriteForSTGObject = 1704,
        STGObjectSetColor = 1705,
        STGObjectChangeAlphaTo = 1706,
        DefineSpecialSTGObject = 1711,
        OnSpecialSTGObjectCreate = 1712,

        DefineCollider = 1801,
        OnColliderCreate = 1802,
        CreateCustomizedCollider = 1803,
        CreateSimpleCollider = 1804,
        Rebound = 1812,
        ColliderTrigger = 1813,
        DropItems = 1821,
        PlayAni = 1822,

        PlaySound = 2201,
        PauseSound = 2202,
        ResumeSound = 2203,
        StopSound = 2204,
        LoadSound = 2205,

        CreateChargeEffect = 1831,
        CreateBurstEffect = 1832,
        ShakeScreen = 1836,
        StopShakeScreen = 1837,
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
            InitLaserNodeCfgs();
            InitLaserExNodeCfgs();
            InitToolsNodeCfgs();
            InitObjectNodeCfgs();
            InitUnitNodeCfgs();
            InitAudioNodeCfgs();
            InitEffectNodeCfgs();
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
            for (i=0;i<9;i++)
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
                defaultAttrValues = new List<object> { "", "", "", "false" },
                allowParents = new List<NodeType> { NodeType.StageGroup },
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.Stage, cfg);
            // StartDialog
            cfg = new NodeConfig
            {
                type = NodeType.StartDialog,
                shortcutPath = "createdialog",
                shortcutTip = "StartDialog",
                defaultAttrValues = new List<object> { "" },
                needAncestors = new List<NodeType> { NodeType.Stage },
            };
            _nodeCfgDic.Add(cfg.type, cfg);
            // DialogWait
            cfg = new NodeConfig
            {
                type = NodeType.DialogWait,
                shortcutPath = "dialogwait",
                shortcutTip = "DialogWait",
                defaultAttrValues = new List<object> { "120" },
                needAncestors = new List<NodeType> { NodeType.StartDialog },
                allowChilds = new List<NodeType> { },
            };
            _nodeCfgDic.Add(cfg.type, cfg);
            // CreateDialogCG
            cfg = new NodeConfig
            {
                type = NodeType.CreateDialogCG,
                shortcutPath = "createdialogcg",
                shortcutTip = "CreateDialogCG",
                defaultAttrValues = new List<object> { "Reimu", "Reimu", "100", "150" },
                needAncestors = new List<NodeType> { NodeType.StartDialog },
                allowChilds = new List<NodeType> { },
            };
            _nodeCfgDic.Add(cfg.type, cfg);
            // HighlightDialogCG
            cfg = new NodeConfig
            {
                type = NodeType.HighlightDialogCG,
                shortcutPath = "highlightdialogcg",
                shortcutTip = "HighlightDialogCG",
                defaultAttrValues = new List<object> { "Reimu", "true" },
                needAncestors = new List<NodeType> { NodeType.StartDialog },
                allowChilds = new List<NodeType> { },
            };
            _nodeCfgDic.Add(cfg.type, cfg);
            // FadeOutDialogCG
            cfg = new NodeConfig
            {
                type = NodeType.FadeoutDialogCG,
                shortcutPath = "fadeoutdialogcg",
                shortcutTip = "FadeOutDialogCG",
                defaultAttrValues = new List<object> { "Reimu" },
                needAncestors = new List<NodeType> { NodeType.StartDialog },
                allowChilds = new List<NodeType> { },
            };
            _nodeCfgDic.Add(cfg.type, cfg);
            // CreateDialogBox
            cfg = new NodeConfig
            {
                type = NodeType.CreateSentence,
                shortcutPath = "sentence",
                shortcutTip = "CreateDialogBox",
                defaultAttrValues = new List<object> { "0", "Test DialogBox...", "100", "150", "120", "1" },
                needAncestors = new List<NodeType> { NodeType.StartDialog },
                allowChilds = new List<NodeType> { },
            };
            _nodeCfgDic.Add(cfg.type, cfg);
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
                needAncestors = new List<NodeType> { NodeType.Stage, NodeType.DefineEnemy, NodeType.DefineBoss, NodeType.DefineSpellCard,
                    NodeType.DefineBullet, NodeType.DefineLaser, NodeType.DefineLinearLaser, NodeType.DefineCurveLaser,
                    NodeType.DefineCollider, NodeType.DefineSTGObject, NodeType.DefineSpecialSTGObject,
                    NodeType.AddTask, },
            };
            _nodeCfgDic.Add(NodeType.AddTask, cfg);
            // TaskWait
            cfg = new NodeConfig
            {
                type = NodeType.TaskWait,
                shortcutPath = "taskwait",
                shortcutTip = "wait",
                defaultAttrValues = new List<object> { "60" },
                needAncestors = new List<NodeType> { NodeType.Stage, NodeType.AddTask, NodeType.SpellCardInit },
                allowChilds = new List<NodeType>(),
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
                allowChilds = new List<NodeType> { NodeType.UnitEventTrigger },
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.DefineEnemy, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.OnEnemyCreate,
                shortcutPath = "enemyinit",
                defaultAttrValues = new List<object> { "", "100000", "10" },
                editOnCreated = false,
                isDeletable = false,
            };
            _nodeCfgDic.Add(NodeType.OnEnemyCreate, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.CreateCustomizedEnemy,
                shortcutPath = "enemycreate",
                shortcutTip = "create enemy",
                defaultAttrValues = new List<object> { "", "0", "0", "" },
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
                defaultAttrValues = new List<object> { "self", "32", "32", "1,1" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.SetDropItems, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.SetInvincible,
                shortcutPath = "unknow",
                shortcutTip = "set invincible",
                defaultAttrValues = new List<object> { "self", "5" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.SetInvincible, cfg);
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
                defaultAttrValues = new List<object> { "2001", "32,32" },
                editOnCreated = false,
                isDeletable = false,
            };
            _nodeCfgDic.Add(NodeType.OnBossCreate, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.CreateBoss,
                shortcutPath = "bosscreate",
                shortcutTip = "create boss",
                defaultAttrValues = new List<object> { "", "0", "240", "boss" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.CreateBoss, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.ShowBossBloodBar,
                shortcutPath = "setwanderprops",
                shortcutTip = "show blood bar",
                defaultAttrValues = new List<object> { "boss", "true" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.ShowBossBloodBar, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.BossSetWanderProps,
                shortcutPath = "setwanderprops",
                shortcutTip = "set wander properties",
                defaultAttrValues = new List<object> { "boss", "-96,96", "112,144", "16,32", "8,16", "IntModeLinear", "MoveXTowardsPlayer" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.BossSetWanderProps, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.BossWander,
                shortcutPath = "bosswander",
                shortcutTip = "boss wander",
                defaultAttrValues = new List<object> { "boss", "60" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.BossWander, cfg);

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

            cfg = new NodeConfig
            {
                type = NodeType.ShowBossPosHint,
                shortcutPath = "bossshowposhint",
                shortcutTip = "show positon hint of boss",
                defaultAttrValues = new List<object> { "boss", "true" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(cfg.type, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.ShowBossAura,
                shortcutPath = "bossshowaura",
                shortcutTip = "show aura of boss",
                defaultAttrValues = new List<object> { "boss", "true", "true" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(cfg.type, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.ShowBossSpellCardHpAura,
                shortcutPath = "spellcardhp",
                shortcutTip = "show hp aura of boss",
                defaultAttrValues = new List<object> { "boss", "true" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(cfg.type, cfg);
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
                defaultAttrValues = new List<object> { "v,angle", "107020" },
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
                defaultAttrValues = new List<object> { "", "self.x", "self.y", "" },
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

        private void InitLaserNodeCfgs()
        {
            NodeConfig cfg;

            cfg = new NodeConfig
            {
                type = NodeType.DefineLaser,
                shortcutPath = "laserdefine",
                shortcutTip = "define laser",
                defaultAttrValues = new List<object> { "" },
                allowParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.DefineLaser, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.OnLaserCreate,
                shortcutPath = "laserinit",
                defaultAttrValues = new List<object> { "rot", "202011", "100", "16", "32" },
                editOnCreated = false,
                isDeletable = false,
            };
            _nodeCfgDic.Add(NodeType.OnLaserCreate, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.CreateCustomizedLaser,
                shortcutPath = "lasercreate",
                shortcutTip = "create laser",
                defaultAttrValues = new List<object> { "", "self.x", "self.y", "" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.CreateCustomizedLaser, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.LaserTurnHalfOn,
                shortcutPath = "laserturnhalfon",
                shortcutTip = "laser turn half on",
                defaultAttrValues = new List<object> { "self", "2", "0" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.LaserTurnHalfOn, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.LaserTurnOn,
                shortcutPath = "laserturnon",
                shortcutTip = "laser turn on",
                defaultAttrValues = new List<object> { "self", "10" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.LaserTurnOn, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.LaserTurnOff,
                shortcutPath = "laserturnoff",
                shortcutTip = "laser turn off",
                defaultAttrValues = new List<object> { "self", "10" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.LaserTurnOff, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.LaserChangeLengthTo,
                shortcutPath = "lasergrow",
                shortcutTip = "laser change length",
                defaultAttrValues = new List<object> { "self", "100", "0", "10" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.LaserChangeLengthTo, cfg);
        }

        private void InitLaserExNodeCfgs()
        {
            NodeConfig cfg;

            cfg = new NodeConfig
            {
                type = NodeType.DefineLinearLaser,
                shortcutPath = "laserdefine",
                shortcutTip = "define linear laser",
                defaultAttrValues = new List<object> { "" },
                allowParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.DefineLinearLaser, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.OnLinearLaserCreate,
                shortcutPath = "laserinit",
                defaultAttrValues = new List<object> { "", "202060", "45", "true", "true" },
                editOnCreated = false,
                isDeletable = false,
            };
            _nodeCfgDic.Add(NodeType.OnLinearLaserCreate, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.CreateCustomizedLinearLaser,
                shortcutPath = "lasercreate",
                shortcutTip = "create linear laser",
                defaultAttrValues = new List<object> { "", "self.x", "self.y", "" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.CreateCustomizedLinearLaser, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.DefineCurveLaser,
                shortcutPath = "laserbentdefine",
                shortcutTip = "define curve laser",
                defaultAttrValues = new List<object> { "" },
                allowParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.DefineCurveLaser, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.OnCurveLaserCreate,
                shortcutPath = "laserbentinit",
                defaultAttrValues = new List<object> { "", "401060", "45", "16" },
                editOnCreated = false,
                isDeletable = false,
            };
            _nodeCfgDic.Add(NodeType.OnCurveLaserCreate, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.CreateCustomizedCurveLaser,
                shortcutPath = "laserbentcreate",
                shortcutTip = "create curve laser",
                defaultAttrValues = new List<object> { "", "self.x", "self.y", "" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.CreateCustomizedCurveLaser, cfg);
        }

        private void InitToolsNodeCfgs()
        {
            NodeConfig cfg;
            cfg = new NodeConfig
            {
                type = NodeType.DefineCollider,
                shortcutPath = "colliderdefine",
                shortcutTip = "define collider",
                defaultAttrValues = new List<object> { "" },
                allowParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.DefineCollider, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.OnColliderCreate,
                shortcutPath = "colliderinit",
                shortcutTip = "",
                defaultAttrValues = new List<object> { "", "0", "0", "0" },
                isDeletable = false,
            };
            _nodeCfgDic.Add(NodeType.OnColliderCreate, cfg);
            
            cfg = new NodeConfig
            {
                type = NodeType.CreateCustomizedCollider,
                shortcutPath = "collidercreate",
                shortcutTip = "create customized collider",
                defaultAttrValues = new List<object> { "", "TypeCircle", "", "" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.CreateCustomizedCollider, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.CreateSimpleCollider,
                shortcutPath = "simplecollider",
                shortcutTip = "create simple collider",
                defaultAttrValues = new List<object> { "TypeCircle", "0", "0", "64", "64", "0", "" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(cfg.type, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.Rebound,
                shortcutPath = "rebound",
                shortcutTip = "add rebound",
                defaultAttrValues = new List<object> { "self", "15", "1" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(cfg.type, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.ColliderTrigger,
                shortcutPath = "collidertrigger",
                shortcutTip = "add collider trigger",
                defaultAttrValues = new List<object> { "self", "2048" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
            };
            _nodeCfgDic.Add(cfg.type, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.DropItems,
                shortcutPath = "dropitems",
                shortcutTip = "drop items",
                defaultAttrValues = new List<object> { "0", "150", "32", "32", "1,3" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(cfg.type, cfg);
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
                type = NodeType.DefineSpecialSTGObject,
                shortcutPath = "objectdefine",
                shortcutTip = "define special object",
                defaultAttrValues = new List<object> { "" },
                allowParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
                editOnCreated = true,
            };
            _nodeCfgDic.Add(NodeType.DefineSpecialSTGObject, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.OnSpecialSTGObjectCreate,
                shortcutPath = "objectinit",
                shortcutTip = "init object",
                defaultAttrValues = new List<object> { "", "VisionMaskEffect", "LayerEffectNormal", "false" },
                isDeletable = false,
            };
            _nodeCfgDic.Add(NodeType.OnSpecialSTGObjectCreate, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.CreateCusomizedSTGObject,
                shortcutPath = "objectcreate",
                shortcutTip = "create object",
                defaultAttrValues = new List<object> { "", "0", "0", "" },
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

            cfg = new NodeConfig
            {
                type = NodeType.STGObjectSetColor,
                shortcutPath = "setcolor",
                shortcutTip = "change color and blendMode of object",
                defaultAttrValues = new List<object> { "self", "1", "1", "1", "1", "" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.STGObjectSetColor, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.STGObjectChangeAlphaTo,
                shortcutPath = "changealphato",
                shortcutTip = "change alpha of object during frame(s)",
                defaultAttrValues = new List<object> { "self", "0", "60" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.STGObjectChangeAlphaTo, cfg);
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
                defaultAttrValues = new List<object> { "self", "3", "0", "false", "0", "UseVelocityAngle" },
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
                defaultAttrValues = new List<object> { "self", "3", "0", "1", "1", "" },
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
                type = NodeType.UnitAttachTo,
                shortcutPath = "connect",
                shortcutTip = "attach to master",
                defaultAttrValues = new List<object> { "last", "self", "true" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.UnitAttachTo, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.UnitSetRelativePos,
                shortcutPath = "setrelpos",
                shortcutTip = "set relative position",
                defaultAttrValues = new List<object> { "self", "10", "0", "0", "true", "true" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(NodeType.UnitSetRelativePos, cfg);

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

            cfg = new NodeConfig
            {
                type = NodeType.UnitEventTrigger,
                shortcutPath = "callbackfunc",
                shortcutTip = "unit event trigger",
                defaultAttrValues = new List<object> { "OnKill" },
                allowParents = new List<NodeType> { NodeType.DefineEnemy, NodeType.DefineBoss, NodeType.DefineCollider, NodeType.DefineSTGObject,
                    NodeType.DefineBullet, NodeType.DefineLaser, NodeType.DefineLinearLaser, NodeType.DefineCurveLaser },
            };
            _nodeCfgDic.Add(NodeType.UnitEventTrigger, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.PlayAni,
                shortcutPath = "playani",
                shortcutTip = "PlayAni",
                defaultAttrValues = new List<object> { "self", "ActionIdle", "DirNull", "" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(cfg.type, cfg);
        }

        private void InitAudioNodeCfgs()
        {
            NodeConfig cfg;

            cfg = new NodeConfig
            {
                type = NodeType.PlaySound,
                shortcutPath = "playbgm",
                shortcutTip = "play sound",
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            cfg.defaultAttrValues = new List<object> { "se_tan00", "0.05", "false" };
            _nodeCfgDic.Add(NodeType.PlaySound, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.StopSound,
                shortcutPath = "stopbgm",
                shortcutTip = "stop sound",
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            cfg.defaultAttrValues = new List<object> { "bgm" };
            _nodeCfgDic.Add(NodeType.StopSound, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.PauseSound,
                shortcutPath = "pausebgm",
                shortcutTip = "pause sound",
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            cfg.defaultAttrValues = new List<object> { "bgm" };
            _nodeCfgDic.Add(NodeType.PauseSound, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.ResumeSound,
                shortcutPath = "resumebgm",
                shortcutTip = "resume sound",
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            cfg.defaultAttrValues = new List<object> { "bgm" };
            _nodeCfgDic.Add(NodeType.ResumeSound, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.LoadSound,
                shortcutPath = "loadbgm",
                shortcutTip = "load sound",
                allowParents = new List<NodeType> { NodeType.Root, NodeType.Folder, NodeType.CodeBlock },
                allowChilds = new List<NodeType>(),
            };
            cfg.defaultAttrValues = new List<object> { "bgm" };
            _nodeCfgDic.Add(NodeType.LoadSound, cfg);
        }

        private void InitEffectNodeCfgs()
        {
            NodeConfig cfg;
            cfg = new NodeConfig
            {
                type = NodeType.CreateChargeEffect,
                shortcutPath = "charge",
                shortcutTip = "create charge effect",
                defaultAttrValues = new List<object> { "0", "0" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(cfg.type, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.CreateBurstEffect,
                shortcutPath = "burst",
                shortcutTip = "create burst effect",
                defaultAttrValues = new List<object> { "0", "0" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(cfg.type, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.ShakeScreen,
                shortcutPath = "shakescreen",
                shortcutTip = "shake screen",
                defaultAttrValues = new List<object> { "shake", "0", "270", "3", "3", "1.5", "5" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(cfg.type, cfg);

            cfg = new NodeConfig
            {
                type = NodeType.StopShakeScreen,
                shortcutPath = "stopshakescreen",
                shortcutTip = "stop shake screen",
                defaultAttrValues = new List<object> { "shake" },
                forbidParents = new List<NodeType> { NodeType.Root, NodeType.Folder },
                allowChilds = new List<NodeType>(),
            };
            _nodeCfgDic.Add(cfg.type, cfg);
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
