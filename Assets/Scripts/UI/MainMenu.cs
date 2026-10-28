using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Game scene";

    public void PlayGame()
    {
        Debug.Log("Кнопка Новая игра нажата!");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        Debug.Log("Настройки в разработке");
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры");
        Application.Quit();
    }
}