using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets {


	public partial class CBaseEntity {
		private AudioSource m_pAudioSource = new AudioSource();

		public void EmitSound(AudioClip sound) {
			m_pAudioSource.PlayOneShot(sound);
		}
	}
}
