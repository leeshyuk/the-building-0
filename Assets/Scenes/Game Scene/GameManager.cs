using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int floor = 1;
    public bool isStrange = false;
    public int tpCount = 0;
    public bool onStage = true;
    public string[] theStranges = { "Door", "Signboard", "Picture", "Ghost"};
    public string now = "None";

    public GameObject baseMap;

    BaseMapController baseMapController;

    private void Start()
    {
        baseMapController = baseMap.GetComponent<BaseMapController>();
        baseMapController.StartStory1();

    }

    public void InitStage(int stage)
    {
        now = "None";
        floor = stage;
        isStrange = !Random.Range(0, 5).Equals(0);
        if (floor == 5)
        {
            isStrange = false;
            transform.Find("Phone").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Phone").gameObject.SetActive(false);
        }
        if (isStrange)
        {
            now = theStranges[Random.Range(0, theStranges.Length)];
            Invoke(now, 0);
        }
        baseMapController.Init(stage);
    }

    void Door()
    {
        transform.GetComponentInChildren<Door>().enabled = !transform.GetComponentInChildren<Door>().enabled;
    }

    void Signboard()
    {
        transform.GetComponentInChildren<Signboard>().enabled = !transform.GetComponentInChildren<Signboard>().enabled;
    }

    void Picture()
    {
        transform.Find("Picture").gameObject.active = !transform.Find("Picture").gameObject.active;
    }

    void Ghost()
    {
        transform.Find("Ghost").gameObject.active = !transform.Find("Ghost").gameObject.active;
    }

    public void Stop()
    {
        Invoke(now, 0);
    }
}
