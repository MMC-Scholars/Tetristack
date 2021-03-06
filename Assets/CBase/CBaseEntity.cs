﻿// This software is under partial ownership by The Ohio State University, 
//for it is a product of student employees. For official policy, see
//https://tco.osu.edu/wp-content/uploads/2013/09/PatentCopyrightPolicy.pdf 
//or contact The Ohio State University's Office of Legal Affairs
/**
 * @author Michael Trunk
 * @startdate	29/9/2017
 * @enddate		3/10/2017
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets {
	abstract partial class g {
		/**
		 * This function is a shorthand for getting a CBaseEntity
		 *		out of a GameObject .
		 */ 
		public static CBaseEntity ToBaseEntity(GameObject obj) {
			return obj.GetComponent<CBaseEntity>();
		}

		public static bool HasBaseEntity(GameObject obj) {
			return ToBaseEntity(obj) != null;
		}

		public static CBaseEntity ToBaseEntity(int index) {
			return CBaseEntity.g_aEntList[index];
		}

		public static CBaseEntity ToBaseEntity(string objName) {
			CBaseEntity result = null;

			foreach (CBaseEntity pEnt in CBaseEntity.g_aEntList) {
				if (pEnt.obj().name.Equals(objName)) {
					result = pEnt;
					break;
				}
			}

			return result;
		}
	}

	/****************************************************************************************
	 * CBaseEntity class is base type for all of MMC's objects.
	 ***************************************************************************************/
	public partial class CBaseEntity : MonoBehaviour {

		//This simple accessor will help us type less later on
		public	GameObject	obj() { return gameObject; }

		/****************************************************************************************
		* Position/transform/velocity aliases
		***************************************************************************************/
		public Vector3		GetAbsOrigin() { return obj().transform.position; }
		public Quaternion	GetAbsAngles() { return obj().transform.rotation; }
		public Vector3		GetAbsVelocity() { return GetPhysics().velocity; }
		public void			SetAbsVelocity(Vector3 velocity) { GetPhysics().velocity = velocity; }
		public Vector3		GetAngVelocity() { return GetPhysics().angularVelocity; }
		public void			SetAngVelocity(Vector3 velocity) { GetPhysics().angularVelocity = velocity; }

		public Transform GetTransform() {
			return obj().transform;
		}

		/****************************************************************************************
		 * Global entity list functionality
		 ***************************************************************************************/
		static public   List<CBaseEntity>   g_aEntList = new List<CBaseEntity>();


		/****************************************************************************************
		 * Private variables for teleporting to original spawn location/angle
		 ***************************************************************************************/
		private Vector3     m_vSpawnLocation;
		private Quaternion  m_qSpawnAngle;
		public  Vector3     m_vSpawnVelocity = new Vector3();
		public	Vector3		GetSpawnLocation() { return m_vSpawnLocation; }

		/****************************************************************************************
		 * Teleport / position functionality
		 ***************************************************************************************/
		public	void	TeleportTo(Vector3 vPosition) {
			obj().transform.SetPositionAndRotation(vPosition, obj().transform.rotation);
		}
		public	void	TeleportDisplaced(Vector3 vOffset) {
			obj().transform.SetPositionAndRotation(obj().transform.position + vOffset,obj().transform.rotation);
		}
		public	Vector3	GetPosition() { return obj().transform.position; }
		public	Vector3 GetOffsetTo(Vector3 vPosition) {
			return vPosition - GetAbsOrigin();
		}
		public	Vector3 GetOffsetTo(CBaseEntity pEnt) {
			return GetOffsetTo(pEnt.GetAbsOrigin());
		}

		public	Quaternion GetAngleTo(Vector3 vPosition) {
			return Quaternion.LookRotation(GetOffsetTo(vPosition));
		}
		public	Quaternion GetAngleTo(CBaseEntity pEnt) {
			return Quaternion.LookRotation(GetOffsetTo(pEnt.GetAbsOrigin()));
		}

		/****************************************************************************************
		 * Visibility functionality
		 ***************************************************************************************/
		public  bool    m_bHideOnSpawn = false;	
		public	bool	CanSee(CBaseEntity pEnt) {
			return Physics.Raycast(GetPosition(), pEnt.GetPosition() - GetPosition());
		}
		public	bool	CanSee(Vector3 vPos) {
			return UnityEngine.Physics.Raycast(GetPosition(), vPos - GetPosition());
		}

		/****************************************************************************************
		 * Generic "Use" functionality
		 ***************************************************************************************/
		public	bool			IsUseable() { return !HasFlag(FL_IGNORE_USE); }
		public	void			SetUseable(bool bUseable) {
			if (bUseable)
				RemoveFlags(FL_IGNORE_USE);
			else
				AddFlags(FL_IGNORE_USE);
		}
		public	bool			Use(CBaseEntity pUser) {
			bool bUsed = IsUseable();
			if (bUsed) {
				if (m_bUseTogglesPickup && pUser is CBaseController) {
					if (!m_bIsPickedUp || !HasParent() || pUser != GetParent())
						Pickup(pUser as CBaseController);
					else
						Drop(pUser as CBaseController);
				}
				OnUsed(pUser);
			}
			return bUsed;
		}
		public	virtual void	OnUsed(CBaseEntity pUser) { }

		/****************************************************************************************
		 * Aliases for transform parenting
		 ***************************************************************************************/
		public	void			SetParent(CBaseEntity pParent) {
			GetTransform().parent = pParent.GetTransform();
		}
		public	CBaseEntity		GetParent() { return g.ToBaseEntity(GetTransform().parent.gameObject); }
		public  bool			HasParent() { return GetTransform().parent != null; }
		private Transform       m_pDefaultParent;
		public  bool			m_bCenterOnParent = false;

		/****************************************************************************************
		 * Physics aliases
		 ***************************************************************************************/
		private bool			m_bHasGravityEnabledByDefault;
		public	bool			HasPhysics()						{ return GetPhysics() != null; }
		public	Rigidbody		GetPhysics()						{ return obj().GetComponent<Rigidbody>(); }
		public	void			SetGravityEnabled(bool bEnabled)	{ GetPhysics().useGravity = bEnabled; }
		public	bool			GetGravityEnabled()					{ return GetPhysics().useGravity; }
		public	void			SetGravityEnabledByDefault(bool bEnabled) { m_bHasGravityEnabledByDefault = bEnabled; }

		/****************************************************************************************
		 * Health functionality
		 ***************************************************************************************/
		private bool    m_bAlive = true;
		private int		m_iHealth;
		public  int		m_iSpawnHealth = 100;

		public	int		GetHealth() { return m_iHealth; }
		public	void	SetHealth(int iHealth) { m_iHealth = iHealth; CalculateDeath(); }
		private	void	CalculateDeath() {
			m_bAlive = HasFlag(FL_INVINCIBLE) || m_iHealth > 0;
		}

		public	void	TraceAttack(CDamageInfo info) {
			if (!HasFlag(FL_NODAMAGE)) {
				m_iHealth -= info.GetDamage();
				CalculateDeath();
				OnTakeDamage(info);
				if (IsDead()) {
					OnKilled(info);
					info.GetAttacker().OnKilledOther(info);
				} else {
					OnTakeDamageAlive(info);
				}
			}
		}

		public	void	Kill() {
			CDamageInfo info = new CDamageInfo(m_iHealth, this, GetPosition());
			TraceAttack(info);
		}

		public	void	OnTakeDamage(CDamageInfo info) { } //Called after damage is subtracted from health
		public	void	OnTakeDamageAlive(CDamageInfo info) { } //called after OnTakeDamage(..), only if still alive
		public	void	OnKilled(CDamageInfo info) { CalculateNextRespawnTime(); obj().SetActive(false); }
		public	void	OnKilledOther(CDamageInfo info) { }

		public	bool	IsAlive()	{ return m_bAlive; }
		public	bool	IsDead()	{ return !IsAlive(); }

		/****************************************************************************************
		 * Respawn functionality resets values to defaults
		 ***************************************************************************************/
		float			m_flLastRespawnTime = 0.0f;
		float			m_flNextRespawnTime = float.MaxValue;
		public	float	GetLastRespawnTime() { return m_flLastRespawnTime; }
		private	void	CalculateNextRespawnTime() { m_flNextRespawnTime = g.curtime + 10.0f; }

		public	virtual void	Respawn() {

			m_flLastRespawnTime = g.curtime;
			obj().SetActive(true);

			if (GetComponent<Renderer>() != null)
				GetComponent<Renderer>().enabled = !m_bHideOnSpawn;

			//reset position
			if (!HasFlag(FL_NO_RESPAWN_TELEPORT))
				obj().transform.SetPositionAndRotation(m_vSpawnLocation,m_qSpawnAngle);

			//set velocity
			if (HasPhysics())
				GetPhysics().velocity = m_vSpawnVelocity;

			//reset bitflags
			m_iFlags = m_iSpawnFlags;

			//reset health
			m_iHealth = m_iSpawnHealth;

			//reset gravity enabled
			if (HasPhysics()) SetGravityEnabled(m_bHasGravityEnabledByDefault);

			//reset parent to default
			GetTransform().parent = m_pDefaultParent;

			if (GetTransform().parent != null && m_bCenterOnParent) {
				TeleportTo(GetTransform().parent.position);
			}

			InitControllerInputs();
		}

		/****************************************************************************************
		 * MonoBehavior overrides
		 ***************************************************************************************/

		// Use this for initialization
		public virtual void		Start() {
			g.CheckForReinitializationOnStart();
			g_aEntList.Add(this);

			m_pAudioSource = obj().AddComponent<AudioSource>();

			//Remember default spawn values
			m_iSpawnFlags		= m_iFlags;
			m_vSpawnLocation	= obj().transform.position;
			m_qSpawnAngle		= obj().transform.rotation;
			if (HasPhysics()) m_bHasGravityEnabledByDefault = GetGravityEnabled();
			m_pDefaultParent	= GetTransform().parent;

			AddFlagToChildren(FL_NO_RESPAWN_TELEPORT);

			Respawn();
		}

		public virtual void		Awake() { }

		public virtual void		OnDestroy() {
			g_aEntList.Remove(this);
		}

		// Update is called once per frame
		public virtual void		Update() {
			//Check for next respawn
			if (g.curtime > m_flNextRespawnTime) {
				if(HasFlag(FL_DESTROY_ON_RESPAWN))
					Destroy(obj());
				else
					Respawn();
			}

			//This is so that picked up objects stop moving/rotating
			//after colliding with a static kinematic object
			if (m_bIsPickedUp) {
				SetAbsVelocity(Vector3.zero);
				SetAngVelocity(Vector3.zero);
			}
		}

		/**
		 * Marks for re-initialization of globals
		 */ 
		public virtual void		OnApplicationQuit() {
			g.MarkForReinitializationOnNextStart();
			
		}

		//These are called when enabled or disabled
		public virtual void	OnEnable() { }
		public virtual void OnDisable() { }
	}
}
