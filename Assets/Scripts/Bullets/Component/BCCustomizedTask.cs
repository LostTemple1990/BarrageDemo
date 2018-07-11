﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BCCustomizedTask : BulletComponent
{
    private EnemyBulletMovable _bullet;
    private List<Task> _taskList;
    private int _taskCount;

    public override void Init(EnemyBulletBase bullet)
    {
        _bullet = bullet as EnemyBulletMovable;
        if ( _taskList == null )
        {
            _taskList = new List<Task>();
        }
        _taskCount = 0;
    }

    public void AddTask(Task task)
    {
        _taskList.Add(task);
        _taskCount++;
    }

    public override void Update()
    {
        int i;
        Task task;
        for (i=0;i<_taskCount;i++)
        {
            task = _taskList[i];
            if ( task != null )
            {
                task.Update();
                if ( task.isFinish )
                {
                    ObjectsPool.GetInstance().RestoreDataClassToPool<Task>(task);
                    _taskList[i] = null;
                }
            }
        }
    }

    public override void Clear()
    {
        Task task;
        for (int i = 0; i < _taskCount; i++)
        {
            task = _taskList[i];
            if (task != null)
            {
                if (task.luaState != null)
                {
                    InterpreterManager.GetInstance().StopTaskThread(task.luaState, task.funcRef);
                }
                ObjectsPool.GetInstance().RestoreDataClassToPool<Task>(task);
            }
        }
        _taskList.Clear();
        _taskCount = 0;
    }
}
