using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets {
	public partial class CBaseEntity : MonoBehaviour {

		//This simple accessor will help us type less later on
		public GameObject obj() { return gameObject; }

		/*
		 * Private variables for teleporting to original spawn location/angle
		 */
		private Vector3     m_vSpawnLocation;
		private Quaternion  m_qSpawnAngle;


		/**
		 * Health functionality
		 */
		private int		m_iHealth;
		public  int		m_iSpawnHealth = 100;
		public	int		GetHealth() { return m_iHealth; }
		//public	void	SetHealth(int iHealth) { m_iHealth = iHealth; }



		/**
		 * Respawn function
		 */
		public void Respawn() {
			//reset position
			obj().transform.SetPositionAndRotation(m_vSpawnLocation,m_qSpawnAngle);

			//reset bitflags
			m_iFlags = m_iSpawnFlags;

			//reset health
			m_iHealth = m_iSpawnHealth;
		}

		// Use this for initialization
		void Start() {

			//Remember default spawn values
			m_iSpawnFlags		= m_iFlags;
			m_vSpawnLocation	= obj().transform.position;
			m_qSpawnAngle = obj().transform.rotation;
		}

		// Update is called once per frame
		void Update() {

		}
	}
}
