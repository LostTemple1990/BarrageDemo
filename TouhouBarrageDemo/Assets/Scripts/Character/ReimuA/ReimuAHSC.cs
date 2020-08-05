using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReimuAHSC : BombBase
{
    private const int MaxEffectCount = 8;

    private List<SCEffect> _effectList;

    public ReimuAHSC()
    {
        _effectList = new List<SCEffect>();
    }

    public override void Start()
    {
        SCEffect effect;
        float angle = MTRandom.GetNextFloat(0, 360);
        for (int i = 0; i < MaxEffectCount; i++)
        {
            effect = new SCEffect();
            effect.Init(angle + i * 45, i);
            _effectList.Add(effect);
        }
        // 音效
        SoundManager.GetInstance().Play("se_masterspark", 0.1f, false, true);
    }

    public override void Update()
    {
        bool isAllFinish = true;
        SCEffect effect;
        for (int i = 0; i < MaxEffectCount; i++)
        {
            effect = _effectList[i];
            if (effect != null && !effect.IsFinish)
            {
                effect.Update();
                isAllFinish = false;
                if (effect.IsFinish)
                {
                    effect.Clear();
                    _effectList[i] = null;
                }
            }
        }
        if (isAllFinish)
        {
            _isFinish = true;
        }
    }

    public override void Clear()
    {
        for (int i = 0; i < MaxEffectCount; i++)
        {
            _effectList[i]?.Clear();
        }
    }

    class SCEffect : ICommand
    {
        private GameObject _go;
        private ParticleSystem _ps;
        private Transform _tf;
        private int _timer;
        /// <summary>
        /// 是否结束
        /// </summary>
        private bool _isFinish;
        private int _index;
        private int _totalTime;
        /// <summary>
        /// 状态
        /// </summary>
        private int _state;
        private float _curRot;
        /// <summary>
        /// 自机引用
        /// </summary>
        private CharacterBase _player;
        /// <summary>
        /// 目标
        /// </summary>
        private EnemyBase _target;
        private int _targetInstID;
        /// <summary>
        /// 当前坐标
        /// </summary>
        private Vector2 _curPos;

        private ObjectColliderBase _collider;
        /// <summary>
        /// 伤害
        /// </summary>
        private float _dmg;
        /// <summary>
        /// collider尺寸
        /// </summary>
        private int _size;

        public SCEffect()
        {
            _go = ResourceManager.GetInstance().GetPrefab("Prefab/Character/ReimuA", "ReimuASC");
            _tf = _go.transform;
            _ps = _go.GetComponent<ParticleSystem>();
            _timer = 0;
            _player = PlayerInterface.GetInstance().GetCharacter();
            UIManager.GetInstance().AddGoToLayer(_go, LayerId.STGNormalEffect);
        }

        public void Init(float angle,int index)
        {
            _dmg = 35;
            _size = 32;
            _curRot = angle;
            _index = index;
            _state = 1;
            _totalTime = 190 - index * 10;
            CommandManager.GetInstance().Register(CommandConsts.PauseGame, this);
            CommandManager.GetInstance().Register(CommandConsts.ContinueGame, this);
            _collider = ColliderManager.GetInstance().CreateColliderByType(eColliderType.Circle);
            _collider.SetPosition(2000, 0);
            _collider.SetSize(_size, _size);
            _collider.RegisterCallback(eColliderGroup.Enemy, CollidedByEnemyCallback);
            _collider.SetEliminateType(eEliminateDef.PlayerSpellCard);
            _collider.SetColliderGroup(eColliderGroup.Enemy | eColliderGroup.EnemyBullet);
            _collider.SetHitEnemyDamage(_dmg);
            _isFinish = false;
        }

        private void CollidedByEnemyCallback(ObjectColliderBase collider,ICollisionObject enemy)
        {
            ClearCollider();
            if (!_isFinish)
            {
                Explode();
                ShakeEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.ShakeEffect) as ShakeEffect;
                effect.DoShake(0, 5, 1, 5);
                //ObjectColliderBase newCollider = ColliderManager.GetInstance().CreateColliderByType(eColliderType.Circle);
                //newCollider.SetSize(_size, _size);
                //newCollider.SetPosition(_curPos);
                //newCollider.SetEliminateType(eEliminateDef.PlayerSpellCard);
                //newCollider.SetColliderGroup(eColliderGroup.Enemy | eColliderGroup.EnemyBullet);
                //newCollider.SetHitEnemyDamage(_dmg);
                //newCollider.SetExistDuration(1);
                _isFinish = true;
            }
        }

        private void ClearCollider()
        {
            if (_collider != null)
            {
                _collider.ClearSelf();
                _collider = null;
            }
        }

        private void Explode()
        {
            STGSpriteEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.SpriteEffect) as STGSpriteEffect;
            effect.SetSprite("STGEffectAtlas", "TransparentCircle", eBlendMode.SoftAdditive, LayerId.STGNormalEffect, true);
            effect.SetExistDuration(30);
            effect.SetPosition(_curPos);
            effect.SetSize(128, 128);
            effect.ChangeWidthTo(192, 30, InterpolationMode.Linear);
            effect.ChangeHeightTo(192, 30, InterpolationMode.Linear);
            effect.DoFade(30);
            SoundManager.GetInstance().Play("se_explode", 0.1f);
            float angle = Random.Range(0f, 360f);
            List<Color> colorList = new List<Color> { new Color(1, 0, 0), new Color(0, 1, 0), new Color(0, 0, 1) };
            for (int i = 0; i < 12; i++)
            {
                effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.SpriteEffect) as STGSpriteEffect;
                effect.SetSprite("STGReimuAtlas", "ReimuAHSCEffect", eBlendMode.SoftAdditive, LayerId.STGNormalEffect, true);
                effect.SetExistDuration(30);
                effect.SetPosition(_curPos);
                effect.DoStraightMove(Random.Range(4f, 6f), i * 30 + angle);
                int colorIndex = Random.Range(0, 3);
                effect.SetSpriteColor(colorList[colorIndex].r, colorList[colorIndex].g, colorList[colorIndex].b);
                effect.DoFade(30);
                effect.SetSize(128, 128);
                effect.ChangeWidthTo(64, 30, InterpolationMode.Linear);
                effect.ChangeHeightTo(64, 30, InterpolationMode.Linear);
            }
        }

        public void Update()
        {
            _timer++;
            if (_state == 1)
            {
                OnState1Update();
            }
            else if (_state == 2)
            {
                OnState2Update();
            }
            if (_state == 3)
            {
                OnState3Update();
            }
            if (_collider != null)
            {
                _collider.SetPosition(_curPos);
            }
        }

        private void OnState1Update()
        {
            if (_timer < _totalTime)
            {
                _curRot += 4;
                float posX = _timer * Mathf.Cos(Mathf.Deg2Rad * _curRot);
                float posY = _timer * Mathf.Sin(Mathf.Deg2Rad * _curRot);
                _curPos = _player.GetPosition() + new Vector2(posX, posY);
                _tf.localPosition = _curPos;
            }
            else
            {
                _state = 2;
            }
        }

        private void OnState2Update()
        {
            bool isInScreen = MathUtil.DetectCollisionBetweenAABBAndAABB(_curPos, _size, _size, Vector2.zero, 192, 224);
            if (_target == null || (_target != null && _target.GetInstanceID() != _targetInstID))
            {
                _target = FindTarget();
                if (_target != null)
                {
                    _targetInstID = _target.GetInstanceID();
                }
            }
            if (_target != null)
            {
                float an = MathUtil.GetAngleBetweenXAxis(_target.GetPosition() - _curPos);
                float a = an - _curRot;
                a = MathUtil.ClampAngle(a);
                if (a >= 180)
                    a -= 360;
                float da = 1200f / (Vector2.Distance(_target.GetPosition(), _curPos) + 1);
                if (da >= Mathf.Abs(a))
                {
                    _curRot = an;
                }
                else
                {
                    _curRot += da * Mathf.Sign(a);
                }
                //Logger.Log(string.Format("a={0},da={1},curRot={2}", a, da, _curRot));
            }
            float dx = 8f * Mathf.Cos(Mathf.Deg2Rad * _curRot);
            float dy = 8f * Mathf.Sin(Mathf.Deg2Rad * _curRot);
            if (isInScreen)
            {
                if (_curPos.x > 192)
                {
                    _curPos.x = 192;
                    dx = dy = 0;
                }
                if (_curPos.x < -192)
                {
                    _curPos.x = -192;
                    dx = dy = 0;
                }
                if (_curPos.y < -224)
                {
                    _curPos.y = -224;
                    dx = dy = 0;
                }
                if (_curPos.y > 224)
                {
                    _curPos.y = 224;
                    dx = dy = 0;
                }
            }
            _curPos += new Vector2(dx, dy);
            _tf.localPosition = _curPos;
            if (_timer >= 230)
            {
                _dmg = _dmg * 0.4f;
                _size = _size * 2;
                _collider.SetSize(64, 64);
                _collider.SetHitEnemyDamage(_dmg);
                _state = 3;
            }
        }

        private void OnState3Update()
        {
            float scale = (_timer - 230) * 0.5f + 1;
            _tf.localScale = new Vector3(scale, scale, 1);
            if (_timer >= 240)
            {
                _state = 4;
                Explode();
                _isFinish = true;
            }
        }

        private EnemyBase FindTarget()
        {
            EnemyBase target = null;
            EnemyBase tmpTarget;
            List<EnemyBase> enemyList = EnemyManager.GetInstance().GetEnemyList();
            int enemyCount = enemyList.Count;
            float maxK = -1;
            for (int i = 0; i < enemyCount; i++)
            {
                tmpTarget = enemyList[i];
                if (tmpTarget != null && tmpTarget.isAvailable && tmpTarget.CanHit())
                {
                    Vector2 dVec = tmpTarget.GetPosition() - _curPos;
                    float k = dVec.y / (Mathf.Abs(dVec.x) + 0.01f);
                    if (k > maxK)
                    {
                        maxK = k;
                        target = tmpTarget;
                    }
                }
            }
            return target;
        }

        public void Clear()
        {
            CommandManager.GetInstance().Remove(CommandConsts.PauseGame, this);
            CommandManager.GetInstance().Remove(CommandConsts.ContinueGame, this);
            GameObject.Destroy(_go);
            ClearCollider();
            _go = null;
            _tf = null;
            _ps = null;
            _player = null;
            _target = null;
        }

        public void Execute(int cmd,object data)
        {
            if (cmd == CommandConsts.PauseGame)
            {
                _ps.Pause();
            }
            else if (cmd == CommandConsts.ContinueGame)
            {
                _ps.Play();
            }
        }

        public bool IsFinish
        {
            get { return _isFinish; }
        }
    }
}
