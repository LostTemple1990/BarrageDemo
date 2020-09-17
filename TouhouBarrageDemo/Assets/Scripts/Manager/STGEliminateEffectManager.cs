using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STGEliminateEffectManager
{
    private const int DefaultSize = 500;
    private const int ExtraSize = 500;

    private const int StateAppearance = 1;
    private const int StateFade = 2;
    private const int StateFinish = 3;

    private const int AppearnaceDuration = 20;
    private const int FadeDuration = 30;

    private const float AppearanceStartScale = 0.5f;
    private const float AppearanceEndScale = 1.2f;

    private const float BEEHalfWidth = 16;
    private const float BEEHalfHeight = 16;

    private static STGEliminateEffectManager _instance;

    public static STGEliminateEffectManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new STGEliminateEffectManager();
        }
        return _instance;
    }

    enum BEEState : byte
    {
        StateAppearance,
        StateFade,
        StateFinish,
    }

    /// <summary>
    /// BulletEliminateEffectData
    /// </summary>
    struct BEEData
    {
        /// <summary>
        /// time
        /// </summary>
        public int time;
        public int totalTime;
        /// <summary>
        /// 当前是否激活，未激活的数据会被覆盖
        /// </summary>
        public bool isActive;
        public BEEState state;
        public Vector3 scale;
        public Vector2 pos;
        public Vector3 eulerAngle;
        public Color color;
    }

    private STGEliminateEffectManager()
    {
        _bulletEffList = new BEEData[DefaultSize];
        _bulletEffMaxSize = DefaultSize;
        _bulletEffCount = 0;
    }

    public void Init()
    {
        _beeGo = ResourceManager.GetInstance().GetPrefab("Prefab/Extra", "BulletEliminateEffectsGo");
        _beeMesh = _beeGo.GetComponent<MeshFilter>().mesh;
        UIManager.GetInstance().AddGoToLayer(_beeGo, LayerId.STGNormalEffect);
        _beVerList = new List<Vector3>();
        _beUVList = new List<Vector2>();
        _beTriList = new List<int>();
        _beColorList = new List<Color>();
    }

    public void Render()
    {
        RenderBulletEliminateEffects();
    }

    private GameObject _beeGo;
    private Mesh _beeMesh;

    private BEEData[] _bulletEffList;
    private int _bulletEffMaxSize;
    private int _bulletEffCount;
    /// <summary>
    /// 子弹消除特效顶点list
    /// </summary>
    private List<Vector3> _beVerList;
    /// <summary>
    /// 子弹消除特效uvlist
    /// </summary>
    private List<Vector2> _beUVList;
    /// <summary>
    /// 子弹消除特效三角形索引list
    /// </summary>
    private List<int> _beTriList;
    /// <summary>
    /// 子弹消除特效颜色list
    /// </summary>
    private List<Color> _beColorList;

    private void RenderBulletEliminateEffects()
    {
        _beeMesh.Clear();
        _beVerList.Clear();
        _beUVList.Clear();
        _beTriList.Clear();
        _beColorList.Clear();
        //BEEData tmpData;
        int i, j;
        bool flag;
        //int verIndex = 0;
        for (i = 0, j = 1; i < _bulletEffCount; i++)
        {
            flag = false;
            if (_bulletEffList[i].isActive)
            {
                flag = true;
            }
            else
            {
                for ( ; j < _bulletEffCount; j++)
                {
                    if (_bulletEffList[j].isActive)
                    {
                        _bulletEffList[i] = _bulletEffList[j];
                        _bulletEffList[j].isActive = false;
                        flag = true;
                        j++;
                        break;
                    }
                }
            }
            if (flag)
            {
                // update and render
                _bulletEffList[i] = UpdateBEE(_bulletEffList[i]);
                RenderBEE(_bulletEffList[i], i * 4);
            }
            else
            {
                break;
            }
        }
        _bulletEffCount = i;
        _beeMesh.SetVertices(_beVerList);
        _beeMesh.uv = _beUVList.ToArray();
        _beeMesh.triangles = _beTriList.ToArray();
        _beeMesh.colors = _beColorList.ToArray();
    }

    public void CreateBulletEliminateEffect(Vector2 pos,Color col)
    {
        // 判断是否需要扩充list
        if (_bulletEffCount >= _bulletEffMaxSize)
        {
            int newMaxSize = _bulletEffMaxSize + ExtraSize;
            BEEData[] newList = new BEEData[newMaxSize];
            for (int i = 0; i < _bulletEffCount; i++)
            {
                newList[i] = _bulletEffList[i];
            }
            _bulletEffList = newList;
            _bulletEffMaxSize = newMaxSize;
        }
        _bulletEffList[_bulletEffCount++] = new BEEData
        {
            pos = pos,
            //color = new Color(col.r, col.g, col.b, 1f),
            color = col,
            state = BEEState.StateAppearance,
            scale = new Vector3(AppearanceStartScale, AppearanceStartScale, 1),
            time = 0,
            isActive = true,
        };
    }

    private BEEData UpdateBEE(BEEData data)
    {
        if (data.state == BEEState.StateAppearance)
        {
            data.time++;
            float scale = MathUtil.GetEaseInQuadInterpolation(AppearanceStartScale, AppearanceEndScale, data.time, AppearnaceDuration);
            data.scale = new Vector3(scale, scale, 1);
            if (data.time >= AppearnaceDuration)
            {
                data.state = BEEState.StateFade;
                data.time = 0;
            }
        }
        else if (data.state ==  BEEState.StateFade)
        {
            data.time++;
            float a = 1f - (float)data.time / FadeDuration;
            Color col = data.color;
            data.color = new Color(col.r, col.g, col.b, a);
            if (data.time >= FadeDuration)
            {
                data.isActive = false;
            }
        }
        return data;
    }

    private void RenderBEE(BEEData data,int verIndex)
    {
        BuildBEEMesh(data.pos, data.scale.x * BEEHalfWidth, data.scale.y * BEEHalfHeight, data.color, ref verIndex);
    }
    
    private void BuildBEEMesh(Vector2 centerPos,float halfWidth,float halfHeight,Color col,ref int verIndex)
    {
        _beVerList.Add(new Vector3(centerPos.x - halfWidth, centerPos.y + halfHeight));
        _beVerList.Add(new Vector3(centerPos.x - halfWidth, centerPos.y - halfHeight));
        _beVerList.Add(new Vector3(centerPos.x + halfWidth, centerPos.y + halfHeight));
        _beVerList.Add(new Vector3(centerPos.x + halfWidth, centerPos.y - halfHeight));
        _beUVList.Add(new Vector2(0, 1));
        _beUVList.Add(new Vector2(0, 0));
        _beUVList.Add(new Vector2(1, 1));
        _beUVList.Add(new Vector2(1, 0));
        _beTriList.Add(verIndex);
        _beTriList.Add(verIndex + 3);
        _beTriList.Add(verIndex + 1);
        _beTriList.Add(verIndex);
        _beTriList.Add(verIndex + 2);
        _beTriList.Add(verIndex + 3);
        for (int i = 0; i < 4; i++)
        {
            _beColorList.Add(col);
        }
        verIndex += 4;
    }

    public void Clear()
    {
        _beeMesh.Clear();
        _beVerList.Clear();
        _beUVList.Clear();
        _beTriList.Clear();
        _beColorList.Clear();
    }

    public void Destroy()
    {
        Clear();
        GameObject.Destroy(_beeGo);
        _beeGo = null;
        _beeMesh = null;
    }
}
