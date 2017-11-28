using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    class CDoor : CBaseEntity
    {

		public bool bOpen;
		public bool bClosed;
        public bool bInvisible;
        public bool bWalkThrough;

        public float flTimeToOpen;
        public float flTimeToClose;

        public GameObject door;


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

		void DestroyDoor(GameObject door)
        {
            Destroy(door);
        }

		



    }
}