using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.AI;

public class Enemy: MonoBehaviour, IArrowHittable
{


    protected GameObject player;

    // Nav mesh travel
    protected NavMeshAgent agent;
    protected GameObject targetChicken;
    

    [SerializeField]
    protected bool destinationReached = false;




    // Target

    protected float forceAmount = 1.0f;
    protected bool enemyShot = false;




    // Player Interaction

    protected float playerDistance;
    protected bool playerClose;
    


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        // Navigation

        agent = GetComponent<NavMeshAgent>();
        
       
    }


    protected void SetEnemyDestination()
    {
        targetChicken = GameManager.Instance.chickens[Random.Range(0, (GameManager.Instance.chickens.Count))];
        agent.SetDestination(targetChicken.transform.position);

    }

    virtual public void Hit(Arrow arrow)


    {
        
        ApplyForce(arrow.transform.forward);

        enemyShot = true;
        GameManager.Instance.SetScore((int)playerDistance);
      
        // Message 

        Debug.Log("Enemy shot!");

        // Haptics

        GameManager.Instance.SendHapticsLH();

    }


    protected void ApplyForce(Vector3 direction)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(direction * forceAmount);
    }




    protected void CheckDestinationReached()
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

    protected void CheckPlayerNear()

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


    protected void SetPlayerDestination()
    {
        agent.SetDestination(player.transform.position);
        agent.speed = 3.0f;
    }


    protected virtual void OnTriggerEnter(Collider other)
    {

    }

    protected void PlayerEaten()
    {
        GameManager.Instance.playerEaten = true;
        Debug.Log("Enemy has eaten you lol");
    }


    



}