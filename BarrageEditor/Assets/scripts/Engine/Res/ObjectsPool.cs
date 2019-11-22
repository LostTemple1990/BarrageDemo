using UnityEngine;
using System.Collections.Generic;

namespace YKEngine
{
    internal class ObjectsPool
    {
        private static ObjectsPool _instance;

        public static ObjectsPool GetInstance()
        {
            if (_instance == null)
                _instance = new ObjectsPool();
            return _instance;
        }

        /// <summary>
        /// 预制体对象池
        /// </summary>
        private Dictionary<string, Stack<GameObject>> _prefabsPool;
        private Transform _poolTf;

        private ObjectsPool()
        {

        }

        public void Init()
        {
            GameObject poolGo = new GameObject();
            poolGo.name = "ObjectsPool";
            _poolTf = poolGo.transform;

            _prefabsPool = new Dictionary<string, Stack<GameObject>>();
        }

        public GameObject GetGameObjectAtPool(string name)
        {
            Stack<GameObject> stack;
            if (_prefabsPool.TryGetValue(name, out stack))
            {
                if (stack.Count > 0)
                    return stack.Pop();
            }
            return null;
        }

        public void RestoreGameObjectToPool(string name,GameObject go)
        {
            Stack<GameObject> stack;
            if (!_prefabsPool.TryGetValue(name, out stack))
            {
                stack = new Stack<GameObject>();
                _prefabsPool.Add(name, stack);
                //Logger.Log("Create new pool stack,name = " + name);
            }
            go.SetActive(false);
            go.transform.SetParent(_poolTf, false);
            stack.Push(go);
        }
    }
}
