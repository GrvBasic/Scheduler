using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace DownloadPlugin
{
    public class ImageDownloader : Singleton<ImageDownloader>
    {
        private static TaskScheduler _scheduler;

        private static TaskScheduler CheckScheduler()
        {
            //Check the refernce of TaskScheduler.cs class
            if (_scheduler == null)
            {
                
                //Create new gameObject with TaskScheduler.cs attached to it.
                GameObject schedulerGameObject = new GameObject("Download Scheduler");
                _scheduler = schedulerGameObject.AddComponent<TaskScheduler>();
                DontDestroyOnLoad(schedulerGameObject);
            }

            return _scheduler;
        }


        public static void ChangeMaximumTask(int count)
        {
            CheckScheduler();
            _scheduler.ChangeMaximumTaskSimultaneously(count);
        }

        //Do not forget to load image from file first. Otherwise for the file not found image will always be downloaded.
        //For the requested sprites, it is first cheked in MemoryCache and if not not found downloading task will be queued.
        private static Dictionary<string, CallbackEvents> duplicateTask = new Dictionary<string, CallbackEvents>();

        private class CallbackEvents
        {
            public UnityEvent<Sprite> eventOnSuccess = new UnityEvent<Sprite>();
            public UnityEvent<string> eventOnFailed = new UnityEvent<string>();
        }


        public static void GetImage(string url, string imageName, Action<Sprite> onSpriteReady,
            Action<string> onDownloadFailed)
        {
            //Add new task to queue

            //Check the reference of taskScheduler
            CheckScheduler();

            string taskName = imageName;


            if (_scheduler.IsTaskInQueue(taskName))
            {
                //If the same task is already in queue. We add new task to duplicateTask's list
                if (!duplicateTask.ContainsKey(taskName))
                {
                    duplicateTask.Add(taskName, new CallbackEvents());
                }

                CallbackEvents callback;
                // We dont add duplicate task to queue. Instead we subscribe to alraeady added task's actions.
                if (duplicateTask.TryGetValue(taskName, out callback))
                {
                    callback.eventOnSuccess.AddListener(sprite => onSpriteReady?.Invoke(sprite));
                    callback.eventOnFailed.AddListener(message => onDownloadFailed?.Invoke(message));
                }

                return;
            }

            CheckImageInDisk(url, imageName, onSpriteReady, onDownloadFailed, taskName);
        }

        static void CheckImageInDisk(string url, string imageName, Action<Sprite> onSpriteReady,
            Action<string> onDownloadFailed, string taskName)
        {
            //Sprite to return 
            Sprite sp = MemoryCache.GetCachedImage(imageName);
            if (sp != null)
            {
                //Return sprite if found in memory cache.
                //But first sprites need to be loaded in spriteCache of MemoryCache.cs 

                InvokeSuccessAction(onSpriteReady, sp, taskName);
                return;
            }

            //When image not found. Add the task to download that image
            _scheduler.AddNewTask(DownloadImage(url, imageName, onSpriteReady, onDownloadFailed),
                TaskType.MediumPriority, imageName);
            _scheduler.AddNewTask(NewFUnc(),TaskType.HighPriority,"Nerw task");
        }

       static IEnumerator NewFUnc()
        {
            yield return null;
        }
        static IEnumerator DownloadImage(string url, string imageName, Action<Sprite> onSpriteReady,
            Action<string> onDownloadFailed)
        {
            //Working Function to download image
            UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
            DownloadHandlerTexture downloadHandlerTexture = new DownloadHandlerTexture(true);

            float downloadSize = GetDownloadFileSize(url);
            if (downloadSize <= 0)
            {
                //For downloadable file size is null we cancel the task
                InvokeFailedAction(onDownloadFailed, "Connection Problem or Protocol Error", imageName);
                yield break;
            }


            Debug.Log(downloadSize + " bytes");
            webRequest.downloadHandler = downloadHandlerTexture;
            webRequest.SendWebRequest();
            Debug.Log(webRequest.downloadProgress);

            float newProgress = 0.1f, oldProgress = 0f;
            while (!webRequest.isDone)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    InvokeFailedAction(onDownloadFailed, "No Internet", imageName);
                    yield break;
                }

                //Waiting to see download progress.
                yield return null;
                Debug.Log(webRequest.downloadProgress);
            }

            Texture image;

            try
            {
                image = DownloadHandlerTexture.GetContent(webRequest);
                webRequest.Dispose();
                _scheduler.SetTaskCompleted(imageName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log(url);

                InvokeFailedAction(onDownloadFailed, e.Message, imageName);

                throw;
            }

            Sprite sp = Sprite.Create((Texture2D) image,
                new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
            AddFileToDisk(imageName, sp);

            InvokeSuccessAction(onSpriteReady, sp, imageName);
        }

        static void AddFileToDisk(string spriteNameR, Sprite spriteR)
        {
            CheckScheduler();

            MemoryCache.CacheImage(spriteNameR, spriteR, true);
        }

        static void InvokeSuccessAction(Action<Sprite> onTaskSuccess, Sprite sp, string taskName)
        {
            onTaskSuccess?.Invoke(sp);

            if (duplicateTask.ContainsKey(taskName))
            {
                duplicateTask[taskName].eventOnSuccess?.Invoke(sp);
                duplicateTask.Remove(taskName);
            }
        }

        static void InvokeFailedAction(Action<string> onFailedTask, string failedMessage, string taskName)
        {
            _scheduler.CancelThisTask(taskName);
            onFailedTask?.Invoke(failedMessage);
            if (duplicateTask.ContainsKey(taskName))
            {
                duplicateTask[taskName].eventOnFailed?.Invoke(failedMessage);
                duplicateTask.Remove(taskName);
            }
        }

        public static void CancelAllOngoingTask()
        {
            CheckScheduler();
            _scheduler.CancelAllOngoingTaskAndRemove();
        }

        public static void PauseAllOngoingTask()
        {
            //Pause tasks scheduler and keep their reference to resume in future.
            CheckScheduler();
            _scheduler.PauseAllOnGoingTasks();
        }

        public static void ResumePausedTask()
        {
            //Resume all tasks that were all previously paused.
            CheckScheduler();
            _scheduler.ResumePausedTask();
        }

        public static void ResumeDownloadTask()
        {
            //Start executing other tasks in queue.
            CheckScheduler();
            _scheduler.KickStartScheduler();
        }

        private static float GetDownloadFileSize(string url)
        {
            UnityWebRequest request = UnityWebRequest.Head(url);
            request.SendWebRequest();
            while (request.result == UnityWebRequest.Result.InProgress)
            {
                //Just wait
            }

            if ((request.result != UnityWebRequest.Result.Success))
                return 0;


            string downloadBytes = request.GetResponseHeader("Content-Length");
            request.Dispose();
            if (downloadBytes == null)
                return 0;
            float downloadSize = float.Parse(downloadBytes);


            return downloadSize;
        }

        public static void AutoResumeIfNoTaskFound()
        {
            //If set to autoresume when no task is found. Those paused task will resume.
            CheckScheduler();
            _scheduler.SetAutoResumePausedTask();
        }
    }
}