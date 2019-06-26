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
        self:MoveTo(0,0,60,MoveModeLinear)
        if Wait(60)==false then return end
    end)
end
BossTable["Marisa"] = {}
BossTable["Marisa"].Init = function(self)
    self:SetAni(2001)
    self:SetPos(0,0)
    self:SetCollisionSize(32,32)
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
    end
end
return
{
   CustomizedBulletTable = CustomizedTable,
   CustomizedEnemyTable = CustomizedEnemyTable,
   BossTable = BossTable,
   Stage = Stage,
}