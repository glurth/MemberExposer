using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

public class MemberExposerUIAdpatersUtilties
{
    static List<MemberExposerUIAdpater> allMemberExposerUIAdpaters;
    static MemberExposerUIAdpatersUtilties()
    {
        allMemberExposerUIAdpaters = new List<MemberExposerUIAdpater>();
        List<GameObject> allPreFabs = FindAssetsByType<GameObject>();
        Debug.Log("Init done: found " + allPreFabs.Count + " total prefabs in project");
        foreach (GameObject go in allPreFabs)
        {
            MemberExposerUIAdpater adapter = go.GetComponentInChildren<MemberExposerUIAdpater>();
            if (adapter != null)
                allMemberExposerUIAdpaters.Add(adapter);
        }
        //allMemberExposerUIAdpaters = FindAssetsByType<MemberExposerUIAdpater>();
       
        Debug.Log("Init done: found " + allMemberExposerUIAdpaters.Count + " adapters in project");
    }
    static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        Debug.Log("Finding assets of type :" + typeof(T).Name);
        string[] guids = AssetDatabase.FindAssets("t:"+typeof(T).Name);
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }
        return assets;
    }

    public static List<MemberExposerUIAdpater> GetAllThatHandleMember(SerializableMemberInfo t)
    {
        if (t == null)
        {
            throw new ArgumentException("null passed to AllMemberExposerUIAdpaters.GetAllThatHandleMember");
        }
        List<MemberExposerUIAdpater> result = new List<MemberExposerUIAdpater>();
        foreach (MemberExposerUIAdpater adapter in allMemberExposerUIAdpaters)
            if (adapter.handlesMemberType(t))
                result.Add(adapter);
      //  UnityEngine.Debug.Log("found " + allMemberExposerUIAdpaters.Count + " compatible adapters");
        return result;
    }
}