using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assemblies;
using static UnityEngine.GraphicsBuffer;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private CircleCollider2D detectRadius;
    [SerializeField] private CircleCollider2D attackRadius;

    [SerializeField] private List<GameObject> pylonTargets;
    [SerializeField] private List<GameObject> playerTargets;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float stoppingDistance = .25f; 

    [SerializeField] private float attackCooldownTime = 2f;

    [SerializeField] private GameManager manager;

    [SerializeField] private int attackDamage = 5;

    [SerializeField] private GameObject currentTarget;
    private Vector3 currentTargetPosition;

    public GameObject PlayerRef;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerRef = GameObject.Find("Player");
        StartCoroutine(retarget());
        StartCoroutine(scan());
        StartCoroutine(attackCheck());
    }

    // Update is called once per frame
    void Update()
    {
        moveToCurrentTarget();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerTargets.Add(collision.gameObject);
        }else if (collision.gameObject.CompareTag("pylon"))
        {
            pylonTargets.Add(collision.gameObject);
        }
    }

    IEnumerator attackCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackCooldownTime);
            Collider2D[] hits2 = Physics2D.OverlapCircleAll(transform.position, 1.5f);
            if (hits2 != null)
            {
                for (int i = 0; i < hits2.Length; i++)
                {
                    if (hits2[i].gameObject.CompareTag("Player"))
                    {
                        attackPlayer();
                    }
                    else if (hits2[i].gameObject.CompareTag("pylon"))
                    {
                        attackPylon(hits2[i].gameObject);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
    }

    private void attackPlayer()
    {
        if (manager.getHealth() > attackDamage)
        {
            manager.playerLoseHealth(attackDamage);
        }
        else
        {
            manager.playerLoseHealth(attackDamage);
            playerTargets.Clear();
            setRandom();
        }
    }
    private void attackPylon(GameObject temp)
    {
        if (temp.GetComponent<Pylons>().pylonHealth > 0)
        {
            temp.GetComponent<Pylons>().decreaseHealth(attackDamage);
        }
        else
        {
            temp.GetComponent<Pylons>().decreaseHealth(attackDamage);
            pylonTargets.Clear();
            setRandom();
        }
    }

    IEnumerator retarget()
    {
        while (true)
        {
            //Debug.Log("Finding target...");
            if (playerTargets.Count > 0)
            {
                currentTarget = playerTargets[0].gameObject;
                currentTargetPosition = currentTarget.transform.position;
            }
            else if (pylonTargets.Count > 0)
            {
                currentTarget = pylonTargets[0].gameObject;
                currentTargetPosition = currentTarget.transform.position;
            }
            else
            {
                setRandom();
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    private void moveToCurrentTarget()
    {
        float distance = Vector2.Distance(transform.position, currentTargetPosition);
        if (distance > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentTargetPosition, speed * Time.deltaTime);
        }
    }

    private void setRandom()
    {
        Vector2 minBounds = new Vector2(-10f, -10f);
        Vector2 maxBounds = new Vector2(10f, 10f);
        float randomX = Random.Range(minBounds.x, maxBounds.x);
        float randomY = Random.Range(minBounds.y, maxBounds.y);
        currentTargetPosition = new Vector3(randomX, randomY, 0);
    }

    IEnumerator scan()
    {
        while (true)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 10);
            if (hits != null)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].gameObject.CompareTag("Player"))
                    {
                        playerTargets.Add(hits[i].gameObject);
                    }
                    else if (hits[i].gameObject.CompareTag("pylon"))
                    {
                        pylonTargets.Add(hits[i].gameObject);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    private void OnMouseDown()
    {
        if(!PlayerRef.gameObject.GetComponent<charecter>().iornTarget && !PlayerRef.gameObject.GetComponent<charecter>().copperTarget&&!PlayerRef.gameObject.GetComponent<charecter>().buildMode&& !PlayerRef.gameObject.GetComponent<charecter>().isInteracting)
        {
            Destroy(gameObject);
        }
    }




}
