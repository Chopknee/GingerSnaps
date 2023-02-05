using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GingerSnaps.Player {
	public class CharacterModel : MonoBehaviour {

		public Transform model = null;

		private Controller controller = null;

		private void Awake() {
			controller = GetComponent<Controller>();

			if (model == null)
				Debug.Log("Please assign the model in the editor!");
		}

		private void Update() {

		}

	}
}