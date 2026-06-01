using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        MusicManager.Instance.PlayMusic("MainMenu");
    }

    public void Play()
    {
        SceneManager.LoadScene("BossFight");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
