using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;

namespace GOGameCore{
    public class NetworkSystem:IManageable {
        public static class NetLauncher {
            private static GameServer ServerObject;
            private static GameClient ClientObject;
            #region NetGame Controll Interface
            /// <summary>
            /// 作为主机启动
            /// </summary>
            public static void LaunchAsServer() {
                if (ServerObject == null){
                    ServerObject = new GameObject("SERVER").AddComponent<GameServer>();
                    GameObject.DontDestroyOnLoad(ServerObject);
                    LaunchAsClient();
                }else{
                    Debug.LogError("[NetworkSystem] Cannot create multiple NetServer instance!");
                }
                isServer = true;
                isNetworkingGame = true;
            }
            /// <summary>
            /// 作为客户端启动并查找开放的主机
            /// </summary>
            public static void LaunchAsClient() {
                if(ClientObject == null){
                    ClientObject = new GameObject("Client").AddComponent<GameClient>();
                    GameObject.DontDestroyOnLoad(ClientObject);
                }else{
                    Debug.LogError("[NetworkSystem] Cannot create multiple NetClient instance!");
                }
                isClient = true;
                isNetworkingGame = true;

            }
            /// <summary>
            /// 关闭网络连接
            /// </summary>
            public static void ShutdownNetConnection() {
                if (ServerObject != null) {
                    GameObject.Destroy(ServerObject.gameObject);
                }
                if (ClientObject != null) {
                    GameObject.Destroy(ClientObject.gameObject);
                }
                isClient = false;
                isServer = false;
                isNetworkingGame = false;
                Debug.Log("[NetworkSystem] Network connection shuteddown");
            }
            #endregion
        }
        private static bool isClient;
        private static bool isServer;
        private static bool isNetworkingGame;
        #region Base State

        public static bool IsClient{
            get{
                return isClient;
            }
        }
        public static bool IsServer {
            get {
                return isServer;
            }
        }
        public static bool IsNetworkingGame {
            get {
                return isNetworkingGame;
            }
        }
        #endregion

        #region IManageable Content
        public int Pass {
            get {
                return -1000;
            }
        }
        public int Order{
            get {
                return -1000;
            }
        }

        public void Initialize() {

        }

        public void Uninitialize() {

        }
        #endregion
    }
}