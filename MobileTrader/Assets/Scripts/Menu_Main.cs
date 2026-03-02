using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Main : MonoBehaviour
{ // script en Canvas scene Menu Main
    void Start()
    { Time.timeScale = 1;}
    public void StartGame()
    { SceneManager.LoadScene(1);}
    public void ExitGame()
    {
        print("salgo de app");
        Application.Quit();
    }
}
