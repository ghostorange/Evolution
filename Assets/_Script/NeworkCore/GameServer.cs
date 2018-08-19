using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using GOGameCore;
using System.Net;

public class GameServer : MonoBehaviour, INetEventListener
{
    
    private NetManager _netServer;
	private NetPeer _ThelastConnected;
    private NetDataWriter _dataWriter;
	private List<NetPeer> ConnectedClient;

    private NetEvent OnClientConnect;
    void Start()
    {
		
		ConnectedClient= new List<NetPeer>();
        _dataWriter = new NetDataWriter();
		_netServer = new NetManager(this);
        _netServer.Start(5000);
		_netServer.MaxConnectAttempts = 20;
        _netServer.DiscoveryEnabled = true;
        _netServer.UpdateTime = 15;
    
	}

    void Update()
    {
        _netServer.PollEvents();
    }

    void LateUpdate()
    {
    }

    void OnDestroy()
    {
        if (_netServer != null)
            _netServer.Stop();
    }
	void OnGetClientInput(ClientInput input){
		Debug.Log ("OnGetClientInput" + input.ClinetID + ">" + input.ToogledBtn.ToString ());
	}
	ClientInput OnConstrucClientInput(){
		Debug.Log ("OnGetClientInput");
		return new ClientInput();
	}
    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[SERVER] We have new peer " + peer.EndPoint);
        _ThelastConnected = peer;
		ConnectedClient.Add (peer);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectReason reason, int socketErrorCode)
    {
		Debug.Log ("[Disconnect] " + peer.ConnectId.ToString ());
    }

	public void OnNetworkError(IPEndPoint endPoint, int socketErrorCode)
    {
        Debug.Log("[SERVER] error " + socketErrorCode);
    }

	public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetDataReader reader,
        UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.DiscoveryRequest)
        {
            Debug.Log("[SERVER] Received discovery request. Send discovery response");
            _netServer.SendDiscoveryResponse(new byte[] {1}, remoteEndPoint);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        request.AcceptIfKey("sample_app");
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
        if (peer == _ThelastConnected)
            _ThelastConnected = null;
		if (ConnectedClient.Contains (peer)) {
			ConnectedClient.Remove (peer);
		} else {
			Debug.Log ("[SERVER] Programing Error");
		}
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader, DeliveryMethod deliveryMethod){
		
		if (ConnectedClient.Contains (peer)) {
			_dataWriter.Reset ();
			byte[] raw = new byte[reader.AvailableBytes];
			reader.GetBytes (raw,reader.AvailableBytes);
			_dataWriter.Put(raw);
			foreach (var client in ConnectedClient) {
				client.Send (_dataWriter, deliveryMethod);
			}
		}
	}
}

#region Netdata
/// <summary>
/// 联网事件
/// </summary>
public class NetEvent:IDecodeable {
    /// <summary>
    /// 网络调用数据
    /// </summary>

    /// <summary>
    /// 事件类型
    /// </summary>
    public enum EventType {
        OnPlayerConnected =0,
        OnPlayerDisconnected =1,
        OnSyncTransformData=2,
        OnNetSpawn = 3,
        OnNetDestroy =4,
        NetInvoke=5
    }
    public EventType eventType;
    private int _sender;
    public int sender {
        get {
            return _sender;
        }
    }
    /// <summary>
    /// OnPlayerConnected/OnPlayerDisconnected  => data is That player ID
    /// OnSyncTransformData                                     => data is TransformData
    /// OnNetSpawn/OnNetDestroy                           => data is ManagedObject Prefabe ID & NetInstanceID(CreatedByServer)
    /// NetInvoke                                                         => data is NetInvokeData
    /// </summary>
    public string data;
    private string baseDataFormate {
        get {
            string buf = "";
            buf += sender.ToString()+","+ eventType.ToString()+",";
            return buf;
        }
    }
    public NetEvent() {

    }
    public NetEvent(NetManager user,EventType type, INetData data) {
        if(user.GetFirstPeer().ConnectionState == ConnectionState.Connected) {
            this._sender = user.LocalPort;
            this.eventType = type;
        } else{
            throw new Exception("Can not Create an NetEvent by an unconnected NetworkState");
        }
    }

    public void Boardcast(NetDataWriter writer, NetPeer recever) {
    }

    public void Decode(string data) {
        throw new NotImplementedException();
    }

    public string Encode() {
        throw new NotImplementedException();
    }
}
public struct NetInvokeData :INetData, IDecodeable {
    private string methodName;
    private string methodParam;
    public string MethodName{
        get {
            return methodName;
        }
    }
    public string MethodParam {
        get {
            return methodParam;
        }
    }

    public void Deserialize(byte[] raw) {

    }

    public byte[] Serialize() {
        var fmt = methodName + "," + methodParam;
        return NetdataUltility.EncodeRaw(fmt);
    }

    public void Decode(string data) {
        throw new NotImplementedException();
    }

    public string Encode() {
        throw new NotImplementedException();
    }

    public NetInvokeData(string methodName,string parm) {
        this.methodName = methodName;
        this.methodParam = parm;
    }
}

