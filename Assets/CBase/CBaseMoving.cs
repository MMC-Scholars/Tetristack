using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    class CBaseMoving : CBaseEntity
    {

		public bool bOpen;
		public bool bClosed;
        public bool bInvisible;
        public bool bWalkThrough;

        public float flTimeToOpen;
        public float flTimeToClose;

        private GameObject door;


		void OpenDoor(bool IsOpen, float TimeToOpen)
        {
			//Timer until TimeToOpen, then...
			
            IsOpen = true;
        }

		void CloseDoor(bool IsOpen, float TimeToClose)
        {
            //Timer until TimeToClose, then...

            IsOpen = false;
        }

		//DestroyDoor()

		







    }
}