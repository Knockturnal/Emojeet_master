using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThrower : MonoBehaviour
{
	private Emoji holding;	//The object we are currently holding
	private Camera mainCam;	//Cached reference to the main camera
	private void Awake()
	{
		mainCam = Camera.main;	//We cache a reference to the camera for performance reasons
	}
	private void OnEnable()
	{
		// Subcribe to events when object is enabled
		TouchControlsManager.OnTouchDown += OnTouchDown;
		TouchControlsManager.OnTouchUp += OnTouchUp;
		TouchControlsManager.OnTouchDrag += OnTouchDrag;
	}

	private void OnDisable()
	{
		// Unsubcribe from events when object is disabled
		TouchControlsManager.OnTouchDown -= OnTouchDown;
		TouchControlsManager.OnTouchUp -= OnTouchUp;
		TouchControlsManager.OnTouchDrag -= OnTouchDrag;
	}

	private void OnTouchDown(Touch eventData)	//This is called when we touch (or click) the screen thanks to the subscribed events above
	{
		Ray ray = mainCam.ScreenPointToRay(eventData.position);	//We define a ray into the camera from where we touched

		Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);	//We debug the ray so we see it as a gizmo in the editor

		if(Physics.Raycast(ray, out RaycastHit hit)) //Did the raycast hit something?
		{
			if (hit.transform.TryGetComponent(out Emoji throwable) )	//If the hit object has the Enemy component we can safely store it to be thrown later
			{
				holding = throwable;
				holding.GrabMe(eventData.position);	//We tell the grabbed object that we picked it up
			}
		}
	}

	private void OnTouchUp(Touch eventData)	//Called when we release the mouse or finger
	{
		if(holding != null) //If we are holding an Emoji
		{
			holding.ThrowMe(eventData.position - eventData.deltaPosition, eventData.position); //Send the current mouse position and the last frame position so we can calculate the throwing speed
			holding = null;	//We are no longer holding anything
		}
	}

	private void OnTouchDrag(Touch eventData)	//We update the position of the held object in its own script, but call it from here
	{
		if (holding != null)
		{
			holding.MoveMe(eventData.position);
		}
	}
}
