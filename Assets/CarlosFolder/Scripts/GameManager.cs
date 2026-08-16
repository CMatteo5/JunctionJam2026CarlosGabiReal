using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
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

    public List<PowerLines> allLines = new List<PowerLines>();

    bool readyToSpawn;
    bool gracePeriod;
    bool introActive;
    Transform playerTransform;

    [SerializeField] float gracePeriodTime;
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
        introActive = true;

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
        checkWinCondition();
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

        // Draw Power Lines
        for (int i = 0; i < allLines.Count; i++)
        {
            if (allLines[i] != null && allLines[i].start != null && allLines[i].end != null)
            {
                allLines[i].renderer.SetPosition(0, allLines[i].start.position);
                allLines[i].renderer.SetPosition(1, allLines[i].end.position);
            }
        }


    }

    public int getIron()
    {
        return playerIron;
    }

    public void addIron(int amount)
    {
        playerIron += amount;
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
        playerCopper += amount;
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
        if (readyToSpawn)
        {
            Vector3 selectedLocation = getRandomLocation();
            if (areaCheck(selectedLocation))
            {
                Instantiate(enemyRef, selectedLocation, transform.rotation);
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
            //Debug.Log("Waiting to Spawn");
            yield return new WaitForSeconds(time);
            //Debug.Log("Spawning...");
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
        background.enabled = false;
        introActive = false;
        //text2.enabled = true;

    }

    public bool getIntroActive()
    {
        return introActive;
    }

    public bool lineExistsBetween(Transform a, Transform b)
    {
        for (int i = 0; i < allLines.Count; i++)
        {
            if (allLines[i] == null)
            {
                continue;
            }
            if ((allLines[i].start == a && allLines[i].end == b) || (allLines[i].start == b && allLines[i].end == a))
            {
                return true;
            }
        }
        return false;
    }

    public PowerLines createPowerLine(GameObject caller, GameObject desiredConnection, bool isPowered)
    {
        GameObject lineHolder = new GameObject("PowerLine");
        LineRenderer startingPoint = lineHolder.AddComponent<LineRenderer>();
        startingPoint.positionCount = 2;
        startingPoint.startWidth = wireThickness;
        startingPoint.endWidth = wireThickness;
        startingPoint.material = new Material(Shader.Find("Sprites/Default"));
        startingPoint.startColor = Color.black;
        startingPoint.endColor = Color.black;
        PowerLines temp = ScriptableObject.CreateInstance<PowerLines>();
        temp.renderer = startingPoint;
        temp.lineObject = lineHolder;
        temp.start = caller.transform;
        temp.end = desiredConnection.transform;
        temp.lineID = generateID();
        temp.source = caller;
        temp.powered = isPowered;
        allLines.Add(temp);
        return temp;
    }

    public int connectedGenerators;

    private void checkWinCondition()
    {
        Generators[] allGenerators = FindObjectsByType<Generators>(FindObjectsSortMode.None);
        connectedGenerators = 0;
        for (int i = 0; i < allGenerators.Length; i++)
        {
            if (allGenerators[i].reachesRadar())
            {
                connectedGenerators++;
            }
        }

        if (allGenerators.Length > 0 && connectedGenerators == allGenerators.Length)
        {
            winGame();
        }
    }

    private void winGame()
    {
        Debug.Log("You've won");
        //SCENE CHANGE GOES HERE
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

    public void cleanUpAllLists()
    {
        for (int i = 0; i < allLines.Count; i++)
        {
            if (allLines[i] == null || allLines[i].start == null || allLines[i].end == null)
            {
                allLines.RemoveAt(i);
                continue;
            }
        }
    }

    public void destroyPowerLine(string target)
    {
        for (int i = 0; i < allLines.Count; i++)
        {

            if (allLines[i].lineID.Equals(target))
            {
                Debug.Log("Hello from destroyPowerLine");
                if (allLines[i].lineObject != null)
                {
                    Destroy(allLines[i].lineObject);
                }
                allLines.RemoveAt(i);
                break;
            }

            cleanUpAllLists();
        }
    }

    public void destroyLinesTouching(Transform node)
    {
        for (int i = allLines.Count - 1; i >= 0; i--)
        {
            if (allLines[i] == null || allLines[i].start == node || allLines[i].end == node)
            {
                if (allLines[i] != null && allLines[i].lineObject != null)
                {
                    Destroy(allLines[i].lineObject);
                }
                allLines.RemoveAt(i);
            }
        }
    }

}