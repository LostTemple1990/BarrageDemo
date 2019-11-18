using System.Xml;

public class EnemyCfg : IParser
{
    public string id;
    public string name;
    /// <summary>
    /// Enemy类别
    /// </summary>
    public eEnemyObjectType type;
    /// <summary>
    /// sprite数据所在的贴图名称
    /// </summary>
    public string aniId;
    /// <summary>
    /// 被击破时的特效类型
    /// </summary>
    public int eliminatedEffectStyle;
    public float collisionHalfWidth;
    public float collisionHalfHeight;
    public float dropHalfWidth;
    public float dropHalfHeight;

    public IParser CreateNewInstance()
    {
        return new EnemyCfg();
    }

    public void parse(XmlElement xmlElement)
    {
        id = xmlElement.GetAttribute("id");
        name = xmlElement.GetAttribute("name");
        type = (eEnemyObjectType)int.Parse(xmlElement.GetAttribute("type"));
        aniId = xmlElement.GetAttribute("aniId");
        eliminatedEffectStyle = int.Parse(xmlElement.GetAttribute("eliminateEffectStyle"));
        collisionHalfWidth = float.Parse(xmlElement.GetAttribute("collHW"));
        collisionHalfHeight = float.Parse(xmlElement.GetAttribute("collHH"));
        dropHalfWidth = float.Parse(xmlElement.GetAttribute("dropHW"));
        dropHalfHeight = float.Parse(xmlElement.GetAttribute("dropHH"));
    }
}
