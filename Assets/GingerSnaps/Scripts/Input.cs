using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


//Generic setup for dealing with input.

namespace GingerSnaps {
	public class Input : MonoBehaviour {

		public static AggregatedControl moveX = null;
		public static AggregatedControl moveY = null;
		public static AggregatedControl cameraX = null;
		public static AggregatedControl cameraY = null;
		public static AggregatedControl jump = null;

		private Control kbMoveX = null;
		private Control kbMoveY = null;
		private Control kbCameraX = null;
		private Control kbCameraY = null;
		private Control kbJump = null;

		private List<AggregatedControl> aggregatedControls = null;

		private List<TrackedGamepad> trackedGamepads = null;

		private void Awake() {
			trackedGamepads = new List<TrackedGamepad>();

			aggregatedControls = new List<AggregatedControl>();

			moveX = new AggregatedControl();
			aggregatedControls.Add(moveX);
			moveY = new AggregatedControl();
			aggregatedControls.Add(moveY);
			cameraX = new AggregatedControl() {bClamp = false};
			aggregatedControls.Add(cameraX);
			cameraY = new AggregatedControl() {bClamp = false};
			aggregatedControls.Add(cameraY);
			jump = new AggregatedControl();
			aggregatedControls.Add(jump);


			//Mouse and keyboard scheme
			kbMoveX = new Control() { axis = Keyboard.current.dKey, axisNegative = Keyboard.current.aKey };
			moveX.controlAlternatives.Add(kbMoveX);

			kbMoveY = new Control() { axis = Keyboard.current.wKey, axisNegative = Keyboard.current.sKey };
			moveY.controlAlternatives.Add(kbMoveY);

			kbCameraX = new Control() { axis = Mouse.current.delta.x, bUseDeadzone = false, axisSensitivity = 0.5f };
			cameraX.controlAlternatives.Add(kbCameraX);

			kbCameraY = new Control() { axis = Mouse.current.delta.y, bUseDeadzone = false, axisSensitivity = 0.5f };
			cameraY.controlAlternatives.Add(kbCameraY);

			kbJump = new Control() { axis = Keyboard.current.spaceKey };
			jump.controlAlternatives.Add(kbJump);

		}

