using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class CMoveLinear : CBaseMoving
    {

        Vector3 moveDirection =  new Vector3(0,1,0);
        Vector3 spawnLocation;

        protected override void processOpen()
        {
			//don't move if the grab buttons are pressed
			if (g.RightController().ButtonsPressed().HasButton(g.IN_TRIGGER)
				|| g.LeftController().ButtonsPressed().HasButton(g.IN_TRIGGER))
				return;

            float currentHeight = g.TowerBuilderRules().GetCurrentHeight();
            Vector3 idealLocation = spawnLocation + new Vector3(0, currentHeight, 0);
            Vector3 displacement = idealLocation - GetAbsOrigin();
            
            displacement *= movementSpeed;
            TeleportTo(displacement + GetAbsOrigin());
        }

        public override void Start()
        {
            base.Start();
            bIsOpening = true;
			spawnLocation = GetAbsOrigin();
        }
    }
}
