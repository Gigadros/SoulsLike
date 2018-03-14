using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
	public class InputHandler : MonoBehaviour {

		float vertical, horizontal;
		bool a_input, b_input, x_input, y_input, rb_input, rt_input, lb_input, lt_input;
		float rt_axis, lt_axis;
		bool leftAxis_down, rightAxis_down;

		StateManager states;
		CameraManager camManager;

		float delta;

		void Start () {
			states = GetComponent<StateManager> ();
			states.Init ();

			camManager = CameraManager.singleton;
			camManager.Init (states);
		}

		void Update () {
			delta = Time.deltaTime;
			states.Tick (delta);
		}

		void FixedUpdate () {
			delta = Time.fixedDeltaTime;
			GetInput ();
			UpdateStates ();
			states.FixedTick (delta);
			camManager.Tick (delta);
		}

		void GetInput () {
			vertical = Input.GetAxis ("Vertical");
			horizontal = Input.GetAxis ("Horizontal");
			a_input = Input.GetButton ("a_input");
			b_input = Input.GetButton ("b_input");
			x_input = Input.GetButton ("x_input");
			y_input = Input.GetButtonDown ("y_input");
			rt_input = Input.GetButton ("rt_input");
			rt_axis = Input.GetAxis ("rt_input");
			if (rt_axis != 0)
				rt_input = true;
			lt_input = Input.GetButton ("lt_input");
			lt_axis = Input.GetAxis ("lt_input");
			if (lt_axis != 0)
				lt_input = true;
			rb_input = Input.GetButton ("rb_input");
			lb_input = Input.GetButton ("lb_input");
			rightAxis_down = Input.GetButtonDown ("ra_input");
			leftAxis_down = Input.GetButtonDown ("la_input");
		}

		void UpdateStates () {
			states.vertical = vertical;
			states.horizontal = horizontal;

//			Vector3 v = vertical * camManager.transform.forward;  -changed to fix lock on movement speed bug for targets above or below player
			Vector3 v = vertical * new Vector3 (camManager.transform.forward.x, 0, camManager.transform.forward.z).normalized;
			Vector3 h = horizontal * camManager.transform.right;
			states.moveDir = (v + h).normalized;
			float m = Mathf.Abs (horizontal) + Mathf.Abs (vertical);
			states.moveAmount = Mathf.Clamp01 (m);

			states.rollInput = b_input;

//			if (b_input) {
//				states.isRunning = states.moveAmount > 0;
//				if (states.isRunning) {
//					states.lockOn = false;
//					camManager.lockOn = false;
//				}
//			} else {
//				states.isRunning = false;
//			}

			states.rb = rb_input;
			states.rt = rt_input;
			states.lb = lb_input;
			states.lt = lt_input;

			if (y_input) {
				states.isTwoHanded = !states.isTwoHanded;
				states.HandleTwoHanded ();
			}

			if (rightAxis_down) {
				states.lockOn = !states.lockOn;
				if (states.lockOnTarget == null)
					states.lockOn = false;
				camManager.lockOnTarget = states.lockOnTarget;
				//states.lockOnTransform = camManager.lockOnTransform;
				camManager.lockOn = states.lockOn;
			}
		}
	}
}