local lib = require "LuaLib"
local consts = Constants
local Stage = {}
local CustomizedTable = {}
local CustomizedEnemyTable = {}
local BossTable = {}
local CustomizedSTGObjectTable = {}

-- Mod name: unnamed
--author="YK"
--allow_practice=true
CustomizedEnemyTable["TestOnKillEnemy"] = {}
CustomizedEnemyTable["TestOnKillEnemy"].Init = function(self)
    self:Init(100000)
    self:SetMaxHp(50)
    self:AddTask(function()
        if Wait(150)==false then return end
        self:MoveTowards(1,315,false,200)
        if Wait(550)==false then return end
        self:MoveTowards(0.5,180,false,700)
    end)
end
CustomizedEnemyTable["TestOnKillEnemy"].OnKill = function(self)
    local posX,posY = self.x,self.y
    last = CreateSimpleBulletById(122151,posX,posY)
    last:SetStraightParas(3,0,true,0,0)
    local master = last
    lib.SetBulletSelfRotation(master,1)
    do local relativeRotation,_d_relativeRotation=(0),(90) for _=1,4 do
        last = CreateSimpleBulletById(104151,0,250)
        last:SetStraightParas(0,0,false,0,0)
        last:AttachTo(master,true)
        last:SetRelativePos(16 * ( cos(relativeRotation) * 1 -  sin(relativeRotation) * 0),16 * ( sin(relativeRotation) * 1 -  cos(relativeRotation) * 0),relativeRotation,true,true)
    relativeRotation=relativeRotation+_d_relativeRotation end end
end
CustomizedTable["YKBullet"] = {}
CustomizedTable["YKBullet"].Init = function(self,v,angle)
    self:SetStyleById()
    self:SetV(3,0,false)
    self:SetAcce(0.05,0,false)
    self:AddTask(function()
        if Wait(60)==false then return end
        self:MoveTo(0,0,60,IntModeLinear)
        if Wait(60)==false then return end
    end)
end
BossTable["Marisa"] = {}
BossTable["Marisa"].Init = function(self)
    self:SetAni(2001)
    self:SetPos(0,280)
    self:SetCollisionSize(32,32)
end
SpellCard["TestSC"] = {}
SpellCard["TestSC"].Init = function(boss)
    SetSpellCardProperties("unknown spell card",1,60,ConditionEliminateAll,true)
end
SpellCard["TestSC"].OnFinish = function(boss)
end
SpellCard["天琴「Orionid Meteor Shower」"] = {}
SpellCard["天琴「Orionid Meteor Shower」"].Init = function(boss)
    SetSpellCardProperties("天琴「Orionid Meteor Shower」",1,60,ConditionEliminateAll,true)
    boss:SetMaxHp(1200)
    boss:SetInvincible(5)
    boss:ShowBloodBar(true)
    last = CreateCustomizedSTGObject("NazrinCG",0,0)
    boss:AddTask(function()
        if Wait(90)==false then return end
    end)
end
SpellCard["天琴「Orionid Meteor Shower」"].OnFinish = function(boss)
end
CustomizedSTGObjectTable["NazrinCG"] = {}
CustomizedSTGObjectTable["NazrinCG"].Init = function(self)
    self:SetSprite("Characters/Nazrin","Nazrin",BlendMode_Normal,LayerEffectBottom,false)
    self:SetPos(200,200)
    self:AddTask(function()
        self:MoveTo(0,0,30,IntModeEaseInQuad)
        if Wait(60)==false then return end
        self:MoveTo(-200,-200,60,IntModeEaseOutQuad)
        if Wait(30)==false then return end
        self:ChangeAlphaTo(0,30)
        if Wait(30)==false then return end
        DelUnit(self)
    end)
end
CustomizedTable["NazrinSC1Bullet0"] = {}
CustomizedTable["NazrinSC1Bullet0"].Init = function(self,waitTime,maxRadius)
    self:SetStyleById(113020)
    self:SetResistEliminatedTypes(23)
    self:AddTask(function()
        local posX = self.x
        local posY = self.y
        self:ChangeProperty(Prop_CurveRadius,ChangeModeChangeTo,0,maxRadius,0,0,0,30,IntModeLinear,1,0)
        if Wait(80-waitTime)==false then return end
        self:SetStyleById(107060)
        local angle = lib.GetAimToPlayerAngle(posX,posY)
        self:SetV(2.5,angle,false)
    end)
end
CustomizedEnemyTable["NazrinSC1Enemy"] = {}
CustomizedEnemyTable["NazrinSC1Enemy"].Init = function(self,toPosX,toPosY,duration)
    self:Init(100020)
    self:SetMaxHp(50)
    self:SetInteractive(false)
    self:AddTask(function()
        self:MoveTo(toPosX,toPosY,60,IntModeEaseInQuad)
        if Wait(60+duration)==false then return end
        KillUnit(self,true)
    end)
    self:AddTask(function()
        if Wait(60)==false then return end
        do for _=1,Infinite do
            if RandomInt(0,1) == 0 then
                last = CreateSimpleBulletById(118010,self.x,self.y)
                last:SetStraightParas(12,RandomInt(-45,45),true,0,0)
            else
                last = CreateSimpleBulletById(113010,self.x,self.y)
                last:SetStraightParas(12,RandomInt(-45,45),true,0,0)
            end
            if Wait(2)==false then return end
        end end
    end)
end
CustomizedSTGObjectTable["NazrinShield"] = {}
CustomizedSTGObjectTable["NazrinShield"].Init = function(self,posX,posY,waitTime)
    self:SetSprite("STGEffectAtlas","TransparentCircle",BlendMode_Normal,LayerEffectBottom,false)
    self:SetSize(160,160)
    self:SetPos(2000,2000)
    self:SetColor(0.55,0.45,0.65,0.75)
    last = CreateSimpleCollider(TypeCircle)
    last:SetPos(2000,2000)
    last:SetSize(80,80)
    last:SetCollisionGroup(8)
    last:AttachTo(self,true)
    last:SetRelativePos(0,0,0,true,true)
    self:AddTask(function()
        if Wait(waitTime)==false then return end
        self:SetPos(posX,posY)
        self:SetV(2.5,0,true)
    end)
