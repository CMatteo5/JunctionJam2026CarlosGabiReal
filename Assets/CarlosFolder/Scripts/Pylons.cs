using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Properties;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Pylons : MonoBehaviour
{
    public bool hasPower;

    [SerializeField] private GameManager manager;
    public GameObject[] connections;

    [SerializeField] private List<PowerLines> localPowerLines = new List<PowerLines>();

    [SerializeField] private List<PowerLines> tempLines = new List<PowerLines>();

    private CircleCollider2D powerRadius;

    [SerializeField]private int pylonHealth;

    [SerializeField] private GameObject brokenPylon;

    [SerializeField] private List<string> connectionIDs = new List<string>();

    public List<string> visited = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject managerObject = GameObject.Find("GameManager");
        manager = managerObject.GetComponent<GameManager>();
        pylonHealth = 50;
        checkAround();
        StartCoroutine(radiusCheck());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool getPower()
    {
        return hasPower;
    }

    public void flip()
    {
        hasPower = !hasPower;
    }

    public void checkAround()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 5, Vector2.right, 5);
        connections = new GameObject[hits.Length];
        for (int i = 0; i < hits.Length; i++)
        {
            bool same = false;
            if (hits[i].collider.gameObject.CompareTag("pylon") || hits[i].collider.gameObject.CompareTag("gemstone") || hits[i].collider.gameObject.CompareTag("generator"))
            {
                for (int j = 0; j < localPowerLines.Count; j++)
                {
                    if (hits[i].collider.gameObject.transform == localPowerLines[j].end)
                    {
                        same = true;
                        break;
                    }
                }

                if (same)
                {
                    same = false;
                    continue;
                }
                connections[i] = hits[i].collider.gameObject;
            }
        }

        for (int i = 0; i < connections.Length; i++)
        {
            if (connections[i] != null)
            {
                localPowerLines.Add(manager.createPowerLine(this.gameObject, connections[i],hasPower));
            }
        }

    }

    private void powerCheck()
    {
        for (int i = 0; i < localPowerLines.Count; i++)
        {
            if (localPowerLines[i].powered)
            {
                hasPower = true;
                return;
            }
            hasPower= false;
        }
    }

    private bool breakdownCheck(Pylons current)
    {

        RaycastHit2D[] hits2 = Physics2D.CircleCastAll(transform.position, 5, Vector2.right, 5);


        for (int i = 0; i < hits2.Length; i++)
        {
            if (hits2[i].collider.gameObject.CompareTag("generator"))
            {
                hasPower = true;
                return true;
            }
            else if (hits2[i].collider.gameObject.CompareTag("pylon"))
            {

                Pylons nextPylon = hits2[i].collider.gameObject.GetComponent<Pylons>();
                if (nextPylon != null && nextPylon != this)
                {

                    hasPower = breakdownCheck(nextPylon);
                }
            }
        }
        hasPower = false;
        return false;
    }



    IEnumerator radiusCheck()
    {
        while (true)
        {
            if (hasPower) {
                yield return new WaitForSeconds(30);
                checkAround();
            }
            else
            {
                yield return null;
            }
        }

    }

    public void healthCheck()
    {
        if (pylonHealth<=0)
        {
            tempLines = new List<PowerLines> ();
            Instantiate(brokenPylon);
            for (int i = 0; i < localPowerLines.Count; i++)
            {
                tempLines.Add(localPowerLines[i]);
                manager.destroyPowerLine(localPowerLines[i].lineID);
            }

            Destroy(gameObject);
        }
    }

}
