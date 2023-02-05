using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GingerSnaps.Player {
	public class CharacterModel : MonoBehaviour {

		public Transform model = null;

		private Controller controller = null;

		private void Awake() {

			if (model == null)
				Debug.Log("Please assign the model in the editor!", gameObject);
		}

		private void Start() {
			controller = GetComponent<Controller>();
		}

		private void Update() {
			if (controller == null || model == null)
				return;

			if (controller.localVelocity.magnitude > 0.01f) {
				model.forward = controller.localVelocity;
			}
		}

	}
}