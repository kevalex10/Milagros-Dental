using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VideoListItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
{
    [Header("UI References")]
    public TextMeshProUGUI Title;
    public Image Icon;

    [HideInInspector]
    public ScrollRect ScrollRect;

    private Video _Video;
    public Video Video
    {
        get { return _Video; }
        set
        {
            _Video = value;
            Title.SetText(value.Title);
            Icon.sprite = Video.Image;
        }
    }

    private bool _IsDragging;

    #region Interface Methods
    public void OnBeginDrag(PointerEventData eventData)
    {
        _IsDragging = true;
        ScrollRect.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _IsDragging = true;
        ScrollRect.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _IsDragging = false;
        ScrollRect.OnEndDrag(eventData);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        ScrollRect.OnInitializePotentialDrag(eventData);
    }
    #endregion

    public void ShowVideo()
    {
        if (_IsDragging)
            return;

        AppManager.Instance.ShowVideo(Video);
    }

    public void Vibrate()
    {
        AppManager.Instance.VibrateButtonClick();
    }
}
