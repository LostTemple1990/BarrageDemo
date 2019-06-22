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
        DefineBullet = 1501,
        OnBulletCreate = 1502,
        CreateBullet = 1053,
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
            #region bullet
            #region define bullet
            cfg = new NodeConfig
            {
                type = NodeType.DefineBullet,
                shortcutPath = "bulletdefine",
                shortcutTip = "define bullet",
                editOnCreated = true,
            };
            cfg.defaultAttrValues = new List<object> { "" };
            _nodeCfgDic.Add(NodeType.DefineBullet, cfg);
            #endregion
            #region onBulletCreate
            cfg = new NodeConfig
            {
                type = NodeType.OnBulletCreate,
                shortcutPath = "bulletdefine",
                shortcutTip = "define bullet",
                editOnCreated = false,
                isDeletable  = false,
            };
            cfg.defaultAttrValues = new List<object> { "v,angle" };
            _nodeCfgDic.Add(NodeType.OnBulletCreate, cfg);
            #endregion
            #region createBullet
            cfg = new NodeConfig
            {
                type = NodeType.CreateBullet,
                shortcutPath = "bulletcreate",
                shortcutTip = "create bullet",
                editOnCreated = true,
            };
            cfg.defaultAttrValues = new List<object> { "", "107010", "0", "0", "" };
            _nodeCfgDic.Add(NodeType.CreateBullet, cfg);
            #endregion
            #endregion
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
