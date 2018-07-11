using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter
{
    public void Update()
    {

    }
}
//if ( frameNode%2 == 0 && frameNode >= 300 )
//{
//    EnemyBulletStraight bullet;
//    //int a = Random.Range(-120, 120);
//    //int b = Random.Range(120, 180);
//    int a = Random.Range(0, 10);
//    string texName = "etama_172";
//    Vector3 pos = new Vector3(0, 180, 0);
//    for (int i = 0; i < 36; i++)
//    {
//        bullet = BulletPool.GetInstance().CreateBullet(BulletId.BulletId_Enemy_Straight) as EnemyBulletStraight;
//        bullet.SetBulletTexture(texName);
//        bullet.SetRotatedByVelocity(true);
//        bullet.SetGrazeParams(8f, 8f);
//        bullet.SetCollisionParams(Consts.CollisionType_Circle, 4f, 4f);
//        //bullet.SetReboundParams(7, 1);
//        bullet.SetToPosition(pos);
//        //int tmp = (frameNode - 300) / 10;
//        //bullet.DoMove(new Vector2(0, 120), 150 - tmp * 15, i * 28 + tmp * 5, 30, 60);
//        bullet.DoMove(90, i * 10 + Random.Range(0, 10), 0, 0);
//    }
//}

//    if (frameNode >= 300 && frameNode % 120 == 0 )
//        {
//            EnemyBulletStraight bullet;
////int a = Random.Range(-120, 120);
////int b = Random.Range(120, 180);
//int a = Random.Range(0, 10);
//string texName = "etama_172";
//Vector3 pos = new Vector3(0, 180, 0);
//            for (int i = 0; i< 36; i++)
//            {
//                bullet = BulletPool.GetInstance().CreateBullet(BulletId.BulletId_Enemy_Straight) as EnemyBulletStraight;
//                bullet.SetBulletTexture(texName);
//                bullet.SetRotatedByVelocity(true);
//                bullet.SetGrazeParams(8f, 8f);
//                bullet.SetCollisionParams(Consts.CollisionType_Circle, 4f, 4f);
//                //bullet.SetReboundParams(7, 1);
//                bullet.SetToPosition(pos);
//                //int tmp = (frameNode - 300) / 10;
//                //bullet.DoMove(new Vector2(0, 120), 150 - tmp * 15, i * 28 + tmp * 5, 30, 60);
//                bullet.DoMove(90, i* 10 + Random.Range(0, 10), 0, 0);
//            }
//        }
//        EnemyLaser laser0, laser1;
//        if (frameNode == 300 )
//        {
//            laser0 = BulletPool.GetInstance().CreateBullet(BulletId.BulletId_Enemy_Laser) as EnemyLaser;
//            laser0.SetBulletTexture("etama_236");
//            laser0.SetLaserSize(10, 200);
//            laser0.SetToPosition(new Vector3(180, 180, 0));
//            laser0.DoRotate(-80, 200);
//            laser0.DoMove(new Vector3(-60, 180, 0), 180);
//            laser0.SetCollisionParams(10, 200);

//            laser1 = BulletPool.GetInstance().CreateBullet(BulletId.BulletId_Enemy_Laser) as EnemyLaser;
//            laser1.SetBulletTexture("etama_236");
//            laser1.SetLaserSize(10, 200);
//            laser1.SetToPosition(new Vector3(120, 180, 180));
//            laser1.SetCollisionParams(10, 200);
//            laser1.DoRotate(260, 200);
//            laser1.DoMove(new Vector3(-120, 180, 0), 180);
//        }