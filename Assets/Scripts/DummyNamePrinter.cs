using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class DummyNamePrinter : MonoBehaviour
    {

        public List<string> Names = new List<string>()
            {"Radha", " Mohan", "Janak", "Manoj", "Jenny", "Ronaldo", "Brack", "John"};

        public List<string> Cars = new List<string>()
            {"Toyota", "Honda", "Ford", "Suzuki", "Maruti", "Scorpio", "Mercedies", "Bentz"};

        /// <summary>
        /// This is a method to list out the list of the people each seconds
        /// </summary>
        public void IAmPrintingNames()
        {
            TaskManager.Instance.CheckAndStartTask(PrintTheNameFromList(Names));
          //  StartCoroutine(PrintTheNameFromList(Names));
        }
        
        /// <summary>
        /// This is a method to list out the list of the cars each seconds
        /// </summary>
        public void IAmPrintingCarsName()
        {
            TaskManager.Instance.CheckAndStartTask(PrintTheNameFromList(Cars));
          // StartCoroutine(PrintTheNameFromList(Cars));
        }

        private IEnumerator PrintTheNameFromList(List<string> namesToList)
        {
            if (namesToList == null || namesToList.Count == 0) yield break;

            int totalLoop = namesToList.Count;
            for (int i = 0; i < totalLoop; i++)
            {
                yield return new WaitForSecondsRealtime(1);
                Debug.Log(namesToList[i]);
            }
            TaskManager.Instance.SetTaskCompleted();
        }
    }
}