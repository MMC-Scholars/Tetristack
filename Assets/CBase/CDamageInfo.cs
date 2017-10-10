// This software is under partial ownership by The Ohio State University, 
//for it is a product of student employees. For official policy, see
//https://tco.osu.edu/wp-content/uploads/2013/09/PatentCopyrightPolicy.pdf 
//or contact The Ohio State University's Office of Legal Affairs
/**
 * @author Michael Trunk
 * @startdate	29/9/2017
 * @enddate		3/10/2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets {
	public enum EDamageType {
			DMG_GENERIC,
			DMG_BLAST,
			DMG_BULLET,
			DMG_LASER,
			DMG_FORCE,
		};

	public class CDamageInfo {
		/**
		 * Private member variables
		 */ 
		int			m_iDamage;
		int         m_iBaseDamage;
		int         m_iMaxDamage = int.MaxValue;
		int         m_iMinDamage = int.MinValue;

		CBaseEntity	m_pAttacker;
		CBaseEntity m_pWeapon;

		EDamageType m_eDamageType = EDamageType.DMG_GENERIC;

		Vector3     m_vDamagePosition;
		Vector3     m_vForce;

		//clamps min and max damage
		void		ClampDamage() {
			m_iDamage = m_iDamage < m_iMinDamage ? m_iMinDamage : m_iDamage;
			m_iDamage = m_iDamage > m_iMaxDamage ? m_iMaxDamage : m_iDamage;
		}

		//Privatise default constructor
		private CDamageInfo() { }

		/**
		 * Public constructors
		 */ 
		public CDamageInfo(int iDamage, CBaseEntity pAttacker, Vector3 vDmgPosition) {
			m_iDamage = m_iBaseDamage = iDamage;
			m_pAttacker = pAttacker;
			m_vDamagePosition = vDmgPosition;
		}
		public CDamageInfo(int iDamage, EDamageType eType, CBaseEntity pAttacker, Vector3 vDmgPosition) {
			m_iDamage = m_iBaseDamage = iDamage;
			m_pAttacker = pAttacker;
			m_vDamagePosition = vDmgPosition;
			m_eDamageType = eType;
		}
		public CDamageInfo(int iDamage, CBaseEntity pAttacker, Vector3 vDmgPosition, Vector3 vForce) {
			m_iDamage = m_iBaseDamage = iDamage;
			m_pAttacker = pAttacker;
			m_vDamagePosition = vDmgPosition;
			m_vForce = vForce;
		}
		public CDamageInfo(int iDamage,EDamageType eType,CBaseEntity pAttacker,Vector3 vDmgPosition,Vector3 vForce) {
			m_iDamage = m_iBaseDamage = iDamage;
			m_pAttacker = pAttacker;
			m_vDamagePosition = vDmgPosition;
			m_vForce = vForce;
			m_eDamageType = eType;
		}
		public CDamageInfo(int iDamage, int iMinDmg, int iMaxDmg, CBaseEntity pAttacker, Vector3 vDmgPosition) {
			m_iDamage = m_iBaseDamage = iDamage;
			m_pAttacker = pAttacker;
			m_vDamagePosition = vDmgPosition;
			m_iMinDamage = iMinDmg;
			m_iMaxDamage = iMaxDmg;
		}
		public CDamageInfo(int iDamage, int iMinDmg, int iMaxDmg, CBaseEntity pAttacker, Vector3 vDmgPosition, Vector3 vForce) {
			m_iDamage = m_iBaseDamage = iDamage;
			m_pAttacker = pAttacker;
			m_vDamagePosition = vDmgPosition;
			m_vForce = vForce;
			m_iMinDamage = iMinDmg;
			m_iMaxDamage = iMaxDmg;
		}
		public CDamageInfo(int iDamage,int iMinDmg,int iMaxDmg, EDamageType eType, CBaseEntity pAttacker,Vector3 vDmgPosition,Vector3 vForce) {
			m_iDamage = m_iBaseDamage = iDamage;
			m_pAttacker = pAttacker;
			m_vDamagePosition = vDmgPosition;
			m_vForce = vForce;
			m_iMinDamage = iMinDmg;
			m_iMaxDamage = iMaxDmg;
			m_eDamageType = eType;
		}


		/**
		 * Public accessors
		 */
		public int			GetBaseDamage()			{ return m_iBaseDamage; }
		public int			GetDamage()				{ return m_iDamage; }
		public void			SetDamage(int iDamage)	{ m_iDamage = iDamage; ClampDamage(); }
		public void			SubtractDamage(int sub) { m_iDamage -= sub; ClampDamage(); }
		public void			AddDamage(int iDamage)	{ m_iDamage += iDamage; ClampDamage(); }
		public void			ScaleDamage(float mult) { m_iDamage = (int) (m_iDamage * mult); ClampDamage(); }
		public int			GetDamageForForce()		{ return (int) m_vForce.magnitude; }
		public Vector3		GetDamagePosition()		{ return m_vDamagePosition; }
		public Vector3		GetDamageForce()		{ return m_vForce; }

		public CBaseEntity	GetAttacker()			{ return m_pAttacker; }
		public CBaseEntity	GetWeapon()				{ return m_pWeapon; }
		public EDamageType	GetDamageType()			{ return m_eDamageType; }
	}
}
