using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarrageEditor
{
    public struct BulletStyleCfg
    {
        public int styleId;
        public string name;
        public string packName;
        public string resName;
        public float showScale;
        public List<int> availableColors;
    }

    public struct ColorCfg
    {
        public int colorId;
        public string colorName;
        public string packName;
        public string resName;
    }

    public class BulletDatabase
    {
        private List<BulletStyleCfg> _styleCfgList;
        private List<ColorCfg> _colorCfgList;

        public BulletDatabase()
        {
            #region bulletStyle
            _styleCfgList = new List<BulletStyleCfg>();
            #region 点弹
            BulletStyleCfg cfg = new BulletStyleCfg
            {
                styleId = 0,
                name = "点弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet100010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 葡萄弹
            cfg = new BulletStyleCfg
            {
                styleId = 1,
                name = "葡萄弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet101010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 小玉
            cfg = new BulletStyleCfg
            {
                styleId = 2,
                name = "小玉",
                packName = "STGBulletsAtlas",
                resName = "Bullet102010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 环玉
            cfg = new BulletStyleCfg
            {
                styleId = 3,
                name = "环玉",
                packName = "STGBulletsAtlas",
                resName = "Bullet103010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 米弹
            cfg = new BulletStyleCfg
            {
                styleId = 4,
                name = "米弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet104010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 链弹
            cfg = new BulletStyleCfg
            {
                styleId = 5,
                name = "链弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet105010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 针弹
            cfg = new BulletStyleCfg
            {
                styleId = 6,
                name = "针弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet106010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 札弹
            cfg = new BulletStyleCfg
            {
                styleId = 7,
                name = "札弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet107010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 鳞弹
            cfg = new BulletStyleCfg
            {
                styleId = 8,
                name = "鳞弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet108010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 铳弹
            cfg = new BulletStyleCfg
            {
                styleId = 9,
                name = "铳弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet109010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 杆菌弹
            cfg = new BulletStyleCfg
            {
                styleId = 10,
                name = "杆菌弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet110010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 星弹（小）
            cfg = new BulletStyleCfg
            {
                styleId = 11,
                name = "星弹（小）",
                packName = "STGBulletsAtlas",
                resName = "Bullet111010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 钱币
            cfg = new BulletStyleCfg
            {
                styleId = 12,
                name = "钱币",
                packName = "STGBulletsAtlas",
                resName = "Bullet112010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 中玉
            cfg = new BulletStyleCfg
            {
                styleId = 13,
                name = "中玉",
                packName = "STGBulletsAtlas",
                resName = "Bullet113010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 椭弹
            cfg = new BulletStyleCfg
            {
                styleId = 14,
                name = "椭弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet114010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 3, 5, 7, 9, 13, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 刀弹
            cfg = new BulletStyleCfg
            {
                styleId = 15,
                name = "刀弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet115010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 3, 5, 7, 9, 13, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 蝶弹
            cfg = new BulletStyleCfg
            {
                styleId = 16,
                name = "蝶弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet116010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 3, 5, 7, 9, 13, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 星弹（大）
            cfg = new BulletStyleCfg
            {
                styleId = 17,
                name = "星弹（大）",
                packName = "STGBulletsAtlas",
                resName = "Bullet117010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 3, 5, 7, 9, 13, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 大玉
            cfg = new BulletStyleCfg
            {
                styleId = 18,
                name = "大玉",
                packName = "STGBulletsAtlas",
                resName = "Bullet118010",
                showScale = 1,
                availableColors = new List<int> { 1, 5, 9, 13 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 蔷薇
            cfg = new BulletStyleCfg
            {
                styleId = 19,
                name = "蔷薇",
                packName = "STGBulletsAtlas",
                resName = "Bullet119010",
                showScale = 1,
                availableColors = new List<int> { 1, 5, 9, 13 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 心弹
            cfg = new BulletStyleCfg
            {
                styleId = 20,
                name = "心弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet120010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 滴弹
            cfg = new BulletStyleCfg
            {
                styleId = 21,
                name = "滴弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet121010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 3, 5, 7, 9, 13, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 大光玉
            cfg = new BulletStyleCfg
            {
                styleId = 22,
                name = "大光玉",
                packName = "STGBulletsAtlas",
                resName = "Bullet122010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 3, 5, 7, 9, 13, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 箭弹
            cfg = new BulletStyleCfg
            {
                styleId = 23,
                name = "箭弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet123010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 3, 5, 7, 9, 13, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 小光玉
            cfg = new BulletStyleCfg
            {
                styleId = 24,
                name = "小光玉",
                packName = "STGBulletsAtlas",
                resName = "Bullet124010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 3, 5, 7, 9, 13, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 休止符
            cfg = new BulletStyleCfg
            {
                styleId = 25,
                name = "休止符",
                packName = "STGBulletsAtlas",
                resName = "Bullet125010",
                showScale = 1,
                availableColors = new List<int> { 0, 1, 3, 5, 7, 9, 13, 15 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 炎弹
            cfg = new BulletStyleCfg
            {
                styleId = 26,
                name = "炎弹",
                packName = "STGBulletsAtlas",
                resName = "Bullet126010_0",
                showScale = 1,
                availableColors = new List<int> { 1, 5, 9, 13 },
            };
            _styleCfgList.Add(cfg);
            #endregion
            #region 音符
            cfg = new BulletStyleCfg
            {
                styleId = 27,
                name = "音符",
                packName = "STGBulletsAtlas",
                resName = "Bullet127010_0",
                showScale = 1,
                availableColors = new List<int> { 1, 5, 9, 13 },
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
        /// 获取所有子弹的配置
        /// </summary>
        /// <returns></returns>
        public List<BulletStyleCfg> GetBulletStyleCfgs()
        {
            return _styleCfgList;
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
        public BulletStyleCfg GetBulletStyleCfgByStyleId(int styleId)
        {
            try
            {
                return _styleCfgList[styleId];
            }
            catch
            {
                return new BulletStyleCfg
                {
                    styleId = -1,
                    availableColors = { },
                };
            }
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
