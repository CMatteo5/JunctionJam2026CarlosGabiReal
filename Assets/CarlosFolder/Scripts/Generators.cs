using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
public class Generators : MonoBehaviour
{
    [SerializeField] private bool hasPower;
    [SerializeField] private GameManager manager;
    //public GameObject[] connections;
    [SerializeField] private List<PowerLines> localPowerLines = new List<PowerLines>();
    private CircleCollider2D powerRadius;
    public List<string> connectionIDs = new List<string>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject managerObject = GameObject.Find("GameManager");
        manager = managerObject.GetComponent<GameManager>();
        hasPower = true;
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
    public bool reachesRadar()
    {
        List<GameObject> visited = new List<GameObject>();
        return radarWalk(this.gameObject, visited);
    }
    private bool radarWalk(GameObject current, List<GameObject> visited)
    {
        if (visited.Contains(current))
        {
            return false;
        }
        visited.Add(current);

        Collider2D[] hits = Physics2D.OverlapCircleAll(current.transform.position, 3);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject == current)
            {
                continue;
            }
            if (hits[i].gameObject.CompareTag("radar"))
            {
                return true;
            }
            if (hits[i].gameObject.CompareTag("pylon") || hits[i].gameObject.CompareTag("generator"))
            {
                if (radarWalk(hits[i].gameObject, visited))
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void checkAround()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 3);
        //connections = new GameObject[hits.Length];
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].gameObject == this.gameObject)
            {
                continue;
            }
            bool same = false;
            if (hits[i].gameObject.CompareTag("pylon") || hits[i].gameObject.CompareTag("gemstone") || hits[i].gameObject.CompareTag("generator"))
            {
                for (int j = 0; j < localPowerLines.Count; j++)
                {
                    if (hits[i].gameObject.transform == localPowerLines[j].end || hits[i].gameObject.transform == localPowerLines[j].start)
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
                //connections[i] = hits[i].collider.gameObject;
                localPowerLines.Add(manager.createPowerLine(this.gameObject, hits[i].gameObject, hasPower));
            }
        }
        //for (int i = 0; i < connections.Length; i++)
        //{
        //    if (connections[i] != null)
        //    {
        //        localPowerLines.Add(manager.createPowerLine(this.gameObject, connections[i],hasPower));
        //    }
        //}
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