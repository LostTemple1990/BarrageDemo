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
        self:MoveTo(toPosX,toPosY,30,IntModeEaseInQuad)
        if Wait(30)==false then return end
        CreateChargeEffect(self.x,self.y)
        if Wait(30+duration)==false then return end
        KillUnit(self,true)
    end)
    self:AddTask(function()
        if Wait(60)==false then return end
        do for _=1,Infinite do
            PlaySound("se_tan00",0.05,false)
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
    self:SetResistEliminatedTypes(7)
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
LoadSound("bossbgm")
LoadSound("se_tan00")
CustomizedEnemyTable["YKStage1Enemy0"] = {}
CustomizedEnemyTable["YKStage1Enemy0"].Init = function(self,angle)
    self:Init(100000)
    self:SetMaxHp(10)
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
    self:SetDropItems(1,20,2,1,64,64)
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
                last:DisableAppearEffect()
                if attach then
                    last:AttachTo(self,true)
                else
                end
                last = CreateSimpleBulletById(118010,self.x,self.y)
                last:SetStraightParas(2,90+r+a,false,0,0)
                last:DisableAppearEffect()
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
CustomizedEnemyTable["YKStage1Enemy1_1"] = {}
CustomizedEnemyTable["YKStage1Enemy1_1"].Init = function(self,r,omega)
    self:Init(100010)
    self:SetMaxHp(140)
    self:SetDropItems(1,10,64,64)
    self:MoveTo(self.x,144,60,IntModeLinear)
    self:AddTask(function()
        if Wait(60)==false then return end
        do local an,_d_an=(RandomFloat(0,360)),(omega * 5) local startAngle,_d_startAngle=(RandomFloat(0,360)),(15) for _=1,Infinite do
            local posX = r * cos(an) + self.x
            local posY = r * sin(an) + self.y
            PlaySound("se_tan00",0.1,false)
            do local vAngle,_d_vAngle=(startAngle + RandomFloat(-7,7)),(360 / 12) for _=1,12 do
                last = CreateSimpleBulletById(107020,posX,posY)
                last:SetStraightParas(2.5,vAngle,false,0,UseVelocityAngle)
                last:AttachTo(self,true)
            vAngle=vAngle+_d_vAngle end end
            if Wait(5)==false then return end
        an=an+_d_an startAngle=startAngle+_d_startAngle end end
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
    self:SetMaxHp(4)
    self:SetDropItems(1,1,32,32)
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
    self:SetMaxHp(700)
    self:SetDropItems(1,5,2,1,64,64)
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
        do  --Phase2
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
        do  --Phase3
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
        do  --Phase4
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
        if Wait(100)==false then return end
        self:MoveTo(0,300,60,IntModeEaseOutQuad)
        if Wait(60)==false then return end
        DelUnit(self)
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
    self:SetDropItems(1,1,64,64)
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
    self:SetMaxHp(900)
    self:SetDropItems(1,10,4,1,6,1,64,64)
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
    self:AddTask(function()
        if Wait(1740)==false then return end
        self:MoveTo(0,300,60,IntModeEaseOutQuad)
        if Wait(60)==false then return end
        DelUnit(self)
    end)
end
BossTable["YKS1_MidBoss"] = {}
BossTable["YKS1_MidBoss"].Init = function(self)
    self:SetAni(2001)
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
        PlaySound("se_laserturnon",0.05,false)
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
    boss:SetMaxHp(400)
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
    boss:SetMaxHp(750)
    boss:SetInvincible(5)
    boss:ShowBloodBar(true)
    boss:ShowSpellCardHpAura(true)
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
    self:SetMaxHp(800)
    self:SetDropItems(1,2,2,1,4,1,6,1,64,64)
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
        if Wait(80)==false then return end
        KillUnit(self,true)
    end)
end
BossTable["YKS1_FinalBoss"] = {}
BossTable["YKS1_FinalBoss"].Init = function(self)
    self:SetAni(2001)
    self:SetCollisionSize(32,32)
end
SpellCard["YKS1_F_NSC1"] = {}
SpellCard["YKS1_F_NSC1"].Init = function(boss)
    SetSpellCardProperties("",1,30,ConditionEliminateAll,false)
    boss:SetMaxHp(500)
    boss:ShowBloodBar(true)
    if Wait(90)==false then return end
    boss:AddTask(function()
        do for _=1,Infinite do
            do for _=1,10 do
                local posX,posY = boss.x,boss.y
                do local startAngle,_d_startAngle=(Angle(boss,player)),(18) for _=1,20 do
                    PlaySound("se_tan00",0.05,false)
                    last = CreateSimpleBulletById(105040,boss.x,boss.y)
                    last:SetStraightParas(5,startAngle,false,0,UseVelocityAngle)
                startAngle=startAngle+_d_startAngle end end
                if Wait(15)==false then return end
            end end
            if Wait(60)==false then return end
        end end
    end)
    boss:AddTask(function()
        do for _=1,Infinite do
            local posX,posY = boss.x,RandomFloat(100,140)
            if posX <= 0 then
                posX = RandomFloat(posX+50,130)
            else
                posX = RandomFloat(-130,posX-50)
            end
            boss:MoveTo(posX,posY,90,IntModeLinear)
            if Wait(100)==false then return end
        end end
    end)
end
SpellCard["YKS1_F_NSC1"].OnFinish = function(boss)
end
CustomizedTable["YKS1_F_SC1_Bullet"] = {}
CustomizedTable["YKS1_F_SC1_Bullet"].Init = function(self,waitTime)
    self:SetStyleById(104040)
    local v = RandomFloat(2,3)
    local angle = RandomFloat(-10,10) - 90
    self:SetV(0,angle,false)
    self:AddTask(function()
        if Wait(waitTime)==false then return end
        self:SetStraightParas(v,angle,false,RandomFloat(0,0.03),UseVelocityAngle)
    end)
end
CustomizedTable["YKS1_F_SC1_Laser"] = {}
CustomizedTable["YKS1_F_SC1_Laser"].Init = function(self,master,angle,duration,omega,rotateDuration)
    self:SetStyleById(202060)
    self:SetSize(600,10)
    self:SetSourceSize(32)
    self.rot = angle
    self:TurnOn(60)
    self:AddTask(function()
        do local laserAngle,_d_laserAngle=(angle),(omega) for _=1,rotateDuration do
            self.rot = laserAngle
            if Wait(1)==false then return end
        laserAngle=laserAngle+_d_laserAngle end end
    end)
    self:AddTask(function()
        do for _=1,duration do
            self:SetPos(master.x,master.y)
            if Wait(1)==false then return end
        end end
        DelUnit(self)
    end)
end
SpellCard["YKS1_F_SC1"] = {}
SpellCard["YKS1_F_SC1"].Init = function(boss)
    SetSpellCardProperties("SpellCard_1",1,45,ConditionEliminateAll,true)
    boss:SetMaxHp(500)
    boss:ShowBloodBar(true)
    last = CreateCustomizedSTGObject("NazrinCG",0,0)
    boss:AddTask(function()
        if Wait(60)==false then return end
        boss:MoveTo(150,150,50,IntModeLinear)
        if Wait(60)==false then return end
        local k = -1
        do for _=1,Infinite do
            boss:MoveTo(k*150,150,120,IntModeLinear)
            if Wait(220)==false then return end
            k = k * -1
        end end
    end)
    boss:AddTask(function()
        if Wait(120)==false then return end
        do for _=1,Infinite do
            last = CreateCustomizedLaser("YKS1_F_SC1_Laser",boss.x,boss.y,boss,-70,180,2.66,120)
            last = CreateCustomizedLaser("YKS1_F_SC1_Laser",boss.x,boss.y,boss,250,180,-2.66,120)
            if Wait(220)==false then return end
        end end
    end)
    boss:AddTask(function()
        if Wait(120)==false then return end
        do local k,_d_k=(0),(1) for _=1,Infinite do
            if Wait(30)==false then return end
            do local posX,_d_posX=(-180),(6) local waitTime,_d_waitTime=(70),(-1) for _=1,30 do
                last = CreateCustomizedBullet("YKS1_F_SC1_Bullet",posX+RandomFloat(-5,5),224,waitTime + RandomInt(-5,5))
                last = CreateCustomizedBullet("YKS1_F_SC1_Bullet",-posX+RandomFloat(-5,5),224,waitTime + RandomInt(-5,5))
                if Wait(1)==false then return end
            posX=posX+_d_posX waitTime=waitTime+_d_waitTime end end
            if Wait(160)==false then return end
        k=k+_d_k end end
    end)
end
SpellCard["YKS1_F_SC1"].OnFinish = function(boss)
    DropItems(boss.x,boss.y,64,64,2,1,4,1,6,1)
end
CustomizedTable["YKS1_F_NSC2_Bullet"] = {}
CustomizedTable["YKS1_F_NSC2_Bullet"].Init = function(self,v,angle,waitTime,finalStyleId)
    self:SetStyleById(102100)
    self:AddTask(function()
        if Wait(150)==false then return end
        self:SetStyleById(113120)
        if Wait(30)==false then return end
        self:SetStyleById(102130)
        self:SetAcce(0.05,angle,false)
        self.maxV = v * 0.1
        if Wait(60)==false then return end
        self.maxV = v
        self:SetStyleById(finalStyleId)
    end)
end
SpellCard["YKS1_F_NSC2"] = {}
SpellCard["YKS1_F_NSC2"].Init = function(boss)
    SetSpellCardProperties("",1,45,ConditionEliminateAll,false)
    boss:SetMaxHp(1000)
    boss:MoveTo(0,136,60,IntModeLinear)
    boss:SetWanderRange(-96,96,128,144)
    boss:SetWanderAmplitude(32,64,0,32)
    boss:SetWanderMode(IntModeEaseOutQuad,MoveXTowardsPlayer)
    local finalIds = {103021,103041,103061,103081,103101,103121,103141}
    boss:AddTask(function()
        if Wait(210)==false then return end
        boss:ShowBloodBar(true)
        local k = -1
        local startAngle = RandomFloat(0,360)
        do local index,_d_index=(0),(1) local maxV,_d_maxV=(2),(0.3) for _=1,Infinite do
            CreateChargeEffect(boss.x,boss.y)
            if Wait(60)==false then return end
            do local radius,_d_radius=(16),(10) local v,_d_v=(maxV),(-0.03) local accAngle,_d_accAngle=(RandomFloat(0,30)),(30) for _=1,45 do
                local bossPosX,bossPosY = boss.x,boss.y
                local playerPosX,playerPosY = player.x,player.y
                do local angle,_d_angle=(startAngle),(30) for _=1,12 do
                    local posX = bossPosX + radius * cos(angle)
                    local posY = bossPosY + radius * sin(angle)
                    if Distance(playerPosX,playerPosY,posX,posY) > 48 and abs(posX) < 192 and abs(posY) < 224 then
                        last = CreateCustomizedBullet("YKS1_F_NSC2_Bullet",posX,posY,v,accAngle,30,finalIds[index%7+1])
                    else
                    end
                angle=angle+_d_angle end end
                if Wait(2)==false then return end
                startAngle = startAngle + 350 / radius * k
            radius=radius+_d_radius v=v+_d_v accAngle=accAngle+_d_accAngle end end
            if Wait(60)==false then return end
            boss:Wander(30)
            if Wait(60)==false then return end
            k = -k
        index=index+_d_index maxV=maxV+_d_maxV end end
    end)
end
SpellCard["YKS1_F_NSC2"].OnFinish = function(boss)
end
CustomizedTable["YKS1_F_SC2_Bullet"] = {}
CustomizedTable["YKS1_F_SC2_Bullet"].Init = function(self,v,angle)
    self:SetStyleById(115010)
    self:SetV(v,angle,false)
    self:AddColliderTrigger(2048,function(collider,collIndex)
        self:SetV(v,angle,false)
    end)
    self:AddColliderTrigger(4096,function(collider,collIndex)
        self:SetV(v / 5,angle,false)
    end)
end
CustomizedSTGObjectTable["YKS1_F_SC2_Field"] = {}
CustomizedSTGObjectTable["YKS1_F_SC2_Field"].Init = function(self,posY,defaultSize)
    self:SetSprite("STGEffectAtlas","EliminateEnemyEffect3",BlendMode_Normal,LayerEffectBottom,false)
    self:SetSize(0,0)
    last = CreateSimpleCollider(TypeCircle)
    last:SetPos(0,posY)
    last:SetSize(defaultSize,defaultSize)
    last:SetCollisionGroup(4096)
    last:AttachTo(self,true)
    last:SetRelativePos(0,0,0,false,true)
    last = CreateSimpleCollider(TypeCircle)
    last:SetPos(0,posY)
    last:SetSize(defaultSize * 1.2,defaultSize * 1.2)
    last:SetCollisionGroup(2048)
    last:AttachTo(self,true)
    last:SetRelativePos(0,0,0,false,true)
    self:AddTask(function()
        do local radius,_d_radius=(0),(defaultSize * 2 / 30) for _=1,30 do
            self:SetSize(radius,radius)
            if Wait(1)==false then return end
        radius=radius+_d_radius end end
        self:MoveTo(120,posY,100,IntModeLinear)
        if Wait(100)==false then return end
        do for _=1,Infinite do
            self:MoveTo(-120,posY,200,IntModeLinear)
            if Wait(200)==false then return end
            self:MoveTo(120,posY,200,IntModeLinear)
            if Wait(200)==false then return end
        end end
    end)
end
SpellCard["YKS1_F_SC2"] = {}
SpellCard["YKS1_F_SC2"].Init = function(boss)
    SetSpellCardProperties("TestSC_2",1,30,ConditionEliminateAll,true)
    boss:SetMaxHp(500)
    boss:ShowBloodBar(true)
    last = CreateCustomizedSTGObject("NazrinCG",0,0)
    boss:AddTask(function()
        boss:MoveTo(0,128,60,IntModeLinear)
        if Wait(60)==false then return end
        local posY = -100
        CreateChargeEffect(0,posY)
        if Wait(30)==false then return end
        last = CreateCustomizedSTGObject("YKS1_F_SC2_Field",0,posY,posY,64)
        shield = last
    end)
    boss:AddTask(function()
        if Wait(120)==false then return end
        CreateChargeEffect(boss.x,boss.y)
        if Wait(60)==false then return end
        do for _=1,Infinite do
            local initAngle = RandomFloat(0,360)
            local k = -1
            do local t,_d_t=(RandomInt(0,60)),(1) local v,_d_v=(10),(-0.02) for _=1,180 do
                local posX,posY = boss.x,boss.y
                initAngle = initAngle + t*t * k
                do local startAngle,_d_startAngle=(initAngle),(18) for _=1,20 do
                    PlaySound("se_tan00",0.05,false)
                    last = CreateCustomizedBullet("YKS1_F_SC2_Bullet",boss.x,boss.y,v,startAngle)
                startAngle=startAngle+_d_startAngle end end
                if Wait(8)==false then return end
            t=t+_d_t v=v+_d_v end end
            if Wait(60)==false then return end
        end end
    end)
    boss:AddTask(function()
        if Wait(120)==false then return end
        do for _=1,Infinite do
            last = CreateSimpleBulletById(122131,-192,-200)
            last:SetStraightParas(5,0,false,0,UseVelocityAngle)
            last = CreateSimpleBulletById(122131,192,-200)
            last:SetStraightParas(5,180,false,0,UseVelocityAngle)
            if Wait(20)==false then return end
        end end
    end)
end
SpellCard["YKS1_F_SC2"].OnFinish = function(boss)
    DropItems(boss.x,boss.y,64,64,2,1,4,1,6,1)
    if shield then DelUnit(shield) shield = nil end
end
CustomizedTable["YKS1_F_NSC3_Bullet"] = {}
CustomizedTable["YKS1_F_NSC3_Bullet"].Init = function(self,colorId,acce,accAngle,omega)
    self:SetStyleById(122001 + colorId * 10)
    self.omega = omega
    self:SetAcce(acce,accAngle,false)
    do local laserAngle,_d_laserAngle=(RandomFloat(0,360)),(90) for _=1,4 do
        last = CreateCustomizedLaser("YKS1_F_NSC3_Laser",self.x,self.y,colorId,laserAngle,self)
    laserAngle=laserAngle+_d_laserAngle end end
    self:AddTask(function()
        do for _=1,Infinite do
            if Wait(5)==false then return end
            last = CreateCustomizedBullet("YKS1_F_NSC3_Bullet_Smear",self.x,self.y,self.id,accAngle)
        end end
    end)
end
CustomizedTable["YKS1_F_NSC3_Bullet_Smear"] = {}
CustomizedTable["YKS1_F_NSC3_Bullet_Smear"].Init = function(self,id,angle)
    self:SetStyleById(id)
    self.orderInLayer = -1
    self:DisableAppearEffect()
    self.checkCollision = false
    self:SetStraightParas(0,angle,false,0,UseVelocityAngle)
    self:ChangeProperty(Prop_Alpha,ChangeModeChangeTo,0,0,0,0,0,20,IntModeLinear,1,0)
    self:ChangeProperty(Prop_ScaleX,ChangeModeChangeTo,0,0,0,0,0,20,IntModeLinear,1,0)
    self:ChangeProperty(Prop_ScaleY,ChangeModeChangeTo,0,0,0,0,0,20,IntModeLinear,1,0)
    self:AddTask(function()
        if Wait(20)==false then return end
        DelUnit(self)
    end)
end
CustomizedTable["YKS1_F_NSC3_Laser"] = {}
CustomizedTable["YKS1_F_NSC3_Laser"].Init = function(self,colorId,angle,master)
    self:SetStyleById(202001 + colorId * 10)
    self:SetSize(50,4)
    self.rot = angle
    self.checkBorder = false
    self:TurnOn(10)
    self:AttachTo(master,true)
    self:SetRelativePos(0,0,angle,true,true)
end
SpellCard["YKS1_F_NSC3"] = {}
SpellCard["YKS1_F_NSC3"].Init = function(boss)
    SetSpellCardProperties("",1,45,ConditionEliminateAll,false)
    boss:SetMaxHp(750)
    boss:ShowBloodBar(true)
    boss:MoveTo(0,128,20,IntModeLinear)
    boss:SetWanderRange(-96,96,128,144)
    boss:SetWanderAmplitude(32,64,0,32)
    boss:SetWanderMode(IntModeEaseOutQuad,MoveXTowardsPlayer)
    local colorIds = {0,1,3,5,7,9,13,15}
    if Wait(180)==false then return end
    boss:AddTask(function()
        if Wait(20)==false then return end
        CreateChargeEffect(boss.x,boss.y)
        if Wait(30)==false then return end
        boss:AddTask(function()
            do for _=1,30 do
                last = CreateCustomizedBullet("YKS1_F_NSC3_Bullet",RandomFloat(-80,192),224,colorIds[RandomInt(1,8)],RandomFloat(0.01,0.1),RandomFloat(240,260),-1)
                if Wait(20)==false then return end
            end end
        end)
        do for _=1,30 do
            last = CreateCustomizedBullet("YKS1_F_NSC3_Bullet",192,RandomFloat(-80,224),colorIds[RandomInt(1,8)],RandomFloat(0.01,0.1),RandomFloat(240,260),-1)
            if Wait(20)==false then return end
        end end
        if Wait(60)==false then return end
        CreateChargeEffect(boss.x,boss.y)
        boss:AddTask(function()
            do for _=1,30 do
                last = CreateCustomizedBullet("YKS1_F_NSC3_Bullet",RandomFloat(-192,80),224,colorIds[RandomInt(1,8)],RandomFloat(0.01,0.1),RandomFloat(-80,-30),1)
                if Wait(20)==false then return end
            end end
        end)
        do for _=1,30 do
            last = CreateCustomizedBullet("YKS1_F_NSC3_Bullet",-192,RandomFloat(-80,224),colorIds[RandomInt(1,8)],RandomFloat(0.01,0.1),RandomFloat(-80,-30),1)
            if Wait(20)==false then return end
        end end
        if Wait(60)==false then return end
        CreateChargeEffect(boss.x,boss.y)
        if Wait(20)==false then return end
        boss:AddTask(function()
            do for _=1,30 do
                last = CreateCustomizedBullet("YKS1_F_NSC3_Bullet",RandomFloat(-80,192),224,colorIds[RandomInt(1,8)],RandomFloat(0.01,0.1),RandomFloat(240,260),-1)
                if Wait(25)==false then return end
            end end
        end)
        boss:AddTask(function()
            do for _=1,30 do
                last = CreateCustomizedBullet("YKS1_F_NSC3_Bullet",RandomFloat(-192,80),224,colorIds[RandomInt(1,8)],RandomFloat(0.01,0.1),RandomFloat(-80,-30),1)
                if Wait(25)==false then return end
            end end
        end)
        boss:AddTask(function()
            do for _=1,30 do
                last = CreateCustomizedBullet("YKS1_F_NSC3_Bullet",192,RandomFloat(-80,224),colorIds[RandomInt(1,8)],RandomFloat(0.01,0.1),RandomFloat(240,260),-1)
                if Wait(25)==false then return end
            end end
        end)
        do for _=1,30 do
            last = CreateCustomizedBullet("YKS1_F_NSC3_Bullet",-192,RandomFloat(-80,224),colorIds[RandomInt(1,8)],RandomFloat(0.01,0.1),RandomFloat(-80,-30),1)
            if Wait(25)==false then return end
        end end
        if Wait(60)==false then return end
        if Wait(60)==false then return end
    end)
end
SpellCard["YKS1_F_NSC3"].OnFinish = function(boss)
end
----撞墙产生激光子弹的bullet
CustomizedTable["YKS1_F_SC3_Bullet0"] = {}
CustomizedTable["YKS1_F_SC3_Bullet0"].Init = function(self,vAngle)
    self:SetStyleById(111061)
    self:SetV(3,vAngle,false)
    self.scale = 3
    self:AddTask(function()
        local angle = 0
        do for _=1,Infinite do
            local posX,posY = self.x,self.y
            if posX > 192 then
                self.scale = 0
                angle = RandomFloat(178,182)
                self:SetV(30,angle,false)
                break
            else
            end
            if posX < -192 then
                self.scale = 0
                angle = RandomFloat(-2,2)
                self:SetV(30,angle,false)
                break
            else
            end
            if posY > 224 then
                self.scale = 0
                angle = RandomFloat(268,272)
                self:SetV(30,angle,false)
                break
            else
            end
            if Wait(1)==false then return end
        end end
        do for _=1,Infinite do
            last = CreateCustomizedBullet("YKS1_F_SC3_Bullet_Laser0",self.x,self.y,angle)
            last = CreateCustomizedBullet("YKS1_F_SC3_Bullet_Laser1",self.x,self.y,angle)
            if Wait(1)==false then return end
        end end
    end)
end
CustomizedTable["YKS1_F_SC3_Bullet1"] = {}
CustomizedTable["YKS1_F_SC3_Bullet1"].Init = function(self,vAngle)
    self:SetStyleById(111041)
    self:SetV(3,vAngle,false)
    self.scale = 3
    self:AddTask(function()
        local angle = 0
        do for _=1,Infinite do
            local posX,posY = self.x,self.y
            if posX > 192 then
                self.scale = 0
                angle = RandomFloat(178,182)
                self:SetV(30,angle,false)
                break
            else
            end
            if posX < -192 then
                self.scale = 0
                angle = RandomFloat(-2,2)
                self:SetV(30,angle,false)
                break
            else
            end
            if posY > 224 then
                self.scale = 0
                angle = RandomFloat(268,272)
                self:SetV(30,angle,false)
                break
            else
            end
            if Wait(1)==false then return end
        end end
        do for _=1,Infinite do
            last = CreateCustomizedBullet("YKS1_F_SC3_Bullet_Laser2",self.x,self.y,angle)
            last = CreateCustomizedBullet("YKS1_F_SC3_Bullet_Laser3",self.x,self.y,angle)
            if Wait(1)==false then return end
        end end
    end)
end
CustomizedTable["YKS1_F_SC3_Bullet2"] = {}
CustomizedTable["YKS1_F_SC3_Bullet2"].Init = function(self,vAngle,extraV)
    self:SetStyleById(111130)
    self:SetV(5 + extraV,vAngle,false)
    self.omega = 2
    self:ChangeProperty(Prop_Velocity,ChangeModeIncBy,-4,0,0,0,0,30,IntModeLinear,1,0)
end
CustomizedTable["YKS1_F_SC3_Bullet3"] = {}
CustomizedTable["YKS1_F_SC3_Bullet3"].Init = function(self,angleOffset)
    self:SetStyleById(117051)
    local offset = RandomFloat(-angleOffset,angleOffset)
    local angle = Angle(self,player) + offset
    self.omega = -2
    self:AddTask(function()
        do local v,_d_v=(0),(0.02) for _=1,Infinite do
            self:SetV(v*v,angle,false)
            last = CreateCustomizedBullet("YKS1_F_SC3_Bullet5",self.x + RandomFloat(-2,2),self.y + RandomFloat(-2,2),angle)
            if Wait(2)==false then return end
        v=v+_d_v end end
    end)
end
CustomizedTable["YKS1_F_SC3_Bullet4"] = {}
CustomizedTable["YKS1_F_SC3_Bullet4"].Init = function(self,vAngle)
    self:SetStyleById(117051)
    self:SetColor(50,50,100,1)
    self.checkCollision = false
    self:ChangeProperty(Prop_Alpha,ChangeModeChangeTo,0,0,0,0,0,20,IntModeLinear,1,0)
    self:ChangeProperty(Prop_ScaleX,ChangeModeChangeTo,0,0,0,0,0,20,IntModeLinear,1,0)
    self:ChangeProperty(Prop_ScaleY,ChangeModeChangeTo,0,0,0,0,0,20,IntModeLinear,1,0)
    local v = RandomFloat(1,1.5)
    self:SetV(v,vAngle,false)
    self:ChangeProperty(Prop_Velocity,ChangeModeChangeTo,0,0.1,0,0,0,50,IntModeLinear,1,0)
    self:AddTask(function()
        if Wait(20)==false then return end
        KillUnit(self,true)
    end)
end
CustomizedTable["YKS1_F_SC3_Bullet5"] = {}
CustomizedTable["YKS1_F_SC3_Bullet5"].Init = function(self,vAngle)
    self:SetStyleById(122051)
    self:SetColor(50,50,100,1)
    self.checkCollision = false
    self:ChangeProperty(Prop_Alpha,ChangeModeChangeTo,0,0,0,0,0,20,IntModeLinear,1,0)
    self:ChangeProperty(Prop_ScaleX,ChangeModeChangeTo,0,0,0,0,0,20,IntModeLinear,1,0)
    self:ChangeProperty(Prop_ScaleY,ChangeModeChangeTo,0,0,0,0,0,20,IntModeLinear,1,0)
    local v = RandomFloat(1,1.5)
    self:SetV(v,vAngle,false)
    self:ChangeProperty(Prop_Velocity,ChangeModeChangeTo,0,0.1,0,0,0,50,IntModeLinear,1,0)
    self:DisableAppearEffect()
    self:AddTask(function()
        if Wait(20)==false then return end
        DelUnit(self)
    end)
end
----激光子弹
CustomizedTable["YKS1_F_SC3_Bullet_Laser0"] = {}
CustomizedTable["YKS1_F_SC3_Bullet_Laser0"].Init = function(self,vAngle)
    self:SetStyleById(123051)
    self.scaleX = 12
    self.scaleY = 0.4
    self:SetColor(100,100,255,0.1)
    self.checkCollision = false
    self:DisableAppearEffect()
    self:SetV(0,vAngle,false)
    self:AddTask(function()
        if Wait(50)==false then return end
        self.checkCollision = true
        self:SetColor(100,100,255,1)
        self:ChangeProperty(Prop_ScaleY,ChangeModeChangeTo,0,1.9,0,0,0,25,IntModeSin,1,0)
        self:ChangeProperty(Prop_ScaleY,ChangeModeChangeTo,0,0.2,0,0,25,25,IntModeCos,1,0)
        if Wait(50)==false then return end
        DelUnit(self)
    end)
end
CustomizedTable["YKS1_F_SC3_Bullet_Laser1"] = {}
CustomizedTable["YKS1_F_SC3_Bullet_Laser1"].Init = function(self,vAngle)
    self:SetStyleById(122051)
    self.scaleX = 6
    self.scaleY = 0.4
    self:SetColor(100,100,255,0)
    self.checkCollision = false
    self:DisableAppearEffect()
    self.navi = true
    self:SetV(0,vAngle,false)
    self:AddTask(function()
        if Wait(50)==false then return end
        self:SetColor(50,50,255,0.2)
        self:ChangeProperty(Prop_ScaleY,ChangeModeChangeTo,0,1.2,0,0,0,25,IntModeSin,1,0)
        self:ChangeProperty(Prop_ScaleY,ChangeModeChangeTo,0,0.2,0,0,25,25,IntModeCos,1,0)
        if Wait(50)==false then return end
        DelUnit(self)
    end)
end
CustomizedTable["YKS1_F_SC3_Bullet_Laser2"] = {}
CustomizedTable["YKS1_F_SC3_Bullet_Laser2"].Init = function(self,vAngle)
    self:SetStyleById(123031)
    self.scaleX = 12
    self.scaleY = 0.4
    self:SetColor(180,100,180,0.1)
    self.checkCollision = false
    self:DisableAppearEffect()
    self:SetV(0,vAngle,false)
    self:AddTask(function()
        if Wait(50)==false then return end
        self:SetColor(180,100,180,1)
        self.checkCollision = true
        self:ChangeProperty(Prop_ScaleY,ChangeModeChangeTo,0,1.9,0,0,0,25,IntModeSin,1,0)
        self:ChangeProperty(Prop_ScaleY,ChangeModeChangeTo,0,0.2,0,0,25,25,IntModeCos,1,0)
        if Wait(50)==false then return end
        DelUnit(self)
    end)
end
CustomizedTable["YKS1_F_SC3_Bullet_Laser3"] = {}
CustomizedTable["YKS1_F_SC3_Bullet_Laser3"].Init = function(self,vAngle)
    self:SetStyleById(122031)
    self.scaleX = 6
    self.scaleY = 0.4
    self:SetColor(180,100,180,0)
    self.checkCollision = false
    self:DisableAppearEffect()
    self.navi = true
    self:SetV(0,vAngle,false)
    self:AddTask(function()
        if Wait(50)==false then return end
        self:SetColor(180,50,180,0.2)
        self:ChangeProperty(Prop_ScaleY,ChangeModeChangeTo,0,1.2,0,0,0,25,IntModeSin,1,0)
        self:ChangeProperty(Prop_ScaleY,ChangeModeChangeTo,0,0.2,0,0,25,25,IntModeCos,1,0)
        if Wait(50)==false then return end
        DelUnit(self)
    end)
end
SpellCard["YKS1_F_SC3"] = {}
SpellCard["YKS1_F_SC3"].Init = function(boss)
    SetSpellCardProperties("SpellCard3",1,40,ConditionEliminateAll,true)
    boss:SetMaxHp(400)
    boss:SetInvincible(15)
    boss:ShowBloodBar(true)
    last = CreateCustomizedSTGObject("NazrinCG",0,0)
    boss:MoveTo(0,144,50,IntModeEaseOutQuad)
    if Wait(120)==false then return end
    boss:AddTask(function()
        do for _=1,Infinite do
            boss:MoveTo(145,160,70,IntModeEaseOutQuad)
            if Wait(70)==false then return end
            do local san,_d_san=(0),(90 / 99) for _=1,100 do
                local posX = 145 - 290 * sin(san)
                local posY = -160 * sin(san*2-90)
                boss:SetPos(posX,posY)
                if Wait(1)==false then return end
            san=san+_d_san end end
            boss:MoveTo(player.x + RandomFloat(-20,20),RandomFloat(150,170),70,IntModeEaseOutQuad)
            if Wait(70)==false then return end
            if Wait(75)==false then return end
            boss:MoveTo(-145,160,70,IntModeEaseOutQuad)
            if Wait(70)==false then return end
            do local san,_d_san=(0),(90 / 99) for _=1,100 do
                local posX = -145 + 290 * sin(san)
                local posY = -160 * sin(san*2-90)
                boss:SetPos(posX,posY)
                if Wait(1)==false then return end
            san=san+_d_san end end
            boss:MoveTo(player.x + RandomFloat(-20,20),RandomFloat(150,170),70,IntModeEaseOutQuad)
            if Wait(70)==false then return end
            if Wait(75)==false then return end
            boss:MoveTo(RandomFloat(-20,20),RandomFloat(150,170),70,IntModeEaseOutQuad)
            if Wait(70)==false then return end
            boss:MoveTo(RandomFloat(-20,20),RandomFloat(150,170),70,IntModeEaseOutQuad)
            if Wait(80)==false then return end
            if Wait(60)==false then return end
        end end
    end)
    boss:AddTask(function()
        do for _=1,Infinite do
            if Wait(70)==false then return end
            --星弹，撞墙产生激光
            do local angle,_d_angle=(90),(150 / 19) for _=1,20 do
                last = CreateCustomizedBullet("YKS1_F_SC3_Bullet0",boss.x,boss.y,angle)
                if Wait(5)==false then return end
            angle=angle+_d_angle end end
            if Wait(70)==false then return end
            do for _=1,5 do
                local posX,posY = boss.x,boss.y
                local angleToPlayer = Angle(boss,player)
                do local angle,_d_angle=(angleToPlayer - 135 + RandomFloat(-10,10)),(270 / 24) for _=1,25 do
                    last = CreateCustomizedBullet("YKS1_F_SC3_Bullet2",posX + cos(angle) * 20,posY + sin(angle) * 20,angle,1)
                angle=angle+_d_angle end end
                if Wait(15)==false then return end
            end end
            if Wait(70)==false then return end
            --星弹，撞墙产生激光
            do local angle,_d_angle=(90),(-150 / 19) for _=1,20 do
                last = CreateCustomizedBullet("YKS1_F_SC3_Bullet1",boss.x,boss.y,angle)
                if Wait(5)==false then return end
            angle=angle+_d_angle end end
            if Wait(70)==false then return end
            do for _=1,5 do
                local posX,posY = boss.x,boss.y
                local angleToPlayer = Angle(boss,player)
                do local angle,_d_angle=(angleToPlayer - 135 + RandomFloat(-10,10)),(270 / 24) for _=1,25 do
                    last = CreateCustomizedBullet("YKS1_F_SC3_Bullet2",posX + cos(angle) * 20,posY + sin(angle) * 20,angle,1)
                angle=angle+_d_angle end end
                if Wait(15)==false then return end
            end end
            if Wait(70)==false then return end
            do  --cross laser
                local posX,posY = boss.x,boss.y
                do local angle,_d_angle=(0),(-180 / 19) local m,_d_m=(0),(1) for _=1,20 do
                    if m % 2 == 0 then
                        last = CreateCustomizedBullet("YKS1_F_SC3_Bullet1",posX,posY,angle)
                    else
                        last = CreateCustomizedBullet("YKS1_F_SC3_Bullet0",posX,posY,angle)
                    end
                angle=angle+_d_angle m=m+_d_m end end
            end
            do  --other bullet
                local posX,posY = boss.x,boss.y
                do local range,_d_range=(120),(-80 / 39) local m,_d_m=(0),(1) for _=1,40 do
                    last = CreateCustomizedBullet("YKS1_F_SC3_Bullet3",boss.x,boss.y,range)
                    if Wait(2)==false then return end
                range=range+_d_range m=m+_d_m end end
            end
            if Wait(60)==false then return end
        end end
    end)
end
SpellCard["YKS1_F_SC3"].OnFinish = function(boss)
    DropItems(boss.x,boss.y,64,64,2,1,4,1,6,1)
end
CustomizedTable["YKS1_F_NSC4_Bullet"] = {}
CustomizedTable["YKS1_F_NSC4_Bullet"].Init = function(self,id,angle)
    self:SetStyleById(id)
    self.orderInLayer = -1
    self:DisableAppearEffect()
    self.checkCollision = false
    self:SetStraightParas(0,angle,false,0,UseVelocityAngle)
    self:ChangeProperty(Prop_Alpha,ChangeModeChangeTo,0,0,0,0,0,20,IntModeLinear,1,0)
    self:ChangeProperty(Prop_ScaleX,ChangeModeChangeTo,0,0,0,0,0,20,IntModeLinear,1,0)
    self:ChangeProperty(Prop_ScaleY,ChangeModeChangeTo,0,0,0,0,0,20,IntModeLinear,1,0)
    self:AddTask(function()
        if Wait(20)==false then return end
        DelUnit(self)
    end)
end
CustomizedTable["YKS1_F_NSC4_Laser"] = {}
CustomizedTable["YKS1_F_NSC4_Laser"].Init = function(self,angle,reboundPara,reboundCount)
    self:SetStyleById(302011)
    self:SetLength(45)
    self:SetSourceEnable(true)
    self:SetHeadEnable(true)
    self:SetV(5,angle,false)
    self:AddTask(function()
        do for _=1,Infinite do
            if reboundCount > 0 then
                local reboundFlag = 0
                local curPosX,curPosY = self.x,self.y
                local tmpRebound = reboundPara
                local reboundAngle = self.vAngle
                if tmpRebound >= Constants.ReboundBottom and curPosY < -224 then
                    reboundFlag = 1
                    reboundAngle = -reboundAngle
                    curPosY = -448 - curPosY
                    tmpRebound = tmpRebound - Constants.ReboundBottom
                else
                end
                if tmpRebound >= Constants.ReboundTop and curPosY > 224 then
                    reboundFlag = 1
                    reboundAngle = -reboundAngle
                    curPosY = 448 - curPosY
                    tmpRebound = tmpRebound - Constants.ReboundTop
                else
                end
                if tmpRebound >= Constants.ReboundRight and curPosX > 192 then
                    reboundFlag = 2
                    reboundAngle = 180 - reboundAngle
                    curPosX = 384 - curPosX
                    tmpRebound = tmpRebound - Constants.ReboundRight
                else
                end
                if tmpRebound >= Constants.ReboundLeft and curPosX < -192 then
                    reboundFlag = 2
                    reboundAngle = -180 - reboundAngle
                    curPosX = -384 - curPosX
                else
                end
                if reboundFlag ~= 0 then
                    reboundCount = reboundCount - 1
                    if reboundFlag == 1 then
                        last = CreateCustomizedLinearLaser("YKS1_F_NSC4_Laser",curPosX,curPosY,reboundAngle,reboundPara,reboundCount)
                    else
                        last = CreateCustomizedLinearLaser("YKS1_F_NSC4_Laser1",curPosX,curPosY,reboundAngle)
                    end
                    reboundCount = 0
                else
                end
            else
            end
            if Wait(1)==false then return end
        end end
    end)
end
CustomizedTable["YKS1_F_NSC4_Laser1"] = {}
CustomizedTable["YKS1_F_NSC4_Laser1"].Init = function(self,angle)
    self:SetStyleById(302011)
    self:SetLength(45)
    self:SetSourceEnable(true)
    self:SetHeadEnable(true)
    self:SetV(3.5,angle,false)
    self:AddTask(function()
        if Wait(45)==false then return end
        self:SetV(0,angle,false)
        if Wait(35)==false then return end
        local normalizedX = cos(angle+180)
        local normalizedY = sin(angle+180)
        local posX,posY = self.x,self.y
        self:SetHeadEnable(false)
        do local i,_d_i=(0),(1) for _=1,5 do
            last = CreateSimpleBulletById(104010,posX + i * normalizedX * 28,posY + i * normalizedY * 28)
            last:SetStraightParas(2,angle + 180,false,0.1,angle)
            last.maxV = 5
            if Wait(2)==false then return end
        i=i+_d_i end end
        DelUnit(self)
    end)
end
SpellCard["YKS1_F_NSC4"] = {}
SpellCard["YKS1_F_NSC4"].Init = function(boss)
    SetSpellCardProperties("",1,30,ConditionEliminateAll,false)
    boss:SetMaxHp(500)
    boss:ShowBloodBar(true)
    boss:MoveTo(0,128,20,IntModeLinear)
    boss:SetWanderRange(-96,96,96,144)
    boss:SetWanderAmplitude(32,64,0,32)
    boss:SetWanderMode(IntModeEaseOutQuad,MoveXTowardsPlayer)
    if Wait(210)==false then return end
    boss:AddTask(function()
        do for _=1,Infinite do
            local laserCount = RandomInt(4,5)
            local angleInterval = RandomFloat(13,17)
            do local angle,_d_angle=(90 + (laserCount - 0.5) * angleInterval),(-angleInterval) for _=1,laserCount * 2 do
                local laserCount = RandomInt(4,5)
                local angleInterval = RandomFloat(13,17)
                last = CreateCustomizedLinearLaser("YKS1_F_NSC4_Laser",boss.x,boss.y,angle,7,5)
            angle=angle+_d_angle end end
            if Wait(45)==false then return end
            boss:Wander(60)
            if Wait(10)==false then return end
            do for _=1,4 do
                do local vAngle,_d_vAngle=(RandomFloat(0,360)),(18) for _=1,20 do
                    last = CreateSimpleBulletById(107010,boss.x,boss.y)
                    last:SetStraightParas(3,vAngle,false,0.05,vAngle)
                    last:DisableAppearEffect()
                vAngle=vAngle+_d_vAngle end end
                if Wait(10)==false then return end
            end end
            if Wait(10)==false then return end
        end end
    end)
end
SpellCard["YKS1_F_NSC4"].OnFinish = function(boss)
end
CustomizedTable["YKS1_F_SC4_Laser"] = {}
CustomizedTable["YKS1_F_SC4_Laser"].Init = function(self,angle,isAimToPlayer)
    self:SetStyleById(201060)
    self:SetSize(0,4)
    if isAimToPlayer then
        angle = angle + Angle(self,player)
    else
    end
    self.rot = angle
    self:SetExistDuration(180)
    self:TurnHalfOn(2,0)
    self:ChangeLengthTo(900,0,50)
end
CustomizedTable["YKS1_F_SC4_Bullet_0"] = {}
CustomizedTable["YKS1_F_SC4_Bullet_0"].Init = function(self)
    self:SetStyleById(127051)
    self:SetV(0,270,false)
    self:DisableAppearEffect()
    self:ChangeProperty(Prop_Alpha,ChangeModeChangeTo,0,0,0,0,30,51,IntModeLinear,1,0)
    self:AddTask(function()
        if Wait(41)==false then return end
        self.checkCollision = false
        if Wait(82)==false then return end
        DelUnit(self)
    end)
end
CustomizedTable["YKS1_F_SC4_Bullet_1"] = {}
CustomizedTable["YKS1_F_SC4_Bullet_1"].Init = function(self,angle,interval,v)
    self:SetStyleById(111061)
    self:AddTask(function()
        do for _=1,Infinite do
            self.x = self.x + v / cos(interval+1) * cos(angle+interval)
            self.y = self.y + v / cos(interval+1) * sin(angle+interval)
            if Wait(1)==false then return end
        end end
    end)
end
CustomizedEnemyTable["YKS1_F_SC4_Enemy"] = {}
CustomizedEnemyTable["YKS1_F_SC4_Enemy"].Init = function(self,startPosX,startPosY,angle,isAimToPlayer)
    self:Init(100022)
    self:SetMaxHp(4)
    self:SetInteractive(false)
    self.checkBorder = false
    if isAimToPlayer then
        angle = angle + Angle(startPosX,startPosY,player.x,player.y)
    else
    end
    self:AddTask(function()
        if Wait(60)==false then return end
        self.checkBorder = true
        self:SetPos(startPosX,startPosY)
        self:SetInteractive(true)
        self:SetAcce(0.15,angle,false)
        self.maxV = 10
    end)
    self:AddTask(function()
        if Wait(60)==false then return end
        do for _=1,Infinite do
            last = CreateCustomizedBullet("YKS1_F_SC4_Bullet_0",self.x,self.y)
            if Wait(4)==false then return end
        end end
    end)
    self:AddTask(function()
        do for _=1,Infinite do
            if self.y <= -224 then
                do local angle,_d_angle=(0),(20) for _=1,10 do
                    last = CreateSimpleBulletById(124031,self.x,self.y)
                    last:SetStraightParas(1.5,angle + RandomFloat(-3,3),false,0,UseVelocityAngle)
                angle=angle+_d_angle end end
                break
            else
            end
            if Wait(1)==false then return end
        end end
    end)
end
CustomizedEnemyTable["YKS1_F_SC4_Enemy"].OnKill = function(self)
    do local angle,_d_angle=(0),(36) for _=1,10 do
        last = CreateSimpleBulletById(124031,self.x,self.y)
        last:SetStraightParas(1.5,angle + RandomFloat(-9,9),false,0,UseVelocityAngle)
    angle=angle+_d_angle end end
end
SpellCard["YKS1_F_SC4"] = {}
SpellCard["YKS1_F_SC4"].Init = function(boss)
    SetSpellCardProperties("SpellCard4",1,60,ConditionEliminateAll,true)
    boss:SetMaxHp(750)
    boss:SetInvincible(5)
    boss:ShowBloodBar(true)
    last = CreateCustomizedSTGObject("NazrinCG",0,0)
    boss:MoveTo(0,128,120,IntModeEaseOutQuad)
    if Wait(120)==false then return end
    boss:AddTask(function()
        CreateChargeEffect(boss.x,boss.y)
        if Wait(90)==false then return end
        do for _=1,Infinite do
            do local angle,_d_angle=(0),(-11) for _=1,25 do
                last = CreateCustomizedLaser("YKS1_F_SC4_Laser",192 * cos(angle),260 + 25 * sin(angle),angle,false)
                last = CreateCustomizedEnemy("YKS1_F_SC4_Enemy",0,300,192 * cos(angle),260 + 25 * sin(angle),angle,false)
                if Wait(4)==false then return end
            angle=angle+_d_angle end end
            CreateChargeEffect(boss.x,boss.y)
            if Wait(90)==false then return end
            do local angle,_d_angle=(0),(-20) for _=1,9 do
                local angleOffset = RandomFloat(-5,5)
                last = CreateCustomizedLaser("YKS1_F_SC4_Laser",192 * cos(angle),240 + 15 * sin(angle),angleOffset,true)
                last = CreateCustomizedEnemy("YKS1_F_SC4_Enemy",0,300,192 * cos(angle),240 + 15 * sin(angle),angleOffset,true)
                if Wait(5)==false then return end
            angle=angle+_d_angle end end
            if Wait(180)==false then return end
        end end
    end)
    boss:AddTask(function()
        if Wait(360)==false then return end
        CreateChargeEffect(boss.x,boss.y)
        if Wait(90)==false then return end
        do local angle,_d_angle=(0),(37) for _=1,Infinite do
            do local angle1,_d_angle1=(angle),(60) for _=1,6 do
                do local interval,_d_interval=(-30),(10) for _=1,7 do
                    last = CreateCustomizedBullet("YKS1_F_SC4_Bullet_1",boss.x,boss.y,angle1,interval,1.5)
                interval=interval+_d_interval end end
            angle1=angle1+_d_angle1 end end
            if Wait(60)==false then return end
        angle=angle+_d_angle end end
    end)
end
SpellCard["YKS1_F_SC4"].OnFinish = function(boss)
    DropItems(boss.x,boss.y,64,64,3,1,5,1,7,1)
end
CustomizedSTGObjectTable["YKS1_NazrinCG_1"] = {}
CustomizedSTGObjectTable["YKS1_NazrinCG_1"].Init = function(self)
    self:SetSprite("Characters/Nazrin","Nazrin",BlendMode_Normal,LayerEffectBottom,false)
    self:SetPos(200,200)
    self:AddTask(function()
        self:MoveTo(0,0,30,IntModeEaseInQuad)
        if Wait(60)==false then return end
        self:ChangeAlphaTo(0,60)
        if Wait(30)==false then return end
        DelUnit(self)
    end)
    self:AddTask(function()
        if Wait(60)==false then return end
        do local width,_d_width=(300),(-300 / 60) local height,_d_height=(512),(-512 / 60) for _=1,60 do
            self:SetSize(width,height)
            if Wait(1)==false then return end
        width=width+_d_width height=height+_d_height end end
    end)
end
CustomizedTable["YKS1_F_SC5_Laser"] = {}
CustomizedTable["YKS1_F_SC5_Laser"].Init = function(self,rot,len,waitTime)
    self:SetStyleById(201061)
    self:SetSize(len,8)
    self.rot = rot
    self:TurnHalfOn(2,10)
    self:AddTask(function()
        if Wait(waitTime-140)==false then return end
        self:ChangeAlphaTo(0,0,100)
        if Wait(140)==false then return end
        self:TurnOn(20)
        PlaySound("se_laserturnon",0.01,false)
        if Wait(60)==false then return end
        self:TurnOff(10)
        if Wait(10)==false then return end
        DelUnit(self)
    end)
end
CustomizedTable["YKS1_F_SC5_Bullet0"] = {}
CustomizedTable["YKS1_F_SC5_Bullet0"].Init = function(self,wait)
    self:SetStyleById(113090)
    self:DisableAppearEffect()
    self:SetV(0,RandomFloat(0,360),false)
    self:ChangeProperty(Prop_Velocity,ChangeModeChangeTo,0,0.1,0,0,wait,1,IntModeLinear,1,0)
    self:ChangeProperty(Prop_Velocity,ChangeModeChangeTo,0,2,0,0,wait + 40,1,IntModeLinear,1,0)
end
CustomizedTable["YKS1_F_SC5_Bullet1"] = {}
CustomizedTable["YKS1_F_SC5_Bullet1"].Init = function(self,circleAngle,radius,toPlayerAngle,toPlayerDis,wait)
    self:SetStyleById(116010)
    self:DisableAppearEffect()
    local toPosX = cos(circleAngle)*radius + cos(toPlayerAngle) * toPlayerDis
    local toPosY = sin(circleAngle)*radius + sin(toPlayerAngle) * toPlayerDis
    self:MoveTo(toPosX,toPosY,60,IntModeEaseOutQuad)
    self:AddTask(function()
        if Wait(wait)==false then return end
        local k = RandomInt(0,1)
        k = k * 2 -1
        self:SetPolarParas(Distance(0,0,self.x,self.y),Angle(0,0,self.x,self.y),RandomFloat(0.1,1) * k,RandomFloat(0.5,1) * k,0,0)
        self:ChangeProperty(Prop_Alpha,ChangeModeChangeTo,0,0,0,0,90,90,IntModeLinear,1,0)
        if Wait(180)==false then return end
        DelUnit(self)
    end)
end
SpellCard["YKS1_F_SC5"] = {}
SpellCard["YKS1_F_SC5"].Init = function(boss)
    SetSpellCardProperties("TestSC5",1,45,ConditionTimeOver,true)
    boss:SetMaxHp(500)
    boss:SetInteractive(false)
    last = CreateCustomizedSTGObject("YKS1_NazrinCG_1",0,0)
    boss:MoveTo(0,0,60,IntModeLinear)
    if Wait(60)==false then return end
    boss:AddTask(function()
        local fieldRange = {}
        fieldRange[1] = {-192,0,-224,-224,3,4,6,7,8}
        fieldRange[2] = {0,192,-224,-224,3,4,5,6,8}
        fieldRange[3] = {-192,0,224,224,1,2,5,7,8}
        fieldRange[4] = {0,192,224,224,1,2,5,6,7}
        fieldRange[5] = {-192,-192,-224,0,2,3,4,7,8}
        fieldRange[6] = {-192,-192,0,224,1,2,4,7,8}
        fieldRange[7] = {192,192,-224,0,1,3,4,5,6}
        fieldRange[8] = {192,192,0,224,1,2,3,5,6}
        local GetFieldIndex = function(inputX,inputY)
        	if inputY == -224 then
        		if inputX <= 0 then
        			return 1
        		else
        			return 2
        		end
        	elseif inputY == 224 then
        		if inputX <= 0 then
        			return 3
        		else
        			return 4
        		end
        	elseif inputX == -192 then
        		if inputY <= 0 then
        			return 5
        		else
        			return 6
        		end
        	elseif inputX == 192 then
        		if inputY <= 0 then
        			return 7
        		else
        			return 8
        		end
        	end
        end
        local startIndex = RandomInt(1,8)
        local startX = RandomFloat(fieldRange[startIndex][1],fieldRange[startIndex][2])
        local startY = RandomFloat(fieldRange[startIndex][3],fieldRange[startIndex][4])
        local endIndex,endX,endY = 1,0,0
        CreateChargeEffect(0,0)
        if Wait(60)==false then return end
        do local k,_d_k=(2),(1) for _=1,4 do
            do local i,_d_i=(0),(1) for _=1,15 * k do
                endIndex = fieldRange[startIndex][RandomInt(5,9)]
                endX = RandomFloat(fieldRange[endIndex][1],fieldRange[endIndex][2])
                endY = RandomFloat(fieldRange[endIndex][3],fieldRange[endIndex][4])
                local laserAngle = Angle(startX,startY,endX,endY)
                local laserLen = Distance(startX,startY,endX,endY)
                last = CreateCustomizedLaser("YKS1_F_SC5_Laser",startX,startY,laserAngle,laserLen,140 + k * 60 - i)
                local factor = RandomFloat(0.2,0.8)
                local bulletPosX = startX * factor + endX * (1-factor)
                local bulletPosY = startY * factor + endY * (1-factor)
                if Distance(bulletPosX,bulletPosY,player.x,player.y) > 40 then
                    last = CreateCustomizedBullet("YKS1_F_SC5_Bullet0",bulletPosX,bulletPosY,180 + k * 60 - i)
                else
                end
                startX = endX
                startY = endY
                startIndex = endIndex
                if Wait(1)==false then return end
            i=i+_d_i end end
            if Wait(20)==false then return end
            CreateChargeEffect(0,0)
            if Wait(60)==false then return end
            do local waitTime,_d_waitTime=(120 + 60 * k),(-60) for _=1,k do
                PlaySound("se_tan00",0.05,false)
                do local startAngle,_d_startAngle=(RandomFloat(0,360)),(30) for _=1,12 do
                    local toPlayerAngle = Angle(0,0,player.x,player.y)
                    local toPlayerDis = Distance(0,0,player.x,player.y)
                    last = CreateCustomizedBullet("YKS1_F_SC5_Bullet1",0,0,startAngle,40,toPlayerAngle,toPlayerDis,waitTime)
                startAngle=startAngle+_d_startAngle end end
                if Wait(60)==false then return end
            waitTime=waitTime+_d_waitTime end end
            if Wait(300)==false then return end
        k=k+_d_k end end
    end)
end
SpellCard["YKS1_F_SC5"].OnFinish = function(boss)
end
CustomizedSTGObjectTable["Stage1Logo"] = {}
CustomizedSTGObjectTable["Stage1Logo"].Init = function(self)
    self:SetSprite("Stages/Stage1","Logo",BlendMode_Normal,LayerEffectNormal,false)
    self.alpha = 0
    self:AddTask(function()
        self:ChangeAlphaTo(1,60)
        if Wait(300)==false then return end
        self:ChangeAlphaTo(0,60)
        if Wait(60)==false then return end
        DelUnit(self)
    end)
end
CustomizedSTGObjectTable["TestVisionMask"] = {}
CustomizedSTGObjectTable["TestVisionMask"].Init = function(self)
    self:SetPrefab("VisionMaskEffect",LayerEffectTop,false)
    self:AddTask(function()
        do for _=1,Infinite do
            self:SetMaterialFloat("_PlayerX",player.x)
            self:SetMaterialFloat("_PlayerY",player.y)
            if Wait(1)==false then return end
        end end
    end)
end
Stage["Stage1"] = function()
    PlaySound("oborozuki",0.5,true)
    do  --初始敌机
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
    do  --Logo
        last = CreateCustomizedSTGObject("Stage1Logo",70,130)
        if Wait(400)==false then return end
    end
    do  --Phase2
        last = CreateCustomizedEnemy("YKStage1Enemy1_1",-130,240,30,6)
        last = CreateCustomizedEnemy("YKStage1Enemy1_1",130,240,30,6)
        if Wait(720)==false then return end
        last = CreateCustomizedEnemy("YKStage1Enemy1",0,240,240,true)
        if Wait(600)==false then return end
    end
    do  --Phase3
        if Wait(120)==false then return end
        do for _=1,20 do
            last = CreateCustomizedEnemy("YKStage1Enemy2",-150,240,1)
            last = CreateCustomizedEnemy("YKStage1Enemy2",-75,240,1)
            last = CreateCustomizedEnemy("YKStage1Enemy2",75,240,-1)
            last = CreateCustomizedEnemy("YKStage1Enemy2",150,240,-1)
            if Wait(30)==false then return end
        end end
        if Wait(300)==false then return end
    end
    do  --Phase4
        if Wait(200)==false then return end
        last = CreateCustomizedEnemy("YKStage1Enemy3",0,240)
        if Wait(1000)==false then return end
    end
    do  --Phase5-MidBoss
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
        local boss = CreateBoss("YKS1_MidBoss",0,280)
        boss:ShowAura(true,false)
        boss:ShowPosHint(true)
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
    do  --Phase6
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
    do  --Phase7
        if Wait(200)==false then return end
        last = CreateCustomizedEnemy("YKStage1Enemy6",0,240)
        if Wait(1800)==false then return end
    end
    do  --Phase8
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
    do  --FinalBoss
        StopSound("oborozuki")
        if Wait(200)==false then return end
        --dialog
        if player.characterIndex == 0 then
            if StartDialog(function()
                CreateDialogCG("Player","Reimu",100,150)
                if Wait(30)==false then return end
                HighlightDialogCG("Player",true)
                CreateDialogBox(0,"TestDialogBox0.....",100,150,120,1)
                if Wait(120)==false then return end
                HighlightDialogCG("Player",false)
            end) == false then return end
        else
            if StartDialog(function()
                CreateDialogCG("Player","Marisa",100,150)
                if Wait(30)==false then return end
                HighlightDialogCG("Player",true)
                CreateDialogBox(0,"TestDialogBox0.....",100,150,120,1)
                if Wait(120)==false then return end
                HighlightDialogCG("Player",false)
            end) == false then return end
        end
        ShowBossInfo("Nazrin",0)
        local boss = CreateBoss("YKS1_FinalBoss",0,280)
        boss:MoveTo(0,170,90,IntModeEaseInQuad)
        if Wait(100)==false then return end
        --dialog
        if player.characterIndex == 0 then
            if StartDialog(function()
                CreateDialogCG("Nazrin","Nazrin",450,150)
                if Wait(30)==false then return end
                HighlightDialogCG("Nazrin",true)
                CreateDialogBox(0,"TestDialogBox1.....",450,150,120,-1)
                if Wait(120)==false then return end
                HighlightDialogCG("Nazrin",false)
                HighlightDialogCG("Player",true)
                CreateDialogBox(0,"TestDialogBox2....",100,150,120,1)
                if Wait(120)==false then return end
                FadeOutDialogCG("Nazrin")
                FadeOutDialogCG("Player")
                if Wait(100)==false then return end
            end) == false then return end
        else
            if StartDialog(function()
                CreateDialogCG("Nazrin","Nazrin",450,150)
                if Wait(30)==false then return end
                HighlightDialogCG("Nazrin",true)
                CreateDialogBox(0,"TestDialogBox1.....",450,150,120,-1)
                if Wait(120)==false then return end
                HighlightDialogCG("Nazrin",false)
                HighlightDialogCG("Player",true)
                CreateDialogBox(0,"TestDialogBox2....",100,150,120,1)
                if Wait(120)==false then return end
                FadeOutDialogCG("Nazrin")
                FadeOutDialogCG("Player")
                if Wait(100)==false then return end
            end) == false then return end
        end
        ShowBossInfo("Nazrin",5)
        PlaySound("bossbgm",0.25,true)
        boss:SetPhaseData(4,1,true)
        StartSpellCard(SpellCard["YKS1_F_NSC1"],boss)
        if WaitForSpellCardFinish() == false then return end
        StartSpellCard(SpellCard["YKS1_F_SC1"],boss)
        if WaitForSpellCardFinish() == false then return end
        boss:SetPhaseData(4,1,true)
        StartSpellCard(SpellCard["YKS1_F_NSC2"],boss)
        if WaitForSpellCardFinish() == false then return end
        StartSpellCard(SpellCard["YKS1_F_SC2"],boss)
        if WaitForSpellCardFinish() == false then return end
        boss:SetPhaseData(4,1,true)
        StartSpellCard(SpellCard["YKS1_F_NSC3"],boss)
        if WaitForSpellCardFinish() == false then return end
        StartSpellCard(SpellCard["YKS1_F_SC3"],boss)
        if WaitForSpellCardFinish() == false then return end
        boss:SetPhaseData(4,1,true)
        StartSpellCard(SpellCard["YKS1_F_NSC4"],boss)
        if WaitForSpellCardFinish() == false then return end
        StartSpellCard(SpellCard["YKS1_F_SC4"],boss)
        if WaitForSpellCardFinish() == false then return end
        if Wait(60)==false then return end
        if StartDialog(function()
            CreateDialogCG("Nazrin","Nazrin",450,150)
            if Wait(30)==false then return end
            HighlightDialogCG("Nazrin",true)
            CreateDialogBox(0,"Last SC..",450,150,120,-1)
            if Wait(120)==false then return end
            FadeOutDialogCG("Nazrin")
            if Wait(100)==false then return end
        end) == false then return end
        boss:SetPhaseData(1,true)
        StartSpellCard(SpellCard["YKS1_F_SC5"],boss)
        if WaitForSpellCardFinish() == false then return end
        if Wait(30)==false then return end
        CreateBossDeadEffect(boss.x,boss.y)
        PlaySound("se_bossexplosive",0.2,false)
        if Wait(60)==false then return end
        DelUnit(boss)
    end
    if Wait(60)==false then return end
    if StartDialog(function()
        CreateDialogBox(0,"Test stage is finished...",170,250,120,0.8)
        if Wait(120)==false then return end
        CreateDialogBox(0,"Thank you for testing...",170,250,120,0.8)
        if Wait(100)==false then return end
    end) == false then return end
    FinishStage()
end
Stage["Stage2"] = function()
    PlaySound("bgm",0.5,true)
    do  --TestEnemy
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
    do  --Boss
        if Wait(300)==false then return end
        if StartDialog(function()
            CreateDialogCG("Marisa","Marisa",100,150)
            if Wait(30)==false then return end
            HighlightDialogCG("Marisa",true)
            CreateDialogBox(0,"TestDialogBox0.....",100,150,120,1)
            if Wait(120)==false then return end
            HighlightDialogCG("Marisa",false)
        end) == false then return end
        local boss = CreateBoss("Marisa",0,280)
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