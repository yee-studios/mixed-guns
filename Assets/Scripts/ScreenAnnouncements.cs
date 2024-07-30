using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenAnnouncements : Singleton<ScreenAnnouncements>
{
    ScreenAnnouncement prefab;
    private void Start()
    {
        prefab = PrefabHolder.Instance.ScreenAnnouncementPrefab;
    }

    public static void SpawnAnnouncement(string text, float t = 3f)
    {
        ScreenAnnouncement announcement = Instantiate(Instance.prefab, Instance.transform);
        announcement.text.text = text;
        announcement.transform.DOLocalMoveY(-100f, t).ChangeStartValue(new Vector3(0, 100f, 0)).SetUpdate(true);
        DOTween.To(() => announcement.text.characterSpacing, x => announcement.text.characterSpacing = x, 10f, 3f);
        announcement.text.DOFade(announcement.text.color.a, t).SetUpdate(true)
            .ChangeStartValue(new Color(0,0,0,0)).OnComplete(() => announcement.text.DOFade(0f, t).SetUpdate(true));
        announcement.background.DOFade(announcement.background.color.a, t).SetUpdate(true)
            .ChangeStartValue(new Color(0, 0, 0, 0)).OnComplete(() => announcement.background.DOFade(0f, t).SetUpdate(true));
        Destroy(announcement, t * 2);
    }
}
