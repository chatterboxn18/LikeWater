using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class AssetBundles
{
	[MenuItem("Assets/Build Bundles")]
	static void BuildAssetBundles()
	{
		string bundleDirectory = "Assets/Bundles";
		BuildPipeline.BuildAssetBundles(bundleDirectory, BuildAssetBundleOptions.UncompressedAssetBundle,
			BuildTarget.WebGL);
		var bundles = AssetDatabase.GetAllAssetBundleNames();
		var directory = new DirectoryInfo(bundleDirectory);
		var items = directory.GetFiles();
		foreach (var item in items)
		{
			if (item.Name == directory.Name)
				File.Delete(item.FullName);
			if (item.Name.Contains("manifest") || item.Name.Contains("unity3d"))
				File.Delete(item.FullName);
			
		}

		foreach (var item in items)
		{
			if (((IList) bundles).Contains(item.Name))
			{
				Debug.Log(item.FullName);
				File.Move(item.FullName, item.FullName + ".unity3d");
			}
		}
	}
}
