using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class UnitAniCfg : IParser
{
    public string id;
    public string aniName;
    /// <summary>
    /// sprite数据所在的贴图名称
    /// </summary>
    public string textureName;
    public bool isFlapX;
    public string aniData;
    public Dictionary<string, int> aniIntervalMap;

    public IParser CreateNewInstance()
    {
        return new UnitAniCfg();
    }

    public void parse(XmlElement xmlElement)
    {
        id = xmlElement.GetAttribute("id");
        aniName = xmlElement.GetAttribute("aniName");
        textureName = xmlElement.GetAttribute("textureName");
        aniData = xmlElement.GetAttribute("aniData");
        isFlapX = int.Parse(xmlElement.GetAttribute("isFlapX")) == 0 ? false : true;
        // 根据配置读取ani间隔
        aniIntervalMap = new Dictionary<string, int>();
        string[] sliceStrs = aniData.Split(',');
        int len = sliceStrs.Length;
        int i=0;
        while ( i < len )
        {
            string actName = sliceStrs[i];
            int interval = int.Parse(sliceStrs[i + 2]);
            aniIntervalMap.Add(actName, interval);
            i = i + 3;
        }
    }
}
