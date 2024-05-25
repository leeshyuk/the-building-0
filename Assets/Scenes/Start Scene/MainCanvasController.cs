using UnityEngine;

public class MainCanvasController : MonoBehaviour
{
    public void OnClickStart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game Scene");
    }

    public void OnClickQuit()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
