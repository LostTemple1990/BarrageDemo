local SC = SpellCard
local Condition = Constants.eSpellCardCondition
local EliminateType = Constants.eEliminateType
lib = require "LuaLib"

local CustomizedBulletTable = {}
local CustomizedEnemyTable = {}


--waitTime之后变换成札弹的子弹
CustomizedBulletTable.NazrinSC1Bullet0 = {}
CustomizedBulletTable.NazrinSC1Bullet0.Init = function(bullet,waitTime,maxRadius)
	lib.SetBulletResistEliminatedFlag(bullet,EliminateType.PlayerSpellCard + EliminateType.PlayerDead + EliminateType.HitPlayer)
	lib.AddBulletTask(bullet,function()
		local posX,posY = lib.GetBulletPos(bullet)
		lib.AddBulletComponent(bullet,Constants.BCTypeMoveParasChange)
		lib.AddBulletParaChangeEvent(bullet,5,Constants.ParaChangeMode_ChangeTo,maxRadius,0,Constants.ModeLinear,30)
		--lib.AddBulletParaChangeEvent(bullet,8,Constants.ParaChangeMode_ChangeTo,1.5,50-waitTime,Constants.ModeLinear,10)
		if coroutine.yield(80-waitTime) == false then return end
		lib.ChangeBulletStyleById(bullet,"107060")
		--lib.SetBulletOrderInLayer(bullet,1)
		local angle = lib.GetAimToPlayerAngle(posX,posY)
		lib.SetBulletStraightParas(bullet,2.5,angle,false,0,0)
	end)
end

--会被防护罩消去的子弹
CustomizedBulletTable.NazrinSC1Bullet1 = {}
CustomizedBulletTable.NazrinSC1Bullet1.Init = function(bullet,eliminateDis)
	lib.AddBulletTask(bullet,function()
		for _=1,Infinite do
			local posX,posY = lib.GetBulletPos(bullet)
			local centerX,centerY = lib.GetGlobalVector2("SC1CircleCenter0")
			if lib.GetVectorLength(posX,posY,centerX,centerY) < eliminateDis then
				lib.EliminateBullet(bullet)
			else
				centerX,centerY = lib.GetGlobalVector2("SC1CircleCenter1")
				if lib.GetVectorLength(posX,posY,centerX,centerY) < eliminateDis then
					lib.EliminateBullet(bullet)
				end
			end
			if coroutine.yield(1) == false then return end
		end
	end)
end

CustomizedEnemyTable.NazrinSC1Enemy = {}
CustomizedEnemyTable.NazrinSC1Enemy.Init = function(enemy,toPosX,toPosY,duration)
	lib.SetEnemyMaxHp(enemy,50)
	--冲向玩家
	lib.AddEnemyTask(enemy,function()
		lib.EnemyMoveToPos(enemy,toPosX,toPosY,60,Constants.ModeEaseInQuad)
		if coroutine.yield(60+duration) == false then return end
		--消灭自身
		lib.HitEnemy(enemy,999)
	end)
	--发射大玉和中玉
	lib.AddEnemyTask(enemy,function()
		if coroutine.yield(60) == false then return end
		for _=1,Infinite do
			local posX,posY = lib.GetEnemyPos(enemy)
			local bullet
			local k = lib.GetRandomInt(0,1)
			if k == 0 then
				bullet = lib.CreateCustomizedBullet("NazrinSC1Bullet1","118010",posX,posY,100,1)
				--bullet = lib.CreateSimpleBulletById("0000",posX,posY)
			else
				--bullet = lib.CreateSimpleBulletById("0010",posX,posY)
				bullet = lib.CreateCustomizedBullet("NazrinSC1Bullet1","113010",posX,posY,85,1)
			end
			lib.SetBulletStraightParas(bullet,12,lib.GetRandomInt(-45,45),true,0,0)
			if coroutine.yield(2) == false then return end
		end
	end)
end

