using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dugan {
	public class TimeAnimation : MonoBehaviour {
		
		public delegate void SetDirectionDelegate();
		public SetDirectionDelegate SetDirectionCallback;
		
		public delegate void AnimationUpdateDelegate();
		public AnimationUpdateDelegate AnimationUpdateCallback;
		
		public delegate void AnimationCompleteDelegate();
		public AnimationCompleteDelegate AnimationCompleteCallback;
		
		public enum Type { Default, Loop, PingPong };
		[HideInInspector]
		public Dugan.TimeAnimation.Type type = Dugan.TimeAnimation.Type.Default;
		
		[HideInInspector]
		public bool bUseUnscaledDeltaTime = false;
		
		protected bool bManualUpdate = false;
		
		protected float normalizedVelocity = 1.0f;
		
		protected int direction = -1;
		protected float normalizedTime = 0.0f;
		protected bool bAnimating = false;
		
		protected float lengthInSeconds = 0.0f;
		
		protected bool bPaused = false;
		
		public virtual void Update() {
			if (!bManualUpdate)
				ManualUpdate();
		}
		
		public virtual void OnDestroy() {
			SetDirectionCallback = null;
			AnimationUpdateCallback = null;
			AnimationCompleteCallback = null;
		}
		
		public virtual void ManualUpdate() {
			if (!bAnimating)
				return;
			
			if (bPaused)
				return;
			
			bool bComplete = false;
			
			float deltaTime = 0.0f;
			
			if (bUseUnscaledDeltaTime)
				deltaTime = Time.unscaledDeltaTime;
			else
				deltaTime = Time.deltaTime;
			
			normalizedTime += deltaTime * (float)direction * normalizedVelocity;
			
			if (type == Dugan.TimeAnimation.Type.Default) {
				if ((direction < 0 && normalizedTime <= 0) || (direction > 0 && normalizedTime >= 1))
					bComplete = true;
			}
			else if (type == Dugan.TimeAnimation.Type.Loop) {
				while ((direction < 0 && normalizedTime < 0) || (direction > 0 && normalizedTime > 1)) {
					normalizedTime -= (float)direction;
				}
			}
			else if (type == Dugan.TimeAnimation.Type.PingPong) {
				while ((direction < 0 && normalizedTime <= 0) || (direction > 0 && normalizedTime >= 1)) {
					if (normalizedTime < 0.5f)
						normalizedTime = -normalizedTime;
					else
						normalizedTime = 1.0f - (normalizedTime - 1.0f);
					direction *= -1;
				}
			}
			
			normalizedTime = UnityEngine.Mathf.Clamp(normalizedTime, 0.0f, 1.0f);
			
			if (bComplete) {
				OnComplete();
				return;
			}
			
			if (AnimationUpdateCallback != null)
				AnimationUpdateCallback();
		}
		
		public virtual void Resume() {
			bPaused = false;
		}
		
		public virtual void Pause() {
			bPaused = true;
		}
		
		public virtual void Stop() {
			OnComplete();
		}
		
		protected virtual void OnComplete() {
			bAnimating = false;
			bPaused = false;
			
			if (!bManualUpdate)
				enabled = false;
			
			if (AnimationUpdateCallback != null)
				AnimationUpdateCallback();
			
			if (AnimationCompleteCallback != null)
				AnimationCompleteCallback();
		}
		
		public virtual void ResetTime() {
			normalizedTime = 0.0f;
			
			if (AnimationUpdateCallback != null)
				AnimationUpdateCallback();
			
			if (AnimationCompleteCallback != null)
				AnimationCompleteCallback();
		}
		
		public virtual void SetNormalizedVelocity(float value) {
			if (value <= 0) {
				value = 0.0f;
				lengthInSeconds = 0.0f;
			}
			else {
				normalizedVelocity = value;
				lengthInSeconds = 1.0f / value;
			}
		}
		
		public virtual void SetLengthInSeconds(float value) {
			if (value <= 0) {
				value = 0.0f;
				SetNormalizedVelocity(value);
			}
			else {
				SetNormalizedVelocity(1.0f / value);
			}
			lengthInSeconds = value;
		}
		
		public virtual void SetDirection(int value, bool bInstant = false) {
			if (value <= 0)
				value = -1;
			else
				value = 1;
			
			if (value == direction) {
				//if (!bAnimating)
					//return;
				if (!bInstant)
					return;
			}
			
			direction = value;
			bAnimating = !bInstant;
			
			bPaused = false;
			
			if (bAnimating) {
				if (!bManualUpdate)
					enabled = true;
			}
			else {
				if (direction > 0)
					normalizedTime = 1.0f;
				else
					normalizedTime = 0.0f;
			}	
			
			if (SetDirectionCallback != null)
				SetDirectionCallback();
			
			if (bAnimating) {
				if (AnimationUpdateCallback != null)
					AnimationUpdateCallback();
			}
			else {
				OnComplete();
			}
		}
		
		public void SetManualUpdate(bool bValue) {
			bManualUpdate = bValue;
			enabled = !bManualUpdate && bAnimating;
		}
		
		public void SetType(Dugan.TimeAnimation.Type value) {
			type = value;
		}

		public void SetNormalizedTime(float time) {
			normalizedTime = time;
		}
		
		public void SetTimeInSeconds(float value) {
			value = UnityEngine.Mathf.Clamp(value, 0.0f, lengthInSeconds);
			if (lengthInSeconds > 0)
				normalizedTime = value / lengthInSeconds;
			else
				normalizedTime = 0.0f;
		}
		
		public int GetDirection() {
			return direction;
		}
		
		public bool IsAnimating() {
			return bAnimating;
		}
		
		public bool IsPaused() {
			return bPaused;
		}
		
		public float GetNormalizedTime() {
			return normalizedTime;
		}
		
		public float GetLengthInSeconds() {
			return lengthInSeconds;
		}
		
		public float GetTime() {
			return normalizedTime * lengthInSeconds;
		}
		
		public float GetNormalizedVelocity() {
			return normalizedVelocity;
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