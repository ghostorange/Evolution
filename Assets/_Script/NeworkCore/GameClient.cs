using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Net;
using System.Collections.Generic;
using GOGameCore;

public class GameClient : MonoBehaviour, INetEventListener
{
    public delegate void NetInvokeHandler<T>(T t);
    private NetManager clientInstance;
	private NetPeer connectedServer;
    private List<NetPlayerData> Players;
    private class NetInvoker<T> {
        public string methodName;
        NetInvokeHandler<T> NetInvokeable;
    }
    public void RegionstorNetInvokeble<T>(string name, NetInvokeHandler<T> method) {

    }
    void Start()
	{	
		connectedServer = null;
        clientInstance = new NetManager(this);
        clientInstance.Start();
        clientInstance.UpdateTime = 15;
		writer = new NetDataWriter ();
    }
	private NetDataWriter writer;
	public KeyCode[] RegionKecode = new KeyCode[]{KeyCode.W,KeyCode.S,KeyCode.A,KeyCode.D,KeyCode.LeftControl,KeyCode.Space,KeyCode.Q,KeyCode.E,KeyCode.R,KeyCode.F,
		KeyCode.UpArrow,KeyCode.DownArrow,KeyCode.LeftArrow,KeyCode.RightArrow,KeyCode.Mouse0,KeyCode.Mouse1,KeyCode.Mouse2};
    void FixedUpdate()
    {
        clientInstance.PollEvents();
        var peer = clientInstance.GetFirstPeer();
        if (peer != null && peer.ConnectionState == ConnectionState.Connected)
        {
            // do something here
        }
        else
        {
            clientInstance.SendDiscoveryRequest(new byte[] {1}, 5000);
        }
		writer.Reset();
		bool inputed =false;
		if (connectedServer != null) {
			List<int> Keys = new List<int> ();
			writer.Reset ();
			foreach (var key in RegionKecode) {
				if (Input.GetKeyDown (key)) {
					inputed = true;
					Keys.Add ((int)key);
                    Debug.Log("IN " + (int)key);
                }
            }
			if (inputed) {
				ClientInput input = new ClientInput ();
				input.ToogledBtn = Keys.ToArray ();
				input.ClinetID = clientInstance.LocalPort;
				input.Serialize (writer);
				peer.Send (writer, DeliveryMethod.Sequenced);
				Keys = null;
            }
        }
    }
    void OnDestroy()
    {
        if (clientInstance != null)
            clientInstance.Stop();
    }
		
    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[CLIENT] We connected to " + peer.EndPoint);
		connectedServer = peer;
    }

	public void OnNetworkError(IPEndPoint endPoint, int socketErrorCode)
    {
        Debug.Log("[CLIENT] We received error " + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader, DeliveryMethod deliveryMethod)
	{
  //      byte[] raw = new byte[reader.AvailableBytes];
		//reader.GetBytes (raw, reader.AvailableBytes);
		//string str = System.Text.Encoding.ASCII.GetString (raw);
		//ClientInput input = new ClientInput ();
		//input.DecodeRawDataFromeString (str);

  //      if (input.ClinetID == _netClient.LocalPort) {
  //          Debug.Log("[YOU]  inputed>" + input.ToString());
  //      } else {
  //          Debug.Log("[OTHER]  inputed>"+ input.ToString());
  //      }
        byte[] raw = new byte[reader.AvailableBytes];
        reader.GetBytes(raw, reader.AvailableBytes);
        string str = NetdataUltility.DecodeRaw(raw);
        NetdataUltility.NetdataType dataType = NetdataUltility.NetdataType.UnknownData;
        string decoded = "";
        dataType = NetdataUltility.DepacketData(str, out decoded);
        switch (dataType) {
            case NetdataUltility.NetdataType.EventData:
                NetEvent nEvent = new NetEvent();
                nEvent.Decode(decoded);
                OnNetEvent(nEvent);
                break;
            case NetdataUltility.NetdataType.PlayerInputData:
                ClientInput cInput = new ClientInput();
                cInput.Decode(decoded);
                ExecuteInpute(cInput);
                break;
            case NetdataUltility.NetdataType.SyncableData:
                break;
            case NetdataUltility.NetdataType.UnknownData:
                //Maybe drop this data ??
                Debug.Log("[GameClient] receved a unknownData form server !");
                break;
        }
    }
    private void OnNetEvent(NetEvent netEvent) {
        string rawData = netEvent.data;
        switch (netEvent.eventType) {
            case NetEvent.EventType.OnPlayerConnected:
                NetPlayerData cnntPL = new NetPlayerData();
                cnntPL.Decode(rawData);
                OnPlayerConnected(cnntPL);
                break;
            case NetEvent.EventType.OnPlayerDisconnected:
                NetPlayerData dcnntPL = new NetPlayerData();
                dcnntPL.Decode(rawData);
                OnPlayerDisConnected(dcnntPL);
                break;
            case NetEvent.EventType.OnSyncTransformData:
                break;
            case NetEvent.EventType.OnNetSpawn:
                break;
            case NetEvent.EventType.OnNetDestroy:
                break;
            case NetEvent.EventType.NetInvoke:
                break;
        }
    }
    #region ¡ü¡ü¡ü¡ü Sub Event Method of OnNetEvent
    private void OnPlayerConnected(NetPlayerData id) {

    }
    private void OnPlayerDisConnected(NetPlayerData id) {

    }
    private void OnSyncTransformData(int id) {

    }
    private void OnNetSpawn(int prefabeID,int InstanceDestID) {

    }
    private void OnNetDestroy(int instanceID) {

    }
    private void ExecuteNetInvoke(NetInvokeData InvokeData) {

    }
    #endregion
    private void ExecuteInpute(ClientInput inputData) {
        if (inputData.ClinetID == clientInstance.LocalPort) {
            Debug.Log("[YOU]  inputed>" + inputData.ToString());
        } else {
            Debug.Log("[OTHER]  inputed>" + inputData.ToString());
        }
    }


    private void ExecuteSyncData() {

    }


    #region Base Interfaces
    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetDataReader reader,
        UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.DiscoveryResponse && clientInstance.PeersCount == 0)
        {
            Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
            clientInstance.Connect(remoteEndPoint, "sample_app");
        }
    }
    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
		
    }
    public void OnConnectionRequest(ConnectionRequest request)
    {
        Debug.Log("OnConnectionRequest"+request.Peer.EndPoint);   
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[CLIENT] We disconnected because " + disconnectInfo.Reason);
    }
    #endregion
}