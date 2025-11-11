using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public int coinCount;
    public int deathCount;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI deathText;

    void Update()
    {
        coinText.text = coinCount.ToString();
        deathText.text = deathCount.ToString();
    }
}
