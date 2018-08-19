using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace MyTools.EditorTool{
	static class GUILayoutTool{
		public static Rect Split(this Rect rect,int rowCount,int colCount,int xID,int yID,float space){
			rowCount = Mathf.Max (1, rowCount);
			colCount = Mathf.Max (1, colCount);
			xID = Mathf.Clamp (xID, 0, rowCount - 1);
			yID = Mathf.Clamp (yID, 0, colCount - 1);
			space = Mathf.Max (0, space);

			Rect rectTemp = new Rect ();
			rectTemp.size = new Vector2 ((rect.size.x - space * xID) / (float)rowCount, (rect.size.y - space * yID) / (float)colCount);
			rectTemp.position = rect.position + new Vector2 (rectTemp.size.x * xID + space * xID, rectTemp.size.y * yID + space * yID);
			return rectTemp;
		}
		/// <summary>
		/// Split the specified rect, splitX, splitY, xID and yID.
		/// </summary>
		/// <param name="rect">Rect.</param>
		/// <param name="splitX">Split x (0-1).</param>
		/// <param name="splitY">Split y (0-1).</param>
		/// <param name="xID">X I.</param>
		/// <param name="yID">Y I.</param>
		public static Rect Split(this Rect rect,float splitX,float splitY,int xID,int yID){
			splitX = Mathf.Clamp01 (splitX);
			splitY = Mathf.Clamp01 (splitY);

			Rect rectTemp = new Rect();
			float sizeX = xID == 0 ? rect.size.x * splitX : rect.size.x * (1 - splitX);
			float sizeY = yID == 0 ? rect.size.y * splitY : rect.size.y * (1 - splitY);
			float posX = Mathf.Lerp (rect.xMin, rect.xMax, xID * splitX);
			float posY = Mathf.Lerp (rect.yMin, rect.yMax, yID * splitY);
			rectTemp.size = new Vector2 (sizeX,sizeY);
			rectTemp.position = new Vector2 (posX, posY);
			return rectTemp;
		}
		public static List<Rect> SplitGroup(this Rect rect,int rowCount,int colCount,float space){
			rowCount = Mathf.Max (1, rowCount);
			colCount = Mathf.Max (1, colCount);
			space = Mathf.Max (0, space);
			List<Rect> rectTemp = new List<Rect> ();
			for (int xID = 0; xID < rowCount; xID++) {
				for (int yID = 0; yID < colCount; yID++) {
					Rect rct = new Rect ();
					rct.size = new Vector2 ((rct.size.x - space * xID) / (float)rowCount, (rct.size.y - space * yID) / (float)colCount);
					rct.position = rct.position + new Vector2 (rct.size.x * xID + space * xID, rct.size.y * yID + space * yID);
					rectTemp.Add (rct);
				}
			}

			return rectTemp;
		}
	}
	public class TextureToolWindow : EditorWindow {
		private class Opreator
		{
			public string OpreatorName;
			public string Ditail;
			public float progress;
		}
		private const string splitFilter = " ";
        private static TextureToolWindow Instance;
        [MenuItem("Tools/TextureToolWindow")]
		private static void StartWindow(){
            if (Instance != null) {
                Instance.Focus();
                Instance.ShowNotification(new GUIContent("Hi There =)"));
            } else {
                Instance = EditorWindow.CreateInstance<TextureToolWindow>();
            }
            Instance.Initialize();

        }
        private Opreator FormatOpreator;
		void Initialize(){
			Icon = Resources.Load<Texture> ("TexEditor_Huaji");
			this.titleContent = new GUIContent ("贴图工具v0.1b");
			this.autoRepaintOnSceneChange = true;
			this.maxSize = new Vector2 (800, 500);
			this.minSize = new Vector2 (800, 500);
			leftSideContent = new GUIContent ("");
			rightSideContent = new GUIContent ("");
			this.Show (true);
			FormatOpreator = null;
			EditorApplication.update += EditorUpdate;
		}
		private GUIContent leftSideContent;
		private GUIContent rightSideContent;
		private Rect rect{
			get{ 
				return new Rect (0, 0,800, 500);
			}
		}
		void EditorUpdate(){
		}
		void OnGUI(){
			Rect title = rect.Split (1, 0.1f, 0, 0);
			Rect content = rect.Split (1, 0.1f, 0, 1);
			BeginWindows ();
			GUI.Window (-1, title,OnTitle,"");
			GUI.Window (0, content.Split(0.3f,1f,1,0),OnContent,"Settings");
			GUI.Window (1, content.Split(0.3f,1f,0,0),OnPreview,"Resoult");
			EndWindows ();
//			OnTextureFomat (content);
			if (EditorWindow.focusedWindow == this) {
				this.Repaint ();
			}
		}
		void OnTitle(int windowID){
			if (FormatOpreator != null) {
				EditorUtility.DisplayProgressBar (FormatOpreator.OpreatorName, FormatOpreator.Ditail, FormatOpreator.progress);
			}else{
                EditorUtility.ClearProgressBar();
            }
		}
		private enum FilterType{
			Texture//,Materail,Asset
		}
		private FilterType fType = FilterType.Texture;
		private string externalFilerInvers = "";
		private string externalFiler = "";
		private List<Texture> loadedtex;

		private int countPerPage = 10;
		private int currentPageID =0;
		private int texSizeLevel=6;
		private TextureImporterAlphaSource texAlphaImportMode;
		private int GetOverrideTexSize{
			get{ 
				if (texSizeLevel == 0) {
					return 16;
				}else if (texSizeLevel == 1) {
					return 32;
				}else if (texSizeLevel == 2) {
					return 64;
				}else if (texSizeLevel == 3) {
					return 128;
				}else if (texSizeLevel == 4) {
					return 256;
				}else if (texSizeLevel == 5) {
					return 512;
				}else if (texSizeLevel == 6) {
					return 1024;
				}else if (texSizeLevel == 7) {
					return 2048;
				}else if (texSizeLevel == 8) {
					return 4096;
				}
				return 1024;
			}
		}
		private bool DrawContent{
			get{ 
				return loadedtex != null ? loadedtex.Count > 0 ? true : false : false;
			}
		}
		private void UpdatePreview(){
		}
		void OnPreview(int windowID){
			#region Left Preview
			if(!DrawContent){
				return;
			}

			for(int i =0;i<10;i++){
				int id = currentPageID*10+i;
				if(id<loadedtex.Count){
					if(loadedtex[id] == null){
						GUILayout.Box("Null");
					}else{
						GUILayout.BeginHorizontal();
						EditorGUILayout.ObjectField(loadedtex[id],typeof(Texture));
						EditorGUILayout.SelectableLabel(loadedtex[id].name);
						GUILayout.EndHorizontal();
					}
				}
			}	
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("<<Pre")){
				currentPageID--;
				currentPageID = (int)Mathf.Clamp(currentPageID,0,Mathf.Max(1,((float)loadedtex.Count/(float)countPerPage)));
			}
			GUILayout.Label((currentPageID+1).ToString()+"/"+Mathf.FloorToInt((float)loadedtex.Count/(float)countPerPage+0.5f));
			if(GUILayout.Button("Nex>>")){
				currentPageID++;
				currentPageID = (int)Mathf.Clamp(currentPageID,0,Mathf.Max(1,((float)loadedtex.Count/(float)countPerPage)));
			}
			GUILayout.EndHorizontal();


			#endregion
		}
		#region Formate propertrys
		private TextureImporterFormat texFormat = TextureImporterFormat.DXT5Crunched;
		TextureImporterType texImpType = TextureImporterType.Default;
		bool texAlphaIsTransparent;
		int texCompressionQuality = 50;
		bool texCrunchedCompression;
		bool texOverriden = true;
		private string[] GetAllTextureImportType{
			get{ 
				return new string[]{ 
					"Default",
					"NormalMap",
					"Editorgui & Legacy GUI",
					"Sprite(2D and UI)",
					"Cursor",
					"Cookie",
					"Lightmap",
					"Single Channel"
				};
			}
		}
		#endregion
		void OnContent(int windowID){
			#region Filter Settings
			string count = loadedtex == null ? "" : loadedtex.Count>0 ? loadedtex.Count.ToString() : "";
			EditorGUILayout.HelpBox("筛选素材：" + count,MessageType.None);
			GUILayout.BeginHorizontal();
			GUILayout.Box("Resources Type");
			fType = (FilterType)EditorGUILayout.EnumPopup (fType);
			externalFiler = GUILayout.TextField (externalFiler);
			GUILayout.EndHorizontal();
			EditorGUILayout.HelpBox("反选工具，输入关键词进行反向筛选，多个关键词用空格隔开",MessageType.Info);
			GUILayout.BeginHorizontal();
			GUILayout.Box("InversSelect");
			externalFilerInvers = GUILayout.TextField (externalFilerInvers);
			if (GUILayout.Button ("Search")) {
				currentPageID = 0;
				loadedtex = new List<Texture> ();
				loadedtex = FindAssetsAutoFilter<Texture> (externalFiler);
				if(!System.String.IsNullOrEmpty(externalFilerInvers)){
					string inversfilterRule = externalFilerInvers.Replace(' ',';');
					Debug.Log(inversfilterRule);
					string [] inversRule = inversfilterRule.Split(';');
					if(inversRule.Length==0){
						inversRule = new string[]{"externalFilerInvers"};
					}
					List<Texture> selected = new List<Texture>();
					for(int i=0;i<loadedtex.Count;i++){
						bool include = true;
						foreach(var rule in inversRule){
							if(loadedtex[i].name.Contains(rule)){
								include = false;
								break;
							}
						}
						if(include){
							selected.Add(loadedtex[i]);
						}
					}
					loadedtex = selected;
					selected = null;
				}
			}
			GUILayout.EndHorizontal();
			#endregion
			GUILayout.Space (20);
			EditorGUILayout.HelpBox ("Format Texture",MessageType.None);
			#region Format Settings
			if (DrawContent) {
				TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings ();
				TextureImporterSettings textureSettings = new TextureImporterSettings ();
				TextureImporterCompression texImpCompression = TextureImporterCompression.Compressed;

				GUILayout.BeginHorizontal();
				GUILayout.Label("Texture Type");
				texImpType = (TextureImporterType)EditorGUILayout.Popup ((int)texImpType,GetAllTextureImportType);
				texFormat = (TextureImporterFormat)EditorGUILayout.EnumPopup (texFormat);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Texture Alpha");
				texAlphaImportMode = (TextureImporterAlphaSource)EditorGUILayout.EnumPopup (texAlphaImportMode);
				texAlphaIsTransparent = EditorGUILayout.Toggle("Alpha Is Transparent",texAlphaIsTransparent);
				GUILayout.EndHorizontal();

				GUILayout.Space (10);

				GUILayout.BeginHorizontal();
				texOverriden = EditorGUILayout.ToggleLeft("Override for " + EditorUserBuildSettings.activeBuildTarget.ToString(),texOverriden);
				texImpCompression = (TextureImporterCompression)EditorGUILayout.EnumPopup (texImpCompression);
				texSizeLevel = EditorGUILayout.Popup(texSizeLevel,new string[]{"16","32","64","128","256","512","1024","2048","4096"});
				GUILayout.EndHorizontal();
				if(texImpCompression != TextureImporterCompression.Uncompressed){
					GUILayout.BeginHorizontal();
					texCrunchedCompression = EditorGUILayout.Toggle("Crunched Compression",texCrunchedCompression);
					texCompressionQuality = Mathf.CeilToInt(EditorGUILayout.Slider("Quality",texCompressionQuality,0,100));
					GUILayout.EndHorizontal();
				}
				GUILayout.BeginHorizontal();
				if (GUILayout.Button ("Format")) {
					FormatOpreator = new Opreator();
					FormatOpreator.OpreatorName = "Format";
					//					string[] path = FindAssetsPath<Texture> (externalFiler);
					for(int i=0;i<loadedtex.Count;i++){
						TextureImporter texImporter = GetTextureSettings (AssetDatabase.GetAssetPath(loadedtex[i]));
						texImporter.ReadTextureSettings(textureSettings);
						textureSettings.ApplyTextureType(texImpType);
						textureSettings.filterMode = FilterMode.Bilinear;
						textureSettings.alphaIsTransparency = texAlphaIsTransparent;
						platformSettings.compressionQuality = texCompressionQuality;
						platformSettings.crunchedCompression = texCrunchedCompression;
						platformSettings.textureCompression = texImpCompression;
						Debug.Log("Overriden ?" +texOverriden);
						platformSettings.overridden = texOverriden;
						platformSettings.textureCompression = texImpCompression;
						platformSettings.format = texFormat;
						texImporter.SetTextureSettings(textureSettings);
						texImporter.SetPlatformTextureSettings (platformSettings);
						texImporter.textureType = texImpType;
						texImporter.maxTextureSize = GetOverrideTexSize;
						texImporter.alphaSource = texAlphaImportMode;
						FormatOpreator.Ditail = texImporter.name;
						FormatOpreator.progress = (float)i/(float)loadedtex.Count;
						texImporter.SaveAndReimport();
					}
					FormatOpreator = null;
				}

				GUILayout.EndHorizontal();
			}
			#endregion

			#region External
			float posX = Event.current.mousePosition.x + Mathf.Sin(Time.realtimeSinceStartup)*20;
			float posY = Mathf.Clamp(Event.current.mousePosition.y,200,800) + Mathf.Cos(Time.realtimeSinceStartup)*20;
			float size = 100 + Mathf.Sin(Time.realtimeSinceStartup)*20;
			try{
				GUI.DrawTexture(new Rect(posX,posY,size,size),Icon);
			}catch{
				GUI.Label(new Rect(posX,posY,size,size),"我插件里面的图不见了！喵喵喵？？？");
				Debug.Log("我插件里面的图不见了！喵喵喵？？？");

			}
			#endregion

		}
		private Texture Icon;
		private List<T> FindAssetsAutoFilter<T>(string filter) where T : Object{
			string internalFilter = "";
			List<T> pool = new List<T> ();
			if (typeof(T) == typeof(Texture)) {
				internalFilter = "t:texture" + splitFilter;
			} else if (typeof(T) == typeof(Material)) {
				internalFilter = "t:material" + splitFilter;
			} else {
				internalFilter = "t:asset" + splitFilter;
			}

			var pathCollect = AssetDatabase.FindAssets (internalFilter+filter);
			foreach(var path in pathCollect){
				pool.Add(AssetDatabase.LoadAssetAtPath<T> (AssetDatabase.GUIDToAssetPath (path)));
			}
			return pool;
		}
		private string[] FindAssetsPath<T>(string filter){
			string internalFilter = "";
			List<T> pool = new List<T> ();
			if (typeof(T) == typeof(Texture)) {
				internalFilter = "t:texture" + splitFilter;
			} else if (typeof(T) == typeof(Material)) {
				internalFilter = "t:material" + splitFilter;
			} else {
				internalFilter = "t:asset" + splitFilter;
			}
			return AssetDatabase.FindAssets (internalFilter+filter);
		}
		static TextureImporter GetTextureSettings(string path)
		{
			TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
			textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
			return textureImporter;
		}
	}
}