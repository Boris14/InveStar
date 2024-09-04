using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    [SerializeField] public string sceneName;

    public void NextScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
