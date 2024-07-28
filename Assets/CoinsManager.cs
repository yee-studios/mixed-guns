using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinsManager : Singleton<CoinsManager>
{
    [SerializeField] Image coinImage;
    [SerializeField] TextMeshProUGUI coinsText;

    [SerializeField] int coins;
    public int Coins {
        get { return coins; }
        set {
            coins = value;
            coinsText.text = coins.ToString();
            Image newCoin = Instantiate(coinImage, coinImage.transform.parent);
            newCoin.transform.DOLocalMoveY(1f, 1f);
            newCoin.DOFade(0f, 1f).OnKill(() => Destroy(newCoin.gameObject));
        }
    }
}
