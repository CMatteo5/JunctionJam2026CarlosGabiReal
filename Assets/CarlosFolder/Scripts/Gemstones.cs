using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Properties;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Gemstones : MonoBehaviour
{
    [SerializeField] private bool hasPower;

    [SerializeField] private GameManager manager;
    //[SerializeField] private GameObject[] connections;

    [SerializeField] private List<PowerLines> localPowerLines = new List<PowerLines>();

    private CircleCollider2D powerRadius;

    [SerializeField] private List<string> connectionIDs = new List<string>();

    [SerializeField] private GameObject door;

    private List<Gemstones> visited = new List<Gemstones>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject managerObject = GameObject.Find("GameManager");
        manager = managerObject.GetComponent<GameManager>();
        checkAround();
        StartCoroutine(radiusCheck());
    }

    // Update is called once per frame
    void Update()
    {
        powerCheck();
        if (hasPower)
        {
            door.SetActive(false);
        }
        else
        {
            door.SetActive(true);
        }
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 5);
        //connections = new GameObject[hits.Length];
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject == this.gameObject)
            {
                continue;
            }

            if (hits[i].gameObject.CompareTag("pylon") || hits[i].gameObject.CompareTag("gemstone") || hits[i].gameObject.CompareTag("generator"))
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

    private bool breakdownCheck(Gemstones current)
    {
        if (visited.Contains(current))
        {
            return false;
        }
        visited.Add(current);

        Collider2D[] hits2 = Physics2D.OverlapCircleAll(current.transform.position, 5);


        for (int i = 0; i < hits2.Length; i++)
        {
            if (hits2[i].gameObject.CompareTag("generator"))
            {
                return true;
            }
            else if (hits2[i].gameObject.CompareTag("pylon"))
            {

                Pylons nextPylon = hits2[i].gameObject.GetComponent<Pylons>();
                if (nextPylon != null)
                {

                    if (nextPylon.reachesGenerator())
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

}