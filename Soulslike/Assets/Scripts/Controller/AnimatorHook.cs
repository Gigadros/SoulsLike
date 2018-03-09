using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA{
	public class AnimatorHook : MonoBehaviour {

		Animator anim;
		StateManager states;

		public float rm_muliplier;

		public void Init (StateManager st) {
			states = st;
			anim = st.anim;
		}

		void OnAnimatorMove () {
			if (states.canMove)
				return;

			states.rigid.drag = 0;

			if (rm_muliplier == 0)
				rm_muliplier = 1;

			Vector3 delta = anim.deltaPosition;
			delta.y = 0;
			Vector3 v = (delta *  rm_muliplier) / states.delta;
			states.rigid.velocity = v;
		}
			
	}
}