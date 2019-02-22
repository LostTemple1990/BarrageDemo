using UnityEngine;
using System.Collections;

public interface IPosition
{
    void SetPosition(float x, float y);
    void SetPosition(Vector2 pos);
    Vector2 GetPosition();
    void SetRotation(float rotation);
    float GetRotation();
}
