using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets {
	class CSprite : CBaseEntity {
		public GameObject m_pViewTarget;

		public override void Update() {
			base.Update();
			Vector3 offset = obj().transform.position - m_pViewTarget.transform.position;
			transform.rotation = Quaternion.LookRotation(offset);
		}
	}
}
