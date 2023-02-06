using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GingerSnaps {
	public class Destructable : MonoBehaviour {

		public delegate void Broken(Destructable destructable);

		public Broken OnBroken;
		public float breakForce = 5f;
		public int scoreValue = 2;
		public bool CatCanBreak = true;
		public bool bDoCatnip = false;

		public GameObject[] plantVariants = null;

		private bool bBroken = false;
		private string strPlayerTag = "Player";

		private ParticleSystem dirtParticles = null;

		private new Rigidbody rigidbody = null;

		private void Awake() {
			rigidbody = GetComponent<Rigidbody>();

			dirtParticles = transform.Find("DirtParticles").GetComponent<ParticleSystem>();
			if (plantVariants != null) {
				int rand = Random.Range(0, plantVariants.Length);
				GameObject plantTop = Instantiate(plantVariants[rand]);
				plantTop.transform.SetParent(transform.Find("PlantSpawn"));
				plantTop.transform.localPosition = Vector3.zero;
				plantTop.transform.localRotation = Quaternion.identity;
				plantTop.transform.localScale = Vector3.one;
			}
		}
		
		private void Update() {
			
		}

		private void OnCollisionEnter(Collision col) {
			if (bBroken)
				return;

			if (col.relativeVelocity.magnitude > breakForce) {
				if (col.gameObject.tag != strPlayerTag || CatCanBreak) {
					bBroken = true;
					DoShatter();
					if (OnBroken != null)
						OnBroken(this);
				}
			}
		}

		private void DoShatter() {
			Destroy(rigidbody);
			dirtParticles.Play();
			Collider[] colliders = GetComponentsInChildren<Collider>();
			for (int i = 0; i < colliders.Length; i++) {
				GameObject go = colliders[i].gameObject;
				if (go.name.Contains("Dirt")) {
					Destroy(go);
					continue;
				}
				Rigidbody rb = go.AddComponent<Rigidbody>();
				rb.AddForce(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized * 1000f);
				go.transform.SetParent(transform.parent);
			}
		}
	}
}