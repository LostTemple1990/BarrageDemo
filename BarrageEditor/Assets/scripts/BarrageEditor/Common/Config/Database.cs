using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarrageEditor
{
    public sealed class DatabaseManager
    {
        private static BulletDatabase bulletDatabase;
        private static NodeDatabase nodeDatabase;
        private static LaserDatabase laserDatabase;
        private static EnemyDatabase enemyDatabase;

        public static BulletDatabase BulletDatabase
        {
            get
            {
                if (bulletDatabase == null)
                {
                    bulletDatabase = new BulletDatabase();
                }
                return bulletDatabase;
            }
        }

        public static LaserDatabase LaserDatabase
        {
            get
            {
                if (laserDatabase == null)
                {
                    laserDatabase = new LaserDatabase();
                }
                return laserDatabase;
            }
        }

        public static EnemyDatabase EnemyDatabase
        {
            get
            {
                if (enemyDatabase == null)
                {
                    enemyDatabase = new EnemyDatabase();
                }
                return enemyDatabase;
            }
        }

        public static NodeDatabase NodeDatabase
        {
            get
            {
                if (nodeDatabase == null)
                {
                    nodeDatabase = new NodeDatabase();
                }
                return nodeDatabase;
            }
        }
    }

    public enum BulletType : byte
    {
        Undefined = 0,
        Bullet = 1,
        Laser = 2,
        LinearLaser = 3,
        CurveLaser = 4,
    }
}
