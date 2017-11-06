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