end
SpellCard["NazrinSC1_0"] = {}
SpellCard["NazrinSC1_0"].Init = function(boss)
    SetSpellCardProperties("Easy-SpellCard",1,60,ConditionEliminateAll,true)
    last = CreateCustomizedSTGObject("NazrinCG",0,0)
    boss:SetMaxHp(500)
    boss:SetInvincible(5)
    boss:ShowBloodBar(true)
    --圈形子弹
    boss:AddTask(function()
        local k = 1
        do for _=1,Infinite do
            local posX,posY = boss.x,boss.y
            last = CreateCustomizedBullet("NazrinSC1Bullet0",posX,posY,0,75)
            local master = last
            last:SetPolarParas(0,23.5*0*k,0,1*k)
            last = CreateCustomizedSTGObject("NazrinShield",0,0,posX,posY,80)
            last:AttachTo(master,true)
            do local i,_d_i=(1),(1) for _=1,15 do
                if Wait(1)==false then return end
                last = CreateCustomizedBullet("NazrinSC1Bullet0",posX,posY,i,75)
                last:SetPolarParas(0,23.5*i*k,0,1*k)
            i=i+_d_i end end
            k = -k
            if Wait(90)==false then return end
        end end
    end)
    --servants
    boss:AddTask(function()
        if Wait(300)==false then return end
        do for _=1,Infinite do
            local duration = 120
            last = CreateCustomizedEnemy("NazrinSC1Enemy",boss.x,boss.y,-170,205,duration)
            last = CreateCustomizedEnemy("NazrinSC1Enemy",boss.x,boss.y,170,205,duration)
            if Wait(180)==false then return end
        end end
    end)
    --wander
    boss:AddTask(function()
        boss:SetWanderRange(-150,150,80,125)
        boss:SetWanderAmplitude(0,100,0,15)
        boss:SetWanderMode(IntModeLinear,MoveXTowardsPlayer)
        if Wait(300)==false then return end
        do for _=1,Infinite do
            boss:Wander(120)
            if Wait(420)==false then return end
        end end
    end)
end
SpellCard["NazrinSC1_0"].OnFinish = function(boss)
end
CustomizedTable["ReboundLinearLaser"] = {}
CustomizedTable["ReboundLinearLaser"].Init = function(self,angle,reboundPara,reboundCount)
    self:SetStyleById(202060)
    self:SetLength(45)
    self:SetSourceEnable(true)
    self:SetHeadEnable(true)
    self:SetV(3.5,0,false)
    self:AddTask(function()
        do for _=1,Infinite do
            if reboundCount > 0 then
                local reboundFlag = false
                local curPosX,curPosY = self.x,self.y
                local tmpRebound = reboundPara
                local reboundAngle = self.vAngle
                if tmpRebound >= Constants.ReboundBottom and curPosY < -224 then
                    reboundFlag = true
                    reboundAngle = -reboundAngle
                    curPosY = -448 - curPosY
                    tmpRebound = tmpRebound - Constants.ReboundBottom
                else
                end
                if tmpRebound >= Constants.ReboundTop and curPosY > 224 then
                    reboundFlag = true
                    reboundAngle = -reboundAngle
                    curPosY = 448 - curPosY
                    tmpRebound = tmpRebound - Constants.ReboundTop
                else
                end
                if tmpRebound >= Constants.ReboundRight and curPosX > 192 then
                    reboundFlag = true
                    reboundAngle = 180 - reboundAngle
                    curPosX = 384 - curPosX
                    tmpRebound = tmpRebound - Constants.ReboundRight
                else
                end
                if tmpRebound >= Constants.ReboundLeft and curPosX < -192 then
                    reboundFlag = true
                    reboundAngle = -180 - reboundAngle
                    curPosX = -384 - curPosX
                else
                end
                if reboundFlag == true then
                    reboundCount = reboundCount - 1
                    reboundCount = 0
                    last = CreateCustomizedLinearLaser("ReboundLinearLaser",curPosX,curPosY,reboundAngle,reboundPara,reboundCount)
                else
                end
            else
            end
            if Wait(1)==false then return end
        end end
    end)
end
SpellCard["TestCustomizedLinearLaser"] = {}
SpellCard["TestCustomizedLinearLaser"].Init = function(boss)
    SetSpellCardProperties("unknown spell card",1,60,ConditionEliminateAll,false)
    boss:SetMaxHp(500)
    boss:SetInvincible(5)
    boss:ShowBloodBar(true)
    boss:AddTask(function()
        do for _=1,Infinite do
            local posX,posY = boss.x,boss.y
            do local i,_d_i=(0),(1) for _=1,11 do
                last = CreateCustomizedLinearLaser("ReboundLinearLaser",posX,posY,15+i*15,7,5)
            i=i+_d_i end end
            if Wait(60)==false then return end
        end end
    end)
end
SpellCard["TestCustomizedLinearLaser"].OnFinish = function(boss)
end
CustomizedTable["竖版开海"] = {}
CustomizedTable["竖版开海"].Init = function(self,laserAngle)
    self:SetStyleById(305051)
    self:SetLength(20)
    self:SetSourceEnable(false)
    self:SetHeadEnable(true)
    self:SetV(7.5,laserAngle,false)
end
CustomizedSTGObjectTable["竖版开海"] = {}
CustomizedSTGObjectTable["竖版开海"].Init = function(self)
    self:SetSprite("STGEffectAtlas","MapleLeaf1",BlendMode_Normal,LayerEffectNormal,false)
    self:AddTask(function()
        do for _=1,Infinite do
            local leftPosX,rightPosX = -185,185
            do local i,_d_i=(1),(1) for _=1,15 do
                local k = i / 15
                k = k * k
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX + 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX - 85.5 * k,224,270)
                if Wait(2)==false then return end
            i=i+_d_i end end
            leftPosX,rightPosX = -99.5,99.5
            do local i,_d_i=(1),(1) for _=1,15 do
                local k = i / 15
                k = -k * k + 2 * k
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX + 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX - 85.5 * k,224,270)
                if Wait(2)==false then return end
            i=i+_d_i end end
            leftPosX,rightPosX = -14,14
            do local i,_d_i=(1),(1) for _=1,15 do
                local k = i / 15
                k = k * k
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX - 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX + 85.5 * k,224,270)
                if Wait(2)==false then return end
            i=i+_d_i end end
            leftPosX,rightPosX = -99.5,99.5
            do local i,_d_i=(1),(1) for _=1,15 do
                local k = i / 15
                k = -k * k + 2 * k
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX - 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX + 85.5 * k,224,270)
                if Wait(2)==false then return end
            i=i+_d_i end end
        end end
    end)
end
CustomizedTable["ColliderTriggerLaser"] = {}
CustomizedTable["ColliderTriggerLaser"].Init = function(self,angle)
    self:SetStyleById(201061)
    self:SetSize(0,20)
    self.rot = angle
    self.orderInLayer = 1
    local createCounter = 0
    local hitCollider,minDis = nil,1000
    self:AddTask(function()
        self:TurnHalfOn(5,20)
        if Wait(150)==false then return end
        self:TurnOn(50)
    end)
    self:AddTask(function()
        do local laserAngle,_d_laserAngle=(angle),(0.2) for _=1,Infinite do
            self.rot = laserAngle
            createCounter = createCounter + 1
            if self.length < 400 then
                self.length = self.length + 2.5
            else
            end
            if Wait(1)==false then return end
        laserAngle=laserAngle+_d_laserAngle end end
    end)
    self:AddTask(function()
        if Wait(200)==false then return end
        self:AddColliderTrigger(2048,function(collider,collIndex)
            local tmpDis = Distance(self,collider)
            if tmpDis < minDis then
                hitCollider = collider
                minDis = tmpDis
            else
            end
        end)
    end)
    self:AddTask(function()
        do for _=1,Infinite do
            if hitCollider ~= nil and createCounter % 20 == 0 then
                last = CreateSimpleBulletById(126050,hitCollider.x,hitCollider.y)
                last:SetStraightParas(1,self.rot,false,0,UseVelocityAngle)
            else
            end
            if self.length > minDis then
                self:SetLength(minDis)
            else
            end
            hitCollider = nil
            minDis = 1000
            if Wait(1)==false then return end
        end end
    end)
