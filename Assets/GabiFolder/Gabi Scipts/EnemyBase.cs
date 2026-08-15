using UnityEngine;

public class EnemyBase : MonoBehaviour
{

    private Collider2D myCollision;


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
        if (collision.CompareTag("Player"))
        {

        }
    }
}
