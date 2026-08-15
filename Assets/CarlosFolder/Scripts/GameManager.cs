using System.Collections;
using TMPro;
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

    bool readyToSpawn;
    public Transform playerTransform;

    [SerializeField]  float gracePeriodTime;
    float enemySpawnTime;
    Vector3[] enemySpawns;
    int enemyThreshold = 5;

    [SerializeField] Image background;
    [SerializeField] TextMeshPro text1;
    [SerializeField] TextMeshPro text2;
    [SerializeField] TextMeshPro text3;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        readyToSpawn = false;

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
            StartCoroutine(enemySpawnDelay(enemySpawnTime));
            spawnEnemy();
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
        yield return new WaitForSeconds(time);
        readyToSpawn = true;
    }

    public Vector3 getRandomLocation()
    {
        int randomIndex = Random.Range(0, enemySpawns.Length);
        Vector3 randomLocation = enemySpawns[randomIndex];
        return randomLocation;
    }

    IEnumerator startGame()
    {
        yield return new WaitForSeconds(5);
        text1.enabled = false;
        text2.enabled = true;

    } 

}
