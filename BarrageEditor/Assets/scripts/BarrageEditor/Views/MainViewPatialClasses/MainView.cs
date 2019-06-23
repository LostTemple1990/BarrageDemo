using UnityEngine;
using UnityEngine.UI;
using YKEngine;
using System.Collections.Generic;

namespace BarrageEditor
{
    public partial class MainView : ViewBase,IEventReciver
    {
        private GameObject _fileToggle;
        private RectTransform _fileToggleTf;
        private GameObject _editToggle;
        private GameObject _helpToggle;

        /// <summary>
        /// 工程树形面板的go
        /// </summary>
        private RectTransform _projectPanelTf;
        private RectTransform _projectScrollViewTf;
        private RectTransform _projectContentTf;
        /// <summary>
        /// project节点信息面板最小高度
        /// </summary>
        private const float ProjectContentMinHeight = 521;
        /// <summary>
        /// project面板当前高度
        /// </summary>
        private float _projectContentCurHeight;
        /// <summary>
        /// 节点详细信息的面板
        /// </summary>
        private RectTransform _nodeInfoPanelTf;
        private Scrollbar _projectContentScrollbar;

        struct NodeAttrItem
        {
            public GameObject itemGo;
            public Text attrNameText;
            public InputField valueText;
            public GameObject dropdownArrowGo;
            public Dropdown dropdown;
            public GameObject editBtn;
        }
        /// <summary>
        /// 节点属性的item结构
        /// </summary>
        private List<NodeAttrItem> _nodeAttrItems;
        /// <summary>
        /// 节点名称的text
        /// </summary>
        private Text _nodeNameText;

        /// <summary>
        /// 当前选择的节点
        /// </summary>
        private BaseNode _curSelectedNode;
        /// <summary>
        /// 当前插入节点的方式
        /// </summary>
        private NodeInsertMode _curInsertMode;

        protected override void Init()
        {
            Transform menuBarTf = _view.transform.Find("MenuBar");
            _fileToggleTf = menuBarTf.Find("File") as RectTransform;
            _fileToggle = _fileToggleTf.gameObject;
            _editToggle = menuBarTf.Find("Edit").gameObject;
            _helpToggle = menuBarTf.Find("Help").gameObject;

            InitFocus();

            InitProjectShortcutBar();
            InitNodeShortcutBar();

            InitLogInfoPanel();
            InitNodeInfoPanel();
            InitProjectPanel();

            AddListeners();
            UIManager.GetInstance().RegisterViewUpdate(this);
        }

        #region nodeShortcuts
        private GameObject _nodeShortcutTabGeneral;
        private GameObject _nodeShortcutTabStage;
        private GameObject _nodeShortcutTabTask;
        private GameObject _nodeShortcutTabEnemy;
        private GameObject _nodeShortcutTabBoss;
        private GameObject _nodeShortcutTabBullet;
        private GameObject _nodeShortcutTabTools;
        private GameObject _nodeShortcutTabLaser;

        /// <summary>
        /// 节点快捷图标的容器
        /// </summary>
        private RectTransform _nodeShortcutsContainerTf;
        /// <summary>
        /// 当前选中的节点tab中拥有的所有节点gameObject
        /// </summary>
        private List<GameObject> _nodeShortcutList; 

        enum NodeShortcutTab
        {
            General = 0,
            Stage = 1,
            Task = 2,
            Enemy = 3,
            Boss = 4,
            Bullet = 5,
            Count = 6,
        };

        private Dictionary<NodeShortcutTab, List<NodeType>> _nodeTabShortcutDic;