end
CustomizedTable["TestColliderBullet"] = {}
CustomizedTable["TestColliderBullet"].Init = function(self,dir)
    self:SetStyleById(118050)
    self:AddTask(function()
        self:SetPolarParas(30,0,3,dir,0,30)
        if Wait(20)==false then return end
        self:SetPolarParas(90,20 * dir,0.25,dir,0,30)
    end)
    last = CreateSimpleCollider(TypeCircle)
    last:SetPos(0,2000)
    last:SetSize(14,14)
    last:SetCollisionGroup(2048)
    last:AttachTo(self,true)
    last:SetRelativePos(0,0,0,false,true)
end
SpellCard["TestColliderTriggerSC"] = {}
SpellCard["TestColliderTriggerSC"].Init = function(boss)
    SetSpellCardProperties("TestColliderTriggerSC",1,60,ConditionEliminateAll,true)
    boss:SetMaxHp(500)
    boss:SetInvincible(5)
    boss:ShowBloodBar(true)
    local dir = 1
    boss:MoveTo(0,30,60,IntModeEaseOutQuad)
    boss:AddTask(function()
        if Wait(60)==false then return end
        do local laserAngle,_d_laserAngle=(0),(360/14) for _=1,14 do
            last = CreateCustomizedLaser("ColliderTriggerLaser",0,30,laserAngle)
        laserAngle=laserAngle+_d_laserAngle end end
        do for _=1,Infinite do
            dir = -dir
            do local curveAngle,_d_curveAngle=(RandomFloat(0,360)),(-30 * dir) for _=1,6 do
                last = CreateCustomizedBullet("TestColliderBullet",0,2000,dir)
                if Wait(30)==false then return end
            curveAngle=curveAngle+_d_curveAngle end end
            if Wait(150)==false then return end
        end end
    end)
end
SpellCard["TestColliderTriggerSC"].OnFinish = function(boss)
end
LoadSound("oborozuki")
LoadSound("se_tan00")
CustomizedEnemyTable["YKStage1Enemy0"] = {}
CustomizedEnemyTable["YKStage1Enemy0"].Init = function(self,angle)
    self:Init(100000)
    self:SetMaxHp(20)
    self:SetDropItems(1,1,48,48)
    self:AddTask(function()
        self:SetV(2,angle,false)
        if Wait(71)==false then return end
        do for _=1,100 do
            do local rot,_d_rot=(-60),(30) for _=1,5 do
                last = CreateSimpleBulletById(117050,self.x,self.y)
                last:SetStraightParas(5,rot,true,0,UseVelocityAngle)
                PlaySound("se_tan02",0.5,false)
            rot=rot+_d_rot end end
            if Wait(71)==false then return end
            do local rot,_d_rot=(-60),(40) for _=1,4 do
                last = CreateSimpleBulletById(117130,self.x,self.y)
                last:SetStraightParas(5,rot,true,0,UseVelocityAngle)
            rot=rot+_d_rot end end
            if Wait(71)==false then return end
        end end
    end)
end
CustomizedEnemyTable["YKStage1Enemy0"].OnKill = function(self)
    local posX,posY = self.x,self.y
    last = CreateSimpleBulletById(122151,posX,posY)
    last:SetStraightParas(3,0,true,0,0)
    local master = last
    master.omega = 1
    do local relativeRotation,_d_relativeRotation=(0),(72) for _=1,5 do
        last = CreateSimpleBulletById(104151,0,250)
        last:SetStraightParas(0,0,false,0,0)
        last:AttachTo(master,true)
        last:SetRelativePos(16 * ( cos(relativeRotation) * 1 -  sin(relativeRotation) * 0),16 * ( sin(relativeRotation) * 1 -  cos(relativeRotation) * 0),relativeRotation,true,true)
    relativeRotation=relativeRotation+_d_relativeRotation end end
end
CustomizedEnemyTable["YKStage1Enemy1"] = {}
CustomizedEnemyTable["YKStage1Enemy1"].Init = function(self,life,attach)
    self:Init(100010)
    self:SetMaxHp(life)
    self:SetDropItems(1,2,32,32)
    self:MoveTo(self.x,144,60,IntModeLinear)
    self:AddTask(function()
        if Wait(60)==false then return end
        do for _=1,Infinite do
            do local r,_d_r=(0),(18) local R,_d_R=(10),(0.1) local a,_d_a=(0),(2.1) for _=1,300 do
                last = CreateSimpleBulletById(104060,R*cos(270-r)+self.x,R*sin(270-r)+self.y)
                last:SetStraightParas(1,90-a,false,0.005,90-a)
                if attach then
                    last:AttachTo(self,true)
                else
                end
                last.orderInLayer = 1
                last = CreateSimpleBulletById(104020,R*cos(270+r)+self.x,R*sin(270+r)+self.y)
                last:SetStraightParas(1,90+a,false,0.005,90+a)
                if attach then
                    last:AttachTo(self,true)
                else
                end
                last.orderInLayer = 1
                PlaySound("se_tan00",0.05,false)
                if Wait(1)==false then return end
            r=r+_d_r R=R+_d_R a=a+_d_a end end
            if Wait(60)==false then return end
        end end
    end)
    self:AddTask(function()
        if Wait(60)==false then return end
        do for _=1,Infinite do
            if Wait(10)==false then return end
            do local r,_d_r=(0),(18*15) local R,_d_R=(10),(0.1 * 15) local a,_d_a=(0),(2.1 * 15) for _=1,19 do
                last = CreateSimpleBulletById(118050,self.x,self.y)
                last:SetStraightParas(2,90-r-a,false,0,0)
                if attach then
                    last:AttachTo(self,true)
                else
                end
                last = CreateSimpleBulletById(118010,self.x,self.y)
                last:SetStraightParas(2,90+r+a,false,0,0)
                if attach then
                    last:AttachTo(self,true)
                else
                end
                if Wait(15)==false then return end
            r=r+_d_r R=R+_d_R a=a+_d_a end end
            if Wait(65)==false then return end
        end end
    end)
end
CustomizedTable["YKStage1Enemy2Bullet"] = {}
CustomizedTable["YKStage1Enemy2Bullet"].Init = function(self,id,v,angle)
    self:SetStyleById(id)
    self:SetV(v,angle,false)
    self:AddRebound(7,2)
