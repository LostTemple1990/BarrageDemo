using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarrageEditor
{
    public class BarrageProject
    {
        private static string projectPath = null;

        /// <summary>
        /// 设置工程的路径
        /// </summary>
        /// <param name="path"></param>
        public static void SetProjectPath(string path)
        {
            projectPath = path;
        }

        public static string GetProjectPath()
        {
            return projectPath;
        }

        /// <summary>
        /// 当前是否已经加载了工程
        /// </summary>
        /// <returns></returns>
        public static bool IsProjectLoaded()
        {
            return projectPath != null;
        }

        public static void CreateDefaultNodes()
        {
            BaseNode root = NodeManager.CreateNode(NodeType.Root);
            root.SetParent(null);
            root.SetAttrsDefaultValues();
            BaseNode ps = NodeManager.CreateNode(NodeType.ProjectSetting);
            ps.SetAttrsDefaultValues();
            root.InsertChildNode(ps, -1);
            // 设置根节点
            RootNode = root;
        }

        public static BaseNode RootNode { get; set; }
    }
}
