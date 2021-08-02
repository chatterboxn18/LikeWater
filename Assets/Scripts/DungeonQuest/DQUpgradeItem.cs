using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DQUpgradeItem : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private SimpleButton _button;
    [SerializeField] private TextMeshProUGUI _mainStatText;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Image _overlay;
    private bool _isAvailable;
    public SimpleButton Button => _button;

    public Action Evt_ButtonState = delegate { };

    private int _price;
    private float _priceInflation;

    [SerializeField] private int _level = 1; 
    public int Price => _price;

    public void Evt_PriceUp()
    {
        if (_level < 99)
        {
            _level++;
        }
        else
        {
            return;
        }
        _audioSource.Play();
        _price = Mathf.CeilToInt(_priceInflation * _price);
        _priceText.text = _price.ToString(CultureInfo.InvariantCulture);
        
        _levelText.text = _level.ToString(CultureInfo.InvariantCulture);

    }
    
    public void SetUpgrade(Sprite sprite, string mainText, int originalPrice, float inflation, int level)
    {
        _mainStatText.text = mainText;
        _image.sprite = sprite;
        if (_price <= 0)
            _price = originalPrice;
        _priceText.text = _price.ToString(CultureInfo.InvariantCulture);
        _level = level;
        _levelText.text = _level.ToString(CultureInfo.InvariantCulture);
        _priceInflation = inflation;
    }

    public void Evt_CheckButtonState(int affordable)
    {
        if (affordable < _price && _isAvailable)
        {
            _isAvailable = false;
            _overlay.gameObject.SetActive(true);
            LeanTween.alpha(_overlay.rectTransform, 0.5f, 0.1f);
        }
        else if (affordable >= _price && !_isAvailable)
        {
            _isAvailable = true;
            LeanTween.alpha(_overlay.rectTransform, 0f, 0.1f).setOnComplete(()=>_overlay.gameObject.SetActive(false));
        }
    }

    private void Update()
    {
        Evt_ButtonState();
    }
}
