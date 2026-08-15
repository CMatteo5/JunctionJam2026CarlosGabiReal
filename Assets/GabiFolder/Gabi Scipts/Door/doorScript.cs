using UnityEngine;

public class doorScript : MonoBehaviour
{

    public GameObject doorWallLeft;
    public GameObject doorWallRight;
    public GameObject doorLeft;
    public GameObject doorRight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void doorOpen()
    {
        doorLeft.SetActive(false);
        doorRight.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
