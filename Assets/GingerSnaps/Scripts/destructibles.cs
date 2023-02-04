using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destructibles : MonoBehaviour
{
    public float breakForce = 5f;
    public GameObject breakClone;
    public float scoreValue = 2f;
    private GameObject control;
    // Start is called before the first frame update
    void Start()
    {
        control = GameObject.FindWithTag("GameController");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.relativeVelocity.magnitude);
        if (collision.relativeVelocity.magnitude > breakForce){
        Debug.Log("break");
            Instantiate(breakClone,transform.position,transform.rotation);
            Destroy(gameObject);
            control.gameObject.GetComponent<ScoreControl>().score += scoreValue;
        }
    
    }
}
