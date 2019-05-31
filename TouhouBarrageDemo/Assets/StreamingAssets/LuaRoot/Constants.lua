SpellCard = {}
Constants = {}

Constants.VelocityAngle = -999
Constants.OriginalAngle = 999
Constants.OriginalRotation = -9999.5
Infinite = 9999
Constants.ModeLinear = 1
Constants.ModeEaseInQuad = 2
Constants.ModeEaseOutQuad = 3
Constants.ModeEaseInOutQuad = 4
Constants.ModeSin = 9
Constants.ModeCos = 10

Constants.DirNull = 0
Constants.DirLeft = 1
Constants.DirRight = 2

Constants.ActionTypeIdle = 0
Constants.ActionTypeMove = 1
--Constants.ActionTypeFadeToMove = 2
Constants.ActionTypeCast = 3

Constants.ReboundLeft = 1
Constants.ReboundRight = 2
Constants.ReboundTop = 4
Constants.ReboundBottom = 8

eBulletComponentType = {}
eBulletComponentType.CustomizedTask = 1
eBulletComponentType.ParasChange = 2
eBulletComponentType.Rebound = 3
eBulletComponentType.ColliderTrigger = 4

eParaChangeMode = {}
eParaChangeMode.ChangeTo = 1
eParaChangeMode.IncBy = 2
eParaChangeMode.DecBy = 3

Constants.DirModeMoveXTowardsPlayer = 1
Constants.DirModeMoveYTowardsPlayer = 2
Constants.DirModeMoveTowardsPlayer = 3
Constants.DirModeMoveRandom = 4

Constants.STGWidth = 384
Constants.STGHeight = 448

Constants.OriginalWidth = -1
Constants.OriginalHeight= -1

Constants.eLaserHeadTypeNull = 0
Constants.eLaserHeadTypeWhite = 1
Constants.eLaserHeadTypeRed = 2
Constants.eLaserHeadTypeGreen = 3
Constants.eLaserHeadTypeBlue = 4

Constants.eEliminateType = {}
Constants.eEliminateType.ForcedDelete = 0
Constants.eEliminateType.PlayerSpellCard = 1
Constants.eEliminateType.PlayerDead = 2
Constants.eEliminateType.HitPlayer = 4
Constants.eEliminateType.SpellCardEnd = 8
Constants.eEliminateType.HitObject = 16
Constants.eEliminateType.GravitationField = 32
Constants.eEliminateType.CodeEliminate = 2^8
Constants.eEliminateType.CodeRawEliminate = 2^9
Constants.eEliminateType.CustomizedType0 = 2^10
Constants.eEliminateType.CustomizedType1 = 2^11
Constants.eEliminateType.CustomizedType2 = 2^12
Constants.eEliminateType.CustomizedType3 = 2^13
Constants.eEliminateType.CustomizedType4 = 2^14
Constants.eEliminateType.CustomizedType5 = 2^15
eEliminateType = Constants.eEliminateType


Constants.eSpellCardCondition = {}
Constants.eSpellCardCondition.EliminateAll = 1
Constants.eSpellCardCondition.EliminateOne = 2
Constants.eSpellCardCondition.EliminateSpecificOne = 3
Constants.eSpellCardCondition.TimeOver = 5

Constants.eTweenType = {}
Constants.eTweenType.Alpha = 1
Constants.eTweenType.Color = 2
Constants.eTweenType.Pos2D = 3
Constants.eTweenType.Pos3D = 4
Constants.eTweenType.Rotation = 5
Constants.eTweenType.Scale = 6

eEffectLayer = {}
eEffectLayer.Bottom = 130
eEffectLayer.Normal = 150
eEffectLayer.Top = 650

eColliderType = {}
eColliderType.Circle = 1
eColliderType.Rect = 2

eColliderGroup = {}
eColliderGroup.Player = 1
eColliderGroup.PlayerBullet = 2
eColliderGroup.Enemy = 4
eColliderGroup.EnemyBullet = 8
eColliderGroup.Item = 16

eBlendMode = {}
eBlendMode.Normal = 0
eBlendMode.SoftAdditive = 1

eBulletParaType = {}
eBulletParaType.Velocity = 1
eBulletParaType.Vx = 2
eBulletParaType.Vy = 3
eBulletParaType.VAngel = 4
eBulletParaType.Acc = 5
eBulletParaType.AccAngle = 6
eBulletParaType.MaxVelocity = 7
eBulletParaType.CurveRadius = 11
eBulletParaType.CurveAngle = 12
eBulletParaType.CurveDeltaR = 13
eBulletParaType.CurveOmega = 14
eBulletParaType.CurveCenterX = 15
eBulletParaType.CurveCenterY = 16
eBulletParaType.Alpha = 21
eBulletParaType.ScaleX = 25
eBulletParaType.ScaleY = 26
eBulletParaType.LaserLength = 31
eBulletParaType.LaserWidth = 32