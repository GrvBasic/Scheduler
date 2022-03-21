using System;
using DownloadPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ImageDownloadTester : MonoBehaviour
{
    // Start is called before the first frame update
    public string url =
        "https://lh3.googleusercontent.com/JzmU1TeIFp36JqAvp7reOWrK-8DjDd1pWQ8Pz31hMPooOUKYBk4QzkBbQ6FVxiT07TU";

    public Image image;
    void StartDownload()
    {
        ImageDownloader.ChangeMaximumTask(1);
        ImageDownloader.GetImage(url,SceneManager.GetActiveScene().name,OnDownloadComplete,OnFailed);
        ImageDownloader.GetImage("https://www.yarsagames.com/wp-content/uploads/2019/05/Driving-School-2019-Icon.jpg","2",OnDownloadComplete,OnFailed);
        ImageDownloader.GetImage("https://www.yarsagames.com/wp-content/uploads/2019/02/ludo-board-04.png","3",OnDownloadComplete,OnFailed);
        ImageDownloader.GetImage("https://www.yarsagames.com/wp-content/uploads/2019/10/Snake-and-ladder-DP-image.jpg","4",OnDownloadComplete,OnFailed);
        ImageDownloader.GetImage("https://www.yarsagames.com/wp-content/uploads/2019/10/Marriage-DP-image-1.jpg","5",OnDownloadComplete,OnFailed);
        ImageDownloader.GetImage("https://www.yarsagames.com/wp-content/uploads/2019/04/Rummy-feature-image-03-800x500.png","6",OnDownloadComplete,OnFailed);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene("SampleScene");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene("SampleScene 1");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ImageDownloader.PauseAllOngoingTask();
        }


        if (Input.GetKeyDown(KeyCode.D))
        {
            StartDownload();
        }
    }

    private int a = 0;
    
    void OnDownloadComplete(Sprite sprite)
    {
        // if (a == 3)
        // {
        //     ImageDownloader.PauseAllOngoingTask();
        // }
        image.sprite = sprite;
    }

    void OnFailed(string error)
    {
        image.color=Color.red;
        
    }

   
}
