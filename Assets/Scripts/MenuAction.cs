using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuAction : MonoBehaviour
{
    public void goToGame(){
        SceneManager.LoadScene("MattScene");
    }

    public void ExitGame(){
        Application.Quit();
    }
    
}
