using UnityEngine;
using UnityEngine.InputSystem;

namespace GingerSnaps {
	public class GamepadPointer {
		//For now, there isn't any need to add custom code to the base pointer class. Just static functions for dealing with the cursor updates.

		//Static Functions
		public static Dugan.Input.Pointers.Pointer gamepadPointer = null;

		private static bool bLastPressed = false;

		private static Vector2 position = Vector2.zero;
		public static float speed = 1000.0f;

		private static Popups.GamepadCursor.Popup cursorPopup = null;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() {//Only add a mouse pointer if unity can detect a mouse

			Dugan.Input.PointerManager.AddPointerUpdateEvent(UpdateGamepadPointer);
			gamepadPointer = new Dugan.Input.Pointers.Pointer();
			gamepadPointer.active = true;
			Dugan.Input.PointerManager.AddPointer(gamepadPointer);

			cursorPopup = Dugan.PopupManager.Load<Popups.GamepadCursor.Popup>(true);
			cursorPopup.PostAwake();
			// GameObject.DontDestroyOnLoad(cursorPopup.gameObject);
		}

		private static int UpdateGamepadPointer() {
			if (Gamepad.current == null) {
				gamepadPointer.active = false;
				return 0;
			}

			gamepadPointer.active = true;

			Gamepad pad = Gamepad.current;
			Vector2 delta = new Vector2(pad.rightStick.x.ReadValue(), pad.rightStick.y.ReadValue()) * speed * Time.deltaTime;
			position += delta;

			if (position.x < 0)
				position.x = 0;
			if (position.y < 0)
				position.y = 0;
			if (position.x > Dugan.Screen.screenSizeInUnits.x)
				position.x = Dugan.Screen.screenSizeInUnits.x;
			if (position.y > Dugan.Screen.screenSizeInUnits.y)
				position.y = Dugan.Screen.screenSizeInUnits.y;


			gamepadPointer.state = Dugan.Input.Pointers.Pointer.ClickState.Hover;//Hover is default state for mouse cursor.
			bool bCurrentPressed = pad.buttonSouth.isPressed;

			if (bCurrentPressed && !bLastPressed)
				gamepadPointer.state = Dugan.Input.Pointers.Pointer.ClickState.Down;
			if (bCurrentPressed && bLastPressed)
				gamepadPointer.state = Dugan.Input.Pointers.Pointer.ClickState.Held;
			if (!bCurrentPressed && bLastPressed)
				gamepadPointer.state = Dugan.Input.Pointers.Pointer.ClickState.Up;

			gamepadPointer.Update(position);
			
			bLastPressed = bCurrentPressed;

			return 1;
		}

		public static void SetPosition(Vector2 position) {
			GamepadPointer.position = position;
		}
	}
}
