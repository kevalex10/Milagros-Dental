using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;

public class Category : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
{
    [HideInInspector]
    public CategoryType CategoryType;

    [Header("UI References")]
    public Image Icon;
    public TextMeshProUGUI Text;

    [Header("Icon Sprites")]
    public List<Sprite> Icons;

    [HideInInspector]
    public ScrollRect ScrollRect;

    #region Interface Methods
    public void OnBeginDrag(PointerEventData eventData)
    {
        ScrollRect.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ScrollRect.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ScrollRect.OnEndDrag(eventData);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        ScrollRect.OnInitializePotentialDrag(eventData);
    }
    #endregion

    public void SetCategory(CategoryType categoryType)
    {
        CategoryType = categoryType;

        switch (categoryType)
        {
            case CategoryType.DentitionOverview:
                Icon.sprite = Icons[0];
                Text.SetText("Dentition Overview");
                break;
            case CategoryType.Ergonomic:
                Icon.sprite = Icons[1];
                Text.SetText("Ergonomic");
                break;
            case CategoryType.InfectionControl:
                Icon.sprite = Icons[2];
                Text.SetText("Infection Control");
                break;
            case CategoryType.Impression:
                Icon.sprite = Icons[3];
                Text.SetText("Impression");
                break;
            case CategoryType.MixingDental:
                Icon.sprite = Icons[4];
                Text.SetText("Mixing Dental Materials");
                break;
            case CategoryType.PreventativeDental:
                Icon.sprite = Icons[5];
                Text.SetText("Preventative Dental Care");
                break;
            case CategoryType.RestorativeDental:
                Icon.sprite = Icons[6];
                Text.SetText("Restorative Dental Care");
                break;
        }
    }

    public void ShowVideos()
    {
        //Handheld.PlayFullScreenMovie("My Movie 9.mov", Color.black, FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.AspectFill);
        AppManager.Instance.ShowVideos(this);
    }

    public void Vibrate()
    {
        AppManager.Instance.VibrateButtonClick();
    }
}

public enum CategoryType
{
    DentitionOverview,
    Ergonomic,
    InfectionControl,
    Impression,
    MixingDental,
    PreventativeDental,
    RestorativeDental
}
