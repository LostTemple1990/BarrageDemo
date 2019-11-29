using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class StageBgCfg : IParser
{
    public string id;
    /// <summary>
    /// 场景名称
    /// </summary>
    public string sceneName;
    /// <summary>
    /// 类名
    /// </summary>
    public string className;

    public IParser CreateNewInstance()
    {
        return new StageBgCfg();
    }

    public void parse(XmlElement xmlElement)
    {
        id = xmlElement.GetAttribute("id");
        sceneName = xmlElement.GetAttribute("sceneName");
        className = xmlElement.GetAttribute("className");
    }
}
