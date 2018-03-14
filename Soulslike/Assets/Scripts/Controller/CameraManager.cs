using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
	public class CameraManager : MonoBehaviour {

		public bool lockOn;
		public float followSpeed = 3;
		public float mouseSpeed = 2;
		public float controllerSpeed = 5;

		public Transform target;
		public EnemyTarget lockOnTarget;
		public Transform lockOnTransform;

		[HideInInspector]
		public Transform pivot;
		[HideInInspector]
		public Transform camTrans;
		StateManager states;

		float turnSmoothing = 0.1f;
		public float minAngle = -35;
		public float maxAngle = 35;

		float smoothX;
		float smoothY;
		float smoothXvelocity;
		float smoothYvelocity;
		public float lookAngle;
		public float tiltAngle;

		bool usedRightAxis;

		public void Init (StateManager st) {
			states = st;
			target = st.transform;

			camTrans = Camera.main.transform;
			pivot = camTrans.parent;
		}

		public void Tick (float d) {
			float h = Input.GetAxis ("Mouse X");
			float v = Input.GetAxis ("Mouse Y");

			float c_h = Input.GetAxis ("RightAxis X");
			float c_v = Input.GetAxis ("RightAxis Y");

			float targetSpeed = mouseSpeed;

			if (lockOnTarget != null) {
				if (lockOnTransform == null) {
					lockOnTransform = lockOnTarget.GetTarget ();
					states.lockOnTransform = lockOnTransform;
				}
				if (Mathf.Abs(c_h) > 0.6f) {
					if (!usedRightAxis) {
						if (c_h > 0) {
							lockOnTransform = lockOnTarget.GetTarget ();
						} else {
							lockOnTransform = lockOnTarget.GetTarget (true);
						}
						states.lockOnTransform = lockOnTransform;
						usedRightAxis = true;
					}
				}
			}

			if (usedRightAxis) {
				if (Mathf.Abs(c_h) < 0.6f) {
					usedRightAxis = false;
				}
			}

			if (c_h != 0 || c_v != 0) {
				h = c_h;
				v = c_v;
				targetSpeed = controllerSpeed;
			}

			FollowTarget (d);
			HandleRotations (d, v, h, targetSpeed);
		}

		void FollowTarget (float d) {
			float speed = d * followSpeed;
			Vector3 targetPosition = Vector3.Lerp (transform.position, target.position, speed);
			transform.position = targetPosition;
		}

		void HandleRotations (float d, float v, float h, float targetSpeed) {
			if (turnSmoothing > 0) {
				smoothX = Mathf.SmoothDamp (smoothX, h, ref smoothXvelocity, turnSmoothing);
				smoothY = Mathf.SmoothDamp (smoothY, v, ref smoothYvelocity, turnSmoothing);
			} else {
				smoothX = h;
				smoothY = v;
			}

			tiltAngle -= smoothY * targetSpeed;
			tiltAngle = Mathf.Clamp (tiltAngle, minAngle, maxAngle);
			pivot.localRotation = Quaternion.Euler (tiltAngle, 0, 0);


			if (lockOn && lockOnTarget != null) {
				Vector3 targetDir = lockOnTransform.position - transform.position;
				targetDir.Normalize ();
//				targetDir.y = 0;

				if (targetDir == Vector3.zero)
					targetDir = transform.forward;
				Quaternion targetRot = Quaternion.LookRotation (targetDir);
				transform.rotation = Quaternion.Slerp (transform.rotation, targetRot, d * 9);
				lookAngle = transform.eulerAngles.y;
				return;
			}
			lookAngle += smoothX * targetSpeed;
			if (transform.rotation.x != 0)
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, lookAngle, 0), d * 9);
			else
				transform.rotation = Quaternion.Euler (0, lookAngle, 0);
		}

		public static CameraManager singleton;
		void Awake () {
			singleton = this;
		}
	}
}