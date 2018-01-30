using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets {
	class CSprite : CBaseEntity {
		public GameObject m_pViewTarget;

		public override void Update() {
			base.Update();
			transform.rotation = GetAngleTo(m_pViewTarget.transform.position);
		}
	}
}
