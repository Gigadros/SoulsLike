using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA{
	public class AnimatorHook : MonoBehaviour {

		Animator anim;
		StateManager states;

		public float rm_muliplier;
		bool rolling;
		float roll_t;
		public AnimationCurve rollCurve;

		public void Init (StateManager st) {
			states = st;
			anim = st.anim;
			rollCurve = states.roll_curve;
		}

		public void InitForRoll () {
			rolling = true;
			roll_t = 0;
		}

		public void CloseRoll () {
			if (!rolling)
				return;
			rm_muliplier = 1;
			rolling = false;
		}

		void OnAnimatorMove () {
			if (states.canMove)
				return;

			states.rigid.drag = 0;

			if (rm_muliplier == 0)
				rm_muliplier = 1;

			if (!rolling) {
				Vector3 delta = anim.deltaPosition;
				delta.y = 0;
				Vector3 v = (delta * rm_muliplier) / states.delta;
				states.rigid.velocity = v;
			} else {
				roll_t += Time.deltaTime / 0.6f;
				if (roll_t > 1)
					roll_t = 1;
				float zValue = rollCurve.Evaluate (roll_t);
				Vector3 v1 = Vector3.forward * zValue;
				Vector3 relative = transform.TransformDirection (v1);
				Vector3 v2 = (relative * rm_muliplier);
				states.rigid.velocity = v2;
			}
		}
			
	}
}