using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToastController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _toastText;
    [SerializeField] private CanvasGroup _canvasGroup;

    public CanvasGroup CanvasGroup => _canvasGroup;

    public void SetText(string message)
    {
        _toastText.text = message;
    }
}
