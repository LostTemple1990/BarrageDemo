using System.Xml;
using UnityEngine;

public class EnemyBulletDefaultCfg : IParser
{
    public int id;
    public string textureName;
    public bool isRotatedByVAngle;
    public float selfRotationAngle;
    public float grazeHalfWidth;
    public float grazeHalfHeight;
    public float collisionRadius;
    public Color eliminateColor;

    public IParser CreateNewInstance()
    {
        return new EnemyBulletDefaultCfg();
    }

    public void parse(XmlElement xmlElement)
    {
        id = int.Parse(xmlElement.GetAttribute("id"));
        textureName = xmlElement.GetAttribute("textureName");
        isRotatedByVAngle = int.Parse(xmlElement.GetAttribute("isRotatedByVAngle")) == 1 ? true : false;
        selfRotationAngle = float.Parse(xmlElement.GetAttribute("selfRotationAngle"));
        grazeHalfWidth = float.Parse(xmlElement.GetAttribute("grazeHalfWidth"));
        grazeHalfHeight = float.Parse(xmlElement.GetAttribute("grazeHalfHeight"));
        collisionRadius = float.Parse(xmlElement.GetAttribute("collisionRadius"));
        string[] colorStrs = (xmlElement.GetAttribute("eliminateColor")).Split(',');
        eliminateColor = new Color(float.Parse(colorStrs[0]), float.Parse(colorStrs[1]), float.Parse(colorStrs[2]));
    }
}
