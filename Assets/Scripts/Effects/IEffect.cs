using UnityEngine;
using System.Collections;

public interface IEffect
{
    void Init();
    void Update();
    void SetToPos(float posX,float posY);
    void Clear();
    bool IsFinish();
    void FinishEffect();
}
