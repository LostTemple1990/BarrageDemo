using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System;

/// <summary>
/// 从外部文件取得数据的类
/// </summary>
public class DataManager
{
    private static DataManager _instance;

    public static DataManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new DataManager();
        }
        return _instance;
    }


    private Dictionary<string, object> _datasMap;
    /// <summary>
    /// <para>key - xml名称</para>
    /// <para>value - 对应解析类的名称</para>
    /// </summary>
    private Dictionary<string, string> _parserMap;
    private string CfgFolderPath;

    public void Init()
    {
        _parserMap = new Dictionary<string, string>();
        _parserMap.Add("AnimationCfgs", "UnitAniCfg");
        _parserMap.Add("EnemyCfgs", "EnemyCfg");
        _parserMap.Add("EnemyBulletDefaultCfgs", "EnemyBulletDefaultCfg");
        _parserMap.Add("DropItemsCfgs", "DropItemsCfg");
        _parserMap.Add("PlayerBulletCfgs", "PlayerBulletCfg");
        _datasMap = new Dictionary<string, object>();
        CfgFolderPath = Path.Combine(Application.streamingAssetsPath, "Configs");
    }

    public object GetDatasByName(string name)
    {
        object datas;
        if (!_datasMap.TryGetValue(name, out datas))
        {
            // 解析xml
            datas = ParseXML(name);
        }
        return datas;
    }

    private object ParseXML(string name)
    {
        string filepath = Path.Combine(CfgFolderPath, name + ".xml");
        XmlDocument xmlDoc = new XmlDocument();
        // 设置忽视XML注释
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;
        XmlReader reader = XmlReader.Create(filepath, settings);
        xmlDoc.Load(reader);
        XmlNode xmlNode = xmlDoc.SelectSingleNode("Data");
        XmlNodeList nodeList = xmlNode.ChildNodes;
        string parserName;
        if (this._parserMap.TryGetValue(name, out parserName))
        {
            Type type = Type.GetType(parserName);
            IParser parser = (IParser)type.Assembly.CreateInstance(type.Name);
            IParser newParser;
            Dictionary<string, IParser> dic = new Dictionary<string, IParser>();
            foreach (XmlElement xe in nodeList)
            {
                newParser = parser.CreateNewInstance();
                newParser.parse(xe);
                dic.Add(xe.GetAttribute("id"), newParser);
            }
            this._datasMap.Add(name, dic);
            return dic;
        }
        reader.Close();
        return null;
    }
}