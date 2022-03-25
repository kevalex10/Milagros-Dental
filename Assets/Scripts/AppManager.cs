using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject CategoriesPrfefab;
    public GameObject CategoryPrefab;
    public GameObject VideoPrefab;
    public GameObject RichTextPrefab;
    public GameObject ImagePrefab;

    [Header("Search Field")]
    public TMP_InputField SearchField;

    [Header("UI Panels")]
    public GameObject MainPanel;
    public GameObject VideosListPanel;
    public GameObject VideosPanel;
    public GameObject LoadingPanel;

    [Header("UI Panel Containers")]
    public GameObject CategoriesContainer;
    public GameObject VideosContainer;
    public GameObject LessonContentContainer;

    [Header("Video Panel References")]
    public TextMeshProUGUI VideoListPanelHeader;
    public TextMeshProUGUI VideoPanelContentHeader;
    public Image VideoPanelHeaderImage;
    public GameObject PlayButtonImage;
    public Button PlayButton;

    [Header("Scriptable Objects")]
    public VideosListScriptableObject VideosListScriptableObject;

    private List<GameObject> _AllPanels = new List<GameObject>();

    public const float HIDE_ALPHA_VALUE = 0;
    public const float SHOW_ALPHA_VALUE = 1;
    public const float PANEL_TWEEN_TIME = 0.2f;

    public static AppManager Instance;

    // The active video lesson that's currently open
    private Video ActiveVideo;

    private void Awake()
    {
        Instance = this;

        // Make the app run as fast as possible
        Application.targetFrameRate = 300;
    }

    private void Start()
    {
        InstantiateCategories();

        _AllPanels.Add(MainPanel);
        _AllPanels.Add(VideosListPanel);
        _AllPanels.Add(VideosPanel);
    }

    #region Helper Methods
    private List<Video> GetVideos(CategoryType type)
    {
        return VideosListScriptableObject.Videos.FindAll(x => x.Category == type);
    }

    private List<Video> GetVideos(string searchInput)
    {
        return VideosListScriptableObject.Videos.FindAll(x => x.Title.ToLower().Contains(searchInput.Trim().ToLower()));
    }

    private void ClearTransformChildren(Transform transform)
    {
        // Clear the video list of any previous items
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void CreateVideoItemsList(List<Video> videos)
    {
        ClearTransformChildren(VideosContainer.transform);

        // Create video list items
        foreach (Video video in videos)
        {
            GameObject videoPrefab = Instantiate(VideoPrefab, VideosContainer.transform);

            VideoListItem videoListItem = videoPrefab.GetComponent<VideoListItem>();
            videoListItem.ScrollRect = VideosContainer.transform.parent.parent.GetComponent<ScrollRect>();
            videoListItem.Video = video;
        }
    }

    [Obsolete]
    public void PlayActiveVideo()
    {
        VideoManager.Instance.PlayVideo(ActiveVideo.VideoURL);
    }
    #endregion

    #region UI Methods
    public void ShowPanel(GameObject panelToShow)
    {
        StartCoroutine(IShowPanel(panelToShow));
    }

    public void HidePanel(GameObject panel)
    {
        panel.GetComponent<CanvasGroup>().DOFade(HIDE_ALPHA_VALUE, PANEL_TWEEN_TIME).SetEase(Ease.Linear).OnComplete(() => { panel.SetActive(false); });
    }

    public void ReturnToHome()
    {
        Toolbar.Instance.Show();
        Toolbar.Instance.HideBackButton();
        SearchField.SetTextWithoutNotify("");
        ShowPanel(MainPanel);
    }

    private void InstantiateCategories()
    {
        StartCoroutine(IInstantiateCategories());
    }

    public void SearchVideo()
    {
        StartCoroutine(IShowVideos(SearchField.text));
    }

    public void ShowVideos(Category category)
    {
        StartCoroutine(IShowVideos(category));
    }

    public void ShowVideo(Video video)
    {
        // If lesson has no video, disable the button
        PlayButton.interactable = !string.IsNullOrEmpty(video.VideoURL);
        PlayButtonImage.SetActive(PlayButton.interactable);
        ActiveVideo = video;

        Toolbar.Instance.Hide();
        StartCoroutine(IShowVideo(video));
    }

    public void ShowLoadingPanel()
    {
        LoadingPanel.SetActive(true);
        LoadingPanel.GetComponent<CanvasGroup>().DOFade(SHOW_ALPHA_VALUE, PANEL_TWEEN_TIME).SetEase(Ease.Linear);
    }

    public void HideLoadingPanel()
    {
        LoadingPanel.GetComponent<CanvasGroup>().DOFade(HIDE_ALPHA_VALUE, PANEL_TWEEN_TIME).SetEase(Ease.Linear).OnComplete(() => { LoadingPanel.SetActive(false); });
    }

    public void UpdateLoadingProgress(int progress)
    {
        LoadingPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText($"Loading Video: {progress}%");
    }
    #endregion

    #region Coroutines
    private IEnumerator IShowPanel(GameObject panelToShow)
    {
        // Hide all panels besides the one to show
        foreach (GameObject panel in _AllPanels)
        {
            if (panel != panelToShow)
            {
                HidePanel(panel);
            }
        }

        if(panelToShow != MainPanel)
        {
            Toolbar.Instance.ShowBackButton();
        }

        yield return new WaitForSeconds(PANEL_TWEEN_TIME);

        // Scroll all elements to the top
        MainPanel.GetComponent<ScrollRect>().ScrollToTop();
        VideosListPanel.GetComponent<ScrollRect>().ScrollToTop();
        VideosPanel.transform.GetChild(0).GetComponent<ScrollRect>().ScrollToTop();

        panelToShow.SetActive(true);
        panelToShow.GetComponent<CanvasGroup>().DOFade(SHOW_ALPHA_VALUE, PANEL_TWEEN_TIME).SetEase(Ease.Linear);
    }

    private IEnumerator IInstantiateCategories()
    {
        GameObject categories = null;

        int columnCount = 3;

        foreach (int i in Enum.GetValues(typeof(CategoryType)))
        {
            // Create the row that will hold 3 categories
            if (columnCount == 3)
            {
                columnCount = 0;
                categories = Instantiate(CategoriesPrfefab, CategoriesContainer.transform);
            }

            // Create the category
            Category category = Instantiate(CategoryPrefab, categories.transform).GetComponent<Category>();
            category.GetComponent<Category>().ScrollRect = CategoriesContainer.transform.parent.parent.GetComponent<ScrollRect>();
            category.SetCategory((CategoryType)i);

            columnCount++;

        }

        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(CategoriesContainer.transform.parent.GetComponent<RectTransform>());
    }

    private IEnumerator IShowVideos(Category category)
    {
        Toolbar.Instance.Show();
        VideoListPanelHeader.SetText(category.Text.text);
        CreateVideoItemsList(GetVideos(category.CategoryType));

        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(VideosContainer.transform.parent.GetComponent<RectTransform>());

        // SHow the videos panel
        ShowPanel(VideosListPanel);
    }

    private IEnumerator IShowVideos(string searchInput)
    {
        Toolbar.Instance.Show();
        VideoListPanelHeader.SetText("Videos");
        CreateVideoItemsList(GetVideos(searchInput));

        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(VideosContainer.transform.parent.GetComponent<RectTransform>());

        // Show the videos panel
        ShowPanel(VideosListPanel);
    }

    private IEnumerator IShowVideo(Video video)
    {
        VideoPanelContentHeader.SetText(video.Title);
        VideoPanelHeaderImage.sprite = video.Image;

        ClearTransformChildren(LessonContentContainer.transform);

        foreach (VideoContent content in video.Content)
        {
            // If there is no text it's an image
            if (string.IsNullOrEmpty(content.RichText))
            {
                GameObject Imageprefab = Instantiate(ImagePrefab, LessonContentContainer.transform);
                Image image = Imageprefab.GetComponent<Image>();
                image.sprite = content.Image;
                //image.SetNativeSize();
            }
            else
            {
                // Create a rich text and set its content
                GameObject richText = Instantiate(RichTextPrefab, LessonContentContainer.transform);
                richText.GetComponent<TextMeshProUGUI>().SetText(content.RichText);
            }
        }

        ShowPanel(VideosPanel);

        yield return new WaitForSeconds(PANEL_TWEEN_TIME);
        LayoutRebuilder.ForceRebuildLayoutImmediate(LessonContentContainer.transform.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(LessonContentContainer.transform.parent.GetComponent<RectTransform>());
    }
    #endregion

    #region Haptics
    public void VibrateButtonClick()
    {
        // TODO: Implement haptics
        //////Debug.Log("Vibrate!");
        //////HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        //////Handheld.Vibrate();

        ////if (!AGVibrator.HasVibrator())
        ////{
        ////    Debug.LogWarning("This device does not have vibrator");
        ////}

        ////if (!AGVibrator.AreVibrationEffectsSupported)
        ////{
        ////    Debug.LogWarning("This device does not support vibration effects API!");
        ////    return;
        ////}

        ////AGVibrator.Cancel();
        //////Create a one shot vibration for 200 ms at default amplitude
        ////AGVibrator.Vibrate(VibrationEffect.CreateOneShot(100, -2));
    }
    #endregion
}
