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
            if (_onPointerClick != null)
            {
                _onPointerClick();
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (_onPointerDown != null)
            {
                _onPointerDown();
            }
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (_onPointerEnter != null)
            {
                _onPointerEnter();
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            if (_onPointerExit != null)
            {
                _onPointerExit();
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (_onPointerUp != null)
            {
                _onPointerUp();
            }
        }

        public void OnBeginDrag(PointerEventData data)
        {
            if (_onDragBegin != null)
            {
                _onDragBegin();
            }
        }

        public void OnEndDrag(PointerEventData data)
        {
            if (_onDragEnd != null)
            {
                _onDragEnd();
            }
        }

        public void OnDrag(PointerEventData data)
        {
            if (_onDrag != null)
            {
                _onDrag(data.delta);
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

        public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
        where T : IEventSystemHandler
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);
            GameObject current = data.pointerCurrentRaycast.gameObject;
            for (int i = 0; i < results.Count; i++)
            {
                if (current != results[i].gameObject)
                {
                    ExecuteEvents.Execute(results[i].gameObject, data, function);
                    break;
                    //RaycastAll后ugui会自己排序，如果你只想响应透下去的最近的一个响应，这里ExecuteEvents.Execute后直接break就行。
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
