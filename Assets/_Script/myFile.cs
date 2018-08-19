using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

namespace MyTools.IO{
	public class myFile  {
		public bool fileExists(string file){
			return File.Exists (file);
		}

		public bool FloaderCheck(string fileFloader){
			bool exists;
			exists = Directory.Exists (fileFloader);
			// Application.dataPath + "//Save//";//存储配置文件的文件夹
			if (!exists)//如果不存在就创建文件夹
			{
				Directory.CreateDirectory(fileFloader);//创建该文件夹
			}
			//UnityEditor.AssetDatabase.Refresh();
			return exists;
		}
		public string ReadTextFile(string filePath){
			string inf = "";
			try
			{
				FileStream readText = new FileStream(filePath,FileMode.Open);
				int f_length= (int)readText.Length;
				byte[] f_rdByte = new byte[f_length];
//				int f_read = readText.Read(f_rdByte,0,f_length);
				inf = System.Text.Encoding.UTF8.GetString(f_rdByte);
				Debug.LogWarning(inf);
				readText.Close();
			}
			catch (IOException e)
			{
				throw(e);
			}
			return inf;
		}

		/// <summary>
		/// 将指定字符串内容写入到Text文件,若已存在则覆盖
		/// </summary>
		/// <param name="str">要写入的字符串</param>
		/// <param name="fileName">文件的路径</param>
		public void WriteToText(string str, string fileName,FileMode mod)
		{
			try
			{
				FileStream fileStream = new FileStream(fileName, mod, FileAccess.Write);
				byte[] byteSave = Encoding.Default.GetBytes(str);
				fileStream.Write(byteSave, 0, byteSave.Length);
				fileStream.Flush();
				fileStream.Close();
			}
			catch (IOException e)
			{
				CreateFile (fileName);
				FileStream fileStream = new FileStream(fileName, mod, FileAccess.Write);
				byte[] byteSave = Encoding.Default.GetBytes(str);
				fileStream.Write(byteSave, 0, byteSave.Length);
				fileStream.Flush();
				fileStream.Close();
			}
		}

		public void CreateFile(string path){
			FileStream file = new FileStream (path, FileMode.Create);
		}
		public void UpdateAsset(){
			//UnityEditor.AssetDatabase.Refresh();
		}
	}
}