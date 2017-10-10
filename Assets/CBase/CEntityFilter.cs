// This software is under partial ownership by The Ohio State University, 
//for it is a product of student employees. For official policy, see
//https://tco.osu.edu/wp-content/uploads/2013/09/PatentCopyrightPolicy.pdf 
//or contact The Ohio State University's Office of Legal Affairs
/**
 * @author Michael Trunk
 * @startdate	29/9/2017
 * @enddate		2/10/2017
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

		/****************************************************************************************
		 * Public helper functions
		 ***************************************************************************************/
		bool				FilterEntity(CBaseEntity pEnt) {
			bool bResult = false;
			if (pEnt != null) {
				switch(m_eFilterMode) {
				case EFilterMode.Normal:
					bResult = FilterEntityNorm(pEnt);
					break;
				case EFilterMode.Inverted:
					bResult = !FilterEntityNorm(pEnt);
					break;
				case EFilterMode.Random:
					bResult = NRand.randBool(m_flRandomChance);
					break;
				case EFilterMode.AllowAll:
					bResult = true;
					break;
				case EFilterMode.BlockAll:
					bResult = false;
					break;
				}
			}
			return bResult;
		}

		//@requires input is non-null
		List<CBaseEntity>	FilterEntityList(List<CBaseEntity> pEntList) {
			List<CBaseEntity> result;

			if (m_eFilterMode == EFilterMode.BlockAll) {
				result = new List<CBaseEntity>(0);
			} else if (m_eFilterMode == EFilterMode.AllowAll) {
				result = new List<CBaseEntity>(pEntList);
			} else {
				result = new List<CBaseEntity>(pEntList.Count());
				for (int i = 0; i < pEntList.Count(); i++) {
					if (FilterEntity(pEntList.ElementAt(i))) {
						result.Add(pEntList.ElementAt(i));
					}
				}
			}
			
			return result;
		}
	};
}
