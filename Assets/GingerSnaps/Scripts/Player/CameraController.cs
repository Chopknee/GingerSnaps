using UnityEngine;
using UnityEngine.InputSystem;

namespace GingerSnaps.Player {
	public class CameraController : MonoBehaviour {

		//Editable vars
		public float minDistance = 2.0f;
		public float maxDistance = 6.0f;

		public float minDistFromOffset = 0.0f;
		public float maxDistFromOffset = 6.0f;

		public Vector2 axisSensitivity = new Vector2(6.0f, 1.0f);

		private Transform _target = null;
		public Transform target {
			get { return _target; }
			set {
				_target = value;
			}
		}

		public Vector3 targetOffset = new Vector3(0.0f, 0.0f, 0.0f);

		public LayerMask collisionMask;

		//Non-editable vars
		private Vector2 smoothedCameraAxis = Vector2.zero;
		private Vector2 smoothedMoveAxis = Vector2.zero;

		private float vertAxisValue = 0.5f;

		private void Awake() {

		}

		private void Update() {
			if (_target == null)
				return;

			//Positioning first
			Vector3 distance = ((_target.position + targetOffset) - transform.position);
			float distanceMagnitude = distance.magnitude;

			//Player movement of the camera
			Vector2 cameraAxis = new Vector3(Input.cameraX.axisValue, Input.cameraY.axisValue);
			Vector2 moveAxis = Input.GetNormalizedVector(Input.moveX.axisValue, Input.moveY.axisValue);

			smoothedCameraAxis = Vector2.Lerp(smoothedCameraAxis, -cameraAxis, Time.smoothDeltaTime * 10.0f);
			smoothedMoveAxis = Vector2.Lerp(smoothedMoveAxis, -moveAxis, Time.smoothDeltaTime * 10.0f);
			//Vertical
			//delta += new Vector3(0.0f, smoothedInput.y, 0.0f);
			vertAxisValue += smoothedCameraAxis.y * axisSensitivity.y * Time.smoothDeltaTime;
			vertAxisValue = Mathf.Clamp01(vertAxisValue);

			//Apply the vert and horiz movement

			//Horizontal
			Vector3 hDelta = transform.right * smoothedCameraAxis.x * axisSensitivity.x * Time.smoothDeltaTime;//Rotation is handled in a slightly different way than how I'd usually handle it.
			if (hDelta.magnitude < 0.01f)
				hDelta += transform.right * (smoothedMoveAxis.x * 0.75f) * axisSensitivity.x * Time.smoothDeltaTime;

			//Apply movement limits
			
			
			float horizontalDistance = (vertAxisValue * (maxDistance - minDistance)) + minDistance;
			Vector3 horizPos = new Vector3(transform.localPosition.x + targetOffset.x + hDelta.x, 0.0f, transform.localPosition.z + targetOffset.z + hDelta.z).normalized * horizontalDistance;

			float verticalDistance = (vertAxisValue * (maxDistFromOffset - minDistFromOffset)) + minDistFromOffset;
			Vector3 vertPos = new Vector3(0.0f, verticalDistance + targetOffset.y, 0.0f);

			transform.localPosition = horizPos + vertPos;

			//Looking second

			//Recalculate the distance since position updated
			distance = ((_target.position + targetOffset) - transform.position);
			transform.forward = distance.normalized;
		}
	}
}
