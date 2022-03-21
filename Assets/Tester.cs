using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Tester : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            PP();
        }
    }

    void PP()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}

