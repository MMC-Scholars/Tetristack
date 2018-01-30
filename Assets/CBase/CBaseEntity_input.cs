// This software is under partial ownership by The Ohio State University, 
//for it is a product of student employees. For official policy, see
//https://tco.osu.edu/wp-content/uploads/2013/09/PatentCopyrightPolicy.pdf 
//or contact The Ohio State University's Office of Legal Affairs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets {

	/****************************************************************************************
	* These variables and hooks are used on start-up to link any entity to controller input
	***************************************************************************************/
	public partial class CBaseEntity {

		/****************************************************************************************
		* Call this function to hook a function to a given button input on the given controller
		* @param iButton can have multiple buttons
		* Call this from an overriden InitControllerInputs() function
		***************************************************************************************/
		public void HookInputFunction(CBaseController pController, ulong iButton, bool bTouchInput, bool bActivateOnRelease, EntityControllerInputDelegate pFunction) {
			//Find our button object to register with
			CButtonSet bs = bTouchInput ? pController.ButtonsTouched() : pController.ButtonsPressed();

			List<CButton> bl = new List<CButton>();
			for (int i = 0; i < bs.m_pButtons.Count; i++) {
				if ((bs.m_pButtons[i].m_iFlag & iButton) > 0)
					bl.Add(bs.m_pButtons[i]);
			}

			EntityControllerInputEvent ecie;
			ecie.m_pEnt = this;
			ecie.m_pFunction = pFunction;

			for (int i = 0; i < bl.Count; i++) {
				CButton b = bl[i];
				if (bActivateOnRelease)
					b.m_pReleasedEvents.Add(ecie);
				else
					b.m_pPressedEvents.Add(ecie);
			}
		}

		//this hooks the input events to our generic use functionality
		public bool m_bInputEventsTriggersUse;
		public float m_flInputEventRadius = 128.0f;
		//public bool m_bCollisionRequiredForUse = true;
		public bool m_bUseTogglesPickup = false;
		private static bool Hook_Use(CBaseEntity pEnt, CBaseController pController) {
			if (pEnt.GetOffsetTo(pController).magnitude < pEnt.m_flInputEventRadius)
				return pEnt.Use(pController);
			return false;
		}

		/****************************************************************************************
		* These next functions and variables link our booleans to the events for each controller
		***************************************************************************************/
		[System.Serializable]
		public class CButtonEnableList {
			public bool m_bTrigger  = false,
						m_bGrip     = false,
						m_bA        = false,
						m_bB        = false,
						m_bJoystick	= false;

			public ulong GetFlags() {
				ulong flags = 0UL;
				if (m_bTrigger)		flags |= g.IN_TRIGGER;
				if (m_bGrip)		flags |= g.IN_GRIP;
				if (m_bA)			flags |= g.IN_A;
				if (m_bB)			flags |= g.IN_B;
				if (m_bJoystick)	flags |= g.IN_JOYSTICK;
				return flags;
			}
		}

		[System.Serializable]
		public class CButtonInputSet {
			public CButtonEnableList m_pPressPressed	= new CButtonEnableList(); 
			public CButtonEnableList m_pPressReleased	= new CButtonEnableList();
			public CButtonEnableList m_pTouchPressed	= new CButtonEnableList();
			public CButtonEnableList m_pTouchReleased	= new CButtonEnableList();

			public void Start(CBaseController pController, CBaseEntity pEnt) {
				//builds flags from our booleans
				ulong pressPressedFlags = m_pPressPressed.GetFlags();
				ulong pressReleasedFlags = m_pPressReleased.GetFlags();
				ulong touchPressedFlags = m_pTouchPressed.GetFlags();
				ulong touchReleasedFlags = m_pTouchReleased.GetFlags();

				CButtonSet touch = pController.ButtonsTouched();
				CButtonSet press = pController.ButtonsPressed();

				//iterate through all the buttons
				if (pEnt.m_bInputEventsTriggersUse) { 
					EntityControllerInputEvent input = new EntityControllerInputEvent();
					input.m_pEnt = pEnt;
					input.m_pFunction = CBaseEntity.Hook_Use;

					for (int i = 0; i < g.IN_NUM_BUTTONS; i++) {
						CButton b = press.m_pButtons[i];
						if ((pressPressedFlags & b.m_iFlag) > 0)
							b.m_pPressedEvents.Add(input);
						if ((pressReleasedFlags & b.m_iFlag) > 0)
							b.m_pReleasedEvents.Add(input);
					}
					for (int i = 0; i < g.IN_NUM_BUTTONS; i++) {
						CButton b = touch.m_pButtons[i];
						if ((touchPressedFlags & b.m_iFlag) > 0)
							b.m_pPressedEvents.Add(input);
						if ((touchReleasedFlags & b.m_iFlag) > 0)
							b.m_pReleasedEvents.Add(input);
					}
				}
			}
		}

		public CButtonInputSet m_pLeftControllerInputs = null;
		public CButtonInputSet m_pRightControllerInputs = null;

		public virtual void InitControllerInputs() {
			CBaseController left = g.LeftController();
			CBaseController right = g.RightController();

			if (m_pLeftControllerInputs == null) m_pLeftControllerInputs = new CButtonInputSet();
			if (m_pRightControllerInputs == null) m_pRightControllerInputs = new CButtonInputSet();

			m_pLeftControllerInputs.Start(left, this);
			m_pRightControllerInputs.Start(right, this);
		}

		/****************************************************************************************
		* Pickup functionality
		***************************************************************************************/
		public	void SetIsTouchedByController(bool bTouch) { m_bIsTouchedByController = bTouch; }
		private bool m_bIsTouchedByController;
		private bool m_bIsPickedUp;

		void Pickup(CBaseController pController) {
			if (pController.m_pColliding.Contains(this)
				&& pController.ButtonsPressed().HasButton(g.IN_TRIGGER)) {
				m_bIsPickedUp = true;
				SetAbsVelocity(Vector3.zero);
				SetAngVelocity(Vector3.zero);
				SetParent(pController);
				SetGravityEnabled(false);
				OnPickedUp(pController);
			}
		}
		void Drop(CBaseController pController) {
			if (GetParent() is CBaseController
				&& !pController.ButtonsPressed().HasButton(g.IN_TRIGGER)) {
				m_bIsPickedUp = false;
				SetAbsVelocity(pController.GetAbsVelocityManualCalc());
				GetTransform().SetParent(null);
				SetGravityEnabled(m_bHasGravityEnabledByDefault);
				OnDropped(pController);
			}
		}

		public virtual void OnPickedUp(CBaseController pController) {}
		public virtual void OnDropped(CBaseController pController) {}
	}
}
