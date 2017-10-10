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

		int         m_iResolution;

		Vector3     m_vStart; //starting point does match m_vMaxXYZ
		float       m_flStepY; //distance between sample locations on Y axis
		float       m_flStepX; //distance between sample locations on X axis
		float       m_flHeight; //how far down do we raycast?

		static Vector3 s_vDown = new Vector3(0,0,-1);

		//perform other position calculations
		private void		init() {
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
		 * Public constructor
		 ***************************************************************************************/
		public CHeightmapGenerator(Vector3 _vMinXYZ,Vector3 _vMaxXYZ,int _iResolution) {
			m_vMinXYZ = _vMinXYZ;
			m_vMaxXYZ = _vMaxXYZ;
			m_iResolution = _iResolution;
			//init();
		}

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

		public void GenerateAndPrint() {
			CHeightmap map = Generate();
			for (int j = 0; j < m_iResolution; j++) {
				for (int i = 0; i < m_iResolution; i++) {
					Debug.Log(map.GetPixelAsFloat(j,i));
				}
			}
		}

		public override void Start() {
			base.Start();
		}
	}
}