end
CustomizedEnemyTable["YKStage1Enemy2"] = {}
CustomizedEnemyTable["YKStage1Enemy2"].Init = function(self,dir)
    self:Init(100000)
    self:SetMaxHp(200)
    --自机狙
    self:MoveTo(self.x,40,180,IntModeLinear)
    self:AddTask(function()
        if Wait(10)==false then return end
        do for _=1,40 do
            PlaySound("se_tan00",0.05,false)
            last = CreateSimpleBulletById(105130,self.x,self.y)
            last:SetStraightParas(2.5,0,true,0,0)
            last = CreateSimpleBulletById(104030,self.x,self.y)
            last:SetStraightParas(2.5,10,true,0,0)
            last = CreateSimpleBulletById(104030,self.x,self.y)
            last:SetStraightParas(2.5,-10,true,0,0)
            if Wait(10)==false then return end
        end end
    end)
    self:AddTask(function()
        if Wait(190)==false then return end
        if dir == 1 then
            self:SetStraightParas(1,45,false,0.05,0)
        else
            self:SetStraightParas(1,135,false,0.05,180)
        end
    end)
end
CustomizedTable["YKStage1Enemy3Bullet"] = {}
CustomizedTable["YKStage1Enemy3Bullet"].Init = function(self,id,v,angle,rebound)
    self:SetStyleById(id)
    self:SetV(v,angle,false)
    if rebound then
        self:AddRebound(7,1)
    else
    end
end
CustomizedEnemyTable["YKStage1Enemy3"] = {}
CustomizedEnemyTable["YKStage1Enemy3"].Init = function(self)
    self:Init(100022)
    self:SetMaxHp(1800)
    --测试反弹子弹
    self:MoveTo(0,144,60,IntModeLinear)
    self:AddTask(function()
        if Wait(60)==false then return end
        last = lib.CreateChargeEffect(self.x,self.y)
        if Wait(60)==false then return end
        do local a1,_d_a1=(0),(27) local a2,_d_a2=(90),(18) for _=1,20 do
            PlaySound("se_tan00",0.05,false)
            do local a,_d_a=(90),(20) for _=1,18 do
                last = CreateCustomizedBullet("YKStage1Enemy3Bullet",self.x+27*cos(a),self.y+27*sin(a),102060,2.75,a1,false)
                last = CreateCustomizedBullet("YKStage1Enemy3Bullet",self.x+27*cos(a),self.y+27*sin(a),102060,2.75,a1+180,false)
            a=a+_d_a end end
            if Wait(8)==false then return end
        a1=a1+_d_a1 a2=a2+_d_a2 end end
        do
            if Wait(20)==false then return end
            last = lib.CreateChargeEffect(self.x,self.y)
            if Wait(60)==false then return end
            do local a1,_d_a1=(0),(27) local a2,_d_a2=(90),(18) for _=1,20 do
                PlaySound("se_tan00",0.05,false)
                do local a,_d_a=(90),(36) for _=1,10 do
                    last = CreateCustomizedBullet("YKStage1Enemy3Bullet",self.x+65*cos(a),self.y+65*sin(a),102010,4,a1,false)
                    last = CreateCustomizedBullet("YKStage1Enemy3Bullet",self.x+45*cos(a),self.y+45*sin(a),102010,4,a1+180,false)
                a=a+_d_a end end
                if Wait(8)==false then return end
            a1=a1+_d_a1 a2=a2+_d_a2 end end
        end
        do
            if Wait(20)==false then return end
            last = lib.CreateChargeEffect(self.x,self.y)
            if Wait(60)==false then return end
            do local a1,_d_a1=(0),(27) local a2,_d_a2=(90),(18) for _=1,20 do
                PlaySound("se_tan00",0.05,false)
                do local a,_d_a=(90),(36) for _=1,10 do
                    last = CreateCustomizedBullet("YKStage1Enemy3Bullet",self.x+65*cos(a),self.y+65*sin(a),102010,4,a1,false)
                    last = CreateCustomizedBullet("YKStage1Enemy3Bullet",self.x+45*cos(a),self.y+45*sin(a),102010,4,a1+180,false)
                a=a+_d_a end end
                do local a,_d_a=(90),(20) for _=1,18 do
                    last = CreateCustomizedBullet("YKStage1Enemy3Bullet",self.x+27*cos(a),self.y+27*sin(a),102060,2.75,a1,false)
                    last = CreateCustomizedBullet("YKStage1Enemy3Bullet",self.x+27*cos(a),self.y+27*sin(a),102060,2.75,a1+180,false)
                a=a+_d_a end end
                if Wait(8)==false then return end
            a1=a1+_d_a1 a2=a2+_d_a2 end end
        end
        do
            if Wait(20)==false then return end
            last = lib.CreateChargeEffect(self.x,self.y)
            if Wait(60)==false then return end
            do local a1,_d_a1=(0),(27) local a2,_d_a2=(90),(18) for _=1,20 do
                PlaySound("se_tan00",0.05,false)
                do local a,_d_a=(90),(36) for _=1,10 do
                    last = CreateCustomizedBullet("YKStage1Enemy3Bullet",self.x+65*cos(a),self.y+65*sin(a),102010,4,a1,true)
                    last = CreateCustomizedBullet("YKStage1Enemy3Bullet",self.x+45*cos(a),self.y+45*sin(a),102010,4,a1+180,true)
                a=a+_d_a end end
                do local a,_d_a=(90),(20) for _=1,18 do
                    last = CreateCustomizedBullet("YKStage1Enemy3Bullet",self.x+27*cos(a),self.y+27*sin(a),102060,2.75,a1,false)
                    last = CreateCustomizedBullet("YKStage1Enemy3Bullet",self.x+27*cos(a),self.y+27*sin(a),102060,2.75,a1+180,false)
                a=a+_d_a end end
                if Wait(8)==false then return end
            a1=a1+_d_a1 a2=a2+_d_a2 end end
        end
    end)
end
CustomizedSTGObjectTable["YKStage1LaserObject0"] = {}
CustomizedSTGObjectTable["YKStage1LaserObject0"].Init = function(self)
    self:SetSprite("STGEffectAtlas","MapleLeaf1",BlendMode_Normal,LayerEffectNormal,false)
    self:AddTask(function()
        do for _=1,6 do
            local leftPosX,rightPosX = -185,185
            do local i,_d_i=(1),(1) for _=1,15 do
                local k = i / 15
                k = k * k
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX + 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX - 85.5 * k,224,270)
                if Wait(2)==false then return end
            i=i+_d_i end end
            leftPosX,rightPosX = -99.5,99.5
            do local i,_d_i=(1),(1) for _=1,15 do
                local k = i / 15
                k = -k * k + 2 * k
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX + 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX - 85.5 * k,224,270)
                if Wait(2)==false then return end
            i=i+_d_i end end
            leftPosX,rightPosX = -14,14
            do local i,_d_i=(1),(1) for _=1,15 do
                local k = i / 15
                k = k * k
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX - 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX + 85.5 * k,224,270)
                if Wait(2)==false then return end
            i=i+_d_i end end
            leftPosX,rightPosX = -99.5,99.5
            do local i,_d_i=(1),(1) for _=1,15 do
                local k = i / 15
                k = -k * k + 2 * k
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX - 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX + 85.5 * k,224,270)
                if Wait(2)==false then return end
            i=i+_d_i end end
        end end
        DelUnit(self)
    end)
