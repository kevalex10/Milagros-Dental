using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class VideoManager : MonoBehaviour
{
    public static VideoManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Obsolete]
    private IEnumerator DownloadAndPlayVideo(string videoUrl)
    {
        string saveFileName = "MyStreamedVideo.mp4";


        bool overwriteVideo = true;

        // Where to Save the Video.
        string saveDir = Path.Combine(Application.persistentDataPath, saveFileName);

        // Play back Directory.
        string playbackDir = saveDir;

#if UNITY_IPHONE
        playbackDir = "file://" + saveDir;
#endif

        bool downloadSuccess = false;

        byte[] vidData = null;
        string[] persistantData = Directory.GetFiles(Application.persistentDataPath);

        if (persistantData.Contains(playbackDir) && !overwriteVideo)
        {
            // Video already exists, play it.
            PlayVideo(playbackDir);
            yield break;
        }
        else if (persistantData.Contains(playbackDir) && overwriteVideo)
        {
            // Video already exists, but we are re-downloading it.
            yield return DownloadData(videoUrl, (status, dowloadData) =>
            {
                downloadSuccess = status;
                vidData = dowloadData;
            });
        }
        else
        {
            // Video does not exist. Download it.
            yield return DownloadData(videoUrl, (status, dowloadData) =>
            {
                downloadSuccess = status;
                vidData = dowloadData;
            });
        }

        Debug.Log("Download Success! " + downloadSuccess);

        // Save then play if there was no download error
        if (downloadSuccess)
        {
            //Save Video
            SaveVideoFile(saveDir, vidData);

            //Play Video
            PlayDownloadedVideo(playbackDir);
        }
    }

    // Downloads the Video
    [Obsolete]
    private IEnumerator DownloadData(string videoUrl, Action<bool, byte[]> result)
    {
        // Download Video
        UnityWebRequest webRequest = UnityWebRequest.Get(videoUrl);
        webRequest.Send();

        // Wait until download is done
        while (!webRequest.isDone)
        {
            AppManager.Instance.UpdateLoadingProgress((int)(webRequest.downloadProgress * 100));
            yield return null;
        }

        if (webRequest.isNetworkError)
        {
            Debug.LogError("Error while downloading Video: " + webRequest.error);
            yield break;
        }

        Debug.Log("Video downloaded!");

        // Retrieve downloaded Data
        result(!webRequest.isNetworkError, webRequest.downloadHandler.data);
    }

    // Saves the video
    private bool SaveVideoFile(string saveDir, byte[] vidData)
    {
        try
        {
            FileStream stream = new FileStream(saveDir, FileMode.Create);
            stream.Write(vidData, 0, vidData.Length);
            stream.Close();
            Debug.Log("Video downloaded to: " + saveDir.Replace("/", "\\"));
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error while saving Video File: " + e.Message);
        }
        return false;
    }

    // Plays the video
    private void PlayDownloadedVideo(string path)
    {
        AppManager.Instance.HideLoadingPanel();
        Handheld.PlayFullScreenMovie(path, Color.black, FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.AspectFit);
    }

    [Obsolete]
    public void PlayVideo(string url)
    {
        AppManager.Instance.ShowLoadingPanel();
        StartCoroutine(DownloadAndPlayVideo(url));
    }
}
