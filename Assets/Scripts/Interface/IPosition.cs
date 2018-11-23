using UnityEngine;
using System.Collections;

public interface IPosition
{
    void SetToPosition(float x, float y);
    void SetToPosition(Vector2 pos);
    Vector2 GetPosition();
    void SetRotation(float rotation);
    float GetRotation();
}