end
CustomizedSTGObjectTable["YKStage1LaserObject1"] = {}
CustomizedSTGObjectTable["YKStage1LaserObject1"].Init = function(self)
    self:SetSprite("STGEffectAtlas","MapleLeaf1",BlendMode_Normal,LayerEffectNormal,false)
    self:AddTask(function()
        do for _=1,12 do
            leftPosX,rightPosX = -14,185
            do local i,_d_i=(1),(1) for _=1,30 do
                local k = i / 30
                k = k * k
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX - 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX - 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX - 85.5 * k,224,-45)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX - 85.5 * k,224,-135)
                if Wait(1)==false then return end
            i=i+_d_i end end
            leftPosX,rightPosX = -99.5,99.5
            do local i,_d_i=(1),(1) for _=1,30 do
                local k = i / 30
                k = -k * k + 2 * k
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX - 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX - 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX - 85.5 * k,224,-45)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX - 85.5 * k,224,-135)
                if Wait(1)==false then return end
            i=i+_d_i end end
            local leftPosX,rightPosX = -185,14
            do local i,_d_i=(1),(1) for _=1,30 do
                local k = i / 30
                k = k * k
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX + 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX + 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX + 85.5 * k,224,-45)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX + 85.5 * k,224,-135)
                if Wait(1)==false then return end
            i=i+_d_i end end
            leftPosX,rightPosX = -99.5,99.5
            do local i,_d_i=(1),(1) for _=1,30 do
                local k = i / 30
                k = -k * k + 2 * k
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX + 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX + 85.5 * k,224,270)
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX + 85.5 * k,224,-45)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX + 85.5 * k,224,-135)
                if Wait(1)==false then return end
            i=i+_d_i end end
        end end
        DelUnit(self)
    end)
end
CustomizedEnemyTable["YKStage1Enemy5"] = {}
CustomizedEnemyTable["YKStage1Enemy5"].Init = function(self,dir)
    self:Init(100000)
    self:SetMaxHp(10)
    --自机狙
    self:MoveTo(self.x,RandomFloat(100,130),120,IntModeLinear)
    self:AddTask(function()
        if Wait(10)==false then return end
        do for _=1,10 do
            PlaySound("se_tan00",0.025,false)
            last = CreateSimpleBulletById(102060,self.x,self.y)
            last:SetStraightParas(10,0,true,0,0)
            if Wait(2)==false then return end
        end end
    end)
    self:AddTask(function()
        if Wait(150)==false then return end
        if dir == 1 then
            self:SetStraightParas(0.5,45,false,0.05,0)
        else
            self:SetStraightParas(0.5,135,false,0.05,180)
        end
        if Wait(150)==false then return end
        DelUnit(self)
    end)
end
CustomizedEnemyTable["YKStage1Enemy6"] = {}
CustomizedEnemyTable["YKStage1Enemy6"].Init = function(self)
    self:Init(100010)
    self:SetMaxHp(2000)
    local dir = 1
    self:MoveTo(0,30,90,IntModeEaseOutQuad)
    self:SetInvincible(5)
    self:AddTask(function()
        if Wait(100)==false then return end
        do local laserAngle,_d_laserAngle=(0),(360/14) for _=1,14 do
            last = CreateCustomizedLaser("ColliderTriggerLaser",0,30,laserAngle)
            last:AttachTo(self,true)
        laserAngle=laserAngle+_d_laserAngle end end
        do for _=1,Infinite do
            dir = -dir
            do local curveAngle,_d_curveAngle=(RandomFloat(0,360)),(-30 * dir) for _=1,6 do
                last = CreateCustomizedBullet("TestColliderBullet",0,2000,dir)
                if Wait(30)==false then return end
            curveAngle=curveAngle+_d_curveAngle end end
            if Wait(150)==false then return end
        end end
    end)
end
BossTable["YKS1_MidBoss"] = {}
BossTable["YKS1_MidBoss"].Init = function(self)
    self:SetAni(2001)
    self:SetPos(0,280)
    self:SetCollisionSize(32,32)
end
CustomizedTable["YKS1_M_NSCLaser0"] = {}
CustomizedTable["YKS1_M_NSCLaser0"].Init = function(self,angle,existDuration,interval)
    self:SetStyleById(201061)
    self:SetSize(500,16)
    self.rot = angle
    self.orderInLayer = 2
    self:TurnHalfOn(2,0)
    self:AddTask(function()
        if Wait(30)==false then return end
        self:TurnOn(10)
        if Wait(existDuration+10)==false then return end
        self:TurnOff(20)
        if Wait(20)==false then return end
        KillUnit(self,true)
    end)
    if interval ~= 0 then
        self:AddTask(function()
            if Wait(40)==false then return end
            local normalizedX,normalizedY = cos(angle),sin(angle)
            do local i,_d_i=(1),(1) for _=1,Infinite do
                local posX = self.x + interval * i * normalizedX
                local posY = self.y + interval * i * normalizedY
                if posX > 192 or posX < -192 or posY >224 or posY <-224 then
                    break
                else
                    last = CreateCustomizedBullet("YKS1_M_NSCBullet0",posX,posY,angle,existDuration + 10 - 2 * i)
                end
                if Wait(2)==false then return end
            i=i+_d_i end end
        end)
    else
    end
end
CustomizedTable["YKS1_M_NSCLaser1"] = {}
CustomizedTable["YKS1_M_NSCLaser1"].Init = function(self,canRotate)
    self:SetStyleById(202061)
    self:SetSize(0,128)
    self.rot = Angle(self,player)
    self:TurnHalfOn(2,0)
    self:ChangeLengthTo(500,10,30)
    self:AddTask(function()
        if Wait(60)==false then return end
        self:TurnOn(10)
        if Wait(60)==false then return end
        self:TurnOff(10)
        if Wait(10)==false then return end
        KillUnit(self,true)
    end)
    if canRotate then
        self:AddTask(function()
            local prePosX = player.x
            if Wait(70)==false then return end
            local nowPosX = player.x
            local omega = prePosX > nowPosX and -0.2 or 0.2
            do for _=1,60 do
                self.rot = self.rot + omega
                if Wait(1)==false then return end
            end end
        end)
    else
    end
end
CustomizedTable["YKS1_M_NSCBullet0"] = {}
CustomizedTable["YKS1_M_NSCBullet0"].Init = function(self,angle,waitTime)
    self:SetStyleById(104061)
    self:SetV(0,angle,false)
    self.orderInLayer = 1
    self:AddTask(function()
        if Wait(waitTime)==false then return end
        self:SetStraightParas(0,RandomFloat(0,360),false,0.03,UseVelocityAngle)
        self.maxV = 2.5
    end)
