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
        /// <summary>
        /// 选择了一个节点
        /// <para>param1 : BaseNode</para>
        /// </summary>
        public const int NodeSelected = 2001;
        /// <summary>
        /// 展开/收起一个节点
        /// </summary>
        public const int NodeExpanded = 2002;
        public const int FocusOnNode = 2003;
    }
}
