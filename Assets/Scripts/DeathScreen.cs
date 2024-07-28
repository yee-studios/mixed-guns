using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : Singleton<DeathScreen>
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] Button button;

    // TODO maybe move these sounds and put them all together with other sounds?
    [SerializeField] AudioClip clickSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip swooshSound;
    [SerializeField] AudioClip sweeshSound;
    [SerializeField] AudioClip swuushSound;

    protected override void Awake()
    {
        base.Awake();
        title.enabled = false;
        button.gameObject.SetActive(false);
        button.onClick.AddListener(OnClick);
    }

    public void BreatheIn() => OneShotSoundsCreator.PlaySound(sweeshSound);
    public void BreatheOut() => OneShotSoundsCreator.PlaySound(swuushSound);

    void OnClick()
    {
        button.interactable = false;
        OneShotSoundsCreator.PlaySound(clickSound);
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSecondsRealtime(1f);
        AsyncOperation op = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        while (!op.isDone)
        {
            yield return null;
        }
    }

    public void Initialize()
    {
        OneShotSoundsCreator.PlaySound(deathSound);
        HealthBorders.Instance.Image.DOFade(0f, 1f);

        title.enabled = true;
        button.gameObject.SetActive(true);
        button.interactable = false;
        gameObject.SetActive(true);

        title.transform.DOScale(1f, 2f).ChangeStartValue(Vector3.zero);
        title.DOColor(title.color, 1f).ChangeStartValue(new Color(0,0,0,0)).OnComplete(() => title.DOColor(Color.red, 2f));
        DOTween.To(() => title.characterSpacing, x => title.characterSpacing = x, 20f, 3f).ChangeStartValue(0f);

        button.transform
            .DOLocalMove(Vector3.up * -100f, 3f)
            .ChangeStartValue(Vector3.up * -500f)
            .SetEase(Ease.OutSine)
            .OnComplete(() => { button.interactable = true; OneShotSoundsCreator.PlaySound(swooshSound); });
    }
}
