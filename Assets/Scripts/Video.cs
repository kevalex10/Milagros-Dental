using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

[Serializable]
public class Video
{
    public CategoryType Category;
    public string Title;
    public Sprite Image;

    public List<VideoContent> Content;

}

[Serializable]
public class VideoContent
{
    [Multiline(40)]
    public string RichText;

    public Sprite Image;
}
