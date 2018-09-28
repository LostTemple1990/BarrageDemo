﻿using System.Xml;
using UnityEngine;

public class PlayerBulletCfg : IParser
{
    public int id;
    /// <summary>
    /// 贴图所在包名
    /// </summary>
    public string packName;
    /// <summary>
    /// 子弹贴图名称
    /// </summary>
    public string textureName;
    public bool isRotatedByVAngle;
    public float selfRotationAngle;
    public float collisionRadius;
    //public Color eliminateColor;

    public IParser CreateNewInstance()
    {
        return new PlayerBulletCfg();
    }

    public void parse(XmlElement xmlElement)
    {
        id = int.Parse(xmlElement.GetAttribute("id"));
        packName = xmlElement.GetAttribute("packName");
        textureName = xmlElement.GetAttribute("texName");
        isRotatedByVAngle = int.Parse(xmlElement.GetAttribute("isRotatedByVAngle")) == 1 ? true : false;
        selfRotationAngle = float.Parse(xmlElement.GetAttribute("selfRotationAngle"));
        collisionRadius = float.Parse(xmlElement.GetAttribute("collisionRadius"));
        //string[] colorStrs = (xmlElement.GetAttribute("eliminateColor")).Split(',');
        //eliminateColor = new Color(float.Parse(colorStrs[0]), float.Parse(colorStrs[1]), float.Parse(colorStrs[2]));
    }
}