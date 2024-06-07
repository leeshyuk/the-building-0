using TMPro;
using UnityEngine;

public class BaseMapController : MonoBehaviour
{

    public GameObject[] signboards;
    public TextMeshPro[] infoTexts;

    public GameObject elevator;
    public AudioSource[] elevatorAudioSounds;
    ElevatorController elevatorController;

    public GameObject[] doors;

    public 


    // Start is called before the first frame update
    void Start()
    {
        elevatorController = elevator.GetComponent<ElevatorController>();
        Init(1);
    }

    public void Init(int floor)
    {
        for (int i = 0; i < signboards.Length; i++)
        {
            string roomNumber = signboards[i].GetComponent<SignboardController>().initialNumber;
            signboards[i].GetComponent<SignboardController>().initialNumber = string.Format("{0}{1}{2}", floor, roomNumber[1], roomNumber[2]);
        }
        infoTexts[0].text = string.Format("{0}F", floor);
        infoTexts[1].text = string.Format("{0}F", floor);
        infoTexts[2].text = string.Format("{0}", floor);
        infoTexts[3].text = string.Format("{0}", floor);
        infoTexts[4].text = string.Format("{0}", floor);
        infoTexts[5].text = string.Format("{0}", floor);
        for (int i = 0;i < doors.Length;i++)
        {
            if (doors[i].GetComponent<LRD1Controller>()) doors[i].GetComponent<LRD1Controller>().angle = 0;
            else doors[i].GetComponent<LRD2Controller>().angle = 0;
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
        elevatorAudioSounds[2].Stop();
        elevatorAudioSounds[3].Stop();
    }
}