function SC.NazrinTest(boss)
	lib.SetSpellCardProperties("Easy-SpellCard",60,Condition.EliminateAll,true,function()
		local effect = lib.GetGlobalUserData("SC1Effect0")
		lib.SetEffectFinish(effect)
		lib.RemoveGlobalUserData("SC1Effect0")
		effect = lib.GetGlobalUserData("SC1Effect1")
		lib.SetEffectFinish(effect)
		lib.RemoveGlobalUserData("SC1Effect1")
	end)
	--bossCG
	do
		local tweenList = {}
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=0,duration=30,beginValue={x=200,y=200},endValue={x=0,y=0},mode=Constants.ModeEaseInQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=60,duration=60,beginValue={x=0,y=0},endValue={x=-200,y=-200},mode=Constants.ModeEaseOutQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=0,duration=0,beginValue=1,endValue=1,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=90,duration=30,beginValue=1,endValue=0,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Scale,delay=0,duration=0,beginValue={x=0.75,y=0.75,z=1},endValue={x=0.75,y=0.75,z=1},mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		lib.PlayCharacterCG("CG/face04ct",tweenList)
	end
	lib.SetBossInvincible(boss,5)
	lib.SetEnemyMaxHp(boss,5000)
	lib.ShowBossBloodBar(boss,true)
	--圈形子弹的task
	lib.AddEnemyTask(boss,function()
		local k = 1
		for _=1,Infinite do
			local i
			local posX,posY = lib.GetEnemyPos(boss)
			lib.SetGlobalVector2("SC1CircleCache",posX,posY)
			for i=0,15 do
				local bullet = lib.CreateCustomizedBullet("NazrinSC1Bullet0","113020",posX,posY,i,75,2)
				lib.SetBulletCurvePara(bullet,0,23.5*i*k,0,1*k)
				if coroutine.yield(1)==false then return end
			end
			k = -k
			if coroutine.yield(90)==false then return end
		end
	end)
	--圈形防护罩0
	lib.AddEnemyTask(boss,function()
		local spriteEffect = lib.CreateSpriteEffectWithProps("STGEffectAtlas","TransparentCircle",eBlendMode.Normal,eEffectLayer.Bottom,false,0)
		lib.SetEffectToPos(spriteEffect,2000,2000)
		lib.SetSpriteEffectScale(spriteEffect,5,5)
		lib.SetSpriteEffectColor(spriteEffect,0.55,0.45,0.65,0.75)
		lib.SetGlobalUserData("SC1Effect0",spriteEffect)
		--ObjectCollider
		local collider = lib.CreateObjectColliderByType(eColliderType.Circle)
		lib.SetObjectColliderSize(collider,80,80)
		lib.SetObjectColliderToPos(collider,2000,2000)
		lib.SetObjectColliderColliderGroup(collider,eColliderGroup.PlayerBullet)
		--
		if coroutine.yield(80)==false then return end
		for _=1,Infinite do
			--等待圈形子弹发射
			--if coroutine.yield(80)==false then return end
			local posX,posY = lib.GetGlobalVector2("SC1CircleCache")
			local angle = lib.GetAimToPlayerAngle(posX,posY)
			local vx = 2.5 * math.cos(math.rad(angle))
			local vy = 2.5 * math.sin(math.rad(angle))
			for _=0,211 do
				posX = posX + vx
				posY = posY + vy
				lib.SetGlobalVector2("SC1CircleCenter0",posX,posY)
				lib.SetEffectToPos(spriteEffect,posX,posY)
				lib.SetObjectColliderToPos(collider,posX,posY)
				if coroutine.yield(1)==false then return end
			end
		end
	end)
	--圈形防护罩1
	lib.AddEnemyTask(boss,function()
		local spriteEffect = lib.CreateSpriteEffectWithProps("STGEffectAtlas","TransparentCircle",eBlendMode.Normal,eEffectLayer.Bottom,false,0)
		lib.SetEffectToPos(spriteEffect,2000,2000)
		lib.SetSpriteEffectScale(spriteEffect,5,5)
		lib.SetSpriteEffectColor(spriteEffect,0.55,0.45,0.65,0.75)
		lib.SetGlobalUserData("SC1Effect1",spriteEffect)
		--ObjectCollider
		local collider = lib.CreateObjectColliderByType(eColliderType.Circle)
		lib.SetObjectColliderSize(collider,80,80)
		lib.SetObjectColliderToPos(collider,2000,2000)
		lib.SetObjectColliderColliderGroup(collider,eColliderGroup.PlayerBullet)
		--
		if coroutine.yield(186)==false then return end
		for _=1,Infinite do
			--等待圈形子弹发射
			--if coroutine.yield(80)==false then return end
			local posX,posY = lib.GetGlobalVector2("SC1CircleCache")
			local angle = lib.GetAimToPlayerAngle(posX,posY)
			local vx = 2.5 * math.cos(math.rad(angle))
			local vy = 2.5 * math.sin(math.rad(angle))
			for _=0,211 do
				posX = posX + vx
				posY = posY + vy
				lib.SetGlobalVector2("SC1CircleCenter1",posX,posY)
				lib.SetEffectToPos(spriteEffect,posX,posY)
				lib.SetObjectColliderToPos(collider,posX,posY)
				if coroutine.yield(1)==false then return end
			end
		end
	end)
	--使魔的task
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(300)==false then return end
		for _=1,Infinite do
			local hpRate = lib.GetBossSpellCardHpRate(0)
			local timeLeftRate = lib.GetSpellCardTimeLeftRate()
			local duration = 120 - math.ceil(60*math.min(hpRate,timeLeftRate))
			local posX,posY = lib.GetEnemyPos(boss)
			local enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0998",posX,posY,-170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0998",posX,posY,170,205,duration,3)
			if coroutine.yield(180)==false then return end
		end
	end)
	--移动的task
	lib.AddEnemyTask(boss,function()
		lib.SetEnemyWanderRange(boss,-150,150,80,125)
		lib.SetEnemyWanderAmplitude(boss,-100,100,-15,15)
		lib.SetEnemyWanderMode(boss,Constants.ModeLinear,Constants.DirModeMoveRandom)
		if coroutine.yield(300)==false then return end
		for _=0,Infinite do
			lib.EnemyDoWander(boss,120)
			if coroutine.yield(420)==false then return end
		end
	end)
end

function SC.NazrinSC1_0(boss)
	lib.SetSpellCardProperties("Easy-SpellCard",60,Condition.EliminateAll,true,function()
		local effect = lib.GetGlobalUserData("SC1Effect0")
		lib.SetEffectFinish(effect)
		lib.RemoveGlobalUserData("SC1Effect0")
		effect = lib.GetGlobalUserData("SC1Effect1")
		lib.SetEffectFinish(effect)
		lib.RemoveGlobalUserData("SC1Effect1")
	end)
	--bossCG
	do
		local tweenList = {}
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=0,duration=30,beginValue={x=200,y=200},endValue={x=0,y=0},mode=Constants.ModeEaseInQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=60,duration=60,beginValue={x=0,y=0},endValue={x=-200,y=-200},mode=Constants.ModeEaseOutQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=0,duration=0,beginValue=1,endValue=1,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=90,duration=30,beginValue=1,endValue=0,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Scale,delay=0,duration=0,beginValue={x=0.75,y=0.75,z=1},endValue={x=0.75,y=0.75,z=1},mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		lib.PlayCharacterCG("CG/face04ct",tweenList)
	end
	lib.SetBossInvincible(boss,5)
	lib.SetEnemyMaxHp(boss,500)
	lib.ShowBossBloodBar(boss,true)
	--圈形子弹的task
	lib.AddEnemyTask(boss,function()
		local k = 1
		for _=1,Infinite do
			local i
			local posX,posY = lib.GetEnemyPos(boss)
			lib.SetGlobalVector2("SC1CircleCache",posX,posY)
			for i=0,15 do
				local bullet = lib.CreateCustomizedBullet("NazrinSC1Bullet0","113020",posX,posY,i,75,2)
				lib.SetBulletCurvePara(bullet,0,23.5*i*k,0,1*k)
				if coroutine.yield(1)==false then return end
			end
			k = -k
			if coroutine.yield(90)==false then return end
		end
	end)
	--圈形防护罩0
	lib.AddEnemyTask(boss,function()
		local spriteEffect = lib.CreateSpriteEffectWithProps("STGEffectAtlas","TransparentCircle",eBlendMode.Normal,eEffectLayer.Bottom,false,0)
		lib.SetEffectToPos(spriteEffect,2000,2000)
		lib.SetSpriteEffectScale(spriteEffect,5,5)
		lib.SetSpriteEffectColor(spriteEffect,0.55,0.45,0.65,0.75)
		lib.SetGlobalUserData("SC1Effect0",spriteEffect)
		if coroutine.yield(80)==false then return end
		for _=1,Infinite do
			--等待圈形子弹发射
			--if coroutine.yield(80)==false then return end
			local posX,posY = lib.GetGlobalVector2("SC1CircleCache")
			local angle = lib.GetAimToPlayerAngle(posX,posY)
			local vx = 2.5 * math.cos(math.rad(angle))
			local vy = 2.5 * math.sin(math.rad(angle))
			for _=0,211 do
				posX = posX + vx
				posY = posY + vy
				lib.SetGlobalVector2("SC1CircleCenter0",posX,posY)
				lib.SetEffectToPos(spriteEffect,posX,posY)
				if coroutine.yield(1)==false then return end
			end
		end
	end)
	--圈形防护罩1
	lib.AddEnemyTask(boss,function()
		local spriteEffect = lib.CreateSpriteEffectWithProps("STGEffectAtlas","TransparentCircle",eBlendMode.Normal,eEffectLayer.Bottom,false,0)
		lib.SetEffectToPos(spriteEffect,2000,2000)
		lib.SetSpriteEffectScale(spriteEffect,5,5)
		lib.SetSpriteEffectColor(spriteEffect,0.55,0.45,0.65,0.75)
		lib.SetGlobalUserData("SC1Effect1",spriteEffect)
		if coroutine.yield(186)==false then return end
		for _=1,Infinite do
			--等待圈形子弹发射
			--if coroutine.yield(80)==false then return end
			local posX,posY = lib.GetGlobalVector2("SC1CircleCache")
			local angle = lib.GetAimToPlayerAngle(posX,posY)
			local vx = 2.5 * math.cos(math.rad(angle))
			local vy = 2.5 * math.sin(math.rad(angle))
			for _=0,211 do
				posX = posX + vx
				posY = posY + vy
				lib.SetGlobalVector2("SC1CircleCenter1",posX,posY)
				lib.SetEffectToPos(spriteEffect,posX,posY)
				if coroutine.yield(1)==false then return end
			end
		end
	end)
	--使魔的task
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(300)==false then return end
		for _=1,Infinite do
			local hpRate = lib.GetBossSpellCardHpRate(0)
			local timeLeftRate = lib.GetSpellCardTimeLeftRate()
			local duration = 120 - math.ceil(60*math.min(hpRate,timeLeftRate))
			local posX,posY = lib.GetEnemyPos(boss)
			local enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0020",posX,posY,-170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0020",posX,posY,170,205,duration,3)
			if coroutine.yield(180)==false then return end
		end
	end)
	--移动的task
	lib.AddEnemyTask(boss,function()
		lib.SetEnemyWanderRange(boss,-150,150,80,125)
		lib.SetEnemyWanderAmplitude(boss,-100,100,-15,15)
		lib.SetEnemyWanderMode(boss,Constants.ModeLinear,Constants.DirModeMoveRandom)
		if coroutine.yield(300)==false then return end
		for _=0,Infinite do
			lib.EnemyDoWander(boss,120)
			if coroutine.yield(420)==false then return end
		end
	end)
