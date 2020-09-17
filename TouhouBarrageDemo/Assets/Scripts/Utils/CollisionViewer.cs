#define RenderEnemyCollision
#define RenderEnemyBulletCollision
#define RenderPlayerBulletCollision
#define RenderPlayerCollision
#define RenderColliderCollision

using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

public class CollisionViewer
{
    private static CollisionViewer _instance = new CollisionViewer();

    public static CollisionViewer Instance
    {
        get
        {
            return _instance;
        }
    }

    private GameObject _viewerGo;
    private Mesh _mesh;
    private List<Vector3> _verList;
    private List<Vector2> _uvList;
    private List<int> _triList;
    private List<Color> _colorList;
    private int _verStartIndex;

    private CollisionViewer()
    {

    }

    public void Init()
    {
        _viewerGo = ResourceManager.GetInstance().GetPrefab("Prefab/Extra", "CollisionViewer");
        _mesh = _viewerGo.GetComponent<MeshFilter>().mesh;
        UIManager.GetInstance().AddGoToLayer(_viewerGo, LayerId.STGTopEffect);
        _verList = new List<Vector3>();
        _uvList = new List<Vector2>();
        _triList = new List<int>();
        _colorList = new List<Color>();
        _verStartIndex = 0;
    }

    public void Render()
    {
        _mesh.Clear();
        _verList.Clear();
        _uvList.Clear();
        _triList.Clear();
        _colorList.Clear();
        _verStartIndex = 0;
        RenderPlayer();
        RenderPlayerBullet();
        RenderEnemy();
        RenderEnemyBullet();
        RenderCollider();
        _mesh.vertices = _verList.ToArray();
        _mesh.uv = _uvList.ToArray();
        _mesh.triangles = _triList.ToArray();
        _mesh.colors = _colorList.ToArray();
    }

    [Conditional("RenderPlayerCollision")]
    private void RenderPlayer()
    {
        CharacterBase player = PlayerInterface.GetInstance().GetCharacter();
        Color col = new Color(50f / 255, 255f / 255, 50f / 255);
        RenderCircle(player.GetPosition(), player.collisionRadius, col);
    }

    [Conditional("RenderPlayerBulletCollision")]
    private void RenderPlayerBullet()
    {
        Color col = new Color(127f / 255, 127f / 255, 192f / 255);
        List<PlayerBulletBase> list = BulletsManager.GetInstance().GetPlayerBulletList();
        //int count = list.Count;
        foreach (var bullet in list)
        {
            if (bullet != null && bullet.DetectCollision())
            {
                int nextIndex = 0;
                do
                {
                    CollisionDetectParas para = bullet.GetCollisionDetectParas(nextIndex);
                    RenderCollisionGraphic(para, col);
                    nextIndex = para.nextIndex;
                } while (nextIndex != -1);
            }
        }
    }

    [Conditional("RenderEnemyCollision")]
    private void RenderEnemy()
    {
        Color col = new Color(255f / 255, 255f / 255, 128f / 255);
        List<EnemyBase> list = EnemyManager.GetInstance().GetEnemyList();
        //int count = list.Count;
        foreach (var enemy in list)
        {
            if (enemy != null && enemy.DetectCollision())
            {
                CollisionDetectParas para = enemy.GetCollisionDetectParas();
                RenderCollisionGraphic(para, col);
            }
        }
    }

    [Conditional("RenderEnemyBulletCollision")]
    private void RenderEnemyBullet()
    {
        Color col = new Color(255f / 255, 50f / 255, 50f / 255);
        List<EnemyBulletBase> list = BulletsManager.GetInstance().GetEnemyBulletList();
        //int count = list.Count;
        foreach (var bullet in list)
        {
            if (bullet != null && bullet.DetectCollision())
            {
                int nextIndex = 0;
                do
                {
                    CollisionDetectParas para = bullet.GetCollisionDetectParas(nextIndex);
                    RenderCollisionGraphic(para, col);
                    nextIndex = para.nextIndex;
                } while (nextIndex != -1);
            }
        }
    }

    [Conditional("RenderColliderCollision")]
    private void RenderCollider()
    {
        Color col = new Color(255f / 255, 50f / 255, 255f / 255);
        int count;
        List<ObjectColliderBase> list = ColliderManager.GetInstance().GetColliderList(out count);
        //int count = list.Count;
        foreach (var collider in list)
        {
            if (collider != null)
            {
                RenderCollisionGraphic(collider.GetCollisionDetectParas(), col);
            }
        }
    }

