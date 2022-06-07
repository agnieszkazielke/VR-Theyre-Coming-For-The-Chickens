using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour
{

    private Animator chickenAnim;

    public bool chickenEaten = false;

    

    // Start is called before the first frame update
    void Start()
    {
        chickenAnim = GetComponent<Animator>();
                
    }


    public void ChickenEaten()
    {
        chickenAnim.SetBool("Dead", true);
        chickenEaten = true;
        transform.position = new Vector3(transform.position.x, 6.55f, transform.position.z);
        GameManager.Instance.RecalculateChickens(gameObject);
        GameManager.Instance.ManageDeadChickenAudio();
             
                
        Invoke("RemoveChicken", 10.0f);


    }

    public void RemoveChicken()
    {
        Destroy(gameObject);

    }    




}
