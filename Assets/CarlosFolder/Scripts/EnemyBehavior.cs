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
    [SerializeField] private float stoppingDistance = 1f; 

    [SerializeField] private float attackCooldownTime = 2f;

    [SerializeField] private GameManager manager;

    [SerializeField] private int attackDamage = 5;

    [SerializeField] private GameObject currentTarget;
    private Vector3 currentTargetPosition;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(retarget());
        StartCoroutine(attackCheck());

    }

    // Update is called once per frame
    void Update()
    {
        moveToCurrentTarget();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("player"))
        {
            playerTargets.Add(collision.gameObject);
        }else if (collision.gameObject.CompareTag("pylon"))
        {
            pylonTargets.Add(collision.gameObject);
        }
    }

    IEnumerator attackCheck()
    {
        yield return new WaitForSeconds(attackCooldownTime);
        Collider2D[] hits2 = Physics2D.OverlapCircleAll(transform.position, 1.5f);
        if (hits2 != null)
        {
            for (int i = 0; i < hits2.Length; i++)
            {
                if (hits2[i].gameObject.CompareTag("player"))
                {
                    attackPlayer();
                }
                else if(hits2[i].gameObject.CompareTag("pylon"))
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

    private void attackPlayer()
    {
        manager.playerLoseHealth(attackDamage);
    }
    private void attackPylon(GameObject temp)
    {
        temp.GetComponent<Pylons>().decreaseHealth(attackDamage);
    }

    IEnumerator retarget()
    {
        if(playerTargets.Count > 0)
        {
            currentTarget = playerTargets[0].gameObject;
            currentTargetPosition = currentTarget.transform.position;
        }else if(pylonTargets.Count > 0)
        {
            currentTarget = pylonTargets[0].gameObject;
            currentTargetPosition = currentTarget.transform.position;
        }
        else
        {
            Vector2 minBounds = new Vector2(-10f, -10f);
            Vector2 maxBounds = new Vector2(10f, 10f);
            float randomX = Random.Range(minBounds.x, maxBounds.x);
            float randomY = Random.Range(minBounds.y, maxBounds.y);
            currentTargetPosition = new Vector3(randomX, randomY, 0);
        }
        yield return new WaitForSeconds(10);
    }

    private void moveToCurrentTarget()
    {
        float distance = Vector2.Distance(transform.position, currentTargetPosition);
        if (distance > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentTargetPosition, speed * Time.deltaTime);
        }
    }

    


}
