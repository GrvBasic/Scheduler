using System;
using System.Collections;
using System.Collections.Generic;
using DownloadPlugin;
using UnityEngine;
using Random = UnityEngine.Random;

public class PopupScheduler : MonoBehaviour
{
    private static TaskScheduler scheduler;

    private static TaskScheduler CheckScheduler()
    {
        if (scheduler == null)
        {
            GameObject schedulerGameObject = new GameObject("Popup Scheduler");
            scheduler = schedulerGameObject.AddComponent<TaskScheduler>();
            GameObject.DontDestroyOnLoad(schedulerGameObject);
        }
        scheduler.ChangeMaximumTaskSimultaneously(1);
        return scheduler;
    }

    public static void AddTask(GameObject gt)
    {
        CheckScheduler();
        scheduler.AddNewTask(ShowPopup(gt),TaskType.HighPriority,gt.name);
    }

   static IEnumerator ShowPopup(GameObject popUp)
    {
        yield return null;
       popUp.SetActive(true);
    }

    public static void SetTaskCompleted(string popUp)
    {
        scheduler.SetTaskCompleted(popUp);
        
    }

    public static void CancelAllTask()
    {
        scheduler.CancelAllOngoingTaskAndRemove();
    }
}
