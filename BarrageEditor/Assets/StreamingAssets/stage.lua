local lib = require "LuaLib"
local consts = Constants
local Stage = {}
local CustomizedTable = {}
local CustomizedEnemyTable = {}
local BossTable = {}

-- Mod name: unnamed
--author="YK"
--allow_practice=true
CustomizedEnemyTable.TestOnKillEnemy = {}
CustomizedEnemyTable.TestOnKillEnemy.Init = function(self)
    self:SetMaxHp(10)
end
CustomizedTable.YKBullet = {}
CustomizedTable.YKBullet.Init = function(self,v,angle)
    self:SetV(3,0,false)
    self:SetAcce(0.05,0,false)
end
Stage["Stage1"] = function()
    last = lib.CreateCustomizedEnemy("TestOnKillEnemy",100000,0,0,0)
    last = lib.CreateCustomizedBullet("YKBullet",107010,0,0,3,0,2)
end
return
{
   CustomizedBulletTable = CustomizedTable,
   CustomizedEnemyTable = CustomizedEnemyTable,
   BossTable = BossTable,
   Stage = Stage,
}