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

        private static Dictionary<string, NodeData> updateDic = new Dictionary<string, NodeData>();

        public static void UpdateProject(string projectPath)
        {
            NodeData nd = FileUtils.DeserializeFileToObject(projectPath) as NodeData;
            if (nd == null)
            {
                Logger.LogError("Deserialize nd file fail!");
            }
            updateDic.Clear();
            UpdateNodeData(nd);
            FileUtils.SerializableObjectToFile(projectPath, nd);
        }

        private static void UpdateNodeData(NodeData nd)
        {
            #region 第一版修改,将EnemyId从createEnemy变为OnCreateEnemy的属性
            //// 修改的函数
            //if (nd.type==(int)NodeType.DefineEnemy)
            //{
            //    string typeName = nd.attrValues[0];
            //    if (!updateDic.ContainsKey(typeName))
            //    {
            //        updateDic.Add(typeName, nd.childs[0]);
            //    }
            //}
            //else if (nd.type == (int)NodeType.CreateCustomizedEnemy)
            //{
            //    string typeName = nd.attrValues[0];
            //    string enemyId = nd.attrValues[1];
            //    nd.attrValues.RemoveAt(1);
            //    NodeData updateData;
            //    if (updateDic.TryGetValue(typeName, out updateData))
            //    {
            //        List<string> newValues = new List<string>();
            //        newValues.Add(updateData.attrValues[1]);
            //        newValues.Add(enemyId);
            //        newValues.Add(updateData.attrValues[0]);
            //        updateData.attrValues = newValues;
            //        updateDic.Remove(typeName);
            //    }
            //}
            #endregion
            #region 第二版修改,将BulletId从createBullet变为OnCreateBullet的属性
            // 修改的函数
            //if (nd.type == (int)NodeType.DefineBullet)
            //{
            //    string typeName = nd.attrValues[0];
            //    if (!updateDic.ContainsKey(typeName))
            //    {
            //        updateDic.Add(typeName, nd.childs[0]);
            //    }
            //}
            //else if (nd.type == (int)NodeType.CreateCustomizedBullet)
            //{
            //    string typeName = nd.attrValues[0];
            //    string enemyId = nd.attrValues[1];
            //    nd.attrValues.RemoveAt(1);
            //    NodeData updateData;
            //    if (updateDic.TryGetValue(typeName, out updateData))
            //    {
            //        List<string> newValues = new List<string>();
            //        newValues.Add(updateData.attrValues[0]);
            //        newValues.Add(enemyId);
            //        updateData.attrValues = newValues;
            //        updateDic.Remove(typeName);
            //    }
            //}
            #endregion
            #region 第三版修改，统一CreateCustomizedSTGObject接口
            //if (nd.type == (int)NodeType.CreateCusomizedSTGObject)
            //{
            //    List<string> newValues = new List<string>();
            //    newValues.Add(nd.attrValues[0]);
            //    newValues.Add("0");
            //    newValues.Add("0");
            //    newValues.Add(nd.attrValues[1]);
            //    nd.attrValues = newValues;
            //}
            #endregion
            #region 第四版修改，删除NodeDropItem中的ItemType，ItemCount属性
            if (nd.type == (int)NodeType.SetDropItems)
            {
                List<string> newValues = new List<string>();
                newValues.Add(nd.attrValues[0]);
                newValues.Add(nd.attrValues[1]);
                newValues.Add(nd.attrValues[2]);
                newValues.Add(nd.attrValues[7]);
                nd.attrValues = newValues;
            }
            #endregion
            for (int i=0;i<nd.childs.Count;i++)
            {
                UpdateNodeData(nd.childs[i]);
            }
        }


        public static BaseNode DebugStageNode { get; private set; }
        public static BaseNode DebugFromNode { get; private set; }
        /// <summary>
        /// 关卡调试
        /// </summary>
        public static bool IsDebugStage { get; private set; }


        public static BaseNode DebugSpellCardNode { get; private set; }
        /// <summary>
        /// 符卡调试
        /// </summary>
        public static bool IsDebugSpellCard { get; private set; }

        public static void SetDebugStageNode(BaseNode stageNode,BaseNode debugFromNode)
        {
            IsDebugStage = true;
            DebugStageNode = stageNode;
            DebugFromNode = debugFromNode;
        }

        public static void ClearDebugStageNode()
        {
            IsDebugStage = false;
        }

        public static void SetDebugSpellCardNode(BaseNode scNode)
        {
            IsDebugSpellCard = true;
            DebugSpellCardNode = scNode;
        }

        public static void ClearDebugSpellCardMode()
        {
            IsDebugSpellCard = false;
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