end

function SC.NazrinSC1_1(boss)
	lib.SetSpellCardProperties("Normal-SpellCard",60,Condition.EliminateAll,true,function()
		local effect = lib.GetGlobalUserData("SC1Effect0")
		lib.SetEffectFinish(effect)
		lib.RemoveGlobalUserData("SC1Effect0")
		effect = lib.GetGlobalUserData("SC1Effect1")
		lib.SetEffectFinish(effect)
		lib.RemoveGlobalUserData("SC1Effect1")
	end)
	--bossCG
	do
		local tweenList = {}
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=0,duration=30,beginValue={x=200,y=200},endValue={x=0,y=0},mode=Constants.ModeEaseInQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=60,duration=60,beginValue={x=0,y=0},endValue={x=-200,y=-200},mode=Constants.ModeEaseOutQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=0,duration=0,beginValue=1,endValue=1,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=90,duration=30,beginValue=1,endValue=0,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Scale,delay=0,duration=0,beginValue={x=0.75,y=0.75,z=1},endValue={x=0.75,y=0.75,z=1},mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		lib.PlayCharacterCG("CG/face04ct",tweenList)
	end
	lib.SetBossInvincible(boss,5)
	lib.SetEnemyMaxHp(boss,500)
	lib.ShowBossBloodBar(boss,true)
	--圈形子弹的task
	lib.AddEnemyTask(boss,function()
		local k = 1
		for _=1,Infinite do
			local i
			local posX,posY = lib.GetEnemyPos(boss)
			lib.SetGlobalVector2("SC1CircleCache",posX,posY)
			for i=0,15 do
				local bullet = lib.CreateCustomizedBullet("NazrinSC1Bullet0","113020",posX,posY,i,75,2)
				lib.SetBulletCurvePara(bullet,0,23.5*i*k,0,1*k)
				if coroutine.yield(1)==false then return end
			end
			k = -k
			if coroutine.yield(90)==false then return end
		end
	end)
	--圈形防护罩0
	lib.AddEnemyTask(boss,function()
		local spriteEffect = lib.CreateSpriteEffectWithProps("STGEffectAtlas","TransparentCircle",eBlendMode.Normal,eEffectLayer.Bottom,false,0)
		lib.SetEffectToPos(spriteEffect,2000,2000)
		lib.SetSpriteEffectScale(spriteEffect,5,5)
		lib.SetSpriteEffectColor(spriteEffect,0.55,0.45,0.65,0.75)
		lib.SetGlobalUserData("SC1Effect0",spriteEffect)
		if coroutine.yield(80)==false then return end
		for _=1,Infinite do
			--等待圈形子弹发射
			--if coroutine.yield(80)==false then return end
			local posX,posY = lib.GetGlobalVector2("SC1CircleCache")
			local angle = lib.GetAimToPlayerAngle(posX,posY)
			local vx = 2.5 * math.cos(math.rad(angle))
			local vy = 2.5 * math.sin(math.rad(angle))
			for _=0,211 do
				posX = posX + vx
				posY = posY + vy
				lib.SetGlobalVector2("SC1CircleCenter0",posX,posY)
				lib.SetEffectToPos(spriteEffect,posX,posY)
				if coroutine.yield(1)==false then return end
			end
		end
	end)
	--圈形防护罩1
	lib.AddEnemyTask(boss,function()
		local spriteEffect = lib.CreateSpriteEffectWithProps("STGEffectAtlas","TransparentCircle",eBlendMode.Normal,eEffectLayer.Bottom,false,0)
		lib.SetEffectToPos(spriteEffect,2000,2000)
		lib.SetSpriteEffectScale(spriteEffect,5,5)
		lib.SetSpriteEffectColor(spriteEffect,0.55,0.45,0.65,0.75)
		lib.SetGlobalUserData("SC1Effect1",spriteEffect)
		if coroutine.yield(186)==false then return end
		for _=1,Infinite do
			--等待圈形子弹发射
			--if coroutine.yield(80)==false then return end
			local posX,posY = lib.GetGlobalVector2("SC1CircleCache")
			local angle = lib.GetAimToPlayerAngle(posX,posY)
			local vx = 2.5 * math.cos(math.rad(angle))
			local vy = 2.5 * math.sin(math.rad(angle))
			for _=0,211 do
				posX = posX + vx
				posY = posY + vy
				lib.SetGlobalVector2("SC1CircleCenter1",posX,posY)
				lib.SetEffectToPos(spriteEffect,posX,posY)
				if coroutine.yield(1)==false then return end
			end
		end
	end)
	--使魔的task
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(300)==false then return end
		for _=1,Infinite do
			local hpRate = lib.GetBossSpellCardHpRate(0)
			local timeLeftRate = lib.GetSpellCardTimeLeftRate()
			local duration = 120 - math.ceil(60*math.min(hpRate,timeLeftRate))
			local posX,posY = lib.GetEnemyPos(boss)
			local enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0020",posX,posY,-170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0020",posX,posY,170,205,duration,3)
			if coroutine.yield(180)==false then return end
			posX,posY = lib.GetEnemyPos(boss)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0020",posX,posY,-170,-205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0020",posX,posY,170,-205,duration,3)
			if coroutine.yield(180)==false then return end
		end
	end)
	--移动的task
	lib.AddEnemyTask(boss,function()
		lib.SetEnemyWanderRange(boss,-150,150,80,125)
		lib.SetEnemyWanderAmplitude(boss,-100,100,-15,15)
		lib.SetEnemyWanderMode(boss,Constants.ModeLinear,Constants.DirModeMoveRandom)
		if coroutine.yield(300)==false then return end
		for _=0,Infinite do
			lib.EnemyDoWander(boss,120)
			if coroutine.yield(420)==false then return end
		end
	end)
