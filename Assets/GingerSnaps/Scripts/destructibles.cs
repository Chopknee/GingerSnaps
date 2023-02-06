using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destructibles : MonoBehaviour
{
	public float breakForce = 5f;
	public GameObject breakClone;
	public float scoreValue = 2f;
	private GameObject control;

	public bool CatCanBreak = true;

	public bool bDoCatnip = false;

	// Start is called before the first frame update
	void Start()
	{
		control = GameObject.FindWithTag("GameController");
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	void OnCollisionEnter(Collision col)
	{
		// Debug.Log(col.relativeVelocity.magnitude);
		if (col.relativeVelocity.magnitude > breakForce){
			if (col.gameObject.tag != "Player" || CatCanBreak) {
				Instantiate(breakClone,transform.position,transform.rotation);
				Destroy(gameObject);
				control.gameObject.GetComponent<ScoreControl>().score += scoreValue;
				if (bDoCatnip) {
					//Trigger catnip effect
					GingerSnaps.Scenes.Game.Scene.DoSick();
					Debug.Log("SICK MODE");
				}
			}
	   
		}
	
	}
}
