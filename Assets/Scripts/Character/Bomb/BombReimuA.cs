using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombReimuA : BombBase
{
    private const int BombCount = 3;
    private const float DefaultRadius = 32;
    /// <summary>
    /// 生成炸弹的地点偏移量
    /// </summary>
    private List<Vector3> _posOffsets;
    private List<GameObject> _bombs;
    private List<Transform> _bombsTf;
    private List<Color> _colors;
    private List<ColliderCircle> _colliderList;

    private int _curState;
    private int _time;
    private int _duration;
    private float _curScale;
    private float _detectRadius;
    private Vector2 _detectCenter;

    public BombReimuA()
    {
        _posOffsets = new List<Vector3>();
        _posOffsets.Add(new Vector3(0,20,0));
        _posOffsets.Add(new Vector3(-14,-14,0));
        _posOffsets.Add(new Vector3(14,-14,0));
        _bombs = new List<GameObject>();
        _bombsTf = new List<Transform>();
        _colors = new List<Color>();
        _colors.Add(new Color(0.5f, 0, 0.5f, 0.5f));
        _colors.Add(new Color(0, 0, 0.5f, 0.5f));
        _colors.Add(new Color(0, 0.5f, 0, 0.5f));
        _colliderList = new List<ColliderCircle>();
    }

    public override void Start()
    {
        int i;
        GameObject go;
        Transform tf;
        Vector2 playerPos = Global.PlayerPos;
        ColliderCircle collider;
        Vector2 bombPos;
        for (i=0;i<BombCount;i++)
        {
            go = ResourceManager.GetInstance().GetPrefab("Prefab/Character", "ReimuABomb");
            tf = go.transform;
            UIManager.GetInstance().AddGoToLayer(go, LayerId.STGNormalEffect);
            _bombs.Add(go);
            _bombsTf.Add(tf);
            bombPos = new Vector2(playerPos.x + _posOffsets[i].x, playerPos.y + _posOffsets[i].y);
            tf.localPosition = bombPos;
            tf.localScale = Vector3.zero;
            SpriteRenderer sp = tf.Find("Bomb").GetComponent<SpriteRenderer>();
            sp.material.SetColor("_Color", _colors[i]);
            // 创建ObjectCollider
            collider = ColliderManager.GetInstance().CreateColliderByType(eColliderType.Circle) as ColliderCircle;
            collider.SetSize(0,0);
            collider.SetColliderGroup(eColliderGroup.EnemyBullet|eColliderGroup.Enemy);
            collider.SetToPositon(bombPos.x, bombPos.y);
            collider.SetEliminateType(eEliminateDef.PlayerSpellCard);
            collider.SetHitEnemyDamage(1);
            _colliderList.Add(collider);
        }
        _curState = 1;
        _time = 0;
        _duration = 180;
        _isFinish = false;
    }

    public override void Update()
    {
        if ( _curState == 1 )
        {
            UpdateState1();
        }
        else if ( _curState == 2 )
        {
            UpdateState2();
        }
        else
        {
            _isFinish = true;
        }
    }

    public void UpdateState1()
    {
        _time++;
        _curScale = MathUtil.GetEaseOutQuadInterpolation(0, 1.25f, _time, _duration);
        _detectRadius = _curScale * DefaultRadius;
        for (int i=0;i<BombCount;i++)
        {
            _bombsTf[i].localScale = new Vector3(_curScale, _curScale, 1);
            _colliderList[i].SetSize(_detectRadius, _detectRadius);
        }
        if (_time >= _duration)
        {
            _curState = 2;
            _time = 0;
            _duration = 60;
        }
    }

    public void UpdateState2()
    {
        _time++;
        _curScale = MathUtil.GetEaseOutQuadInterpolation(1.25f, 10f, _time, _duration);
        _detectRadius = _curScale * DefaultRadius;
        //CheckCollisionWithEnemyBullets();
        //CheckCollisionWithEnemy();
        for (int i = 0; i < BombCount; i++)
        {
            _bombsTf[i].localScale = new Vector3(_curScale, _curScale, 1);
            _colliderList[i].SetSize(_detectRadius, _detectRadius);
        }
        if ( _time >= _duration )
        {
            // 全屏伤害
            ColliderCircle collider = ColliderManager.GetInstance().CreateColliderByType(eColliderType.Circle) as ColliderCircle;
            collider.SetSize(500, 500);
            collider.SetColliderGroup(eColliderGroup.EnemyBullet | eColliderGroup.Enemy);
            collider.SetToPositon(Global.PlayerPos.x, Global.PlayerPos.y);
            collider.SetEliminateType(eEliminateDef.PlayerSpellCard);
            collider.SetHitEnemyDamage(400);
            collider.SetExistDuration(1);
            _curState = 3;
        }
    }

    public override void Clear()
    {
        int count = _bombs.Count;
        for (int i=0;i< count; i++)
        {
            _colliderList[i].ClearSelf();
            GameObject.Destroy(_bombs[i]);
        }
        _bombs.Clear();
        _bombsTf.Clear();
        _colliderList.Clear();
    }
}
