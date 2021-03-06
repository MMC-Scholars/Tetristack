﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets {
	class CHeightmap {
		/****************************************************************************************
		 * Private member variables
		 ***************************************************************************************/
		private int                 m_iResolution,
									m_iMaxY = 0, //coordinates of maximum value
									m_iMaxX = 0;

		public  List<List<int>>		m_aaValues;

		/****************************************************************************************
		 * Public constructors
		 ***************************************************************************************/
		public CHeightmap(int _iResolution) {
			m_iResolution = _iResolution;
			m_aaValues = new List<List<int>>();
			m_aaValues.Capacity = _iResolution;
			for (int i = 0; i < _iResolution; i++) {
				List<int> pList = new List<int>();
				pList.Capacity = _iResolution;

				for(int j = 0; j < _iResolution; j++)
					pList.Add(-1);

				m_aaValues.Add(pList);
				
			}
		}

		/****************************************************************************************
		 * Public accessors
		 ***************************************************************************************/
		public	int		GetResolution()					{ return m_iResolution; }
		public	int		GetPixelAsInt(int j, int i)		{ return m_aaValues.ElementAt(j).ElementAt(i); }
		public	float	GetPixelAsFloat(int j, int i)	{ return 1.0f * GetPixelAsInt(j,i) / int.MaxValue; }

		public	void	CalculateMax() {
			int curMax = m_aaValues[0][0];
			m_iMaxX = 0;
			m_iMaxY = 0;
			for (int j = 0; j < m_iResolution; j++) {
				for (int i = 0; i < m_iResolution; i++) {
					if (m_aaValues[j][i] > curMax) {
						curMax = m_aaValues[j][i];
						m_iMaxX = i;
						m_iMaxY = j;
					}
				}
			}
		}

		//The "Max" is not a literal value, but a proportion
		// For float, proportion is 0.0-1.0 of height
		// For int, proportion is GetMaxAsInt() / MAX_INT
		public	int 	GetMaxAsInt() { return m_aaValues[m_iMaxY][m_iMaxX]; }
		public	float 	GetMaxAsFloat() { return (float) (1.0 * GetMaxAsInt() / int.MaxValue); }
	}
}
