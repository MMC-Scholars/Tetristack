// This software is under partial ownership by The Ohio State University, 
//for it is a product of student employees. For official policy, see
//https://tco.osu.edu/wp-content/uploads/2013/09/PatentCopyrightPolicy.pdf 
//or contact The Ohio State University's Office of Legal Affairs
/**
 * @author Michael Trunk
 * @startdate	6/11/2017
 * @enddate		7/11/2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets {
	class CTowerBuilderRules : CGameRules {

		private CHeightmapGenerator m_pMeasuringStick;

		/****************************************************************************************
		 * Calls for a re-measuring of the max-height from the heightmap generator, and
		 *		returns the measured value.
		 ***************************************************************************************/
		static float MEASURE_DURATION = 3.0f;
		public float Remeasure() {
			float flMeasure = 0.0f;
			if (!m_pMeasuringStick.SequenceActive()) {
				m_pMeasuringStick.SequenceBegin(MEASURE_DURATION);
			}
			flMeasure = m_pMeasuringStick.SequenceMeasure();
			return flMeasure;
		}

		/****************************************************************************************
		 * Updates visual displays of high scores
		 ***************************************************************************************/
		public void UpdateDisplays() {
			//TODO link to other objects & implement!
		}

		/****************************************************************************************
		 * Ensures that there are enough blocks of each type.
		 ***************************************************************************************/
		public void CheckBlockCounts() {
			//TODO iterate through block entities, check their booleans to count them, and dispatch templated spawning of more of them 
		}

	}
}
