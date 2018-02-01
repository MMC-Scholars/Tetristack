using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets {
	abstract partial class g {
		public static bool			HasBaseBlock(GameObject obj) { return ToBaseBlock(obj) != null; }

		public static CBaseBlock	ToBaseBlock(GameObject obj) {
			return obj.GetComponent<CBaseBlock>();
		}
	}

	public class CBaseBlock : CBaseEntity {

		public CBaseBlock m_pSource = null; //block which was used as a template to instantiate this block

		public bool IsInstantiated() { return m_pSource != null; }
		public bool m_bHasEnteredBuildingArea = false;

		public AudioClip m_pPickupSound;
		public AudioClip m_pDroppedSound;

		/**
		 * Counts the total number of blocks in the game.
		 */ 
		public static int count() {
			int count = 0;
			for (int i = 0; i < CBaseEntity.g_aEntList.Count(); i++) {
				if(g.ToBaseEntity(i) is CBaseBlock) i++;
			}
			return count;
		}

		/**
		 * Counts the total number of blocks instantiated from the given source.
		 */ 
		public static int countFromSource(CBaseBlock pSource) {
			int count = 0;
			for(int i = 0; i < CBaseEntity.g_aEntList.Count(); i++) {
				CBaseBlock pBlock = g.ToBaseEntity(i) as CBaseBlock;
				if(pBlock != null && pBlock.m_pSource == pSource) count++;
			}
			return count;
		}

		public override void OnDestroy() {
			base.OnDestroy();
		}

		public override void Start() {
			base.Start();
			m_bHasEnteredBuildingArea = false;
			if (IsInstantiated()) {
				AddFlags(FL_NODAMAGE | FL_DESTROY_ON_RESPAWN); //so that blocks are removed on restart round
				//SetGravityEnabled(false);
			}
		}

		public override void Update() {
			if (!GetGravityEnabled()) {
				SetAbsVelocity(Vector3.zero);
				SetAngVelocity(Vector3.zero);
			}
		}

		public override void OnPickedUp(CBaseController pController) {
			base.OnPickedUp(pController);
			EmitSound(m_pPickupSound);
		}

		public override void OnDropped(CBaseController pController) {
			base.OnDropped(pController);
			g.TowerBuilderRules().OnBlockDropped(this);
			EmitSound(m_pDroppedSound);
		}
	}
}
