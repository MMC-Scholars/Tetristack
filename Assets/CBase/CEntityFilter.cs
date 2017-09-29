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
	enum EFilterType {
		ByName,
		ByTag,
		ByIsActive,
		ByIsVisible,
		ByIsSolid,
		ByIsPhysicsSimulated,
	};

	enum EFilterMode {
		Normal,
		Inverted,
		Random,
		BlockAll,
		AllowAll,
	};

	class CEntityFilter {
		private EFilterType m_eFilterType;
		private EFilterMode m_eFilterMode;

		public  float       m_flRandomChance;
		public  string      m_sTextComparator;

		/****************************************************************************************
		 * Private helper functions
		 ***************************************************************************************/
		bool	PassRandom() { return NRand.randBool(m_flRandomChance); }
		bool	FilterEntityNorm(CBaseEntity pEnt) {
			bool result = false;
			switch(m_eFilterType) {
			case EFilterType.ByName:
				result = pEnt.name.Equals(m_sTextComparator);
				break;
			case EFilterType.ByTag:
				result = pEnt.CompareTag(m_sTextComparator);
				break;
			case EFilterType.ByIsActive:
				result = pEnt.obj().activeInHierarchy;
				break;
			case EFilterType.ByIsVisible:
				Renderer pRenderer = pEnt.obj().GetComponent<Renderer>();
				if (pRenderer != null)
					result = pRenderer.enabled;
				break;
			case EFilterType.ByIsSolid:
				Collider pCollider = pEnt.obj().GetComponent<Collider>();
				if (pCollider != null)
					result = pCollider.enabled;
				break;
			case EFilterType.ByIsPhysicsSimulated:
				Rigidbody pBody = pEnt.obj().GetComponent<Rigidbody>();
				result = pBody != null;
				break;
			}

			return result;
		}
	};
}
