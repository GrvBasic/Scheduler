using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DownloadPlugin
{
    public class Scheduler : MonoBehaviour
    {
        #region Variables

        [SerializeField] private bool _isPreviousTaskCancellabe;
        [FormerlySerializedAs("startTask")] [SerializeField] private bool isTaskExecutable;
        private IEnumerator _currenTask;


        [SerializeField] private int totalTaskStarted, noOfTasksInqueue, totaltaskCompleted,pauseTasksInqueue, maximumTaskAllowed = 1;

        //List of tasks in queue to be executed
        private Dictionary<string, IEnumerator> totalTaskDictionary = new Dictionary<string, IEnumerator>();

        //List of ongoing tasks.
        private Dictionary<string, IEnumerator> curentTaskDictionary = new Dictionary<string, IEnumerator>();

        //High priority task is independent task. For eg: Memory Read
        private Queue<KeyValuePair<string, IEnumerator>> highPriorityTasks =
            new Queue<KeyValuePair<string, IEnumerator>>();

        //mediumPriorityTask is task that might be dependent of highPriority task.
        //For eg: Download file only if file not found.
        private Queue<KeyValuePair<string, IEnumerator>> mediumPrioritTask =
            new Queue<KeyValuePair<string, IEnumerator>>();

        //Tasks that may be dependent of both high Priority or MediumPriority task.
        private Queue<KeyValuePair<string, IEnumerator>> lowPriorityTask =
            new Queue<KeyValuePair<string, IEnumerator>>();

        [SerializeField] private List<string> completedTaskList = new List<string>();

        private Dictionary<string, IEnumerator> stoppedTask = new Dictionary<string, IEnumerator>();
        #endregion

        private List<TaskInfo> taskInformation = new List<TaskInfo>();
        void Start()
        {
            isTaskExecutable = true;
            CheckAndStartTask();
        }

        public bool IsTaskInQueue(string taskName)
        {
            return totalTaskDictionary.ContainsKey(taskName);
        }

        #region Adding And StartingTask

        public void AddNewTask(IEnumerator newTask, TaskType taskTypeReceived, string taskName)
        {
            
            //Add new task to queue.
            //This try block is to avoid adding same task to queue.
            //If there are needed multiple request for same task. Make a helper class to do so. 
            try
            {
                
                totalTaskDictionary.Add(taskName, newTask);
                noOfTasksInqueue++;
                taskInformation.Add(new TaskInfo(newTask,taskTypeReceived,taskName));
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                CheckAndStartTask();
                Debug.Log("Task already exists in queue");
                return;
            }


            if (taskTypeReceived == TaskType.HighPriority)
            {
                highPriorityTasks.Enqueue(new KeyValuePair<string, IEnumerator>(taskName, newTask));
            }
            else if (taskTypeReceived == TaskType.MediumPriority)
            {
                mediumPrioritTask.Enqueue(new KeyValuePair<string, IEnumerator>(taskName, newTask));
            }
            else if (taskTypeReceived == TaskType.LowPriority)
            {
                mediumPrioritTask.Enqueue(new KeyValuePair<string, IEnumerator>(taskName, newTask));
            }

      
            CheckAndStartTask();
        }
        void CheckAndStartTask()
        {
            if(!isTaskExecutable)
                return;
            //If currently running tasks count is greater than the provided limit.
            //We check for if previous task is cancellable or not
            if (curentTaskDictionary.Count >= maximumTaskAllowed)
            {
                //Cancel previous task and the control goes down to StartNewTask();
                if (_isPreviousTaskCancellabe)
                    CancelCurrentTask();
                else
                {
                    return;
                }
            }

            //Gets the current executable task ._currentTask will be assigned from GetNewTask()
            GetCurrentTaskNew();

            //If no task found simply return and dont execute anything.
            if (_currenTask == null)
                return;
            StartNewTask();
        }
        void StartNewTask()
        {
            //Starts new task. 
            StartCoroutine(_currenTask);
            totalTaskStarted++;
            Debug.Log($"Total Task started {totalTaskStarted}");
        }

        #endregion

        #region TaskEvents
        void CancelCurrentTask()
        {
            //Cancelling current task to perform new task
            if (_currenTask == null)
                return;
            StopCoroutine(_currenTask);
        }
        public void SetTaskCompleted(string taskNameR)
        {
            //For the tasks completed. We need to clear it from the list and start another task.
            //So we update informations regarding that task.
            if (!totalTaskDictionary.ContainsKey(taskNameR))
            {
                Debug.LogError($"{taskNameR} invalid task name .Make sure task name is correct ");
                return;
            }

            totalTaskDictionary.Remove(taskNameR);


            noOfTasksInqueue--;
            curentTaskDictionary.Remove(taskNameR);
            totaltaskCompleted++;
            Debug.Log($"Total Task completed {totaltaskCompleted}");
            completedTaskList.Add(taskNameR);
            if (completedTaskList.Count > 5)
            {
                completedTaskList.Remove(completedTaskList[0]);
            }

            CheckAndStartTask();
        }

        public void CancelAllOngoingTaskAndRemove()
        {
            isTaskExecutable = false;
            //Stop all current Ongoing tasks and remove their reference to resume in future.
            List<string> taskNameN = new List<string>();
            foreach (KeyValuePair<string, IEnumerator> currentTask in curentTaskDictionary)
            {
                StopCoroutine(curentTaskDictionary[currentTask.Key]);
                taskNameN.Add(currentTask.Key);
            }

            for (int i = 0; i < curentTaskDictionary.Count; i++)
            {
                //Clear task from ongoing tasks list. 
                curentTaskDictionary.Remove(taskNameN[0]);
                //Clear task from total tasks list.
                totalTaskDictionary.Remove(taskNameN[0]);
                noOfTasksInqueue--;
            }
        }
        public void PauseAllOnGoingTasks()
        {
            //Stop all current Ongoing tasks and keep their reference to resume in future.
            isTaskExecutable = false;
            //Cancels all the executing task and donot remove referrence of those stopped tasks.

            foreach (KeyValuePair<string, IEnumerator> currentTask in curentTaskDictionary)
            {
                StopCoroutine(curentTaskDictionary[currentTask.Key]);
                stoppedTask.Add(currentTask.Key,currentTask.Value);
       
            }
            curentTaskDictionary.Clear();
          
            pauseTasksInqueue = stoppedTask.Count;
        }
        public void CancelThisTask(string nameOfTaskR)
        {
            //If task is started to execute. Either failed or completed that task.
            //That task needs to be cleared from queue.
            StopCoroutine(curentTaskDictionary[nameOfTaskR]);
            curentTaskDictionary.Remove(nameOfTaskR);
            totalTaskDictionary.Remove(nameOfTaskR);
            noOfTasksInqueue--;
        }
        #endregion
       
        
        public void GetCurrentTaskNew()
        {
            //Gets new task from queue to be executed.
            KeyValuePair<string, IEnumerator> newTask = new KeyValuePair<string, IEnumerator>();
            _currenTask = null;

            if (highPriorityTasks.Count > 0)
            {
                newTask = highPriorityTasks.Dequeue();
                curentTaskDictionary.Add(newTask.Key, newTask.Value);
                _currenTask = newTask.Value;
            }
            else if (mediumPrioritTask.Count > 0)
            {
                newTask = mediumPrioritTask.Dequeue();
                curentTaskDictionary.Add(newTask.Key, newTask.Value);
                _currenTask = newTask.Value;
            }
            else if (lowPriorityTask.Count > 0)
            {
                newTask = mediumPrioritTask.Dequeue();
                curentTaskDictionary.Add(newTask.Key, newTask.Value);
                _currenTask = newTask.Value;
            }
        }
        public void ChangeMaximumTaskSimultaneously(int count)
        {
            //Change number of minimum task to perform simultaneously.
            maximumTaskAllowed = count;
        }

        public void ResumeTask()
        {
            isTaskExecutable = true;
            if (stoppedTask.Count > 0)
            {
           
                stoppedTask.Clear();

            }
            pauseTasksInqueue = stoppedTask.Count;
            CheckAndStartTask();

        }

        public class  TaskInfo
        {
            public IEnumerator _task;
            public string _taskIdentifier;
            public TaskType _type;

            public TaskInfo(IEnumerator taskR,  TaskType taskTypeR,string taskNameR)
            {
                _task = taskR;
                _taskIdentifier = taskNameR;
                _type = taskTypeR;
            }
        }
    }
}
