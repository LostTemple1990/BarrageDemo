using System.Xml;

public class EnemyBulletDefaultCfg : IParser
{
    public string id;
    public string prefabName;
    public bool isRotatedByVAngle;
    public float selfRotationAngle;
    public float grazeHalfWidth;
    public float grazeHalfHeight;
    public float collisionRadius;

    public IParser CreateNewInstance()
    {
        return new EnemyBulletDefaultCfg();
    }

    public void parse(XmlElement xmlElement)
    {
        id = xmlElement.GetAttribute("id");
        prefabName = xmlElement.GetAttribute("prefabName");
        isRotatedByVAngle = int.Parse(xmlElement.GetAttribute("isRotatedByVAngle")) == 1 ? true : false;
        selfRotationAngle = float.Parse(xmlElement.GetAttribute("selfRotationAngle"));
        grazeHalfWidth = float.Parse(xmlElement.GetAttribute("grazeHalfWidth"));
        grazeHalfHeight = float.Parse(xmlElement.GetAttribute("grazeHalfHeight"));
        collisionRadius = float.Parse(xmlElement.GetAttribute("collisionRadius"));
    }
}