end

function SC.NazrinSC1_2(boss)
	lib.SetSpellCardProperties("Hard-SpellCard",60,Condition.EliminateAll,true,function()
		local effect = lib.GetGlobalUserData("SC1Effect0")
		lib.SetEffectFinish(effect)
		lib.RemoveGlobalUserData("SC1Effect0")
		effect = lib.GetGlobalUserData("SC1Effect1")
		lib.SetEffectFinish(effect)
		lib.RemoveGlobalUserData("SC1Effect1")
	end)
	--bossCG
	do
		local tweenList = {}
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=0,duration=30,beginValue={x=200,y=200},endValue={x=0,y=0},mode=Constants.ModeEaseInQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=60,duration=60,beginValue={x=0,y=0},endValue={x=-200,y=-200},mode=Constants.ModeEaseOutQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=0,duration=0,beginValue=1,endValue=1,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=90,duration=30,beginValue=1,endValue=0,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Scale,delay=0,duration=0,beginValue={x=0.75,y=0.75,z=1},endValue={x=0.75,y=0.75,z=1},mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		lib.PlayCharacterCG("CG/face04ct",tweenList)
	end
	lib.SetBossInvincible(boss,5)
	lib.SetEnemyMaxHp(boss,500)
	lib.ShowBossBloodBar(boss,true)
	--圈形子弹的task
	lib.AddEnemyTask(boss,function()
		local k = 1
		for _=1,Infinite do
			local i
			local posX,posY = lib.GetEnemyPos(boss)
			lib.SetGlobalVector2("SC1CircleCache",posX,posY)
			for i=0,15 do
				local bullet = lib.CreateCustomizedBullet("NazrinSC1Bullet0","113020",posX,posY,i,75,2)
				lib.SetBulletCurvePara(bullet,0,23.5*i*k,0,1*k)
				if coroutine.yield(1)==false then return end
			end
			k = -k
			if coroutine.yield(90)==false then return end
		end
	end)
	--圈形防护罩0
	lib.AddEnemyTask(boss,function()
		local spriteEffect = lib.CreateSpriteEffectWithProps("STGEffectAtlas","TransparentCircle",eBlendMode.Normal,eEffectLayer.Bottom,false,0)
		lib.SetEffectToPos(spriteEffect,2000,2000)
		lib.SetSpriteEffectScale(spriteEffect,5,5)
		lib.SetSpriteEffectColor(spriteEffect,0.55,0.45,0.65,0.75)
		lib.SetGlobalUserData("SC1Effect0",spriteEffect)
		if coroutine.yield(80)==false then return end
		for _=1,Infinite do
			--等待圈形子弹发射
			--if coroutine.yield(80)==false then return end
			local posX,posY = lib.GetGlobalVector2("SC1CircleCache")
			local angle = lib.GetAimToPlayerAngle(posX,posY)
			local vx = 2.5 * math.cos(math.rad(angle))
			local vy = 2.5 * math.sin(math.rad(angle))
			for _=0,211 do
				posX = posX + vx
				posY = posY + vy
				lib.SetGlobalVector2("SC1CircleCenter0",posX,posY)
				lib.SetEffectToPos(spriteEffect,posX,posY)
				if coroutine.yield(1)==false then return end
			end
		end
	end)
	--圈形防护罩1
	lib.AddEnemyTask(boss,function()
		local spriteEffect = lib.CreateSpriteEffectWithProps("STGEffectAtlas","TransparentCircle",eBlendMode.Normal,eEffectLayer.Bottom,false,0)
		lib.SetEffectToPos(spriteEffect,2000,2000)
		lib.SetSpriteEffectScale(spriteEffect,5,5)
		lib.SetSpriteEffectColor(spriteEffect,0.55,0.45,0.65,0.75)
		lib.SetGlobalUserData("SC1Effect1",spriteEffect)
		if coroutine.yield(186)==false then return end
		for _=1,Infinite do
			--等待圈形子弹发射
			--if coroutine.yield(80)==false then return end
			local posX,posY = lib.GetGlobalVector2("SC1CircleCache")
			local angle = lib.GetAimToPlayerAngle(posX,posY)
			local vx = 2.5 * math.cos(math.rad(angle))
			local vy = 2.5 * math.sin(math.rad(angle))
			for _=0,211 do
				posX = posX + vx
				posY = posY + vy
				lib.SetGlobalVector2("SC1CircleCenter1",posX,posY)
				lib.SetEffectToPos(spriteEffect,posX,posY)
				if coroutine.yield(1)==false then return end
			end
		end
	end)
	--使魔的task
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(300)==false then return end
		for _=1,Infinite do
			local hpRate = lib.GetBossSpellCardHpRate(0)
			local timeLeftRate = lib.GetSpellCardTimeLeftRate()
			local duration = 120 - math.ceil(60*math.min(hpRate,timeLeftRate))
			local posX,posY = lib.GetEnemyPos(boss)
			local enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0020",posX,posY,-170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0020",posX,posY,170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0020",posX,posY,-170,-205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0020",posX,posY,170,-205,duration,3)
			if coroutine.yield(180)==false then return end
		end
	end)
	--移动的task
	lib.AddEnemyTask(boss,function()
		lib.SetEnemyWanderRange(boss,-150,150,80,125)
		lib.SetEnemyWanderAmplitude(boss,-100,100,-15,15)
		lib.SetEnemyWanderMode(boss,Constants.ModeLinear,Constants.DirModeMoveRandom)
		if coroutine.yield(300)==false then return end
		for _=0,Infinite do
			lib.EnemyDoWander(boss,120)
			if coroutine.yield(420)==false then return end
		end
	end)
end

