
namespace Dugan.Mathf {
	public class Easing {

		public enum OverflowMode { None, Clamp, Loop, PingPong }

		public enum EasingType { 
			Linear,
			InQuad, OutQuad, InOutQuad,
			InCubic, OutCubic, InOutCubic,
			InQuart, OutQuart, InOutQuart,
			InSine, OutSine, InOutSine,
			InExpo, OutExpo, InOutExpo,
			InQuint, OutQuint, InOutQuint,
			InCirc, OutCirc, InOutCirc,
			InElastic, OutElastic, InOutElastic,
			InBack, OutBack, InOutBack,
			InBounce, OutBounce, InOutBounce
		};

		public static float Ease(float a, EasingType type, OverflowMode overflowMode = OverflowMode.None) {
			switch (type) {
				case EasingType.InQuad: return EaseInQuad(a, overflowMode);
				case EasingType.OutQuad: return EaseOutQuad(a, overflowMode);
				case EasingType.InOutQuad: return EaseInOutQuad(a, overflowMode);
				case EasingType.InCubic: return EaseInCubic(a, overflowMode);
				case EasingType.OutCubic: return EaseOutCubic(a, overflowMode);
				case EasingType.InOutCubic: return EaseInOutCubic(a, overflowMode);
				case EasingType.InQuart: return EaseInQuart(a, overflowMode);
				case EasingType.OutQuart: return EaseOutQuart(a, overflowMode);
				case EasingType.InOutQuart: return EaseInOutQuart(a, overflowMode);
				case EasingType.InSine: return EaseInSine(a, overflowMode);
				case EasingType.OutSine: return EaseOutSine(a, overflowMode);
				case EasingType.InOutSine: return EaseInOutSine(a, overflowMode);
				case EasingType.InExpo: return EaseInExpo(a, overflowMode);
				case EasingType.OutExpo: return EaseOutExpo(a, overflowMode);
				case EasingType.InOutExpo: return EaseInOutExpo(a, overflowMode);
				case EasingType.InQuint: return EaseInQuint(a, overflowMode);
				case EasingType.OutQuint: return EaseOutQuint(a, overflowMode);
				case EasingType.InOutQuint: return EaseInOutQuint(a, overflowMode);
				case EasingType.InCirc: return EaseInCirc(a, overflowMode);
				case EasingType.OutCirc: return EaseOutCirc(a, overflowMode);
				case EasingType.InOutCirc: return EaseInOutCirc(a, overflowMode);
				case EasingType.InElastic: return EaseInElastic(a, 1.0f, 0.3f, overflowMode);
				case EasingType.OutElastic: return EaseOutElastic(a, 1.0f, 0.3f, overflowMode);
				case EasingType.InOutElastic: return EaseInOutElastic(a, 1.0f, 0.5f, overflowMode);
				case EasingType.InBack: return EaseInBack(a, 1.70158f, overflowMode);
				case EasingType.OutBack: return EaseOutBack(a, 1.70158f, overflowMode);
				case EasingType.InOutBack: return EaseInOutBack(a, 1.70158f, overflowMode);
				case EasingType.InBounce: return EaseInBounce(a, overflowMode);
				case EasingType.OutBounce: return EaseOutBounce(a, overflowMode);
				case EasingType.InOutBounce: return EaseInOutBounce(a, overflowMode);
			}
			
			return Overflow(a, overflowMode);
		}

		public static float EaseInQuad(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			return EaseInQuad(a);
		}

		public static float EaseOutQuad(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			return -a * ( a - 2.0f );
		}

		public static float EaseInOutQuad(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);

			if (a < 0.5f) {
				return 2.0f * a * a;
			} else {
				a = a * 2.0f - 1.0f;
				return -0.5f * ( a * ( a - 2.0f ) - 1.0f);
			}
		}

		public static float EaseInCubic(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			return a * a * a;
		}

		public static float EaseOutCubic (float a, OverflowMode overflowMode = OverflowMode.None ) {
			a = Overflow(a, overflowMode);
			a = a - 1.0f;
			return a * a * a + 1.0f;
		}

		public static float EaseInOutCubic(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			a = a * 2.0f;
			if (a < 1.0f)
				return 0.5f * a * a * a;
			else
				a = a - 2.0f;
				return 0.5f * ( a * a * a + 2.0f );
		}

		public static float EaseInQuart(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			return a * a * a * a;
		}

		public static float EaseOutQuart(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			a = a - 1.0f;
			return -( ( a * a * a * a ) - 1.0f );
		}

