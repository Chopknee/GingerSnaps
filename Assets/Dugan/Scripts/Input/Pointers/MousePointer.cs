using UnityEngine;

namespace Dugan.Input.Pointers {
	public class MousePointer {
		//For now, there isn't any need to add custom code to the base pointer class. Just static functions for dealing with the cursor updates.

		//Static Functions
		public static Pointer mousePointer = null;

		private static bool bLastPressed = false;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init() {//Only add a mouse pointer if unity can detect a mouse
			if (UnityEngine.InputSystem.Mouse.current != null)
				PointerManager.AddPointerUpdateEvent(UpdateMousePointer);

			mousePointer = new Pointer();
			mousePointer.active = true;
			PointerManager.AddPointer(mousePointer);
		}

		private static int UpdateMousePointer() {
			mousePointer.state = Pointer.ClickState.Hover;//Hover is default state for mouse cursor.
			bool bCurrentPressed = UnityEngine.InputSystem.Mouse.current.leftButton.isPressed;

			if (bCurrentPressed && !bLastPressed)
				mousePointer.state = Pointer.ClickState.Down;
			if (bCurrentPressed && bLastPressed)
				mousePointer.state = Pointer.ClickState.Held;
			if (!bCurrentPressed && bLastPressed)
				mousePointer.state = Pointer.ClickState.Up;

			mousePointer.Update(UnityEngine.InputSystem.Mouse.current.position.ReadValue());

			bLastPressed = bCurrentPressed;

			return 1;
		}
	}
}
