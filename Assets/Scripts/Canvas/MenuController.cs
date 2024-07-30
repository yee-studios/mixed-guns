using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Image progressBarImage;

    private void Awake()
    {
        playButton.onClick.AddListener(PlayPressed);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(PlayPressed);
    }

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
