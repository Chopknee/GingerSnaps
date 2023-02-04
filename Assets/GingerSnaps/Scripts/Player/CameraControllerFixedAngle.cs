using UnityEngine;
using UnityEngine.InputSystem;

namespace GingerSnaps.Player {
	public class CameraControllerFixedAngle : MonoBehaviour {

		public Vector3 forwardAngle = Vector3.forward;

		public float followDistance = 3.0f;

		private Transform _target = null;
		public Transform target {
			get { return _target; }
			set {
				_target = value;
			}
		}

		private void Awake() {

		}

		private void Update() {
			if (_target == null)
				return;

			Vector3 targetPosition = _target.position - forwardAngle * followDistance;
			transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10.0f);
		}
	}
}
