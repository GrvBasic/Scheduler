#ImageDownloader



##How to download Image through scheduler.
GetImage(string url,string imageName,Action<Sprite> onSpriteReady,Action<string> onDownloadFailed)
- To Download a image we just call GetImage(...) by providing required parameters.
- url: Link where the image file is located.
- imageName: Name of the image it will be referenced as. This imageName will be used as taskName and fileName to save that image.
- onSpriteReady : Action to be invoked if valid sprite is found in memory or downloaded.
- onDownloadFailed : Action to be invoked if sprite not found in memory or download failed.

##How to cancel all ongoing download task and resume.
- If downloading task needs to be paused, we just call ImageDownloader.PauseAllOngoingTask()
- In order to resume all those stopped task, we call ImageDownloader.ResumeCancelledTask() and all the task previously stopped will be started again.
- ChangeMaximumTask(taskCounts) modifies no of tasks to run simultaneously. By default it is 1. 


#Usable functions:
###ChangeMaximumTask(int count)
- This function sets no of images to download simultaneously and should be called before adding task.

###GetImage(url, imageName,onSpriteReady,onDownloadFailed)
- imageName : This variable name is important as it will be used as a taskName and fileName to save downloaded image file.
- imageName will be used to check image available in memory.

###CancelAllOngoingTask()
- This function can be used to stop all current started task and they cannot be resumed later.

###PauseAllOngoingTask()
- This function can be used to stop all current started task but can be resumed later using ResumeTask().

###ResumePausedTask()
- This function can be used to restart all stopped task by PauseAllOngoingTask()

###ResumeDownloadTask()
- Once all current tasks are paused or cancelled.There may be other tasks in queue. It wont be started itself.
- So we use this function to continue other download task.

###AutoResumeIfNoTaskFound()
- This function sets to auto start paused task when all current task is completed in the list. If this feature is to be used call this function once in the beginning while adding first task.








