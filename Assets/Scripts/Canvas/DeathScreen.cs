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

    protected override void Awake()
    {
        base.Awake();
        title.enabled = false;
        button.gameObject.SetActive(false);
        button.onClick.AddListener(OnClick);
    }

    public void BreatheIn() => OneShotSoundsCreator.PlayOneShot(AudioClipsManager.Instance.BreathIn);
    public void BreatheOut() => OneShotSoundsCreator.PlayOneShot(AudioClipsManager.Instance.BreathOut);

    void OnClick()
    {
        button.interactable = false;
        OneShotSoundsCreator.PlayOneShot(AudioClipsManager.Instance.Click);
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
        OneShotSoundsCreator.PlayOneShot(AudioClipsManager.Instance.ScreenDeath);
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
            .OnComplete(() => { button.interactable = true; OneShotSoundsCreator.PlayOneShot(AudioClipsManager.Instance.Swoosh); });
    }
}
