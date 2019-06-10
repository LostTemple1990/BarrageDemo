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

        public bool editFirst;
    }

    public enum NodeType : int
    {
        Root = 0,
        ProjectSetting = 1,
        Folder = 2,
        CodeBlock = 3,
        Code = 4,
        DefineBullet = 10,
    }

    public class NodeDatabase
    {
        private Dictionary<NodeType, NodeConfig> _nodeCfgDic;

        public NodeDatabase()
        {
            _nodeCfgDic = new Dictionary<NodeType, NodeConfig>();
            NodeConfig cfg;
            List<object> values;
            #region root
            cfg = new NodeConfig { type = NodeType.Root };
            cfg.defaultAttrValues = new List<object>();
            _nodeCfgDic.Add(NodeType.Root, cfg);
            #endregion
            #region projectSetting
            cfg = new NodeConfig { type = NodeType.ProjectSetting };
            values = new List<object>();
            values.Add("unnamed");
            values.Add("YK");
            values.Add("true");
            cfg.defaultAttrValues = values;
            _nodeCfgDic.Add(NodeType.ProjectSetting, cfg);
            #endregion
            #region general
            #region folder
            cfg = new NodeConfig
            {
                type = NodeType.Folder,
                shortcutPath = "folder",
                shortcutTip = "folder",
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
            };
            cfg.defaultAttrValues = new List<object> { "" };
            _nodeCfgDic.Add(NodeType.CodeBlock, cfg);
            #endregion
            #endregion
            #region bullet
            #region define bullet
            cfg = new NodeConfig
            {
                type = NodeType.DefineBullet,
                shortcutPath = "bulletdefine",
                shortcutTip = "define bullet",
                editFirst = true,
            };
            cfg.defaultAttrValues = new List<object> { "" };
            _nodeCfgDic.Add(NodeType.DefineBullet, cfg);
            #endregion
            #endregion
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
