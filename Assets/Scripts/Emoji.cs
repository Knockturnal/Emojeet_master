using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emoji : MonoBehaviour
{
	[SerializeField]
	private Rigidbody rb;	//The attached RigidBody
	private Plane gamePlane; //Because we want the object to behave like it is on a 2D plane we defint that plane so we can raycast against it

	private Vector3 grabOffset;	//If we grab the dude off-center we correct for that offset
	private Camera mainCam; //We cache a reference to the main camera for raycasting
	[HideInInspector]
	public EnemySpawner spawnedMe;  //The enemy spawner script that spawned this object. We assign it when we instantiate this in the spawner script
	[SerializeField]
	private float lifeTime; //How long the object lasts after passing through the barrier
	private bool interactable = true;	//We set this to false when we pass through the barrier
	[SerializeField]
	private GameObject deathParticles;	//The particle system to signify our death
	[SerializeField]
	private AudioSource throwSound; //The sound we play when throwing
	[SerializeField]
	private float throwStrengthMultiplier;	//How hard we throw the emoji

	private void Awake()
	{
		mainCam = Camera.main;
		gamePlane = new Plane(Vector3.forward, transform.position);
	}

	public void GrabMe(Vector3 atPosition)
	{
		if (interactable)
		{
			rb.useGravity = false;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			grabOffset = ScreenToWorldPlane(atPosition) - transform.position;
		}
	}

	public void MoveMe(Vector3 nextPosition)
	{
		if (interactable)
		{
			rb.position = ScreenToWorldPlane(nextPosition) - grabOffset;
		}
	}

	public void ThrowMe(Vector3 lastPos, Vector3 currentPos)
	{
		if (interactable)
		{
			rb.useGravity = true;
			
			Vector3 throwForce = ScreenToWorldPlane(currentPos) - ScreenToWorldPlane(lastPos);
			throwForce *= 1f / Time.deltaTime;

			if(throwForce.magnitude > 5f) //We only play the sound if we actually throw with some force
			{
				throwSound.pitch = Random.Range(0.8f, 1.2f);
				throwSound.Play();
			}

			rb.velocity = throwForce * 0.35f * throwStrengthMultiplier;
			rb.angularVelocity = Vector3.one * throwForce.magnitude;
			
		}
	}

	private Vector3 ScreenToWorldPlane(Vector3 screenPos) 
	{
		Ray ray = mainCam.ScreenPointToRay(screenPos);
		gamePlane.Raycast(ray, out float dist);
		return (mainCam.ScreenPointToRay(screenPos).GetPoint(dist));
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Barrier")) 
		{
			rb.useGravity = true;
			interactable = false;
			Invoke("Kill", lifeTime);
		}
	}

	public void Kill() 
	{
		spawnedMe.SpawnEmoji();
		GameObject newParts = Instantiate(deathParticles, transform.position, Quaternion.identity);
		Destroy(newParts, 3f);
		Destroy(gameObject);
	}
}
