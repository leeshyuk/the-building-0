using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public GameObject rightDoor, leftDoor;

    public bool isOpen = false;

    private Vector3[] initialCoodinate = new Vector3[2];
    private Vector3[] openCoodinate = { new(-0.55f, 0, 0), new(0.55f, 0, 0) };

    private Vector3 rightVelocity = Vector3.zero;
    private Vector3 leftVelocity = Vector3.zero;

    private void Awake()
    {
        initialCoodinate[0] = rightDoor.transform.localPosition;
        initialCoodinate[1] = leftDoor.transform.localPosition;
        print(initialCoodinate[0]);
        print(initialCoodinate[1]);

        openCoodinate[0] += initialCoodinate[0];
        openCoodinate[1] += initialCoodinate[1];
        print(openCoodinate[0]);
        print(openCoodinate[1]);
    }

    private void Update()
    {
        if (isOpen)
        {
            rightDoor.transform.localPosition = Vector3.SmoothDamp(rightDoor.transform.localPosition, openCoodinate[0], ref rightVelocity, 0.5f);
            leftDoor.transform.localPosition = Vector3.SmoothDamp(leftDoor.transform.localPosition, openCoodinate[1], ref leftVelocity, 0.5f);
        }
        else
        {
            rightDoor.transform.localPosition = Vector3.SmoothDamp(rightDoor.transform.localPosition, initialCoodinate[0], ref rightVelocity, 0.5f);
            leftDoor.transform.localPosition = Vector3.SmoothDamp(leftDoor.transform.localPosition, initialCoodinate[1], ref leftVelocity, 0.5f);
        }
    }
}
