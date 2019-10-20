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
    self:SetDropItems(PPointNormal,3,48,48)
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
    self:SetSprite("Characters/Satori","Satori",BlendMode_Normal,LayerEffectBottom,false)
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
    boss:SetMaxHp(50)
    boss:SetInvincible(5)
    boss:ShowBloodBar(true)
    --圈形子弹
    boss:AddTask(function()
        local k = 1
        do for _=1,Infinite do
            local posX,posY = boss.x,boss.y
            last = CreateCustomizedBullet1("NazrinSC1Bullet0",posX,posY,0,75)
            local master = last
            last:SetPolarParas(0,23.5*0*k,0,1*k)
            last = CreateCustomizedSTGObject("NazrinShield",0,0,posX,posY,80)
            last:AttachTo(master,true)
            do local i,_d_i=(1),(1) for _=1,15 do
                if Wait(1)==false then return end
                last = CreateCustomizedBullet1("NazrinSC1Bullet0",posX,posY,i,75)
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
CustomizedTable["竖版开海"].Init = function(self)
    self:SetStyleById(305051)
    self:SetLength(20)
    self:SetSourceEnable(false)
    self:SetHeadEnable(true)
    self:SetV(8,270,false)
end
CustomizedSTGObjectTable["竖版开海"] = {}
CustomizedSTGObjectTable["竖版开海"].Init = function(self)
    self:SetSprite("STGEffectAtlas","MapleLeaf1",BlendMode_Normal,LayerEffectNormal,false)
    self:AddTask(function()
        do for _=1,Infinite do
            local leftPosX,rightPosX = -185,185
            do local i,_d_i=(1),(1) for _=1,25 do
                local k = i / 25
                k = math.pow(k,1.4)
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX + 171 * k,224)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX - 171 * k,224)
                if Wait(2)==false then return end
            i=i+_d_i end end
            leftPosX,rightPosX = -14,14
            do local i,_d_i=(1),(1) for _=1,25 do
                local k = i / 25
                k = math.pow(k,1.4)
                last = CreateCustomizedLinearLaser("竖版开海",leftPosX - 171 * k,224)
                last = CreateCustomizedLinearLaser("竖版开海",rightPosX + 171 * k,224)
                if Wait(2)==false then return end
            i=i+_d_i end end
        end end
    end)
end
LoadSound("oborozuki")
LoadSound("se_tan00")
CustomizedEnemyTable["YKStage1Enemy0"] = {}
CustomizedEnemyTable["YKStage1Enemy0"].Init = function(self,angle)
    self:Init(100000)
    self:SetMaxHp(20)
    self:SetDropItems(PPointNormal,1,48,48)
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
    end
end
Stage["Stage2"] = function()
    PlaySound("bgm",0.5,true)
    do
        last = CreateCustomizedEnemy("TestOnKillEnemy",0,185)
        local enemy = last
        if Wait(10000)==false then return end
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
                    last = CreateCustomizedBullet1("NazrinSC1Bullet0",posX,posY,i,75)
                    last:SetPolarParas(0,23.5*i*k,0,1*k)
                    if Wait(1)==false then return end
                i=i+_d_i end end
                k = -k
                if Wait(90)==false then return end
            end end
        end)
    end
    do
        if Wait(300)==false then return end
        last = CreateBoss("Marisa")
        local boss = last
        boss:MoveTo(0,170,90,IntModeEaseInQuad)
        if Wait(100)==false then return end
        boss:SetPhaseData(1,1,1,1,true)
        StartSpellCard(SpellCard["NazrinSC1_0"],boss)
        if WaitForSpellCardFinish() == false then return end
    end
    last = CreateCustomizedSTGObject("竖版开海",500,0)
end

SetDebugStageName("__TestSCStage")
BossTable["__TestSCBoss"] = {}
BossTable["__TestSCBoss"].Init = function(self)
    self:SetAni(2001)
    self:SetPos(0,280)
    self:SetCollisionSize(32,32)
end
Stage["__TestSCStage"] = function()
    last = CreateBoss("__TestSCBoss")
    local boss = last
    boss:MoveTo(0,170,90,IntModeEaseInQuad)
    if Wait(100)==false then return end
    boss:SetPhaseData(1,true)
    StartSpellCard(SpellCard["NazrinSC1_0"],boss)
    if WaitForSpellCardFinish() == false then return end
end

return
{
   CustomizedBulletTable = CustomizedTable,
   CustomizedEnemyTable = CustomizedEnemyTable,
   BossTable = BossTable,
   CustomizedSTGObjectTable = CustomizedSTGObjectTable,
   Stage = Stage,
}