using System.Collections;
using UnityEngine;

public class charecter : MonoBehaviour
{

    public float speed = 0.5f;
    private float blah = 2; //THIS IS THE TIMER FOR THE COURTINE FOR INTERACTIN ex:Mining
    private Rigidbody2D rb;
    private Collider2D myCollision;
    private Vector2 input;
    public SpriteRenderer InteractCircle;

    private bool copperTarget = false;
    private bool iornTarget = false;
    private bool isInteracting = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollision = GetComponent<Collider2D>();
        InteractCircle.enabled = false;

        StartCoroutine(interactTimer());

    }

    // Update is called once per frame
    void Update()
    {
        if(!isInteracting)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            input.Normalize();
        }



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
            InteractCircle.enabled=true;
        }
        if(iornTarget)
        {
            print("IORN");
            InteractCircle.enabled = true;
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

    public void StopMovement()
    {
        isInteracting = true;
    }

    public void StartMovment()
    {
        isInteracting = false;
    }

    IEnumerator interactTimer()
    {
        yield return new WaitForSeconds(blah); //MAGIC NUMBER TIME FOR INTERACTING
    }


    private void FixedUpdate()
    {
        rb.linearVelocity = input * speed;
    }
}
