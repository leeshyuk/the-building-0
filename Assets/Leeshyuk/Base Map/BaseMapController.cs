using TMPro;
using UnityEngine;

public class BaseMapController : MonoBehaviour
{

    public GameObject[] signboards;
    public TextMeshPro[] infoTexts;

    public GameObject elevator;
    public AudioSource[] elevatorAudioSounds;
    ElevatorController elevatorController;


    // Start is called before the first frame update
    void Start()
    {
        elevatorController = elevator.GetComponent<ElevatorController>();
    }

    public void Init(int floor)
    {
        for (int i = 0; i < signboards.Length; i++)
        {
            string roomNumber = signboards[i].GetComponent<SignboardController>().initialNumber;
            signboards[i].GetComponent<SignboardController>().initialNumber = string.Format("{0}{1}{2}", floor, roomNumber[1], roomNumber[2]);
        }
        for (int i = 0; i < infoTexts.Length; i++)
        {
            infoTexts[i].text = string.Format("{0}F", floor);
        }
    }

    public void StartStory1()
    {
        elevatorAudioSounds[0].Play();
        Invoke(nameof(StartStory2), 5);
    }

    void StartStory2()
    {
        elevatorController.ChangeText("2");
        Invoke(nameof(StartStory3), 5);
    }

    void StartStory3()
    {
        elevatorController.ChangeText("3");
        Invoke(nameof(StartStory4), 5);
    }

    void StartStory4()
    {
        elevatorAudioSounds[1].Play();
        elevatorAudioSounds[2].Play();
        elevatorController.ChangeText("4");
        Invoke(nameof(StartStory5), 5);
    }

    void StartStory5()
    {
        elevatorAudioSounds[0].Stop();
        elevatorController.ChangeText("3");
        Invoke(nameof(StartStory6), 5);
    }
    void StartStory6()
    {
        elevatorAudioSounds[1].Stop();
        elevatorController.ChangeText("2");
        Invoke(nameof(StartStory7), 3);
    }

    void StartStory7()
    {
        elevatorAudioSounds[2].Stop();
        elevatorController.ChangeText("2");
        Invoke(nameof(StartStory8), 1);
    }
    void StartStory8()
    {
        elevatorController.ChangeText("1");
        elevatorAudioSounds[3].Play();
        elevator.transform.Find("Car").tag = "Interactable Object";
        Invoke(nameof(StartStory9), 5);
    }
    void StartStory9()
    {
        elevatorAudioSounds[3].Stop();
    }

}
