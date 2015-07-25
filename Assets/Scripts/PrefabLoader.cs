using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JSONEncoderDecoder;

public class PrefabLoader {

	private Dictionary<string, GameObject> prefabList;
	private Dictionary<string, AudioClip> audioPrefabList;
	private Hashtable filePaths;
	private static PrefabLoader instance;
	
	public static void Init()
	{
		if(instance == null) {
			instance = new PrefabLoader();
			instance.filePaths = (Hashtable)JSON.JsonDecode( (Resources.Load("resources") as TextAsset).text );

			instance.prefabList = new Dictionary<string, GameObject>();
			instance.audioPrefabList = new Dictionary<string, AudioClip>();
		}
	}

	private static Object LoadPrefab(string prefabName)
	{
		string filePath = GetFilePath(prefabName);
		if (filePath != null)
		{
			Object obj = Resources.Load(filePath);
			if (obj != null)
			{
				if (obj is AudioClip)
				{
					instance.audioPrefabList.Add(prefabName, (AudioClip)obj);
					return obj;
				}
				else if(obj is GameObject)
				{
					instance.prefabList.Add(prefabName, (GameObject)obj);
					return obj;
				}
			}
		}
		return null;
	}

	public static void LoadAllPrefabs() {
		foreach( DictionaryEntry entry in instance.filePaths ) {
			LoadPrefab((string)entry.Key);
		}
	}

	private static GameObject GetLoadedPrefab(string prefabName)
	{
		return instance.prefabList[prefabName];
	}
	private static AudioClip GetLoadedAudioPrefab(string prefabName)
	{
		return instance.audioPrefabList[prefabName];
	}

	private static bool isLoaded(string prefabName)
	{
		return instance.prefabList.ContainsKey(prefabName) || instance.audioPrefabList.ContainsKey(prefabName);
	}
	
	private static string GetFilePath(string prefabName)
	{
		if (instance.filePaths.ContainsKey(prefabName))
	    {
			return (string)instance.filePaths[prefabName];
		}
		return null;
	}

	public static GameObject Instantiate(string prefabName, Vector3 pos, Quaternion rot)
	{
		GameObject obj = LookupPrefab(prefabName);

		return GameObject.Instantiate(obj, pos, rot) as GameObject;
	}

	private static object _LookupPrefab(string prefabName, bool audio = false)
	{
		object obj;
		if (instance == null)
		{
			PrefabLoader.Init();
		}
		
		if (!isLoaded(prefabName))
		{
			obj = LoadPrefab(prefabName);
		}
		else
		{
			if (audio)
			{
				obj = GetLoadedAudioPrefab(prefabName);
			}
			else
			{
				obj = GetLoadedPrefab(prefabName);
			}
		}

		#if UNITY_EDITOR
		if(obj == null) {
			Debug.LogError( "Prefab lookup failed: " + prefabName );
			Debug.Break();
		}
		#endif

		return obj;
	}

	public static GameObject LookupPrefab(string prefabName)
	{
		return (GameObject)_LookupPrefab(prefabName);
	}
	public static AudioClip LookupAudioPrefab(string prefabName)
	{
		return (AudioClip)_LookupPrefab(prefabName, true);
	}

	public static AudioClip InstantiateAudio(string prefabName, Vector3 pos)
	{
		AudioClip obj = LookupAudioPrefab(prefabName);
		
		return AudioClip.Instantiate(obj, pos, Quaternion.identity) as AudioClip;
	}
}