end
SpellCard["YKS1_M_NSC"] = {}
SpellCard["YKS1_M_NSC"].Init = function(boss)
    SetSpellCardProperties("",1,60,ConditionEliminateAll,false)
    boss:SetMaxHp(500)
    boss:SetInvincible(5)
    boss:ShowBloodBar(true)
    boss:AddTask(function()
        boss:MoveTo(0,140,60,IntModeLinear)
        if Wait(70)==false then return end
        do for _=1,Infinite do
            local startAngle = Angle(boss,player)
            do local i,_d_i=(1),(1) for _=1,20 do
                last = CreateCustomizedLaser("YKS1_M_NSCLaser0",boss.x,boss.y,startAngle+4.125+i*16.5,93-i*3,32)
                last = CreateCustomizedLaser("YKS1_M_NSCLaser0",boss.x,boss.y,startAngle-4.125-i*16.5,93-i*3,32)
                if Wait(3)==false then return end
            i=i+_d_i end end
            if Wait(180)==false then return end
        end end
    end)
    boss:AddTask(function()
        if Wait(70)==false then return end
        do for _=1,Infinite do
            if Wait(60)==false then return end
            last = CreateCustomizedLaser("YKS1_M_NSCLaser1",boss.x,boss.y,true)
            if Wait(180)==false then return end
        end end
    end)
end
SpellCard["YKS1_M_NSC"].OnFinish = function(boss)
end
SpellCard["YKS1_M_SC"] = {}
SpellCard["YKS1_M_SC"].Init = function(boss)
    SetSpellCardProperties("FirstSpellCard",1,60,ConditionEliminateAll,true)
    last = CreateCustomizedSTGObject("NazrinCG",0,0)
    boss:SetMaxHp(1500)
    boss:SetInvincible(5)
    boss:ShowBloodBar(true)
    --圈形子弹
    boss:AddTask(function()
        local k = 1
        do for _=1,Infinite do
            local posX,posY = boss.x,boss.y
            last = CreateCustomizedBullet("NazrinSC1Bullet0",posX,posY,0,75)
            local master = last
            last:SetPolarParas(0,23.5*0*k,0,1*k)
            last = CreateCustomizedSTGObject("NazrinShield",0,0,posX,posY,80)
            last:AttachTo(master,true)
            do local i,_d_i=(1),(1) for _=1,15 do
                if Wait(1)==false then return end
                last = CreateCustomizedBullet("NazrinSC1Bullet0",posX,posY,i,75)
                last:SetPolarParas(0,23.5*i*k,0,1*k)
            i=i+_d_i end end
            k = -k
            if Wait(90)==false then return end
        end end
    end)
    --servants
    boss:AddTask(function()
        if Wait(300)==false then return end
        do for _=1,Infinite do
            local duration = 120
            last = CreateCustomizedEnemy("NazrinSC1Enemy",boss.x,boss.y,-170,205,duration)
            last = CreateCustomizedEnemy("NazrinSC1Enemy",boss.x,boss.y,170,205,duration)
            if Wait(180)==false then return end
            last = CreateCustomizedEnemy("NazrinSC1Enemy",boss.x,boss.y,-170,-205,duration)
            last = CreateCustomizedEnemy("NazrinSC1Enemy",boss.x,boss.y,170,-205,duration)
            if Wait(180)==false then return end
        end end
    end)
    --wander
    boss:AddTask(function()
        boss:SetWanderRange(-150,150,80,125)
        boss:SetWanderAmplitude(0,100,0,15)
        boss:SetWanderMode(IntModeLinear,MoveXTowardsPlayer)
        if Wait(300)==false then return end
        do for _=1,Infinite do
            boss:Wander(120)
            if Wait(420)==false then return end
        end end
    end)
end
SpellCard["YKS1_M_SC"].OnFinish = function(boss)
    DropItems(boss.x,boss.y,32,32,1,2,2,1,4,1,6,1)
end
CustomizedTable["YKS1P7CurveLaser0"] = {}
CustomizedTable["YKS1P7CurveLaser0"].Init = function(self,angle,x)
    self:SetStyleById(401051)
    self:SetLength(60)
    self:SetWidth(16)
    self:SetV(4,angle,false)
    self.checkBorder = false
    self:ChangeProperty(Prop_VAngel,ChangeModeIncBy,0,90 * x,0,0,0,15,IntModeLinear,1,0)
    self:ChangeProperty(Prop_VAngel,ChangeModeIncBy,0,-225 * x,0,0,15,45,IntModeLinear,1,0)
    self:ChangeProperty(Prop_VAngel,ChangeModeIncBy,0,135 * x,0,0,110,100,IntModeLinear,1,0)
    self:ChangeProperty(Prop_Velocity,ChangeModeChangeTo,0,1,0,0,0,50,IntModeLinear,1,0)
    self:ChangeProperty(Prop_Velocity,ChangeModeChangeTo,0,4,0,0,50,100,IntModeLinear,1,0)
    self:AddTask(function()
        if Wait(410)==false then return end
        DelUnit(self)
    end)
end
CustomizedTable["YKS1P7CurveLaser1"] = {}
CustomizedTable["YKS1P7CurveLaser1"].Init = function(self,angle,x)
    self:SetStyleById(401031)
    self:SetLength(60)
    self:SetWidth(16)
    self:SetV(1,angle,false)
    self.checkBorder = false
    self:ChangeProperty(Prop_VAngel,ChangeModeIncBy,0,135 * x,0,0,0,15,IntModeLinear,1,0)
    self:ChangeProperty(Prop_VAngel,ChangeModeIncBy,0,-135 * x,0,0,15,30,IntModeLinear,1,0)
    self:ChangeProperty(Prop_VAngel,ChangeModeIncBy,0,135 * x,0,0,45,150,IntModeLinear,1,0)
    self:ChangeProperty(Prop_Velocity,ChangeModeIncBy,0,4,0,0,0,20,IntModeLinear,1,0)
    self:ChangeProperty(Prop_Velocity,ChangeModeDecBy,0,4.9,0,0,20,30,IntModeLinear,1,0)
    self:ChangeProperty(Prop_Velocity,ChangeModeIncBy,0,2.9,0,0,50,50,IntModeLinear,1,0)
    self:ChangeProperty(Prop_Velocity,ChangeModeIncBy,0,2,0,0,100,100,IntModeLinear,1,0)
    self:AddTask(function()
        if Wait(395)==false then return end
        DelUnit(self)
    end)
end
CustomizedTable["YKS1P7Bullet0"] = {}
CustomizedTable["YKS1P7Bullet0"].Init = function(self,v,angle,acce)
    self:SetStyleById(122050)
    self:SetStraightParas(v,angle,false,acce/50,UseVelocityAngle)
    self.checkBorder = false
    self:AddTask(function()
        if Wait(50)==false then return end
        do local laserAngle,_d_laserAngle=(angle - 90),(90) for _=1,3 do
            last = CreateCustomizedCurveLaser("YKS1P7CurveLaser0",self.x,self.y,laserAngle+160,1)
            last = CreateCustomizedCurveLaser("YKS1P7CurveLaser0",self.x,self.y,laserAngle-160,-1)
        laserAngle=laserAngle+_d_laserAngle end end
        last = CreateCustomizedBullet("YKS1P7Bullet2",self.x,self.y)
        DelUnit(self)
    end)