function SC.NazrinSC1_3(boss)
	lib.SetSpellCardProperties("Lunatic-SpellCard",60,Condition.EliminateAll,true,function()
		local effect = lib.GetGlobalUserData("SC1Effect0")
		lib.SetEffectFinish(effect)
		lib.RemoveGlobalUserData("SC1Effect0")
		effect = lib.GetGlobalUserData("SC1Effect1")
		lib.SetEffectFinish(effect)
		lib.RemoveGlobalUserData("SC1Effect1")
	end)
	--bossCG
	do
		local tweenList = {}
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=0,duration=30,beginValue={x=200,y=200},endValue={x=0,y=0},mode=Constants.ModeEaseInQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=60,duration=60,beginValue={x=0,y=0},endValue={x=-200,y=-200},mode=Constants.ModeEaseOutQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=0,duration=0,beginValue=1,endValue=1,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=90,duration=30,beginValue=1,endValue=0,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Scale,delay=0,duration=0,beginValue={x=0.75,y=0.75,z=1},endValue={x=0.75,y=0.75,z=1},mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		lib.PlayCharacterCG("CG/face04ct",tweenList)
	end
	lib.SetBossInvincible(boss,5)
	lib.SetEnemyMaxHp(boss,500)
	lib.ShowBossBloodBar(boss,true)
	--圈形子弹的task
	lib.AddEnemyTask(boss,function()
		local k = 1
		for _=1,Infinite do
			local i
			local posX,posY = lib.GetEnemyPos(boss)
			lib.SetGlobalVector2("SC1CircleCache",posX,posY)
			for i=0,15 do
				local bullet = lib.CreateCustomizedBullet("NazrinSC1Bullet0","113020",posX,posY,i,75,2)
				lib.SetBulletCurvePara(bullet,0,23.5*i*k,0,1*k)
				if coroutine.yield(1)==false then return end
			end
			k = -k
			if coroutine.yield(90)==false then return end
		end
	end)
	--圈形防护罩0
	lib.AddEnemyTask(boss,function()
		local spriteEffect = lib.CreateSpriteEffectWithProps("STGEffectAtlas","TransparentCircle",eBlendMode.Normal,eEffectLayer.Bottom,false,0)
		lib.SetEffectToPos(spriteEffect,2000,2000)
		lib.SetSpriteEffectScale(spriteEffect,5,5)
		lib.SetSpriteEffectColor(spriteEffect,0.55,0.45,0.65,0.75)
		lib.SetGlobalUserData("SC1Effect0",spriteEffect)
		if coroutine.yield(80)==false then return end
		for _=1,Infinite do
			--等待圈形子弹发射
			--if coroutine.yield(80)==false then return end
			local posX,posY = lib.GetGlobalVector2("SC1CircleCache")
			local angle = lib.GetAimToPlayerAngle(posX,posY)
			local vx = 2.5 * math.cos(math.rad(angle))
			local vy = 2.5 * math.sin(math.rad(angle))
			for _=0,211 do
				posX = posX + vx
				posY = posY + vy
				lib.SetGlobalVector2("SC1CircleCenter0",posX,posY)
				lib.SetEffectToPos(spriteEffect,posX,posY)
				if coroutine.yield(1)==false then return end
			end
		end
	end)
	--圈形防护罩1
	lib.AddEnemyTask(boss,function()
		local spriteEffect = lib.CreateSpriteEffectWithProps("STGEffectAtlas","TransparentCircle",eBlendMode.Normal,eEffectLayer.Bottom,false,0)
		lib.SetEffectToPos(spriteEffect,2000,2000)
		lib.SetSpriteEffectScale(spriteEffect,5,5)
		lib.SetSpriteEffectColor(spriteEffect,0.55,0.45,0.65,0.75)
		lib.SetGlobalUserData("SC1Effect1",spriteEffect)
		if coroutine.yield(186)==false then return end
		for _=1,Infinite do
			--等待圈形子弹发射
			--if coroutine.yield(80)==false then return end
			local posX,posY = lib.GetGlobalVector2("SC1CircleCache")
			local angle = lib.GetAimToPlayerAngle(posX,posY)
			local vx = 2.5 * math.cos(math.rad(angle))
			local vy = 2.5 * math.sin(math.rad(angle))
			for _=0,211 do
				posX = posX + vx
				posY = posY + vy
				lib.SetGlobalVector2("SC1CircleCenter1",posX,posY)
				lib.SetEffectToPos(spriteEffect,posX,posY)
				if coroutine.yield(1)==false then return end
			end
		end
	end)
	--使魔的task
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(300)==false then return end
		for _=1,Infinite do
			local hpRate = lib.GetBossSpellCardHpRate(0)
			local timeLeftRate = lib.GetSpellCardTimeLeftRate()
			local duration = 120 - math.ceil(60*math.min(hpRate,timeLeftRate))
			local posX,posY = lib.GetEnemyPos(boss)
			local enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0020",posX,posY,-170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0020",posX,posY,170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0020",posX,posY,-170,-205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0020",posX,posY,170,-205,duration,3)
			if coroutine.yield(180)==false then return end
		end
	end)
	--移动的task
	lib.AddEnemyTask(boss,function()
		lib.SetEnemyWanderRange(boss,-150,150,80,125)
		lib.SetEnemyWanderAmplitude(boss,-100,100,-15,15)
		lib.SetEnemyWanderMode(boss,Constants.ModeLinear,Constants.DirModeMoveRandom)
		if coroutine.yield(300)==false then return end
		for _=0,Infinite do
			lib.EnemyDoWander(boss,120)
			if coroutine.yield(420)==false then return end
		end
	end)
end

CustomizedBulletTable.NazrinSC2Laser = {}
CustomizedBulletTable.NazrinSC2Laser.Init = function(laser,posX,posY,angle,existDuration,createRandomBulletInterval)
	--lib.SetBulletResistEliminatedFlag(bullet,EliminateType.PlayerSpellCard + EliminateType.PlayerDead + EliminateType.HitPlayer)
	lib.SetBulletTexture(laser,"201060")
	lib.SetLaserProps(laser,posX,posY,angle,2,500,150)
	lib.SetBulletDetectCollision(laser,false)
	lib.SetLaserCollisionFactor(laser,0.8)
	lib.AddBulletTask(laser,function()
		lib.ChangeLaserWidth(laser,16,10,30)
		lib.SetBulletDetectCollision(laser,true)
		if coroutine.yield(30) == false then return end
		if coroutine.yield(existDuration) == false then return end
		lib.SetBulletDetectCollision(laser,false)
		lib.ChangeLaserWidth(laser,0,10,0)
		if coroutine.yield(10) == false then return end
		lib.EliminateBullet(laser)
	end)
	if createRandomBulletInterval ~= 0 then
		lib.AddBulletTask(laser,function()
			if coroutine.yield(existDuration+30) == false then return end
			local createBulletCount = math.floor(500/createRandomBulletInterval)
			local normalizedX = math.cos(math.rad(angle))
			local normalizedY = math.sin(math.rad(angle))
			for i=1,createBulletCount do
				local x = normalizedX * i * createRandomBulletInterval + posX
				local y = normalizedY * i * createRandomBulletInterval + posY
				local a = lib.GetRandomInt(0,360)
				local bullet = lib.CreateSimpleBulletById("104060",x,y)
				lib.SetBulletOrderInLayer(bullet,1)
				lib.DoBulletAccelerationWithLimitation(bullet,0.05,a,3)
				--lib.SetBulletStraightParas(bullet,3,a,false,0,0)
			end
		end)
	end
