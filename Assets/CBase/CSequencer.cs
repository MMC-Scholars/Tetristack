// This software is under partial ownership by The Ohio State University, 
//for it is a product of student employees. For official policy, see
//https://tco.osu.edu/wp-content/uploads/2013/09/PatentCopyrightPolicy.pdf 
//or contact The Ohio State University's Office of Legal Affairs
/**
 * @author Michael Trunk
 * @startdate	2/11/2017
 * @enddate		2/11/2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets {
	class CSequencer : CBaseEntity {
		private List<CBaseEntity>	m_aEntities;
		public  int                 m_iInitialLength = 0;
		public  Vector3             m_vOffset;

		public GameObject			m_pPrefab;
		NetworkClient				m_pClient;

		public CSequencer() { }

		/**
		 * Retrieves the number of sequenced objects
		 */
		int Len() { return m_aEntities.Count(); }

		override public void Start() {
			base.Start();

			m_aEntities = new List<CBaseEntity>(m_iInitialLength);

			for (int i = 0; i < m_iInitialLength; i++) {

			}
		}

	}
}
