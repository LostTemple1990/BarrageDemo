using System.Collections.Generic;
using YKEngine;

namespace BarrageEditor
{
    public class ViewID
    {
        public const int MainView = 1;
        public const int MenuBarListView = 2;

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
            UIManager.GetInstance().InitViewCfgs(_viewCfgMap);
        }
    }
}
