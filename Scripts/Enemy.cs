using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.AI;

public class Enemy: MonoBehaviour, IArrowHittable
{


    private GameObject player;

    // Nav mesh travel
    private NavMeshAgent agent;
    private GameObject targetChicken;
    private Vector3 roughDirection;

    [SerializeField]
    private bool destinationReached = false;


    // Audio

    [SerializeField]
    private List<AudioClip> audioClips;
    private AudioClip currentClip;
    private AudioSource source;

    private float minWaitBetweenPlays = 1f;
    private float maxWaitBetweenPlays = 5f;
    private float waitTimeCountdown = -1f;

    // Target

    private float forceAmount = 1.0f;
    [SerializeField]
    private GameObject zombieBody;
    private Animator zombieAnim;
    private bool zombieShot = false;


    // Material

    [SerializeField]
    private SkinnedMeshRenderer[] meshMaterials;
    [SerializeField]
    private Material transMaterial;

    private float fadeSpeed = 0.5f;
    private bool fadeOut = false;

    // Player Interaction

    private float playerDistance;
    private bool playerClose;
    


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        zombieAnim = zombieBody.GetComponent<Animator>();

        // Navigation

        agent = GetComponent<NavMeshAgent>();
        


        // Direct zombie to a random existing chicken

        targetChicken = GameManager.Instance.chickens[Random.Range(0, (GameManager.Instance.chickens.Count))];
        agent.SetDestination(targetChicken.transform.position);
        roughDirection = targetChicken.transform.position - transform.position;
        

        // Audio

        source = GetComponent<AudioSource>();


        // Fade Out: Get all rendered components for various body parts
        meshMaterials = GetComponentsInChildren<SkinnedMeshRenderer>();



    }

    public void Hit(Arrow arrow)


    {
        
        ApplyForce(arrow.transform.forward);


        // Animate Death
        
        zombieAnim.SetBool("shot", true);
        zombieShot = true;
        GameManager.Instance.SetScore((int)playerDistance);

        Invoke("FadeZombieTrigger", 3.0f);


        // Message 

        Debug.Log("Zombie shot!");

        // Haptics

        GameManager.Instance.SendHapticsLH();

    }


    private void ApplyForce(Vector3 direction)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(direction * forceAmount);
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

 
    public void CheckDestinationReached()
    {
        // Check if we've reached the destination - applies only to chickens

        if (transform.position.x < 20 && transform.position.x > 0)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        destinationReached = true;
                    }
                }

                else
                {
                    destinationReached = false;
                }
            }

            else
            {
                destinationReached = false;
            }
        }

    }

    public void CheckPlayerNear()

    {
        playerDistance = Vector3.Distance(transform.position, player.transform.position);

        if (playerDistance < 5.0f)
        {
            playerClose = true;
        }

        else
        {
            playerClose = false;
        }

    }


    private void SetPlayerDestination()
    {
        agent.SetDestination(player.transform.position);
        agent.speed = 3.0f;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            zombieAnim.SetBool("attack", true);
            Invoke("PlayerEaten", 4.0f);
        }
    }

    private void PlayerEaten()
    {
        GameManager.Instance.playerEaten = true;
        Debug.Log("Zombie has eaten you lol");
    }


    // Update is called once per frame
    void Update()
    {
        
        // HEADING 1: Zombie shot and dying

        if (zombieShot)
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

        if (!zombieShot)



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

            // move the zombie away from the chicken / player (WIP)
            //transform.Translate(roughDirection.normalized.x, 0, -roughDirection.normalized.z);
            agent.isStopped = true;
            targetChicken.GetComponent<Chicken>().ChickenEaten();

            destinationReached = false;


        }








        // Check if target chicken has been eaten and if so redirect

        if (targetChicken == null)
        {
            targetChicken = GameManager.Instance.chickens[Random.Range(0, (GameManager.Instance.chickens.Count))];


            agent.SetDestination(targetChicken.transform.position);
            roughDirection = targetChicken.transform.position - transform.position;

        }


        if (!GameManager.Instance.isGameActive)
        {
            Destroy(gameObject);
        }



        
    }


}