using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("game");
    }

    public void OpenControls()
    {
        SceneManager.LoadScene("Control");
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void OpenMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
