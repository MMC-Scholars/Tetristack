﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Assets {
	class CScoreTable {
		float m_flHighScore = 0.0f;

		const string s_sFileName = "highscore.dat";

		/****************************************************************************************
		 * Reloads high score from file
		 ***************************************************************************************/
		public void reloadFromFile() {
			if(!File.Exists(s_sFileName))
				writeToFile();

			FileStream stream = new FileStream(s_sFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			string str = stream.ToString();
			m_flHighScore = float.Parse(str);
			stream.Close();
		}

		public float highScore() {
			return m_flHighScore;
		}

		/****************************************************************************************
		 * Notifies of new score, setting it as the new high score if it is.
		 * Returns whether or not a new high score was set.
		 ***************************************************************************************/
		public bool notifyScore(float flScore) {
			bool newHighScore = flScore > m_flHighScore;
			if(newHighScore) {
				m_flHighScore = flScore;
				writeToFile();
			}
				
			return newHighScore;
		}

		public void writeToFile() {
			FileStream stream = new FileStream(s_sFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
			StreamWriter writer = new StreamWriter(stream);
			writer.Write(m_flHighScore);
			writer.Close();

			stream.Close();
		}

		public CScoreTable() {
			reloadFromFile();
		}
	}
}