        private void InitNodeShortcutBar()
        {
            // 每个tab拥有的节点信息
            _nodeTabShortcutDic = new Dictionary<NodeShortcutTab, List<NodeType>>();
            List<NodeType> typeList;
            // general
            typeList = new List<NodeType> { NodeType.Folder, NodeType.CodeBlock, NodeType.If, NodeType.DefVar, NodeType.Repeat, NodeType.Code, NodeType.Comment };
            _nodeTabShortcutDic.Add(NodeShortcutTab.General, typeList);
            // Stage
            typeList = new List<NodeType> { NodeType.StageGroup, NodeType.Stage };
            _nodeTabShortcutDic.Add(NodeShortcutTab.Stage, typeList);
            //Bullet
            typeList = new List<NodeType> { NodeType.DefineBullet, NodeType.CreateBullet };
            _nodeTabShortcutDic.Add(NodeShortcutTab.Bullet, typeList);


            RectTransform barTf = _viewTf.Find("NodeShortcutBar").GetComponent<RectTransform>();

            _nodeShortcutsContainerTf = barTf.Find("NodeShortcuts").GetComponent<RectTransform>();
            _nodeShortcutList = new List<GameObject>();

            _nodeShortcutTabGeneral = barTf.Find("Gernal").gameObject;
            UIEventListener.Get(_nodeShortcutTabGeneral).AddClick(OnTabGeneralClickHandler);

            _nodeShortcutTabStage = barTf.Find("Stage").gameObject;
            UIEventListener.Get(_nodeShortcutTabStage).AddClick(OnTabStageClickHandler);

            _nodeShortcutTabBullet = barTf.Find("Bullet").gameObject;
            UIEventListener.Get(_nodeShortcutTabBullet).AddClick(OnTabBulletClickHandler);

            // 默认选中general
            _nodeShortcutTabGeneral.GetComponent<Toggle>().isOn = true;
            OnTabGeneralClickHandler();
        }

        private void OnTabGeneralClickHandler()
        {
            AddShortcutsByTab(NodeShortcutTab.General);
        }

        private void OnTabStageClickHandler()
        {
            AddShortcutsByTab(NodeShortcutTab.Stage);
        }

        private void OnTabBulletClickHandler()
        {
            AddShortcutsByTab(NodeShortcutTab.Bullet);
        }

        private void AddShortcutsByTab(NodeShortcutTab tab)
        {
            RemoveAllNodeShortcuts();
            List<NodeType> typeList;
            if (!_nodeTabShortcutDic.TryGetValue(tab, out typeList))
            {
                return;
            }
            for (int i = 0; i < typeList.Count; i++)
            {
                NodeConfig nodeCfg = DatabaseManager.NodeDatabase.GetNodeCfgByNodeType(typeList[i]);
                if (nodeCfg == null)
                {
                    Logger.LogError("Node Config is not exist!! NodeType = " + typeList[i]);
                }
                GameObject shortcutGo = ResourceManager.GetInstance().GetPrefab("Prefabs/Views", "MainView/NodeShortcut");
                RectTransform shortcutTf = shortcutGo.GetComponent<RectTransform>();
                shortcutTf.SetParent(_nodeShortcutsContainerTf, false);
                GameObject btnGo = shortcutTf.Find("BgBtn").gameObject;
                // 添加监听事件
                UIEventListener.Get(btnGo).AddPointerEnter(() =>
                {
                    UIManager.GetInstance().OpenView(ViewID.TooltipView, nodeCfg.shortcutTip);
                });
                UIEventListener.Get(btnGo).AddPointerExit(() =>
                {
                    UIManager.GetInstance().CloseView(ViewID.TooltipView);
                });
                UIEventListener.Get(btnGo).AddClick(() =>
                {
                    TryInsertNode(nodeCfg.type);
                });
                Image img = shortcutTf.Find("Image").GetComponent<Image>();
                img.sprite = ResourceManager.GetInstance().GetSprite("ShortcutsAtlas", nodeCfg.shortcutPath);
                // 添加到列表中
                _nodeShortcutList.Add(shortcutGo);
            }
        }

