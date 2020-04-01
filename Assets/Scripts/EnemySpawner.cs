using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject enemyPrefab;	//The emoji prefab to spawn
	[SerializeField]
	private BoxCollider spawnZone;	//The area the emoji can spawn in
	[SerializeField]
	private Texture[] allEmojis;	//All the sprites we can choose from
	[SerializeField]
	private TrashCan[] goals;	//The scripts controlling the trash can. We need to tell it to move to match the "lane" we spawn in
	private void Start()
	{
		SpawnEmoji();
	}

	public void SpawnEmoji() //Spawn a new emoji
	{
		Vector3 spawnPos = transform.position;	//Initialize the spawn pos to this object's position
		Texture spawnVisual = allEmojis[Random.Range(0, allEmojis.Length + 1)];	//Select a random texture from all the available ones
		spawnPos.z = Random.Range(spawnZone.bounds.min.z, spawnZone.bounds.max.z);	//Set the z position we spawn at to a random position within the box collider

		GameObject newEmoji = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);	//Spawn in the new emoji object
		newEmoji.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", spawnVisual);	//We set the texture on the material on the object to the random sprite we chose
		newEmoji.GetComponent<Emoji>().spawnedMe = this;    //We pass a reference to this script to the emoji (So it can call this function when it dies)

		foreach (TrashCan goal in goals)
		{
			goal.MoveMe(newEmoji.transform.position.z); //Finally we tell the trash can to move to match the "lane" of the spawned emoji
		}
	}
}