end
CustomizedTable["YKS1P7Bullet1"] = {}
CustomizedTable["YKS1P7Bullet1"].Init = function(self,v,angle,acce)
    self:SetStyleById(122050)
    self:SetStraightParas(v,angle,false,acce/50,UseVelocityAngle)
    self.checkBorder = false
    self:AddTask(function()
        if Wait(50)==false then return end
        do local laserAngle,_d_laserAngle=(angle),(120) for _=1,3 do
            last = CreateCustomizedCurveLaser("YKS1P7CurveLaser1",self.x,self.y,laserAngle+100,1)
            last = CreateCustomizedCurveLaser("YKS1P7CurveLaser1",self.x,self.y,laserAngle-100,-1)
        laserAngle=laserAngle+_d_laserAngle end end
        last = CreateCustomizedBullet("YKS1P7Bullet2",self.x,self.y)
        DelUnit(self)
    end)
end
CustomizedTable["YKS1P7Bullet2"] = {}
CustomizedTable["YKS1P7Bullet2"].Init = function(self)
    self:SetStyleById(113011)
    self:SetStraightParas(0,Angle(self,player),false,0,0)
    self:ChangeProperty(Prop_Velocity,ChangeModeChangeTo,0,-0.5,0,0,11,1,IntModeLinear,1,0)
    self:ChangeProperty(Prop_Velocity,ChangeModeIncBy,0,-4.5,0,0,11,20,IntModeLinear,1,0)
    self:ChangeProperty(Prop_Velocity,ChangeModeIncBy,0,6,0,0,31,30,IntModeLinear,1,0)
    self.checkBorder = false
    self:AddTask(function()
        if Wait(50)==false then return end
        self.checkBorder = true
    end)
end
CustomizedEnemyTable["YKS1P7Enemy0"] = {}
CustomizedEnemyTable["YKS1P7Enemy0"].Init = function(self)
    self:Init(100020)
    self:SetMaxHp(1400)
    self:MoveTo(0,144,50,IntModeEaseOutQuad)
    self:AddTask(function()
        if Wait(120)==false then return end
        do for _=1,2 do
            do local a,_d_a=(RandomFloat(0,360)),(360 / 9) for _=1,9 do
                last = CreateCustomizedBullet("YKS1P7Bullet0",self.x,self.y,7,a,-6.5)
                last = CreateCustomizedBullet("YKS1P7Bullet0",self.x,self.y,5.5,a+20,-5)
                last = CreateCustomizedBullet("YKS1P7Bullet0",self.x,self.y,4,a,-3.5)
            a=a+_d_a end end
            if Wait(100)==false then return end
            self:MoveTo(RandomFloat(-65,65),RandomFloat(105,145),80,IntModeEaseOutQuad)
            if Wait(80)==false then return end
            if Wait(100)==false then return end
            do local a,_d_a=(RandomFloat(0,360)),(360 / 9) for _=1,9 do
                last = CreateCustomizedBullet("YKS1P7Bullet1",self.x,self.y,7,a,-6.5)
                last = CreateCustomizedBullet("YKS1P7Bullet1",self.x,self.y,5.5,a+20,-5)
                last = CreateCustomizedBullet("YKS1P7Bullet1",self.x,self.y,4,a,-3.5)
            a=a+_d_a end end
            if Wait(100)==false then return end
            self:MoveTo(RandomFloat(-65,65),RandomFloat(105,145),80,IntModeEaseOutQuad)
            if Wait(80)==false then return end
            if Wait(100)==false then return end
        end end
        self:MoveTo(self.x,300,80,IntModeEaseOutQuad)
        if Wait(80)==false then return end
        DelUnit(self)
    end)
end
CustomizedSTGObjectTable["Stage1Logo"] = {}
CustomizedSTGObjectTable["Stage1Logo"].Init = function(self)
    self:SetSprite("Stages/Stage1","logo",BlendMode_Normal,LayerEffectNormal,false)
    self.alpha = 0
    self:AddTask(function()
        self:ChangeAlphaTo(1,60)
        if Wait(300)==false then return end
        self:ChangeAlphaTo(0,60)
        if Wait(60)==false then return end
        DelUnit(self)
    end)
