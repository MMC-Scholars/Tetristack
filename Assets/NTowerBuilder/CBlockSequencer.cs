using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets;

namespace Assets {
	class CBlockSequencer : CBaseEntity {
		/**
		 * References to CBaseBlock used as templates which are duplicated
		 * to make more blocks
		 */
		public GameObject	m_pI,
							m_pO,
							m_pT,
							m_pL,
							m_pS;

		/**
		 * References to CBaseBlock used as templates which are duplicated
		 * to make more blocks
		 */
		private CBaseBlock  I,
							O,
							T,
							L,
							S;

		Queue<CBaseBlock>   m_qBlocks; //current queue of blocks to duplicate


		/**
		 * Because C#'s list interface is apparantly void of functionality
		 */
		static void swap(List<CBaseBlock> s,int i,int j) {
			CBaseBlock ei = s.ElementAt(i);
			s.RemoveAt(i);
			s.Insert(i,s.ElementAt(j));
			s.RemoveAt(j);
			s.Insert(j,ei);
		}

		/**
		 * Returns a list of Tetromino templates. The list will be of length 7,
		 * and it will contain one of each Tetronimo, but with 2 L
		 * and 2 S because each of those represent two blocks.
		 * In random order.
		 */
		Queue<CBaseBlock> nextTetrisPieceSequence() {
			//Just build the list first and then we'll randomly swap around
			List<CBaseBlock> s = new List<CBaseBlock>();
			s.Add(I);
			s.Add(O);
			s.Add(T);
			s.Add(L);
			s.Add(L);
			s.Add(S);
			s.Add(S);

			//what kind of list interface doesn't have a swap functionality?
			for(int i = 0; i < 20; i++) {
				int j = NRand.randInt(0, 6);
				int k = NRand.randInt(0, 6);
				swap(s,j,k);
			}

			//we don't actually need the enum
			Queue<CBaseBlock> q = new Queue<CBaseBlock>(s);
			return q;
		}

		/**
		 * Duplicates the next piece to the given position
		 */
		public CBaseBlock nextBlock(Vector3 pos) {
			if (m_qBlocks.Count() == 0) {
				m_qBlocks = nextTetrisPieceSequence();
			}
			CBaseBlock source = m_qBlocks.Dequeue();
			CBaseBlock blk = Instantiate(source.obj(),pos,new Quaternion()).GetComponent<CBaseBlock>();

			blk.m_pSource = source;
			blk.AddFlags(FL_NODAMAGE | FL_DESTROY_ON_RESPAWN); //so that blocks are removed on restart round
			return blk;
		}

		public override void Start() {
			base.Start();
			m_qBlocks = new Queue<CBaseBlock>();

			//get CBaseBlock references from objects
			I = g.ToBaseBlock(m_pI);
			O = g.ToBaseBlock(m_pO);
			T = g.ToBaseBlock(m_pT);
			L = g.ToBaseBlock(m_pL);
			S = g.ToBaseBlock(m_pS);
		}
	}
}	
