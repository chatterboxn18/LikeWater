using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager : MonoBehaviour
{
	public static bool Save(string saveName, object saveData)
	{
		BinaryFormatter formatter = GetBinaryFormatter();
		if (!Directory.Exists(Application.persistentDataPath + "/saves"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");
		 
		}

		var path = Application.persistentDataPath + "/saves/" + saveName + ".rv";
		FileStream file = File.Create(path);
		formatter.Serialize(file, saveData);
		file.Close();
		
		return true;
	}

	public static object Load(string path)
	{
		if (!File.Exists(path))
		{
			return null; 
		}

		BinaryFormatter formatter = GetBinaryFormatter();
		FileStream file = File.Open(path, FileMode.Open);

		try
		{
			object save = formatter.Deserialize(file);
			file.Close();
			return save;
		}
		catch
		{
			Debug.LogError("Sad the path is wrong: " + path);
			file.Close();
			return null;
		}
	}
	
	private static BinaryFormatter GetBinaryFormatter()
	{
		BinaryFormatter formatter = new BinaryFormatter();
		return formatter;
	}
}