		public static float EaseInOutQuart(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			a = 2.0f * a;
			if (a < 1.0f)
				return 0.5f * a * a * a * a;
			else {
				a = a - 2.0f;
				return -0.5f * ( ( a * a * a * a ) - 2.0f );
			}
		}

		public static float EaseInSine(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			return -1.0f * UnityEngine.Mathf.Cos(a * UnityEngine.Mathf.PI / 2.0f) + 1.0f;
		}

		public static float EaseOutSine(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			return UnityEngine.Mathf.Sin(a * UnityEngine.Mathf.PI / 2.0f);
		}

		public static float EaseInOutSine(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			return -0.5f * (UnityEngine.Mathf.Cos(UnityEngine.Mathf.PI * a) - 1.0f );
		}

		public static float EaseInExpo(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			if (a == 0.0f)
				return 0;
			else
				return UnityEngine.Mathf.Pow(2.0f, ( 10.0f * ( a - 1.0f ) ));
		}

		public static float EaseOutExpo(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			if (a == 1.0f)
				return 1.0f;
			else
				return -(UnityEngine.Mathf.Pow(2.0f, ( -10.0f * a )) ) + 1.0f;
		}

		public static float EaseInOutExpo(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			if (a == 0.0f)
				return 0.0f;
			else if (a == 1.0f)
				return 1.0f;
			else {
				if (a < 0.5)
					return UnityEngine.Mathf.Pow(2.0f, 20.0f * a - 10.0f ) * 0.5f;
				else
					return (2.0f - UnityEngine.Mathf.Pow(2.0f, -20.0f * a + 10.0f)) * 0.5f;
			}
		}

		public static float EaseInQuint(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			return a * a * a * a * a;
		}

		public static float EaseOutQuint(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			a = 1.0f - a;
			return 1.0f - ( a * a * a * a * a );
		}

		public static float EaseInOutQuint(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			a = 2.0f * a;
			if (a < 1.0f)
				return 0.5f * a * a * a * a * a;
			else {
				a = a - 2.0f;
				return 0.5f * ( ( a * a * a * a * a ) + 2.0f );
			}
		}

		public static float EaseInCirc(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			return 1.0f - (UnityEngine.Mathf.Sqrt(1.0f - a * a) );
		}

		public static float EaseOutCirc (float a, OverflowMode overflowMode = OverflowMode.None ) {
			a = Overflow(a, overflowMode);
			a = a - 1.0f;
			return (UnityEngine.Mathf.Sqrt(1.0f - a * a) );
		}

		public static float EaseInOutCirc(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			a = a * 2.0f;
			if (a < 1.0f)
				return -0.5f * (UnityEngine.Mathf.Sqrt(1.0f - a * a) - 1.0f );
			else {
				a = a - 2.0f;
				return 0.5f * (UnityEngine.Mathf.Sqrt(1.0f - a * a) + 1.0f);
			}
		}

		public static float EaseInElastic(float a, float amplitude = 1.0f, float period = 0.3f, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);

			if (period == 0.0f) {
				period = 0.3f;
			}
			if (amplitude == 0.0f) {
				amplitude = 1;
			}

			float s;
			if (amplitude < 1.0f) {
				amplitude = 1;
				s = period / 4.0f;
			} else
				s = period / ( 2.0f * UnityEngine.Mathf.PI ) * UnityEngine.Mathf.Asin(1.0f / amplitude);

