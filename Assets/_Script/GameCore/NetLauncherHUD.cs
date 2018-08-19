using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOGameCore;

public class NetLauncherHUD : MonoBehaviour {
    private const int btnWidth = 85;
    private const int btnHeight = 35;
    public bool AutoDisplay = true;

    private bool Display;
    private void Awake() {
        Display = AutoDisplay;
    }
    private void OnGUI() {
        if (Display) {
            if (NetworkSystem.IsNetworkingGame) {
                Rect rec_Btn = new Rect(Screen.width - btnWidth*2, 0, btnWidth*2, btnHeight);
                if (GUI.Button(rec_Btn, "ShutdwonNetconnection")) {
                    NetworkSystem.NetLauncher.ShutdownNetConnection();
                }
            } else{
                Rect rec_ServerBtn = new Rect(Screen.width - btnWidth * 2 - 10, 0, btnWidth, btnHeight);
                Rect rec_ClientBtn = new Rect(Screen.width - btnWidth, 0, btnWidth, btnHeight);
                if (GUI.Button(rec_ServerBtn,"Host Server")) {
                    NetworkSystem.NetLauncher.LaunchAsServer();
                }
                if (GUI.Button(rec_ClientBtn, "Join Server")) {
                    NetworkSystem.NetLauncher.LaunchAsClient();
                }
            }
        }
    }
}
