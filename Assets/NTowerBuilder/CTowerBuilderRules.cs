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
using UnityEngine;

namespace Assets {
	abstract partial class g {
		public static float TOWER_BUILDER_MAX_HEIGHT = 2048.0f;

		public static CTowerBuilderRules TowerBuilderRules() {
			return g.pRules as CTowerBuilderRules;
		}
	}

	class CTowerBuilderRules : CGameRules {

		//LINK THESE TO WORLD OBJECTS
		public Camera       m_pScreenshooter;
		public GameObject   _pMeasuringStick;
		public GameObject   _pBlockSequencer;
		//public GameObject   _pPlatform;
		public GameObject   _pScoreText;
		public GameObject   m_pHighScoreHalo;

		//Private references to our actualy entities!
		CHeightmapGenerator	m_pMeasuringStick;
		CBlockSequencer		m_pBlockSequencer;
		//CBaseMoving			m_pPlatform;
		TextMesh            m_pScoreText;

		int                 m_iHandCount = 0; //number of hands in the building area

		//float               m_flNextBlockDrop = 0.0f;
		bool                m_bForceBlockDrop = true;

		//High score interface
		CScoreTable m_pScores = new CScoreTable();

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
		static bool s_bAlwaysMeasure = true;

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
			float flMeasure = 0.0f;

			if(s_bAlwaysMeasure && g.curtime > m_flNextSequenceEnd) {
				m_flNextSequenceEnd = g.curtime + 0.01f;
				flMeasure = m_pMeasuringStick.MeasureMaxWorldUnits();
				//m_pMeasuringStick.GenerateAndPrint();
				OnNewScore(flMeasure);
				bDidMeasure = true;
			} else if (!s_bAlwaysMeasure) {
				if(g.curtime > m_flNextSequenceEnd) {
					m_flNextSequenceEnd = float.MaxValue;
					flMeasure = m_pMeasuringStick.MeasureMaxWorldUnits();
					OnNewScore(flMeasure);
					bDidMeasure = true;
				} else if(SequenceActive()) {
					flMeasure = m_pMeasuringStick.SequenceMeasure();
					bDidMeasure = true;
				}
			}
			if (bDidMeasure) {
				//TODO link to displays!
				string text = String.Format("{0:F2}m", flMeasure);
				m_pScoreText.text = text;
			}
		}


		void UpdateHighScoreHaloHeight() {
			//Debug.DrawLine(new Vector3(),m_pMeasuringStick.CenterBottom());
			m_pHighScoreHalo.transform.position = m_pMeasuringStick.CenterBottom() + new Vector3(0,m_pScores.highScore(),0);
		}

		//Called whenever a new score is measured
		public void OnNewScore(float flScore) {
			AdjustPlatformCameraHeight(flScore);
			if (m_pScores.notifyScore(flScore)) {
				//new high score reached, do some explosions!
				UpdateHighScoreHaloHeight();
			}
		}

		//TODO move camera along with platform
		public void AdjustPlatformCameraHeight(float flScoreHeight) {
			//m_pPlatform.SetPosition(flScoreHeight / g.TOWER_BUILDER_MAX_HEIGHT);
		}

		private CBaseBlock CreateBlock() {
			return m_pBlockSequencer.NextBlock(m_pBlockSequencer.GetTransform().position);
		}

		//Called when a block enters the "veil" of the building area, usually when held by hand
		public void OnBlockEnter(CBaseBlock pBlock) {
			pBlock.SetGravityEnabled(true);

			CreateBlock();
		}

		//Called when a block exits the "veil" of the building area,
		//either bc the player took it out or the tower is falling down
		public void OnBlockExit(CBaseBlock pBlock) {

		}

		//Called when a hand enters the building area
		public void OnHandEnter(GameObject hand) {

		}

		//called when a hand exits the building area
		public void OnHandExit(GameObject hand) {

		}

		/****************************************************************************************
		 * Unity overrides
		 ***************************************************************************************/
		public override void Start() {
			base.Start();
			if (_pBlockSequencer)	m_pBlockSequencer	= _pBlockSequencer.GetComponent<CBlockSequencer>();
			if (_pMeasuringStick)	m_pMeasuringStick	= _pMeasuringStick.GetComponent<CHeightmapGenerator>();
			//if (_pPlatform)			m_pPlatform			= _pPlatform.GetComponent<CBaseMoving>();
			if (_pScoreText)		m_pScoreText		= _pScoreText.GetComponent<TextMesh>();

			

			//m_pPlatform.SetDisplacement(new Vector3(0,0,g.TOWER_BUILDER_MAX_HEIGHT));
			m_pScores.reloadFromFile();
			UpdateHighScoreHaloHeight();

			
		}

		public override void Update() {
			base.Update();
			UpdateDisplays();
			
			if (m_bForceBlockDrop) {
				m_bForceBlockDrop = false;
				//m_flNextBlockDrop = g.curtime + 1.0f;
				CreateBlock();
			}
		}
	}
}
