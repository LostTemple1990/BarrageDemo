local SC = SpellCard
local Condition = Constants.eSpellCardCondition
local BulletResist = Constants.eBulletResistEliminated
lib = require "LuaLib"

local CustomizedBulletTable = {}
local CustomizedEnemyTable = {}


--waitTime之后变换成札弹的子弹
CustomizedBulletTable.NazrinSC1Bullet0 = {}
CustomizedBulletTable.NazrinSC1Bullet0.Init = function(bullet,waitTime,maxRadius)
	lib.SetBulletResistEliminatedFlag(bullet,BulletResist.PlayerBomb + BulletResist.PlayerDead + BulletResist.HitPlayer)
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
			if k == 0 
			then
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
		local spriteEffect = lib.CreateSpriteEffect("TransparentCircle",150,150,2000,2000)
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
		local spriteEffect = lib.CreateSpriteEffect("TransparentCircle",150,150,2000,2000)
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
				local bullet = lib.CreateCustomizedBullet("NazrinSC1Bullet0","0010",posX,posY,i,75,2)
				lib.SetBulletCurvePara(bullet,0,23.5*i*k,0,1*k)
				if coroutine.yield(1)==false then return end
			end
			k = -k
			if coroutine.yield(90)==false then return end
		end
	end)
	--圈形防护罩0
	lib.AddEnemyTask(boss,function()
		local spriteEffect = lib.CreateSpriteEffect("TransparentCircle",150,150,2000,2000)
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
		local spriteEffect = lib.CreateSpriteEffect("TransparentCircle",150,150,2000,2000)
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
			local enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0998",posX,posY,-170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0998",posX,posY,170,205,duration,3)
			if coroutine.yield(180)==false then return end
			posX,posY = lib.GetEnemyPos(boss)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0998",posX,posY,-170,-205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0998",posX,posY,170,-205,duration,3)
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
				local bullet = lib.CreateCustomizedBullet("NazrinSC1Bullet0","0010",posX,posY,i,75,2)
				lib.SetBulletCurvePara(bullet,0,23.5*i*k,0,1*k)
				if coroutine.yield(1)==false then return end
			end
			k = -k
			if coroutine.yield(90)==false then return end
		end
	end)
	--圈形防护罩0
	lib.AddEnemyTask(boss,function()
		local spriteEffect = lib.CreateSpriteEffect("TransparentCircle",150,150,2000,2000)
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
		local spriteEffect = lib.CreateSpriteEffect("TransparentCircle",150,150,2000,2000)
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
			local enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0998",posX,posY,-170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0998",posX,posY,170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0998",posX,posY,-170,-205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0998",posX,posY,170,-205,duration,3)
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
				local bullet = lib.CreateCustomizedBullet("NazrinSC1Bullet0","0010",posX,posY,i,75,2)
				lib.SetBulletCurvePara(bullet,0,23.5*i*k,0,1*k)
				if coroutine.yield(1)==false then return end
			end
			k = -k
			if coroutine.yield(90)==false then return end
		end
	end)
	--圈形防护罩0
	lib.AddEnemyTask(boss,function()
		local spriteEffect = lib.CreateSpriteEffect("TransparentCircle",150,150,2000,2000)
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
		local spriteEffect = lib.CreateSpriteEffect("TransparentCircle",150,150,2000,2000)
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
			local enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0998",posX,posY,-170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0998",posX,posY,170,205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0998",posX,posY,-170,-205,duration,3)
			enemy = lib.CreateCustomizedEnemy("NazrinSC1Enemy","0998",posX,posY,170,-205,duration,3)
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
CustomizedBulletTable.NazrinSC2Laser.Init = function(laser,posX,posY,angle,existDuration)
	--lib.SetBulletResistEliminatedFlag(bullet,BulletResist.PlayerBomb + BulletResist.PlayerDead + BulletResist.HitPlayer)
	lib.SetBulletTexture(laser,"200060")
	lib.SetLaserProps(laser,posX,posY,angle,2,500,150)
	lib.SetBulletDetectCollision(laser,false)
	lib.SetLaserCollisionFactor(laser,0.8)
	lib.AddBulletTask(laser,function()
		if coroutine.yield(30) == false then return end
		lib.ChangeLaserWidth(laser,16,10,30)
		lib.SetBulletDetectCollision(laser,true)
		if coroutine.yield(existDuration) == false then return end
		lib.SetBulletDetectCollision(laser,false)
		lib.ChangeLaserWidth(laser,0,10,0)
		if coroutine.yield(10) == false then return end
		lib.EliminateBullet(laser)
	end)
	lib.AddBulletTask(laser,function()
		if coroutine.yield(existDuration+30) == false then return end
		local createBulletCount = math.floor(500/32)
		local normalizedX = math.cos(math.rad(angle))
		local normalizedY = math.sin(math.rad(angle))
		for i=1,createBulletCount do
			local x = normalizedX * i * 32 + posX
			local y = normalizedY * i * 32 + posY
			local a = lib.GetRandomInt(0,360)
			local bullet = lib.CreateSimpleBulletById("104060",x,y)
			lib.SetBulletStraightParas(bullet,3,a,false,0,0)
		end
	end)
end

function SC.NazrinSC2_0(boss)
	lib.SetSpellCardProperties("LaserSign-Lasers",60,Condition.EliminateAll,true,nil)
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
	--激光部分
	lib.AddEnemyTask(boss,function()
		for _=1,Infinite do
			local posX,posY = lib.GetEnemyPos(boss)
			local playerAngle = lib.GetAimToPlayerAngle(posX,posY)
			for i=1,20 do
				local laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle+11.25+i*15,93-i*3,4)
				laser = lib.CreateCustomizedLaser("NazrinSC2Laser",posX,posY,playerAngle-11.25-i*15,93-i*3,4)
				if coroutine.yield(3) == false then return end
			end
			if coroutine.yield(120) == false then return end
		end
	end)
end

return
{
	CustomizedBulletTable = CustomizedBulletTable,
	CustomizedEnemyTable = CustomizedEnemyTable,
}