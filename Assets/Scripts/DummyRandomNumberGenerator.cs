using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class DummyRandomNumberGenerator : MonoBehaviour
    {
        public float MinimumNumber = 0;
        public float MaximumNumber = 100;

        public void GenerateRandomNumberEachSecond()
        {
            TaskManager.Instance.CheckAndStartTask(GenerateRandomNumbers(1));
            //StartCoroutine(GenerateRandomNumbers(1));
        }

        private IEnumerator GenerateRandomNumbers(float waitTime)
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(waitTime);
                float randomNumber = Random.Range(MinimumNumber, MaximumNumber);
                Debug.Log("Random number is : " + randomNumber);
            }
            
        }
       
    }
}