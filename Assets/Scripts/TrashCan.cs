using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrashCan : MonoBehaviour	//This class only deals with moving the trash can and scoring points when we hit it
{
	[SerializeField]
	private int thisGoalGivesScore;

	private Vector3 startPos;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.TryGetComponent(out Emoji throwable)) //If the object has the Enemy component we can safely assume it is the enemy - no need for tags
		{
			throwable.Kill();	//We tell the emoji to die
			GameController.control.AddScore(thisGoalGivesScore);	//We add score to the gamecontroller singleton
		}
	}

	private void Awake()
	{
		startPos = transform.position;
	}

	public void MoveMe(float newZ) //The EnemySpawner script calls this so tell the trash can to move to its "lane"
	{
		transform.DOMoveZ(newZ, 0.2f);
	}

	private void Update()	//We wiggle the trash can based on score to make the game progressively more difficult
	{
		Vector3 nextPos = transform.position;
		nextPos.x = startPos.x + (2f * Mathf.Sin((((float)GameController.control.score)/4f) * Time.time));	//We move the trash can based on the sine function multiplied by current score
		transform.position = nextPos;
	}
}
