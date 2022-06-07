using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{
    // ENCAPSULATION
    public static GameManager Instance { get; set; }

    public bool isGameActive = false;
    
    [SerializeField]
    private int gameLevel;

    // make sure these are multiples of 3
    private int mediumThreshold = 450;
    private int hardThreshold = 1200;

    // Score (number of chips left)


    private int startScore;
    private int score;
    private int addedScore;
    private int deadChickenIndex;


    private int chickensLeft;
    private int startChickensLeft;


    // Spawning


    [SerializeField]
    private GameObject spawnPrefab;
    private float spawnRate;
    private float changeOverRate = 150.0f;

    // random integer determining where zombies come from at Level 1
    private int spawnArea;

    [SerializeField]
    private List<GameObject> spawnOriginsChapel;
    [SerializeField]
    private List<GameObject> spawnOriginsHouses;
    [SerializeField]
    private List<GameObject> spawnOriginsWindmill;
    [SerializeField]
    private List<GameObject> spawnOriginsWaterTower;



    private List<GameObject> spawnOrigins;

    // Chickens
    
    public List<GameObject> chickens;


    // Player death bool

    public bool playerEaten = false;


    // Full Audio track 

    [SerializeField]
    private GameObject mainAudio;
    [SerializeField]
    private GameObject forestAudio;
    [SerializeField]
    private GameObject walkingAudio;
    [SerializeField]
    private GameObject introAudio;
    [SerializeField]
    private GameObject introNarration;
    [SerializeField]
    private GameObject introWarning;
    [SerializeField]
    private GameObject chickenAudio;
    public GameObject deadChickenAudio;


    public List<AudioClip> deadChickenSounds;


    // Narration

    private AudioSource source;

    private AudioClip currentClip;
    [SerializeField]
    private List<AudioClip> level1_WaterTowerSounds;
    [SerializeField]
    private List<AudioClip> level1_ChapelSounds;
    [SerializeField]
    private List<AudioClip> level1_VillageSounds;
    [SerializeField]
    private List<AudioClip> level1_WindmillsSounds;
    [SerializeField]
    private List<AudioClip> level2_VillageWaterTowerSounds;
    [SerializeField]
    private List<AudioClip> level2_WaterTowerWindmillsSounds;
    [SerializeField]
    private List<AudioClip> level2_WindmillsChapelSounds;
    [SerializeField]
    private List<AudioClip> level2_ChapelVillageSounds;


    // UI

    [SerializeField]
    private GameObject gameMenu;
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject titleText;
    [SerializeField]
    private GameObject mainMenuText;
    [SerializeField]
    private GameObject gameOverText;
    [SerializeField]
    private GameObject mainMenuButton;
    [SerializeField]
    private GameObject goBackButton;

    [SerializeField]
    private GameObject levelTxt;
    [SerializeField]
    private GameObject scoreTxt;
    [SerializeField]
    private GameObject chickenTxt;

    // Player elements

    [SerializeField]
    private GameObject playerXRRig;
    [SerializeField]
    private GameObject bow;
    [SerializeField]
    private GameObject quiver;
    private bool isMoving;
    private bool bowClaimed = false;


    [SerializeField]
    private XRBaseController rightGameController;
    [SerializeField]
    private XRBaseController leftGameController;
    [SerializeField]
    private XRBaseController rightGameControllerMenu;
    [SerializeField]
    private XRBaseController leftGameControllerMenu;
    [SerializeField]
    private GameObject righHandRay;



    // Start is called before the first frame update
    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;
        DontDestroyOnLoad(gameObject);

        
    }

    private void Start()
    {

        Time.timeScale = 1.0f;
        isGameActive = false;
        startChickensLeft = chickens.Count;
        startScore = 0;
        score = startScore;
        chickensLeft = startChickensLeft;
        deadChickenIndex = 0;

        mainMenu.gameObject.SetActive(true);

        // Audio

        source = mainAudio.GetComponent<AudioSource>();
        introNarration.GetComponent<AudioSource>().PlayDelayed(8.0f);

        // activate for testing only
        //StartGame();

    }


    IEnumerator RandomNumberGenerator()
    {
        while (isGameActive)
        {
            spawnArea = Random.Range(0, 4);
            Debug.Log("Spawn Area Change");

            // Audio

            if (gameLevel == 1 )
            {
                // village
                if (spawnArea == 0)
                {
                    currentClip = level1_VillageSounds[Random.Range(0, level1_VillageSounds.Count)];
                }

                // water tower
                if (spawnArea == 1)
                {
                    currentClip = level1_WaterTowerSounds[Random.Range(0, level1_WaterTowerSounds.Count)];
                                       
                }
                // chapel
                if (spawnArea == 2)
                {
                    currentClip = level1_ChapelSounds[Random.Range(0, level1_ChapelSounds.Count)];
                                       
                    
                }
                // windmill - XX
                if (spawnArea == 3)
                {
                    currentClip = level1_WindmillsSounds[Random.Range(0, level1_WindmillsSounds.Count)];
                }

                source.clip = currentClip;
                source.Play();

            }

            if (gameLevel == 2)

            {
                // village & water tower
                if (spawnArea == 0)
                {
                    currentClip = level2_VillageWaterTowerSounds[Random.Range(0, level2_VillageWaterTowerSounds.Count)];

                }

                // water tower & windmills
                if (spawnArea == 1)
                {
                    currentClip = level2_WaterTowerWindmillsSounds[Random.Range(0, level2_WaterTowerWindmillsSounds.Count)];
                }

                // windmills & chapel
                if (spawnArea == 2)
                {
                    currentClip = level2_WindmillsChapelSounds[Random.Range(0, level2_WindmillsChapelSounds.Count)];
                }

                // chapel & village
                if (spawnArea == 3)
                {
                    currentClip = level2_ChapelVillageSounds[Random.Range(0, level2_ChapelVillageSounds.Count)];

                }

                source.clip = currentClip;
                source.Play();


            }

            
            yield return new WaitForSeconds(changeOverRate);
        }
    }

    IEnumerator SpawnRoutine()
    {
        while(isGameActive)
        {
            // At level 1, zombies are spawned in one location for a set period of time, determined by changeOverrate in the RandomNumberGenerator enumerator.
            // Once the enumerator refreshes a new random index, zombies are spawned in a new single location

            if (gameLevel == 1)
            {
                
                // village
                if (spawnArea == 0)
                {
                    int spawnIndex = Random.Range(0, spawnOriginsHouses.Count);

                    Instantiate(spawnPrefab,
                            spawnOriginsHouses[spawnIndex].transform.position, Quaternion.identity);

                }

                // water tower
                if (spawnArea == 1)
                {
                    int spawnIndex = Random.Range(0, spawnOriginsWaterTower.Count);

                    Instantiate(spawnPrefab,
                            spawnOriginsWaterTower[spawnIndex].transform.position, Quaternion.identity);
                }

                // chapel
                if (spawnArea == 2)
                {
                    int spawnIndex = Random.Range(0, spawnOriginsChapel.Count);

                    Instantiate(spawnPrefab,
                            spawnOriginsChapel[spawnIndex].transform.position, Quaternion.identity);
                }

                // windmill
                if (spawnArea == 3)
                {
                    int spawnIndex = Random.Range(0, spawnOriginsWindmill.Count);

                    Instantiate(spawnPrefab,
                            spawnOriginsWindmill[spawnIndex].transform.position, Quaternion.identity);
                }

            }

            // at level 2, zombies are spawned from two neighbouring locations for a set period of time, determined by changeOverrate in the RandomNumberGenerator enumerator.

            if (gameLevel == 2)
            {
                List<GameObject> spawnOriginsLvl2 = new List<GameObject>();

                // village & water tower
                if (spawnArea == 0)
                {
                    spawnOriginsLvl2.AddRange(spawnOriginsHouses);
                    spawnOriginsLvl2.AddRange(spawnOriginsWaterTower);

                }

                // water tower & windmills
                if (spawnArea == 1)
                {
                    spawnOriginsLvl2.AddRange(spawnOriginsWaterTower);
                    spawnOriginsLvl2.AddRange(spawnOriginsWindmill);
                }

                // windmills & chapel
                if (spawnArea == 2)
                {
                    spawnOriginsLvl2.AddRange(spawnOriginsWindmill);
                    spawnOriginsLvl2.AddRange(spawnOriginsChapel);
                }

                // chapel & village
                if (spawnArea == 3)
                {
                    spawnOriginsLvl2.AddRange(spawnOriginsChapel);
                    spawnOriginsLvl2.AddRange(spawnOriginsHouses);
                }

                int spawnIndex = Random.Range(0, spawnOriginsLvl2.Count);

                Instantiate(spawnPrefab,
                        spawnOriginsLvl2[spawnIndex].transform.position, Quaternion.identity);


            }


            // at level 3, zombies are spawned randomly from all locations available in the game

            if (gameLevel == 3)

            {
                int spawnIndex = Random.Range(0, spawnOrigins.Count);


                Instantiate(spawnPrefab,
                            spawnOrigins[spawnIndex].transform.position, Quaternion.identity);




           
            }

            Debug.Log("Zombie Spawned");

            yield return new WaitForSeconds(spawnRate);
        }
    }



    // General game functions

    // ABSTRACTION

    public void StartGame()
    {

        // General


        isGameActive = true;
        gameLevel = 1;
        score = startScore;
        chickensLeft = startChickensLeft;

        // Assemble Spawn locations list

        spawnOrigins = new List<GameObject>();
        spawnOrigins.AddRange(spawnOriginsHouses);
        spawnOrigins.AddRange(spawnOriginsWaterTower);
        spawnOrigins.AddRange(spawnOriginsChapel);
        spawnOrigins.AddRange(spawnOriginsWindmill);

        StartCoroutine(RandomNumberGenerator());
        StartCoroutine(SpawnRoutine());

        // UI

        mainMenu.gameObject.SetActive(false);


        // Audio

        forestAudio.GetComponent<AudioSource>().Play();
        chickenAudio.GetComponent<AudioSource>().Play();
        introAudio.GetComponent<AudioSource>().Stop();
        introWarning.GetComponent<AudioSource>().PlayDelayed(20.0f);


        if (introNarration.GetComponent<AudioSource>().isPlaying)
        {
            introNarration.GetComponent<AudioSource>().Stop();
        }


    }


    public void SendHapticsRH()
    {
        rightGameController.SendHapticImpulse(0.6f, 0.7f);


    }

    public void SendHapticsLH()
    {
        leftGameController.SendHapticImpulse(0.8f, 1.0f);

    }

    public void PlayerMoving()

    {
        isMoving = true;
    }

    public void PlayerStops()

    {
        isMoving = false;
    }

    public void ClaimBow()
    {
        bowClaimed = true;
    }

    public void SetScore(int playerDist)
    {
        addedScore = playerDist;
        score = score + addedScore;

    }


    public void RecalculateChickens(GameObject chicken)
    {
        chickens.Remove(chicken);
        chickensLeft = chickens.Count;
        deadChickenIndex = deadChickenIndex + 1;
        Debug.Log("New chickens count: " + chickensLeft);
    }

    public void ManageDeadChickenAudio()
    {
        source = deadChickenAudio.GetComponent<AudioSource>();
        source.clip = deadChickenSounds[deadChickenIndex];
        source.Play();
    }


    public void PauseGame()
    {
        // Investigate further whether you can pause the game or not
        Time.timeScale = 0;
        gameMenu.gameObject.SetActive(true);

        mainMenuButton.gameObject.SetActive(true);
        goBackButton.gameObject.SetActive(true);

        // Player 

        bow.gameObject.SetActive(false);


        quiver.gameObject.SetActive(true);


        rightGameController.gameObject.SetActive(false);
        rightGameControllerMenu.gameObject.SetActive(true);

        leftGameController.gameObject.SetActive(false);
        leftGameControllerMenu.gameObject.SetActive(true);




    }
    public void ResumeGame()
    {
        
       gameMenu.gameObject.SetActive(false);
       Time.timeScale = 1;

        // Player 

        bow.gameObject.SetActive(true);
        quiver.gameObject.SetActive(true);


        rightGameController.gameObject.SetActive(true);
        rightGameControllerMenu.gameObject.SetActive(false);

        leftGameController.gameObject.SetActive(true);
        leftGameControllerMenu.gameObject.SetActive(false);

    }

 
    public void GameOver()
    {
        isGameActive = false;
        gameMenu.gameObject.SetActive(true);
        goBackButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(true);
        mainMenuText.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
        titleText.gameObject.SetActive(false);

        // Player 

        bow.gameObject.SetActive(false);
        quiver.gameObject.SetActive(false);


        rightGameController.gameObject.SetActive(false);
        rightGameControllerMenu.gameObject.SetActive(true);

        leftGameController.gameObject.SetActive(false);
        leftGameControllerMenu.gameObject.SetActive(true);


        // Audio

        forestAudio.GetComponent<AudioSource>().Stop();
        chickenAudio.GetComponent<AudioSource>().Stop();
    }




    // Update is called once per frame
    void Update()
    {

        // Start game by grabbing the bow

        if (bowClaimed && !isGameActive)
        {
            StartGame();
        }




        if (Time.timeScale == 1)

        // Display current score 

        {
            scoreTxt.GetComponent<TMPro.TextMeshProUGUI>().text = "Score: " + score.ToString();

            chickenTxt.GetComponent<TMPro.TextMeshProUGUI>().text = "Chickens: " + chickensLeft.ToString();

            levelTxt.GetComponent<TMPro.TextMeshProUGUI>().text = "Level: " + gameLevel.ToString();
        }

        // Change difficulty level and spawn rate based on score

        if (score < mediumThreshold)
        {
            gameLevel = 1;

            // Spawn rate rules

            if (score < mediumThreshold / 3)
            {
                spawnRate = 12.0f;
            }

            if (score > mediumThreshold / 3 && score < mediumThreshold / 1.5f)
            {
                spawnRate = 8.0f;
            }

            if (score > mediumThreshold / 1.5f)
            {
                spawnRate = 5.0f;
            }


        }

        if (score >= mediumThreshold && score < hardThreshold)
        {
            gameLevel = 2;

            // Spawn rate rules

            if (score < hardThreshold / 1.5)
            {
                spawnRate = 10.0f;
            }

            if (score > hardThreshold / 1.5)
            {
                spawnRate = 5.0f;
            }



        }

        if (score >= hardThreshold)
        {
            gameLevel = 3;
            spawnRate = 5.0f;
        }


        // Audio - walking


        if (isMoving && !walkingAudio.GetComponent<AudioSource>().isPlaying)
        {
            walkingAudio.GetComponent<AudioSource>().Play();
        }

        if (!isMoving && walkingAudio.GetComponent<AudioSource>().isPlaying)
        {
            walkingAudio.GetComponent<AudioSource>().Stop();
        }


        // Game Over triggers

        // When chicken no. goes down to zero, game ends
        // When player eaten, game ends
        if (isGameActive)
        {
                    if (chickensLeft == 0 || playerEaten)
        {
            GameOver();
        }

        }




    }


}
