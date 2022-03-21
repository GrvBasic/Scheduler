## Task Scheduler plugin
Task Scheduler is meant to schedule all the tasks in it and when the time comes
for that specific task it will be executed. For the scheduling process we use IEnumerator to hold in queue.
So helper class must have some IEnumerator method to do the work.

Features:
- Keeps record of all ongoing and total tasks.
- Can run tasks even when scene switches.
- Easily modify number of tasks to run simultaneously.
- Easily expandable for any type of work that needs scheduling.
- Tasks will run in background if not explicitly cancelled when changing scene or any other conditions.
- Can pause ongoing task with the option to remove from Queue or resume later



All the works that needs scheduling should call their helper class for task scheduling.

#What is a helper class?
Its an specific task based class to ease with task scheduling and execute it.
for eg: We have
- ImageDownloader.cs to download images through queue.
- PopupScheduler.cs to show dialogboxes like login page turn by turn.
- If the requirements are not met, we create another helper class to work efficiently using TaskScheduler

#How to use
- TaskScheduler.cs class shouldnot be used directly for scheduling task.
- We create a helper class for that and works will be executed through it. For eg: ImageDownloader
- If the task is added to queue donot forget to call OnTaskComplete() or CancelThisTask() once the task is started.
- If maximum tasks is not set it will be set to default value 1. We can configure that by  scheduler.ChangeMaximumTaskSimultaneously(3);
- If all current ongoing task needs to be stopped and removed, use CancelAllOngoingTaskAndRemove();
- If all current ongoing tasks need to be stopped and continued later, use PauseAllOnGoingTasks();
- Before adding new task to queue maximumNoOftask can be defined as per need.

#Important notes
- Task in queue will only be executed, if old task will be updated as cancelled or completed to the scheduler. It is because scheduler is restricted to the limited no. of tasks at a time.
- All task needs a identifier, any string value would be enough and pass that same value to add new task or update, on cancel or complete.
- TaskScheduler doesnt allow same task identifier to be added to queue unless the first one is cancelled or completed.
- Independent task will be executed first,than dependentTask.

#Problems that may arise:
- Task in queue may not execute. 

   Scheduler itself doesnt check for tasks in queue after a time duration. When task is added to queue,task completed or task cancelled, only in that case scheduler checks for the conditions to start new task.
- If PauseAllOnGoingTasks() is used to pause all ongoing task. Now all other task in queue wont be started even if new task will be added.


#Usable Functions

### AddNewTask( newTask,taskTypeReceived,taskName)
- This function adds tasks to queue.
- taskName is the reference of tasks and will be used to check their status. It should be unique to each task. 

###SetTaskCompleted(taskName)
-This function is to update task status so that new task from queue would be started.

###CancelThisTask(taskName)
- If task has some problem and couldnt finish. That task should be updated as cancelled for new task to be started.

### ChangeMaximumTaskSimultaneously(int count)
- Changes number of tasks to start simultaneously.

###CancelAllOngoingTaskAndRemove()
- Stops all current tasks and removes them from records and cannot be resumed later.

###PauseAllOnGoingTasks()
-Stops all current tasks but keeps its records and can be resumed later.

###ResumePausedTask()
- Restarts paused task,if there is any.

###SetAutoResumePausedTask()
- This function is called only once to set "_autoResume" to true. 
- Then the scheduler will check for paused tasks when tasks in queue is completed.

###KickStartScheduler()
- This function checks for tasks in queue and if scheduler is not operating in max limit. It will run tasks in maximum limit.


##Note 
-When CancelAllOngoingTaskAndRemove() or PauseAllOnGoingTasks() is used. Scheduler wont start new task itself. So either we ResumePausedTask() or we KickStartScheduler()
to start new task.











