using UnityEngine;
using System.Collections.Generic;

namespace GingerSnaps.Player {
	public class Controller : MonoBehaviour {

		[Header ("Adjustments")]
		public float speed = 8;
		public float groundedAcceleration = 5.5f;
		public float deceleration = 10.0f;
		public float inAirAcceleration = 1.0f;
		public float jumpVelocity = 5f;
		public float downSlopeMaxVelocity = 8;
		public float freefallAcceleration = 5.0f;

		//Ground checking parameters
		public float maxAllowedSlopeAngle = 30f;//If the slope of the ground we're on is greater than this angle, we are not on the ground anymore
		public float maxAllowedGroundHeightOffset = 0.20f;//The maximum height relative to the origin that a contact can be to be counted as ground
		public float maxAllowedLedgeHeightOffset = 0.0f;//The maximum height relative to the origin that a contact can be to be counted as a "valid" ledge hit for climbing up

		public float maxGroundSeparationCheckDistance = 0.5f;

		public float postJumpDelay = 0.1f;
		public LayerMask groundLayerMask = 0;

		public Transform viewpoint = null;

		public float slopeSlideTimeout = 0.15f;

		public bool bAllowDoubleJump = false;

		public bool bInvertVerticalAxis = false;
		public bool bInvertHorizontalAxis = false;

		[Header ("Debugging Values")]

		public bool bTouchingGroundWithFeet = false;
		public bool bFreefalling = false;
		public bool bSlidingDownSlope = false;

		public float currentSlidingTime = 0.0f;
		public bool bTouchingSteepSlope = false;
		public bool bLastSlidingDownSlope = false;

		public bool bJumped = false;
		public bool bJumpedAgain = false;
		public bool bPostJumpDelay = false;
		private float _postJumpDelay = 0.00f;

		public Vector3 lastCollisionNormal = Vector3.zero;
		public Vector3 lastCollisionContactOffset = Vector3.zero;
		public float slopeAngle = 0.0f;

		private Vector3 playerInputDesiredAxis = Vector3.zero;
		private Vector3 slopedPlayerInputDirection = Vector3.zero;
		
		private Collider[] myColliders = null;
		public List<ContactingObject> lastContacts = null;

		private Rigidbody rigidBody = null;

		private bool bApplyJumpVelocity = false;

		public Collider lastClosestObject = null;
		public Vector3 lastClosestObjectPosition = Vector3.zero;
		public Quaternion lastClosestObjectRotation = Quaternion.identity;

		public Collider currentClosestObject = null;

		public Vector3 localVelocity {
			get {
				float mag = rigidBody.velocity.magnitude;
				Vector3 dir = transform.InverseTransformDirection(rigidBody.velocity.normalized);
				return dir * mag;
			}
			set {
				float mag = value.magnitude;
				if (float.IsNaN(mag))
					return;
				// rigidBody.velocity = transform.TransformDirection(value.normalized) * mag;
				rigidBody.AddForce(transform.TransformDirection(value.normalized) * mag - rigidBody.velocity, ForceMode.VelocityChange);
			}
		}

		private void Awake() {
			rigidBody = GetComponent<Rigidbody>();
			myColliders = transform.GetComponentsInChildren<Collider>();
			lastContacts = new List<ContactingObject>();
		}
		
		private void Update() {
			if (bJumped && bTouchingGroundWithFeet && !bFreefalling && !bPostJumpDelay && !bSlidingDownSlope) {//Reallowing the player to move normally after jumping.
				bJumped = false;
				bJumpedAgain = false;
			}

			if (viewpoint == null)
				return;

			//Axis information
			Vector2 mul = new Vector2(bInvertHorizontalAxis? -1.0f : 1.0f, bInvertVerticalAxis? -1.0f : 1.0f);
			Vector2 moveAxis = Input.GetNormalizedVector(Input.moveX.axisValue, Input.moveY.axisValue) * mul;//new Vector2(Input.moveX.axisValue, Input.moveY.axisValue);
			float moveAxisMag = moveAxis.magnitude;

			Vector3 vpForward = viewpoint.forward;
			vpForward.y = 0.0f;
			Vector3  vpRight = viewpoint.right;
			vpRight.y = 0.0f;

			playerInputDesiredAxis = (vpForward * moveAxis.y + vpRight * moveAxis.x).normalized * moveAxisMag;

			if (Input.jump.bPressed)
				OnJumpPressed();

			//Dealing with the post jump delay
			if (bPostJumpDelay) {
				_postJumpDelay += Time.deltaTime;
				if (_postJumpDelay > postJumpDelay)
					bPostJumpDelay = false;
			}

			Vector3 ufv = GetUnderfootVelocity();

			//transform.position += underfootVelocity;
			rigidBody.MovePosition(rigidBody.position + ufv);

			underfootVelocity += ufv * (1.0f / Time.deltaTime);
		}

