using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YKEngine
{
    using System.Collections.Generic;

    public class EventManager
    {
        struct EventData
        {
            public int eventId;
            public object data;
        }

        private static EventManager _instance = new EventManager();

        public static EventManager GetInstance()
        {
            return _instance;
        }

        private Dictionary<int, List<IEventReciver>> _eventReciverMap;
        /// <summary>
        /// 下一帧需要执行的事件列表
        /// </summary>
        private List<EventData> _postEventsList;
        /// <summary>
        /// 下一帧需要派发的事件的个数
        /// </summary>
        private int _postEventsCount;

        public void Init()
        {
            if (_eventReciverMap == null)
            {
                _eventReciverMap = new Dictionary<int, List<IEventReciver>>();
            }
            _postEventsList = new List<EventData>();
            _postEventsCount = 0;
        }

        public void Register(int eventId, IEventReciver reciver)
        {
            List<IEventReciver> reciverList;
            if (!_eventReciverMap.TryGetValue(eventId, out reciverList))
            {
                reciverList = new List<IEventReciver>();
                _eventReciverMap.Add(eventId, reciverList);
            }
            if (!reciverList.Contains(reciver))
            {
                reciverList.Add(reciver);
            }
        }

        public void Remove(int eventId, IEventReciver reciver)
        {
            List<IEventReciver> reciverList;
            if (_eventReciverMap.TryGetValue(eventId, out reciverList))
            {
                int i;
                int count = reciverList.Count;
                for (i = 0; i < count; i++)
                {
                    if (reciverList[i] == reciver)
                    {
                        reciverList[i] = null;
                    }
                }
            }
        }

        public void PostEvent(int eventId, object data = null,bool immediately = true)
        {
            if ( immediately )
            {
                List<IEventReciver> reciverList;
                if (_eventReciverMap.TryGetValue(eventId, out reciverList))
                {
                    int i;
                    int count = reciverList.Count;
                    int removeFlag = 0;
                    for (i = 0; i < count; i++)
                    {
                        if (reciverList[i] != null)
                        {
                            reciverList[i].Execute(eventId, data);
                        }
                        else
                        {
                            removeFlag = 1;
                        }
                    }
                    if (removeFlag == 1)
                    {
                        CommonUtils.RemoveNullElementsInList<IEventReciver>(reciverList);
                    }
                }
            }
            else
            {
                EventData eventData = new EventData
                {
                    eventId = eventId,
                    data = data,
                };
                _postEventsList.Add(eventData);
                _postEventsCount++;
            }
        }

        public void Update()
        {
            if ( _postEventsCount != 0 )
            {
                EventData eventData;
                for (int i=0;i<_postEventsCount;i++)
                {
                    eventData = _postEventsList[i];
                    PostEvent(eventData.eventId, eventData.data, true);
                }
                _postEventsList.Clear();
                _postEventsCount = 0;
            }
        }
    }

    public interface IEventReciver
    {
        void Execute(int eventId, object data);
    }
}
