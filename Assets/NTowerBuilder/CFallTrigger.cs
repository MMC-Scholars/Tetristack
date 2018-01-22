using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets;

namespace Assets {

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
		private void OnTriggerEnter(Collider other) {
			CBaseBlock blk = other.gameObject.GetComponent<CBaseBlock>();
			Debug.Log(blk != null);
			if(blk != null && blk.IsInstantiated()) {
				
				m_iCount++;
				g.TowerBuilderRules().OnBlockFall(blk);
			}
		}


		private void OnCollisionEnter(Collision collision) {
			/*CBaseBlock blk = collision.collider.gameObject.GetComponent<CBaseBlock>();
			Debug.Log("Block fallen!\n");
			if (blk != null && blk.IsInstantiated()) {
				m_iCount++;
				g.TowerBuilderRules().OnBlockFall(blk);
			}*/
		}

		private void OnCollisionExit(Collision collision) {
			CBaseBlock blk = g.ToBaseBlock(collision.collider.gameObject);
			if(blk != null && blk.IsInstantiated()) {
				g.TowerBuilderRules().OnBlockFall(blk);
			}
		}
	}
}
