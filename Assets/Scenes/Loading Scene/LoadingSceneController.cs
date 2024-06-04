using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingSceneController : MonoBehaviour
{
    public TextMeshProUGUI loadingText;

    static string nextScene;

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading Scene");
    }

    // Start is called before the first frame update
    void Start()
    {  
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator LoadSceneProcess()
    {  
        AsyncOperation op =  SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                loadingText.text = string.Format("로딩 중...{0}%", Mathf.Floor(op.progress * 100).ToString());
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                print(timer);
                loadingText.text = string.Format("로딩 중...{0}%", Mathf.Floor((op.progress + Mathf.Lerp(0f, 0.1f, timer * 10)) * 100).ToString());
                if (loadingText.text == "로딩 중...100%")
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
