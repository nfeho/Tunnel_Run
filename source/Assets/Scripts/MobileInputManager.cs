using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public enum MobileInput { Tap = 1, HoldRight = 2, HoldLeft = 4, SwipeUp = 8, SwipeDown = 16, SwipeLeft = 32, SwipeRight = 64, NoInput = 0 }
public enum MobileInputPhases { Holding, Swiping, InitPhase }
public enum SwipeDirection { None = 0, Up = 1, Down = 2, Left = 4, Right = 8 }
public class MobileInputManager : MonoBehaviour
{

    //supported event names, to start listening to this event, use this variables
    public static string e_OnTap = "onTap";
    public static string e_OnHoldRightEnter = "onHoldRightEnter";
    public static string e_OnHoldRightLeave = "onHoldRightLeave";
    public static string e_OnHoldLeftEnter = "onHoldLeftEnter";
    public static string e_OnHoldLeftLeave = "onHoldLeftLeave";
    public static string e_OnSwipeUp = "onSwipeUp";
    public static string e_OnSwipeDown = "onSwipeDown";
    public static string e_OnSwipeLeft = "onSwipeLeft";
    public static string e_OnSwipeRight = "onSwipeRight";

    [SerializeField]
    private float holdTime = 0.20f;
    [SerializeField]
    private float tapTime = 0.17f;
    [SerializeField]
    private float startSwipeError = 20f;
    [SerializeField]
    private float stopSwipeError = 120f;

    Dictionary<int, MobileInput> currentMobileInputs;
    Dictionary<int, float> acumTimes;
    Dictionary<int, Vector2> initTouchPositions;
    Dictionary<int, MobileInputPhases> currentMobilePhases;

    void Start()
    {
        currentMobileInputs = new Dictionary<int, MobileInput>();
        acumTimes = new Dictionary<int, float>();
        initTouchPositions = new Dictionary<int, Vector2>();
        currentMobilePhases = new Dictionary<int, MobileInputPhases>();

    }

