using System;
using System.Collections.Generic;

namespace BarrageEditor
{
    public class CustomDefine
    {
        private static Dictionary<CustomDefineType, Dictionary<string, CustomDefineData>> _customDefineDic;

        private static Dictionary<CustomDefineType, Dictionary<string, CustomDefineData>> customDefineDic
        {
            get
            {
                if ( _customDefineDic == null )
                {
                    _customDefineDic = new Dictionary<CustomDefineType, Dictionary<string, CustomDefineData>>();
                    _customDefineDic.Add(CustomDefineType.SimpleBullet, new Dictionary<string, CustomDefineData>());
                    _customDefineDic.Add(CustomDefineType.Laser, new Dictionary<string, CustomDefineData>());
                    _customDefineDic.Add(CustomDefineType.LinearLaser, new Dictionary<string, CustomDefineData>());
                    _customDefineDic.Add(CustomDefineType.CurveLaser, new Dictionary<string, CustomDefineData>());
                    _customDefineDic.Add(CustomDefineType.Enemy, new Dictionary<string, CustomDefineData>());
                    _customDefineDic.Add(CustomDefineType.Boss, new Dictionary<string, CustomDefineData>());
                }
                return _customDefineDic;
            }
        }

        public static List<string> GetCustomDefineListByType(CustomDefineType type)
        {
            List<string> list = new List<string>();
            Dictionary<string, CustomDefineData> dic = customDefineDic[type];
            foreach (var v in dic.Values)
            {
                list.Add(v.typeName);
            }
            return list;
        }

        /// <summary>
        /// 获取指定的自定义配置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static CustomDefineData GetDataByTypeAndName(CustomDefineType type,string typeName)
        {
            Dictionary<string, CustomDefineData> dic = customDefineDic[type];
            CustomDefineData data = null;
            dic.TryGetValue(typeName, out data);
            return data;
        }

        public static void AddData(CustomDefineType type, string typeName, string paraList)
        {
            if (typeName == "") return;
            CustomDefineData data = new CustomDefineData
            {
                type = type,
                typeName = typeName,
                paraListStr = paraList,
            };
            Dictionary<string, CustomDefineData> dic = customDefineDic[type];
            dic.Add(typeName, data);
        }

        public static void RemoveData(CustomDefineType type, string typeName)
        {
            Dictionary<string, CustomDefineData> dic = customDefineDic[type];
            dic.Remove(typeName);
        }

        public static bool IsExist(CustomDefineType type, string typeName)
        {
            Dictionary<string, CustomDefineData> dic = customDefineDic[type];
            CustomDefineData data;
            return dic.TryGetValue(typeName, out data);
        }

        public static void ModifyDefineName(CustomDefineType type, string fromName,string toName)
        {
            Dictionary<string, CustomDefineData> dic = customDefineDic[type];
            CustomDefineData data;
            if ( dic.TryGetValue(fromName, out data) )
            {
                data.typeName = toName;
                dic.Remove(fromName);
                if ( toName != "" )
                {
                    dic.Add(toName, data);
                }
            }
        }

        public static void ModifyDefineParaList(CustomDefineType type,string name,string paraList)
        {
            Dictionary<string, CustomDefineData> dic = customDefineDic[type];
            CustomDefineData data;
            if (dic.TryGetValue(name, out data))
            {
                data.paraListStr = paraList;
            }
            else
            {
                Logger.Log("CustomType of " + type + " with name " + name + " not found!");
            }
        }

        /// <summary>
        /// 根据节点的类型来获得对应的自定义类型
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public static CustomDefineType GetTypeByNodeType(NodeType nodeType)
        {
            if (nodeType == NodeType.CreateBullet) return CustomDefineType.SimpleBullet;
            return CustomDefineType.Null;
        }

        public static void Clear()
        {
            customDefineDic[CustomDefineType.SimpleBullet].Clear();
            customDefineDic[CustomDefineType.Laser].Clear();
            customDefineDic[CustomDefineType.LinearLaser].Clear();
            customDefineDic[CustomDefineType.CurveLaser].Clear();
            customDefineDic[CustomDefineType.Enemy].Clear();
            customDefineDic[CustomDefineType.Boss].Clear();
        }
    }

    public class CustomDefineData
    {
        public CustomDefineType type;
        public string typeName;
        public string paraListStr;
    }
    
    public enum CustomDefineType
    {
        Null = 0,
        SimpleBullet = 1,
        Laser = 2,
        LinearLaser = 3,
        CurveLaser = 4,
        Enemy = 5,
        Boss = 6,
    }
}
