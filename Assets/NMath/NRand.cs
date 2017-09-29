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
	class NRand {
		private static int a = 9158152, b = 14257153;

		public static void seedFromTime() {
			a = (int) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		}

		public static int randInt() {
			a += b + (a >> 5);
			b += a + (b >> 11);

			return Math.Abs(b);
		}

		public static float randFloat() {
			a += b + (a >> 5);
			b += a + (b >> 11);

			return Math.Abs(b) * 1.0f / Int32.MaxValue;
		}

		public static float randFloat(float min, float max) {
			return min + randFloat() * (max - min);
		}

		public static int randInt(int min, int max) {
			return min + (randInt() % max + 1);
		}

		public static bool randBool(float chance = 0.5f) {
			return randFloat() < chance;
		}
	}
}
