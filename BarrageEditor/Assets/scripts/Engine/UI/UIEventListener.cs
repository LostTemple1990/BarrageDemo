using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace YKEngine
{
    public class UIEventListener : MonoBehaviour, IPointerClickHandler,
                                                     IPointerDownHandler,
                                                     IPointerEnterHandler,
                                                     IPointerExitHandler,
                                                     IPointerUpHandler,
                                                     IBeginDragHandler,
                                                     IEndDragHandler,
                                                     IDragHandler
    {
        public static UIEventListener Get(GameObject go)
        {
            if (go == null)
            {
                Logger.LogWarn("调用UIEventListener的对象为空！");
                return null;
            }
            UIEventListener listener = go.GetComponent<UIEventListener>();
            if (listener == null)
            {
                listener = go.AddComponent<UIEventListener>();
            }
            return listener;
        }

        public static void RemoveAllListeners(GameObject go)
        {
            UIEventListener[] listeners = go.GetComponentsInChildren<UIEventListener>();
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].RemoveAllEvents();
            }
        }

        public enum eEventPassType : byte
        {
            NotPass = 0,
            PassBefore = 1,
            PassAfter = 2,
        }

        public eEventPassType eventPassType;

        private Action _onPointerClick;
        private Action _onPointerDown;
        private Action _onPointerEnter;
        private Action _onPointerExit;
        private Action _onPointerUp;
        private Action<Vector2> _onDrag;
        private Action _onDragBegin;
        private Action _onDragEnd;

        public void OnPointerClick(PointerEventData data)
        {
            if ( eventPassType == eEventPassType.PassBefore )
                PassEvent(data, ExecuteEvents.pointerClickHandler);
            if (_onPointerClick != null)
            {
                _onPointerClick();
            }
            if ( eventPassType == eEventPassType.PassAfter)
                PassEvent(data, ExecuteEvents.pointerClickHandler);
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (eventPassType == eEventPassType.PassBefore)
                PassEvent(data, ExecuteEvents.pointerDownHandler);
            if (_onPointerDown != null)
            {
                _onPointerDown();
            }
            if (eventPassType == eEventPassType.PassAfter)
                PassEvent(data, ExecuteEvents.pointerDownHandler);
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (eventPassType == eEventPassType.PassBefore)
                PassEvent(data, ExecuteEvents.pointerEnterHandler);
            if (_onPointerEnter != null)
            {
                _onPointerEnter();
            }
            if (eventPassType == eEventPassType.PassAfter)
                PassEvent(data, ExecuteEvents.pointerEnterHandler);
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (eventPassType == eEventPassType.PassBefore)
            {
                PassEvent(data, ExecuteEvents.pointerExitHandler);
            }
            if (_onPointerExit != null)
            {
                _onPointerExit();
            }
            if (eventPassType == eEventPassType.PassAfter)
            {
                PassEvent(data, ExecuteEvents.pointerExitHandler);
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (eventPassType == eEventPassType.PassBefore)
            {
                PassEvent(data, ExecuteEvents.pointerUpHandler);
            }
            if (_onPointerUp != null)
            {
                _onPointerUp();
            }
            if (eventPassType == eEventPassType.PassAfter)
            {
                PassEvent(data, ExecuteEvents.pointerUpHandler);
            }
        }

        public void OnBeginDrag(PointerEventData data)
        {
            if (eventPassType == eEventPassType.PassBefore)
            {
                PassEvent(data, ExecuteEvents.beginDragHandler);
            }
            if (_onDragBegin != null)
            {
                _onDragBegin();
            }
            if (eventPassType == eEventPassType.PassAfter)
            {
                PassEvent(data, ExecuteEvents.beginDragHandler);
            }
        }

        public void OnEndDrag(PointerEventData data)
        {
            if (eventPassType == eEventPassType.PassBefore)
            {
                PassEvent(data, ExecuteEvents.endDragHandler);
            }
            if (_onDragEnd != null)
            {
                _onDragEnd();
            }
            if (eventPassType == eEventPassType.PassAfter)
            {
                PassEvent(data, ExecuteEvents.endDragHandler);
            }
        }

        public void OnDrag(PointerEventData data)
        {
            if (eventPassType == eEventPassType.PassBefore)
            {
                PassEvent(data, ExecuteEvents.dragHandler);
            }
            if (_onDrag != null)
            {
                _onDrag(data.delta);
            }
            if (eventPassType == eEventPassType.PassAfter)
            {
                PassEvent(data, ExecuteEvents.dragHandler);
            }
        }


        public void AddClick(Action onClick)
        {
            _onPointerClick = onClick;
        }

        public void AddPointerEnter(Action onPointerEnter)
        {
            _onPointerEnter = onPointerEnter;
        }

        public void AddPointerExit(Action onPointerExit)
        {
            _onPointerExit = onPointerExit;
        }

        public void AddPointDown(Action onPointerDown)
        {
            _onPointerDown = onPointerDown;
        }

        public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            GameObject current = data.pointerCurrentRaycast.gameObject;
            bool breakloop = false;
            for (int i = 0; i < results.Count; i++)
            {
                if ( current == results[i].gameObject)
                {
                    breakloop = true;
                    continue;
                }
                if (breakloop && current != results[i].gameObject)
                {
                    ExecuteEvents.Execute(results[i].gameObject, data, function);
                    break;
                }
            }
        }

        public void RemoveAllEvents()
        {
            _onPointerClick = null;
            _onPointerEnter = null;
            _onPointerExit = null;
        }
    }
}
