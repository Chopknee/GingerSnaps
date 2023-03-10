using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GingerSnaps {
	public class Popup : MonoBehaviour {

		public delegate void Event(Popup popup);
		public Event OnClosed;
		public Event OnCloseBegin;

		public bool bDestroyOnClose = false;

		protected Dugan.TimeAnimation timeAnimation = null;

		protected Dugan.UI.Button[] buttons = null;

		protected virtual void Awake() {
			timeAnimation = gameObject.AddComponent<Dugan.TimeAnimation>();
			timeAnimation.SetLengthInSeconds(0.5f);
			timeAnimation.bUseUnscaledDeltaTime = true;
		}

		public virtual void PostAwake() {
			Dugan.Screen.OnResize += OnResize;
			OnResize();

			buttons = GetComponentsInChildren<Dugan.UI.Button>();
			SetButtonsInteractive(false);//Give the buttons an initial interactive state

			timeAnimation.AnimationUpdateCallback += OnAnimationUpdate;
			timeAnimation.SetDirection(-1, true);
			timeAnimation.AnimationCompleteCallback += OnAnimationCompleteInt;
			timeAnimation.SetDirection(1);
		}

		public void SetDirection(int direction, bool bInstant = false) {
			if (direction < 0 && OnCloseBegin != null)
				OnCloseBegin(this);

			timeAnimation.SetDirection(direction, bInstant);
			SetButtonsInteractive(false);

			OnSetDirection(direction, bInstant);
		}

		protected virtual void OnSetDirection(float direction, bool bInstant) {}

		public void SetButtonsInteractive(bool bInteractive) {
			//
			if (buttons == null) {
				Debug.Log("You probably forgot to run post awake on popup " + gameObject.name + "!");
				return;
			}
			
			for (int i = 0; i < buttons.Length; i++) {
				buttons[i].SetInteractive(bInteractive, -1);
			}
		}

		protected virtual void OnAnimationUpdate() { }

		protected virtual void OnAnimationCompleteInt() {
			if (timeAnimation.GetDirection() == -1) {
				if (OnClosed != null)
					OnClosed(this);

				if (bDestroyOnClose)
					Dugan.PopupManager.Unload(gameObject);
			} else {
				SetButtonsInteractive(true);
			}

			//Force sync transforms? Probably better to move this to the popups which set time scale
			Physics.SyncTransforms();

			OnAnimationComplete();
		}
		protected virtual void OnAnimationComplete() {}

		public virtual void OnSystemBackReleased() {}
		protected virtual void OnResize() {}

		protected virtual void OnDestroy() {
			Dugan.Screen.OnResize -= OnResize;
		}

	}
}