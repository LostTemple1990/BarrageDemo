local lib = require "LuaLib"
local consts = Constants
local Stage = {}
local CustomizedTable = {}
local CustomizedEnemyTable = {}
local BossTable = {}

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
    self:SetPos(0,0)
    self:SetCollisionSize(32,32)
end
SpellCard["TestSC"] = {}
SpellCard["TestSC"].Init = function(boss)
    SetSpellCardProperties("unknown spell card",1,60,ConditionEliminateAll,nil)
end
SpellCard["TestSC"].OnFinish = function(boss)
end
CustomizedTable["NazrinSC1Bullet0"] = {}
CustomizedTable["NazrinSC1Bullet0"].Init = function(self,waitTime,maxRadius)
    self:SetResistEliminatedTypes(7)
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
                        last:SetStraightParas(2,angle,true,0.25,VelocityAngle)
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
        if Wait(100000)==false then return end
        last = CreateBoss("Marisa")
        local boss = last
        StartSepllCard(SpellCard["TestSC"],boss)
        if WaitForSpellCardFinish() == false then return end
    end
end
return
{
   CustomizedBulletTable = CustomizedTable,
   CustomizedEnemyTable = CustomizedEnemyTable,
   BossTable = BossTable,
   Stage = Stage,
}