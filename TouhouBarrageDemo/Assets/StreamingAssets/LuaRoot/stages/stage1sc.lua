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
		lib.AddBulletComponent(bullet,eBulletComponentType.ParasChange)
		lib.AddBulletParaChangeEvent(bullet,5,eParaChangeMode.ChangeTo,0,maxRadius,0,0,0,30,Constants.ModeLinear,1,0)
		--lib.AddBulletParaChangeEvent(bullet,8,Constants.ParaChangeMode_ChangeTo,1.5,50-waitTime,Constants.ModeLinear,10)
		if coroutine.yield(80-waitTime) == false then return end
		lib.SetBulletStyleById(bullet,"107060")
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

SC["NazrinTest"] = {}
SC["NazrinTest"].Init = function(boss)
	SetSpellCardProperties("Easy-SpellCard",60,Condition.EliminateAll,true)
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
		lib.SetObjectColliderColliderGroup(collider,Group_PlayerBullet)
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
			local enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,-170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,170,205,duration,3)
			if coroutine.yield(180)==false then return end
		end
	end)
	--移动的task
	lib.AddEnemyTask(boss,function()
		lib.SetEnemyWanderRange(boss,-150,150,80,125)
		lib.SetEnemyWanderAmplitude(boss,0,100,0,15)
		lib.SetEnemyWanderMode(boss,Constants.ModeLinear,Constants.DirModeMoveRandom)
		if coroutine.yield(300)==false then return end
		for _=0,Infinite do
			lib.EnemyDoWander(boss,120)
			if coroutine.yield(420)==false then return end
		end
	end)
end

SC["NazrinTest"].OnFinish = function(boss)
	local effect = lib.GetGlobalUserData("SC1Effect0")
	lib.SetEffectFinish(effect)
	lib.RemoveGlobalUserData("SC1Effect0")
	effect = lib.GetGlobalUserData("SC1Effect1")
	lib.SetEffectFinish(effect)
	lib.RemoveGlobalUserData("SC1Effect1")
end

SC["NazrinSC1_0"] = {}
SC["NazrinSC1_0"].Init = function(boss)
	lib.SetSpellCardProperties("Easy-SpellCard",60,Condition.EliminateAll,true)
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
			local enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,-170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,170,205,duration,3)
			if coroutine.yield(180)==false then return end
		end
	end)
	--移动的task
	lib.AddEnemyTask(boss,function()
		lib.SetEnemyWanderRange(boss,-150,150,80,125)
		lib.SetEnemyWanderAmplitude(boss,0,100,0,15)
		lib.SetEnemyWanderMode(boss,Constants.ModeLinear,Constants.DirModeMoveRandom)
		if coroutine.yield(300)==false then return end
		for _=0,Infinite do
			lib.EnemyDoWander(boss,120)
			if coroutine.yield(420)==false then return end
		end
	end)
end

SC["NazrinSC1_0"].OnFinish = function(boss)
	local effect = lib.GetGlobalUserData("SC1Effect0")
	lib.SetEffectFinish(effect)
	lib.RemoveGlobalUserData("SC1Effect0")
	effect = lib.GetGlobalUserData("SC1Effect1")
	lib.SetEffectFinish(effect)
	lib.RemoveGlobalUserData("SC1Effect1")
end

SC["NazrinSC1_1"] = {}
SC["NazrinSC1_1"].Init = function(boss)
	SetSpellCardProperties("Normal-SpellCard",60,Condition.EliminateAll,true)
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
			local enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,-170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,170,205,duration,3)
			if coroutine.yield(180)==false then return end
			posX,posY = lib.GetEnemyPos(boss)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,-170,-205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,170,-205,duration,3)
			if coroutine.yield(180)==false then return end
		end
	end)
	--移动的task
	lib.AddEnemyTask(boss,function()
		lib.SetEnemyWanderRange(boss,-150,150,80,125)
		lib.SetEnemyWanderAmplitude(boss,0,100,0,15)
		lib.SetEnemyWanderMode(boss,Constants.ModeLinear,Constants.DirModeMoveRandom)
		if coroutine.yield(300)==false then return end
		for _=0,Infinite do
			lib.EnemyDoWander(boss,120)
			if coroutine.yield(420)==false then return end
		end
	end)
end

SC["NazrinSC1_1"].OnFinish = function(boss)
	local effect = lib.GetGlobalUserData("SC1Effect0")
	lib.SetEffectFinish(effect)
	lib.RemoveGlobalUserData("SC1Effect0")
	effect = lib.GetGlobalUserData("SC1Effect1")
	lib.SetEffectFinish(effect)
	lib.RemoveGlobalUserData("SC1Effect1")
end

SC["NazrinSC1_2"] = {}
SC["NazrinSC1_2"].Init = function(boss)
	lib.SetSpellCardProperties("Hard-SpellCard",60,Condition.EliminateAll,true)
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
			local enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,-170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,-170,-205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,170,-205,duration,3)
			if coroutine.yield(180)==false then return end
		end
	end)
	--移动的task
	lib.AddEnemyTask(boss,function()
		lib.SetEnemyWanderRange(boss,-150,150,80,125)
		lib.SetEnemyWanderAmplitude(boss,0,100,0,15)
		lib.SetEnemyWanderMode(boss,Constants.ModeLinear,Constants.DirModeMoveRandom)
		if coroutine.yield(300)==false then return end
		for _=0,Infinite do
			lib.EnemyDoWander(boss,120)
			if coroutine.yield(420)==false then return end
		end
	end)
end

SC["NazrinSC1_2"].OnFinish = function(boss)
	local effect = lib.GetGlobalUserData("SC1Effect0")
	lib.SetEffectFinish(effect)
	lib.RemoveGlobalUserData("SC1Effect0")
	effect = lib.GetGlobalUserData("SC1Effect1")
	lib.SetEffectFinish(effect)
	lib.RemoveGlobalUserData("SC1Effect1")
end

SC["NazrinSC1_3"] = {}
SC["NazrinSC1_3"].Init = function(boss)
	SetSpellCardProperties("Lunatic-SpellCard",60,Condition.EliminateAll,true)
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
			local enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,-170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,-170,-205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","100020",posX,posY,170,-205,duration,3)
			if coroutine.yield(180)==false then return end
		end
	end)
	--移动的task
	lib.AddEnemyTask(boss,function()
		lib.SetEnemyWanderRange(boss,-150,150,80,125)
		lib.SetEnemyWanderAmplitude(boss,0,100,0,15)
		lib.SetEnemyWanderMode(boss,Constants.ModeLinear,Constants.DirModeMoveRandom)
		if coroutine.yield(300)==false then return end
		for _=0,Infinite do
			lib.EnemyDoWander(boss,120)
			if coroutine.yield(420)==false then return end
		end
	end)