		private Vector3 underfootVelocity = Vector3.zero;

		//Gets the movement and rotational delta of the underfoot object
		private Vector3 GetUnderfootVelocity() {
			if (currentClosestObject == null) {
				lastClosestObject = null;
				return Vector3.zero;
			}

			if (lastClosestObject == null || lastClosestObject != currentClosestObject) {
				lastClosestObject = currentClosestObject;
				lastClosestObjectPosition = lastClosestObject.transform.position;
				lastClosestObjectRotation = lastClosestObject.transform.rotation;
				return Vector3.zero;
			}

			Vector3 delta = lastClosestObject.transform.position - lastClosestObjectPosition;
			lastClosestObjectPosition = lastClosestObject.transform.position;

			Quaternion rotDelta = lastClosestObject.transform.rotation * Quaternion.Inverse(lastClosestObjectRotation);
			Vector3 localPosition = rigidBody.position - lastClosestObject.transform.position;
			delta += (rotDelta * localPosition) - localPosition;
			lastClosestObjectRotation = lastClosestObject.transform.rotation;
			
			return delta;
		}

		private void FixedUpdate() {
			Vector3 localGravityVector = transform.TransformDirection(Physics.gravity.normalized) * Physics.gravity.magnitude;//Gravity transformed to the local space (such that 'y' is always the vertical axis)
			Vector3 gravityNormalized = localGravityVector.normalized;
			Vector3 currentVelocity = localVelocity;

			//Ground checking
			CheckIfFeetAreTouchingGround(Time.fixedDeltaTime);

			bool bWasFreefalling = bFreefalling;

			if (bTouchingGroundWithFeet)
				bFreefalling = false;

			//Checking if the player is on a slope too steep to move

			if (!bJumped && !bFreefalling && !bSlidingDownSlope && !bTouchingGroundWithFeet) {
				//This specific situation where the player's velocity causes them to fly off an edge, but we'd expect them to stay grounded
				//Also checks if the player is falling (due to them not jumping and walking off a ledge)
				//Debug.DrawRay(rigidBody.position - gravityNormalized * 0.15f, gravityNormalized, Color.red, 0.1f);
				if (Physics.Raycast(rigidBody.position - gravityNormalized * 0.15f, gravityNormalized, out RaycastHit hit2, maxGroundSeparationCheckDistance, groundLayerMask)) {
					//Still not super happy with how this is handled. Causes glitchiness while going up or down steep enough slopes
					if (currentVelocity.y > 0.0f)
						currentVelocity.y = 0.0f;

					Vector3 dir = (hit2.point - rigidBody.position);
					rigidBody.position = rigidBody.position + (dir.normalized * (dir.magnitude * 0.5f));

				} else {
					bFreefalling = true;
				}
			}

			
			//Apply player desired movement axis
			float moveAxisMag = playerInputDesiredAxis.magnitude;
			slopedPlayerInputDirection = Vector3.Cross(playerInputDesiredAxis, lastCollisionNormal);
			slopedPlayerInputDirection = Quaternion.AngleAxis(90.0f, lastCollisionNormal) * slopedPlayerInputDirection;

			if (!bFreefalling && !bSlidingDownSlope) {
				if (bTouchingGroundWithFeet && !bJumped) {
					if (moveAxisMag > 0.0f)
						currentVelocity = Vector3.Lerp(currentVelocity, slopedPlayerInputDirection * speed, Time.fixedDeltaTime * groundedAcceleration);
					else
						currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.fixedDeltaTime * deceleration);
				} else if (bJumped) {
					float yVel = currentVelocity.y;

					if (moveAxisMag > 0.0f)
						currentVelocity = Vector3.Lerp(currentVelocity, playerInputDesiredAxis * speed, Time.fixedDeltaTime * inAirAcceleration);

					currentVelocity.y = yVel;
				}
			} else if (bFreefalling && moveAxisMag > 0.0f) {
				float yVel = currentVelocity.y;
				currentVelocity = Vector3.Lerp(currentVelocity, playerInputDesiredAxis * speed, Time.fixedDeltaTime * inAirAcceleration);
				currentVelocity.y = yVel;
			}

