using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BarrageEditor
{
    public sealed class DatabaseManager
    {
        private static BulletDatabase bulletDatabase;

        public static BulletDatabase BulletDatabase
        {
            get
            {
                if (bulletDatabase == null )
                {
                    bulletDatabase = new BulletDatabase();
                }
                return bulletDatabase;
            }
        }
    }
}
