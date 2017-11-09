using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    class CBaseMoving : CBaseEntity
    {
        //door status
		public bool bIsOpen;
		public bool bIsClosed;
        public bool bIsLocked;

        //door movement status
        public bool bIsOpening;
        public bool bIsClosing;
        public bool bIsWaitingToClose;

        //how long object has been moving
        public float movementTime;

        //time before moving
        public float waitingTime;

        //Lerp values
        private float currentLerp;
        private float previousLerp;

        //door moveSpeed
        public float movementSpeed;

        //declaring methods for door functionality
        public virtual void SetSpeed(float SetSpeed) { }
        public virtual void OnOpened() { }
        public virtual void OnClosed() { }

        public virtual void OnUsedOpen() { }
        public virtual void OnUsedClosed() { }
        public virtual void OnUsedLocked() { }


        public float delayBeforeReset = -1.0f;


        void Open()
        {
            if (bIsLocked)
            {
                OnUsedLocked();
                return;
            }
            if (bIsOpening)
            {
                return;
            }
            bIsOpening = true;
            bIsClosing = false;

            if (delayBeforeReset > 0.0f)
            {
                bIsWaitingToClose = true;
                waitingTime = 0.0f;
            }


            OnOpened();

        }

        void Close()
        {
            if (bIsLocked)
            {
                OnUsedLocked();
                return;
            }
            if (bIsClosing)
            {
                return;
            }
            bIsClosing = true;
            bIsOpening = false;

            if (delayBeforeReset > 0.0f)
            {
                bIsWaitingToClose = false;
                waitingTime = 0.0f;
            }

            OnClosed();

        }

        void toggle()
        {
            if (bIsLocked)
            {
                OnUsedLocked();
                return;
            }

            if (bIsClosing || bIsClosed)
            {
                Open();
            }
            else
            {
                Close();
            }

        }

        void pause()
        {
            bIsClosing = false;
            bIsOpening = false;
        }














        





        
















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