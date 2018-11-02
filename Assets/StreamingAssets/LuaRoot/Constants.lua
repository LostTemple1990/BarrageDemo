SpellCard = {}
Constants = {}

Constants.VelocityAngle = -999
Constants.OriginalAngle = 999
Infinite = 9999
Constants.ModeLinear = 1
Constants.ModeEaseInQuad = 2
Constants.ModeEaseOutQuad = 3
Constants.ModeEaseInOutQuad = 4
Constants.ModeSin = 9

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

Constants.BCTypeCustomizedTask = 1
Constants.BCTypeMoveParasChange = 2
Constants.BCTypeRebound = 3

Constants.ParaChangeMode_ChangeTo = 1
Constants.ParaChangeMode_IncBy = 2
Constants.ParaChangeMode_DecBy = 3

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
Constants.eEliminateType.CodeEliminate = 32
Constants.eEliminateType.CodeRawEliminate = 64


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
