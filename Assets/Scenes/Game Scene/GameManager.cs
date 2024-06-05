using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int floor = 1;
    public bool isStrange = false;
    public int tpCount = 0;
    public bool onStage = true;
    public string[] theStranges = { "Door", "Can", "Signboard", "Picture", "Rotation", "Ghost"};
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
        isStrange = Random.Range(0, 2).Equals(0);
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

    void Can()
    {
        transform.GetComponentInChildren<Can>().enabled = !transform.GetComponentInChildren<Can>().enabled;
    }

    void Signboard()
    {
        transform.GetComponentInChildren<Signboard>().enabled = !transform.GetComponentInChildren<Signboard>().enabled;
    }

    void Picture()
    {
        transform.GetComponentInChildren<Picture>().enabled = !transform.GetComponentInChildren<Picture>().enabled;
    }

    void Rotation()
    {
        transform.GetComponentInChildren<Rotation>().enabled = !transform.GetComponentInChildren<Rotation>().enabled;
    }

    void Ghost()
    {
        transform.GetComponentInChildren<Ghost>().enabled = !transform.GetComponentInChildren<Ghost>().enabled;
    }

    public void Stop()
    {
        Invoke(now, 0);
    }
}