        private void TryInsertNode(NodeType nodeType)
        {
            //if (!BarrageProject.IsProjectLoaded())
            //{
            //    BarrageProject.LogError("Please load a project before you want to insert");
            //    return;
            //}
            if (_curSelectedNode == null)
            {
                BarrageProject.LogError("Please select a node before you want to insert");
                return;
            }
            BaseNode parentNode = null;
            BaseNode newNode = null;
            int insertIndex = -1;
            GetParentNodeAndInsertIndexByInsertMode(_curInsertMode, _curSelectedNode, out parentNode, out insertIndex);
            if (parentNode != null)
            {
                // 先判断是否可以插入子节点
                if ( !parentNode.CheckCanInsertChildNode(nodeType) )
                {
                    string msg = string.Format("can not insert {0} as child of {1}", nodeType, parentNode.GetNodeType());
                    BarrageProject.LogError(msg);
                    return;
                }
                // 插入子节点
                newNode = NodeManager.CreateNode(nodeType);
                newNode.SetAttrsDefaultValues();
                parentNode.InsertChildNode(newNode, insertIndex);
                // 设置操作记录
                OpInsertHM hm = new OpInsertHM
                {
                    parentIndex = NodeManager.GetNodeIndex(parentNode),
                    childIndex = parentNode.GetChildIndex(newNode),
                    insertNodeData = NodeManager.SaveAsNodeData(newNode, true),
                };
                Undo.AddToUndoTask(hm);
                // 展开父节点
                parentNode.Expand(true);
                newNode.OnSelected(true);
                SetContentToNode(newNode);
            }
        }

        /// <summary>
        /// 根据当前节点以及插入方式来获取插入的父节点以及插入的位置索引
        /// </summary>
        /// <param name="insertMode"></param>
        /// <param name="curNode"></param>
        /// <param name="parent"></param>
        /// <param name="index"></param>
        private void GetParentNodeAndInsertIndexByInsertMode(NodeInsertMode insertMode, BaseNode curNode, out BaseNode parent, out int index)
        {
            // 选中根节点的时候，只能作为子节点插入
            if (curNode.GetNodeType() == NodeType.Root)
            {
                parent = _curSelectedNode;
                index = -1;
            }
            else
            {
                if (insertMode == NodeInsertMode.InsertAfter)
                {
                    parent = _curSelectedNode.parentNode;
                    index = parent.GetChildIndex(_curSelectedNode) + 1;
                }
                else if (insertMode == NodeInsertMode.InsertBefore)
                {
                    parent = _curSelectedNode.parentNode;
                    index = parent.GetChildIndex(_curSelectedNode);
                }
                else
                {
                    parent = _curSelectedNode;
                    index = -1;
                }
            }
        }

        private void RemoveAllNodeShortcuts()
        {
            GameObject item;
            for (int i=0;i<_nodeShortcutList.Count;i++)
            {
                item = _nodeShortcutList[i];
                UIEventListener.Get(item.transform.Find("BgBtn").gameObject).RemoveAllEvents();
                GameObject.Destroy(item);
            }
            _nodeShortcutList.Clear();
        }
        #endregion

        #region Log
        enum LogLevel : byte
        {
            Normal = 1,
            Warning = 2,
            Error = 3,
        }

        private RectTransform _logInfoContainer;
        private ScrollRect _logPanelScrollRect;
        private Scrollbar _logPanelScrollBar;
        private List<GameObject> _logGoList;
        private const int LogMaxCount = 50;

        private void InitLogInfoPanel()
        {
            Transform tf = _viewTf.Find("LogPanel/Scroll View");
            _logPanelScrollRect = tf.GetComponent<ScrollRect>();
            _logPanelScrollBar = tf.Find("Scrollbar Vertical").GetComponent<Scrollbar>();
            _logInfoContainer = tf.Find("Viewport/Content").GetComponent<RectTransform>();
            _logGoList = new List<GameObject>();
        }

        private void AddLogItem(LogLevel level,string msg)
        {
            GameObject go = ResourceManager.GetInstance().GetPrefab("Prefabs/Views", "MainView/LogItem");
            Transform tf = go.transform;
            Image logImg = tf.Find("LogImage").GetComponent<Image>();
            Text logText = tf.Find("LogText").GetComponent<Text>();
            if (level == LogLevel.Normal)
                logImg.sprite = ResourceManager.GetInstance().GetSprite("ShortcutsAtlas", "Info");
            else if (level == LogLevel.Warning)
                logImg.sprite = ResourceManager.GetInstance().GetSprite("ShortcutsAtlas", "Warning");
            else
                logImg.sprite = ResourceManager.GetInstance().GetSprite("ShortcutsAtlas", "Error");
            logText.text = "    " + msg;
            if ( _logGoList.Count >= LogMaxCount)
            {
                GameObject removeGo = _logGoList[0];
                _logGoList.RemoveAt(0);
                GameObject.Destroy(removeGo);
            }
            tf.SetParent(_logInfoContainer,false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_logInfoContainer);
            _logPanelScrollRect.Rebuild(CanvasUpdate.PostLayout);
            _logPanelScrollBar.value = 0;
            _logGoList.Add(go);
        }
        #endregion

