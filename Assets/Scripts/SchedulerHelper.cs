using System.Collections;
using System.Collections.Generic;
using DownloadPlugin;
using UnityEngine;

public abstract class SchedulerHelper : Singleton<SchedulerHelper>
{

    protected virtual TaskScheduler CheckScheduler(TaskScheduler scheduler,string gameObjectName)
    {
        if (scheduler == null)
        {
            GameObject schedulerGameObject = new GameObject(gameObjectName);
            scheduler = schedulerGameObject.AddComponent<TaskScheduler>();
            GameObject.DontDestroyOnLoad(schedulerGameObject);
        }
        scheduler.ChangeMaximumTaskSimultaneously(5);
        return scheduler;
    }

    public virtual TaskScheduler CheckSchedulerTest(TaskScheduler scheduler,string gameObjectName)
    {
        if (scheduler == null)
        {
            GameObject schedulerGameObject = new GameObject(gameObjectName);
            scheduler = schedulerGameObject.AddComponent<TaskScheduler>();
            GameObject.DontDestroyOnLoad(schedulerGameObject);
        }
        scheduler.ChangeMaximumTaskSimultaneously(5);
        return scheduler;
    }
}
