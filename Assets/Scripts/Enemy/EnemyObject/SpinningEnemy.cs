using UnityEngine;
using System.Collections;

public class SpinningEnemy : EnemyObjectBase
{
    protected string _prefabName;
    protected GameObject _go;
    protected Transform _tf;

    protected Transform _border0Tf;
    protected Transform _border1Tf;

    private static Vector3 Border0RotateEuler = new Vector3(0,0,4);
    private static Vector3 Border1RotateEuler = new Vector3(0,0,-3);

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
            UIManager.GetInstance().AddGoToLayer(_go, LayerId.Enemy);
        }
    }

    public override void Update()
    {
        _border0Tf.Rotate(Border0RotateEuler);
        _border1Tf.Rotate(Border1RotateEuler);
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

    public override EnemyObjectType GetObjectType()
    {
        return EnemyObjectType.SpinningEnemy;
    }

    public override void Clear()
    {
        UIManager.GetInstance().HideGo(_go);
        ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _go);
        _go = null;
        _tf = null;
        _border0Tf = null;
        _border1Tf = null;
    }
}
