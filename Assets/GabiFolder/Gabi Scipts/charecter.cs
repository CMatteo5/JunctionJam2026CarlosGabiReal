using System.Collections;
using UnityEngine;
public class charecter : MonoBehaviour
{
    public float speed = 0.5f;
    private float blah = 1.5f; //THIS IS THE TIMER FOR THE COURTINE FOR INTERACTIN ex:Mining
    private Rigidbody2D rb;
    private Collider2D myCollision;
    private Vector2 input;
    public Animator myAnim;
    private bool isMoving = false;
    public SpriteRenderer InteractCircle;
    public bool copperTarget = false;
    public bool iornTarget = false;
    public bool isInteracting = false;
    public bool buildMode = false;
    public bool canPlace = false;
    public bool dead = false;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject pylonPrefab;
    private GameManager manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject managerObject = GameObject.Find("GameManager");
        manager = managerObject.GetComponent<GameManager>();
        rb = GetComponent<Rigidbody2D>();
        myCollision = GetComponent<Collider2D>();
        InteractCircle.enabled = false;
        myAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            return;
        }
        if (!isInteracting)
        {
            isMoving = true;
            myAnim.SetBool("isMoving", isMoving);
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            input.Normalize();
        }
        else
        {
            isMoving = false;
            myAnim.SetBool("isMoving", isMoving);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            toggle();
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (buildMode)
            {
                TryBuild();
            }
            else
            {
                Interact();
            }
        }

        //flip
        if(input.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (input.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    public void setDead(bool state)
    {
        dead = state;
        input = Vector2.zero;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        if (myCollision != null)
        {
            myCollision.enabled = !state;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = !state;
        }
        if (myAnim != null)
        {
            myAnim.enabled = !state;
        }
    }
    public bool canAfford()
    {
        return manager.getIron() >= 1 && manager.getCopper() >= 1;
    }
    private void TryBuild()
    {
        if (canPlace && pylonPrefab != null && !isInteracting && canAfford())
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            StartCoroutine(buildTimer(mouseWorldPos));
        }
    }
    private void Interact()
    {
        if (copperTarget && !isInteracting)
        {
            manager.addCopper(1);
            manager.playInteract();
            StartCoroutine(interactTimer());

        }
        if (iornTarget && !isInteracting)
        {
            manager.addIron(1);
            manager.playInteract();
            StartCoroutine(interactTimer());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("copper"))
        {
            copperTarget = true;
        }
        if (collision.CompareTag("iorn"))
        {
            iornTarget = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("copper"))
        {
            copperTarget = false;
        }
        if (collision.CompareTag("iorn"))
        {
            iornTarget = false;
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
        InteractCircle.enabled = true;
        isInteracting = true;
        yield return new WaitForSeconds(blah); //MAGIC NUMBER TIME FOR INTERACTING
        isInteracting = false;
        InteractCircle.enabled = false;
    }
    IEnumerator buildTimer(Vector3 spawnPosition)
    {
        InteractCircle.enabled = true;
        isInteracting = true;
        yield return new WaitForSeconds(blah); //MAGIC NUMBER TIME FOR INTERACTING
        Instantiate(pylonPrefab, spawnPosition, Quaternion.identity);
        manager.removeIron(1);
        manager.removeCopper(1);
        manager.playInteract();
        isInteracting = false;
        InteractCircle.enabled = false;
    }
    private void FixedUpdate()
    {
        if (dead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        if (!isInteracting)
        {
            rb.linearVelocity = input * speed;
        }
        else
        {
            rb.linearVelocity = input * 0;
        }
    }
    public void toggle()
    {
        buildMode = !buildMode;
    }
}