end

CustomizedBulletTable.NazrinSC2Spark = {}
CustomizedBulletTable.NazrinSC2Spark.Init = function(laser,posX,posY,canRotate)
	lib.SetBulletTexture(laser,"201060")
	local laserAngle = lib.GetAimToPlayerAngle(posX,posY)
	lib.SetLaserProps(laser,posX,posY,laserAngle,2,0,500)
	lib.SetBulletDetectCollision(laser,false)
	lib.SetBulletResistEliminatedFlag(laser,EliminateType.PlayerSpellCard + EliminateType.PlayerDead + EliminateType.HitPlayer)
	lib.SetLaserCollisionFactor(laser,0.8)
	lib.AddBulletTask(laser,function()
		lib.ChangeLaserHeight(laser,500,30,0)
		if coroutine.yield(60) == false then return end
		lib.ChangeLaserWidth(laser,128,10,0)
		lib.SetBulletDetectCollision(laser,true)
		if coroutine.yield(60) == false then return end
		lib.SetBulletDetectCollision(laser,false)
		lib.ChangeLaserWidth(laser,0,10,0)
		if coroutine.yield(10) == false then return end
		lib.EliminateBullet(laser)
	end)
	--转向玩家的方向
	if canRotate then
		lib.AddBulletTask(laser,function()
			local prePlayerPosX,prePlayerPosY = lib.GetPlayerPos()
			if coroutine.yield(70) == false then return end
			local nowPlayerPosX,nowPlayerPosY = lib.GetPlayerPos()
			local omega = prePlayerPosX > nowPlayerPosX and -0.2 or 0.2
			lib.SetLaserRotateParaWithOmega(laser,omega,60)
		end)
	end
end

function SC.NazrinSC2_0(boss)
	lib.SetSpellCardProperties("LaserSign-Easy",60,Condition.EliminateAll,true,nil)
	--bossCG
	do
		local tweenList = {}
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=0,duration=30,beginValue={x=200,y=200},endValue={x=0,y=0},mode=Constants.ModeEaseInQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=60,duration=60,beginValue={x=0,y=0},endValue={x=-200,y=-200},mode=Constants.ModeEaseOutQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=0,duration=0,beginValue=1,endValue=1,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=90,duration=30,beginValue=1,endValue=0,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Scale,delay=0,duration=0,beginValue={x=0.75,y=0.75,z=1},endValue={x=0.75,y=0.75,z=1},mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		lib.PlayCharacterCG("CG/face04ct",tweenList)
	end
	lib.SetBossInvincible(boss,5)
	lib.SetEnemyMaxHp(boss,500)
	lib.ShowBossBloodBar(boss,true)
	--散射激光部分
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(60) == false then return end
		for _=1,Infinite do
			local posX,posY = lib.GetEnemyPos(boss)
			local playerAngle = lib.GetAimToPlayerAngle(posX,posY)
			for i=1,20 do
				local laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle+26.25+i*15,93-i*3,0,5)
				laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle-26.25-i*15,93-i*3,0,5)
				if coroutine.yield(3) == false then return end
			end
			if coroutine.yield(120) == false then return end
		end
	end)
	--自机狙激光
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(60) == false then return end
		if coroutine.yield(60) == false then return end
		for _=1,Infinite do
			local laserPosX,laserPosY = lib.GetEnemyPos(boss)
			local laser = lib.CreateCustomizedLaser("NazrinSC2Spark",laserPosX,laserPosY,false,3)
			if coroutine.yield(180) == false then return end
		end
	end)
end

function SC.NazrinSC2_1(boss)
	lib.SetSpellCardProperties("LaserSign-Normal",60,Condition.EliminateAll,true,nil)
	--bossCG
	do
		local tweenList = {}
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=0,duration=30,beginValue={x=200,y=200},endValue={x=0,y=0},mode=Constants.ModeEaseInQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=60,duration=60,beginValue={x=0,y=0},endValue={x=-200,y=-200},mode=Constants.ModeEaseOutQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=0,duration=0,beginValue=1,endValue=1,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=90,duration=30,beginValue=1,endValue=0,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Scale,delay=0,duration=0,beginValue={x=0.75,y=0.75,z=1},endValue={x=0.75,y=0.75,z=1},mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		lib.PlayCharacterCG("CG/face04ct",tweenList)
	end
	lib.SetBossInvincible(boss,5)
	lib.SetEnemyMaxHp(boss,500)
	lib.ShowBossBloodBar(boss,true)
	--散射激光部分
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(120) == false then return end
		for _=1,Infinite do
			local posX,posY = lib.GetEnemyPos(boss)
			local playerAngle = lib.GetAimToPlayerAngle(posX,posY)
			for i=1,20 do
				local laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle+26.25+i*15,93-i*3,50,5)
				laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle-26.25-i*15,93-i*3,50,5)
				if coroutine.yield(3) == false then return end
			end
			if coroutine.yield(120) == false then return end
		end
	end)
	--自机狙激光
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(120) == false then return end
		if coroutine.yield(60) == false then return end
		for _=1,Infinite do
			local laserPosX,laserPosY = lib.GetEnemyPos(boss)
			local laser = lib.CreateCustomizedLaser("NazrinSC2Spark",laserPosX,laserPosY,false,3)
			if coroutine.yield(180) == false then return end
		end
	end)
end

function SC.NazrinSC2_2(boss)
	lib.SetSpellCardProperties("LaserSign-Hard",60,Condition.EliminateAll,true,nil)
	--bossCG
	do
		local tweenList = {}
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=0,duration=30,beginValue={x=200,y=200},endValue={x=0,y=0},mode=Constants.ModeEaseInQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=60,duration=60,beginValue={x=0,y=0},endValue={x=-200,y=-200},mode=Constants.ModeEaseOutQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=0,duration=0,beginValue=1,endValue=1,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=90,duration=30,beginValue=1,endValue=0,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Scale,delay=0,duration=0,beginValue={x=0.75,y=0.75,z=1},endValue={x=0.75,y=0.75,z=1},mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		lib.PlayCharacterCG("CG/face04ct",tweenList)
	end
	lib.SetBossInvincible(boss,5)
	lib.SetEnemyMaxHp(boss,500)
	lib.ShowBossBloodBar(boss,true)
	--散射激光部分
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(120) == false then return end
		for _=1,Infinite do
			local posX,posY = lib.GetEnemyPos(boss)
			local playerAngle = lib.GetAimToPlayerAngle(posX,posY)
			for i=1,20 do
				local laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle+4.125+i*16.5,93-i*3,32,5)
				laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle-4.125-i*16.5,93-i*3,32,5)
				if coroutine.yield(3) == false then return end
			end
			if coroutine.yield(120) == false then return end
		end
	end)
	--自机狙激光
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(120) == false then return end
		if coroutine.yield(60) == false then return end
		for _=1,Infinite do
			local laserPosX,laserPosY = lib.GetEnemyPos(boss)
			local laser = lib.CreateCustomizedLaser("NazrinSC2Spark",laserPosX,laserPosY,false,3)
			if coroutine.yield(180) == false then return end
		end
	end)