    private void RenderCollisionGraphic(CollisionDetectParas para,Color col)
    {
        if (para.type == CollisionDetectType.Circle)
        {
            RenderCircle(para.centerPos, para.radius, col);
        }
        else if (para.type == CollisionDetectType.Rect)
        {
            RenderAABB(para.centerPos, para.halfWidth, para.halfHeight, col);
        }
        else if (para.type == CollisionDetectType.ItalicRect)
        {
            RenderOBB(para.centerPos, para.halfWidth, para.halfHeight, para.angle, col);
        }
    }

    public void RenderCircle(Vector2 centerPos,float radius,Color col)
    {
        _verList.Add(new Vector3(centerPos.x - radius, centerPos.y + radius));
        _verList.Add(new Vector3(centerPos.x - radius, centerPos.y - radius));
        _verList.Add(new Vector3(centerPos.x + radius, centerPos.y + radius));
        _verList.Add(new Vector3(centerPos.x + radius, centerPos.y - radius));
        _uvList.Add(new Vector2(0, 1));
        _uvList.Add(new Vector2(0, 0));
        _uvList.Add(new Vector2(0.5f, 1));
        _uvList.Add(new Vector2(0.5f, 0));
        _triList.Add(_verStartIndex);
        _triList.Add(_verStartIndex + 3);
        _triList.Add(_verStartIndex + 1);
        _triList.Add(_verStartIndex);
        _triList.Add(_verStartIndex + 2);
        _triList.Add(_verStartIndex + 3);
        for (int i = 0; i < 4; i++)
        {
            _colorList.Add(col);
        }
        _verStartIndex += 4;
    }

    public void RenderAABB(Vector2 centerPos,float halfWidth,float halfHeight,Color col)
    {
        _verList.Add(new Vector3(centerPos.x - halfWidth, centerPos.y + halfHeight));
        _verList.Add(new Vector3(centerPos.x - halfWidth, centerPos.y - halfHeight));
        _verList.Add(new Vector3(centerPos.x + halfWidth, centerPos.y + halfHeight));
        _verList.Add(new Vector3(centerPos.x + halfWidth, centerPos.y - halfHeight));
        _uvList.Add(new Vector2(0.75f, 1));
        _uvList.Add(new Vector2(0.75f, 0));
        _uvList.Add(new Vector2(1, 1));
        _uvList.Add(new Vector2(1, 0));
        _triList.Add(_verStartIndex);
        _triList.Add(_verStartIndex + 3);
        _triList.Add(_verStartIndex + 1);
        _triList.Add(_verStartIndex);
        _triList.Add(_verStartIndex + 2);
        _triList.Add(_verStartIndex + 3);
        for (int i = 0; i < 4; i++)
        {
            _colorList.Add(col);
        }
        _verStartIndex += 4;
    }

    public void RenderOBB(Vector2 centerPos,float halfWidth,float halfHeight,float rot,Color col)
    {
        Vector2 vec0 = MathUtil.GetVec2AfterRotate(-halfWidth, halfHeight, 0, 0, rot);
        Vector2 vec1 = MathUtil.GetVec2AfterRotate(-halfWidth, -halfHeight, 0, 0, rot);
        _verList.Add(new Vector3(centerPos.x + vec0.x, centerPos.y + vec0.y));
        _verList.Add(new Vector3(centerPos.x + vec1.x, centerPos.y + vec1.y));
        _verList.Add(new Vector3(centerPos.x - vec1.x, centerPos.y - vec1.y));
        _verList.Add(new Vector3(centerPos.x - vec0.x, centerPos.y - vec0.y));
        _uvList.Add(new Vector2(0.75f, 1));
        _uvList.Add(new Vector2(0.75f, 0));
        _uvList.Add(new Vector2(1, 1));
        _uvList.Add(new Vector2(1, 0));
        _triList.Add(_verStartIndex);
        _triList.Add(_verStartIndex + 3);
        _triList.Add(_verStartIndex + 1);
        _triList.Add(_verStartIndex);
        _triList.Add(_verStartIndex + 2);
        _triList.Add(_verStartIndex + 3);
        for (int i = 0; i < 4; i++)
        {
            _colorList.Add(col);
        }
        _verStartIndex += 4;
    }

    public void Clear()
    {
        //GameObject.Destroy(_viewerGo);
        //_viewerGo = null;
        //_mesh = null;
        _mesh.Clear();
        _verList.Clear();
        _uvList.Clear();
        _triList.Clear();
        _colorList.Clear();
    }
}
