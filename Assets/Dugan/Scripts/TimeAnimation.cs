using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan {
	public class TimeAnimation : MonoBehaviour {
		
		public delegate void AnimationEvent();
		public AnimationEvent AnimationCompleteCallback;
		public AnimationEvent SetDirectionCallback;
		public AnimationEvent AnimationUpdateCallback;

		protected float seconds = 0;

		protected int direction = 1;

		protected bool bPaused = false;
		protected bool bComplete = false;

		protected float alpha = 0;

		protected bool bPlaying = false;

		public bool bUseUnscaledDeltaTime = false;

		public void SetPaused(bool bValue) {
			bPaused = bValue;
		}

		public bool GetPaused() {
			return bPaused;
		}

		public bool GetComplete() {
			return bComplete;
		}

		public bool IsPlaying() {
			return bPlaying;
		}

		public void SetDirection(int value, bool bInstant = false) {
			int newDir = (int)UnityEngine.Mathf.Sign(value);
				
			direction = newDir;
			bComplete = bInstant;
			bPlaying = true;

			if (SetDirectionCallback != null)
				SetDirectionCallback();

			if (bInstant) {
				if (direction == 1)
					alpha = 1;
				else if (direction == -1)
					alpha = 0;

				if (AnimationUpdateCallback != null)
					AnimationUpdateCallback();

				bPlaying = false;
				if (AnimationCompleteCallback != null) {
					AnimationCompleteCallback();
				}
			}
		}

		public int GetDirection() {
			return direction;
		}

		public float GetTime() {
			return alpha * seconds;
		}

		public void SetLengthInSeconds(float value) {
			seconds = value;
		}

		public float GetLengthInSeconds() {
			return seconds;
		}

		public void SetNormalizedTime(float value) {
			alpha = UnityEngine.Mathf.Clamp(value, 0, 1);
		}

		public float GetNormalizedTime() {
			return alpha;
		}

		protected virtual void Update() {
			ManualUpdate();
		}

		public void ManualUpdate() {
			if (!bPaused && !bComplete) {
				float deltaTime = bUseUnscaledDeltaTime? Time.unscaledDeltaTime : Time.deltaTime;

				alpha += direction * (deltaTime / seconds);

				float sign = UnityEngine.Mathf.Sign(direction);

				if (sign == 1) {
					if (alpha >= 1) {
						alpha = 1;
						bComplete = true;
					}
				} else if (sign == -1) {
					if (alpha <= 0) {
						alpha = 0;
						bComplete = true;
					}
				}

				if (AnimationUpdateCallback != null)
					AnimationUpdateCallback();

				if (bComplete) {
					bPlaying = false;
					if (AnimationCompleteCallback != null)
						AnimationCompleteCallback();
				}

			}
		}

		public static float GetNormalizedTimeInTimeSlice(float currentTime, float timeSliceStart, float timeSliceLength) {
			if (currentTime <= 0)
				return 0.0f;
			if (timeSliceLength <= 0)
				return 1.0f;

			if (timeSliceStart <= 0)
				timeSliceStart = 0.0f;

			float timeSliceCurrentTime = currentTime - timeSliceStart;
			if (timeSliceCurrentTime <= 0)
				return 0.0f;
			if (timeSliceCurrentTime >= timeSliceLength)
				return 1.0f;

			return timeSliceCurrentTime / timeSliceLength;
		}
	}
}