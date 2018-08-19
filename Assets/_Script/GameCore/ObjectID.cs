using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOGameCore{
    /// <summary>
    /// 对象唯一识别码
    /// </summary>
    [ExecuteInEditMode,DisallowMultipleComponent]
    public class ObjectID : MonoBehaviour, IRecognizable {
        [SerializeField]
        private int mID;
        public int Identity {
            get {
                if(mID == 0) {
                    mID = GameManager.IDProvider.RequestID(this);
                }
                return mID;
            }
            private set {
                mID = value;
            }
        }
        void Awake() {
            ObjectID[] identifyedPool = GameObject.FindObjectsOfType<ObjectID>();
            foreach(var item in identifyedPool) {
                if(item.Identity == this.Identity && item!= this) {
                    Identity = GameManager.IDProvider.RequestID(this);
                    Debug.Log("[ObjectID] "+this.name+" Auto Reset ID " + Identity);
                }
            }
        }
        void Reset() {
            Debug.Log("[ObjectID] "+ Identity);
        }
    }
}