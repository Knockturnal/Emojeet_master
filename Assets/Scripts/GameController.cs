using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameController : MonoBehaviour
{
	public static GameController control;

	public int score;

	[SerializeField]
	private TextMeshProUGUI scoreText;
	[SerializeField]
	private AudioSource scoreSound;

	private void Awake()
	{
		DoSingletonCheck();
	}

	bool DoSingletonCheck() //This logic checks if this is the only copy of this class in existance. It also returns a bool so we can do some logic only after this returns true, though we're not currently implementing that
	{
		if (control != this)    //Is this object *already* the static reference?
		{
			if (control)    //If not, then is there *another* static reference?
			{
				Destroy(gameObject);    //If yes, we don't want this one
				return false;
			}
			else
			{
				control = this; //If else, make this the new static reference

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

	public void AddScore(int toAdd) //This is called from the trash can script
	{
		score += toAdd;	//We add the score we got in
		scoreText.text = score.ToString();	//We update the score text
		scoreText.transform.DOShakeScale(0.2f); //Shake the score text for added juice
		scoreSound.pitch = 0.9f + ((float)toAdd) / 10f;	//Set the pitch higher if we score more
		scoreSound.Play();	//Play the score sound
	}

}
