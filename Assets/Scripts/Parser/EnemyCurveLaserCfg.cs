using System.Xml;
using UnityEngine;

public class EnemyCurveLaserCfg : IParser
{
    public string id;
    /// <summary>
    /// 所用的材质名称
    /// </summary>
    public string materialName;
    public int colorIndex;
    public int colorsCount;
    public string laserHeadTexName;
    public string laserSourceTexId;
    public Color eliminateColor;

    public IParser CreateNewInstance()
    {
        return new EnemyCurveLaserCfg();
    }

    public void parse(XmlElement xmlElement)
    {
        id = xmlElement.GetAttribute("id");
        materialName = xmlElement.GetAttribute("materialName");
        colorIndex = int.Parse(xmlElement.GetAttribute("colorIndex"));
        colorsCount = int.Parse(xmlElement.GetAttribute("colorsCount"));
        laserHeadTexName = xmlElement.GetAttribute("laserHeadTexName");
        laserSourceTexId = xmlElement.GetAttribute("laserSourceTexId");
        string[] colorStrs = (xmlElement.GetAttribute("eliminateColor")).Split(',');
        eliminateColor = new Color(float.Parse(colorStrs[0]), float.Parse(colorStrs[1]), float.Parse(colorStrs[2]));
    }
}
