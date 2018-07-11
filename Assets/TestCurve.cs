using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCurve : MonoBehaviour
{
    /// <summary>
    /// 分段
    /// </summary>
    public const int Segment = 50;
    public const int LineLength = 100;
    public const int LineHigh = 2;

    private Vector3[] _vecs;
    private int[] _triVecs;

	// Use this for initialization
	void Start () {
        _vecs = new Vector3[Segment*2];
        _triVecs = new int[Segment*6];
        // y = 20sin(x/50)
        int i, j;
        float tmpX, tmpY,curAngle;
        curAngle = 0f;
        float deltaAngle = Mathf.Deg2Rad * 180f / Segment;
        int tmpIndex;
        for (i=0;i<Segment;i++)
        {
            tmpIndex = i * 2;
            tmpX = 1f * i * LineLength / Segment;
            tmpY = Mathf.Sin(curAngle) * 20f;
            _vecs[tmpIndex] = new Vector3(tmpX,tmpY+2,0);
            _vecs[tmpIndex+1] = new Vector3(tmpX, tmpY -2,0);
            curAngle += deltaAngle;
        }
        for (i=0;i<Segment-1;i++)
        {
            tmpIndex = i * 6;
            _triVecs[tmpIndex] = i * 2;
            _triVecs[tmpIndex + 1] = i * 2 + 3;
            _triVecs[tmpIndex + 2] = i * 2 + 1;
            _triVecs[tmpIndex + 3] = i * 2;
            _triVecs[tmpIndex + 4] = i * 2 + 2;
            _triVecs[tmpIndex + 5] = i * 2 + 3;
        }
        Mesh mesh = this.gameObject.GetComponent<MeshFilter>().mesh;
        mesh.vertices = _vecs;
        mesh.triangles = _triVecs;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
