using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreControl : MonoBehaviour
{
    public float secRemain = 10f;
    public float score = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        secRemain -= Time.deltaTime;
        if (secRemain <= 0){
                secRemain = 0;

        }
    }
}
