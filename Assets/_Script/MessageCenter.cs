using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MyTools.IO; 

namespace MyTools{
	public class MessageCenter <T> {
		private struct EventPack{
			public string eventName;
			public object data;
			public static EventPack Create(string _eventName,object _data){
				EventPack item = new EventPack ();
				item.eventName = _eventName;
				item.data = _data;
				return item;
			}
			public override string ToString ()
			{
				return string.Format (eventName+">"+data.ToString());
			}
		}
		private static List<EventPack> EventHistory;
		private Dictionary<string ,EventHandlerT<T>> mEvenDictionary = new Dictionary<string, EventHandlerT<T>>(); 
		public delegate void EventDataNotifyHandler(int ID,string Event,T data);
		public delegate void EventHandler();
		public delegate void EventHandlerT<T>(T eventData);
		private static MessageCenter<T> mInstance;
		/// <summary>
		/// Occurs when on will send event.
		/// </summary>
		public static event EventDataNotifyHandler OnWillSendEvent;
		static MessageCenter<T> Instance{
			get{
				if (mInstance == null) {
					EventHistory = new List<EventPack> ();
					mInstance = new MessageCenter<T> ();
					mInstance.mEvenDictionary = new Dictionary<string, EventHandlerT<T>> ();
					mInstance.TriggerInfo = new Dictionary<EventInfo<T>, EventHandler> ();
					UnityEngine.SceneManagement.SceneManager.sceneUnloaded += mInstance.Uninitialize;
					Debug.Log("[MessageCenter] initiaLized : "+mInstance.GetType().ToString());
				}
				return mInstance;
			}
		}

		private static bool initiaLized{
			get{ 
				return !(Instance == null);
			}
		}

		private void Uninitialize(UnityEngine.SceneManagement.Scene arg0){
			mInstance = null;
			//Out put Event History
			string data = "";
			foreach(var item in EventHistory){
				data += item.ToString () + "\n";
			}
			data += "######################################################"+System.DateTime.Now.ToString ()+"######################################\n";
			myFile mfile = new myFile ();
			mfile.FloaderCheck ("./data");
			mfile.WriteToText (data, "./data/EventLogger.log", System.IO.FileMode.Append);
			Debug.Log("[MessageCenter] Event History was saved at './data/EventLogger.log'");
			mfile = null;
			Debug.Log("[MessageCenter] has been uninitiaLized");
		}
		private static int EventID;
		private static bool blocked = false;
		/// <summary>
		/// 阻止当前消息发送
		/// </summary>
		/// <param name="eventID">通过OnWillSendEvent获取到的事件ID.</param>
		public static void GrabEvent(int eventID){
			if (eventID == EventID) {
				blocked = true;
			}
		}
		private static System.Random randCreator;
		private static int GetRandID(){
			if (randCreator == null) {
				randCreator = new System.Random (9571204);
			}
			return randCreator.Next (10000,99999);
		}
		/// <summary>
		/// Sends the message.
		/// </summary>
		/// <param name="eventName">Event name.</param>
		/// <param name="eventData">Event data.</param>
		public static void SendMessage(string eventName,T eventData){
			blocked = false;
			EventID = GetRandID ();
			if (Instance.mEvenDictionary.ContainsKey (eventName)) {
				if (Instance.mEvenDictionary [eventName] != null) {
					if (OnWillSendEvent != null) {
						OnWillSendEvent.Invoke (EventID,eventName,eventData);
					}
					if (!blocked) {
						Instance.mEvenDictionary [eventName] (eventData);
						EventHistory.Add (EventPack.Create (eventName, eventData));
					} else {
						Debug.Log("[MessageCenter] send Event was blocked: "+eventName+" > "+eventData.ToString());
					}
				} else {
					Debug.LogWarning ("[MessageCenter] UNNORMAL ERROR");
				}
			} else {
				Debug.LogWarning ("[MessageCenter] NO SUCH EVENT NAME FOUND  :"+eventName);
			}
			EventInfo<T> key = EventInfo<T>.Create (eventName, eventData);
			if(Instance.TriggerInfo.ContainsKey(key)){
				if (Instance.TriggerInfo [key] != null) {
					Instance.TriggerInfo [key] ();
				} else {
					Debug.LogWarning ("[MessageCenter] NO SUCH EVENT NAME FOUND  :"+eventName+" eventData");
				}
			}
		}
		/// <summary>
		/// Adds the listener.
		/// </summary>
		/// <param name="eventName">Event name.</param>
		/// <param name="call">Call.</param>
		public static void AddListener(string eventName,EventHandlerT<T> call){

			if (Instance.mEvenDictionary.ContainsKey (eventName)) {
				Instance.mEvenDictionary [eventName] += call;
			} else {
				Instance.mEvenDictionary.Add (eventName,call);
				//Instance.mEvenDictionary [eventName] += call;
			}
			Debug.LogWarning ("[MessageCenter] LISTENER ADD  :"+eventName);

		}
		public static void RemoveListener (string eventName,EventHandlerT<T> call){

			if (Instance.mEvenDictionary.ContainsKey (eventName)) {
				Instance.mEvenDictionary [eventName] -= call;
			} else {
				Debug.LogWarning ("[MessageCenter] NO SUCH EVENT NAME FOUND");
			}
		}
		#region 20180509
		private Dictionary<EventInfo<T>,EventHandler> TriggerInfo;
		private struct EventInfo<T>
		{
			public string eventName;
			public T eventData;
			public static EventInfo<T> Create(string name , T data){
				EventInfo<T> inf = new EventInfo<T> ();
				inf.eventName = name;
				inf.eventData = data;
				return inf;
			}
		}
		/// <summary>
		/// 添加一个指定eventName和eventData的委托，作为触发器
		/// 当MessageCenter<T>在发送事件的时候会自动检测eventName 和 eventData 然后调用对应的委托
		/// </summary>
		/// <param name="eventName">Event name.</param>
		/// <param name="eventData">Event data.</param>
		/// <param name="call">Call.</param>
		public static void AddTrigger(string eventName,T eventData,EventHandler call){
			EventInfo<T> key = EventInfo<T>.Create(eventName,eventData);
			if (Instance.TriggerInfo.ContainsKey (key)) {
				Instance.TriggerInfo[key]+=call;
			} else {
				Instance.TriggerInfo.Add (key, call);
			}
		}
		public static void RemoveTrigger(string eventName,T eventData,EventHandler call){
			EventInfo<T> key = EventInfo<T>.Create(eventName,eventData);
			if (Instance.TriggerInfo.ContainsKey (key)) {
				try{
					Instance.TriggerInfo[key] -= call;
				}catch{
					Debug.LogWarning ("[MessageCenter] NO SUCH TRIGGER FOUND");
				}
			} else {
				Debug.LogWarning ("[MessageCenter] NO SUCH TRIGGER FOUND");
			}
		}
		#endregion
	}
	[System.Serializable]
	public class MsgContent{
		private Type parmaterType;
		private object pramValue;
		private MsgContent(){
		}
		public MsgContent(object parm){
			pramValue = parm;
			parmaterType = parm.GetType ();
		}

		public object TryGetValue(out Type type){
			var ppts = parmaterType.GetProperties (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.GetField);
			if (ppts == null) {
				type = parmaterType;
				return pramValue;
			} else {
				type = ppts [0].GetType ();
				return ppts [0].GetValue (pramValue, null);
			}
		}
	}
}