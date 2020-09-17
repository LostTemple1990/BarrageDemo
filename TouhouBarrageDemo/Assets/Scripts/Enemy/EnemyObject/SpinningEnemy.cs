using UnityEngine;
using System.Collections;

public class SpinningEnemy : EnemyObjectBase
{
    protected static Color DefaultColor = new Color(1, 1, 1, 1);
    protected static Vector3 DefaultScale = new Vector3(1, 1, 1);

    protected string _prefabName;
    protected GameObject _go;
    protected Transform _tf;

    protected Transform _border0Tf;
    protected Transform _border1Tf;

    private static Vector3 Border0RotateEuler = new Vector3(0,0,4);
    private static Vector3 Border1RotateEuler = new Vector3(0,0,-3);

    protected SpriteRenderer _border0SR;
    protected SpriteRenderer _border1SR;
    protected SpriteRenderer _bodySR;

    public override void Init()
    {
        
    }

    public override void SetEnemyAni(string aniName)
    {
        _prefabName = aniName;
        if (_go == null)
        {
            _go = ResourceManager.GetInstance().GetPrefab("Prefab/Enemy", aniName);
            _tf = _go.transform;
            _border0Tf = _tf.Find("Border0");
            _border1Tf = _tf.Find("Border1");
            _bodySR = _tf.Find("Object").GetComponent<SpriteRenderer>();
            _border0SR = _border0Tf.GetComponent<SpriteRenderer>();
            _border1SR = _border1Tf.GetComponent<SpriteRenderer>();
            UIManager.GetInstance().AddGoToLayer(_go, LayerId.Enemy);
        }
    }

    public override void Update()
    {
        _border0Tf.Rotate(Border0RotateEuler);
        _border1Tf.Rotate(Border1RotateEuler);
        if (_isColorChanged)
        {
            _bodySR.color = _curColor;
            _border0SR.color = _curColor;
            _border1SR.color = _curColor;
            _isColorChanged = false;
        }
        if (_isScaleChanged)
        {
            _tf.localScale = _curScale;
            _isScaleChanged = false;
        }
    }

    public override void SetToPosition(float posX, float posY)
    {
        _tf.localPosition = new Vector2(posX, posY);
    }

    public override void SetToPosition(Vector2 pos)
    {
        _tf.localPosition = pos;
    }

    public override GameObject GetObject()
    {
        return _go;
    }

    public override eEnemyObjectType GetObjectType()
    {
        return eEnemyObjectType.SpinningEnemy;
    }

    public override void SetColor(float r, float g, float b, float a)
    {
        _curColor = new Color(r, g, b, a);
        _isColorChanged = true;
    }

    public override void SetScale(float scaleX, float scaleY)
    {
        _curScale = new Vector3(scaleX, scaleY, 1);
        _isScaleChanged = true;
    }

    public override void Clear()
    {
        if (_curColor != DefaultColor)
        {
            _bodySR.color = DefaultColor;
            _border0SR.color = DefaultColor;
            _border1SR.color = DefaultColor;
            _curColor = DefaultColor;
        }
        if (_curScale != DefaultScale)
        {
            _tf.localScale = DefaultScale;
            _curScale = DefaultScale;
        }
        UIManager.GetInstance().HideGo(_go);
        ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _go);
        _go = null;
        _tf = null;
        _border0Tf = null;
        _border1Tf = null;
    }
}
