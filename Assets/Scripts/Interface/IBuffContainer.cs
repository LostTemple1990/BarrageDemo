using UnityEngine;
using System.Collections;

public interface IBuffContainer
{
    IBuff GetBuffById(int id);
    void AddBuff(IBuff buff);
    void RemoveBuff(IBuff buff);
}