			if (bApplyJumpVelocity) {
				bApplyJumpVelocity = false;
				currentVelocity.y = jumpVelocity;
			}
				

			if (bSlidingDownSlope) {
				Vector3 downSlope = Vector3.Cross(gravityNormalized, lastCollisionNormal);
				downSlope = (Quaternion.AngleAxis(90.0f, lastCollisionNormal) * downSlope).normalized;
				currentVelocity = Vector3.Lerp(currentVelocity, downSlope * downSlopeMaxVelocity, Time.fixedDeltaTime * freefallAcceleration);
			}

			if (currentVelocity.magnitude < 0.0001f)
				currentVelocity = Vector3.zero;

			if (bFreefalling || !bTouchingGroundWithFeet)
				currentVelocity += localGravityVector * Time.fixedDeltaTime;

			// Vector3 vel = GatherUnderfootMovement();
			if (bJumped || bFreefalling && underfootVelocity != Vector3.zero) {
				currentVelocity += underfootVelocity * Time.fixedDeltaTime;
			}

			underfootVelocity = Vector3.zero;

			localVelocity = currentVelocity;
		}

		private void OnJumpPressed() {
			if (bFreefalling)
				return;

			if (bTouchingGroundWithFeet && !bJumped && !bPostJumpDelay) {
				bJumped = true;
				bPostJumpDelay = true;
				_postJumpDelay = 0.0f;
				bApplyJumpVelocity = true;
			} else if (bJumped && !bJumpedAgain && bAllowDoubleJump) {
				bJumpedAgain = true;
				bPostJumpDelay = true;
				_postJumpDelay = 0.0f;
				bApplyJumpVelocity = true;
			}
		}

		private void CheckIfFeetAreTouchingGround(float deltaTime) {

			bTouchingGroundWithFeet = false;
			bSlidingDownSlope = false;
			bTouchingSteepSlope = false;

			//Grab all ground objects
			lastCollisionNormal = Vector3.zero;
			lastCollisionContactOffset = Vector3.zero;
			int validContactCount = 0;

			bool bHasValidGroundContact = false;

			float closestDistance = 0.0f;
			Collider closestCollider = null;

			for (int i = 0; i < lastContacts.Count; i++) {
				for (int j = 0; j < lastContacts[i].contacts.Length; j++) {
					ContactingObject.Contact contact = lastContacts[i].contacts[j];
					if (contact.collider == null)
						continue;

					Vector3 pointLocalPosition = contact.point - rigidBody.position;

					if (pointLocalPosition.y < maxAllowedGroundHeightOffset) {
						bTouchingGroundWithFeet = true;
						validContactCount ++;
						lastCollisionNormal += contact.normal;
						lastCollisionContactOffset += contact.point;

						float slope = Vector3.SignedAngle(Vector3.up, contact.normal, Vector3.up);
						if (slope < maxAllowedSlopeAngle) {
							bHasValidGroundContact = true;

							float dist = (rigidBody.position - contact.point).magnitude;
							if (closestCollider == null) {
								closestCollider = contact.collider;
								closestDistance = dist;
							} else {
								if (dist < closestDistance) {
									closestCollider = contact.collider;
									closestDistance = dist;
								}
							}
								
						}
					}
				}
			}

			currentClosestObject = closestCollider;

			bTouchingSteepSlope = !bHasValidGroundContact && bTouchingGroundWithFeet;

			if (bTouchingSteepSlope) {
				if (currentSlidingTime + deltaTime <= slopeSlideTimeout)
					currentSlidingTime += deltaTime;
				else
					currentSlidingTime = slopeSlideTimeout;
				
			} else {
				if (currentSlidingTime - deltaTime > 0.0f)
					currentSlidingTime -= deltaTime;
				else
					currentSlidingTime = 0.0f;
			}

			bSlidingDownSlope = currentSlidingTime >= slopeSlideTimeout;

			// if (bSlidingDownSlope != bLastSlidingDownSlope) {
			// 	bLastSlidingDownSlope = bSlidingDownSlope;
			// 	if (!bSlidingDownSlope)
			// 		currentSlidingTime = 0.0f;
			// }

			if (validContactCount > 0) {
				lastCollisionNormal.Normalize();//Normalize the normal of the contact surfaces
				lastCollisionContactOffset = lastCollisionContactOffset / (float)validContactCount;//Get the average contact position
				lastCollisionContactOffset = rigidBody.position - lastCollisionContactOffset;//Finally turn it into an offset from the player origin
			}
		}

