using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [SerializeField]private List<GameObject> popUps;
    private void Start()
    {
        PopupScheduler.AddTask(popUps[0]);
        PopupScheduler.AddTask(popUps[1]);
    }

  
}
