// This software is under partial ownership by The Ohio State University, 
//for it is a product of student employees. For official policy, see
//https://tco.osu.edu/wp-content/uploads/2013/09/PatentCopyrightPolicy.pdf 
//or contact The Ohio State University's Office of Legal Affairs
/**
 * @author Michael Trunk
 * @startdate	6/11/2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets {
	abstract partial class g {
		public static CGameRules pRules;
	}

	class CGameRules : CBaseEntity {

		/**
		 * Sets all entities to their default health and spawn flags.
		 * Does this by calling Respawn() on all entities.
		 */ 
		static void ReloadAllEntities() {
			for(int i = 0; i < g_aEntList.Count; i++) {
				CBaseEntity pEnt = g_aEntList[i];
				if (pEnt != null) {
					pEnt.Respawn();
				}
			}
		}

		/**
		 * Call this to restart the round.
		 */ 
		float m_flLastRoundRestart = 0.0f;
		public virtual void RestartRound() {
			m_flLastRoundRestart = g.curtime;
			ReloadAllEntities();
		}

		public override void Start() {
			base.Start();
			m_iSpawnFlags = m_iFlags = FL_IGNORE_USE | FL_NODAMAGE | FL_TRIGGERIGNORE;
			g.pRules = this;
		}

		public override void Update() {
			base.Update();
		}

		public virtual	void FixedUpdate() {
			g.updateGlobals();
		}
	}
}
