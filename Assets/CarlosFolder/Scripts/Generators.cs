using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Generators : MonoBehaviour
{
    public bool hasPower;

    public GameManager manager;
    public GameObject[] connections;

    public List<PowerLines> localPowerLines = new List<PowerLines>();
    public CircleCollider2D powerRadius;

    public List<string> connectionIDs = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            if (hits[i].collider.gameObject.CompareTag("pylon") || hits[i].collider.gameObject.CompareTag("gemstone")|| hits[i].collider.gameObject.CompareTag("generator"))
            {
                for (int j = 0; j <localPowerLines.Count; j++)
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
                localPowerLines.Add(manager.createPowerLine(this.gameObject, connections[i]));
            }
        }

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