        private void InitNodeInfoPanel()
        {
            _nodeInfoPanelTf = _viewTf.Find("NodeInfoPanel").GetComponent<RectTransform>();
            _nodeAttrItems = new List<NodeAttrItem>();
            _nodeNameText = _viewTf.Find("NodeInfoPanel/NodeTypeTextItem/NodeTypeText").GetComponent<Text>();

            ResetNodeAttrInfo();
        }

        private void InitProjectPanel()
        {
            _projectPanelTf = _viewTf.Find("ProjectPanel").GetComponent<RectTransform>();
            _projectScrollViewTf = _viewTf.Find("ProjectPanel/Scroll View").GetComponent<RectTransform>();
            _projectContentTf = _projectScrollViewTf.Find("Viewport/Content").GetComponent<RectTransform>();
            _projectContentScrollbar = _projectScrollViewTf.Find("Scrollbar Vertical").GetComponent<Scrollbar>();
            _curSelectedNode = null;
            EventManager.GetInstance().Register(EditorEvents.BeforeProjectChanged, this);
            EventManager.GetInstance().Register(EditorEvents.AfterProjectChanged, this);
            EventManager.GetInstance().Register(EditorEvents.NodeSelected, this);
            EventManager.GetInstance().Register(EditorEvents.NodeExpanded, this);
            EventManager.GetInstance().Register(EditorEvents.Log, this);
            EventManager.GetInstance().Register(EditorEvents.LogWarning, this);
            EventManager.GetInstance().Register(EditorEvents.LogError, this);
            //EventManager.GetInstance().Register(EditorEvents.FocusOnNode, this);
            NodeManager.SetNodeItemContainer(_projectContentTf);
            TestProjectNodeItems();
        }

        public void AddListeners()
        {
            UIEventListener.Get(_fileToggle).AddClick(OnFileTabClick);
        }

        private void OnFileTabClick()
        {
            Vector2 uiPos = UIManager.GetInstance().GetUIPosition(_fileToggleTf);
            Vector2 viewPos = new Vector2(uiPos.x - _fileToggleTf.rect.width / 2, uiPos.y - _fileToggleTf.rect.height / 2);
            UIManager.GetInstance().OpenView(ViewID.MenuBarListView, 1, viewPos);
        }

        private void UpdateAttrPanel()
        {

        }

        private void TestProjectNodeItems()
        {
            BarrageProject.CreateDefaultNodes();
            BarrageProject.RootNode.Expand(true);
        }

        /// <summary>
        /// 将视角切换到node处
        /// </summary>
        /// <param name="node"></param>
        private void SetContentToNode(BaseNode node)
        {
            int showIndex = node.GetShowIndex();
            float toY = showIndex * BaseNode.NodeHeight;
            float max = _projectContentCurHeight - ProjectContentMinHeight;
            float toValue = 1;
            // 当前内容没有填满content，
            if (max <= 0)
            {
                toValue = 1;
            }
            // 节点之下的内容不足整个content的高度，则默认拉到最底下
            else if (toY > max)
            {
                toValue = 0;
            }
            else
            {
                toValue = 1 - toY / max;
            }
            // 根据content的最大高度进行校正
            //Logger.Log("posY = " + toY + " max = " + max);
            //Logger.Log("toValue = " + toValue);
            _projectContentScrollbar.value = toValue;
            //_projectContentTf.anchoredPosition = new Vector2(0, toY);
        }

        public void Execute(int eventId, object data)
        {
            switch ( eventId )
            {
                case EditorEvents.BeforeProjectChanged:
                    BeforeProjectChanged();
                    break;
                case EditorEvents.NodeSelected:
                    OnNodeSelected(data as BaseNode);
                    break;
                case EditorEvents.NodeExpanded:
                    OnNodeExpanded((int)data);
                    break;
                case EditorEvents.Log:
                    AddLogItem(LogLevel.Normal, data as string);
                    break;
                case EditorEvents.LogWarning:
                    AddLogItem(LogLevel.Warning, data as string);
                    break;
                case EditorEvents.LogError:
                    AddLogItem(LogLevel.Error, data as string);
                    break;
            }
        }

