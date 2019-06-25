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
    end
end
BossTable["Marisa"] = {}
BossTable["Marisa"].Init = function(self)
    self:SetAni(2001)
    self:SetPos(0,0)
    self:SetCollisionSize(32,32)
end
SC["MarisaNonSC0"] = function(boss)
    SetSpellCardProperties("unknown spell card",1,60,ConditionEliminateAll,nil)
end
Stage["Stage1"] = function()
    last = CreateCustomizedEnemy("TestOnKillEnemy",100000,0,0,0)
    last = CreateCustomizedBullet("YKBullet",107010,0,0,3,0,2)
    do
        last = CreateBoss("Marisa")
        local boss = last
        StartSepllCard(["MarisaNonSC0"],boss)
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