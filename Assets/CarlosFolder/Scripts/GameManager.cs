using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
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

    public GameObject respawnArea;

    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI text1;
    [SerializeField] TextMeshProUGUI text2;
    [SerializeField] TextMeshProUGUI text3;
    [SerializeField] TextMeshProUGUI text4;
    [SerializeField] TextMeshProUGUI text5;
    [SerializeField] TextMeshProUGUI youDiedText;

    [SerializeField] TextMeshProUGUI ironText;
    [SerializeField] TextMeshProUGUI copperText;
    [SerializeField] TextMeshProUGUI healthText;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip industrialMusic;
    [SerializeField] private AudioClip enemyDeathClip;
    [SerializeField] private AudioClip interactClip;
    [SerializeField] private AudioClip[] laserClips;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        readyToSpawn = false;
        gracePeriod = true;
        introActive = true;

        text1.enabled = true;
        text2.enabled = false;
        text3.enabled = false;
        text4.enabled = false;
        text5.enabled = false;
        youDiedText.enabled = false;

        playerIron = 0;
        playerCopper = 0;
        playerHealth = 25;
        updateUI();

        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("enemySpawn");
        enemySpawns = new Vector3[taggedObjects.Length];
        for (int i = 0; i < taggedObjects.Length; i++)
        {
            enemySpawns[i] = taggedObjects[i].transform.position;
        }
        StartCoroutine(startGame());
        //tempDevMode();
        StartCoroutine(enemySpawnDelay(enemySpawnTime));

        if (musicSource != null && industrialMusic != null)
        {
            musicSource.clip = industrialMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
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

    public void updateUI()
    {
        if (ironText != null)
        {
            ironText.text = "Iron: " + playerIron.ToString();
        }
        if (copperText != null)
        {
            copperText.text = "Copper: " + playerCopper.ToString();
        }
        if (healthText != null)
        {
            healthText.text = "Health: " + playerHealth.ToString();
        }
    }

    public void addIron(int amount)
    {
        playerIron += amount;
        updateUI();
    }
    public void removeIron(int amount)
    {
        playerIron -= amount;
        updateUI();
    }

    public int getCopper()
    {
        return playerCopper;
    }

    public void addCopper(int amount)
    {
        playerCopper += amount;
        updateUI();
    }

    public void removeCopper(int amount)
    {
        playerCopper -= amount;
        updateUI();
    }

    public int getHealth()
    {
        return playerHealth;
    }

    private bool isRespawning;

    public void playerLoseHealth(int amount)
    {
        if (isRespawning)
        {
            return;
        }
        playerHealth -= amount;
        updateUI();
        if (playerHealth <= 0)
        {
            Debug.Log("You Died");
            StartCoroutine(respawn());
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

    private void tempDevMode()
    {
        if (text1 == null)
        {
            return;
        }
        text1.enabled = false;
        background.enabled = false;
        introActive = false;
    }


    //WORK ON THIS LATER***********************
    IEnumerator startGame()
    {
        yield return new WaitForSeconds(8);
        text1.enabled = false;
        text2.enabled = true;
        yield return new WaitForSeconds(8);
        text2.enabled = false;
        text3.enabled = true;
        yield return new WaitForSeconds(8);
        text3.enabled = false;
        text4.enabled = true;
        yield return new WaitForSeconds(7);
        text4.enabled = false;
        text5.enabled = true;
        yield return new WaitForSeconds(7);
        text5.enabled = false;
        background.enabled = false;
        introActive = false;
    }

    public bool getIntroActive()
    {
        return introActive;
    }

    public void playEnemyDeath()
    {
        if (sfxSource != null && enemyDeathClip != null)
        {
            sfxSource.PlayOneShot(enemyDeathClip);
        }
    }

    public void playInteract()
    {
        if (sfxSource != null && interactClip != null)
        {
            sfxSource.PlayOneShot(interactClip);
        }
    }

    public void playLaser()
    {
        if (sfxSource != null && laserClips != null && laserClips.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, laserClips.Length);
            sfxSource.PlayOneShot(laserClips[index]);
        }
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
    private bool hasWon;

    private void checkWinCondition()
    {
        if (hasWon)
        {
            return;
        }

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
            hasWon = true;
            winGame();
        }
    }

    private void winGame()
    {
        Debug.Log("You've won");
        SceneManager.LoadScene("ThanksForPlaying!");
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

    IEnumerator respawn()
    {
        isRespawning = true;
        charecter playerScript = PlayerRef.GetComponent<charecter>();
        playerScript.setDead(true);
        yield return new WaitForSeconds(5);
        PlayerRef.gameObject.transform.position = respawnArea.transform.position;
        playerHealth = 25;
        updateUI();
        playerScript.setDead(false);
        isRespawning = false;
    }

}