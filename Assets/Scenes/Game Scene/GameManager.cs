using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int floor = 1;
    public bool isStrange = false;
    public int tpCount = 0;
    public bool onStage = true;

    public GameObject baseMap;

    BaseMapController baseMapController;

    private void Start()
    {
        baseMapController = baseMap.GetComponent<BaseMapController>();
        baseMapController.StartStory1();
        InitStage(floor);

    }

    public void InitStage(int stage)
    {
        floor = stage;
        if (floor == 5)
        {
            print("핸드폰");
        }
        isStrange = Random.Range(0, 1).Equals(0);
        isStrange = true;
        baseMapController.Init(stage);
    }
}
