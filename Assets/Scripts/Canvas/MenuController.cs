using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Image progressBarImage;
    [SerializeField] Slider soundSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] AudioMixer mixer;

    private void Awake()
    {
        playButton.onClick.AddListener(PlayPressed);
        soundSlider.onValueChanged.AddListener(SoundVolume);
        musicSlider.onValueChanged.AddListener(MusicVolume);
        if(mixer.GetFloat("soundsVol", out float soundVol)) soundSlider.value = Mathf.Pow(10, (soundVol / 20));
        if(mixer.GetFloat("musicVol", out float musicVol)) musicSlider.value = Mathf.Pow(10, (musicVol / 20));
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(PlayPressed);
        soundSlider.onValueChanged.RemoveListener(SoundVolume);
        musicSlider.onValueChanged.RemoveListener(MusicVolume);

    }

    void SoundVolume(float value) => mixer.SetFloat("soundsVol", Mathf.Log10(value) * 20);
    void MusicVolume(float value) => mixer.SetFloat("musicVol", Mathf.Log10(value) * 20);

    void PlayPressed()
    {
        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync("PlayerTest");
        while (!loading.isDone)
        {
            progressBarImage.transform.localScale = new Vector3(loading.progress,1,1);
            yield return null;
        }
    }
}
