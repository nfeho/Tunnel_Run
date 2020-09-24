using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public void OnPointerEnter(PointerEventData eventData)
    {

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);


        FPSRunnerMotor.Instance.canShoot = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!PauseMenuManager.Paused)
        {
            FPSRunnerMotor.Instance.SetCustomCursor();
        }

        FPSRunnerMotor.Instance.canShoot = true;
    }
}
