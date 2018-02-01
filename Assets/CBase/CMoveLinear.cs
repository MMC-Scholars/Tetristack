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
            float currentHeight = g.TowerBuilderRules().getCurrentHeight();
            Vector3 idealLocation = spawnLocation + new Vector3(0, currentHeight, 0);
            Vector3 displacement = idealLocation - GetAbsOrigin();
            
            displacement *= movementSpeed;
            TeleportTo(displacement + GetAbsOrigin());

        }

        private void Start()
        {
            
            base.Start();
            bIsOpening = true;
            spawnLocation = GetAbsOrigin();


        }
    }
}