[System.Serializable]
public class ClientInput :INetSerializable , IDecodeable {
    public int ClinetID;
	public float Axis0_X;
	public float Axis0_Y;
	public float Axis1_X;
	public float Axis1_Y;
	public int[] ToogledBtn;

	public void Serialize(NetDataWriter writer) {
		writer.Put(ClientInput.Serialize(this));
	}
    public void Deserialize(NetDataReader reader) {
		byte[] raw = new byte[reader.AvailableBytes];
		reader.GetBytes(raw,reader.AvailableBytes);
		ClientInput inputBuf = Deserialize (raw);
		this.ClinetID = inputBuf.ClinetID;
		this.Axis0_X = inputBuf.Axis0_X;
		this.Axis0_Y = inputBuf.Axis0_Y;
		this.Axis1_X = inputBuf.Axis1_X;
		this.Axis1_Y = inputBuf.Axis1_Y;
		this.ToogledBtn = inputBuf.ToogledBtn;
    }
	public override string ToString ()
	{
		string buf = "";
		buf += ClinetID.ToString ()+":";
		buf += Axis0_X.ToString ()+",";
		buf += Axis0_Y.ToString ()+",";
		buf += Axis1_X.ToString ()+",";
		buf += Axis1_Y.ToString ()+"-";
		foreach (var btn in ToogledBtn) {
			buf+= btn.ToString()+",";
		}
		//to remove final " "
		buf = buf.Remove (buf.Length - 1);
		return buf;
	}
	public void DecodeRawDataFromString(string raw){
		string[] id_input = raw.Split (':');
		string[] inputGroup = id_input [1].Split ('-');
		string[] axisData = inputGroup[0].Split(',');
		string[] btnData = inputGroup[1].Split(',');
		this.ClinetID = int.Parse (id_input [0]);
		this.Axis0_X = float.Parse(axisData [0]);		this.Axis0_Y = float.Parse(axisData [1]);
		this.Axis1_X = float.Parse(axisData [2]);		this.Axis1_Y = float.Parse(axisData [3]);
		this.ToogledBtn = new int[btnData.Length];
		for (int i=0;i<btnData.Length;i++) {
			this.ToogledBtn [i] = int.Parse (btnData [i]);
		}
	}
	public static byte[] Serialize(ClientInput input){
		byte[] buf = System.Text.Encoding.ASCII.GetBytes (input.ToString());
		return buf;
	}
	public static ClientInput Deserialize(byte[] raw){
		ClientInput input = new ClientInput ();
		input.DecodeRawDataFromString (System.Text.Encoding.ASCII.GetString(raw));
		return input;
	}

    public void Decode(string data) {
        DecodeRawDataFromString(data);
    }

    public string Encode() {
        return this.ToString();
    }
}
#endregion

public struct TransformData :INetData,IDecodeable {
    public int netSpawnedID;
    public Vector3 position;
    public Quaternion rotation;


    public string Encode() {
        return System.String.Format("{0},{1},{2},{3},{4},{5},{6},{7}", netSpawnedID, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w);
    }
    public void Decode(string data) {
        string[] dataSeg = data.Split(',');

    }

    public byte[] Serialize() {
        return NetdataUltility.EncodeRaw(Encode());
    }
    public void Deserialize(byte[] raw) {
        Decode(NetdataUltility.DecodeRaw(raw));
    }


 
}

public class NetPlayerData:INetData, IDecodeable {
    private int netID;
    private int playerType;
    private int playerState;
    public int NetID {
        get {
            return netID;
        }
    }
    public int PlayerType {
        get {
            return playerType;
        }
    }
    public int PlayerState {
        get {
            return playerState;
        }
    }

    public string Encode() {
        return System.String.Format("{0},{1},{2}", NetID, PlayerType, PlayerState); 
    }
    public void Decode(string data) {
        string[] toRaw = data.Split(',');
        if (toRaw.Length != 3) {
            throw new Exception("[NetPlayerData] Encode failed :" + data);
        } else {
            netID = int.Parse(toRaw[0]);
            playerType = int.Parse(toRaw[1]);
            playerType = int.Parse(toRaw[2]);
        }
    }
    public void Deserialize(byte[] raw) {
        Decode(NetdataUltility.DecodeRaw(raw));
    }
    public byte[] Serialize() {
        return NetdataUltility.EncodeRaw(Encode());
    }
}

public static class SerializeUltility {
    public static byte[] RawSerialize(object anything) {
        int rawsize = Marshal.SizeOf(anything);
        IntPtr buffer = Marshal.AllocHGlobal(rawsize);
        Marshal.StructureToPtr(anything, buffer, false);
        byte[] rawdatas = new byte[rawsize];
        Marshal.Copy(buffer, rawdatas, 0, rawsize);
        Marshal.FreeHGlobal(buffer);
        return rawdatas;
    }
    public static object RawDeserialize(byte[] rawdatas, Type anytype) {
        int rawsize = Marshal.SizeOf(anytype);
        if (rawsize > rawdatas.Length)
            return null;
        IntPtr buffer = Marshal.AllocHGlobal(rawsize);
        Marshal.Copy(rawdatas, 0, buffer, rawsize);
        object retobj = Marshal.PtrToStructure(buffer, anytype);
        Marshal.FreeHGlobal(buffer);
        return retobj;
    }
}