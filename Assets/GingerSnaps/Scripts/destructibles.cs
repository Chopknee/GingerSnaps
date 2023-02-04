using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destructibles : MonoBehaviour
{
    public float breakForce = 5f;
    public GameObject breakClone;
    // Start is called before the first frame update
    void Start()
    {
        
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
        }
    
    }
}
