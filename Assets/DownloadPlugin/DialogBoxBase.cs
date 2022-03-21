using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class DialogBoxBase : MonoBehaviour
{
    public virtual void OpenDialog()
    {
        
    }
   
    public virtual void CloseDialog()
    {
        gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        OpenDialog();
    }

    public virtual void SetTaskComplete(string gameObjectName)
    {
        PopupScheduler.SetTaskCompleted(gameObjectName);
    }

    
}
