using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnermovescript : MonoBehaviour
{
	[SerializeField] private GameManager gm;
	//public Transform newSpawnPos;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			gm.ResetLevel();
		}
	}
}
