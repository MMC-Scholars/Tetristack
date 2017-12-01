using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.NTowerBuilder {

	abstract partial class g {
		CFallTrigger pFallTrigger = null;
	}

	/**
	 * Communicates to the gamerules how many blocks fall off the platform.
	 */ 
	class CFallTrigger : CBaseEntity {

		private int m_iCount = 0;

		public int FallenBlockCount() { return m_iCount; }

		/************************************************************
		 * Unity overrides
		 ***********************************************************/ 
		private void OnCollisionEnter(Collision collision) {
			CBaseBlock blk = collision.collider.gameObject.GetComponent<CBaseBlock>();
			if (blk != null && blk.isInstantiated()) {
				m_iCount++;
			}
		}

		private void OnCollisionExit(Collision collision) {
			CBaseBlock blk = collision.collider.gameObject.GetComponent<CBaseBlock>();
			if(blk != null && blk.isInstantiated()) {
				CBaseBlock.Destroy(blk.obj());
			}
		}
	}
}