			a -= 1.0f;
			return -1.0f * ( amplitude * UnityEngine.Mathf.Pow(2.0f, ( 10.0f * a )) * UnityEngine.Mathf.Sin(( a - s ) * ( 2.0f * UnityEngine.Mathf.PI ) / period) );
		}

		public static float EaseOutElastic (float a, float amplitude = 1.0f, float period = 0.3f, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);

			if (period == 0.0f) {
				period = 0.3f;
			}
			if (amplitude == 0.0f) {
				amplitude = 1;
			}

			float s;
			if (amplitude < 1.0f) {
				amplitude = 1.0f;
				s = period / 4.0f;
			} else
				s = period / ( 2.0f * UnityEngine.Mathf.PI ) * UnityEngine.Mathf.Asin(1.0f / amplitude);

			return amplitude * UnityEngine.Mathf.Pow(2.0f, ( -10.0f * a )) * UnityEngine.Mathf.Sin(( a - s ) * ( 2.0f * UnityEngine.Mathf.PI / period )) + 1;
		}

		public static float EaseInOutElastic (float a, float amplitude = 1.0f, float period = 0.5f, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);

			if (period == 0.0f) {
				period = 0.5f;
			}
			if (amplitude == 0.0) {
				amplitude = 1.0f;
			}
			float s;
			if (amplitude< 1.0f) {
				amplitude = 1.0f;
				s = period / 4.0f;
			} else {
				s = period / ( 2.0f * UnityEngine.Mathf.PI ) * UnityEngine.Mathf.Asin(1.0f / amplitude);
			}

			a *= 2.0f;
			if (a< 1.0f) {
				a = a - 1.0f;
				return -0.5f * ( amplitude * UnityEngine.Mathf.Pow(2.0f, ( 10.0f * a )) * UnityEngine.Mathf.Sin(( a - s ) * 2.0f * UnityEngine.Mathf.PI / period) );
			} else {
				a = a - 1.0f;
				return amplitude * UnityEngine.Mathf.Pow(2.0f, ( -10.0f * a )) * UnityEngine.Mathf.Sin(( a - s ) * 2.0f * UnityEngine.Mathf.PI / period) * 0.5f + 1f;
			}
		}

		public static float EaseInBack(float a, float s = 1.70158f, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			if (s == 0.0f)
				s = 1.70158f;
			return a * a * ( ( s + 1.0f ) * a - s );
		}

		public static float EaseOutBack(float a, float s = 1.70158f, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			if (s == 0.0f)
				s = 1.70158f;
			a = a - 1;
			return a * a * ( ( s + 1.0f ) * a + s ) + 1.0f;
		}

		public static float EaseInOutBack(float a, float s = 1.70158f, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			if (s == 0.0f)
				s = 1.70158f;
			a = a * 2.0f;
			if (a < 1.0f) {
				s *= 1.525f;
				return 0.5f * ( a * a * ( ( s + 1.0f ) * a - s ) );
			} else {
				a -= 2.0f;
				s *= 1.525f;
				return 0.5f * ( a * a * ( ( s + 1.0f ) * a + s ) + 2.0f );
			}
		}

		public static float EaseInBounce(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);
			float eob = EaseOutBounce(1.0f - a);
			return 1.0f - eob;
		}

		public static float EaseOutBounce(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);

			if (a < ( 1.0f / 2.75f )) {
				return 7.5625f * a * a;
			} else if (a < ( 2.0f / 2.75f )) {
				a -= ( 1.5f / 2.75f );
				return 7.5625f * a * a + 0.75f;
			} else if (a < ( 2.5f / 2.75f )) {
				a -= ( 2.25f / 2.75f );
				return 7.5625f * a * a + 0.9375f;
			} else {
				a -= ( 2.65f / 2.75f );
				return 7.5625f * a * a + 0.984375f;
			}
		}

		public static float EaseInOutBounce(float a, OverflowMode overflowMode = OverflowMode.None) {
			a = Overflow(a, overflowMode);

			if (a < 0.5f)
				return (1.0f - EaseOutBounce(1 - 2.0f * a)) / 2.0f;
			else
				return (1.0f + EaseOutBounce(2.0f * a - 1)) / 2.0f;
		}

		public static float Overflow(float a, OverflowMode mode) {
			switch (mode) {
				case OverflowMode.Clamp:
					return UnityEngine.Mathf.Clamp(a, 0.0f, 1.0f);
				case OverflowMode.Loop:
					return a - UnityEngine.Mathf.Floor(a);
				case OverflowMode.PingPong:
					if ((UnityEngine.Mathf.Floor(a) % 2) == 0)
						return a - UnityEngine.Mathf.Floor(a);
					else
						return 1.0f - (a - UnityEngine.Mathf.Floor(a));
			}

			return a;
		}

		public static float LerpExtrapolate(float a, float b, float t) {
			return t * b + (1.0f - t) * a;
		}

		public static UnityEngine.Vector2 LerpExtrapolate(UnityEngine.Vector2 a, UnityEngine.Vector2 b, float t) {
			return new UnityEngine.Vector2(LerpExtrapolate(a.x, b.x, t), LerpExtrapolate(a.y, b.y, t));
		}

		public static UnityEngine.Vector3 LerpExtrapolate(UnityEngine.Vector3 a, UnityEngine.Vector3 b, float t) {
			return new UnityEngine.Vector3(LerpExtrapolate(a.x, b.x, t), LerpExtrapolate(a.y, b.y, t), LerpExtrapolate(a.z, b.z, t));
		}
	}
}