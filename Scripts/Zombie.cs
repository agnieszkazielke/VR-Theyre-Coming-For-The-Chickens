using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.AI;

public class Zombie: Enemy
{


    // Audio

    [SerializeField]
    private List<AudioClip> audioClips;
    private AudioClip currentClip;
    private AudioSource source;

    private float minWaitBetweenPlays = 1f;
    private float maxWaitBetweenPlays = 5f;
    private float waitTimeCountdown = -1f;

    // Target

  
    [SerializeField]
    private GameObject zombieBody;
    private Animator zombieAnim;


    // Material

    [SerializeField]
    private SkinnedMeshRenderer[] meshMaterials;
    [SerializeField]
    private Material transMaterial;

    private float fadeSpeed = 0.5f;
    private bool fadeOut = false;

  


    // Start is called before the first frame update
    void Start()
    {
        
        zombieAnim = zombieBody.GetComponent<Animator>();
        player = GameObject.Find("Player");

        agent = GetComponent<NavMeshAgent>();

        // Direct zombie to a random existing chicken

        SetEnemyDestination();

        

        // Audio

        source = GetComponent<AudioSource>();


        // Fade Out: Get all rendered components for various body parts
        meshMaterials = GetComponentsInChildren<SkinnedMeshRenderer>();



    }

    override public void Hit(Arrow arrow)


    {

        // Animate Death

        zombieAnim.SetBool("shot", true);

        // Fade zombie

        Invoke("FadeZombieTrigger", 3.0f);

        base.Hit(arrow);

    }




    private void FadeZombieTrigger()
    {

            
        // Change material to a non-texture one 
        foreach (SkinnedMeshRenderer meshmat in meshMaterials)
        {
            meshmat.material = transMaterial;
            
        }

        fadeOut = true;

    }




    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            zombieAnim.SetBool("attack", true);
            Invoke("PlayerEaten", 4.0f);
        }
    }




    // Update is called once per frame
    void Update()
    {
        
        // HEADING 1: Zombie shot and dying

        if (enemyShot)
        {
            agent.isStopped = true;
        }

        // Fade Out Script triggered by RemoveZombie function

        if (fadeOut)

        {
            foreach (SkinnedMeshRenderer meshmat in meshMaterials)
            {
                Color objectColor = meshmat.material.color;
                float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                meshmat.material.color = objectColor;

                if (objectColor.a <= 0)
                {
                    fadeOut = false;
                    Destroy(gameObject);
                }
            }
        }

        // HEADING 2: Zombie alive navigating to destination

        if (!enemyShot)



        {
                        // Audio - Play randomised sounds

            if (!source.isPlaying)
            {
                if (waitTimeCountdown < 0f)
                {
                    currentClip = audioClips[Random.Range(0, audioClips.Count)];
                    source.clip = currentClip;
                    source.Play();
                    waitTimeCountdown = Random.Range(minWaitBetweenPlays, maxWaitBetweenPlays);
                }
                else
                {
                    waitTimeCountdown -= Time.deltaTime;
                }
            }

            if (!agent.isStopped)

            {
                // Measure distance between zombie and player
                CheckPlayerNear();

                // Check for zombie reaching destination (chickens)
                CheckDestinationReached();
            }



        }



        if (playerClose)
            {
                // If player gets too close, zombie will try to attack it - need separate function?

                SetPlayerDestination();
            }


        if (destinationReached)
        {
            // Animate zombie attack
            zombieAnim.SetBool("attack", true);


            agent.isStopped = true;
            targetChicken.GetComponent<Chicken>().ChickenEaten();

            destinationReached = false;


        }








        // Check if target chicken has been eaten and if so redirect

        if (targetChicken == null)
        {
            targetChicken = GameManager.Instance.chickens[Random.Range(0, (GameManager.Instance.chickens.Count))];


            agent.SetDestination(targetChicken.transform.position);
            

        }


        if (!GameManager.Instance.isGameActive)
        {
            Destroy(gameObject);
        }



        
    }


}