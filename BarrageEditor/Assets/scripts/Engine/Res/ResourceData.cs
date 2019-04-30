using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YKEngine
{
    public class ResourceData
    {
        public string packName;
        public object datasDic;

        public void Init(string[] datas)
        {
            packName = datas[0];
        }

        public object GetObject(string id)
        {
            if (datasDic != null)
            {
                Object obj;
                Dictionary<string, Object> resMap = (Dictionary<string, Object>)datasDic;
                if (resMap.TryGetValue(id, out obj))
                {
                    return obj;
                }
            }
            return null;
        }

        public void AddObject(string key, Object go)
        {
            Dictionary<string, Object> resMap = (Dictionary<string, Object>)datasDic;
            if (resMap != null)
            {
                try
                {
                    resMap.Add(key, go);
                }
                catch (System.Exception e)
                {
                    Logger.Log(e.Message);
                }
            }
        }
    }
}
