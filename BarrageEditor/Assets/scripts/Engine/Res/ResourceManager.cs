using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace YKEngine
{
    public class ResourceManager
    {
        private static ResourceManager _instance;

        public static ResourceManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ResourceManager();
            }
            return _instance;
        }

        private ResourceManager()
        {

        }

        private Dictionary<string, ResourceData> _resourceDataDic;
        private Material _spriteDefaultMat;
        /// <summary>
        /// 图集的字典
        /// </summary>
        private Dictionary<string, SpriteAtlas> _atlasDic;
        /// <summary>
        /// sprite的不同混合模式的材质
        /// </summary>
        private List<Material> _spriteMatList;
        /// <summary>
        /// 储存对象原型的字典
        /// </summary>
        private Dictionary<string, GameObject> _protoDic;

        public void Init()
        {
            _resourceDataDic = new Dictionary<string, ResourceData>();
            _atlasDic = new Dictionary<string, SpriteAtlas>();
            ParseSpriteAtlas("STGReimuAtlas");
            ParseSpriteAtlas("STGEnemyAtlas0");
            ParseSpriteAtlas("STGNazrinAtlas");
            ParseResource("etama9", 16);
            // sprite的材质
            _spriteMatList = new List<Material>();
            _spriteMatList.Add(Resources.Load<Material>("materials/SpriteDefaultMat"));
            _spriteMatList.Add(Resources.Load<Material>("materials/SpriteSoftAdditiveMat"));
            _protoDic = new Dictionary<string, GameObject>();
        }

        /// <summary>
        /// 根据配置解析资源，临时使用resource.load
        /// </summary>
        private void ParseResource(string packName, int count)
        {
            ResourceData resData = new ResourceData();
            resData.Init(new string[] { packName });
            Dictionary<string, Object> resMap = new Dictionary<string, Object>();
            Texture2D texture;
            Sprite sp;
            for (int i = 0; i < count; i++)
            {
                texture = Resources.Load(packName + "/" + packName + "_" + i) as Texture2D;
                if (texture != null)
                {
                    sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    resMap.Add(packName + "_" + i, sp);
                }
            }
            resData.datasDic = resMap;
            _resourceDataDic.Add(packName, resData);
        }

        /// <summary>
        /// 解析一整个atlas
        /// </summary>
        /// <param name="atlasName"></param>
        public void ParseSpriteAtlas(string atlasName)
        {
            ResourceData resData = new ResourceData();
            resData.Init(new string[] { atlasName });
            Dictionary<string, Object> resMap = new Dictionary<string, Object>();
            resData.datasDic = resMap;
            _resourceDataDic.Add(atlasName, resData);

            string path = "SpriteAtlas/" + atlasName;
            SpriteAtlas atlas = Resources.Load<SpriteAtlas>(path);
            if (atlas != null)
            {
                int len = atlas.spriteCount;
                Sprite[] sprites = new Sprite[len];
                atlas.GetSprites(sprites);
                for (int i = 0; i < len; i++)
                {
                    resMap.Add(sprites[i].name, sprites[i]);
                }
            }
        }

        /// <summary>
        /// 从图集中获得sprite
        /// </summary>
        /// <param name="atlasName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        public Sprite GetSprite(string atlasName, string resName)
        {
            ResourceData resourceData;
            if (!_resourceDataDic.TryGetValue(atlasName, out resourceData))
            {
                resourceData = new ResourceData();
                resourceData.Init(new string[] { atlasName });
                _resourceDataDic.Add(atlasName, resourceData);
                Dictionary<string, Object> resMap = new Dictionary<string, Object>();
                resourceData.datasDic = resMap;
            }
            Sprite sp = resourceData.GetObject(resName) as Sprite;
            // 若当前缓存中不存在，则从图集中创建一份
            if (sp == null)
            {
                sp = CreateSpriteFromAtlas(atlasName, resName);
                if (sp != null)
                {
                    Dictionary<string, Object> resMap = resourceData.datasDic as Dictionary<string, Object>;
                    resMap.Add(resName, sp);
                }
            }
            return sp;
        }

        /// <summary>
        /// 从图集中创建sprite的一份拷贝
        /// </summary>
        /// <param name="packName"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        public Sprite CreateSpriteFromAtlas(string packName, string resName)
        {
            SpriteAtlas atlas = GetSpriteAtlas(packName);
            Sprite sp = null;
            if (atlas != null)
            {
                sp = atlas.GetSprite(resName);
            }
            return sp;
        }

        /// <summary>
        /// 根据图集名称获取图集
        /// <para>图集暂时存放在Resources/SpriteAtlas下</para>
        /// </summary>
        /// <param name="atlasName">图集名称</param>
        /// <returns></returns>
        public SpriteAtlas GetSpriteAtlas(string atlasName)
        {
            SpriteAtlas atlas;
            if (!_atlasDic.TryGetValue(atlasName, out atlas))
            {
                atlas = Resources.Load<SpriteAtlas>("SpriteAtlas/" + atlasName);
                if (atlas != null)
                {
                    _atlasDic.Add(atlasName, atlas);
                }
            }
            return atlas;
        }

        public T GetResource<T>(string packName, string resName)
        {
            ResourceData resData;
            if (_resourceDataDic.TryGetValue(packName, out resData))
            {
                return (T)resData.GetObject(resName);
            }
            return default(T);
        }

        public GameObject GetPrefab(string packName, string resName)
        {
            GameObject proto;
            if (!_protoDic.TryGetValue("resName", out proto))
            {
                proto = Resources.Load<GameObject>(packName + "/" + resName);
                if ( proto == null )
                {
                    Logger.LogError("Create Proto Fail!Path " + packName + "/" + resName + "is not exist!");
                }
                _protoDic.Add(resName, proto);
                proto.SetActive(false);
            }
            GameObject prefab = GameObject.Instantiate(proto) as GameObject;
            prefab.SetActive(true);
            return prefab;
        }

        public GameObject GetPrefab(string packName, string resName, Transform parentTf, bool instantiateInWorldSpace)
        {
            GameObject proto;
            if (!_protoDic.TryGetValue(resName, out proto))
            {
                proto = Resources.Load<GameObject>(packName + "/" + resName);
                _protoDic.Add(resName, proto);
                proto.SetActive(false);
            }
            GameObject prefab = GameObject.Instantiate(proto, parentTf, instantiateInWorldSpace) as GameObject;
            prefab.SetActive(true);
            return prefab;
        }

        /// <summary>
        /// 获取精灵默认的材质
        /// </summary>
        /// <returns></returns>
        public Material GetSpriteDefualtMaterial()
        {
            if (_spriteDefaultMat == null)
            {
                _spriteDefaultMat = Resources.Load<Material>("materials/SpriteDefaultMat");
            }
            return _spriteDefaultMat;
        }

        /// <summary>
        /// 根据混合模式获取对应的精灵的材质
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Material GetSpriteMatByBlendMode(eBlendMode mode)
        {
            int index = (int)mode;
            return _spriteMatList[index];
        }

        /// <summary>
        /// 检测是否已经有对应名称的资源数据
        /// </summary>
        /// <param name="resName"></param>
        /// <returns></returns>
        public bool CheckResourceData(string resName)
        {
            ResourceData data;
            if (!_resourceDataDic.TryGetValue(resName, out data))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据resName拿到ResourceData
        /// <para>若没有，则创建一个新的</para>
        /// </summary>
        /// <param name="resName"></param>
        /// <returns></returns>
        private ResourceData GetOrCreateResourceData(string resName)
        {
            ResourceData data;
            if (!_resourceDataDic.TryGetValue(resName, out data))
            {
                data = new ResourceData();
                data.Init(new string[] { resName });
                data.datasDic = new Dictionary<string, Object>(); ;
            }
            return data;
        }
    }
}
