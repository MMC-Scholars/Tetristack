// This software is under partial ownership by The Ohio State University, 
//for it is a product of student employees. For official policy, see
//https://tco.osu.edu/wp-content/uploads/2013/09/PatentCopyrightPolicy.pdf 
//or contact The Ohio State University's Office of Legal Affairs
/**
 * @author Michael Trunk
 * @startdate	6/11/2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets {
	class CTowerBuilderRules : CGameRules {

		//LINK THESE TO WORLD OBJECTS
		public CHeightmapGenerator	m_pMeasuringStick;
		public CBlockSequencer      m_pBlockSequencer;


		//High score interface
		CScoreTable scores = new CScoreTable();

		/****************************************************************************************
		 * Checks if we're currently in a measuring sequence
		 ***************************************************************************************/
		public bool SequenceActive() { return m_pMeasuringStick.SequenceActive(); }

		/****************************************************************************************
		 * Marks for the beginning of a new measuring sequence
		 ***************************************************************************************/
		float m_flNextSequenceEnd;
		static float MEASURE_DURATION = 3.0f;
		public void SequenceBegin() { m_flNextSequenceEnd = g.curtime + MEASURE_DURATION; m_pMeasuringStick.SequenceBegin(MEASURE_DURATION); }

		/****************************************************************************************
		 * Calls for a re-measuring of the max-height from the heightmap generator, and
		 *		returns the measured value.
		 * Does NOT begin a measuring sequence and does not effect displays.
		 ***************************************************************************************/
		public float Remeasure() {
			return m_pMeasuringStick.MeasureMaxWorldUnits();
		}

		/****************************************************************************************
		 * Updates visual displays of high scores
		 ***************************************************************************************/
		public void UpdateDisplays() {
			bool bDidMeasure = false;
			float flMeasure;

			if(g.curtime > m_flNextSequenceEnd) {
				m_flNextSequenceEnd = float.MaxValue;
				OnNewHighScore();
				bDidMeasure = true;
			}
			else if (SequenceActive()) {
				flMeasure = m_pMeasuringStick.SequenceMeasure();
				bDidMeasure = true;
			}
			if (bDidMeasure) {
				//TODO link to displays!
			}
		}

		//Called whenever a new high score is measured
		public void OnNewHighScore() {

		}

		/****************************************************************************************
		 * Ensures that there are enough blocks of each type.
		 ***************************************************************************************/
		public void CheckBlockCounts() {
			//TODO iterate through block entities, check their booleans to count them, and dispatch templated spawning of more of them 
		}

		/****************************************************************************************
		 * Unity overrides
		 ***************************************************************************************/
		public override void Update() {
			base.Update();
			UpdateDisplays();
		}
	}
}
