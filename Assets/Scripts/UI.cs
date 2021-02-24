using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text teamName;
    public Button restartBtn;

    public void DisplayWinningTeam(string teamName)
    {
        this.teamName.gameObject.SetActive(true);
        restartBtn.gameObject.SetActive(true);

        this.teamName.text = teamName;
        Time.timeScale = 0;
    }

    public void RestartPressed()
    {
        this.teamName.gameObject.SetActive(false);
        restartBtn.gameObject.SetActive(false);

        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
