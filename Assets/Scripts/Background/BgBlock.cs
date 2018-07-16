using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgBlock
{
    protected GameObject _blockObj;

    protected float _width;

    protected float _height;

    protected int _sortingOrder;

    public BgBlock()
    {
        _sortingOrder = 0;
    }

    /// <summary>
    /// 设置背景块的对象
    /// </summary>
    /// <param name="obj"></param>
    public void SetObject(GameObject obj)
    {
        _blockObj = obj;
    }

    public void SetObject(GameObject obj,float width,float height)
    {
        _blockObj = obj;
        _width = width;
        _height = height;
    }

    public float width
    {
        get { return _width; }
        set { _width = value; }
    }

    public float height
    {
        get { return _height; }
        set { _height = value; }
    }

    public void SetToPos(Vector3 pos)
    {
        _blockObj.transform.localPosition = pos;
    }

    public void AddToPanel(Transform canvasTf)
    {
        _blockObj.transform.parent = canvasTf;
    }

    /// <summary>
    /// 复制方法
    /// </summary>
    /// <returns></returns>
    public BgBlock Clone()
    {
        if ( _blockObj == null )
        {
            return null;
        }
        GameObject cloneObj = GameObject.Instantiate(_blockObj);
        BgBlock cloneBlock = new BgBlock();
        cloneBlock.SetObject(cloneObj, _width, _height);
        return cloneBlock;
    }
}
