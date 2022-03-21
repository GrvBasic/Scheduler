#PopUp Scheduler

Lets Suppose we want to show dialog boxes like loginPage, dailyRewards, MiniGame etc...
To help with that process we use PopupScheduler.cs .It takes the list of gameobjects to be enabled, keeps them in queue and displays on by one.

Features:
Uses task scheduler to show dialogbox one by one.

##How To use

- ###Add New Task <br>
We just call PopUpScheduler.AddTask(GameObject gt) and the task will be added to queue.

- ###Update Task Info
Once the DialogBox is displayed do not forget to call SetTaskCompleted(taskName). Most Probably call this method in DialogBox CloseButton operation.

#Note
- In case of adding task, the parameter gameobject, its name will be used to add task in queue.
Now when updating SetTaskCompleted(). We provide same gameObjects name.
Since PopupScheduler uses taskScheduler we need to provide valid name when adding task and updating task info.
- Provided gameobjects will be enabled and disabled but not instantiated.
- if any animation effects are to be added when closing or opening dialog 






