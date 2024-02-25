using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class AssetBundleBuilder
{
    [MenuItem("Assets/Build-AssetBundles")]
    static void Build()
    {
        string assetBundleDir = "Assets/AssetBundles";
        if(!Directory.Exists(assetBundleDir))
        {
            Directory.CreateDirectory(assetBundleDir);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);   
    }

}
