﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public GameObject mainBase;
	public GameObject miningBase;
	public List<GameObject> types;

	private GameObject container;

	public Queue<GameObject> entities = new Queue<GameObject>();

	public int nextType;
	public float timeToSpawn;
	public float interval = 3;
	public int enemyCount;

	public int enemyPerWave = 5;

	public bool dead;

	private static int spawnerCount;

	// Use this for initialization
	void Start ()
	{
		nextType = -1;
		timeToSpawn = interval;
		dead = false;
		enemyCount = 0;
		container = new GameObject("Container " + spawnerCount++);
		container.transform.parent = gameObject.transform;
	}

	private void OnNavMeshReady()
	{
		UpgradeDefences();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!dead)
		{
			timeToSpawn -= Time.deltaTime;
			if (timeToSpawn <= 0)
			{
				timeToSpawn = interval;
				SpawnEnemy(types[nextType]);
			}	
		}
	}

	public void SpawnEnemy(GameObject type)
	{
		if (entities.Count > 0)
		{
			var next = entities.Dequeue();
			if (next != null)
			{
				next.GetComponent<EnemyController>().active = true;
				next.GetComponent<Rigidbody>().isKinematic = false;
			}
		}
		else if (container.transform.childCount == 0)
		{
			dead = true;
			var spawners = gameObject.GetComponentsInChildren<Spawner>();
			foreach (var spawner in spawners)
			{
				spawner.UpgradeDefences();
			}
		}
	}

	private GameObject doSpawnEnemy(GameObject type, GameObject at)
	{
		var spawnPos = at.transform.position;
		spawnPos.y = 0;
		var enemy = Instantiate(type, spawnPos, Quaternion.identity, container.transform);
		enemy.name = "Enemy LVL " + (nextType + 1) + ": " + enemyCount++;
		var enemyController = enemy.GetComponent<EnemyController>();
		enemyController.mineObject = miningBase;
		enemyController.baseObject = mainBase;
		enemyController.active = false;
		enemy.GetComponent<Rigidbody>().isKinematic = true;
		return enemy;
	}

	public void UpgradeDefences()
	{
		if (dead)
		{
			return;
		}
		if (nextType + 1 < types.Count)
		{
			nextType++;
		}
		
		for (var j = 0; j < enemyPerWave; j++)
		{
			var spawned = doSpawnEnemy(types[nextType], miningBase);
			
			entities.Enqueue(spawned);
		}
	}
}
