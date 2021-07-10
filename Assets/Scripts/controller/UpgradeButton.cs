using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeButton : Button
{
    public Action Evt_Upgrade = delegate {  };

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        Evt_Upgrade();
    }
}
