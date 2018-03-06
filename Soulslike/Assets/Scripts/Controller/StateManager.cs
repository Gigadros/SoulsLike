using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
	public class StateManager : MonoBehaviour {
		[Header("Init")]
		public GameObject activeModel;
		[Header("Inputs")]
		public float vertical;
		public float horizontal;
		public float moveAmount;
		public Vector3 moveDir;
		[Header("Stats")]
		public float moveSpeed = 3.5f;
		public float runSpeed = 5.5f;
		public float rotateSpeed = 9;
		public float toGround = 0.5f;
		[Header("States")]
		public bool onGround;
		public bool run;
		public bool lockon;

		[HideInInspector]
		public Animator anim;
		[HideInInspector]
		public Rigidbody rigid;
		[HideInInspector]
		public float delta;
		[HideInInspector]
		public LayerMask ignoreLayers;

		public void Init() {

			SetupAnimator ();
			rigid = GetComponent<Rigidbody> ();
			rigid.angularDrag = 999;
			rigid.drag = 4;
			rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

			gameObject.layer = 8;
			ignoreLayers = ~(1 << 9);

			anim.SetBool ("onGround", true);
		}

		void SetupAnimator () {
			if (activeModel == null) {
				anim = GetComponentInChildren<Animator> ();
				if (anim == null) {
					Debug.Log ("No model found");
				} else {
					activeModel = anim.gameObject;
				}
			}

			if (anim == null)
				anim = activeModel.GetComponent<Animator> ();
		}

		public void FixedTick (float d) {
			delta = d;

			rigid.drag = (moveAmount > 0 || !onGround) ? 0 : 4;

			float targetSpeed = moveSpeed;
			if (run)
				targetSpeed = runSpeed;

			if (onGround)
				rigid.velocity = moveDir * (targetSpeed * moveAmount);

			if (run)
				lockon = false;

			if (!lockon) {
				Vector3 targetDir = moveDir;
				targetDir.y = 0;
				if (targetDir == Vector3.zero)
					targetDir = transform.forward;
				Quaternion tr = Quaternion.LookRotation (targetDir);
				Quaternion targetRotation = Quaternion.Slerp (transform.rotation, tr, delta * moveAmount * rotateSpeed);
				transform.rotation = targetRotation;
				}

			HandleMovementAnimations ();
		}

		public void Tick (float d) {
			delta = d;
			onGround = OnGround ();
			anim.SetBool ("onGround", onGround);
		}

		void HandleMovementAnimations () {
			anim.SetBool ("run", run);
			anim.SetFloat ("vertical", moveAmount, 0.4f, delta);
		}

		public bool OnGround () {
			bool r = false;

			Vector3 origin = transform.position + (Vector3.up * toGround);
			Vector3 dir = -Vector3.up;
			float dis = toGround + 0.3f;
			RaycastHit hit;
			if (Physics.Raycast (origin, dir, out hit, dis, ignoreLayers)) {
				r = true;
				Vector3 targetPosition = hit.point;
				transform.position = targetPosition;
			}
			return r;
		}
	}
}