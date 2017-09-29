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
		public static float		curtime = 0.1f,
								prevtime = 0.0f,
								frametime = 0.1f;

		private static Stopwatch timer;

		public static void init() {
			timer = new Stopwatch();
			timer.Start();
			curtime = (float) (1.0 * timer.ElapsedMilliseconds / 1000);
		}

		public static void updateGlobals() {
			prevtime = curtime;
			curtime = (float) (1.0 * timer.ElapsedMilliseconds / 1000);
			frametime = curtime - prevtime;
		}
	}
}