end
Stage["Stage1"] = function()
    PlaySound("oborozuki",0.5,true)
    do
        if Wait(60)==false then return end
        do for _=1,8 do
            last = CreateCustomizedEnemy("YKStage1Enemy0",-192,200,-20)
            if Wait(71)==false then return end
        end end
        do for _=1,8 do
            last = CreateCustomizedEnemy("YKStage1Enemy0",192,200,-160)
            if Wait(71)==false then return end
        end end
    end
    do
        last = CreateCustomizedSTGObject("Stage1Logo",0,150)
        if Wait(400)==false then return end
    end
    do
        last = CreateCustomizedEnemy("YKStage1Enemy1",-150,240,200,false)
        if Wait(300)==false then return end
        last = CreateCustomizedEnemy("YKStage1Enemy1",150,240,200,false)
        if Wait(300)==false then return end
        last = CreateCustomizedEnemy("YKStage1Enemy1",0,240,600,true)
        if Wait(600)==false then return end
    end
    do
        if Wait(60)==false then return end
        do for _=1,20 do
            last = CreateCustomizedEnemy("YKStage1Enemy2",-150,240,1)
            last = CreateCustomizedEnemy("YKStage1Enemy2",-75,240,1)
            last = CreateCustomizedEnemy("YKStage1Enemy2",75,240,-1)
            last = CreateCustomizedEnemy("YKStage1Enemy2",150,240,-1)
            if Wait(30)==false then return end
        end end
        if Wait(300)==false then return end
    end
    do
        if Wait(200)==false then return end
        last = CreateCustomizedEnemy("YKStage1Enemy3",0,240)
        if Wait(1000)==false then return end
    end
    do
        if Wait(200)==false then return end
        if player.characterIndex == 0 then
            if StartDialog(function()
                CreateDialogCG("Reimu","Reimu",100,150)
                if Wait(30)==false then return end
                HighlightDialogCG("Reimu",true)
                CreateDialogBox(0,"TestDialogBox0.....",100,150,120,1)
                if Wait(120)==false then return end
                CreateDialogBox(0,"TestDialogBox1.....",100,150,120,1)
                if Wait(120)==false then return end
                FadeOutDialogCG("Reimu")
                if Wait(100)==false then return end
            end) == false then return end
        else
            if StartDialog(function()
                CreateDialogCG("Marisa","Marisa",100,150)
                if Wait(30)==false then return end
                HighlightDialogCG("Marisa",true)
                CreateDialogBox(0,"TestDialogBox0.....",100,150,120,1)
                if Wait(120)==false then return end
                CreateDialogBox(0,"TestDialogBox1.....",100,150,120,1)
                if Wait(120)==false then return end
                FadeOutDialogCG("Marisa")
                if Wait(100)==false then return end
            end) == false then return end
        end
        last = CreateBoss("YKS1_MidBoss")
        ShowBossInfo("Nazrin",0)
        local boss = last
        boss:MoveTo(0,170,90,IntModeEaseInQuad)
        if Wait(100)==false then return end
        boss:SetPhaseData(3,1,true)
        StartSpellCard(SpellCard["YKS1_M_NSC"],boss)
        if WaitForSpellCardFinish() == false then return end
        StartSpellCard(SpellCard["YKS1_M_SC"],boss)
        if WaitForSpellCardFinish() == false then return end
        if Wait(100)==false then return end
        boss:MoveTo(0,300,60,IntModeEaseOutQuad)
        if Wait(60)==false then return end
        DelUnit(boss)
    end
    do
        if Wait(200)==false then return end
        last = CreateCustomizedSTGObject("YKStage1LaserObject0",0,300)
        if Wait(220)==false then return end
        do local startX,_d_startX=(-160),(40) for _=1,4 do
            last = CreateCustomizedEnemy("YKStage1Enemy5",startX,240,1)
            if Wait(10)==false then return end
        startX=startX+_d_startX end end
        if Wait(210)==false then return end
        do local startX,_d_startX=(160),(-40) for _=1,4 do
            last = CreateCustomizedEnemy("YKStage1Enemy5",startX,240,-1)
            if Wait(10)==false then return end
        startX=startX+_d_startX end end
        if Wait(500)==false then return end
        last = CreateCustomizedSTGObject("YKStage1LaserObject1",0,300)
        if Wait(200)==false then return end
        do local startX,_d_startX=(-160),(40) for _=1,4 do
            last = CreateCustomizedEnemy("YKStage1Enemy5",startX,240,1)
            if Wait(10)==false then return end
        startX=startX+_d_startX end end
        if Wait(240)==false then return end
        do local startX,_d_startX=(160),(-40) for _=1,4 do
            last = CreateCustomizedEnemy("YKStage1Enemy5",startX,240,-1)
            if Wait(10)==false then return end
        startX=startX+_d_startX end end
        if Wait(240)==false then return end
        do local startX,_d_startX=(-160),(40) for _=1,4 do
            last = CreateCustomizedEnemy("YKStage1Enemy5",startX,240,1)
            if Wait(10)==false then return end
        startX=startX+_d_startX end end
        if Wait(240)==false then return end
        do local startX,_d_startX=(160),(-40) for _=1,4 do
            last = CreateCustomizedEnemy("YKStage1Enemy5",startX,240,-1)
            if Wait(10)==false then return end
        startX=startX+_d_startX end end
        if Wait(240)==false then return end
    end
    do
        if Wait(200)==false then return end
        last = CreateCustomizedEnemy("YKStage1Enemy6",0,240)
        if Wait(1800)==false then return end
    end
    do
        if Wait(200)==false then return end
        if StartDialog(function()
            CreateDialogCG("Nazrin","Nazrin",450,150)
            if Wait(30)==false then return end
            HighlightDialogCG("Nazrin",true)
            CreateDialogBox(0,"掉帧攻击！！.jpg",450,150,120,-1)
            if Wait(120)==false then return end
            HighlightDialogCG("Nazrin",false)
            FadeOutDialogCG("Nazrin")
            if Wait(100)==false then return end
        end) == false then return end
        last = CreateCustomizedEnemy("YKS1P7Enemy0",0,240)
        if Wait(1800)==false then return end
    end
    if Wait(10000)==false then return end
    FinishStage()
end
Stage["Stage2"] = function()
    PlaySound("bgm",0.5,true)
    do
        if Wait(100)==false then return end
        last = CreateCustomizedEnemy("TestOnKillEnemy",0,185)
        local enemy = last
        enemy:AddTask(function()
            if Wait(150)==false then return end
            enemy:MoveTowards(1,315,false,200)
            if Wait(350)==false then return end
            enemy:MoveTowards(0.5,180,false,700)
        end)
        enemy:AddTask(function()
            if Wait(100)==false then return end
            do for _=1,10 do
                do for _=1,6 do
                    do local angle,_d_angle=(-35),(35) for _=1,3 do
                        last = CreateSimpleBulletById(104060,enemy.x,enemy.y)
                        last:SetStraightParas(2,angle,true,0.25,angle)
                    angle=angle+_d_angle end end
                    if Wait(5)==false then return end
                end end
                if Wait(100)==false then return end
            end end
        end)
        enemy:AddTask(function()
            local k = 1
            do for _=1,Infinite do
                local posX,posY = enemy.x,enemy.y
                do local i,_d_i=(0),(1) for _=1,16 do
                    last = CreateCustomizedBullet("NazrinSC1Bullet0",posX,posY,i,75)
                    last:SetPolarParas(0,23.5*i*k,0,1*k)
                    if Wait(1)==false then return end
                i=i+_d_i end end
                k = -k
                if Wait(90)==false then return end
            end end
        end)
        if Wait(500)==false then return end
    end
    do
        if Wait(300)==false then return end
        if StartDialog(function()
            CreateDialogCG("Marisa","Marisa",100,150)
            if Wait(30)==false then return end
            HighlightDialogCG("Marisa",true)
            CreateDialogBox(0,"TestDialogBox0.....",100,150,120,1)
            if Wait(120)==false then return end
            HighlightDialogCG("Marisa",false)
        end) == false then return end
        last = CreateBoss("Marisa")
        local boss = last
        boss:MoveTo(0,170,90,IntModeEaseInQuad)
        if Wait(100)==false then return end
        if StartDialog(function()
            CreateDialogCG("Nazrin","Nazrin",450,150)
            if Wait(30)==false then return end
            HighlightDialogCG("Nazrin",true)
            CreateDialogBox(0,"TestDialogBox1.....",450,150,120,-1)
            if Wait(120)==false then return end
            HighlightDialogCG("Nazrin",false)
            HighlightDialogCG("Marisa",true)
            CreateDialogBox(0,"TestDialogBox2....",100,150,120,1)
            if Wait(120)==false then return end
            FadeOutDialogCG("Nazrin")
            FadeOutDialogCG("Marisa")
            if Wait(100)==false then return end
        end) == false then return end
        boss:SetPhaseData(1,1,1,1,true)
        StartSpellCard(SpellCard["NazrinSC1_0"],boss)
        if WaitForSpellCardFinish() == false then return end
        StartSpellCard(SpellCard["NazrinSC1_0"],boss)
        if WaitForSpellCardFinish() == false then return end
        StartSpellCard(SpellCard["NazrinSC1_0"],boss)
        if WaitForSpellCardFinish() == false then return end
        StartSpellCard(SpellCard["NazrinSC1_0"],boss)
        if WaitForSpellCardFinish() == false then return end
    end
    last = CreateCustomizedSTGObject("竖版开海",500,0)
    if Wait(1000)==false then return end
    FinishStage()
end
return
{
   CustomizedBulletTable = CustomizedTable,
   CustomizedEnemyTable = CustomizedEnemyTable,
   BossTable = BossTable,
   CustomizedSTGObjectTable = CustomizedSTGObjectTable,
   Stage = Stage,
}