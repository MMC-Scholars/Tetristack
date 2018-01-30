using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Valve.VR;
using UnityEngine;
using UnityEditor;

namespace Assets {

	public abstract partial class g {
		public const ulong	IN_TRIGGER  = (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Trigger));
		public const ulong	IN_GRIP     = (1UL << ((int)EVRButtonId.k_EButton_Grip));
		public const ulong	IN_B     = (1UL << ((int) EVRButtonId.k_EButton_ApplicationMenu));
		public const ulong  IN_A     = (1UL << ((int) EVRButtonId.k_EButton_A));
		public const ulong	IN_JOYSTICK = (1UL << ((int)EVRButtonId.k_EButton_SteamVR_Touchpad));
		public const uint   IN_NUM_BUTTONS = 4;
	}

	public delegate bool EntityControllerInputDelegate(CBaseEntity pEnt, CBaseController pController);

	[System.Serializable]
	public struct EntityControllerInputEvent {
		public CBaseEntity						m_pEnt;
		public EntityControllerInputDelegate	m_pFunction;

		public void Invoke(CBaseController pController) {
			m_pFunction(m_pEnt, pController);
		}
	}

	//This helps us handle pressed/released events for each button
	//so that we don't have be repetitive later on
	[System.Serializable]
	public class CButton {
		public CButton(ulong flag) { m_iFlag = flag; ResetEventList(); }

		public ulong m_iFlag;
		public List<EntityControllerInputEvent> m_pPressedEvents;
		public List<EntityControllerInputEvent> m_pReleasedEvents;

		public void PressedEvent(CBaseController pController) {
			for (int i = 0; i < m_pPressedEvents.Count; i++)
				m_pPressedEvents[i].Invoke(pController);
		}
		public void ReleasedEvent(CBaseController pController) {
			for (int i = 0; i < m_pReleasedEvents.Count; i++)
				m_pReleasedEvents[i].Invoke(pController);
		}

		public void ResetEventList() {
			m_pPressedEvents = new List<EntityControllerInputEvent>();
			m_pReleasedEvents = new List<EntityControllerInputEvent>();
		}
	}

	/****************************************************************************************
	* Buttons variable keeps a fast-access way to check what buttons are currently being
	* pressed or touched, from anywhere
	* 
	* Each controller has two button sets, one for pressed and one for touched.
	***************************************************************************************/
	[System.Serializable]
	public class CButtonSet {
		public CButtonSet(CBaseController pParent) {
			m_pParent = pParent;
			m_pButtons = new List<CButton>();
			m_pButtons.Add(new CButton(g.IN_TRIGGER));
			m_pButtons.Add(new CButton(g.IN_GRIP));
			m_pButtons.Add(new CButton(g.IN_A));
			m_pButtons.Add(new CButton(g.IN_B));
			m_pButtons.Add(new CButton(g.IN_JOYSTICK));
		}
		private CBaseController m_pParent;

		private ulong   m_iChangedButtons = 0UL;
		private ulong   m_iButtons = 0UL;
		private ulong   m_iLockedButtons = 0UL;
		public	ulong	GetButtons() { return m_iButtons; }
		public	ulong	GetButtonsChanged() { return m_iChangedButtons; }
		public	bool	HasButton(ulong iButton) { return (m_iButtons & iButton) > 0UL; }
		public	bool	HasButtonChanged(ulong iButton) { return (m_iChangedButtons & iButton) > 0UL; }
		public	bool	HasButtonLocked(ulong iButton) { return (m_iLockedButtons & iButton) > 0UL; }
		public	void	AddButton(ulong iButton) {
			if (!HasButtonLocked(iButton))
				m_iButtons |= iButton;
		}
		public	void	RemoveButton(ulong iButton) {
			if (!HasButtonLocked(iButton))
				m_iButtons |= ~iButton;
		}
		public	void	AddButtonLocked(ulong iButton) {
			m_iLockedButtons |= iButton;
		}
		public	void	RemoveButtonLock(ulong iButton) {
			m_iLockedButtons |= ~iButton;
		}

		//This is called from update to keep track of what controllers are being pressed
		//returns true if buttons have changed, false otherwise
		public	bool	UpdateToMatch(ulong iButtons) {

			ulong oldButtons = m_iButtons;

			ulong addedButtons		= iButtons & ~oldButtons;
			ulong removedButtons    = oldButtons & ~iButtons;

			//apply locks
			addedButtons &= ~m_iLockedButtons;
			removedButtons &= ~m_iLockedButtons;

			ulong changedButtons	= addedButtons | removedButtons;

			//change buttons
			m_iButtons ^= changedButtons;

			bool bChanged = m_iButtons != oldButtons;
			if (bChanged) {
				//check events
				//check pressed events first
				for (byte i = 0; i < g.IN_NUM_BUTTONS; i++) {
					CButton b = m_pButtons[i];
					if ((b.m_iFlag & addedButtons) > 0)
						b.PressedEvent(m_pParent);
					else if ((b.m_iFlag & removedButtons) > 0)
						b.ReleasedEvent(m_pParent);
				}
			}

			return bChanged;
		}
		public List<CButton> m_pButtons;
													
	}

	public partial class CBaseController : CBaseEntity {

		/****************************************************************************************
		* Two sets of buttons
		***************************************************************************************/
		public CButtonSet m_bsTouch;
		public CButtonSet m_bsPress;

		public CButtonSet ButtonsTouched() { return m_bsTouch; }
		public CButtonSet ButtonsPressed() { return m_bsPress; }

		public ulong m_nButtons;


		/****************************************************************************************
		* Button update keeps track of our pressed buttons
		***************************************************************************************/
		private bool	m_bEveryOtherFrame = false;
		void			UpdateButtons() {
			m_bEveryOtherFrame = !m_bEveryOtherFrame;
			if (m_bEveryOtherFrame || !m_bEnabled)
				return;

			ulong buttons = m_pController.controllerState.ulButtonTouched;
			m_bsTouch.UpdateToMatch(buttons);

			buttons = m_pController.controllerState.ulButtonPressed;
			m_bsPress.UpdateToMatch(buttons);

			m_nButtons = m_bsPress.GetButtons();
		}
	}
}
