using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarrageEditor
{
    public class DefineDatabase
    {

    }

    public class DefineData
    {
        public DefineType type;
        public string typeName;
        public string paraListStr;
    }
    
    public enum DefineType
    {
        SimpleBullet = 1,
        Laser = 2,
        LinearLaser = 3,
        CurveLaser = 4,
        Enemy = 5,
        Boss = 6,
    }
}