end

SC["NazrinSC1_3"].OnFinish = function(boss)
	local effect = lib.GetGlobalUserData("SC1Effect0")
	lib.SetEffectFinish(effect)
	lib.RemoveGlobalUserData("SC1Effect0")
	effect = lib.GetGlobalUserData("SC1Effect1")
	lib.SetEffectFinish(effect)
	lib.RemoveGlobalUserData("SC1Effect1")
end

CustomizedBulletTable.NazrinSC2Laser = {}
CustomizedBulletTable.NazrinSC2Laser.Init = function(laser,angle,existDuration,createRandomBulletInterval)
	--lib.SetBulletResistEliminatedFlag(bullet,EliminateType.PlayerSpellCard + EliminateType.PlayerDead + EliminateType.HitPlayer)
	laser:SetStyleById(201060)
	laser.rot = angle
	laser:SetSize(500,2)
	laser:SetExistDuration(150)
	laser:TurnOn(0)
	lib.SetBulletDetectCollision(laser,false)
	lib.SetLaserCollisionFactor(laser,0.8)
	lib.AddBulletTask(laser,function()
		lib.ChangeLaserWidthTo(laser,16,30,10)
		lib.SetBulletDetectCollision(laser,true)
		if coroutine.yield(30) == false then return end
		if coroutine.yield(existDuration) == false then return end
		lib.SetBulletDetectCollision(laser,false)
		laser:TurnOff(10)
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
CustomizedBulletTable.NazrinSC2Spark.Init = function(laser,canRotate)
	lib.SetBulletStyleById(laser,"202060")
	laser:SetSize(0,2)
	laser.rot = Angle(laser,player)
	laser:SetExistDuration(500)
	lib.SetBulletDetectCollision(laser,false)
	lib.SetBulletResistEliminatedFlag(laser,EliminateType.PlayerSpellCard + EliminateType.PlayerDead + EliminateType.HitPlayer)
	lib.SetLaserCollisionFactor(laser,0.8)
	lib.AddBulletTask(laser,function()
		lib.ChangeLaserLengthTo(laser,500,0,30)
		if coroutine.yield(60) == false then return end
		lib.ChangeLaserWidthTo(laser,128,0,10)
		lib.SetBulletDetectCollision(laser,true)
		if coroutine.yield(60) == false then return end
		lib.SetBulletDetectCollision(laser,false)
		lib.ChangeLaserWidthTo(laser,0,0,10)
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

SC["NazrinSC2_0"] = {}
SC["NazrinSC2_0"].Init = function(boss)
	lib.SetSpellCardProperties("LaserSign-Easy",60,Condition.EliminateAll,true)
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
				local laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle+26.25+i*15,93-i*3,0)
				laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle-26.25-i*15,93-i*3,0)
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
			local laser = lib.CreateCustomizedLaser("NazrinSC2Spark",laserPosX,laserPosY,false)
			if coroutine.yield(180) == false then return end
		end
	end)
end

SC["NazrinSC2_1"] = {}
SC["NazrinSC2_1"].Init = function(boss)
	lib.SetSpellCardProperties("LaserSign-Normal",60,Condition.EliminateAll,true)
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
				local laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle+26.25+i*15,93-i*3,50)
				laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle-26.25-i*15,93-i*3,50)
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
			local laser = lib.CreateCustomizedLaser("NazrinSC2Spark",laserPosX,laserPosY,false)
			if coroutine.yield(180) == false then return end
		end
	end)
end

SC["NazrinSC2_2"] = {}
SC["NazrinSC2_2"].Init = function(boss)
	SetSpellCardProperties("LaserSign-Hard",60,Condition.EliminateAll,true)
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
				local laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle+4.125+i*16.5,93-i*3,32)
				laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle-4.125-i*16.5,93-i*3,32)
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
			local laser = lib.CreateCustomizedLaser("NazrinSC2Spark",laserPosX,laserPosY,false)
			if coroutine.yield(180) == false then return end
		end
	end)
end

SC["NazrinSC2_3"] = {}
SC["NazrinSC2_3"].Init = function(boss)
	SetSpellCardProperties("LaserSign-Lunatic",60,Condition.EliminateAll,true)
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
				local laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle+4.125+i*16.5,93-i*3,32)
				laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle-4.125-i*16.5,93-i*3,32)
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
			local laser = lib.CreateCustomizedLaser("NazrinSC2Spark",laserPosX,laserPosY,true)
			if coroutine.yield(180) == false then return end
		end
	end)
end

CustomizedBulletTable.WriggleBullet = {}
CustomizedBulletTable.WriggleBullet.Init = function(bullet,velocity,angle,waitTime,finalStyleId)
	lib.AddBulletTask(bullet,function()
		if coroutine.yield(150) == false then return end
		lib.SetBulletStyleById(bullet,"113120")
		if coroutine.yield(waitTime) == false then return end
		lib.SetBulletStyleById(bullet,"102120")
		lib.DoBulletAccelerationWithLimitation(bullet,0.05,angle,velocity*0.1)
		if coroutine.yield(90) == false then return end
		lib.SetBulletStyleById(bullet,finalStyleId)
		lib.DoBulletAccelerationWithLimitation(bullet,0.05,angle,velocity)
	end)
end

SC["WriggleSC"] = {}
SC["WriggleSC"].Init = function(boss)
	lib.SetSpellCardProperties("WriggleSign",60,Condition.EliminateAll,true)
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
	lib.SetEnemyWanderAmplitude(boss,0,32,0,32)
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
CustomizedBulletTable.OrionidsLaser.Init = function(laser,angle,isAimToPlayer,styleId)
	laser:SetStyleById(styleId)
	laser:SetSize(0,2)
	if isAimToPlayer then
		angle = angle + Angle(laser,player)
	end
	laser.rot = angle
	laser:SetExistDuration(180)
	laser:TurnHalfOn(2,0)
	lib.ChangeLaserLengthTo(laser,900,0,50)
end

