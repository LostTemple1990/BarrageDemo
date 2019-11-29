using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgBlockContainer
{
    private float _height;
    private float _speed;
    private GameObject _container;
    private Transform _containerTf;
    private float _curPosZ;
    private List<GameObject> _blockObjList;
    private int _blockCount;

    private float _intervalMinX;
    private float _intervalMaxX;
    private float _intervalMinY;
    private float _intervalMaxY;
    private float _intervalMinZ;
    private float _intervalMaxZ;

    private float _disappearZ;

    public BgBlockContainer(Transform bgLayerTf)
    {
        _container = new GameObject();
        _container.name = "BgContainer";
        _containerTf = _container.transform;
        _containerTf.SetParent(bgLayerTf, false);
        _curPosZ = 0;
    }

    public void Init()
    {
        _intervalMinX = 0;
        _intervalMaxX = 0;
        _intervalMinY = 0;
        _intervalMaxY = 0;
        _intervalMinZ = 0;
        _intervalMaxZ = 0;
    }

    public void SetBlockObject(GameObject obj, int count)
    {
        if (_blockObjList == null)
        {
            _blockObjList = new List<GameObject>();
        }
        obj.transform.SetParent(_containerTf, false);
        _blockObjList.Add(obj);
        GameObject blockObj;
        for (int i = 1; i < count; i++)
        {
            blockObj = GameObject.Instantiate(obj);
            blockObj.transform.SetParent(_containerTf, false);
            _blockObjList.Add(blockObj);
        }
        _blockCount = count;
    }

    public void SetIntervalRangeX(float minX, float maxX)
    {
        _intervalMinX = minX;
        _intervalMaxX = maxX;
    }

    public void SetIntervalRangeY(float minY, float maxY)
    {
        _intervalMinY = minY;
        _intervalMaxY = maxY;
    }
    public void SetIntervalRangeZ(float minZ, float maxZ)
    {
        _intervalMinZ = minZ;
        _intervalMaxZ = maxZ;
    }

    public void SetDisappearZ(float posZ)
    {
        _disappearZ = posZ;
    }

    public void GenerateBlocks(Vector3 startPos)
    {
        GameObject blockObj = _blockObjList[0];
        blockObj.transform.localPosition = startPos;
        Vector3 pos = startPos;
        for (int i = 1; i < _blockCount; i++)
        {
            blockObj = _blockObjList[i];
            pos += new Vector3(Random.Range(_intervalMinX, _intervalMaxX), Random.Range(_intervalMinY, _intervalMaxY), Random.Range(_intervalMinZ, _intervalMaxZ));
            blockObj.transform.localPosition = pos;
        }
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void Update()
    {
        int i = 0;
        GameObject blockObj;
        Vector3 pos;
        for (i = 0; i < _blockCount; i++)
        {
            blockObj = _blockObjList[i];
            pos = blockObj.transform.localPosition;
            pos.z += _speed;
            blockObj.transform.localPosition = pos;
        }
        while (true)
        {
            blockObj = _blockObjList[0];
            pos = blockObj.transform.localPosition;
            if (pos.z <= _disappearZ)
            {
                // 根据最后一个背景块的位置作为参考设置新的背景块的位置
                pos = _blockObjList[_blockCount - 1].transform.localPosition;
                pos += new Vector3(Random.Range(_intervalMinX, _intervalMaxX), Random.Range(_intervalMinY, _intervalMaxY), Random.Range(_intervalMinZ, _intervalMaxZ));
                blockObj.transform.localPosition = pos;
                _blockObjList.Add(blockObj);
                _blockObjList.RemoveAt(0);
            }
            else
            {
                break;
            }
        }
    }

    public void Clear()
    {
        GameObject.Destroy(_container);
        _container = null;
        _containerTf = null;
        _blockObjList.Clear();
        _blockObjList = null;
    }
}