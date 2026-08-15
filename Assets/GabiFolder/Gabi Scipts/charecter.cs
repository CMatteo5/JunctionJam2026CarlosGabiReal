using UnityEngine;

public class charecter : MonoBehaviour
{

    public float speed = 0.5f;
    private Rigidbody2D rb;
    private Collider2D myCollision;
    private Vector2 input;

    private bool copperTarget = false;
    private bool iornTarget = false;


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



        if (Input.GetMouseButtonDown(0))
        {
            Interact();
        }
    }


    private void Interact()
    {
        if(copperTarget)
        {
            print("COPPER");
        }
        if(iornTarget)
        {
            print("IORN");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("copper"))
        {
            copperTarget = true;
        }
        if(collision.CompareTag("iorn"))
        {
            iornTarget = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("copper"))
        {
            copperTarget = false;
        }
        if(collision.CompareTag("iorn"))
        {
            iornTarget= false;
        }
    }




    private void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }
}
