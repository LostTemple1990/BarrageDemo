lib = require "LuaLib"
local consts = Constants
local Stage = {}

local CustomizedTable = {}
CustomizedTable.SinBullet = {}
CustomizedTable.SinBullet.Init = function(self,incX,angle,isNegative)
	lib.AddBulletTask(self,function()
		local continue
		local beginX,beginY = lib.GetBulletPos(self)
		local curX,curY
		curX = 0
		--local angle = lib.GetAimToPlayerAngle(beginX,beginY)
		--lib.LogLuaNumber(curX,curY,beginX,beginY,angle,5)
		for _=1,999 do
			curY = 30*math.sin(2*math.rad(curX))
			if isNegative then curY = -curY end
			lib.SetBulletPos(self,lib.GetPosAfterRotate(curX+beginX,curY+beginY,beginX,beginY,angle))
			curX = curX + incX
			continue = coroutine.yield(1);
			if ( continue==false ) then return end
		end
	end)
end

CustomizedTable.NazrinLaser = {}
CustomizedTable.NazrinLaser.Init = function(laser,enemy,angle,existDuration,toAngle,rotateDuration)
	local posX,posY = lib.GetEnemyPos(enemy)
	lib.SetLaserProps(laser,posX,posY,angle,4,600,existDuration)
	lib.SetLaserRotatePara(laser,toAngle,rotateDuration)
	lib.SetBulletDetectCollision(laser,false)
	lib.ShowLaserWarningLine(laser,60)
	lib.SetLaserCollisionDetectParas(laser,7,300)
	lib.AddBulletTask(laser,function()
		lib.ChangeLaserWidthTo(laser,60,0,60)
		if coroutine.yield(60)==false then return end
		lib.SetBulletDetectCollision(laser,true)
	end)
	lib.AddBulletTask(laser,function()
		for _=1,Infinite do
			posX,posY = lib.GetEnemyPos(enemy)
			lib.SetBulletPos(laser,posX,posY)
			if ( coroutine.yield(1)==false ) then return end
		end
	end)
end

CustomizedTable.ReboundLinearLaser = {}
CustomizedTable.ReboundLinearLaser.Init = function(laser,posX,posY,angle,reboundPara,reboundCount)
	lib.SetBulletPos(laser,posX,posY)
	lib.SetBulletStyleById(laser,"202060")
	lib.SetLinearLaserLength(laser,45)
	lib.LinearLaserDoStraightMove(laser,3.5,angle,false,0,0)
	lib.AddBulletTask(laser,function()
		lib.SetLinearLaserHeadEnable(laser,true)
		lib.SetLinearLaserSourceEnable(laser,true,3)
		for _=1,Infinite do
			if reboundCount > 0 then
				local reboundFlag = 0
				local curPosX,curPosY = lib.GetBulletPos(laser)
				local tmpRebound = reboundPara
				local reboundAngle = lib.GetBulletPara(eBulletParaType.VAngel)
				if tmpRebound >= Constants.ReboundBottom and curPosY < -224 then
					reboundFlag = 1
					reboundAngle = -reboundAngle
					curPosY = -448 - curPosY
					tmpRebound = tmpRebound - Constants.ReboundBottom
				end
				if tmpRebound >= Constants.ReboundTop and curPosY > 224 then
					reboundFlag = 1
					reboundAngle = -reboundAngle
					curPosY = 448 - curPosY
					tmpRebound = tmpRebound - Constants.ReboundTop
				end
				if tmpRebound >= Constants.ReboundRight and curPosX > 192 then
					reboundFlag = 1
					reboundAngle = 180 - reboundAngle
					curPosX = 384 - curPosX
					tmpRebound = tmpRebound - Constants.ReboundRight
				end
				if tmpRebound >= Constants.ReboundLeft and curPosX < -192 then
					reboundFlag = 1
					reboundAngle = -180 - reboundAngle
					curPosX = -384 - curPosX
				end
				if reboundFlag == 1 then
					reboundCount = reboundCount - 1
					lib.CreateCustomizedLinearLaser("ReboundLinearLaser",curPosX,curPosY,reboundAngle,reboundPara,reboundCount,5)
					return
				end
			end
			if coroutine.yield(1)==false then return end
		end
	end)
end