		private void OnApplicationFocus() {
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update() {
			for (int i = 0; i < trackedGamepads.Count; i++) {
				trackedGamepads[i].bAlive = false;
			}

			//Look for and setup new controllers
			Gamepad[] gamepads = Gamepad.all.ToArray();
			for (int i = 0; i < gamepads.Length; i++) {
				bool untracked = true;

				for (int j = 0; j < trackedGamepads.Count; j++) {
					if (gamepads[i].deviceId == trackedGamepads[j].id) {//Found the gamepad, not adding it to the track list
						untracked = false;
						trackedGamepads[j].bAlive = true;
						break;
					}
				}

				if (untracked) {//Set this new controller up.
					Gamepad currentGamepad = gamepads[i];
					TrackedGamepad gp = new TrackedGamepad(gamepads[i]);
					gp.bAlive = true;
					trackedGamepads.Add(gp);
					//Create a default control shceme
					Control gpMoveX = new Control() { axis = currentGamepad.leftStick.x };
					moveX.controlAlternatives.Add(gpMoveX);
					Control gpMoveY = new Control() { axis = currentGamepad.leftStick.y };
					moveY.controlAlternatives.Add(gpMoveY);
					Control gpCameraX = new Control() { axis = currentGamepad.rightStick.x };
					cameraX.controlAlternatives.Add(gpCameraX);
					Control gpCameraY = new Control() { axis = currentGamepad.rightStick.y };
					cameraY.controlAlternatives.Add(gpCameraY);
					Control gpJump = new Control() { axis = currentGamepad.buttonSouth };
					jump.controlAlternatives.Add(gpJump);

					Debug.Log("Tracking new gamepad " + currentGamepad.name + " " + currentGamepad.deviceId);
				}
			}

			//Check for dead controllers and clean them up
			for (int i = 0; i < trackedGamepads.Count; i++) {
				if (!trackedGamepads[i].bAlive) {
					//Remove this gamepad and kill it.
					Debug.Log("Removing gamepad " + trackedGamepads[i].id);

					for (int j = 0; j < aggregatedControls.Count; j++) {
						aggregatedControls[j].CleanDisconnectedDevice(trackedGamepads[i].id);
					}
					trackedGamepads.RemoveAt(i);
				}
			}

			//Update the aggregations
			for (int i = 0; i < aggregatedControls.Count; i++) {
				aggregatedControls[i].Update();
			}
		}

		private static Input _Instance = null;
		public static Input Instance {get; private set;}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RuntimeInitialization() {
			if (_Instance != null)
				return;

			_Instance = new GameObject("HammyFarmer.Input").AddComponent<Input>();
			DontDestroyOnLoad(_Instance.gameObject);
		}

		public class TrackedGamepad {
			public int id = 0;
			public bool bAlive = true;

			public Gamepad gamepad = null;

			public TrackedGamepad(Gamepad gamepad) {
				this.gamepad = gamepad;
				id = gamepad.deviceId;

			}
		}

		public class AggregatedControl {
			public List<Control> controlAlternatives = new List<Control>();

			public bool bPressed = false;
			public bool bHeld = false;
			public bool bReleased = false;
			public float axisValue = 0.0f;

			public bool bClamp = true;

			public void Update() {
				axisValue = 0.0f;

				for (int i = 0; i < controlAlternatives.Count; i++) {
					controlAlternatives[i].Update();

					axisValue += controlAlternatives[i].axisValue;
				}
				if (bClamp)
					axisValue = Mathf.Max(Mathf.Min(1, axisValue), -1);

				if (axisValue != 0.0f) {
					bReleased = false;

					if (!bHeld && bPressed) {
						bPressed = false;
						bHeld = true;
					}

					if (!bHeld && !bPressed) {
						bPressed = true;
					}
				} else {
					if (bReleased)
						bReleased = false;
						
					if (bHeld || bPressed) {
						bPressed = false;
						bHeld = false;
						bReleased = true;
					}
				}
			}

			public void CleanDisconnectedDevice(int deviceid) {
				for (int i = 0; i < controlAlternatives.Count; i++) {
					if (controlAlternatives[i].axis.device.deviceId == deviceid)
						controlAlternatives.RemoveAt(i);
				}
			}
		}

		public class Control {//Not totally sure how to deal with rebinding yet.

			public UnityEngine.InputSystem.Controls.AxisControl axis = null;
			public UnityEngine.InputSystem.Controls.AxisControl axisNegative = null;

			public float axisValue = 0.0f;

			public float deadzone = 0.05f;
			public float axisSensitivity = 1.0f;

			public bool bUseDeadzone = true;

			public void Update() {
				axisValue = 0.0f;

				if (axis == null)
					return;
				
				if (axis != null && axisNegative == null) {
					axisValue = axis.ReadValue();
				} else {
					axisValue = axis.ReadValue() - axisNegative.ReadValue();
				}

				if (bUseDeadzone)
					axisValue = Dugan.TimeAnimation.GetNormalizedTimeInTimeSlice(Mathf.Abs(axisValue), deadzone, 1.0f - deadzone) * Mathf.Sign(axisValue);
				else
					axisValue = axisValue * axisSensitivity;
			}
		}

		public static Vector2 GetNormalizedVector(float x, float y) {
			Vector2 axis = new Vector2(x, y);
			float mag = axis.magnitude;
			if (mag > 1.0f) {
				axis.x = axis.x / mag;
				axis.y = axis.y / mag;
			}

			//Debug.Log(x + " " + y + " " + mag);
			return axis;
		}
	}
}