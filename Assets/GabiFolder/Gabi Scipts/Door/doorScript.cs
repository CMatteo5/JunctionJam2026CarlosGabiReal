using UnityEngine;

public class doorScript : MonoBehaviour
{

    public GameObject door;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void doorOpen()
    {
        door.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
