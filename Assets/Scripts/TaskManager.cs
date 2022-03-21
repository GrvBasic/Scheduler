using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    #region Instance Setter

    public static TaskManager Instance { get; set; }
    

    private void Awake()
    {
        Instance = this;
    }


    #endregion

    private List< IEnumerator> executingTask = new List<IEnumerator>();
    public void CheckAndStartTask(IEnumerator taskExecutor)
    {
        if (executingTask.Count >0)
        {
            ForceStopCurrentTask();
        }
        executingTask.Add(taskExecutor);
        StartNewTask();
    }

    public void SetTaskCompleted()
    {
        executingTask.Clear();
    }

    void StartNewTask()
    {
        StartCoroutine(executingTask[0]);
    }
    void ForceStopCurrentTask()
    {
        Debug.Log("Force Stopping current ongoing task");
        StopCoroutine(executingTask[0]);
        executingTask.Remove(executingTask[0]);
    }


}