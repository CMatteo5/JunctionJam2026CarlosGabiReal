using UnityEngine;

public class Radars : MonoBehaviour
{

    [SerializeField] private GameManager manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject managerObject = GameObject.Find("GameManager");
        manager = managerObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
