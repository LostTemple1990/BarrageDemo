using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarrageEditor
{
    public struct LaserStyleCfg
    {
        /// <summary>
        /// 射线类型，直线激光、激光、曲线激光
        /// </summary>
        public BulletType type;
        /// <summary>
        /// 类型前缀的数字
        /// </summary>
        public int typePrefixNum;
        public int styleId;
        public string name;
        public string packName;
        public string resName;
        public float showScale;
        public List<int> availableColors;
    }

    public class LaserDatabase
    {
        private List<LaserStyleCfg> _styleCfgList;
        private List<ColorCfg> _colorCfgList;

        public LaserDatabase()
        {
            #region Style
            _styleCfgList = new List<LaserStyleCfg>();
            // Laser
            #region 普通激光
            LaserStyleCfg cfg = new LaserStyleCfg
            {
                type = BulletType.Laser,
                typePrefixNum = 2,
                styleId = 1,
                name = "Style1",
                packName = "STGLaserAtlas0",
                resName = "Laser201010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 尖头激光
            cfg = new LaserStyleCfg
            {
                type = BulletType.Laser,
                typePrefixNum = 2,
                styleId = 2,
                name = "Style2",
                packName = "STGLaserAtlas0",
                resName = "Laser202010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 鳞弹头激光
            cfg = new LaserStyleCfg
            {
                type = BulletType.Laser,
                typePrefixNum = 2,
                styleId = 3,
                name = "Style3",
                packName = "STGLaserAtlas0",
                resName = "Laser203010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 米弹激光
            cfg = new LaserStyleCfg
            {
                type = BulletType.Laser,
                typePrefixNum = 2,
                styleId = 4,
                name = "Style4",
                packName = "STGLaserAtlas0",
                resName = "Laser204010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            // LinearLaser
            #region 尖头激光
            cfg = new LaserStyleCfg
            {
                type = BulletType.LinearLaser,
                typePrefixNum = 3,
                styleId = 2,
                name = "Style2",
                packName = "STGLinearLaserAtlas0",
                resName = "Laser302010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 鳞弹头激光
            cfg = new LaserStyleCfg
            {
                type = BulletType.LinearLaser,
                typePrefixNum = 3,
                styleId = 3,
                name = "Style3",
                packName = "STGLinearLaserAtlas0",
                resName = "Laser303010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 米弹激光
            cfg = new LaserStyleCfg
            {
                type = BulletType.LinearLaser,
                typePrefixNum = 3,
                styleId = 4,
                name = "Style4",
                packName = "STGLinearLaserAtlas0",
                resName = "Laser304010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 曲线激光贴图
            cfg = new LaserStyleCfg
            {
                type = BulletType.LinearLaser,
                typePrefixNum = 3,
                styleId = 5,
                name = "Style5",
                packName = "STGLinearLaserAtlas1",
                resName = "Laser305010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            // CurveLaser
            #region 曲线激光贴图
            cfg = new LaserStyleCfg
            {
                type = BulletType.CurveLaser,
                typePrefixNum = 4,
                styleId = 1,
                name = "Style1",
                packName = "STGCurveLaserAtlas",
                resName = "Laser405010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #endregion

            #region color
            _colorCfgList = new List<ColorCfg>();
            #region DeepGray
            ColorCfg colorCfg = new ColorCfg
            {
                colorId = 0,
                colorName = "DeepGray",
                packName = "ColorAtlas",
                resName = "COLOR_DEEP_GRAY",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region DeepRed
            colorCfg = new ColorCfg
            {
                colorId = 1,
                colorName = "DeepRed",
                packName = "ColorAtlas",
                resName = "COLOR_DEEP_RED",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region Red
            colorCfg = new ColorCfg
            {
                colorId = 2,
                colorName = "Red",
                packName = "ColorAtlas",
                resName = "COLOR_RED",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region DeepPurple
            colorCfg = new ColorCfg
            {
                colorId = 3,
                colorName = "DeepPurple",
                packName = "ColorAtlas",
                resName = "COLOR_DEEP_PURPLE",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region Purple
            colorCfg = new ColorCfg
            {
                colorId = 4,
                colorName = "Purple",
                packName = "ColorAtlas",
                resName = "COLOR_PURPLE",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region DeepBlue
            colorCfg = new ColorCfg
            {
                colorId = 5,
                colorName = "DeepBlue",
                packName = "ColorAtlas",
                resName = "COLOR_DEEP_BLUE",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region Blue
            colorCfg = new ColorCfg
            {
                colorId = 6,
                colorName = "Blue",
                packName = "ColorAtlas",
                resName = "COLOR_BLUE",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region RoyalBlue
            colorCfg = new ColorCfg
            {
                colorId = 7,
                colorName = "RoyalBlue",
                packName = "ColorAtlas",
                resName = "COLOR_ROYAL_BLUE",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region Cyan
            colorCfg = new ColorCfg
            {
                colorId = 8,
                colorName = "Cyan",
                packName = "ColorAtlas",
                resName = "COLOR_DEEP_CYAN",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region DeepGreen
            colorCfg = new ColorCfg
            {
                colorId = 9,
                colorName = "DeepGreen",
                packName = "ColorAtlas",
                resName = "COLOR_DEEP_GREEN",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region Green
            colorCfg = new ColorCfg
            {
                colorId = 10,
                colorName = "Green",
                packName = "ColorAtlas",
                resName = "COLOR_GREEN",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region Chartreuse 
            colorCfg = new ColorCfg
            {
                colorId = 11,
                colorName = "Chartreuse",
                packName = "ColorAtlas",
                resName = "COLOR_CHARTREUSE",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region Yellow
            colorCfg = new ColorCfg
            {
                colorId = 12,
                colorName = "Yellow",
                packName = "ColorAtlas",
                resName = "COLOR_YELLOW",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region GoldenYellow
            colorCfg = new ColorCfg
            {
                colorId = 13,
                colorName = "GoldenYellow",
                packName = "ColorAtlas",
                resName = "COLOR_GOLDEN_YELLOW",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region Orange
            colorCfg = new ColorCfg
            {
                colorId = 14,
                colorName = "Orange",
                packName = "ColorAtlas",
                resName = "COLOR_ORANGE",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #region Gray
            colorCfg = new ColorCfg
            {
                colorId = 15,
                colorName = "Gray",
                packName = "ColorAtlas",
                resName = "COLOR_GRAY",
            };
            _colorCfgList.Add(colorCfg);
            #endregion
            #endregion
        }

        /// <summary>
        /// 获取激光的配置
        /// </summary>
        /// <returns></returns>
        public List<LaserStyleCfg> GetLaserStyleCfgsByType(BulletType type)
        {
            List<LaserStyleCfg> cfgs = new List<LaserStyleCfg>();
            LaserStyleCfg cfg;
            int len = _styleCfgList.Count;
            for (int i=0;i<len;i++)
            {
                cfg = _styleCfgList[i];
                if (cfg.type == type)
                {
                    cfgs.Add(cfg);
                }
            }
            return cfgs;
        }

        public List<ColorCfg> GetColorCfgs()
        {
            return _colorCfgList;
        }

        /// <summary>
        /// 获取弹型配置
        /// </summary>
        /// <param name="styleId"></param>
        /// <returns></returns>
        public LaserStyleCfg GetLaserStyleCfg(BulletType type,int styleId)
        {
            int len = _styleCfgList.Count;
            for (int i=0;i<len;i++)
            {
                if (_styleCfgList[i].type == type && _styleCfgList[i].styleId == styleId)
                {
                    return _styleCfgList[i];
                }
            }
            return new LaserStyleCfg
            {
                type = BulletType.Undefined
            };
        }

        /// <summary>
        /// 获取子弹颜色配置
        /// </summary>
        /// <param name="colorId"></param>
        /// <returns></returns>
        public ColorCfg GetColorCfgByColorId(int colorId)
        {
            return _colorCfgList[colorId];
        }
    }
}
