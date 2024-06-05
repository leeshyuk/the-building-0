using UnityEngine;
using UnityEngine.SceneManagement;

public class PhoneController : MonoBehaviour
{
    public GameObject player;
    public void ExecuteInteraction()
    {
        SceneManager.LoadScene("Ending Scene");
    }


}
