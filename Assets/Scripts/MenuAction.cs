using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuAction : MonoBehaviour
{
    public void goToGame(){
        SceneManager.LoadScene("MattScene");
    }

    public void ExitGame(){
        Application.Quit();
    }
}