CustomizedBulletTable.OrionidsBullet0 = {}
CustomizedBulletTable.OrionidsBullet0.Init = function(bullet)
	lib.SetBulletStraightParas(bullet,0,270,false,0,0)
	lib.SetBulletAppearEffectAvailable(bullet,false)
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
		enemy:SetPos(startPosX,startPosY)
		lib.SetEnemyInteractive(enemy,true)
		lib.EnemyAccMoveTowardsWithLimitation(enemy,0,angle,0.15,10)
	end)
	lib.AddEnemyTask(enemy,function()
		if coroutine.yield(60) == false then return end
		for _=1,Infinite do
			local posX,posY = lib.GetEnemyPos(enemy)
			lib.CreateCustomizedBullet("OrionidsBullet0","127051",posX,posY,0)
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

SC["OrionidsSC"] = {}
SC["OrionidsSC"].Init = function(boss)
	SetSpellCardProperties("Orionid Meteor Shower",60,Condition.EliminateAll,true)
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
					local laser = lib.CreateCustomizedLaser("OrionidsLaser",320*cos(angle),274+25*sin(angle),angle,false,"201060")
					local enemy = lib.CreateCustomizedEnemy("OrionidsEnemy","100022",0,300,320*math.cos(math.rad(angle)),274+25*math.sin(math.rad(angle)),angle,false,4)
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
					local laser = lib.CreateCustomizedLaser("OrionidsLaser",320*math.cos(math.rad(angle)),254+15*math.sin(math.rad(angle)),angleOffset,true,"201060")
					local enemy = lib.CreateCustomizedEnemy("OrionidsEnemy","100022",0,300,320*math.cos(math.rad(angle)),254+15*math.sin(math.rad(angle)),angleOffset,true,4)
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

CustomizedBulletTable.MarisaSC0Bullet0 = {}
CustomizedBulletTable.MarisaSC0Bullet0.Init = function(bullet,vAngle)
	lib.SetBulletStraightParas(bullet,3,vAngle,false,0,0)
	lib.SetBulletScale(bullet,3)
	lib.AddBulletTask(bullet,function()
		local angle = 0
		for _=1,Infinite do
			local posX,posY = lib.GetBulletPos(bullet)
			if posX > 192 then
				lib.SetBulletAlpha(bullet,0)
				lib.SetBulletDetectCollision(bullet,false)
				angle = lib.GetRandomFloat(178,182)
				lib.SetBulletStraightParas(bullet,30,angle,false,0,0)
				break
			end
			if posX < -192 then
				lib.SetBulletAlpha(bullet,0)
				lib.SetBulletDetectCollision(bullet,false)
				angle = lib.GetRandomFloat(-2,2)
				lib.SetBulletStraightParas(bullet,30,angle,false,0,0)
				break
			end
			if posY > 224 then
				lib.SetBulletAlpha(bullet,0)
				lib.SetBulletDetectCollision(bullet,false)
				angle = lib.GetRandomFloat(268,272)
				lib.SetBulletStraightParas(bullet,30,angle,false,0,0)
				break
			end
			if coroutine.yield(1) == false then return end
		end
		for _=1,Infinite do
			local posX,posY = lib.GetBulletPos(bullet)
			lib.CreateCustomizedBullet("MarisaSC0LaserBullet0","123051",posX,posY,angle,1)
			lib.CreateCustomizedBullet("MarisaSC0LaserBullet1","122051",posX,posY,angle,1)
			if coroutine.yield(1) == false then return end
		end
	end)
end

CustomizedBulletTable.MarisaSC0Bullet1 = {}
CustomizedBulletTable.MarisaSC0Bullet1.Init = function(bullet,vAngle)
	lib.SetBulletStraightParas(bullet,3,vAngle,false,0,0)
	lib.SetBulletScale(bullet,3)
	lib.AddBulletTask(bullet,function()
		local angle = 0
		for _=1,Infinite do
			local posX,posY = lib.GetBulletPos(bullet)
			if posX > 192 then
				lib.SetBulletAlpha(bullet,0)
				lib.SetBulletDetectCollision(bullet,false)
				angle = lib.GetRandomFloat(178,182)
				lib.SetBulletStraightParas(bullet,30,angle,false,0,0)
				break
			end
			if posX < -192 then
				lib.SetBulletAlpha(bullet,0)
				lib.SetBulletDetectCollision(bullet,false)
				angle = lib.GetRandomFloat(-2,2)
				lib.SetBulletStraightParas(bullet,30,angle,false,0,0)
				break
			end
			if posY > 224 then
				lib.SetBulletAlpha(bullet,0)
				lib.SetBulletDetectCollision(bullet,false)
				angle = lib.GetRandomFloat(268,272)
				lib.SetBulletStraightParas(bullet,30,angle,false,0,0)
				break
			end
			if coroutine.yield(1) == false then return end
		end
		for _=1,Infinite do
			local posX,posY = lib.GetBulletPos(bullet)
			lib.CreateCustomizedBullet("MarisaSC0LaserBullet2","123031",posX,posY,angle,1)
			lib.CreateCustomizedBullet("MarisaSC0LaserBullet3","122031",posX,posY,angle,1)
			if coroutine.yield(1) == false then return end
		end
	end)
end

CustomizedBulletTable.MarisaSC0Bullet2 = {}
CustomizedBulletTable.MarisaSC0Bullet2.Init = function(bullet,angle,extraVelocity)
	lib.AddBulletTask(bullet,function()
		local v,dv = 5,-4/19
		for _=1,20 do
			lib.SetBulletStraightParas(bullet,v+extraVelocity,angle,false,0,0)
			if coroutine.yield(1) == false then return end
			v = v + dv
		end
	end)
end

CustomizedBulletTable.MarisaSC0Bullet3 = {}
CustomizedBulletTable.MarisaSC0Bullet3.Init = function(bullet,angleOffsetRange)
	local angleOffset = lib.GetRandomFloat(-angleOffsetRange,angleOffsetRange)
	local angle = lib.GetAimToPlayerAngle(lib.GetBulletPos(bullet)) + angleOffset
	lib.AddBulletTask(bullet,function()
		do
			local v,dv = 0,0.02
			for _=1,Infinite do
				lib.SetBulletStraightParas(bullet,v*v,angle,false,0,0)
				local bulletPosX,bulletPosY = lib.GetBulletPos(bullet)
				lib.CreateCustomizedBullet("MarisaSC0Bullet5","122051",bulletPosX+lib.GetRandomFloat(-2,2),bulletPosY+lib.GetRandomFloat(-2,2),angle,1)
				if coroutine.yield(1) == false then return end
				v = v + dv
			end
		end
	end)
end

CustomizedBulletTable.MarisaSC0Bullet4 = {}
CustomizedBulletTable.MarisaSC0Bullet4.Init = function(bullet,angle)
	lib.SetBulletColor(bullet,0,0,100)
	lib.SetBulletDetectCollision(bullet,false)
	lib.AddBulletTask(bullet,function()
		do
			local alpha,dAlpha = 1,-1/19
			local scale,dScale = 1,-1/19
			for _=1,20 do
				lib.SetBulletScale(bullet,scale)
				lib.SetBulletColorWithAlpha(bullet,50,50,100,alpha)
				if coroutine.yield(1) == false then return end
				scale = scale + dScale
				alpha = alpha + dAlpha
			end
			lib.EliminateBullet(bullet)
		end
	end)
	lib.AddBulletTask(bullet,function()
		do
			local v = lib.GetRandomFloat(1,1.5)
			local dv = -v/49
			for _=1,50 do
				lib.SetBulletStraightParas(bullet,v+0.1,angle,false,0,0)
				if coroutine.yield(1) == false then return end
				v = v + dv
			end
		end
	end)
end

CustomizedBulletTable.MarisaSC0Bullet5 = {}
CustomizedBulletTable.MarisaSC0Bullet5.Init = function(bullet,angle)
	lib.SetBulletColor(bullet,50,50,100)
	lib.SetBulletDetectCollision(bullet,false)
	lib.AddBulletComponent(bullet,eBulletComponentType.ParasChange)
	lib.AddBulletParaChangeEvent(bullet,eBulletParaType.Alpha,eParaChangeMode.ChangeTo,0,0,0,0,0,20,Constants.ModeLinear,1,0)
	local v = lib.GetRandomFloat(1,1.5)
	lib.SetBulletStraightParas(bullet,v+0.1,angle,false,0,0)
	lib.AddBulletParaChangeEvent(bullet,eBulletParaType.Velocity,eParaChangeMode.DecBy,0,v/50*20,0,0,0,20,Constants.ModeLinear,1,0)
	lib.BulletDoScale(bullet,0,0,20)
	lib.SetBulletAppearEffectAvailable(bullet,false)
	lib.AddBulletTask(bullet,function()
		do
			if coroutine.yield(20) == false then return end
			lib.EliminateBullet(bullet)
		end
	end)
end

CustomizedBulletTable.MarisaSC0LaserBullet0 = {}
CustomizedBulletTable.MarisaSC0LaserBullet0.Init = function(bullet,bulletAngle)
	lib.SetBulletPara(bullet,eBulletParaType.ScaleX,0.2)
	lib.SetBulletPara(bullet,eBulletParaType.ScaleY,12)
	lib.SetBulletColorWithAlpha(bullet,100,100,255,0.1)
	lib.SetBulletStraightParas(bullet,0,bulletAngle,false,0,0)
	lib.SetBulletDetectCollision(bullet,false)
	lib.SetBulletAppearEffectAvailable(bullet,false)
	lib.AddBulletTask(bullet,function()
		if coroutine.yield(50) == false then return end
		lib.SetBulletDetectCollision(bullet,true)
		lib.SetBulletColorWithAlpha(bullet,100,100,255,1)
		local angle,dAngle = 0,180/48
		--for _=1,50 do
			--lib.SetBulletPara(bullet,eBulletParaType.ScaleX,math.sin(math.rad(angle))*1.7+0.2)
			--if coroutine.yield(1) == false then return end
			--angle = angle + dAngle
		--end
		lib.AddBulletComponent(bullet,eBulletComponentType.ParasChange)
		lib.AddBulletParaChangeEvent(bullet,eBulletParaType.ScaleX,eParaChangeMode.ChangeTo,0,1.9,0,0,0,25,Constants.ModeSin,1,0)
		lib.AddBulletParaChangeEvent(bullet,eBulletParaType.ScaleX,eParaChangeMode.ChangeTo,0,0.2,0,0,25,25,Constants.ModeCos,1,0)
		if coroutine.yield(50) == false then return end
		lib.EliminateBullet(bullet)
	end)
end

CustomizedBulletTable.MarisaSC0LaserBullet1 = {}
CustomizedBulletTable.MarisaSC0LaserBullet1.Init = function(bullet,bulletAngle)
	lib.SetBulletPara(bullet,eBulletParaType.ScaleX,0.2)
	lib.SetBulletPara(bullet,eBulletParaType.ScaleY,6)
	lib.SetBulletColorWithAlpha(bullet,100,100,255,0)
	lib.SetBulletStraightParas(bullet,0,bulletAngle,false,0,0)
	lib.SetBulletDetectCollision(bullet,false)
	lib.SetBulletAppearEffectAvailable(bullet,false)
	lib.AddBulletTask(bullet,function()
		if coroutine.yield(50) == false then return end
		lib.SetBulletDetectCollision(bullet,true)
		lib.SetBulletColorWithAlpha(bullet,50,50,255,0.2)
		local angle,dAngle = 0,180/48
		--for _=1,50 do
			--lib.SetBulletPara(bullet,eBulletParaType.ScaleX,math.sin(math.rad(angle))*1+0.2)
			--if coroutine.yield(1) == false then return end
			--angle = angle + dAngle
		--end
		lib.AddBulletComponent(bullet,eBulletComponentType.ParasChange)
		lib.AddBulletParaChangeEvent(bullet,eBulletParaType.ScaleX,eParaChangeMode.ChangeTo,0,1.2,0,0,0,25,Constants.ModeSin,1,0)
		lib.AddBulletParaChangeEvent(bullet,eBulletParaType.ScaleX,eParaChangeMode.ChangeTo,0,0.2,0,0,25,25,Constants.ModeCos,1,0)
		if coroutine.yield(50) == false then return end
		lib.EliminateBullet(bullet)
	end)
end

CustomizedBulletTable.MarisaSC0LaserBullet2 = {}
CustomizedBulletTable.MarisaSC0LaserBullet2.Init = function(bullet,bulletAngle)
	lib.SetBulletPara(bullet,eBulletParaType.ScaleX,0.2)
	lib.SetBulletPara(bullet,eBulletParaType.ScaleY,12)
	lib.SetBulletColorWithAlpha(bullet,180,100,180,0.1)
	lib.SetBulletStraightParas(bullet,0,bulletAngle,false,0,0)
	lib.SetBulletDetectCollision(bullet,false)
	lib.SetBulletAppearEffectAvailable(bullet,false)
	lib.AddBulletTask(bullet,function()
		if coroutine.yield(50) == false then return end
		lib.SetBulletDetectCollision(bullet,true)
		lib.SetBulletColorWithAlpha(bullet,180,100,180,1)
		local angle,dAngle = 0,180/48
		--for _=1,50 do
			--lib.SetBulletPara(bullet,eBulletParaType.ScaleX,math.sin(math.rad(angle))*1.7+0.2)
			--if coroutine.yield(1) == false then return end
			--angle = angle + dAngle
		--end
		lib.AddBulletComponent(bullet,eBulletComponentType.ParasChange)
		lib.AddBulletParaChangeEvent(bullet,eBulletParaType.ScaleX,eParaChangeMode.ChangeTo,0,1.9,0,0,0,25,Constants.ModeSin,1,0)
		lib.AddBulletParaChangeEvent(bullet,eBulletParaType.ScaleX,eParaChangeMode.ChangeTo,0,0.2,0,0,25,25,Constants.ModeCos,1,0)
		if coroutine.yield(50) == false then return end
		lib.EliminateBullet(bullet)
	end)
end

CustomizedBulletTable.MarisaSC0LaserBullet3 = {}
CustomizedBulletTable.MarisaSC0LaserBullet3.Init = function(bullet,bulletAngle)
	lib.SetBulletPara(bullet,eBulletParaType.ScaleX,0.2)
	lib.SetBulletPara(bullet,eBulletParaType.ScaleY,6)
	lib.SetBulletColorWithAlpha(bullet,180,100,180,0)
	lib.SetBulletStraightParas(bullet,0,bulletAngle,false,0,0)
	lib.SetBulletDetectCollision(bullet,false)
	lib.SetBulletAppearEffectAvailable(bullet,false)
	lib.AddBulletTask(bullet,function()
		if coroutine.yield(50) == false then return end
		lib.SetBulletDetectCollision(bullet,true)
		lib.SetBulletColorWithAlpha(bullet,180,50,180,0.2)
		local angle,dAngle = 0,180/48
		--for _=1,50 do
			--lib.SetBulletPara(bullet,eBulletParaType.ScaleX,math.sin(math.rad(angle))*1+0.2)
			--if coroutine.yield(1) == false then return end
			--angle = angle + dAngle
		--end
		lib.AddBulletComponent(bullet,eBulletComponentType.ParasChange)
		lib.AddBulletParaChangeEvent(bullet,eBulletParaType.ScaleX,eParaChangeMode.ChangeTo,0,1.2,0,0,0,25,Constants.ModeSin,1,0)
		lib.AddBulletParaChangeEvent(bullet,eBulletParaType.ScaleX,eParaChangeMode.ChangeTo,0,0.2,0,0,25,25,Constants.ModeCos,1,0)
		if coroutine.yield(50) == false then return end
		lib.EliminateBullet(bullet)
	end)
end

SC["MarisaSC0"] = {}
SC["MarisaSC0"].Init = function(boss)
	lib.SetSpellCardProperties("Gamma HyperNova",40,Condition.EliminateAll,true)
	--lib.EnemyMoveToPos(boss,0,128,120,Constants.ModeEaseOutQuad)
	lib.SetBossInvincible(boss,15)
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
	lib.EnemyMoveToPos(boss,0,144,50,Constants.ModeEaseOutQuad)
	if coroutine.yield(120) == false then return end
	--移动task
	lib.AddEnemyTask(boss,function()
		for _=1,Infinite do
			lib.EnemyMoveToPos(boss,145,160,70,Constants.ModeEaseOutQuad)
			if coroutine.yield(70) == false then return end
			do
				local san,dSan = 0,90/99
				for _=1,100 do
					local posY = -160 * math.sin(math.rad(san*2-90))
					local posX = 145 - 290 * math.sin(math.rad(san))
					lib.SetEnemyPos(boss,posX,posY)
					if coroutine.yield(1) == false then return end
					san = san + dSan
				end
			end
			do
				local playerPosX,playerPosY = lib.GetPlayerPos()
				lib.EnemyMoveToPos(boss,playerPosX+lib.GetRandomFloat(-20,20),lib.GetRandomFloat(150,170),70,Constants.ModeEaseOutQuad)
				if coroutine.yield(70) == false then return end
			end
			if coroutine.yield(75) == false then return end
			lib.EnemyMoveToPos(boss,-145,160,70,Constants.ModeEaseOutQuad)
			if coroutine.yield(70) == false then return end
			do
				local san,dSan = 0,90/99
				for _=1,100 do
					local posY = -160 * math.sin(math.rad(san*2-90))
					local posX = -145 + 290 * math.sin(math.rad(san))
					lib.SetEnemyPos(boss,posX,posY)
					if coroutine.yield(1) == false then return end
					san = san + dSan
				end
			end
			do
				local playerPosX,playerPosY = lib.GetPlayerPos()
				lib.EnemyMoveToPos(boss,playerPosX+lib.GetRandomFloat(-20,20),lib.GetRandomFloat(150,170),70,Constants.ModeEaseOutQuad)
				if coroutine.yield(70) == false then return end
			end
			if coroutine.yield(75) == false then return end
			do
				lib.EnemyMoveToPos(boss,lib.GetRandomFloat(-20,20),lib.GetRandomFloat(150,170),70,Constants.ModeEaseOutQuad)
				if coroutine.yield(70) == false then return end
			end
			do
				lib.EnemyMoveToPos(boss,lib.GetRandomFloat(-20,20),lib.GetRandomFloat(150,170),40,Constants.ModeEaseOutQuad)
			end
			if coroutine.yield(80) == false then return end
			if coroutine.yield(60) == false then return end
		end
	end)
	lib.AddEnemyTask(boss,function()
		for _=1,Infinite do
			if coroutine.yield(70) == false then return end
			do --星弹，撞墙产生激光
				local angle,dAngle = 90,150/19
				for _=1,20 do
					local posX,posY = lib.GetEnemyPos(boss)
					lib.CreateCustomizedBullet("MarisaSC0Bullet0","111060",posX,posY,angle,1)
					if coroutine.yield(5) == false then return end
					angle = angle + dAngle
				end
			end
			if coroutine.yield(70) == false then return end
			do
				for _=1,5 do
					local posX,posY = lib.GetEnemyPos(boss)
					local angleToPlayer = lib.GetAimToPlayerAngle(posX,posY)
					local angle,dAngle = angleToPlayer - 135 + lib.GetRandomFloat(-10,10),270/24
					for _=1,25 do
						lib.CreateCustomizedBullet("MarisaSC0Bullet2","111130",posX+math.cos(math.rad(angle))*20,posY+math.sin(math.rad(angle))*20,angle,1,2)
						lib.CreateCustomizedBullet("MarisaSC0Bullet2","111130",posX+math.cos(math.rad(angle))*20,posY+math.sin(math.rad(angle))*20,angle,0.2,2)
						angle = angle + dAngle
					end
					if coroutine.yield(15) == false then return end
				end
			end
			if coroutine.yield(70) == false then return end
			do --星弹，撞墙产生激光
				local angle,dAngle = 90,-150/19
				for _=1,20 do
					local posX,posY = lib.GetEnemyPos(boss)
					lib.CreateCustomizedBullet("MarisaSC0Bullet1","111040",posX,posY,angle,1)
					if coroutine.yield(5) == false then return end
					angle = angle + dAngle
				end
			end
			if coroutine.yield(70) == false then return end
			do
				for _=1,5 do
					local posX,posY = lib.GetEnemyPos(boss)
					local angleToPlayer = lib.GetAimToPlayerAngle(posX,posY)
					local angle,dAngle = angleToPlayer - 135 + lib.GetRandomFloat(-10,10),270/24
					for _=1,25 do
						lib.CreateCustomizedBullet("MarisaSC0Bullet2","111130",posX+math.cos(math.rad(angle))*20,posY+math.sin(math.rad(angle))*20,angle,1,2)
						lib.CreateCustomizedBullet("MarisaSC0Bullet2","111130",posX+math.cos(math.rad(angle))*20,posY+math.sin(math.rad(angle))*20,angle,0.2,2)
						angle = angle + dAngle
					end
					if coroutine.yield(15) == false then return end
				end
			end
			if coroutine.yield(70) == false then return end
			do
				local posX,posY = lib.GetEnemyPos(boss)
				local angle,dAngle = 0,-180/19
				local m,dm = 0,1
				for _=1,20 do
					if m%2 == 0 then
						lib.CreateCustomizedBullet("MarisaSC0Bullet0","111040",posX,posY,angle,1)
					else
						lib.CreateCustomizedBullet("MarisaSC0Bullet0","111060",posX,posY,angle,1)
					end
					angle = angle + dAngle
					m = m + dm
				end
			end
			do
				local range,dRange = 120,-80/39
				for _=1,40 do
					local posX,posY = lib.GetEnemyPos(boss)
					lib.CreateCustomizedBullet("MarisaSC0Bullet3","117051",posX,posY,range,1)
					if coroutine.yield(2) == false then return end
				end
			end
			do
				if coroutine.yield(60) == false then return end
			end
		end
	end)
end

CustomizedBulletTable.PatchouliNonSC0Bullet0 = {}
CustomizedBulletTable.PatchouliNonSC0Bullet0.Init = function(bullet,laserId,acce,accAngle,omega)
	lib.SetBulletSelfRotation(bullet,omega)
	lib.SetBulletStraightParas(bullet,0,0,false,acce,accAngle)
	do
		local laserStartAngle,dAngle = lib.GetRandomFloat(0,360),90
		local posX,posY = lib.GetBulletPos(bullet)
		for _=1,4 do
			last = lib.CreateLaser(laserId,posX,posY,laserStartAngle,40,8,-1)
			last:TurnOn(0)
			lib.AttatchToMaster(last,bullet,true)
			lib.SetAttachmentRelativePos(last,0,0,laserStartAngle,true,true)
			laserStartAngle = laserStartAngle + dAngle
		end
	end
	do
		lib.AddBulletTask(bullet,function()
			for _=1,Infinite do
				local posX,posY = lib.GetBulletPos(bullet)
				local rotation = lib.GetBulletPara(bullet,eBulletParaType.VAngel)
				local id = lib.GetBulletId(bullet)
				lib.CreateCustomizedBullet("PatchouliNonSC0Bullet1",id,posX,posY,rotation,1)
				if coroutine.yield(5) == false then return end
			end
		end)
	end
end

CustomizedBulletTable.PatchouliNonSC0Bullet1 = {}
CustomizedBulletTable.PatchouliNonSC0Bullet1.Init = function(bullet,angle)
	lib.SetBulletDetectCollision(bullet,false)
	lib.AddBulletComponent(bullet,eBulletComponentType.ParasChange)
	lib.AddBulletParaChangeEvent(bullet,eBulletParaType.Alpha,eParaChangeMode.ChangeTo,0,0,0,0,0,20,Constants.ModeLinear,1,0)
	lib.SetBulletStraightParas(bullet,0,angle,false,0,0)
	lib.BulletDoScale(bullet,0,0,20)
	lib.SetBulletAppearEffectAvailable(bullet,false)
	lib.AddBulletTask(bullet,function()
		do
			if coroutine.yield(20) == false then return end
			lib.EliminateBullet(bullet)
		end
	end)
end

SC["PatchouliNonSC0"] = {}
SC["PatchouliNonSC0"].Init = function(boss)
	SetSpellCardProperties("",60,Condition.EliminateAll,false)
	lib.EnemyMoveToPos(boss,0,128,120,Constants.ModeLinear)
	boss:SetInvincible(5)
	lib.SetEnemyMaxHp(boss,1400)
	lib.ShowBossBloodBar(boss,true)
	if coroutine.yield(120) == false then return end
	local i , di = 0 , 1
	local lightBallIds = {"122001","122011","122031","122051","122071","122091","122131","122151"}
	local laserIds = {"202001","202011","202031","202051","202071","202091","202131","202151"}
	lib.AddEnemyTask(boss,function()
		for _=1,Infinite do
			for _=1,30 do
				local index = lib.GetRandomInt(1,8)
				local posX = lib.GetRandomFloat(-60,192)
				local posY = 224
				local acce = lib.GetRandomFloat(0.01,0.1)
				local accAngle = lib.GetRandomFloat(240,260)
				lib.CreateCustomizedBullet("PatchouliNonSC0Bullet0",lightBallIds[index],posX,posY,laserIds[index],acce,accAngle,-1,4)
				index = lib.GetRandomInt(1,8)
				posX = 192
				local posY = lib.GetRandomFloat(-90,224)
				local acce = lib.GetRandomFloat(0.01,0.1)
				local accAngle = lib.GetRandomFloat(240,260)
				lib.CreateCustomizedBullet("PatchouliNonSC0Bullet0",lightBallIds[index],posX,posY,laserIds[index],acce,accAngle,-1,4)
				if coroutine.yield(20) == false then return end
			end
		end
	end)
	lib.AddEnemyTask(boss,function()
		if coroutine.yield(60) == false then return end
		for _=1,Infinite do
			for _=1,30 do
				local index = lib.GetRandomInt(1,8)
				local posX = lib.GetRandomFloat(-192,60)
				local posY = 224
				local acce = lib.GetRandomFloat(0.01,0.1)
				local accAngle = lib.GetRandomFloat(-80,-30)
				lib.CreateCustomizedBullet("PatchouliNonSC0Bullet0",lightBallIds[index],posX,posY,laserIds[index],acce,accAngle,1,4)
				index = lib.GetRandomInt(1,8)
				posX = -192
				local posY = lib.GetRandomFloat(-90,224)
				local acce = lib.GetRandomFloat(0.01,0.1)
				local accAngle = lib.GetRandomFloat(-80,-30)
				lib.CreateCustomizedBullet("PatchouliNonSC0Bullet0",lightBallIds[index],posX,posY,laserIds[index],acce,accAngle,1,4)
				if coroutine.yield(20) == false then return end
			end
		end
	end)
end

CustomizedEnemyTable.PatchouliEnemy0 = {}
CustomizedEnemyTable.PatchouliEnemy0.Init = function(enemy,startRotation,omega)
	lib.SetEnemyMaxHp(enemy,5)
	lib.SetEnemyInteractive(enemy,false)
	local laser = lib.CreateCustomizedLaser("PatchouliNonSC1Laser",enemy.x,enemy.y,enemy,startRotation,omega)
	lib.AddEnemyTask(enemy,function()
		local curRotation,dCurRotation = startRotation,omega
		for _=1,210 do
			lib.SetAttachmentRelativePos(enemy,80*math.cos(math.rad(curRotation)),80*math.sin(math.rad(curRotation)),0,false,false)
			if coroutine.yield(1) == false then return end
			curRotation = curRotation + dCurRotation
		end
		lib.RawEliminateEnemy(enemy)
	end)
	lib.AddEnemyTask(enemy,function()
		local i,di = 0,omega*30
		for _=1,4 do
			local posX,posY = lib.GetEnemyPos(enemy)
			local angle,dAngle = 0,40
			for _=1,9 do
				local bullet = lib.CreateSimpleBulletById("124011",posX,posY)
				lib.SetBulletStraightParas(bullet,3,angle,true,0,0)
				bullet = lib.CreateSimpleBulletById("124011",posX,posY)
				lib.SetBulletStraightParas(bullet,2,angle+10,true,0,0)
				bullet = lib.CreateSimpleBulletById("124011",posX,posY)
				lib.SetBulletStraightParas(bullet,1,angle-10,true,0,0)
				angle = angle + dAngle
			end
			if coroutine.yield(30) == false then return end
			i = i + di
		end
	end)
end

CustomizedBulletTable.PatchouliNonSC1Laser = {}
CustomizedBulletTable.PatchouliNonSC1Laser.Init = function(self,master,startRotation,omega)
	self:SetStyleById(202011)
	local posX,posY=  lib.GetEnemyPos(master)
	self:SetSize(500,32)
	self:SetExistDuration(80)
	self.orderInLayer = -1
	lib.AttatchToMaster(self,master,true)
	lib.SetAttachmentRelativePos(self,0,0,startRotation,false,true)
	self:TurnOn(45)
	lib.AddBulletTask(self,function()
		if coroutine.yield(45) == false then return end
		lib.SetLaserRotateParaWithOmega(self,omega,150)
		if coroutine.yield(105) == false then return end
		self:TurnOff(60)
		if coroutine.yield(60) == false then return end
		lib.EliminateBullet(self)
	end)
end

SC["PatchouliNonSC1"] = {}
SC["PatchouliNonSC1"].Init = function(boss)
	lib.EnemyMoveToPos(boss,0,128,120,Constants.ModeLinear)
	if coroutine.yield(120) == false then return end
	SetSpellCardProperties("",60,Condition.EliminateAll,false)
	lib.SetBossInvincible(boss,5)
	lib.SetEnemyMaxHp(boss,500)
	lib.ShowBossBloodBar(boss,true)
	if coroutine.yield(60) == false then return end
	--if coroutine.yield(120) == false then return end
	lib.SetEnemyWanderRange(boss,-100,100,112,144)
	lib.SetEnemyWanderAmplitude(boss,24,60,5,15)
	lib.SetEnemyWanderMode(boss,Constants.ModeLinear,Constants.DirModeMoveRandom)
	lib.AddEnemyTask(boss,function()
		for _=1,Infinite do
			do
				local angle,dAngle = lib.GetAngleToPlayer(boss),72
				for _=1,5 do
					local enemy = lib.CreateCustomizedEnemy("PatchouliEnemy0","100002",0,300,angle,-0.45,2)
					lib.AttatchToMaster(enemy,boss,true)
					angle = angle + dAngle
				end
			end
			if coroutine.yield(200) == false then return end
			lib.EnemyDoWander(boss,60)
			if coroutine.yield(60) == false then return end
			do
				local angle,dAngle = lib.GetAngleToPlayer(boss)-60,72
				for _=1,5 do
					local enemy = lib.CreateCustomizedEnemy("PatchouliEnemy0","100002",0,300,angle,0.45,2)
					lib.AttatchToMaster(enemy,boss,true)
					angle = angle + dAngle
				end
			end
			if coroutine.yield(200) == false then return end
			lib.EnemyDoWander(boss,60)
			if coroutine.yield(60) == false then return end
		end
	end)
end

CustomizedBulletTable.PatchouliSC0_FireLine = {}
CustomizedBulletTable.PatchouliSC0_FireLine.Init = function(bullet)
	local collider = lib.CreateObjectColliderByType(eColliderType.Circle)
	lib.SetObjectColliderSize(collider,16,16)
	lib.SetObjectColliderColliderGroup(collider,Group_CustomizedGroup1)
	lib.AttatchToMaster(collider,bullet,true)
	lib.SetAttachmentRelativePos(collider,0,0,0,false,true)
	lib.AddBulletTask(bullet,function()
		if coroutine.yield(180) == false then return end
		lib.SetBulletStraightParas(bullet,35/60,90,false,0,0)
		if coroutine.yield(60) == false then return end
		for _=1,Infinite do
			lib.SetBulletStraightParas(bullet,70/120,-90,false,0,0)
			if coroutine.yield(120) == false then return end
			lib.SetBulletStraightParas(bullet,70/120,90,false,0,0)
			if coroutine.yield(120) == false then return end
			lib.SetBulletStraightParas(bullet,0,90,false,0,0)
			if coroutine.yield(120) == false then return end
		end
	end)
end

CustomizedBulletTable.PatchouliSC0_FireBall = {}
CustomizedBulletTable.PatchouliSC0_FireBall.Init = function(bullet,vAngle)
	local accAngle = -90 + lib.GetRandomFloat(-3,3)
	lib.SetBulletStraightParas(bullet,3,vAngle,false,0,0)
	lib.DoBulletAccelerationWithLimitation(bullet,0.03,accAngle,4)
	lib.AddBulletComponent(bullet,eBulletComponentType.ColliderTrigger)
	local count = 1
	lib.AddBulletColliderTriggerEvent(bullet,Group_CustomizedGroup1,function(collider,collIndex)
		if count == 0 then return end
		local posX,posY = lib.GetBulletPos(bullet)
		for _=1,7 do
			local _tmpVar = lib.CreateSimpleBulletById("113130",posX,posY)
			lib.SetBulletStraightParas(_tmpVar,lib.GetRandomFloat(2,5.7),lib.GetRandomFloat(45,135),false,0,0)
			lib.DoBulletAccelerationWithLimitation(_tmpVar,0.04,-90 + lib.GetRandomFloat(-2,2),3)
		end
		lib.EliminateBullet(bullet)
		count = 0
	end)
end

CustomizedBulletTable.PatchouliSC0_FireRain = {}
CustomizedBulletTable.PatchouliSC0_FireRain.Init = function(bullet,vAngle)
	lib.SetBulletScale(bullet,0.6)
	lib.SetBulletStraightParas(bullet,2,-90,false,0,0)
	lib.AddBulletComponent(bullet,eBulletComponentType.ColliderTrigger)
	local count = 1
	lib.AddBulletColliderTriggerEvent(bullet,Group_CustomizedGroup1,function(collider,collIndex)
		if count == 0 then return end
		lib.SetBulletStyleById(bullet,126130)
		count = 0
	end)
end

SC["PatchouliSC1"] = {}
SC["PatchouliSC1"].Init = function(boss)
	SetSpellCardProperties("日金符 [Alchemy Furnace]",60,Condition.EliminateAll,true)
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
	if coroutine.yield(120) == false then return end
	do
		local x,dx = 0,4.5
		for _=1,51 do
			local _tmpVar = lib.CreateCustomizedBullet("PatchouliSC0_FireLine","124131",-224+x,0,0)
			_tmpVar = lib.CreateCustomizedBullet("PatchouliSC0_FireLine","124131",224-x,0,0)
			if coroutine.yield(1) == false then return end
			x = x + dx
		end
	end
	if coroutine.yield(120) == false then return end
	do
		lib.AddEnemyTask(boss,function()
			for _=1,Infinite do
				lib.CreateCustomizedBullet("PatchouliSC0_FireRain","124130",lib.GetRandomFloat(-185,185),220,0)
				if coroutine.yield(4) == false then return end
			end
		end)
	end
	do
		lib.SetEnemyWanderRange(boss,-105,105,128,144)
		lib.SetEnemyWanderAmplitude(boss,30,60,0,8)
		lib.SetEnemyWanderMode(boss,Constants.ModeLinear,Constants.DirModeMoveXTowardsPlayer)
		for _=1,Infinite do
			lib.EnemyDoWander(boss,60)
			if coroutine.yield(60) == false then return end
			local angle,dAngle = lib.GetRandomFloat(0,360),30
			local posX,posY = lib.GetEnemyPos(boss)
			for _=1,12 do
				lib.CreateCustomizedBullet("PatchouliSC0_FireBall","118130",posX,posY,angle,1)
				angle = angle + dAngle
			end
			if coroutine.yield(90) == false then return end
		end
	end
end

CustomizedBulletTable.ReisenSC0_Bullet = {}
CustomizedBulletTable.ReisenSC0_Bullet.Init = function(bullet,vAngle)
	local v = lib.GetRandomFloat(5,8)
	lib.SetBulletStraightParas(bullet,v,vAngle,false,0,0)
	lib.AddBulletComponent(bullet,eBulletComponentType.ColliderTrigger)
	lib.AddBulletColliderTriggerEvent(bullet,eEliminateType.CustomizedType0,function(collider,collIndex)
		lib.SetBulletStraightParas(bullet,v,vAngle,false,0,0)
	end)
	lib.AddBulletColliderTriggerEvent(bullet,eEliminateType.CustomizedType1,function(collider,collIndex)
		lib.SetBulletStraightParas(bullet,v/3,vAngle,false,0,0)
	end)
end

SC["ReisenSC0"] = {}
SC["ReisenSC0"].Init = function(boss)
	SetSpellCardProperties("水符 [Alchemy Furnace]",60,Condition.EliminateAll,true)
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
	if coroutine.yield(120) == false then return end
	do
		lib.AddEnemyTask(boss,function()
			local spriteEffect = lib.CreateSpriteEffectWithProps("STGEffectAtlas","TransparentCircle",eBlendMode.Normal,eEffectLayer.Bottom,false,0)
			lib.SetEffectToPos(spriteEffect,0,-100)
			lib.SetSpriteEffectScale(spriteEffect,4,4)
			lib.SetSpriteEffectColor(spriteEffect,0.55,0.45,0.65,0.75)
			--ObjectCollider
			local outterCollider = lib.CreateObjectColliderByType(eColliderType.Circle)
			lib.SetObjectColliderSize(outterCollider,80,80)
			lib.SetObjectColliderToPos(outterCollider,0,-100)
			lib.SetObjectColliderColliderGroup(outterCollider,Group_CustomizedGroup0)
			local innerCollider = lib.CreateObjectColliderByType(eColliderType.Circle)
			lib.SetObjectColliderSize(innerCollider,64,64)
			lib.SetObjectColliderToPos(innerCollider,0,-100)
			lib.SetObjectColliderColliderGroup(innerCollider,Group_CustomizedGroup1)
			--
			if coroutine.yield(120)==false then return end
			do
				local i,di = 0,1
				for _=1,120 do
					local posX = 100*i/120
					lib.SetEffectToPos(spriteEffect,posX,-100)
					lib.SetObjectColliderToPos(outterCollider,posX,-100)
					lib.SetObjectColliderToPos(innerCollider,posX,-100)
					if coroutine.yield(1)==false then return end
					i = i + di
				end
			end
			do
				for _=1,Infinite do
					do
						local i,di = 0,1
						for _=1,240 do
							local posX = 100-200*i/240
							lib.SetEffectToPos(spriteEffect,posX,-100)
							lib.SetObjectColliderToPos(outterCollider,posX,-100)
							lib.SetObjectColliderToPos(innerCollider,posX,-100)
							if coroutine.yield(1)==false then return end
							i = i + di
						end
					end
					do
						local i,di = 0,1
						for _=1,240 do
							local posX = -100+200*i/240
							lib.SetEffectToPos(spriteEffect,posX,-100)
							lib.SetObjectColliderToPos(outterCollider,posX,-100)
							lib.SetObjectColliderToPos(innerCollider,posX,-100)
							if coroutine.yield(1)==false then return end
							i = i + di
						end
					end
				end
			end
		end)
		lib.AddEnemyTask(boss,function()
			
		end)
	end
end

return
{
	CustomizedBulletTable = CustomizedBulletTable,
	CustomizedEnemyTable = CustomizedEnemyTable,
	BossTable = {},
	Stage = {},
}