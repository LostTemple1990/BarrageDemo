using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class STGBreakScreenEffect : IEffect
{
    private static int WidthSegment = 8;
    private static int HeightSegment = 8;

    private static int StateWaitCaptureScreen = 1;
    private static int StateCreateFragments = 2;
    private static int StateFragmentsBeginFall = 3;
    private static int StateFragmentsFalling = 4;
    private static int StateFragmentsFallingFinish = 5;
    /// <summary>
    /// 碎片停止运动的Y坐标
    /// </summary>
    private static float FragFinishPosY = -Consts.GameHeight / 2 - 100;

    private bool _isFinish;
    /// <summary>
    /// 是否截图完成
    /// </summary>
    private bool _isCaptureComplete;

    private Texture2D _screenTexture;

    private Material _fragMat;

    private List<Vector3> _vertexList;
    private List<Vector2> _uvList;
    /// <summary>
    /// 碎片的位置信息列表
    /// </summary>
    private List<FragmentData> _fragList;
    private int _fragCount;

    private int _curState;

    public void Init()
    {
        _isFinish = false;
        _isCaptureComplete = false;
        if (_fragMat == null)
        {
            _fragMat = new Material(ResourceManager.GetInstance().GetSpriteDefualtMaterial());
        }
        if ( _fragList == null )
        {
            _fragList = new List<FragmentData>();
        }
        _fragCount = 0;
        _curState = StateWaitCaptureScreen;
        //StartCoroutine(CaptureSTGScreen());
        CaptureSTGScreen();
    }

    public void CaptureSTGScreen()
    {
        //yield return new WaitForEndOfFrame();
        Rect rect = new Rect(0,0,Global.STGActualSize.x,Global.STGActualSize.y);
        RenderTexture renderTexture = new RenderTexture((int)rect.width, (int)rect.height, 1);
        Camera stgCamera = UIManager.GetInstance().GetSTGCamera();
        stgCamera.targetTexture = renderTexture;
        stgCamera.Render();
        RenderTexture.active = renderTexture;
        _screenTexture = new Texture2D((int)Global.STGActualSize.x,(int)Global.STGActualSize.y);
        _screenTexture.ReadPixels(rect,0,0);
        _screenTexture.Apply();
        stgCamera.targetTexture = null;
        RenderTexture.active = null;
        _isCaptureComplete = true;
    }

    private void CreateFragmentObjects()
    {
        CalVertices();
        // 设置材质贴图
        _fragMat.mainTexture = _screenTexture;
        // 从左下往右上遍历生成碎片
        GameObject fragObject;
        FragmentData fragData;
        int index0,index1,index2;
        int i, j;
        // 每行的顶点数目
        int vecCountPerLine = WidthSegment + 1;
        for (i=0;i<HeightSegment;i++)
        {
            for (j=0;j<WidthSegment;j++)
            {
                // 第一块碎片
                index0 = i * vecCountPerLine + j;
                index1 = (i + 1) * vecCountPerLine + j;
                index2 = i * vecCountPerLine + j + 1;
                fragObject = CreateFragmentObject(index0,index1,index2);
                fragData = new FragmentData
                {
                    fragTf = fragObject.transform
                };
                _fragList.Add(fragData);
                // 第二块碎片
                index0 = index2;
                index2 = (i + 1) * vecCountPerLine + j + 1;
                fragObject = CreateFragmentObject(index0, index1, index2);
                fragData = new FragmentData
                {
                    fragTf = fragObject.transform
                };
                _fragList.Add(fragData);
                _fragCount += 2;
            }
        }
        // 切换状态
        _curState = StateFragmentsBeginFall;
    }

    private void CalVertices()
    {
        List<Vector3> vertexList = new List<Vector3>();
        List<Vector2> uvList = new List<Vector2>();
        float widthRange = 0.5f / WidthSegment;
        float heightRange = 0.5f / HeightSegment;
        int i, j;
        //int tmpWidSeg, tmpHeightSeg;
        float baseX, baseY;
        // 边界判断判断用，限制Y的取值范围
        float rangeMinY, rangeMaxY;
        Vector2 uv;
        float stgWidth = Consts.GameWidth;
        float stgHeight = Consts.GameHeight;
        for (i = 0; i <= HeightSegment; i++)
        {
            // 处于上下边界的顶点不能随机取值
            if (i == 0 || i == HeightSegment)
            {
                rangeMinY = rangeMaxY = 0f;
            }
            else
            {
                rangeMinY = -1;
                rangeMaxY = 1;
            }
            baseX = 0;
            baseY = 1f * i / HeightSegment;
            // 放入当前行第一个
            uv = new Vector2(baseX, baseY + Random.Range(rangeMinY, rangeMaxY) * heightRange);
            uvList.Add(uv);
            vertexList.Add(new Vector3((uv.x - 0.5f) * stgWidth, (uv.y - 0.5f) * stgHeight, 0));
            // 填入中间的部分
            for (j = 1; j < WidthSegment; j++)
            {
                baseX = 1f * j / WidthSegment;
                uv = new Vector2(baseX + Random.Range(-1, 1) * widthRange, baseY + Random.Range(rangeMinY, rangeMaxY) * heightRange);
                uvList.Add(uv);
                vertexList.Add(new Vector3((uv.x - 0.5f) * stgWidth, (uv.y - 0.5f) * stgHeight, 0));
            }
            // 放入当前行最后一个
            baseX = 1;
            uv = new Vector2(baseX, baseY + Random.Range(rangeMinY, rangeMaxY) * heightRange);
            uvList.Add(uv);
            vertexList.Add(new Vector3((uv.x - 0.5f) * stgWidth, (uv.y - 0.5f) * stgHeight, 0));
        }
        // 顶点和uv列表赋值
        _vertexList = vertexList;
        _uvList = uvList;
    }

    private GameObject CreateFragmentObject(int index0,int index1,int index2)
    {
        GameObject fragObject = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "ScreenFragmentObject");
        // 添加到TopEffectLayer
        UIManager.GetInstance().AddGoToLayer(fragObject,LayerId.TopEffect);
        // 获取组件
        MeshFilter meshFilter = fragObject.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = fragObject.GetComponent<MeshRenderer>();
        Mesh mesh = new Mesh();
        // 赋予材质
        meshRenderer.material = _fragMat;
        meshRenderer.sortingLayerName = "TopEffect";
        // 计算顶点
        Vector3 vec0 = _vertexList[index0];
        Vector3 vec1 = _vertexList[index1];
        Vector3 vec2 = _vertexList[index2];
        Vector3 centerPos = (vec0 + vec1 + vec2) / 3;
        // 设置坐标
        fragObject.transform.localPosition = centerPos;
        // 计算相对位置
        vec0 -= centerPos;
        vec1 -= centerPos;
        vec2 -= centerPos;
        mesh.SetVertices(new List<Vector3>() { vec0, vec1, vec2 });
        // 设置三角形以及计算uv
        mesh.triangles = new int[] { 0, 1, 2 };
        mesh.uv = new Vector2[] { _uvList[index0], _uvList[index1], _uvList[index2] };
        meshFilter.mesh = mesh;
        return fragObject;
    }

    public void Update()
    {
        if ( _curState == StateWaitCaptureScreen )
        {
            WaitCaptureScreen();
        }
        else if ( _curState == StateCreateFragments )
        {
            CreateFragmentObjects();
        }
        else if ( _curState == StateFragmentsBeginFall )
        {
            SetVelocityForFragments();
        }
        else if ( _curState == StateFragmentsFalling )
        {
            UpdateFragments();
        }
        else if ( _curState == StateFragmentsFallingFinish )
        {
            _isFinish = true;
        }
    }

    private void WaitCaptureScreen()
    {
        if ( _isCaptureComplete )
        {
            _curState = StateCreateFragments;
        }
    }

    /// <summary>
    /// 为碎片设置初始速度
    /// </summary>
    private void SetVelocityForFragments()
    {
        int i;
        FragmentData fragData;
        for (i = 0; i < _fragCount;i++)
        {
            fragData = _fragList[i];
            fragData.vx = Random.Range(-0.5f, 0.5f);
            fragData.vy = Random.Range(-3f, -5f);
            fragData.rotateEulerAngle = new Vector3(0, Random.Range(1.5f,2.5f), 0);
            fragData.isFinish = false;
        }
        // 改变状态
        _curState = StateFragmentsFalling;
    }

    /// <summary>
    ///  更新碎片位置信息
    /// </summary>
    public void UpdateFragments()
    {
        int i;
        FragmentData fragData;
        Transform fragTf;
        Vector3 pos;
        bool isFinish = true;
        for (i = 0; i < _fragCount; i++)
        {
            fragData = _fragList[i];
            if ( !fragData.isFinish )
            {
                fragTf = fragData.fragTf;
                pos = fragTf.localPosition;
                pos.x += fragData.vx;
                pos.y += fragData.vy;
                fragTf.localPosition = pos;
                fragTf.Rotate(fragData.rotateEulerAngle);
                isFinish = false;
                if ( pos.y <= FragFinishPosY )
                {
                    // 置空
                    fragData.fragTf.gameObject.SetActive(false);
                    fragData.isFinish = true;
                }
            }
        }
        if ( isFinish )
        {
            _curState = StateFragmentsFallingFinish;
        }
    }

    public void SetToPos(float posX, float posY)
    {
        throw new System.NotImplementedException();
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }

    public bool IsFinish()
    {
        return _isFinish;
    }

    public void FinishEffect()
    {
        _isFinish = true;
    }
}

class FragmentData
{
    public Transform fragTf;
    public float vx;
    public float vy;
    public Vector3 rotateEulerAngle;
    public bool isFinish;
}
