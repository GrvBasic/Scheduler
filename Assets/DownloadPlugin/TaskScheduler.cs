using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace DownloadPlugin
{
    public class TaskScheduler : MonoBehaviour
    {
        #region Variables

        [SerializeField] private bool _isPreviousTaskCancellabe, _autoResume;

        [FormerlySerializedAs("startTask")] [SerializeField]
        private bool isTaskExecutable = true;

        private IEnumerator _currenTask;


        [SerializeField] private int totalTaskStarted,
            noOfTasksInqueue,
            totaltaskCompleted,
            pauseTasksInqueue,
            maximumTaskAllowed = 2;

        //List of tasks in queue to be executed
        private Dictionary<string, IEnumerator> totalTaskDictionary = new Dictionary<string, IEnumerator>();

        //List of ongoing tasks.
        private Dictionary<string, IEnumerator> curentTaskDictionary = new Dictionary<string, IEnumerator>();

        //Independent task is executed first than dependent task.
        private Queue<KeyValuePair<string, IEnumerator>> IndependentTaskList =
            new Queue<KeyValuePair<string, IEnumerator>>();

        //Dependent task that might be dependent of Independent task.
        private Queue<KeyValuePair<string, IEnumerator>> DependentTaskList =
            new Queue<KeyValuePair<string, IEnumerator>>();


        [SerializeField] private List<string> completedTaskList = new List<string>();

        private Dictionary<string, IEnumerator> pausedTaskList = new Dictionary<string, IEnumerator>();

        #endregion

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
                IndependentTaskList.Enqueue(new KeyValuePair<string, IEnumerator>(taskName, newTask));
            }
            else if (taskTypeReceived == TaskType.MediumPriority)
            {
                DependentTaskList.Enqueue(new KeyValuePair<string, IEnumerator>(taskName, newTask));
            }
            else if (taskTypeReceived == TaskType.LowPriority)
            {
                DependentTaskList.Enqueue(new KeyValuePair<string, IEnumerator>(taskName, newTask));
            }


            CheckAndStartTask();
        }

        void CheckAndStartTask()
        {
            //This function checks for if there are tasks to execute .
            if (!isTaskExecutable)
                return;
    
            Debug.Log(curentTaskDictionary.Count + "taskCount " + maximumTaskAllowed);
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
            GetCurrentTask();

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
            //Cancelling current task to perform new task. This method is used if new task is to be executed by cancelling old one.
            //In case if maximumTaskLimit Reached
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
            /*if (completedTaskList.Count > 5)
            {
                completedTaskList.Remove(completedTaskList[0]);
            }*/

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
            isTaskExecutable = false;
            //Stop all current Ongoing tasks and keep their reference to resume in future.
            //Cancels all the executing task and donot remove reference of those stopped tasks.
            List<string> keysOfPausedTask = new List<string>();
            foreach (KeyValuePair<string, IEnumerator> currentTask in curentTaskDictionary)
            {
                StopCoroutine(curentTaskDictionary[currentTask.Key]);
                //Adding stopped task to a separate list to resume later.
                pausedTaskList.Add(currentTask.Key, currentTask.Value);
                //We will remove stopped task from curentTaskDictionary after this loop complets.
                //So we save keys of those task in a separate list.
                keysOfPausedTask.Add(currentTask.Key);
            }

            for (int i = 0; i < keysOfPausedTask.Count; i++)
            {
                curentTaskDictionary.Remove(keysOfPausedTask[i]);
            }


            pauseTasksInqueue = pausedTaskList.Count;
        }

        public void CancelThisTask(string nameOfTaskR)
        {
            //If task is started to execute.if it fails to complete by some error .We update its status for cancelled.
            StopCoroutine(curentTaskDictionary[nameOfTaskR]);
            curentTaskDictionary.Remove(nameOfTaskR);
            totalTaskDictionary.Remove(nameOfTaskR);
            noOfTasksInqueue--;
            CheckAndStartTask();
        }

        #endregion

        void GetCurrentTask()
        {
            //Gets new task from queue to be executed.
            KeyValuePair<string, IEnumerator> newTask = new KeyValuePair<string, IEnumerator>();
            _currenTask = null;

            if (IndependentTaskList.Count > 0)
            {
                newTask = IndependentTaskList.Dequeue();
                curentTaskDictionary.Add(newTask.Key, newTask.Value);
                _currenTask = newTask.Value;
            }
            else if (DependentTaskList.Count > 0)
            {
                newTask = DependentTaskList.Dequeue();
                curentTaskDictionary.Add(newTask.Key, newTask.Value);
                _currenTask = newTask.Value;
            }

            if (curentTaskDictionary.Count < 1 && _autoResume)
            {
                //If there is no current tasks we simply check any stopped tasks in queue and operate them
                ResumePausedTask();
            }
        }

        public void ChangeMaximumTaskSimultaneously(int count)
        {
            //Change number of maximum task to perform simultaneously.
            maximumTaskAllowed = count;
            //  KickStartScheduler();
        }

        public void ResumePausedTask()
        {
            //Checking and resuming paused tasks.
            isTaskExecutable = true;
            if (pausedTaskList.Count <= 0)
            {
                Debug.Log("NoTask to resume");
                return;
            }

            List<string> keysOfPausedTask = new List<string>();
            foreach (KeyValuePair<string, IEnumerator> stoppedWork in pausedTaskList)
            {
                DependentTaskList.Enqueue(new KeyValuePair<string, IEnumerator>(stoppedWork.Key, stoppedWork.Value));
                keysOfPausedTask.Add(stoppedWork.Key);
            }

            //Clear the list of stopped works.
            for (int i = 0; i < pausedTaskList.Count; i++)
            {
                pausedTaskList.Remove(keysOfPausedTask[i]);
               
            }
         KickStartScheduler();
            pauseTasksInqueue = pausedTaskList.Count;
        }

        public void KickStartScheduler()
        {
            //When tasks are cancelled or paused scheduler wont start new task and remains inactive.
            //So we call this function to resume other tasks in queue.

            isTaskExecutable = true;
            for (int i = 0; i < maximumTaskAllowed; i++)
            {
                CheckAndStartTask();
            }
        }

  
        public void SetAutoResumePausedTask()
        {
            //When there is no task to do when fetched from GetCurrentTask() but there are paused tasks.
            //If _autoResume is set to true. All paused tasks will be executed.
            isTaskExecutable = true;
            _autoResume = true;
        }
    }
}