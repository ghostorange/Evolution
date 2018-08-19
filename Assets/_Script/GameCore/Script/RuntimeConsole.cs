using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeConsole : MonoBehaviour {
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	static void InitializeUltiliy(){
		GameObject LogUltility = new GameObject ("Console");
		instance = LogUltility.AddComponent<RuntimeConsole> ();
	}
	private static RuntimeConsole instance;
    // Use this for initialization
	void Awake () {
        LogData = new List<string>();
		Application.logMessageReceived+= Application_logMessageReceived;
	}
	void OnDestroy(){
		Application.logMessageReceived-= Application_logMessageReceived;
	}
	void Application_logMessageReceived (string condition, string stackTrace, LogType type)
	{
        string logLine = System.String.Format("[{0}]:{1}", type.ToString(), condition);
        logLine += "\n";
        LogData.Add (logLine);
		if (LogData.Count > 10) {
			LogData.RemoveAt (0);
		}
		LogDataDital = "";
		foreach (var item in LogData) {
			LogDataDital += item;
		}
	}
	private string LogDataDital="";
	private List<string> LogData = new List<string>();
	// Update is called once per frame
	void OnGUI () {
		if (LogDataDital != "") {
			Rect rect = new Rect (0, 0, Screen.width, 200);
			GUI.TextArea (rect,LogDataDital, 1000);
		}
	}

    public static void SetTexColor(Color color,ref string tex) {
        string hvsCol = ColorToHex(color);
        string buf = System.String.Format("<color=#{0}>{1}</color>", hvsCol,tex);
        tex = buf;
    }

    #region ColorDecode Tool
    public static string ColorToHex(Color color) {
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        string hex = string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);
        return hex;
    }

    /// <summary>
    /// hex转换到color
    /// </summary>
    /// <param name="hex"></param>
    /// <returns></returns>
    public Color HexToColor(string hex) {
        byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        float r = br / 255f;
        float g = bg / 255f;
        float b = bb / 255f;
        float a = cc / 255f;
        return new Color(r, g, b, a);
    }
    #endregion
}
