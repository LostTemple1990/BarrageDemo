using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YKEngine;
using System.Runtime.InteropServices;

namespace BarrageEditor
{
    public class MenuBarListView : ViewBase,IEventReciver
    {
        struct MenuItemData
        {
            public int id;
            public string name;
            public Action clickHandler;
        }


        private const int ItemId_New = 1;
        private const int ItemId_File = 2;
        private const int ItemId_Save = 3;
        private const int ItemId_SaveAs = 4;
        private const int ItemId_Close = 5;
        private const int ItemId_Exit = 6;


        private Dictionary<int, MenuItemData> _itemDataDic;
        /// <summary>
        /// 每个tab对应的itemId集合
        /// </summary>
        private Dictionary<int, List<int>> _listDatasDic;

        /// <summary>
        /// list的容器tf
        /// </summary>
        private Transform _containerTf;
        /// <summary>
        /// 当前显示的list的类型
        /// <para>1 file</para>
        /// <para>2 edit</para>
        /// <para>3 about</para>
        /// </summary>
        private int _curShowType;
        /// <summary>
        /// 
        /// </summary>
        private List<Transform> _itemTFList;

        protected override void Init()
        {
            _containerTf = _viewTf;
            #region 初始化每个tab对应的itemIds
            _listDatasDic = new Dictionary<int, List<int>>();
            List<int> list = new List<int>();
            list.Add(ItemId_New);
            list.Add(ItemId_File);
            list.Add(ItemId_Save);
            list.Add(ItemId_SaveAs);
            list.Add(ItemId_Close);
            list.Add(ItemId_Exit);
            _listDatasDic.Add(1, list);
            #endregion
            #region init listDatas
            _itemDataDic = new Dictionary<int, MenuItemData>();
            MenuItemData itemData = new MenuItemData
            {
                id = ItemId_File,
                name = "new...",
                clickHandler = NewClickHandler,
            };
            _itemDataDic.Add(ItemId_New, itemData);
            itemData = new MenuItemData
            {
                id = ItemId_File,
                name = "open...",
                clickHandler = OpenClickHander,
            };
            _itemDataDic.Add(ItemId_File, itemData);
            itemData = new MenuItemData
            {
                id = ItemId_File,
                name = "save...",
                clickHandler = SaveClickHander,
            };
            _itemDataDic.Add(ItemId_Save, itemData);
            itemData = new MenuItemData
            {
                id = ItemId_File,
                name = "saveAs...",
                clickHandler = SaveAsClickHander,
            };
            _itemDataDic.Add(ItemId_SaveAs, itemData);
            itemData = new MenuItemData
            {
                id = ItemId_File,
                name = "close...",
                clickHandler = CloseClickHander,
            };
            _itemDataDic.Add(ItemId_Close, itemData);
            itemData = new MenuItemData
            {
                id = ItemId_File,
                name = "exit...",
                clickHandler = ExitClickHandler,
            };
            _itemDataDic.Add(ItemId_Exit, itemData);
            #endregion
            _itemTFList = new List<Transform>();
            EventManager.GetInstance().Register(EngineEventID.WindowFocusChanged, this);
        }

        public override void OnShow(object data)
        {
            int showType = (int)data;
            RefreshList(showType);
        }

        public void RefreshList(int showType)
        {
            if (_curShowType != showType)
            {
                _curShowType = showType;
                List<int> itemIds;
                if (_listDatasDic.TryGetValue(_curShowType, out itemIds))
                {
                    for (int i = 0; i < itemIds.Count; i++)
                    {
                        AddItem(itemIds[i]);
                    }
                }
            }
        }

        private void AddItem(int itemId)
        {
            MenuItemData itemData;
            if (!_itemDataDic.TryGetValue(itemId, out itemData)) return;
            // 创建item
            GameObject item = ResourceManager.GetInstance().GetPrefab("Prefabs/Views/MainMenuList", "MainMenuListItem", _containerTf, false);
            Text itemText = item.transform.Find("Text").GetComponent<Text>();
            itemText.text = itemData.name;
            UIEventListener.Get(item).AddClick(itemData.clickHandler);
            // 将tf添加到list中
            _itemTFList.Add(item.transform);
        }

