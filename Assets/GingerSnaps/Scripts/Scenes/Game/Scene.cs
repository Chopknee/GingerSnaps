using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GingerSnaps.Scenes.Game {
	public class Scene : MonoBehaviour {

		private new Camera camera = null;

		private Player.Controller player = null;
		private Player.CameraController playerCamera = null;

		private void Awake() {
			player = transform.Find("Player").gameObject.AddComponent<Player.Controller>();
			playerCamera = new GameObject("PlayerCamera").AddComponent<Player.CameraController>();
			playerCamera.transform.SetParent(player.transform);
			playerCamera.target = player.transform;
			playerCamera.targetOffset = new Vector3(0.0f, 1.75f, 0.0f);
			playerCamera.collisionMask = 1 << 0;//Default layer

			player.viewpoint = playerCamera.transform;
			player.groundLayerMask = playerCamera.collisionMask;

			camera = transform.Find("Camera").GetComponent<Camera>();

			camera.transform.SetParent(playerCamera.transform);
			camera.transform.localPosition = Vector3.zero;
			camera.transform.localScale = Vector3.one;
			camera.transform.localRotation = Quaternion.identity;
		}

	}
}