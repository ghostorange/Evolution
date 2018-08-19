using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOGameCore {
    public static class NetdataUltility {
        private const string HeaderSplitSymble = ">";
        private static byte[] HeaderSplitSymbleRaw{
            get {
                return System.Text.Encoding.ASCII.GetBytes(HeaderSplitSymble);
            }
        }
        public enum NetdataType {
            EventData = 0,
            PlayerInputData = 1,
            SyncableData = 2,
            UnknownData = 3
        }
        private static bool TryGetPacktedDataType(string data, out NetdataType type) {
            type = NetdataType.UnknownData;
            string[] splited = data.Split(HeaderSplitSymble.ToCharArray());
            int id = splited.Length == 2 ? int.Parse (splited[0]) : -1;
            type = id != -1 ? (NetdataType)id : type;
            return id != -1;
        }
        public static byte[] EncodeRaw(string str) {
            if (str.Contains(HeaderSplitSymble)) {
                throw new System.Exception("EncodRaw Error : Can not Encode a data which contains '>' HeaderSplitSymble");
            }
            byte[] buf = System.Text.Encoding.ASCII.GetBytes(str);
            return buf;
        }
        public static string DecodeRaw(byte[] raw) {
            string str = System.Text.Encoding.ASCII.GetString(raw);
            return str;
        }
        public static bool EnableCheck;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] EncodePacketedDataRaw(string data) {
            
            byte[] buf = System.Text.Encoding.ASCII.GetBytes(data);
            return buf;
        }
        public static string DecodePacketData(byte[] raw) {
            string str = System.Text.Encoding.ASCII.GetString(raw);
            return str;
        }
        /// <summary>
        /// 打包数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string PacketData(NetdataType type,string data) {
            string packed = type.ToString() + HeaderSplitSymble + data;
            return packed;
        }
        /// <summary>
        /// 解包数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static NetdataType DepacketData(string data,out string dest) {
            dest = data;
            NetdataType type = NetdataType.UnknownData;
            if (TryGetPacktedDataType(data,out type)) {
                dest = data.Split(HeaderSplitSymble.ToCharArray())[1];
            }
            return type;
        }
        
    }
    public interface INetData {
        byte[] Serialize();
        void Deserialize(byte[] raw);
    }
    /// <summary>
    /// 网络数据类型
    /// 所有的网络数据Encode结果不得包含 “&”
    /// </summary>
    public interface IDecodeable {
        void Decode(string data);
        string Encode();
    }
}

