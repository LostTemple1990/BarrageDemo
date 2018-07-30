using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class STGBurstEffect : IEffect
{
    private List<GameObject> _burstList;
    public void Init()
    {
        if ( _burstList == null )
        {
            _burstList = new List<GameObject>();
        }
    }

    public void Update()
    {
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }

    public void FinishEffect()
    {
        throw new System.NotImplementedException();
    }
}

class BurstObject
{
    public GameObject go;
    public Transform tf;
    public SpriteRenderer spRenderer;
    public int time;
    public int duration;
    float vx;
    float vy;
    Vector3 dScale;
    Vector3 ratoteAngle;
    public int state;
    public float dAlpha;
    public Color tmpColor;

    public void Update()
    {
        // 位置
        Vector3 pos = tf.localPosition;
        pos.x += vx;
        pos.y += vy;
        tf.localPosition = pos;
        // 放大倍数
        Vector3 scale = tf.localScale;
        scale += dScale;
        tf.localScale = scale;
        // 旋转角度
        tf.Rotate(ratoteAngle);
        // 透明度
        time++;
        if ( time >= duration )
        {

        }
    }

    public void Clear()
    {

    }
}
