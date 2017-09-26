using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets {
	/**
	 * Definitions for the bitflags used by CBaseEntity. They are defined separately here
	 * for the sake of organization.
	 * 
	 */ 
	public partial class CBaseEntity : MonoBehaviour {

		/**
		 * Flag accessors/setters
		 */ 
		public	int         m_iFlags = 0;
		public	int			GetFlags() { return m_iFlags; }
		public	void		SetFlags(int iFlags) { m_iFlags = iFlags; }

		/**
		 * The flags we originally spawned with
		 */
		private int         m_iSpawnFlags = 0;

		/**
		 * These are the bit flags
		 */ 
		public	static int  FL_INVINCIBLE       = 1 << 0,	//Clamp health to always be above one?
							FL_NODAMAGE         = 1 << 1,	//Block all damage? (health stays at default value)
							FL_TRIGGERIGNORE    = 1 << 2;	//Should MMC's triggers ignore this entity?

		/**
		 * Use this to check if an object has a specific flag
		 */ 
		public bool		HasFlag(int iFlag) {
			return (m_iFlags & iFlag) > 0;		
		}
							
	}
}
