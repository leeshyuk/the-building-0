using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public GameObject rightDoor, leftDoor;

    public bool isOpen = false;

    private Vector3[] initialCoordinate = new Vector3[2];
    private Vector3[] openCoordinate = { new(-0.55f, 0, 0), new(0.55f, 0, 0) };

    private Vector3 rightVelocity = Vector3.zero;
    private Vector3 leftVelocity = Vector3.zero;

    private void Awake()
    {
        initialCoordinate[0] = rightDoor.transform.localPosition;
        initialCoordinate[1] = leftDoor.transform.localPosition;

        openCoordinate[0] += initialCoordinate[0];
        openCoordinate[1] += initialCoordinate[1];
    }

    private void Update()
    {
        if (isOpen)
        {
            rightDoor.transform.localPosition = Vector3.SmoothDamp(rightDoor.transform.localPosition, openCoordinate[0], ref rightVelocity, 0.5f);
            leftDoor.transform.localPosition = Vector3.SmoothDamp(leftDoor.transform.localPosition, openCoordinate[1], ref leftVelocity, 0.5f);
        }
        else
        {
            rightDoor.transform.localPosition = Vector3.SmoothDamp(rightDoor.transform.localPosition, initialCoordinate[0], ref rightVelocity, 0.5f);
            leftDoor.transform.localPosition = Vector3.SmoothDamp(leftDoor.transform.localPosition, initialCoordinate[1], ref leftVelocity, 0.5f);
        }
    }
}
