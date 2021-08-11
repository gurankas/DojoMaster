using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene("Gym_Gurankas");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
