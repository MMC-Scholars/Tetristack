// This software is under partial ownership by The Ohio State University, 
//for it is a product of student employees. For official policy, see
//https://tco.osu.edu/wp-content/uploads/2013/09/PatentCopyrightPolicy.pdf 
//or contact The Ohio State University's Office of Legal Affairs
/**
 * @author Michael Trunk
 * @startdate	29/9/2017
 * @enddate		29/9/2017
 */

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using UnityEngine;

namespace Assets {
	abstract partial class g {
		public	static float	curtime,
								prevtime,
								frametime;

		private static Stopwatch timer;

		private static bool g_bReinitialize = true;


		/**
		 * Marks that the game or editor should re-initialize globals
		 * on the next Start()
		 * This must be done to ensure that some globals are set before
		 *		all objects are loaded.
		 * Only call this on the last frame via OnApplicationQuit()
		 *  -All CBaseEntity already do this
		 */
		public static void MarkForReinitializationOnNextStart() {
			g_bReinitialize = true;
		}

		public static bool CheckForReinitializationOnStart() {
			bool bReInitialized = false;
			if (curtime < 0) {
				init();
				bReInitialized = true;
				CBaseEntity.g_aEntList = new List<CBaseEntity>();
			}
			return bReInitialized;
		}

		public static void init() {
			g_bReinitialize = false;

			curtime = 0.03f;
			prevtime = 0.0f;
			frametime = 0.03f;

			timer = new Stopwatch();
			timer.Start();

			NRand.seedFromTime();
		}

		/**
		 * Called by CGameRules to update global times
		 */ 
		public static void updateGlobals() {
			prevtime = curtime;
			curtime = (float) (1.0 * timer.ElapsedMilliseconds / 1000);
			frametime = curtime - prevtime;
		}
	}
}