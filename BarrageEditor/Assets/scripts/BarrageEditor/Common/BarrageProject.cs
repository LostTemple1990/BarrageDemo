using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YKEngine;

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

        public static void LoadProject(string projectPath)
        {
            NodeData nd = FileUtils.DeserializeFileToObject(projectPath) as NodeData;
            if (nd == null)
            {
                Logger.LogError("Deserialize nd file fail!");
            }
            SetProjectPath(projectPath);
            BaseNode root = NodeManager.CreateNodesByNodeDatas(nd);
            root.SetParent(null);
            RootNode = root;
            root.Expand(true);
        }

        public static void UnloadProject()
        {
            CustomDefine.Clear();
            if (RootNode != null)
                RootNode.Destroy();
            RootNode = null;
            SetProjectPath(null);
        }

        public static void Log(string msg)
        {
            EventManager.GetInstance().PostEvent(EditorEvents.Log, msg);
        }

        public static void LogWarning(string msg)
        {
            EventManager.GetInstance().PostEvent(EditorEvents.LogWarning, msg);
        }

        public static void LogError(string msg)
        {
            EventManager.GetInstance().PostEvent(EditorEvents.LogError, msg);
        }
    }
}
