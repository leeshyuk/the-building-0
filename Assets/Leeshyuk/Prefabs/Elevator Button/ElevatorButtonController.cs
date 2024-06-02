using UnityEngine;

public class ElevatorButtonController : MonoBehaviour
{
    public GameObject elevator;

    public float waitingTime = 2f;
    private float remainingTime;


    private void Awake()
    {
        remainingTime = waitingTime;
    }

    private void Update()
    {
        if (elevator.GetComponent<ElevatorController>().isOpen)
        {
            remainingTime -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (remainingTime < 0) CloseDoor();
    }

    public void ExecuteInteraction()
    {
        remainingTime = waitingTime;
        if (!elevator.GetComponent<ElevatorController>().isOpen)
        {
            elevator.GetComponent<ElevatorController>().isOpen = true;
        }
    }

    private void CloseDoor()
    {
        elevator.GetComponent<ElevatorController>().isOpen = false;
        remainingTime = 2f;
    }
}
