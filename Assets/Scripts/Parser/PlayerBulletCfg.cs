using System.Xml;
using UnityEngine;

public class PlayerBulletCfg : IParser
{
    public string id;
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
    public string eliminateEffectAtlas;
    public string elminaateEffectSprite;
    public Color eliminateColor;
    /// <summary>
    /// 被消除时需要播放的消除特效类型
    /// </summary>
    public int eliminatedEffectType;
    /// <summary>
    /// 消除特效的参数
    /// </summary>
    public PlayerBulletHitEffectParas hitEffectParas;

    public IParser CreateNewInstance()
    {
        return new PlayerBulletCfg();
    }

    public void parse(XmlElement xmlElement)
    {
        id = xmlElement.GetAttribute("id");
        packName = xmlElement.GetAttribute("packName");
        textureName = xmlElement.GetAttribute("texName");
        isRotatedByVAngle = int.Parse(xmlElement.GetAttribute("isRotatedByVAngle")) == 1 ? true : false;
        selfRotationAngle = float.Parse(xmlElement.GetAttribute("selfRotationAngle"));
        collisionRadius = float.Parse(xmlElement.GetAttribute("collisionRadius"));
        string str = xmlElement.GetAttribute("eliminateSprite");
        if ( str != null && str != "" )
        {
            string[] eliminateStrs = xmlElement.GetAttribute("eliminateSprite").Split(',');
            eliminateEffectAtlas = eliminateStrs[0];
            elminaateEffectSprite = eliminateStrs[1];
            string[] colorStrs = (xmlElement.GetAttribute("eliminateColor")).Split(',');
            eliminateColor = new Color(float.Parse(colorStrs[0]), float.Parse(colorStrs[1]), float.Parse(colorStrs[2]));
        }
        eliminatedEffectType = int.Parse(xmlElement.GetAttribute("eliminatedEffectType"));
        hitEffectParas = new PlayerBulletHitEffectParas();
        hitEffectParas.Init(xmlElement.GetAttribute("hitEffectParas"));
    }
}
