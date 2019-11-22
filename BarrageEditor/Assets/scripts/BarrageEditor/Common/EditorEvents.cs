using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarrageEditor
{
    public class EditorEvents
    {
        /// <summary>
        /// 显示提示文本
        /// </summary>
        public const int ShowTooltip = 1001;
        public const int Log = 1011;
        public const int LogWarning = 1012;
        public const int LogError = 1013;
        /// <summary>
        /// 工程文件改变
        /// </summary>
        public const int BeforeProjectChanged = 2001;
        public const int AfterProjectChanged = 2002;
        /// <summary>
        /// 选择了一个节点
        /// <para>param1 : BaseNode</para>
        /// </summary>
        public const int NodeSelected = 3001;
        /// <summary>
        /// 展开/收起一个节点完成之后调用
        /// </summary>
        public const int NodeExpandedFinished = 3002;
        /// <summary>
        /// 当节点可见/描述文本发生变化时抛出
        /// <para>更新ProjectPanel的宽度</para>
        /// <para>param1 NodeBase node</para>
        /// </summary>
        public const int UpdateProjectPanelWidth = 3003;

        public const int FocusOnNode = 3005;
        /// <summary>
        /// define类型的节点被删除
        /// <para>CustomizedType type</para>
        /// <para>string typeName</para>
        /// </summary>
        public const int DefineNodeDestroy = 3006;
    }
}
