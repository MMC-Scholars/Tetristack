using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets {
	class CBaseTextureToggle : CBaseEntity{
		public Material m_pStartingMaterial;
		public Material m_pToggledMaterial;

		public override void OnUsed(CBaseEntity pUser) {
			base.OnUsed(pUser);
			CBaseController controller = pUser as CBaseController;
			if (controller != null) {
				if (controller.ButtonsPressed().HasButton(g.IN_TRIGGER))
					obj().GetComponent<MeshRenderer>().material = m_pToggledMaterial;
				else
					obj().GetComponent<MeshRenderer>().material = m_pStartingMaterial;
			}
		}

		public override void Start() {
			base.Start();
			obj().GetComponent<MeshRenderer>().material = m_pStartingMaterial;
		}
	}
}
