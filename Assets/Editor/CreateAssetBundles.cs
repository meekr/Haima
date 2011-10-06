using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;
 
public class CreateAssetbundles {
 
	[MenuItem("Custom/Create Assetbundles")]
	public static void Execute() {
		Dictionary<string, List<Texture>> bundles = new Dictionary<string, List<Texture>>();
		
		foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets)) {
			if (!(o is Texture)) continue;
			if (o.name.Contains("@")) continue;
			string assetPath = AssetDatabase.GetAssetPath(o);
			string[] parts = assetPath.Split(Path.DirectorySeparatorChar);
            if (!assetPath.Contains("/Bundles/") || parts.Length <= 4)
				continue;
			
			string path = parts[2] + "-" + parts[3] + ".assetbundle";
			if (!bundles.ContainsKey(path))
				bundles.Add(path, new List<Texture>());
			bundles[path].Add((Texture)o);
		}
		
		
		// Create a directory to store the generated assetbundles.
        if (!Directory.Exists(AssetbundlePath))
            Directory.CreateDirectory(AssetbundlePath);
		
		foreach (string path in bundles.Keys){
			// Delete existing assetbundles for current object
	        string[] existingAssetbundles = Directory.GetFiles(AssetbundlePath);
	        foreach (string bundle in existingAssetbundles) {
	            if (path == bundle)
	                File.Delete(bundle);
	        }
	
			if (bundles[path].Count > 0) {
				//Debug.Log(AssetbundlePath + path);
				BuildPipeline.BuildAssetBundle(null, bundles[path].ToArray(), AssetbundlePath + path, BuildAssetBundleOptions.CollectDependencies, BuildTarget.iPhone);	
			}
		}
	}
 
	public static string AssetbundlePath
	{
		get { return "assetbundles" + Path.DirectorySeparatorChar; }
	}
 
	// This method loads all files at a certain path and
	// returns a list of specific assets.
	public static List<T> CollectAll<T>(string path) where T : Object
	{
		List<T> l = new List<T>();
		string[] files = Directory.GetFiles(path);
 
		foreach (string file in files)
		{
			if (file.Contains(".meta")) continue;
			T asset = (T) AssetDatabase.LoadAssetAtPath(file, typeof(T));
			if (asset == null) throw new Exception("Asset is not " + typeof(T) + ": " + file);
			l.Add(asset);
		}
		return l;
	}
	
	// Returns the path to the directory that holds the specified FBX.
	static string CharacterRoot(GameObject character)
	{
		string root = AssetDatabase.GetAssetPath(character);
		return root.Substring(0, root.LastIndexOf('/') + 1);
	}
 
}
