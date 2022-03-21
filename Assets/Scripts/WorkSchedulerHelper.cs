using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DownloadPlugin;

public class WorkSchedulerHelper : Singleton<WorkSchedulerHelper>
{

    public static void AddNewWork(string taskName,TaskType taskType,Action workingFunction)
    {
        // DownloadScheduler.Instance.AddNewTask(StartWork(taskName,workingFunction),taskType,taskName);
    }

    static IEnumerator  StartWork(string  taskNameR,Action workingFunction)
    {
        yield return new WaitForSeconds(0.1f);
        workingFunction?.Invoke();
    }
}