        private void BeforeProjectChanged()
        {
            if ( _curSelectedNode != null )
            {
                ResetNodeAttrInfo();
                _curSelectedNode = null;
            }
        }

        private void OnNodeSelected(BaseNode node)
        {
            if (node == _curSelectedNode) return;
            if ( _curSelectedNode != null )
            {
                ResetNodeAttrInfo();
                _curSelectedNode.OnSelected(false);
            }
            _curSelectedNode = node;
            UpdateNodeNameItem();
            UpdateNodeAttrItems();
            // 检测是否需要打开第一个参数的编辑界面
            NodeConfig cfg = DatabaseManager.NodeDatabase.GetNodeCfgByNodeType(node.GetNodeType());
            if ( cfg.editOnCreated && node.GetAttrByIndex(0).GetValueString() == "" )
            {
                node.GetAttrByIndex(0).OpenEditView();
            }
        }

        private void OnNodeExpanded(int lastIndex)
        {
            // index是从0开始计算的，因此算高度的时候要+1
            float preferredHeight = (lastIndex + 1) * BaseNode.NodeHeight;
            if (preferredHeight < ProjectContentMinHeight)
            {
                preferredHeight = ProjectContentMinHeight;
            }
            _projectContentCurHeight = preferredHeight;
            _projectContentTf.sizeDelta = new Vector2(0, preferredHeight);
            _projectScrollViewTf.GetComponent<ScrollRect>().Rebuild(CanvasUpdate.PostLayout);
            //LayoutRebuilder.ForceRebuildLayoutImmediate(_projectScrollViewTf);
        }

        private void ResetNodeAttrInfo()
        {
            // 节点解除绑定
            if ( _curSelectedNode != null )
            {
                List<BaseNodeAttr> _attributeList = _curSelectedNode.GetAttrs();
                for (int i=0;i<_attributeList.Count;i++)
                {
                    _attributeList[i].UnbindItem();
                }
            }
            _nodeNameText.text = "please select a node to show attributes";
            for (int i=0;i<_nodeAttrItems.Count;i++)
            {
                NodeAttrItem item = _nodeAttrItems[i];
                item.itemGo.SetActive(false);
                item.dropdown.options.Clear();
                item.dropdownArrowGo.SetActive(true);
            }
        }

        private void UpdateNodeNameItem()
        {
            _nodeNameText.text = "Node type: " + _curSelectedNode.GetNodeName();
        }

        private void UpdateNodeAttrItems()
        {
            List<BaseNodeAttr> _attributeList = _curSelectedNode.GetAttrs();
            int attrCount = _attributeList.Count;
            int i;
            BaseNodeAttr attribute;
            if ( attrCount > _nodeAttrItems.Count )
            {
                for (i = _nodeAttrItems.Count; i < attrCount; i++)
                {
                    NodeAttrItem item = CreateNodeAttrItem();
                    _nodeAttrItems.Add(item);
                }
            }
            for (i = 0; i < attrCount; i++)
            {
                attribute = _attributeList[i];
                _nodeAttrItems[i].itemGo.SetActive(true);
                attribute.BindItem(_nodeAttrItems[i].itemGo);
            }
        }

        private NodeAttrItem CreateNodeAttrItem()
        {
            GameObject itemGo = ResourceManager.GetInstance().GetPrefab("Prefabs/Views", "MainView/NodeAttributeItem");
            RectTransform tf = itemGo.GetComponent<RectTransform>();
            tf.SetParent(_nodeInfoPanelTf, false);
            NodeAttrItem item = new NodeAttrItem();
            item.itemGo = itemGo;
            item.attrNameText = tf.Find("AttrNameText").GetComponent<Text>();
            item.valueText = tf.Find("Dropdown/Label").GetComponent<InputField>();
            item.dropdownArrowGo = tf.Find("Dropdown/Arrow").gameObject;
            item.dropdown = tf.Find("Dropdown").GetComponent<Dropdown>();
            item.editBtn = tf.Find("EditBtn").gameObject;
            return item;
        }

        public override void Update()
        {
            CheckFocus();
            CheckHotKeys();
        }
    }
}
