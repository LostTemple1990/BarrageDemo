using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarrageEditor
{
    public struct EnemyStyleCfg
    {
        /// <summary>
        /// 分组
        /// </summary>
        public int group;
        public int styleId;
        public string name;
        public string packName;
        public string resName;
    }

    public class EnemyDatabase
    {
        private List<EnemyStyleCfg> _styleCfgList;

        public EnemyDatabase()
        {
            _styleCfgList = new List<EnemyStyleCfg>();
            #region 普通妖精
            EnemyStyleCfg cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100000,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy000",
            };
            _styleCfgList.Add(cfg);

            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100001,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy001",
            };
            _styleCfgList.Add(cfg);

            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100002,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy002",
            };
            _styleCfgList.Add(cfg);

            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100003,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy003",
            };
            _styleCfgList.Add(cfg);

            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100004,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy004",
            };
            _styleCfgList.Add(cfg);

            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100005,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy005",
            };
            _styleCfgList.Add(cfg);

            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100006,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy006",
            };
            _styleCfgList.Add(cfg);

            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100007,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy007",
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 中型妖精
            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100030,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy030",
            };
            _styleCfgList.Add(cfg);

            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100031,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy031",
            };
            _styleCfgList.Add(cfg);

            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100040,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy040",
            };
            _styleCfgList.Add(cfg);

            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100041,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy041",
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 大妖精
            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100010,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy010",
            };
            _styleCfgList.Add(cfg);

            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100011,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy011",
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 阴阳玉
            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100020,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy020",
            };
            _styleCfgList.Add(cfg);

            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100021,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy021",
            };
            _styleCfgList.Add(cfg);

            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100022,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy022",
            };
            _styleCfgList.Add(cfg);

            cfg = new EnemyStyleCfg
            {
                group = 1,
                styleId = 100023,
                name = "Style1",
                packName = "STGEnemyAtlas",
                resName = "enemy023",
            };
            _styleCfgList.Add(cfg);
            #endregion
        }

        public List<EnemyStyleCfg> GetEnemyStyleCfgs()
        {
            return _styleCfgList;
        }

        /// <summary>
        /// 获取敌机配置
        /// </summary>
        /// <param name="styleId"></param>
        /// <returns></returns>
        public EnemyStyleCfg GetEnemyStyleCfg(int styleId)
        {
            int len = _styleCfgList.Count;
            for (int i = 0; i < len; i++)
            {
                if (_styleCfgList[i].styleId == styleId)
                {
                    return _styleCfgList[i];
                }
            }
            return new EnemyStyleCfg
            {
                styleId = -1
            };
        }
    }
}
