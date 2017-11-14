// This software is under partial ownership by The Ohio State University, 
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
	}

	/****************************************************************************************
	 * CBaseEntity class is base type for all of MMC's objects.
	 ***************************************************************************************/
	public partial class CBaseEntity : MonoBehaviour {

		//This simple accessor will help us type less later on
		public	GameObject	obj() { return gameObject; }

		/****************************************************************************************
		 * Global entity list functionality
		 ***************************************************************************************/
		static public   List<CBaseEntity>   g_aEntList = new List<CBaseEntity>();


		/****************************************************************************************
		 * Private variables for teleporting to original spawn location/angle
		 ***************************************************************************************/
		private Vector3     m_vSpawnLocation;
		private Quaternion  m_qSpawnAngle;

		/****************************************************************************************
		 * Teleport / positition functionality
		 ***************************************************************************************/
		public	void	TeleportTo(Vector3 vPosition) {
			obj().transform.SetPositionAndRotation(vPosition, obj().transform.rotation);
		}
		public	Vector3	GetPosition() { return obj().transform.position; }

		/****************************************************************************************
		 * Visibility functionality
		 ***************************************************************************************/
		public	bool	CanSee(CBaseEntity pEnt) {
			return Physics.Raycast(GetPosition(), pEnt.GetPosition() - GetPosition());
		}
		public	bool	CanSee(Vector3 vPos) {
			return Physics.Raycast(GetPosition(), vPos - GetPosition());
		}

		/****************************************************************************************
		 * Generic "Use" functionality
		 ***************************************************************************************/
		public	bool			IsUseable() { return !HasFlag(FL_IGNORE_USE); }
		public	void			Use(CBaseEntity pUser) {
			if (!IsUseable()) {
				OnUsed(pUser);
			}
		}
		public	virtual void	OnUsed(CBaseEntity pUser) { }

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

		public	void	Respawn() {
			if(HasFlag(FL_DESTROY_ON_RESPAWN)) {
				Destroy(obj());
			} else {
				m_flLastRespawnTime = g.curtime;
				obj().SetActive(true);

				//reset position
				obj().transform.SetPositionAndRotation(m_vSpawnLocation,m_qSpawnAngle);

				//reset bitflags
				m_iFlags = m_iSpawnFlags;

				//reset health
				m_iHealth = m_iSpawnHealth;
			}
		}

		/****************************************************************************************
		 * MonoBehavior overrides
		 ***************************************************************************************/

		// Use this for initialization
		public virtual void		Start() {
			g.CheckForReinitializationOnStart();
			g_aEntList.Add(this);

			//Remember default spawn values
			m_iSpawnFlags		= m_iFlags;
			m_vSpawnLocation	= obj().transform.position;
			m_qSpawnAngle		= obj().transform.rotation;
			Respawn();
		}

		public virtual void		OnDestroy() {
			g_aEntList.Remove(this);
		}

		// Update is called once per frame
		public virtual void		Update() {
			//Check for next respawn
			if (g.curtime > m_flNextRespawnTime)
				Respawn();
		}

		/**
		 * Marks for re-initialization of globals
		 */ 
		public virtual void		OnApplicationQuit() {
			g.MarkForReinitializationOnNextStart();
		}
	}
}
