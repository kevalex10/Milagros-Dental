using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{
    [Header("UI References")]
    public Image BackButton;
    public Image BackButtonIcon;
    public GameObject SearchBar;

    private CanvasGroup _CanvasGroup;

    public static Toolbar Instance;

    private void Awake()
    {
        Instance = this;
        _CanvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        SearchBar.SetActive(true);
        _CanvasGroup.DOFade(1f, AppManager.PANEL_TWEEN_TIME).SetEase(Ease.Linear);
        BackButton.DOFade(1, AppManager.PANEL_TWEEN_TIME).SetEase(Ease.Linear);
    }

    public void Hide()
    {
        _CanvasGroup.DOFade(0f, AppManager.PANEL_TWEEN_TIME).SetEase(Ease.Linear).OnComplete(()=> { SearchBar.SetActive(false); });
        BackButton.DOFade(0.87f, AppManager.PANEL_TWEEN_TIME).SetEase(Ease.Linear);
    }

    public void ShowBackButton()
    {
        BackButtonIcon.DOFade(AppManager.SHOW_ALPHA_VALUE, AppManager.PANEL_TWEEN_TIME);
    }

    public void HideBackButton()
    {
        BackButtonIcon.DOFade(AppManager.HIDE_ALPHA_VALUE, AppManager.PANEL_TWEEN_TIME);
    }
}
