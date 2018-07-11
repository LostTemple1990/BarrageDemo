using System.Xml;

public class EnemyCfg : IParser
{
    public string id;
    public string name;
    /// <summary>
    /// Enemy类别
    /// </summary>
    public EnemyObjectType type;
    /// <summary>
    /// sprite数据所在的贴图名称
    /// </summary>
    public string aniId;
    public int maxHp;
    public float collisionHalfWidth;
    public float collisionHalfHeight;
    public string dropId;
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
        type = (EnemyObjectType)int.Parse(xmlElement.GetAttribute("type"));
        aniId = xmlElement.GetAttribute("aniId");
        maxHp = int.Parse(xmlElement.GetAttribute("maxHp"));
        collisionHalfWidth = float.Parse(xmlElement.GetAttribute("collHW"));
        collisionHalfHeight = float.Parse(xmlElement.GetAttribute("collHH"));
        dropId = xmlElement.GetAttribute("dropId");
        dropHalfWidth = float.Parse(xmlElement.GetAttribute("dropHW"));
        dropHalfHeight = float.Parse(xmlElement.GetAttribute("dropHH"));
    }
}
