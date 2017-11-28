// This software is under partial ownership by The Ohio State University, 
//for it is a product of student employees. For official policy, see
//https://tco.osu.edu/wp-content/uploads/2013/09/PatentCopyrightPolicy.pdf 
//or contact The Ohio State University's Office of Legal Affairs
/**
 * @author Michael Trunk
 * @startdate	6/13/2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets {
	class CHeightmapGenerator : CBaseEntity {

		/****************************************************************************************
		 * Private members
		 ***************************************************************************************/
		Vector3     m_vMinXYZ,
					m_vMaxXYZ;

		public int	m_iResolution = 512;

		Vector3     m_vStart; //starting point does match m_vMaxXYZ
		float       m_flStepY; //distance between sample locations on Y axis
		float       m_flStepX; //distance between sample locations on X axis
		float       m_flHeight; //how far down do we raycast?

		float       m_flSequenceDuration;
		float       m_flSequenceStartTime;
		float       m_flSequenceEndTime;
		float       m_flSequenceNextMeasureTime		= 0.0f;
		const float s_flSequenceNextMeasureInterval = 0.2f;
		float       m_flSequenceLastMeasure;
		const float s_flSequenceRandomAmplitude     = 0.5f;	//initial amount of random amplitude added to measurement; measurement gets more precise as time passes.

		static Vector3 s_vDown = new Vector3(0,0,-1);

		//List of blocks contained in the bounds of this generator
		List<CBaseBlock> m_aIntersecting = new List<CBaseBlock>();

		//perform other position calculations
		private void		init() {
			Renderer bounds = obj().GetComponent<Renderer>();
			if (bounds != null) {
				m_vMinXYZ = bounds.bounds.min;
				m_vMaxXYZ = bounds.bounds.max;
			}

			float dispX = m_vMaxXYZ.x - m_vMinXYZ.x;
			float dispY = m_vMaxXYZ.y - m_vMinXYZ.y;

			m_flStepX = dispX / m_iResolution;
			m_flStepY = dispY / m_iResolution;

			m_flHeight = m_vMaxXYZ.z - m_vMinXYZ.z;

			m_vStart = m_vMinXYZ + new Vector3(m_flStepX / 2, m_flStepY / 2, m_flHeight);
		}

		private int cast(int j, int i) {
			Vector3 vStart = m_vStart + new Vector3(m_flStepY * j, m_flStepX * i, 0);
			RaycastHit tr = new RaycastHit();
			Physics.Raycast(vStart,s_vDown,out tr);
			return (int)((tr.distance / m_flHeight) * int.MaxValue);
		}

		/****************************************************************************************
		 * Public interface
		 ***************************************************************************************/
		public CHeightmapGenerator() { }

		public CHeightmap Generate() {
			CHeightmap dest = new CHeightmap(m_iResolution);
			for (int j = 0; j < m_iResolution; j++) {
				for (int i = 0; i < m_iResolution; i++) {
					dest.m_aaValues[j][i] = cast(j,i);
				}
			}
			dest.CalculateMax();
			return dest;
		}

		//for debugging
		public void GenerateAndPrint() {
			CHeightmap map = Generate();
			for (int j = 0; j < m_iResolution; j++) {
				for (int i = 0; i < m_iResolution; i++) {
					Debug.Log(map.GetPixelAsFloat(j,i));
				}
			}
		}

		//Generates a new heightmap and returns the max height, IN WORLD UNITS.
		public float MeasureMaxWorldUnits() {
			CHeightmap map = Generate();
			return m_flHeight * map.GetMaxAsFloat();
		}

		/**
		 * Begins a measurement sequence to last for the given duration.
		 * A Measurement sequence makes this CHeightmapGenerator repeatedly
		 *		re-measure the max height and get more precise over time
		 */ 
		public void	SequenceBegin(float flDuration) {
			m_flSequenceDuration	= flDuration;
			m_flSequenceStartTime	= g.curtime;
			m_flSequenceEndTime		= g.curtime + flDuration;
		}

		/**
		 * Returns the current max height as measured by the current sequence.
		 */ 
		public float SequenceMeasure() {
			if (g.curtime > m_flSequenceNextMeasureTime) {
				m_flSequenceNextMeasureTime += s_flSequenceNextMeasureInterval;
				m_flSequenceLastMeasure = MeasureMaxWorldUnits();
			}
			//result has a little randomness. The randomness decreases as the final measurement approaches.
			return m_flSequenceLastMeasure + NRand.randFloat(-1,1) * s_flSequenceRandomAmplitude * SequenceRemainingTimeProportion();
		}

		/**
		 * Returns the amount of time remaining in the sequence
		 */ 
		public float SequenceRemainingTime() {
			return m_flSequenceEndTime - m_flSequenceStartTime;
		}

		/**
		 * Returns the amount of time remaining in the sequence, as a proportion
		 *		of the total duration of the sequence.
		 */ 
		public float SequenceRemainingTimeProportion() {
			return SequenceRemainingTime() / m_flSequenceDuration;
		}

		/**
		 * Returns whether or not this CHeightMapGenerator is currently in a sequence.
		 */ 
		public bool SequenceActive() {
			return g.curtime > m_flSequenceStartTime && g.curtime < m_flSequenceEndTime;
		}

		/**
		 * Calls collider functions to detect what other CBaseEntity are colliding with this
		 */
		private void ReloadIntersecting() {
			float halfZ = m_flHeight / 2;
			float halfY = m_vMaxXYZ.y - m_vMinXYZ.y;
			float halfX = m_vMaxXYZ.x - m_vMinXYZ.x;
			Vector3 center = (m_vMinXYZ + m_vMaxXYZ) / 2;

			m_aIntersecting.Clear();

			Collider[] colls = Physics.OverlapBox(center, new Vector3(halfX, halfY, halfZ));

			foreach (Collider col in colls) {
				CBaseBlock ent = g.ToBaseBlock(col.gameObject);
				if (ent != null) {
					m_aIntersecting.Add(ent);
				}
			}
		}

		/**
		 * Returns a heightmap texture in R8 format.
		 */ 
		Texture2D ToTexture() {
			CHeightmap hmap = this.Generate();
			Texture2D image = new Texture2D(m_iResolution, m_iResolution, TextureFormat.R8, false);

			for (int i = 0; i < m_iResolution; i++) {
				for (int j = 0; j < m_iResolution; j++) {
					int c = hmap.m_aaValues[i][j];
					image.SetPixel(i,j,new Color(c,c,c));
				}
			}

			return image;
		}

		/****************************************************************************************
		 * Unity overrides
		 ***************************************************************************************/
		private void OnCollisionEnter(Collision collision) {
			CBaseBlock blk = g.ToBaseBlock(collision.collider.gameObject);
			if (blk != null) {
				m_aIntersecting.Add(blk);
				g.TowerBuilderRules().OnBlockEnter(blk);
			}

		}

		private void OnCollisionExit(Collision collision) {
			CBaseBlock blk = g.ToBaseBlock(collision.collider.gameObject);
			if(blk != null) {
				m_aIntersecting.Remove(blk);
			}
		}
		public override void Start() {
			base.Start();

			//reinstantiate list
			ReloadIntersecting();

			//initialize position variables to get ready for generating heightmaps.
			init();

			//Hide ourselves
			obj().GetComponent<Renderer>().enabled = false;
		}

		public override void Update() {
			base.Update();
		}
	}
}
