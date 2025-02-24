using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnermovescript : MonoBehaviour
{
	//public Transform newSpawnPos;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			collision.gameObject.GetComponent<PlayerController>().setRespawn(this.gameObject);
			this.gameObject.SetActive(false);
		}
	}
}