        public override void OnClose()
        {
            RemoveAllItems();
            EventManager.GetInstance().Remove(EngineEventID.WindowFocusChanged, this);
            base.OnClose();
        }

        private void RemoveAllItems()
        {
            GameObject itemGo;
            for (int i = 0; i < _itemTFList.Count; i++)
            {
                itemGo = _itemTFList[i].gameObject;
                UIEventListener.RemoveAllListeners(itemGo);
                GameObject.Destroy(itemGo);
            }
            _itemTFList.Clear();
        }

        private void NewClickHandler()
        {
            Close();
            string savePath = FileUtils.SaveFile("选择新建工程的位置", "关卡数据(*.nd)\0*.nd\0");
            if (savePath != null)
            {
                EventManager.GetInstance().PostEvent(EditorEvents.BeforeProjectChanged);
                BarrageProject.UnloadProject();
                // todo 载入固定位置的一个模板
                string templatePath = Application.streamingAssetsPath + "/template.nd";
                BarrageProject.LoadProject(templatePath);
                BarrageProject.SetProjectPath(savePath);
                EventManager.GetInstance().PostEvent(EditorEvents.AfterProjectChanged);
                // 当前文件
                BarrageProject.Log("current project file: " + FileUtils.GetFileNameByPath(savePath));
            }
        }

        private void OpenClickHander()
        {
            Close();
            string openPath = FileUtils.OpenFile("选择关卡数据", "关卡数据(*.nd)\0*.nd\0", false);
            if ( openPath != null )
            {
                EventManager.GetInstance().PostEvent(EditorEvents.BeforeProjectChanged);
                BarrageProject.UnloadProject();
                BarrageProject.LoadProject(openPath);
                EventManager.GetInstance().PostEvent(EditorEvents.AfterProjectChanged);
                BarrageProject.Log("current project file: " + FileUtils.GetFileNameByPath(openPath));
            }
        }

        private void SaveClickHander()
        {
            Close();
            string savePath = BarrageProject.GetProjectPath();
            if ( savePath != null )
            {
                BaseNode root = BarrageProject.RootNode;
                NodeData nd = NodeManager.SaveAsNodeData(root, true);
                FileUtils.SerializableObjectToFile(savePath, nd);
            }
        }

        private void SaveAsClickHander()
        {
            Close();
            string path = BarrageProject.GetProjectPath();
            if (path == null)
                return;
            string savePath = FileUtils.SaveFile("选择保存数据", "关卡数据(*.nd)\0*.nd\0");
            if (savePath != null)
            {
                BaseNode root = BarrageProject.RootNode;
                NodeData nd = NodeManager.SaveAsNodeData(root, true);
                FileUtils.SerializableObjectToFile(savePath, nd);
                BarrageProject.SetProjectPath(savePath);
            }
        }

        private void CloseClickHander()
        {
            Close();
            return;
            //string filePath = Application.streamingAssetsPath + "../../../../TouhouBarrageDemo/Assets/StreamingAssets/LuaRoot/stages/TestEditorStage.lua";
            //if (filePath != null)
            //{
            //    BaseNode root = BarrageProject.RootNode;
            //    string luaData = "";
            //    root.ToLua(0, ref luaData);
            //    FileUtils.WriteToFile(luaData, filePath);
            //}
            string openPath = FileUtils.OpenFile("选择要更新的关卡数据", "关卡数据(*.nd)\0*.nd\0", false);
            if (openPath != null)
            {
                EventManager.GetInstance().PostEvent(EditorEvents.BeforeProjectChanged);
                BarrageProject.UnloadProject();
                BarrageProject.UpdateProject(openPath);
                BarrageProject.LoadProject(openPath);
                EventManager.GetInstance().PostEvent(EditorEvents.AfterProjectChanged);
                BarrageProject.Log("current project file: " + FileUtils.GetFileNameByPath(openPath));
            }
        }

        private void ExitClickHandler()
        {
            Application.Quit();
        }

        public void Execute(int eventId, object data)
        {
            if ( eventId == EngineEventID.WindowFocusChanged )
            {
                if ( (int)data != _viewId )
                {
                    Close();
                }
            }
        }
    }
}
