using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOGameCore {
    [RequireComponent(typeof(ObjectID))]
    public class MovementAction :MonoBehaviour, IMovementInvoker {
        private float moveSpeed;
        public float MoveSpeed {
            get {
                return moveSpeed;
            }

            set {
                moveSpeed = value;
            }
        }

        public Transform Actor {
            get {
                return transform;
            }
        }
        private ObjectID mIdentifycations;
        public int Identity {
            get {
                return mIdentifycations.Identity;
            }
        }

        public void InvokeMoveCMD(IMoveCMD cmd) {

        }
    }
}
