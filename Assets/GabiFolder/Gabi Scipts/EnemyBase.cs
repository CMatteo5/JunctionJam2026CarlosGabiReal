using UnityEngine;

public class EnemyBase : MonoBehaviour
{

    private Collider2D myCollision;
    private Vector2 targetPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myCollision = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("YUP");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            targetPosition = collision.transform.position;
            print("PLAYER DETECTED");
        }
    }
}