    // Update is called once per frame
    void Update()
    {
        Touch[] touches = Input.touches;

        foreach (var touch in touches)
        {

            if (acumTimes.ContainsKey(touch.fingerId))
            {
                acumTimes[touch.fingerId] += Time.deltaTime;
            }

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    //add variables
                    acumTimes.Add(touch.fingerId, Time.deltaTime);
                    currentMobileInputs.Add(touch.fingerId, MobileInput.NoInput);
                    initTouchPositions.Add(touch.fingerId, touch.position);
                    currentMobilePhases.Add(touch.fingerId, MobileInputPhases.InitPhase);
                    //Debug.Log("MIM mobileInput added: " + currentMobileInputs[touch.fingerId]);

                    break;
                case TouchPhase.Stationary:
                    CheckHoldingStartInput(touch);
                    break;
                case TouchPhase.Moved:
                    if (currentMobilePhases[touch.fingerId] == MobileInputPhases.InitPhase)
                    {
                        var deltaPosition = touch.position - initTouchPositions[touch.fingerId];
                        if(Math.Abs(deltaPosition.x) > startSwipeError || Math.Abs(deltaPosition.y) > startSwipeError)
                        {
                            currentMobilePhases[touch.fingerId] = MobileInputPhases.Swiping;
                        }
                    }
                    break;
                case TouchPhase.Ended:
                  // Debug.Log("acumTime for " + touch.fingerId + ": " + acumTimes[touch.fingerId] + ", deltaPosition: " +(touch.position - initTouchPositions[touch.fingerId]));

                    CheckHoldingStopInput(touch);
                    CheckSwipeInput(touch);
                    CheckTapInput(touch);

                    //remove variables
                   // Debug.Log("MIM mobileInput " + touch.fingerId + " removed: " + currentMobileInputs[touch.fingerId] + ", phase: " + currentMobilePhases[touch.fingerId]);

                    acumTimes.Remove(touch.fingerId);
                    currentMobileInputs.Remove(touch.fingerId);
                    initTouchPositions.Remove(touch.fingerId);
                    currentMobilePhases.Remove(touch.fingerId);
                    break;
            }
        }
    }

    /// <summary>
    /// check tap and trigger events with position
    /// </summary>
    /// <param name="touch">current touch input</param>
    void CheckTapInput(Touch touch)
    {
        if (currentMobilePhases[touch.fingerId] == MobileInputPhases.InitPhase)
        {
            if (acumTimes[touch.fingerId] < tapTime)
            {
                currentMobileInputs[touch.fingerId] = MobileInput.Tap;
                //Debug.Log("MIM mobileInput " + touch.fingerId + " changed to: " + currentMobileInputs[touch.fingerId]);
                EventManager.TriggerEvent(e_OnTap, touch.position as object);
            }
        }
    }

    /// <summary>
    /// check holding start and trigger supported events
    /// </summary>
    /// <param name="touch">current touch input></param>
    void CheckHoldingStartInput(Touch touch)
    {
        if (acumTimes[touch.fingerId] > holdTime)
        {
            if (currentMobilePhases[touch.fingerId] == MobileInputPhases.InitPhase)
            {
                currentMobilePhases[touch.fingerId] = MobileInputPhases.Holding;
                if (touch.position.x < Screen.width / 2)
                {
                    currentMobileInputs[touch.fingerId] = MobileInput.HoldLeft;
                   // Debug.Log("MIM mobileInput " + touch.fingerId + " changed to: " + currentMobileInputs[touch.fingerId]);
                    EventManager.TriggerEvent(e_OnHoldLeftEnter);

                }
                else if (touch.position.x > Screen.width / 2)
                {
                    currentMobileInputs[touch.fingerId] = MobileInput.HoldRight;
                   // Debug.Log("MIM mobileInput " + touch.fingerId + " changed to: " + currentMobileInputs[touch.fingerId]);
                    EventManager.TriggerEvent(e_OnHoldRightEnter);
                }
            }
        }
    }

    /// <summary>
    /// check holding stop and trigger supported events
    /// </summary>
    /// <param name="touch">current touch input</param>
    void CheckHoldingStopInput(Touch touch)
    {
        if (currentMobilePhases[touch.fingerId] == MobileInputPhases.Holding)
        {
            switch (currentMobileInputs[touch.fingerId])
            {
                case MobileInput.HoldLeft:
                    EventManager.TriggerEvent(e_OnHoldLeftLeave);
                    break;
                case MobileInput.HoldRight:
                    EventManager.TriggerEvent(e_OnHoldRightLeave);
                    break;

            }
        }
    }
    /// <summary>
    /// check swipe direction and trigger event for supported swipe directions
    /// </summary>
    /// <param name="touch">current touch input</param>
    void CheckSwipeInput(Touch touch)
    {
        if (currentMobilePhases[touch.fingerId] == MobileInputPhases.Swiping)
        {

            //check swipe direction
            var deltaPos = touch.position - initTouchPositions[touch.fingerId];
            // Debug.Log("swipe deltaPos:" + deltaPos);
            SwipeDirection swipeDir = SwipeDirection.None;
            if (!(Math.Abs(deltaPos.x) > stopSwipeError && Math.Abs(deltaPos.y) > stopSwipeError))
            {
                if (Math.Abs(deltaPos.x) > stopSwipeError)
                {
                    if (deltaPos.x > 0)
                    {
                        swipeDir |= SwipeDirection.Right;
                    }
                    if (deltaPos.x < 0)
                    {
                        swipeDir |= SwipeDirection.Left;
                    }
                }

                if (Math.Abs(deltaPos.y) > stopSwipeError)
                {
                    if (deltaPos.y < 0)
                    {
                        swipeDir |= SwipeDirection.Down;

                    }
                    if (deltaPos.y > 0)
                    {
                        swipeDir |= SwipeDirection.Up;
                    }
                }
            }


            //trigger swipe event
            if (swipeDir == SwipeDirection.Up)
            {
                EventManager.TriggerEvent(e_OnSwipeUp);
                currentMobileInputs[touch.fingerId] = MobileInput.SwipeUp;
               // Debug.Log("MIM mobileInput " + touch.fingerId + " changed to: " + currentMobileInputs[touch.fingerId]);

            }
            if (swipeDir == SwipeDirection.Down)
            {
                EventManager.TriggerEvent(e_OnSwipeDown);
                currentMobileInputs[touch.fingerId] = MobileInput.SwipeDown;
               // Debug.Log("MIM mobileInput " + touch.fingerId + " changed to: " + currentMobileInputs[touch.fingerId]);

            }
            if (swipeDir == SwipeDirection.Left)
            {
                EventManager.TriggerEvent(e_OnSwipeLeft);
                currentMobileInputs[touch.fingerId] = MobileInput.SwipeLeft;
                //Debug.Log("MIM mobileInput " + touch.fingerId + " changed to: " + currentMobileInputs[touch.fingerId]);

            }
            if (swipeDir == SwipeDirection.Right)
            {
                EventManager.TriggerEvent(e_OnSwipeRight);
                currentMobileInputs[touch.fingerId] = MobileInput.SwipeRight;
              //  Debug.Log("MIM mobileInput " + touch.fingerId + " changed to: " + currentMobileInputs[touch.fingerId]);

            }
        }
    }
}
