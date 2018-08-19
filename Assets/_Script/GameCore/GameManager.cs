using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace GOGameCore{

    /// <summary>
    /// 被GameManager管理，继承此接口时，尽量不要使用Awake进行初始化
    /// </summary>
    public interface IManageable {
        int Pass{ get; }
        int Order{ get; }
        void Initialize();
        void Uninitialize();
    }
    public class GameManager : MonoBehaviour{

        private void Awake() {
            
        }

        #region Update
        /// <summary>
        /// to Sync All Command
        /// </summary>
        private void FixedUpdate() {
            
        }
        void OnGetCommandRequest(ICommand cmd,IInvoker invoker) {
            
        }
        void Update() {

        }

        #endregion
        /// <summary>
        /// Send Request
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="invoker"></param>
        public static void ExecuteCommand(ICommand cmd,IInvoker invoker) {
            if (NetworkSystem.IsNetworkingGame) {
                OnMultiPlayerRequest(cmd, invoker);
            } else {
                OnSinglePlayerRequest(cmd, invoker);
            }
        }

        #region Internal
        /// <summary>
        /// If Game is Under singlePlayer mode
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="invoker"></param>
        static void OnSinglePlayerRequest(ICommand cmd, IInvoker invoker) {

        }
        /// <summary>
        /// If Game is Under multiPlayer mode
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="invoker"></param>
        static void OnMultiPlayerRequest(ICommand cmd, IInvoker invoker) {
        }
        #endregion

        #region Ultilitys
        /// <summary>
        /// 游戏快照创建还原工具
        /// </summary>
        public static class GameSanpShotManager {
            private class SanpShotData:IDecodeable {
                //dataSegmentSplit : "-" //use this in group
                //dataSplit :"/"                //use this in parameter
                public NetPlayerData[] Players;
                public int[] NetSpawnedObjects;

                public void Decode(string data) {

                }

                public string Encode() {
                    string fmt = "";

                    return fmt;
                }
                //And Other Gaming Data Here
            }
        }
        public static class ObjectPoolingManager {
        }
        /// <summary>
        /// 随机数生成器
        /// </summary>
        public static class GameRandom {
            private static System.Random randCreator;
            private static System.Random RandCreator {
                get {
                    if (randCreator == null) {
                        randCreator = new System.Random(System.DateTime.Now.Millisecond);
                    }
                    return randCreator;
                }
            }
            public static void SetRandSeed(int seed) {
                randCreator = new System.Random(seed);
            }
            public static int GetRand(int start,int end) {
                return randCreator.Next(start, end);
            }
        }
        /// <summary>
        /// ID生成器
        /// </summary>
        public static class IDProvider{
            private const int startID = 10000000;
            private const int endID = 99999999;

            public static void SetRandSeed(int seed) {

            }
            public static int RequestID(IRecognizable target) {
                return GameRandom.GetRand(startID, endID);
            }
        }
        public static class ExternalFileLoader {
            public static byte[] LoadData(string path) {
                byte[] data = new byte[] { };
                

                return data;
            }
        }
        public static class GameRecorder {

        }
        public static class GameReplayer {
            public static void RecordCMD(ICommand cmd,IInvoker invoker) {

            }
        }
        #endregion
    }
}

