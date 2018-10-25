using System.Xml;
using UnityEngine;

public class EnemyLinearLaserCfg : IParser
{
    public string id;
    public string laserTexName;
    public eBlendMode blendMode;
    public string laserHeadTexName;
    public string laserSourceTexId;
    public Color eliminateColor;

    public IParser CreateNewInstance()
    {
        return new EnemyLinearLaserCfg();
    }

    public void parse(XmlElement xmlElement)
    {
        id = xmlElement.GetAttribute("id");
        laserTexName = xmlElement.GetAttribute("laserTexName");
        blendMode = (eBlendMode)int.Parse(xmlElement.GetAttribute("materialType"));
        laserHeadTexName = xmlElement.GetAttribute("laserHeadTexName");
        laserSourceTexId = xmlElement.GetAttribute("laserSourceTexId");
        string[] colorStrs = (xmlElement.GetAttribute("eliminateColor")).Split(',');
        eliminateColor = new Color(float.Parse(colorStrs[0]), float.Parse(colorStrs[1]), float.Parse(colorStrs[2]));
    }
}