end

function SC.NazrinSC2_3(boss)
	lib.SetSpellCardProperties("LaserSign-Lunatic",60,Condition.EliminateAll,true,nil)
	--bossCG
	do
		local tweenList = {}
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=0,duration=30,beginValue={x=200,y=200},endValue={x=0,y=0},mode=Constants.ModeEaseInQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=60,duration=60,beginValue={x=0,y=0},endValue={x=-200,y=-200},mode=Constants.ModeEaseOutQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=0,duration=0,beginValue=1,endValue=1,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=90,duration=30,beginValue=1,endValue=0,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Scale,delay=0,duration=0,beginValue={x=0.75,y=0.75,z=1},endValue={x=0.75,y=0.75,z=1},mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		lib.PlayCharacterCG("CG/face04ct",tweenList)
	end
	lib.SetBossInvincible(boss,5)
	lib.SetEnemyMaxHp(boss,500)
	lib.ShowBossBloodBar(boss,true)
	--散射激光部分
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(120) == false then return end
		for _=1,Infinite do
			local posX,posY = lib.GetEnemyPos(boss)
			local playerAngle = lib.GetAimToPlayerAngle(posX,posY)
			for i=1,20 do
				local laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle+4.125+i*16.5,93-i*3,32,5)
				laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle-4.125-i*16.5,93-i*3,32,5)
				if coroutine.yield(3) == false then return end
			end
			if coroutine.yield(120) == false then return end
		end
	end)
	--自机狙激光
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(120) == false then return end
		if coroutine.yield(60) == false then return end
		for _=1,Infinite do
			local laserPosX,laserPosY = lib.GetEnemyPos(boss)
			local laser = lib.CreateCustomizedLaser("NazrinSC2Spark",laserPosX,laserPosY,true,3)
			if coroutine.yield(180) == false then return end
		end
	end)
end

CustomizedBulletTable.WriggleBullet = {}
CustomizedBulletTable.WriggleBullet.Init = function(bullet,velocity,angle,waitTime,finalStyleId)
	lib.CreateAppearEffectForSimpleBullet(bullet)
	lib.AddBulletTask(bullet,function()
		if coroutine.yield(150) == false then return end
		lib.ChangeBulletStyleById(bullet,"113120")
		if coroutine.yield(waitTime) == false then return end
		lib.ChangeBulletStyleById(bullet,"102120")
		lib.DoBulletAccelerationWithLimitation(bullet,0.05,angle,velocity*0.1)
		if coroutine.yield(90) == false then return end
		lib.ChangeBulletStyleById(bullet,finalStyleId)
		lib.DoBulletAccelerationWithLimitation(bullet,0.05,angle,velocity)
	end)
end

function SC.WriggleSC(boss)
	lib.SetSpellCardProperties("WriggleSign",60,Condition.EliminateAll,true,nil)
	lib.EnemyMoveToPos(boss,0,128,120,Constants.ModeEaseOutQuad)
	lib.SetBossInvincible(boss,5)
	lib.SetEnemyMaxHp(boss,1000)
	lib.ShowBossBloodBar(boss,true)
	--bossCG
	do
		local tweenList = {}
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=0,duration=30,beginValue={x=200,y=200},endValue={x=0,y=0},mode=Constants.ModeEaseInQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=60,duration=60,beginValue={x=0,y=0},endValue={x=-200,y=-200},mode=Constants.ModeEaseOutQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=0,duration=0,beginValue=1,endValue=1,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=90,duration=30,beginValue=1,endValue=0,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Scale,delay=0,duration=0,beginValue={x=0.75,y=0.75,z=1},endValue={x=0.75,y=0.75,z=1},mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		lib.PlayCharacterCG("CG/face04ct",tweenList)
	end
	if coroutine.yield(121) == false then return end
	lib.SetEnemyWanderRange(boss,-96,96,128,144)
	lib.SetEnemyWanderAmplitude(boss,-32,32,-32,32)
	lib.SetEnemyWanderMode(boss,Constants.ModeEaseOutQuad,Constants.DirModeMoveXTowardsPlayer)
	local i , di = 0 , 1
	local finalIds={"103021","103041","103061","103081","103101","103121","103141"}
	for _=1,Infinite do
		local startAngle = lib.GetRandomFloat(0,360)
		local dStartAngle = 0
		local bossPosX,bossPosY = lib.GetEnemyPos(boss)
		lib.CreateChargeEffect(bossPosX,bossPosY)
		if coroutine.yield(60) == false then return end
		do
			--曲线半径
			local radius,dRadius = 16,10
			local v,dv = 2,-0.03
			for _=1,45 do
				local angle,dAngle = startAngle,30
				local playerPosX,playerPosY = lib.GetPlayerPos()
				for _=1,12 do
					local posX = bossPosX + radius * math.cos(math.rad(angle))
					local posY = bossPosY + radius * math.sin(math.rad(angle))
					if lib.GetVectorLength(posX,posY,playerPosX,playerPosY) > 48 and math.abs(posX) < 320 and math.abs(posY) < 240 then
						lib.CreateCustomizedBullet("WriggleBullet","102100",posX,posY,v,angle+dAngle,30,finalIds[i%7+1],4)
					end
					angle = angle + dAngle
				end
				startAngle = startAngle + 350/radius*(-1)^i
				dStartAngle = dStartAngle + 23
				if coroutine.yield(2) == false then return end
				radius = radius + dRadius
				v = v + dv
			end
			if coroutine.yield(60) == false then return end
			lib.EnemyDoWander(boss,30)
			if coroutine.yield(60) == false then return end
			i = i + di
		end
	end
end

CustomizedBulletTable.OrionidsLaser = {}
CustomizedBulletTable.OrionidsLaser.Init = function(laser,posX,posY,angle,isAimToPlayer,styleId)
	lib.SetBulletStyleById(laser,styleId)
	if isAimToPlayer then 
		angle = angle + lib.GetAimToPlayerAngle(posX,posY)
	end
	lib.SetLaserProps(laser,posX,posY,angle,2,0,180)
	lib.SetBulletDetectCollision(laser,false)
	lib.ChangeLaserHeight(laser,900,50,0)
end

CustomizedBulletTable.OrionidsBullet0 = {}
CustomizedBulletTable.OrionidsBullet0.Init = function(bullet)
	lib.SetBulletStraightParas(bullet,0,270,false,0,0)
	lib.AddBulletTask(bullet,function()
		if coroutine.yield(30) == false then return end
		local alpha,dAlpha = 255,-5
		for _ = 1,52 do
			lib.SetBulletAlpha(bullet,alpha/255)
			alpha = alpha + dAlpha
			if coroutine.yield(1) == false then return end
		end
	end)
	lib.AddBulletTask(bullet,function()
		if coroutine.yield(41) == false then return end
		lib.SetBulletDetectCollision(bullet,false)
		if coroutine.yield(82) == false then return end
		lib.EliminateBullet(bullet)
	end)
