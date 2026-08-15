using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class GameManager : MonoBehaviour

{
    public GameObject PlayerRef;
    public int playerIron;
    public int playerCopper;
    public int playerHealth;
    [SerializeField] GameObject enemyRef;

    public float wireThickness;

    public List<PowerLines> allLines = new List<PowerLines> ();

    bool readyToSpawn;
    bool gracePeriod;
    Transform playerTransform;

    [SerializeField]  float gracePeriodTime;
    public float enemySpawnTime;
    Vector3[] enemySpawns;
    int enemyThreshold = 5;

    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI text1;
    [SerializeField] TextMeshProUGUI text2;
    [SerializeField] TextMeshProUGUI text3;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        readyToSpawn = false;
        gracePeriod = true;

        playerIron = 0;
        playerCopper = 0;
        playerHealth = 25;

        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("enemySpawn");
        enemySpawns = new Vector3[taggedObjects.Length];
        for (int i = 0; i < taggedObjects.Length; i++)
        {
            enemySpawns[i] = taggedObjects[i].transform.position;
        }
        StartCoroutine(startGame());
        StartCoroutine(enemySpawnDelay(enemySpawnTime));
    }

    // Update is called once per frame
    void Update()
    { 
        
    }

    private void FixedUpdate()
    {
        //Set the player's transform
        playerTransform = PlayerRef.transform;

        //Wait for the grace period, then start to spawn enemies at loactions
        if (gracePeriodTime > 0)
        {
            gracePeriodTime -= Time.deltaTime;
        }
        else
        {
            gracePeriod = false;
        }

        //Draw Power Lines
        for (int i = 0; i < allLines.Count; i++)
        {
            allLines[i].renderer.SetPosition(0, allLines[i].start.position);
            allLines[i].renderer.SetPosition(1,allLines[i].end.position);
        }

    }

    public int getIron()
    {
        return playerIron;
    }

    public void addIron(int amount)
    {
        playerIron+= amount;
    }
    public void removeIron(int amount)
    {
        playerIron -= amount;
    }

    public int getCopper()
    {
        return playerCopper;
    }

    public void addCopper(int amount)
    {
        playerCopper+= amount;
    }

    public void removeCopper(int amount)
    {
        playerCopper -= amount;
    }

    public int getHealth()
    {
        return playerHealth;
    }

    public void playerLoseHealth(int amount)
    {
        playerHealth -= amount;
        if (playerHealth < 0)
        {
            //Add a respawn later
        }
    }

    public void spawnEnemy()
    {
        if (readyToSpawn) {
            Vector3 selectedLocation = getRandomLocation();
            if (areaCheck(selectedLocation))
            {
                Instantiate(enemyRef,selectedLocation, transform.rotation);
                readyToSpawn = false;
            }
            else
            {
                Debug.Log("finding another location");
                spawnEnemy();
                return;
            }

        }
    }

    private bool areaCheck(Vector3 selectedLocation)
    {
        int hitTargets = 0;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(selectedLocation, 5, Vector2.right, 5);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("enemy"))
            {
                hitTargets++;
            }
        }


        if (hitTargets < enemyThreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator enemySpawnDelay(float time)
    {
        if (gracePeriod)
        {
            yield return new WaitForSeconds(gracePeriodTime);
        }
        while (true)
        {
            Debug.Log("Waiting to Spawn");
            yield return new WaitForSeconds(time);
            Debug.Log("Spawning...");
            readyToSpawn = true;
            spawnEnemy();
        }
    }

    public Vector3 getRandomLocation()
    {
        int randomIndex = UnityEngine.Random.Range(0, enemySpawns.Length);
        Vector3 randomLocation = enemySpawns[randomIndex];
        return randomLocation;
    }


    //WORK ON THIS LATER***********************
    IEnumerator startGame()
    {
        yield return new WaitForSeconds(5);
        text1.enabled = false;
        background.enabled= false;
        //text2.enabled = true;

    } 

    public PowerLines createPowerLine(GameObject caller, GameObject desiredConnection)
    {
        LineRenderer startingPoint = caller.GetComponent<LineRenderer>();
        startingPoint.positionCount = 2;
        startingPoint.startWidth = wireThickness;
        startingPoint.endWidth = wireThickness;
        PowerLines temp = ScriptableObject.CreateInstance<PowerLines>();
        temp.renderer = startingPoint;
        temp.start = caller.transform;
        temp.end = desiredConnection.transform;
        temp.lineID = generateID();
        allLines.Add(temp);
        return temp;
    }

    public string generateID()
    {
        int tempNum1 = UnityEngine.Random.Range(0, 10);
        int tempNum2 = UnityEngine.Random.Range(0, 10);
        int tempNum3 = UnityEngine.Random.Range(0, 10);
        string result = $"{tempNum1}{tempNum2}{tempNum3}";
        for (int i = 0; i < allLines.Count; i++)
        {
            if (result.Equals(allLines[i].name))
            {
                return generateID();
            }
        }
        return result;
    }

    public void destroyPowerLine(string target)
    {
        for (int i = 0; i < allLines.Count; i++)
        {
            if (allLines[i].lineID == target)
            {
                allLines.RemoveAt(i);
            }
        }
    }

}
