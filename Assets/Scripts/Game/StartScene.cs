using UnityEngine;


public class StartScene : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