		private void OnCollisionEnter(Collision collisionInfo) {
			//Prevent duplicate contacts
			for (int i = 0; i < lastContacts.Count; i++) {
				if (lastContacts[i].collider == collisionInfo.collider) {
					return;
				}
			}

			//possibly need to keep track of the contact point to ensure raycasting is correct?
			ContactingObject obj = new ContactingObject();
			obj.collider = collisionInfo.collider;
			for (int i = 0; i < collisionInfo.contactCount; i++) {
				ContactPoint contactPoint = collisionInfo.GetContact(i);
				if (i < obj.contacts.Length) {
					obj.contacts[i].collider = contactPoint.otherCollider;
					obj.contacts[i].normal = contactPoint.normal;
					obj.contacts[i].point = contactPoint.point;
					obj.contacts[i].separation = contactPoint.separation;
				}
			}

			lastContacts.Add(obj);
		}

		private void OnCollisionStay(Collision collisionInfo) {
			//Find the appropriate contact track
			ContactingObject obj = null;
			for (int i = 0; i < lastContacts.Count; i++) {
				if (lastContacts[i].collider == collisionInfo.collider) {
					obj = lastContacts[i];
					break;
				}
			}

			if (obj == null) {
				obj = new ContactingObject();
				obj.collider = collisionInfo.collider;
				lastContacts.Add(obj);
			}
			
			for (int i = 0; i < obj.contacts.Length; i++) {
				obj.contacts[i].collider = null;
				if (i < collisionInfo.contactCount) {
					ContactPoint contactPoint = collisionInfo.GetContact(i);
					obj.contacts[i].collider = contactPoint.otherCollider;
					obj.contacts[i].normal = contactPoint.normal;
					obj.contacts[i].point = contactPoint.point;
					obj.contacts[i].separation = contactPoint.separation;
				}
			}
		}

		private void OnCollisionExit(Collision collisionInfo) {
			//Remove the tracked contact
			for (int i = 0; i < lastContacts.Count; i++) {
				if (lastContacts[i].collider == collisionInfo.collider) {
					lastContacts.RemoveAt(i);
					break;
				}
			}
		}

		private void OnDrawGizmos() {
			if (rigidBody == null)
				return;
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(rigidBody.position, rigidBody.position + rigidBody.velocity);
			Gizmos.color = Color.red;
			Vector3 conPos = rigidBody.position - lastCollisionContactOffset;
			Gizmos.DrawLine(conPos, conPos + lastCollisionNormal);
			Gizmos.DrawWireSphere(conPos, 0.25f);

			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, transform.position + playerInputDesiredAxis);

			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(transform.position, transform.position + slopedPlayerInputDirection);

			Gizmos.color = Color.black;
			Vector3 downSlope = Vector3.Cross(Physics.gravity.normalized, lastCollisionNormal);
			downSlope = Quaternion.AngleAxis(90.0f, lastCollisionNormal) * downSlope;
			Gizmos.DrawLine(rigidBody.position, rigidBody.position + downSlope.normalized);

			for (int i = 0; i < lastContacts.Count; i++) {
				for (int j = 0; j < lastContacts[i].contacts.Length; j++) {
					ContactingObject.Contact obj = lastContacts[i].contacts[j];
					if (obj.collider == null)
						continue;
					Gizmos.color = Color.cyan;
					Gizmos.DrawLine(obj.point, obj.point + obj.normal);
				}
			}
		}

		[System.Serializable]
		public class ContactingObject {
			public Collider collider = null;
			public Contact[] contacts = new Contact[10];

			public ContactingObject() {
				for (int i = 0; i < contacts.Length; i++) {
					contacts[i] = new Contact();
				}
			}

			[System.Serializable]
			public class Contact {
				public Collider collider = null;
				public Vector3 normal = Vector3.zero;
				public Vector3 point = Vector3.zero;
				public float separation = 0.0f;
			}
		}
	}
}
