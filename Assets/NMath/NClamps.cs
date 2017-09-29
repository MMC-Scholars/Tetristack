// This software is under partial ownership by The Ohio State University, 
//for it is a product of student employees. For official policy, see
//https://tco.osu.edu/wp-content/uploads/2013/09/PatentCopyrightPolicy.pdf 
//or contact The Ohio State University's Office of Legal Affairs
/**
 * @author Michael Trunk
 * @startdate	29/9/2017
 * @enddate		29/9/2017
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Assets {
	class NClamps {
		public static void clamp(ref int val, int min, int max) {
			if (val < min)
				val = min;
			else if (val > max)
				val = max;
		}
		public static void clampm(ref int val, int min) {
			val = val < min ? min : val;
		}

		public static void clampM(ref int val, int max) {
			val = val > max ? max : val;
		}
	}
}