CustomizedTable.XiongBullet0 = {}
CustomizedTable.XiongBullet0.Init = function(bullet,waitTime,maxRadius)
	lib.AddBulletTask(bullet,function()
		local posX,posY = lib.GetBulletPos(bullet)
		lib.AddBulletComponent(bullet,eBulletComponentType.ParasChange)
		lib.AddBulletParaChangeEvent(bullet,eBulletParaType.CurveRadius,eParaChangeMode.ChangeTo,0,maxRadius,0,0,0,30,Constants.ModeLinear,1,0)
		--lib.AddBulletParaChangeEvent(bullet,8,Constants.ParaChangeMode_ChangeTo,1.5,50-waitTime,Constants.ModeLinear,10)
		if coroutine.yield(80-waitTime) == false then return end
		lib.SetBulletStyleById(bullet,"107060")
		--lib.SetBulletOrderInLayer(bullet,1)
		local angle = lib.GetAimToPlayerAngle(posX,posY)
		lib.SetBulletStraightParas(bullet,2.5,angle,false,0,0)
	end)
end

CustomizedTable.SC1Bullet1 = {}
CustomizedTable.SC1Bullet1.Init = function(bullet,eliminateDis)
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

local CustomizedEnemyTable = {}
CustomizedEnemyTable.SC1Enemy = {}
CustomizedEnemyTable.SC1Enemy.Init = function(enemy,toPosX,toPosY,duration)
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
				bullet = lib.CreateCustomizedBullet("SC1Bullet1","0000",posX,posY,100,1)
				--bullet = lib.CreateSimpleBulletById("0000",posX,posY)
			else
				--bullet = lib.CreateSimpleBulletById("0010",posX,posY)
				bullet = lib.CreateCustomizedBullet("SC1Bullet1","0010",posX,posY,85,1)
			end
			lib.SetBulletStraightParas(bullet,12,lib.GetRandomInt(-45,45),true,0,0)
			if coroutine.yield(2) == false then return end
		end
	end)
end

CustomizedEnemyTable.TestKillEnemy = {}
CustomizedEnemyTable.TestKillEnemy.Init = function(enemy)
	do
		lib.SetEnemyMaxHp(enemy,50)
	end
	do
		local itemDatas = {}
		table.insert(itemDatas,{itemType = 1,itemCount = 5})
		lib.SetEnemyDropItems(enemy,itemDatas,48,48)
	end
end
CustomizedEnemyTable.TestKillEnemy.OnKill = function(enemy)
	do
		local k = 1
		local posX,posY = lib.GetEnemyPos(enemy)
		for _=1,1 do
			local i
			for i=0,15 do
				local bullet = lib.CreateCustomizedBullet("XiongBullet0","113020",posX,posY,i,75,2)
				lib.SetBulletCurvePara(bullet,0,24*i*k,0,1.5*k)
			end
		end
	end
	do
		local posX,posY = lib.GetEnemyPos(enemy)
		local master = lib.CreateSimpleBulletById("122151",posX,posY)
		lib.SetBulletStraightParas(master,3,255,false,0,0)
		lib.SetBulletSelfRotation(master,1)
		local relativeRotation = 0
		for _=1,4 do
			local bullet = lib.CreateSimpleBulletById("104151",0,250)
			lib.AttatchToMaster(bullet,master,true)
			local posX = 16 * (math.cos(math.rad(relativeRotation)) * 1 - math.sin(math.rad(relativeRotation)) * 0)
			local posY = 16 * (math.sin(math.rad(relativeRotation)) * 1 - math.cos(math.rad(relativeRotation)) * 0)
			lib.SetAttachmentRelativePos(bullet,posX,posY,relativeRotation,true,true)
			relativeRotation = relativeRotation + 90
		end
	end
end


local BossTable = {}
BossTable.MidBoss = {}
BossTable.MidBoss.Init = function(boss)
	lib.SetBossAni(boss,"2001")
	lib.SetEnemyPos(boss,0,280)
	lib.SetEnemyCollisionParas(boss,32,32)
