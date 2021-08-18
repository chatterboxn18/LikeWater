using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Queendom
{
  public class MagicItem : MonoBehaviour
  {
    [SerializeField] protected TextMeshProUGUI _textBox;
    protected int _value;
    public int Value => _value;

    public virtual void SetText(int value)
    {
      _textBox.text = value.ToString(CultureInfo.InvariantCulture);
      _value = value;
    }
  }


}
