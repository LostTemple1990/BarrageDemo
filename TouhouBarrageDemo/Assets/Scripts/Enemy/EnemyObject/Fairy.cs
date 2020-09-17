using UnityEngine;
using System.Collections;

public class Fairy : EnemyObjectBase
{
    protected AnimationCharacter _enemyAni;
    protected GameObject _enemyGo;
    protected Transform _tf;
    protected int _moveTime;
    protected int _moveDuration;
    protected bool _isMoving;

    public override void Init()
    {
        _isMoving = false;
    }

    public override void SetEnemyAni(string aniId)
    {
        if ( _enemyAni == null )
        {
            _enemyAni = AnimationManager.GetInstance().CreateAnimation<AnimationCharacter>(LayerId.Enemy);
            _enemyGo = _enemyAni.GetAniParent();
            _tf = _enemyGo.transform;
        }
        _enemyAni.Play(aniId, AniActionType.Idle, Consts.DIR_NULL);
    }

    public override void Update()
    {
        if ( _isMoving )
        {
            _moveTime++;
            if ( _moveTime >= _moveDuration )
            {
                _isMoving = false;
                _enemyAni.Play(AniActionType.Idle, Consts.DIR_NULL);
            }
        }
        if (_isColorChanged)
        {
            _enemyAni.SetColor(_curColor.r, _curColor.g, _curColor.b, _curColor.a);
            _isColorChanged = false;
        }
        if (_isScaleChanged)
        {
            _enemyAni.SetScale(_curScale.x, _curScale.y);
            _isScaleChanged = false;
        }
    }

    public override void DoAction(AniActionType actType, int dir, int duration=Consts.MaxDuration)
    {
        if (_enemyAni.Play(actType, dir))
        {
            if (actType == AniActionType.Move)
            {
                _isMoving = true;
                _moveTime = 0;
                _moveDuration = duration;
            }
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
        return _enemyGo;
    }

    public override void SetColor(float r, float g, float b, float a)
    {
        _curColor = new Color(r, g, b, a);
        _isColorChanged = true;
    }

    public override void SetScale(float scaleX, float scaleY)
    {
        _curScale = new Vector2(scaleX, scaleY);
        _isScaleChanged = true;
    }

    public override void Clear()
    {
        _enemyGo = null;
        _tf = null;
        AnimationManager.GetInstance().RemoveAnimation(_enemyAni);
        _enemyAni = null;
    }

    public override eEnemyObjectType GetObjectType()
    {
        return eEnemyObjectType.Fairy;
    }
}