end
BossTable.MidBoss.Task = function(boss)
	lib.EnemyMoveToPos(boss,0,170,90,Constants.ModeEaseInQuad)
	coroutine.yield(100)
	lib.SetBossCurPhaseData(boss,1,1,1,1,true)
	local sc = lib.CreateSpellCard("Easy-SpellCard",500,60,5)
	lib.SetSpellCardTask(sc,function()
		--圈形子弹的task
		lib.AddEnemyTask(boss,function()
			local k = 1
			for _=1,Infinite do
				local i
				local posX,posY = lib.GetEnemyPos(boss)
				lib.SetGlobalVector2("SC1CircleCache",posX,posY)
				for i=0,15 do
					local bullet = lib.CreateCustomizedBullet("XiongBullet0","0010",posX,posY,i,75,2)
					lib.SetBulletCurvePara(bullet,0,23.5*i*k,0,1*k)
					if coroutine.yield(1)==false then return end
				end
				k = -k
				if coroutine.yield(90)==false then return end
			end
		end)
		--圈形防护罩0
		lib.AddEnemyTask(boss,function()
			local spriteEffect = lib.CreateSpriteEffect("Effects_1",150,150,2000,2000)
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
			local spriteEffect = lib.CreateSpriteEffect("Effects_1",150,150,2000,2000)
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
				local hpRate = lib.GetBossSpellCardHpRate()
				local timeLeftRate = lib.GetBossSpellCardTimeLeftRate()
				local duration = 120 - math.ceil(60*math.min(hpRate,timeLeftRate))
				local posX,posY = lib.GetEnemyPos(boss)
				local enemy = lib.CreateCustomizedEnemy("SC1Enemy","0998",posX,posY,-170,205,duration,3)
				enemy = lib.CreateCustomizedEnemy("SC1Enemy","0998",posX,posY,170,205,duration,3)
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
	end)
	lib.SetSpellCardFinishFunc(sc,function()
		local effect = lib.GetGlobalUserData("SC1Effect0")
		lib.SetEffectFinish(effect)
		lib.RemoveGlobalUserData("SC1Effect0")
		effect = lib.GetGlobalUserData("SC1Effect1")
		lib.SetEffectFinish(effect)
		lib.RemoveGlobalUserData("SC1Effect1")
	end)
	lib.EnterSpellCard(boss,sc)
	--normal难度
	local sc = lib.CreateSpellCard("Normal-SpellCard",500,60,5)
	lib.SetSpellCardTask(sc,function()
		--圈形子弹的task
		lib.AddEnemyTask(boss,function()
			local k = 1
			for _=1,Infinite do
				local i
				local posX,posY = lib.GetEnemyPos(boss)
				lib.SetGlobalVector2("SC1CircleCache",posX,posY)
				for i=0,15 do
					local bullet = lib.CreateCustomizedBullet("XiongBullet0","0010",posX,posY,i,75,2)
					lib.SetBulletCurvePara(bullet,0,24*i*k,0,1.5*k)
					if coroutine.yield(1)==false then return end
				end
				k = -k
				if coroutine.yield(90)==false then return end
			end
		end)
		--圈形防护罩0
		lib.AddEnemyTask(boss,function()
			for _=1,Infinite do
				--等待圈形子弹发射
				if coroutine.yield(80)==false then return end
				local posX,posY = lib.GetGlobalVector2("SC1CircleCache")
				local angle = lib.GetAimToPlayerAngle(posX,posY)
				local vx = 2.5 * math.cos(math.rad(angle))
				local vy = 2.5 * math.sin(math.rad(angle))
				for _=0,131 do
					posX = posX + vx
					posY = posY + vy
					lib.SetGlobalVector2("SC1CircleCenter0",posX,posY)
					if coroutine.yield(1)==false then return end
				end
			end
		end)
		--圈形防护罩1
		lib.AddEnemyTask(boss,function()
			if coroutine.yield(106)==false then return end
			for _=1,Infinite do
				--等待圈形子弹发射
				local posX,posY = lib.GetGlobalVector2("SC1CircleCache")
				if coroutine.yield(80)==false then return end
				local angle = lib.GetAimToPlayerAngle(posX,posY)
				local vx = 2.5 * math.cos(math.rad(angle))
				local vy = 2.5 * math.sin(math.rad(angle))
				for _=0,131 do
					posX = posX + vx
					posY = posY + vy
					lib.SetGlobalVector2("SC1CircleCenter1",posX,posY)
					if coroutine.yield(1)==false then return end
				end
			end
		end)
		--使魔的task
		lib.AddEnemyTask(boss,function()
			if coroutine.yield(300)==false then return end
			for _=1,Infinite do
				local hpRate = lib.GetBossSpellCardHpRate()
				local timeLeftRate = lib.GetBossSpellCardTimeLeftRate()
				local duration = 120 - math.ceil(60*math.min(hpRate,timeLeftRate))
				local posX,posY = lib.GetEnemyPos(boss)
				local enemy = lib.CreateCustomizedEnemy("SC1Enemy","0998",posX,posY,-170,205,duration,3)
				enemy = lib.CreateCustomizedEnemy("SC1Enemy","0998",posX,posY,170,205,duration,3)
				if coroutine.yield(180)==false then return end
				posX,posY = lib.GetEnemyPos(boss)
				enemy = lib.CreateCustomizedEnemy("SC1Enemy","0998",posX,posY,-170,-205,duration,3)
				enemy = lib.CreateCustomizedEnemy("SC1Enemy","0998",posX,posY,170,-205,duration,3)
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
	end)
	lib.EnterSpellCard(boss,sc)
	--hard难度
	local sc = lib.CreateSpellCard("Hard-SpellCard",500,60,5)
	lib.SetSpellCardTask(sc,function()
		--圈形子弹的task
		lib.AddEnemyTask(boss,function()
			local k = 1
			for _=1,Infinite do
				local i
				local posX,posY = lib.GetEnemyPos(boss)
				for i=0,15 do
					local bullet = lib.CreateCustomizedBullet("XiongBullet0","0010",posX,posY,i,75,2)
					lib.SetBulletCurvePara(bullet,0,24*i*k,0,1.5*k)
					if coroutine.yield(1)==false then return end
				end
				k = -k
				if coroutine.yield(90)==false then return end
			end
		end)
		--圈形防护罩0
		lib.AddEnemyTask(boss,function()
			for _=1,Infinite do
				local posX,posY = lib.GetEnemyPos(boss)
				lib.SetGlobalVector2("SC1CircleCenter0",posX,posY)
				--等待圈形子弹发射
				if coroutine.yield(80)==false then return end
				local angle = lib.GetAimToPlayerAngle(posX,posY)
				local vx = 2.5 * math.cos(math.rad(angle))
				local vy = 2.5 * math.sin(math.rad(angle))
				for _=0,131 do
					posX = posX + vx
					posY = posY + vy
					lib.SetGlobalVector2("SC1CircleCenter0",posX,posY)
					if coroutine.yield(1)==false then return end
				end
			end
		end)
		--圈形防护罩1
		lib.AddEnemyTask(boss,function()
			if coroutine.yield(106)==false then return end
			for _=1,Infinite do
				local posX,posY = lib.GetEnemyPos(boss)
				lib.SetGlobalVector2("SC1CircleCenter1",posX,posY)
				--等待圈形子弹发射
				if coroutine.yield(80)==false then return end
				local angle = lib.GetAimToPlayerAngle(posX,posY)
				local vx = 2.5 * math.cos(math.rad(angle))
				local vy = 2.5 * math.sin(math.rad(angle))
				for _=0,131 do
					posX = posX + vx
					posY = posY + vy
					lib.SetGlobalVector2("SC1CircleCenter1",posX,posY)
					if coroutine.yield(1)==false then return end
				end
			end
		end)
		--使魔的task
		lib.AddEnemyTask(boss,function()
			if coroutine.yield(300)==false then return end
			for _=1,Infinite do
				local hpRate = lib.GetBossSpellCardHpRate()
				local timeLeftRate = lib.GetBossSpellCardTimeLeftRate()
				local duration = 120 - math.ceil(60*math.min(hpRate,timeLeftRate))
				local posX,posY = lib.GetEnemyPos(boss)
				local enemy = lib.CreateCustomizedEnemy("SC1Enemy","0998",posX,posY,-170,205,duration,3)
				enemy = lib.CreateCustomizedEnemy("SC1Enemy","0998",posX,posY,170,205,duration,3)
				enemy = lib.CreateCustomizedEnemy("SC1Enemy","0998",posX,posY,-170,-205,duration,3)
				enemy = lib.CreateCustomizedEnemy("SC1Enemy","0998",posX,posY,170,-205,duration,3)
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
	end)
	lib.EnterSpellCard(boss,sc)
	--lunatic难度
	local sc = lib.CreateSpellCard("Lunatic-SpellCard",500,60,5)
	lib.SetSpellCardTask(sc,function()
		--圈形子弹的task1
		lib.AddEnemyTask(boss,function()
			local k = 1
			for _=1,Infinite do
				local i
				local posX,posY = lib.GetEnemyPos(boss)
				for i=0,15 do
					local bullet = lib.CreateCustomizedBullet("XiongBullet0","0010",posX,posY,i,75,2)
					lib.SetBulletCurvePara(bullet,0,24*i*k,0,1.5*k)
					if coroutine.yield(1)==false then return end
				end
				k = -k
				if coroutine.yield(90)==false then return end
			end
		end)
		--圈形子弹的task2
		lib.AddEnemyTask(boss,function()
			local k = -1
			for _=1,Infinite do
				local i
				local posX,posY = lib.GetEnemyPos(boss)
				for i=0,11 do
					local bullet = lib.CreateCustomizedBullet("XiongBullet0","0010",posX,posY,i,37.5,2)
					lib.SetBulletCurvePara(bullet,0,31.5*i*k,0,1.5*k)
					if coroutine.yield(1)==false then return end
				end
				k = -k
				if coroutine.yield(94)==false then return end
			end
		end)
		--圈形防护罩0
		lib.AddEnemyTask(boss,function()
			for _=1,Infinite do
				local posX,posY = lib.GetEnemyPos(boss)
				lib.SetGlobalVector2("SC1CircleCenter0",posX,posY)
				--等待圈形子弹发射
				if coroutine.yield(80)==false then return end
				local angle = lib.GetAimToPlayerAngle(posX,posY)
				local vx = 2.5 * math.cos(math.rad(angle))
				local vy = 2.5 * math.sin(math.rad(angle))
				for _=0,131 do
					posX = posX + vx
					posY = posY + vy
					lib.SetGlobalVector2("SC1CircleCenter0",posX,posY)
					if coroutine.yield(1)==false then return end
				end
			end
		end)
		--圈形防护罩1
		lib.AddEnemyTask(boss,function()
			if coroutine.yield(106)==false then return end
			for _=1,Infinite do
				local posX,posY = lib.GetEnemyPos(boss)
				lib.SetGlobalVector2("SC1CircleCenter1",posX,posY)
				--等待圈形子弹发射
				if coroutine.yield(80)==false then return end
				local angle = lib.GetAimToPlayerAngle(posX,posY)
				local vx = 2.5 * math.cos(math.rad(angle))
				local vy = 2.5 * math.sin(math.rad(angle))
				for _=0,131 do
					posX = posX + vx
					posY = posY + vy
					lib.SetGlobalVector2("SC1CircleCenter1",posX,posY)
					if coroutine.yield(1)==false then return end
				end
			end
		end)
		--使魔的task
		lib.AddEnemyTask(boss,function()
			if coroutine.yield(300)==false then return end
			for _=1,Infinite do
				local hpRate = lib.GetBossSpellCardHpRate()
				local timeLeftRate = lib.GetBossSpellCardTimeLeftRate()
				local duration = 120 - math.ceil(60*math.min(hpRate,timeLeftRate))
				local posX,posY = lib.GetEnemyPos(boss)
				local enemy = lib.CreateCustomizedEnemy("SC1Enemy","0998",posX,posY,-170,205,duration,3)
				enemy = lib.CreateCustomizedEnemy("SC1Enemy","0998",posX,posY,170,205,duration,3)
				enemy = lib.CreateCustomizedEnemy("SC1Enemy","0998",posX,posY,-170,-205,duration,3)
				enemy = lib.CreateCustomizedEnemy("SC1Enemy","0998",posX,posY,170,-205,duration,3)
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
	end)
	lib.EnterSpellCard(boss,sc)
	
	--lib.PlayBossAni(boss,Constants.ActionTypeCast,Constants.DirNull,90)
	--coroutine.yield(100)
	--lib.EnemyMoveToPos(boss,170,170,90,Constants.ModeEaseInQuad)
	lib.SetBossCurPhaseData(boss,1,1,1,true)
	sc = lib.CreateSpellCard("Non-SpellCard",5,50,2)
	lib.SetSpellCardTask(sc,function()
		lib.AddEnemyTask(boss,function()
			local j,posX,posY,beginAngle
			for _=1,Infinite do
				for _=1,10 do
					posX,posY = lib.GetEnemyPos(boss)
					beginAngle = lib.GetAimToPlayerAngle(posX,posY)
					for j=0,19 do
						lib.PlaySound("se_tan00",false)
						local bullet = lib.CreateSimpleBulletById("0154",posX,posY)
						lib.SetBulletStraightParas(bullet,5,beginAngle+j*18,true,0,0)
					end
					if ( coroutine.yield(15)==false ) then return end
				end
				if ( coroutine.yield(60)==false ) then return end
			end
		end)
		lib.AddEnemyTask(boss,function()
			local posX,posY,nextX
			for _=1,Infinite do
				posX,posY = lib.GetEnemyPos(boss)
				if posX <= 0 then nextX = lib.GetRandomFloat(posX+50,130) end
				if posX > 0 then nextX = lib.GetRandomFloat(-130,posX-50) end
				lib.EnemyMoveToPos(boss,nextX,posY,90,Constants.ModeLinear)
				if ( coroutine.yield(100)==false ) then return end
			end
		end)
	end)
	lib.EnterSpellCard(boss,sc)
	
	sc = lib.CreateSpellCard("Non-SpellCard",5,60,2)
	lib.SetSpellCardTask(sc,function()
		lib.AddEnemyTask(boss,function()
			local j,tmpPosX,tmpPosY
			local posX,posY = lib.GetEnemyPos(boss)
			for _=1,Infinite do
				for j=0,19 do
					lib.PlaySound("se_tan00",false)
					tmpPosX = lib.GetRandomFloat(-190+20*j,-170+20*j)
					tmpPosY = lib.GetRandomFloat(-20,20) + posY
					local bullet = lib.CreateSimpleBulletById("0218",tmpPosX,tmpPosY)
					lib.SetBulletStraightParas(bullet,0,90,false,0,0)
					lib.DoBulletAccelerationWithLimitation(bullet,0.07,Constants.VelocityAngle,60)
					bullet = lib.CreateSimpleBulletById("0218",tmpPosX,tmpPosY)
					lib.SetBulletStraightParas(bullet,0,-90,false,0,0)
					lib.DoBulletAccelerationWithLimitation(bullet,0.07,Constants.VelocityAngle,60)
				end
				if ( coroutine.yield(30)==false ) then return end
			end
		end)
		lib.AddEnemyTask(boss,function()
			local posX,posY = lib.GetEnemyPos(boss)
			local tmp
			for _=1,Infinite do
				tmp = lib.GetRandomInt(-1,2)
				if tmp ~= 0 then
					posX,posY = lib.GetEnemyPos(boss)
					posX = posX + tmp * lib.GetRandomFloat(25,50)
					if posX > 150 then posX = 150 end
					if posX < -150 then posX = -150 end
					lib.EnemyMoveToPos(boss,posX,posY,60,Constants.ModeLinear)
				end
				if ( coroutine.yield(90)==false ) then return end
			end
		end)
	end)
	lib.EnterSpellCard(boss,sc)
	
	sc = lib.CreateSpellCard("SpellCard",500,60,2)
	lib.SetSpellCardTask(sc,function()
		if ( coroutine.yield(60)==false ) then return end
		lib.EnemyMoveToPos(boss,150,180,50,Constants.ModeLinear)
		if ( coroutine.yield(60)==false ) then return end
		--BOSS加速横移
		lib.AddEnemyTask(boss,function()
			local k = -1
			for _=1,Infinite do
				lib.EnemyMoveToPos(boss,k*150,150,120,Constants.ModeEaseInQuad)
				k = -k
				if ( coroutine.yield(220)==false ) then return end
			end
		end)
		--扫射激光
		lib.AddEnemyTask(boss,function()
			for _=1,Infinite do
				local laser = lib.CreateCustomizedLaser("NazrinLaser","etama_236",boss,-70,180,250,120,5)
				laser = lib.CreateCustomizedLaser("NazrinLaser","etama_236",boss,250,180,-70,120,5)
				if ( coroutine.yield(220)==false ) then return end
			end
		end)
		--生成从上而下的子弹
		lib.AddEnemyTask(boss,function()
			local k = 0
			local posX,posY
			for _=1,Infinite do
				if k < 120 then
					if k % 2 == 0 then
						posX,posY = lib.GetEnemyPos(boss)
						posX = posX + lib.GetRandomFloat(-10,10)
						local bullet = lib.CreateSimpleBulletById("0172",posX,225)
						local vy = lib.GetRandomFloat(2,3)
						lib.SetBulletStraightParas(bullet,vy,-90,false,0,0)
					end
				end
				if k >= 220 then k = 0 end
				k = k + 1
				if ( coroutine.yield(1)==false ) then return end
			end
		end)
	end)
	lib.EnterSpellCard(boss,sc)
end

function Stage.StageTask()
	lib.PlaySound("bgm",true)
	--local spriteEffect = lib.CreateSpriteEffectWithProps("STGCommonAtlas","Circle",eBlendMode.Normal,eEffectLayer.Bottom,false,0)
	--lib.SetEffectToPos(spriteEffect,0,0)
	--lib.SetSpriteEffectScale(spriteEffect,5,5)
	--lib.SetSpriteEffectColor(spriteEffect,0.55,0.45,0.65,0.75)
	--lib.SetSpriteEffectColor(spriteEffect,0,0,0,0.9)
	--local collider = lib.CreateObjectColliderByType(eColliderType.Circle)
	--lib.SetObjectColliderSize(collider,80,80)
	--lib.SetObjectColliderToPos(collider,0,0)
	--lib.SetObjectColliderColliderGroup(collider,eColliderGroup.PlayerBullet)
	--
	if coroutine.yield(200) == false then return end
	do
		--local enemy = lib.CreateNormalEnemyById("0000",0,185);
		local enemy = lib.CreateCustomizedEnemy("TestKillEnemy","0000",0,185,0)
		lib.AddEnemyTask(enemy,function()
			if coroutine.yield(150)==false then return end
			lib.EnemyMoveTowards(enemy,1,315,200);
			if coroutine.yield(550)==false then return end
			lib.EnemyMoveTowards(enemy,0.5,180,700);
		end)
		lib.AddEnemyTask(enemy,function()
			if ( coroutine.yield(10000)==false ) then return end
			for _=1,10 do
				for _=1,6 do
					lib.PlaySound("se_tan00",false)
					local posX,posY = lib.GetEnemyPos(enemy)
					local bullet
					do local angle=-35 for _=1,3 do
						bullet = lib.CreateSimpleBulletById("104060",posX,posY)
						--lib.SetBulletStraightParas(bullet,0.5,angle,true,0.25,Constants.VelocityAngle)
						lib.SetBulletStraightParas(bullet,2,angle,true,0.25,Constants.VelocityAngle)
						angle = angle+35
					end end
					if ( coroutine.yield(5)==false ) then return end
				end
				if ( coroutine.yield(100)==false ) then return end
			end
		end)
		--测试直线激光
		lib.AddEnemyTask(enemy,function()
			--if coroutine.yield(10000) == false then return end
			local laser,angle,i
			angle = lib.GetAimToPlayerAngle(lib.GetEnemyPos(enemy))
			for _=1,Infinite do
				angle = lib.GetAimToPlayerAngle(lib.GetEnemyPos(enemy))
				for i=0,18 do
					local posX,posY = lib.GetEnemyPos(enemy)
					laser = lib.CreateLinearLaser("304060",45,posX,posY)
					lib.SetLinearLaserHeadEnable(laser,true)
					lib.LinearLaserDoStraightMove(laser,3,angle+i*20,0.02,60)
					if ( coroutine.yield(3) == false ) then return end
				end
			end
		end)
		--测试自定义直线激光
		lib.AddEnemyTask(enemy,function()
			if coroutine.yield(10000) == false then return end
			local laser,i
			for _=1,5 do
				local posX,posY = lib.GetEnemyPos(enemy)
				for i=0,10 do
					laser = lib.CreateCustomizedLinearLaser("ReboundLinearLaser",posX,posY,15+i*15,7,5,5)
				end
				if ( coroutine.yield(60) == false ) then return end
			end
		end)
		--测试曲线激光
		lib.AddEnemyTask(enemy,function()
			if coroutine.yield(10000) == false then return end
			local laser,i
			for i=1,18 do
				local posX,posY = lib.GetEnemyPos(enemy)
				local angle = lib.GetAimToPlayerAngle(posX,posY)
				laser = lib.CreateCurveLaser("401100",45,posX,posY)
				lib.SetCurveLaserCurveParas(laser,0,i*20,3,3)
				lib.SetCurveLaserWidth(laser,5,3)
				if coroutine.yield(5) == false then return end
			end
		end)
	end
	if coroutine.yield(300) == false then return end
	do
		local boss = lib.CreateBoss("MidBoss")
		lib.EnemyMoveToPos(boss,0,170,90,Constants.ModeEaseInQuad)
		if coroutine.yield(100) == false then return end
		lib.SetBossCurPhaseData(boss,1,1,1,1,true)
		lib.StartSpellCard(SpellCard.PatchouliSC1,boss)
		--lib.StartSpellCard(SpellCard.NazrinSC2_0,boss)
		if lib.WaitForSpellCardFinish() == false then return end
		lib.StartSpellCard(SpellCard.PatchouliNonSC1,boss)
		if lib.WaitForSpellCardFinish() == false then return end
		lib.StartSpellCard(SpellCard.NazrinSC2_2,boss)
		if lib.WaitForSpellCardFinish() == false then return end
		lib.StartSpellCard(SpellCard.NazrinSC2_3,boss)
		if lib.WaitForSpellCardFinish() == false then return end
		lib.SetBossCurPhaseData(boss,1,1,1,1,true)
		lib.StartSpellCard(SpellCard.NazrinSC1_0,boss)
		if lib.WaitForSpellCardFinish() == false then return end
		lib.StartSpellCard(SpellCard.NazrinSC1_1,boss)
		if lib.WaitForSpellCardFinish() == false then return end
		lib.StartSpellCard(SpellCard.NazrinSC1_2,boss)
		if lib.WaitForSpellCardFinish() == false then return end
		lib.StartSpellCard(SpellCard.NazrinSC1_3,boss)
		if lib.WaitForSpellCardFinish() == false then return end
	end
	if coroutine.yield(10000) == false then return end
	--自定义子弹
	do
		local enemy = lib.CreateNormalEnemyById("0001",0,280)
		lib.AddEnemyTask(enemy,function()
			local continue,tmpEnemy
			tmpEnemy = enemy
			lib.EnemyMoveTowards(tmpEnemy,2,-90,50)
			continue = coroutine.yield(60);
			if ( continue==false ) then return end
			lib.AddEnemyTask(tmpEnemy,function()
				local posX,posY = lib.GetEnemyPos(tmpEnemy)
				for _=1,999 do
					local angle = lib.GetAimToPlayerAngle(posX,posY)
					for _=1,30 do
						lib.PlaySound("se_tan00",false)
						local bullet = lib.CreateCustomizedBullet("SinBullet","0172",posX,posY,7,angle,true,3)
						bullet = lib.CreateCustomizedBullet("SinBullet","0172",posX,posY,7,angle,false,3)
						--lib.SetBulletCurvePara(bullet,10,i*18,1.5,-0.5)
						continue = coroutine.yield(3);
						if ( continue==false ) then return end
					end
				end
				continue = coroutine.yield(120);
				if ( continue==false ) then return end
			end)
		end)
	end
	coroutine.yield(600)
	--不动的直线激光，生成3敌机，生成露米娅的月光符
	do
		local i
		for i=2,2 do
			local enemy
			enemy = lib.CreateNormalEnemyById("0010",-240+i*120,280);
			lib.AddEnemyTask(enemy,function()
				local continue,tmpEnemy
				tmpEnemy = enemy
				lib.EnemyMoveTowards(tmpEnemy,2,-90,50)
				continue = coroutine.yield(60);
				if ( continue==false ) then return end
				--每90帧发射一圈子弹
				lib.AddEnemyTask(tmpEnemy,function()
					local tmpEnemy0,continue0,j
					tmpEnemy0 = tmpEnemy
					local posX,posY = lib.GetEnemyPos(tmpEnemy0)
					for _=1,999 do
						for j=1,20 do
							lib.PlaySound("se_tan00",false)
							local bullet = lib.CreateSimpleBulletById("0172",posX,posY)
							lib.SetBulletStraightParas(bullet,2,j*18,true,0,0)
						end
						continue = coroutine.yield(90);
						if ( continue==false ) then return end
					end
				end)
				--直线激光
				lib.AddEnemyTask(tmpEnemy,function()
					local posX,posY = lib.GetEnemyPos(tmpEnemy)
					for _=1,999 do
						local laser = lib.CreateLaser("etama_236",posX,posY,0,5,300,300)
						lib.SetLaserRotatePara(laser,-80,150)
						lib.SetLaserCollisionDetectParas(laser,3,300)
						laser = lib.CreateLaser("etama_236",posX,posY,180,5,300,300)
						lib.SetLaserRotatePara(laser,260,150)
						lib.SetLaserCollisionDetectParas(laser,3,300)
						continue = coroutine.yield(350);
						if ( continue==false ) then return end
					end
				end)
			end)
		end
	end
	coroutine.yield(600);
	--5层交叉弹
	do
		local enemy = lib.CreateNormalEnemyById("0001",0,280)
		lib.AddEnemyTask(enemy,function()
			local continue,tmpEnemy
			tmpEnemy = enemy
			lib.EnemyMoveTowards(tmpEnemy,2,-90,50)
			continue = coroutine.yield(60);
			if ( continue==false ) then return end
			lib.AddEnemyTask(tmpEnemy,function()
				local tmpEnemy0,continue0,i
				tmpEnemy0 = tmpEnemy
				local posX,posY = lib.GetEnemyPos(tmpEnemy0)
				for _=1,5 do
					for i=1,20 do
						lib.PlaySound("se_tan00",false)
						local bullet = lib.CreateSimpleBulletById("0172",posX,posY)
						lib.SetBulletCurvePara(bullet,10,i*18,1.5,0.5)
						local bullet = lib.CreateSimpleBulletById("0172",posX,posY)
						lib.SetBulletCurvePara(bullet,10,i*18,1.5,-0.5)
					end
					continue = coroutine.yield(60);
					if ( continue==false ) then return end
				end
			end)
		end)
	end
	coroutine.yield(600);
	--依次生成8个敌机，每个敌机都会放出自机狙
	do
		local i
		--每隔60帧生成1个敌机
		for i=1,8 do
			local enemy,continue
			enemy = lib.CreateNormalEnemyById("0000",-240+i*40,280);
			lib.AddEnemyTask(enemy,function()
				local continue,tmpEnemy
				tmpEnemy = enemy;
				lib.EnemyMoveTowards(tmpEnemy,1,270,60);		
			end)
			--敌机子弹10连发的3弹自机狙
			lib.AddEnemyTask(enemy,function()
				local tmpEnemy,continue;
				tmpEnemy = enemy
				continue = coroutine.yield(60);
				if ( continue==false ) then return end
				for _=1,10 do
					for _=1,6 do
						lib.PlaySound("se_tan00",false)
						local posX,posY = lib.GetEnemyPos(tmpEnemy)
						local bullet
						do local angle=-35 for _=1,3 do
							bullet = lib.CreateSimpleBulletById("0172",posX,posY)
							lib.SetBulletStraightParas(bullet,3,angle,true,0,0)
							angle = angle+35
						end end
						continue = coroutine.yield(5);
						if ( continue==false ) then return end
					end
					continue = coroutine.yield(100);
					if ( continue==false ) then return end
				end
			end)
			continue = coroutine.yield(60);
			if ( continue==false ) then return end
		end
	end
end

return
{
	CustomizedBulletTable = CustomizedTable,
	CustomizedEnemyTable = CustomizedEnemyTable,
	BossTable = BossTable,
	Stage = Stage,
}