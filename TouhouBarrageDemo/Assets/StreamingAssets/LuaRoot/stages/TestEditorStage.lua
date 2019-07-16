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
    self:SetMaxHp(10)
end
CustomizedTable["YKBullet"] = {}
CustomizedTable["YKBullet"].Init = function(self,v,angle)
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
    SetSpellCardProperties("unknown spell card",1,60,ConditionEliminateAll,nil)
end
SpellCard["TestSC"].OnFinish = function(boss)
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
    SetSpellCardProperties("Easy-SpellCard",1,60,ConditionEliminateAll,nil)
    last = CreateCustomizedSTGObject("NazrinCG")
    boss:SetMaxHp(5000)
    boss:SetInvincible(5)
    boss:ShowBloodBar(true)
    --圈形子弹
    boss:AddTask(function()
        local k = 1
        do for _=1,Infinite do
            local posX,posY = boss.x,boss.y
            last = CreateCustomizedBullet("NazrinSC1Bullet0",113020,posX,posY,0,75,2)
            local master = last
            last:SetPolarParas(0,23.5*0*k,0,1*k)
            last = CreateCustomizedSTGObject("NazrinShield",posX,posY,80)
            last:AttachTo(master,true)
            do local i,_d_i=(1),(1) for _=1,15 do
                if Wait(1)==false then return end
                last = CreateCustomizedBullet("NazrinSC1Bullet0",113020,posX,posY,i,75,2)
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
            last = CreateCustomizedEnemy("NazrinSC1Enemy",100020,boss.x,boss.y,-170,205,duration,3)
            last = CreateCustomizedEnemy("NazrinSC1Enemy",100020,boss.x,boss.y,170,205,duration,3)
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
Stage["Stage1"] = function()
    do
        last = CreateCustomizedEnemy("TestOnKillEnemy",100000,0,185,0)
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
                    last = CreateCustomizedBullet("NazrinSC1Bullet0",113020,posX,posY,i,75,2)
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