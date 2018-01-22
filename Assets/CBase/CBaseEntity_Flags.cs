// This software is under partial ownership by The Ohio State University, 
//for it is a product of student employees. For official policy, see
//https://tco.osu.edu/wp-content/uploads/2013/09/PatentCopyrightPolicy.pdf 
//or contact The Ohio State University's Office of Legal Affairs
/**
 * @author Michael Trunk
 * @startdate	29/9/2017
 * @enddate		29/9/2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets {
	/**
	 * Definitions for the bitflags used by CBaseEntity. They are defined separately here
	 * for the sake of organization.
	 */ 
	public partial class CBaseEntity : MonoBehaviour {

		/**
		 * Flag accessors/setters
		 */ 
		public	int         m_iFlags = 0;
		public	int			GetFlags() { return m_iFlags; }
		public	void		SetFlags(int iFlags) { m_iFlags = iFlags; }
		public	void		AddFlags(int iFlags) { m_iFlags |= iFlags; }
		public	void		RemoveFlags(int iFlags) { m_iFlags &= ~iFlags; }

		/**
		 * The flags we originally spawned with
		 */
		protected int         m_iSpawnFlags = 0;

		/**
		 * These are the bit flags
		 */
		public  static int  FL_INVINCIBLE           = 1 << 0,	//Clamp health to always be above one?
							FL_NODAMAGE             = 1 << 1,	//Block all damage? (health stays at default value)
							FL_TRIGGERIGNORE        = 1 << 2,	//Should MMC's triggers ignore this entity?
							FL_IGNORE_USE           = 1 << 3,
							FL_DESTROY_ON_RESPAWN   = 1 << 4,	//use this for instantiated copies that should be destroyed on restart round
							FL_ALLOW_PICKUP         = 1 << 5;

		/**
		 * Use this to check if an object has a specific flag
		 */ 
		public bool		HasFlag(int iFlag) {
			return (m_iFlags & iFlag) > 0;
		}
		
		/**
		 * These set and check bit flags to set entity vulnerability
		 */ 
		//Can we be damaged and killed?
		public bool		IsVulnerable() { return !HasFlag(FL_INVINCIBLE) && !HasFlag(FL_NODAMAGE); } 
		
		//Sets flags such that this entity can be damaged and killed
		public void		SetVulnerable(bool bVulnerable) {
			int flags = FL_INVINCIBLE | FL_NODAMAGE;

			if (bVulnerable)
				RemoveFlags(flags);
			else
				AddFlags(flags);
		}
	}
}
