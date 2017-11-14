using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets {
	abstract partial class g {
		public static CBaseBlock ToBaseBlock(GameObject obj) {
			return obj.GetComponent<CBaseBlock>();
		}
	}

	class CBaseBlock : CBaseEntity {

		public CBaseBlock m_pSource; //block which was used as a template to instantiate this block

		public bool isInstantiated() { return m_pSource != null; }
	}
}
