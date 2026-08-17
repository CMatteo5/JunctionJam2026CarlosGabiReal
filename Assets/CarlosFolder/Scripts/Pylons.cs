using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Properties;
using UnityEngine;

public class Pylons : MonoBehaviour
{
    [SerializeField] private bool hasPower;

    [SerializeField] private GameManager manager;
    //[SerializeField] private GameObject[] connections;

    [SerializeField] private List<PowerLines> localPowerLines = new List<PowerLines>();

    [SerializeField] private List<PowerLines> tempLines = new List<PowerLines>();

    private CircleCollider2D powerRadius;

    [SerializeField] public int pylonHealth;

    [SerializeField] private GameObject brokenPylon;

    [SerializeField] private List<string> connectionIDs = new List<string>();

    private List<Pylons> visited = new List<Pylons>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject managerObject = GameObject.Find("GameManager");
        manager = managerObject.GetComponent<GameManager>();
        pylonHealth = 200;
        checkAround();
        StartCoroutine(radiusCheck());
    }

    // Update is called once per frame
    void Update()
    {
        powerCheck();
        healthCheck();
    }

    public bool getPower()
    {
        return hasPower;
    }

    public void flip()
    {
        hasPower = !hasPower;
    }

    private void checkAround()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 4);
        //connections = new GameObject[hits.Length];
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject == this.gameObject)
            {
                continue;
            }

            if (hits[i].gameObject.CompareTag("pylon") || hits[i].gameObject.CompareTag("gemstone") || hits[i].gameObject.CompareTag("generator") || hits[i].gameObject.CompareTag("radar"))
            {
                if (manager.lineExistsBetween(this.transform, hits[i].gameObject.transform))
                {
                    continue;
                }
                localPowerLines.Add(manager.createPowerLine(this.gameObject, hits[i].gameObject, hasPower));
            }
        }

        //for (int i = 0; i < connections.Length; i++)
        //{
        //    if (connections[i] != null)
        //    {

        //    }
        //}

    }

    private void powerCheck()
    {
        visited.Clear();
        hasPower = breakdownCheck(this);
        for (int i = 0; i < localPowerLines.Count; i++)
        {
            localPowerLines[i].powered = hasPower;
        }
    }

    public bool reachesGenerator()
    {
        visited.Clear();
        return breakdownCheck(this);
    }

    private bool breakdownCheck(Pylons current)
    {
        if (visited.Contains(current))
        {
            return false;
        }
        visited.Add(current);

        Collider2D[] hits2 = Physics2D.OverlapCircleAll(current.transform.position, 4);


        for (int i = 0; i < hits2.Length; i++)
        {
            if (hits2[i].gameObject.CompareTag("generator"))
            {
                return true;
            }
            else if (hits2[i].gameObject.CompareTag("pylon"))
            {

                Pylons nextPylon = hits2[i].gameObject.GetComponent<Pylons>();
                if (nextPylon != null && nextPylon != current)
                {

                    if (breakdownCheck(nextPylon))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }



    IEnumerator radiusCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(30);
            checkAround();
        }

    }

    private void healthCheck()
    {
        if (pylonHealth <= 0)
        {
            Instantiate(brokenPylon, transform.position, transform.rotation);
            manager.destroyLinesTouching(transform);
            Destroy(gameObject);
        }
    }

    public void decreaseHealth(int amount)
    {
        pylonHealth -= amount;
    }

}