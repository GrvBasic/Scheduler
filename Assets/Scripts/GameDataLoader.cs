using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DownloadPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataLoader : MonoBehaviour
{
    public static event Action OnTaskComplete;
    private void Start()
   {
    //   string taskName = "ReadCacheImage";
      // DownloadScheduler.Instance.AddNewTask(ReadCachedImagFiles(taskName),TaskType.HighPriority,taskName);
    
    
   }

    private void Update()
    {
      
    }

    IEnumerator ReadCachedImagFiles(string taskNameR)
   {
      ReadWriteImages.ReadAllCachedImage();
      yield return null;
      // DownloadScheduler.Instance.SetTaskCompleted(taskNameR);
   }

   public void SwitchScene()
   {
     //  WorkScheduler.Instance.CancelAllOnGoingTasks();
       SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
   }
}
