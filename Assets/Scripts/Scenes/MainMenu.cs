using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string nextScene;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
