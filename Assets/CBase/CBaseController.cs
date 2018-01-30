// This software is under partial ownership by The Ohio State University, 
//for it is a product of student employees. For official policy, see
//https://tco.osu.edu/wp-content/uploads/2013/09/PatentCopyrightPolicy.pdf 
//or contact The Ohio State University's Office of Legal Affairs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets {

	/****************************************************************************************
	* CBaseController hooks our CBaseEntity interface into the SteamVR and OpenVR interfaces
	*		and contains extendable base functionality 
	***************************************************************************************/
	public partial class CBaseController : CBaseEntity {

		private SteamVR_TrackedController m_pController;

		//public static List<CBaseController> s_pControllers;

		/****************************************************************************************
		* Master input enable/disable
		***************************************************************************************/
		private bool	m_bEnabled = true;
		void			SetInputEnabled(bool bEnabled) { m_bEnabled = bEnabled; }
		public bool		GetInputEnabled() { return m_bEnabled; }

		/****************************************************************************************
		* Monobehavior OnEnable()/OnDisable() will enable/disable input
		***************************************************************************************/
		public override void OnEnable() {
			base.OnEnable();
			SetInputEnabled(true);
		}

		public override void OnDisable() {
			base.OnDisable();
			SetInputEnabled(false);
		}

		/****************************************************************************************
		* Awake and respawn functions setup everything
		***************************************************************************************/
		public override void Awake() {
			base.Awake();
		}

		public void RemoveAllLinkedEntityInputs() {
			//Initialize buttons sets
			m_bsPress = new CButtonSet(this);
			m_bsTouch = new CButtonSet(this);
		}

		public override void Respawn() {
			base.Respawn();
			m_vPreviousOrigin = GetSpawnLocation();
			SteamVR_TrackedController pController = obj().GetComponent<SteamVR_TrackedController>();
			if (pController != null) {
				m_pController = pController;
			}
			else {
				Debug.LogWarning("CBaseController expected a SteamVR_TrackController to be attached to object " + obj().name);
			}

			//Initialize buttons sets
			//m_bsPress = new CButtonSet(this);
			//m_bsTouch = new CButtonSet(this);
			m_pColliding = new List<CBaseEntity>();
		}

		public override void OnApplicationQuit() {
			base.OnApplicationQuit();
		}

		/****************************************************************************************
		* Monobehavior Update() helps track OpenVR input changes
		***************************************************************************************/
		public override void Update() {
			base.Update();
			UpdateButtons();
			AbsVelocityManualCalc();
		}

		/****************************************************************************************
		* Collisions keep track of what we are/aren't touching
		***************************************************************************************/
		public List<CBaseEntity> m_pColliding;
		private void OnTriggerEnter(Collider collider) {
			CBaseEntity pEnt = g.ToBaseEntity(collider.gameObject);
			if (pEnt != null) {
				pEnt.SetIsTouchedByController(true);
				m_pColliding.Add(pEnt);
			}
		}
		private void OnTriggerExit(Collider collider) {
			CBaseEntity pEnt = g.ToBaseEntity(collider.gameObject);
			if (pEnt != null) {
				pEnt.SetIsTouchedByController(false);
				m_pColliding.Remove(pEnt);
			}
		}

		/****************************************************************************************
		* Grip state makes pickup logic more intuitive and less buggy
		***************************************************************************************/
		/*public enum EGripState {
			EMPTY = 0,
			WANTS_TO_GRIP,
			HOLD
		}
		EGripState GetGripState() {
			if (GetTransform().childCount > 0)
				return EGripState.HOLD;
			if (m_pColliding.Count > 0)
				return EGripState.WANTS_TO_GRIP;
			return EGripState.EMPTY;
		}*/
		public bool HasObjectPickedUp() {
			return GetTransform().childCount > 0;
		}



		/****************************************************************************************
		* Because any rigidbody's don't seem to follow the controller at all
		***************************************************************************************/
		Vector3 m_vPreviousOrigin;
		Vector3 m_vVelocity;
		void AbsVelocityManualCalc() {
			m_vVelocity = (GetAbsOrigin() - m_vPreviousOrigin) / g.frametime;
			m_vPreviousOrigin = GetAbsOrigin();
		}
		public Vector3 GetAbsVelocityManualCalc() { return m_vVelocity; }


	}

}
