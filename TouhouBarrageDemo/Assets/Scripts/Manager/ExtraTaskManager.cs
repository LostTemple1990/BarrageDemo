using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraTaskManager : ITaskExecuter
{
    private static ExtraTaskManager _instance;

    public static ExtraTaskManager GetInstance()
    {
        if ( _instance == null )
        {
            _instance = new ExtraTaskManager();
        }
        return _instance;
    }

    private List<Task> _addTaskList;
    private int _addCount;
    private List<Task> _taskList;
    private int _taskCount;
    private int _clearTime;

    public void Init()
    {
        _taskList = new List<Task>();
        _addTaskList = new List<Task>();
        _taskCount = 0;
        _clearTime = 0;
        _addCount = 0;
    }

    public void Update()
    {
        UpdateTasks();
        CheckClearTaskList();
        if ( _addCount > 0 )
        {
            _taskList.AddRange(_addTaskList);
            _taskCount += _addCount;
            _addTaskList.Clear();
            _addCount = 0;
        }
    }

    private void CheckClearTaskList()
    {
        _clearTime++;
        if (_clearTime >= 600)
        {
            int i, j;
            Task task;
            for (i = 0, j = 1; i < _taskCount; i++)
            {
                task = _taskList[i];
                if (task == null)
                {
                    j = j == 1 ? i + 1 : j;
                    int findFlag = 0;
                    for (; j < _taskCount; j++)
                    {
                        if (_taskList[j] != null)
                        {
                            findFlag = 1;
                            _taskList[i] = _taskList[j];
                            _taskList[j] = null;
                            j++;
                            break;
                        }
                    }
                    if (findFlag == 0)
                    {
                        break;
                    }
                }
            }
            _taskList.RemoveRange(i, _taskCount - i);
            _taskCount = i;
            _clearTime = 0;
        }
    }

    public void AddTask(Task task)
    {
        _addTaskList.Add(task);
        _addCount++;
    }

    private void UpdateTasks()
    {
        int i;
        Task task;
        for (i=0;i<_taskCount;i++)
        {
            task = _taskList[i];
            if ( task != null )
            {
                task.Update();
                if (task.isFinish)
                {
                    ObjectsPool.GetInstance().RestorePoolClassToPool<Task>(task);
                    _taskList[i] = null;
                }
            }
        }
    }

    public void Clear()
    {
        ClearTasks();
    }

    private void ClearTasks()
    {
        int i;
        Task task;
        for (i = 0; i < _taskCount; i++)
        {
            task = _taskList[i];
            if (task != null)
            {
                if (!task.isFinish)
                {
                    InterpreterManager.GetInstance().StopTaskThread(task);
                    ObjectsPool.GetInstance().RestorePoolClassToPool<Task>(task);
                }
            }
        }
        _taskList.Clear();
        _taskCount = 0;
        _clearTime = 0;
    }
}
