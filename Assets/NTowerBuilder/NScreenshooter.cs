using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets {

	/**
	 * This namespace handles camera-to-external-location operations
	 */ 
	abstract class NScreenshooter {

		/**
		 * Given a camera, renders the view of the camera onto the given texture.
		 * The square image will completely fill the resolution of the given texture.
		 * Restores camera's original parameters
		 */ 
		public static void RenderToTexture2D(Camera pCamera, Texture2D pTexture) {
			//remember camera's aspect ratio so it can be restored later
			float originalAspect = pCamera.aspect;
			RenderTexture originalTarget = pCamera.targetTexture;

			//set aspect ratio to square
			pCamera.aspect = 1.0f;

			//get resolution
			int res = Math.Min(pTexture.width, pTexture.height);

			RenderTexture tempRT = new RenderTexture(res, res, 24);
			tempRT.Create();
			pCamera.targetTexture = tempRT;
			pCamera.Render();
			pCamera.targetTexture = originalTarget; //avoid later errors

			//set global RenderTexture reference to our own
			RenderTexture.active = tempRT;

			//load global RenderTexture to the Texture2D
			pTexture.ReadPixels(new Rect(0,0,res,res),0,0);

			//destory the RenderTexture to ensure that memory is released
			RenderTexture.active = null;
			tempRT.Release();
			Component.Destroy(tempRT);

			//restore original aspect ratio
			pCamera.aspect = originalAspect;
		}

		/**
		 * Rengers 
		 */ 
		public static void RenderToPNG(Camera pCamera, int iRes, string sRelativePath) {
			//just create a new texture, render to that, and export from there
			Texture2D tmp = new Texture2D(iRes, iRes, TextureFormat.RGB24, false);

			RenderToTexture2D(pCamera, tmp);

			byte[] imageData = tmp.EncodeToPNG();
			string filepath = Application.persistentDataPath + sRelativePath;
			System.IO.File.WriteAllBytes(filepath,imageData);

			//ensure memory is released
			Texture2D.Destroy(tmp);
		}
	}
}
