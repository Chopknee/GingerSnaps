using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreControl : MonoBehaviour
{
    public float secRemain = 10f;
    public float score = 0f;
    private float finalScore = -1f;
    private float displayScore = 0;

    public GameObject status;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        secRemain -= Time.deltaTime;
        if (secRemain <= 0f){
            if (finalScore == -1){
                finalScore = score;
            }
                ///secRemain = 0;
                status.SetActive(true);
                if (displayScore < finalScore){
                    displayScore ++;
                }

                status.GetComponent<TMPro.TextMeshProUGUI>().text = "Final Score: " + displayScore.ToString();
              if (secRemain <= -10f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }  
        
        }
        
    }
}
