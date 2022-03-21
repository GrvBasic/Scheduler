using System;
using System.Collections;
using System.Collections.Generic;
using DownloadPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{
    [SerializeField] private Image imageToUpdate;
    public  event Action<Sprite> actionOnImageLoad;
    public  event Action<string> actionOnImageFailedToGet;
    public List<string> imageUrls,imageNames;

    private string testurl = "https://www.kentonline.co.uk/_media/img/1200x0/3GJ0BBS20I1B6560XHAH.jpg";
    private string testImageName = "Myfruit";
    [SerializeField] private List<Sprite> foundImages;



    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {
            //Add new task
            //ImageDownloader.ChangeMaximumTask();
            ImageDownloader.ChangeMaximumTask(5);
            ImageDownloader.GetImage(testurl,testImageName,actionOnImageLoad,actionOnImageFailedToGet);
         
        }
    }

    void updateUi(Sprite sp)
    {
        foundImages.Add(sp);
        try
        {
            imageToUpdate.sprite = sp;
        }
        catch (Exception e)
        {
         Debug.Log("Failed to update ui");
 
        }
       
    }

    void FailedToGetImage(string eMessage)
    {
        Debug.Log(eMessage);
    }

    private void OnEnable()
    {
        actionOnImageLoad = updateUi;
        actionOnImageFailedToGet = FailedToGetImage;
    }

    private void OnDisable()
    {
        actionOnImageLoad -= updateUi;
        actionOnImageFailedToGet = FailedToGetImage;
    }

    public void StartTask()
    {
        Debug.Log(imageUrls.Count+" no of tasks");
        ImageDownloader.AutoResumeIfNoTaskFound();
        //Add new task
        for (int i = 0; i <imageUrls.Count; i++)
        {
            ImageDownloader.GetImage(imageUrls[i],imageNames[i],actionOnImageLoad,actionOnImageFailedToGet);
        }
    }

    public void PauseTask()
    {
        ImageDownloader.PauseAllOngoingTask();
    }

    public void ResumePausedTask()
    {
        ImageDownloader.ResumePausedTask();
    }

    public void ResumeDownload()
    {
        ImageDownloader.ResumeDownloadTask();
    }

    public void AddNewTask()
    {
        ImageDownloader.GetImage("https://pixy.org/src/481/4817715.jpg","Tree",actionOnImageLoad,actionOnImageFailedToGet);
    }
}
