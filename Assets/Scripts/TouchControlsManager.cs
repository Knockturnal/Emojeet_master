using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControlsManager : MonoBehaviour	//This class picks up touches and other scripts can subscribe to its events to listen for touches
{
	public static TouchControlsManager manager;	//This static reference

	private Vector3 lastMousePos;	//The mouse position last frame

	public delegate void TouchDelegate(Touch eventData);
	public static event TouchDelegate OnTouchDown;
	public static event TouchDelegate OnTouchUp;
	public static event TouchDelegate OnTouchDrag;

	private void Awake()
	{
		DoSingletonCheck();
	}

	bool DoSingletonCheck() //This logic checks if this is the only copy of this class in existance. It also returns a bool so we can do some logic only after this returns true, though we're not currently implementing that
	{
		if (manager != this)    //Is this object *already* the static reference?
		{
			if (manager)    //If not, then is there *another* static reference?
			{
				Destroy(gameObject);    //If yes, we don't want this one
				return false;
			}
			else
			{
				manager = this; //If else, make this the new static reference

				//We could make this object persist between scenes here if we needed it to:
				//DontDestroyOnLoad(this);

				return true;
			}
		}
		else
		{
			return true;
		}
	}

	void Update()
    {
		Touch currentTouch = new Touch();

		#if UNITY_EDITOR	//If we are in the Unity Editor we simulate a touch

		if (Input.GetMouseButtonUp(0))	//If we detect the LMB released we simulate a touch ended
		{
			currentTouch.phase = TouchPhase.Ended;
			currentTouch.position = Input.mousePosition;
			currentTouch.deltaPosition = Input.mousePosition - lastMousePos;

		} 
		else if (Input.GetMouseButtonDown(0))	//If we detect a LMB pressed down we simulate a touch started
		{
			currentTouch.phase = TouchPhase.Began;
			currentTouch.position = Input.mousePosition;
			currentTouch.deltaPosition = Input.mousePosition - lastMousePos;
		}
		else if (Input.GetMouseButton(0) && Input.mousePosition != lastMousePos)	//If we detect a LMB *held* and the mouse is moving compared to last frame we simulate a touch moved
		{
			currentTouch.phase = TouchPhase.Moved;
			currentTouch.position = Input.mousePosition;
			currentTouch.deltaPosition = Input.mousePosition - lastMousePos;
		}
		else	//Finally if we don't detect any of those we simulate a canceled touch. In theory we could simulate more cases, but this game doesn't need them
		{
			currentTouch.phase = TouchPhase.Canceled;
		}

		lastMousePos = Input.mousePosition; //The mouse position this frame - next frame we can check against it to see how the cursor moved over the last frame.

		#else    //If we are not in the Unity editor (the assumption is that we only build for mobile), we can check for touches

		if(Input.touchCount == 1) //There is exactly one touch
		{
			currentTouch = Input.GetTouch(0); //We get the info from that one touch and store it in a variable
		}

		#endif

		switch (currentTouch.phase)	//We invoke the different event based on what touch phase we are in (or we are simulating)
		{
			case TouchPhase.Began:
				OnTouchDown?.Invoke(currentTouch);
				break;
			case TouchPhase.Moved:
				OnTouchDrag?.Invoke(currentTouch);
				break;
			case TouchPhase.Stationary:
				break;
			case TouchPhase.Ended:
				OnTouchUp?.Invoke(currentTouch);
				break;
			case TouchPhase.Canceled:
				break;
		}
	}
}
