using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DQUpgradeItem : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private SimpleButton _button;
    public SimpleButton Button => _button;

    public void SetUpgrade(Sprite sprite)
    {
        _image.sprite = sprite;
    }
}
