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
	public abstract partial class g {
		public static float TOWER_BUILDER_MAX_HEIGHT = 2048.0f;

		public static CTowerBuilderRules TowerBuilderRules() {
			return g.pRules as CTowerBuilderRules;
		}
	}

	public class CTowerBuilderRules : CGameRules {

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
		float				m_flCurrentHeight;
		//float               m_flNextBlockDrop = 0.0f;
		bool                m_bForceBlockDrop = true;
		

		public AudioClip    m_pRestartRoundSound;

		AudioSource         m_pMusicSource;
		public AudioClip    m_pGameMusic;

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
				string text = String.Format("{0:F1}m", flMeasure);
				m_pScoreText.text = text;
				m_flCurrentHeight = flMeasure;

				//calc music pitch
				float newpitch = 1 + (GetCurrentHeight()- 1.0f) / m_pScores.highScore();
				if (newpitch < 1) newpitch = 1;
				m_pMusicSource.pitch = newpitch;
			}
		}

		public float GetCurrentHeight() { return m_flCurrentHeight; }

		void UpdateHighScoreHaloHeight() {
			Vector3 dest = m_pMeasuringStick.CenterBottom()+new Vector3(0,m_pScores.highScore() - 0.12f,0);
			if (dest.y < m_pMeasuringStick.CenterBottom().y)
				dest.y = m_pMeasuringStick.CenterBottom().y;
			m_pHighScoreHalo.transform.position = dest;
		}

		//Called whenever a new score is measured
		public void OnNewScore(float flScore) {
			AdjustPlatformCameraHeight(flScore);
			if (m_pScores.notifyScore(flScore)) {
				//new high score reached, do some explosions!
				//UpdateHighScoreHaloHeight();

				//just hide ourselves to stop annoying the player
				m_pHighScoreHalo.SetActive(false);

				//stop the music
				m_pMusicSource.mute = true;
			}
		}

		//TODO move camera along with platform
		public void AdjustPlatformCameraHeight(float flScoreHeight) {
			//m_pPlatform.SetPosition(flScoreHeight / g.TOWER_BUILDER_MAX_HEIGHT);
		}

		public CBaseBlock CreateBlock() {
			CBaseBlock pBlock = m_pBlockSequencer.NextBlock(m_pBlockSequencer.GetTransform().position);
			pBlock.SetGravityEnabled(false);
			if (m_pBlockSequencer.NumGenerated() > 1 && m_bDropBlocks) {
				pBlock.TeleportDisplaced(new Vector3(0, 2, 0));
				pBlock.m_bMoveDown = true;
			}
			return pBlock;
		}

		public float BlockDownSpeed() {
			return 0.2f + 0.2f * (GetCurrentHeight() / m_pScores.highScore());
		}

		//Called when a block enters the "veil" of the building area, usually when held by hand
		public void OnBlockEnter(CBaseBlock pBlock) {
			if (!pBlock.m_bHasEnteredBuildingArea) {
				pBlock.m_bHasEnteredBuildingArea = true;
				pBlock.SetGravityEnabledByDefault(true);
				if (!m_bDropBlocks)
					CreateBlock();
			}
		}

		public void OnBlockDropped(CBaseBlock pBlock) {
			UpdateDisplays();
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

		//called when a block falls below the building platform
		public void OnBlockFall(CBaseBlock pBlock) {
			RestartRound();
		}

		public override void RestartRound() {
			base.RestartRound();
			m_bForceBlockDrop = true;

			EmitSound(m_pRestartRoundSound);

			m_pMusicSource.pitch = 1;
			m_pMusicSource.volume = 1;
		}

		public override void Respawn() {
			base.Respawn();
			m_flCurrentHeight = 0.5f;

			Hook_ToggleMusic(g.LeftController());
			Hook_ToggleMusic(g.RightController());
			Hook_ToggleBlockDrop(g.LeftController());
			Hook_ToggleBlockDrop(g.RightController());


			m_pMusicSource.mute = m_bMuteStatus;
		}

		/****************************************************************************************
		 * Unity overrides
		 ***************************************************************************************/
		public override void Start() {
			//create music player
			m_pMusicSource = obj().AddComponent<AudioSource>();
			m_pMusicSource.clip = m_pGameMusic;
			m_pMusicSource.loop = true;
			m_pMusicSource.volume = 1.0f;
			m_pMusicSource.Play();
			m_bMuteStatus = false;

			base.Start();
			if (_pBlockSequencer)	m_pBlockSequencer	= _pBlockSequencer.GetComponent<CBlockSequencer>();
			if (_pMeasuringStick)	m_pMeasuringStick	= _pMeasuringStick.GetComponent<CHeightmapGenerator>();
			//if (_pPlatform)			m_pPlatform			= _pPlatform.GetComponent<CBaseMoving>();
			if (_pScoreText)		m_pScoreText		= _pScoreText.GetComponent<TextMesh>();

			m_pScores.reloadFromFile();
			m_pMeasuringStick.init();
			UpdateHighScoreHaloHeight();

			m_pScoreText.text = String.Format("{0:F1}m", m_pScores.highScore());

			
		}

		public override void Update() {
			base.Update();
			//UpdateDisplays();
			
			if (m_bForceBlockDrop) {
				m_bForceBlockDrop = false;
				//m_flNextBlockDrop = g.curtime + 1.0f;
				CreateBlock();
			}
		}

		/****************************************************************************************
		 * Music toggle function
		 ***************************************************************************************/
		private bool m_bMuteStatus = false;
		public bool ToggleMusic(CBaseEntity pEnt, CBaseController pController) {
			m_bMuteStatus = !m_bMuteStatus;
			m_pMusicSource.mute = !m_pMusicSource.mute;
			return true;
		}

		private void Hook_ToggleMusic(CBaseController pController) {
			HookInputFunction(pController, g.IN_A, false, false, ToggleMusic);
		}

		/****************************************************************************************
		 * Block drop toggle function
		 ***************************************************************************************/
		public bool                m_bDropBlocks = false;
		public bool ToggleBlockDrop(CBaseEntity pEnt, CBaseController pController) {
			m_bDropBlocks = !m_bDropBlocks;
			return true;
		}
		private void Hook_ToggleBlockDrop(CBaseController pController) {
			HookInputFunction(pController, g.IN_B, false, false, ToggleBlockDrop);
		}
	}
}
