using UnityEngine;

public class charecter : MonoBehaviour
{

    public float speed = 0.5f;
    private Rigidbody2D rb;
    private Collider2D myCollision;
    private Vector2 input;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollision = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        input.Normalize();
    }


    public void OnCollisionEnter2D(Collision2D other)
    {
        
    }
    public void interact()
    {

    }




    private void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }
}
