using System.Collections.Generic;
using YKEngine;

namespace BarrageEditor
{
    public class ViewID
    {
        public const int MainView = 1001;
        public const int MenuBarListView = 1002;
        /// <summary>
        /// 提示文本界面
        /// </summary>
        public const int TooltipView = 1003;
        /// <summary>
        /// 节点属性文本编辑界面
        /// </summary>
        public const int AttrEditTextView = 2001;
        // <summary>
        /// 子弹id选择界面
        /// </summary>
        public const int AttrEditBulletIdView = 2002;
        /// <summary>
        /// 弹型选择界面
        /// </summary>
        public const int AttrSelectBulletStyleView = 2003;
        /// <summary>
        /// 颜色选择界面
        /// </summary>
        public const int AttrSelectBulletColorView = 2004;
        /// <summary>
        /// 选择自定义类型的界面
        /// </summary>
        public const int AttrSelectCustomizedTypeView = 2005;
        /// <summary>
        /// 自定义类型的参数编辑界面
        /// </summary>
        public const int AttrEditParasView = 2006;
        /// <summary>
        /// 编辑忽略碰撞组
        /// </summary>
        public const int AttrEditCollisionGroup = 2007;
        /// <summary>
        /// 编辑抵抗的消除类型
        /// </summary>
        public const int AttrEditResistEliminatedTypes = 2008;


        private static Dictionary<int, ViewCfg> _viewCfgMap;

        public static void Init()
        {
            _viewCfgMap = new Dictionary<int, ViewCfg>();
            // 主界面
            ViewCfg cfg = new ViewCfg()
            {
                viewId = MainView,
                resPath = "MainView/MainUI",
                className = "BarrageEditor.MainView",
                layer = LayerId.Bottom
            };
            _viewCfgMap.Add(MainView, cfg);
            // 菜单栏下拉框
            cfg = new ViewCfg()
            {
                viewId = MenuBarListView,
                resPath = "MainMenuList/MainMenuList",
                className = "BarrageEditor.MenuBarListView",
                layer = LayerId.Normal
            };
            _viewCfgMap.Add(MenuBarListView, cfg);
            #region 提示文本界面
            cfg = new ViewCfg()
            {
                viewId = TooltipView,
                resPath = "MainView/TooltipView",
                className = "BarrageEditor.TooltipView",
                layer = LayerId.Normal
            };
            _viewCfgMap.Add(TooltipView, cfg);
            #endregion
            // 节点属性文本编辑界面
            cfg = new ViewCfg()
            {
                viewId = AttrEditTextView,
                resPath = "EditViews/EditTextView",
                className = "BarrageEditor.AttrEditTextView",
                layer = LayerId.Normal
            };
            _viewCfgMap.Add(AttrEditTextView, cfg);
            #region 子弹id选择界面
            cfg = new ViewCfg()
            {
                viewId = AttrEditBulletIdView,
                resPath = "EditViews/EditBulletIdView",
                className = "BarrageEditor.AttrEditBulletIdView",
                layer = LayerId.Normal
            };
            _viewCfgMap.Add(AttrEditBulletIdView, cfg);
            #endregion
            #region 弹型选择界面
            cfg = new ViewCfg()
            {
                viewId = AttrSelectBulletStyleView,
                resPath = "EditViews/SelectBulletStyleView",
                className = "BarrageEditor.AttrSelectBulletStyleView",
                layer = LayerId.Normal
            };
            _viewCfgMap.Add(AttrSelectBulletStyleView, cfg);
            #endregion
            #region 颜色选择界面
            cfg = new ViewCfg()
            {
                viewId = AttrSelectBulletColorView,
                resPath = "EditViews/SelectBulletColorView",
                className = "BarrageEditor.AttrSelectBulletColorView",
                layer = LayerId.Normal
            };
            _viewCfgMap.Add(AttrSelectBulletColorView, cfg);
            #endregion
            #region 自定义类型选择界面
            cfg = new ViewCfg()
            {
                viewId = AttrSelectCustomizedTypeView,
                resPath = "EditViews/SelectCustomizedTypeView",
                className = "BarrageEditor.AttrEditSelectCustomizedTypeView",
                layer = LayerId.Normal
            };
            _viewCfgMap.Add(AttrSelectCustomizedTypeView, cfg);
            #endregion
            #region 参数编辑界面
            cfg = new ViewCfg()
            {
                viewId = AttrEditParasView,
                resPath = "EditViews/EditParaView",
                className = "BarrageEditor.AttrEditParasView",
                layer = LayerId.Normal
            };
            _viewCfgMap.Add(AttrEditParasView, cfg);
            #endregion
            #region 编辑忽略碰撞组界面
            cfg = new ViewCfg()
            {
                viewId = AttrEditCollisionGroup,
                resPath = "EditViews/EditCollisionGroupView",
                className = "BarrageEditor.AttrEditCollisionGroupView",
                layer = LayerId.Normal
            };
            _viewCfgMap.Add(AttrEditCollisionGroup, cfg);
            #endregion
            #region 编辑抵抗消除类型界面
            cfg = new ViewCfg()
            {
                viewId = AttrEditResistEliminatedTypes,
                resPath = "EditViews/EditResistEliminatedTypesView",
                className = "BarrageEditor.AttrEditResistEliminatedTypesView",
                layer = LayerId.Normal
            };
            _viewCfgMap.Add(AttrEditResistEliminatedTypes, cfg);
            #endregion
            UIManager.GetInstance().InitViewCfgs(_viewCfgMap);
        }
    }
}
