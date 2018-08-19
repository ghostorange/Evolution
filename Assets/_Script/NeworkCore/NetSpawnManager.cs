using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOGameCore{
    /// <summary>
    /// this is Local Work Manager, Just Instantiate Object and Link a NetWorkID,
    ///  and this ID in each client was same , Send form server
    /// </summary>
    public class NetSpawnManager : MonoBehaviour {
        private static NetSpawnManager mInstance;
        private static NetSpawnManager Instance {
            get {
                if(mInstance == null) {
                    mInstance = GameObject.FindObjectOfType<NetSpawnManager>();
                }
                return mInstance;
            }
        }
        private bool initialized {
            get {
                return ManagedSpawnable != null && SpawnedObject!=null && SpawnableIDBoostBuffer!=null;
            }
        }
        [SerializeField]
        private List<GameObject> ManagedSpawnable;
        private Dictionary<int, GameObject> SpawnedObject;
        private Dictionary<GameObject, int> SpawnableIDBoostBuffer;

        private void Awake() {
            SpawnedObject = new Dictionary<int, GameObject>();
            SpawnableIDBoostBuffer = new Dictionary<GameObject, int>();
        }


        public static int GetSpawnableObjectID(GameObject obj) {
            if (Instance.ManagedSpawnable.Contains(obj)) {
                return Instance.SpawnableIDBoostBuffer[obj];
            }
            throw new System.Exception("[NetSpawnManager] Cannot Spawn a Unmanaged GameObject Prefabe");
        }
        public static GameObject SpawnObject(int id,int setInstanceID) {
            GameObject clone = GameObject.Instantiate(Instance.ManagedSpawnable[id]);
            Instance.SpawnedObject.Add(setInstanceID, clone);
            return clone;
        }
        public static void DestroyObject(int instanceID) {
            if (Instance.SpawnedObject.ContainsKey(instanceID)) {
                GameObject.Destroy(Instance.SpawnedObject[instanceID]);
                Instance.SpawnedObject.Remove(instanceID);
            }
        }
        public static class SpawnedObjectUltility {
            public static GameObject Object(int netInstanceID) {
                if (Instance.SpawnedObject.ContainsKey(netInstanceID)) {
                    return Instance.SpawnedObject[netInstanceID];
                }
                throw new System.Exception("[SpawnedObjectUltility] Can find Net Spawned target : " + netInstanceID);
            }
        }
    }
}