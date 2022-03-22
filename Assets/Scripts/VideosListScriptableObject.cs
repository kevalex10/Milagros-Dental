using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VideosListData", menuName = "Scriptable Objects/VideosListScriptableObject", order = 1)]
public class VideosListScriptableObject : ScriptableObject
{
    public List<Video> Videos;
}