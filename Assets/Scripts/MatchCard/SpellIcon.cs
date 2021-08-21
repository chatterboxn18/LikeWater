using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Queendom {
  public class SpellIcon : MonoBehaviour
  {
    [SerializeField] private Image _image;
    [SerializeField] private CanvasGroup _selectedGroup;
    private int _index;
    public int Index => _index;

    public void Selected(bool on) {
      LeanTween.alphaCanvas(_selectedGroup, on ? 1 : 0, QueendomConfig.TransitionTime);
    }

    public void SetColor(Color color) {
      _image.color = color;
    }

    public void SetIndex(int index)
    {
      _index = index;
    }

    public void SetImage(Sprite sprite) {
      _image.sprite = sprite;
    }
  }
}