end

CustomizedBulletTable.OrionidsBullet1 = {}
CustomizedBulletTable.OrionidsBullet1.Init = function(bullet,angle,interval,v)
	lib.AddBulletTask(bullet,function()
		local posX,posY
		for _=1,Infinite do
			posX,posY = lib.GetBulletPos(bullet)
			posX = posX + v / math.cos(math.rad(interval+1)) * math.cos(math.rad(angle+interval))
			posY = posY + v / math.cos(math.rad(interval+1)) * math.sin(math.rad(angle+interval))
			lib.SetBulletPos(bullet,posX,posY)
			if coroutine.yield(1) == false then return end
		end
	end)
end

CustomizedEnemyTable.OrionidsEnemy = {}
CustomizedEnemyTable.OrionidsEnemy.Init = function(enemy,startPosX,startPosY,angle,isAimToPlayer)
	lib.SetEnemyMaxHp(enemy,5)
	lib.SetEnemyInteractive(enemy,false)
	if isAimToPlayer then
		angle = angle + lib.GetAimToPlayerAngle(startPosX,startPosY)
	end
	lib.AddEnemyTask(enemy,function()
		if coroutine.yield(60) == false then return end
		lib.SetEnemyPos(enemy,startPosX,startPosY)
		lib.SetEnemyInteractive(enemy,true)
		lib.EnemyAccMoveTowardsWithLimitation(enemy,0,angle,0.15,10)
	end)
	lib.AddEnemyTask(enemy,function()
		if coroutine.yield(60) == false then return end
		for _=1,Infinite do
			local posX,posY = lib.GetEnemyPos(enemy)
			lib.CreateCustomizedBullet("OrionidsBullet0","125051",posX,posY,0)
			if coroutine.yield(4) == false then return end
		end
	end)
	lib.AddEnemyTask(enemy,function()
		for _=1,Infinite do
			local posX,posY = lib.GetEnemyPos(enemy)
			if posY <= -224 then
				local angle,dAngle = 0,20
				local bullet
				for _= 1,10 do
					bullet = lib.CreateSimpleBulletById("124031",posX,posY)
					lib.SetBulletStraightParas(bullet,1.5,angle+lib.GetRandomFloat(-3,3),false,0,0)
					angle = angle + dAngle
				end
				break
			end
			if coroutine.yield(1) == false then return end
		end
	end)
end

CustomizedEnemyTable.OrionidsEnemy.OnKill = function(enemy)
	local posX,posY = lib.GetEnemyPos(enemy)
	local angle,dAngle = 0,36
	local bullet
	for _= 1,10 do
		bullet = lib.CreateSimpleBulletById("124031",posX,posY)
		lib.SetBulletStraightParas(bullet,1.5,angle+lib.GetRandomFloat(-9,9),false,0,0)
		angle = angle + dAngle
	end
end

function SC.OrionidsSC(boss)
	lib.SetSpellCardProperties("Orionid Meteor Shower",60,Condition.EliminateAll,true,nil)
	--lib.EnemyMoveToPos(boss,0,128,120,Constants.ModeEaseOutQuad)
	lib.SetBossInvincible(boss,5)
	lib.SetEnemyMaxHp(boss,1200)
	lib.ShowBossBloodBar(boss,true)
	--bossCG
	do
		local tweenList = {}
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=0,duration=30,beginValue={x=200,y=200},endValue={x=0,y=0},mode=Constants.ModeEaseInQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Pos2D,delay=60,duration=60,beginValue={x=0,y=0},endValue={x=-200,y=-200},mode=Constants.ModeEaseOutQuad}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=0,duration=0,beginValue=1,endValue=1,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Alpha,delay=90,duration=30,beginValue=1,endValue=0,mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		do
			local tween = {type=Constants.eTweenType.Scale,delay=0,duration=0,beginValue={x=0.75,y=0.75,z=1},endValue={x=0.75,y=0.75,z=1},mode=Constants.ModeLinear}
			table.insert(tweenList,tween)
		end
		lib.PlayCharacterCG("CG/face04ct",tweenList)
	end
	lib.AddEnemyTask(boss,function()
		local bossPosX,bossPosY = lib.GetEnemyPos(boss)
		lib.CreateChargeEffect(bossPosX,bossPosY)
		if coroutine.yield(90) == false then return end
		for _=1,Infinite do
			do
				local angle,dAngle = 0,-11
				for _=1,25 do
					local laser = lib.CreateCustomizedLaser("OrionidsLaser",320*math.cos(math.rad(angle)),274+25*math.sin(math.rad(angle)),angle,false,"201060",5)
					local enemy = lib.CreateCustomizedEnemy("OrionidsEnemy","0022",0,300,320*math.cos(math.rad(angle)),274+25*math.sin(math.rad(angle)),angle,false,4)
					if coroutine.yield(4) == false then return end
					angle = angle + dAngle
				end
			end
			lib.CreateChargeEffect(bossPosX,bossPosY)
			if coroutine.yield(90) == false then return end
			do
				local angle,dAngle = 0,-20
				for _=1,9 do
					local angleOffset = lib.GetRandomFloat(-5,5)
					local laser = lib.CreateCustomizedLaser("OrionidsLaser",320*math.cos(math.rad(angle)),254+15*math.sin(math.rad(angle)),angleOffset,true,"201060",5)
					local enemy = lib.CreateCustomizedEnemy("OrionidsEnemy","0022",0,300,320*math.cos(math.rad(angle)),254+15*math.sin(math.rad(angle)),angleOffset,true,4)
					if coroutine.yield(5) == false then return end
					angle = angle + dAngle
				end
			end
			if coroutine.yield(180) == false then return end
		end 
	end)
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(360) == false then return end
		local bossPosX,bossPosY = lib.GetEnemyPos(boss)
		lib.CreateChargeEffect(bossPosX,bossPosY)
		if coroutine.yield(90) == false then return end
		do
			local angle,dAngle = 0,37
			for _=1,Infinite do
				local angle1,dAngle1 = angle,60
				for _=1,6 do
					local interval,dInterval = -30,10
					for _=1,7 do
						lib.CreateCustomizedBullet("OrionidsBullet1","111061",bossPosX,bossPosY,angle1,interval,1.5,3)
						interval = interval + dInterval
					end
					angle1 = angle1 + dAngle1
				end
				if coroutine.yield(60) == false then return end
				angle = angle + dAngle
			end
		end
	end)
end

return
{
	CustomizedBulletTable = CustomizedBulletTable,
	CustomizedEnemyTable = CustomizedEnemyTable,
}