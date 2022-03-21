using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardDialog :DialogBoxBase
{
  public override void OpenDialog()
    {
      Debug.Log("opening dialog");
    }
  
  
  
  public override void CloseDialog()
  {
    gameObject.SetActive(false);
    base.SetTaskComplete(gameObject.name);
    
  }
}
