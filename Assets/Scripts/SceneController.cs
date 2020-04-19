using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{

  public RobotFriend robotFriend;

  public PlayerController player;

  public List<Transform> enemySpawnLocations;
  public List<Transform> itemSpawnLocations;

  public List<Item> itemPrefabs;

  public List<Enemy> enemies;

  public int waveNum;
  public float waveCooldownTime;
  public float enemySpawnChance;
  public float spawnCoolDown;

  public float currentWaveCooldownTime;
  public float lastSpawn;
  public bool waveRunning;
  public int enemyWaveNum;
  public int killedWaveEnemy;
  public int spawnedWaveEnemy;

  public float itemSpawnCoolDown;
  public float lastItemSpawnTime;
  public float itemSpawnChance;

  public Text waveNumText;

  private Item[] itemChanceArray;
  private bool failed;
  private bool win;

  public AudioSource sharedAudioSource;

  public AudioClip enemyDeadSound1;
  public AudioClip enemyDeadSound2;

  // Start is called before the first frame update
  void Start()
  {
    EndWave();


    itemChanceArray = new Item[100];
    var index = 0;

    foreach (var item in itemPrefabs)
    {
      for (var i = index; i < (index + item.spawnChance * 100) && i < 100; i++)
      {
        itemChanceArray[i] = item;
      }

      index += (int)(item.spawnChance * 100);
    }

  }

  // Update is called once per frame
  void Update()
  {
    if (win || failed) {
      return;
    }

    if (!waveRunning)
    {
      currentWaveCooldownTime -= Time.deltaTime;
      if (currentWaveCooldownTime <= 0)
      {
        StartWave();
      }
    }
    else
    {
      if (killedWaveEnemy >= enemyWaveNum)
      {
        EndWave();
      }
      else
      {
        if (spawnedWaveEnemy < enemyWaveNum || robotFriend.isDead || player.isDead)
        {
          if (Time.time - lastSpawn > spawnCoolDown && UnityEngine.Random.value < enemySpawnChance + 0.1 * waveNum)
          {
            SpawnEnemy();
          }
        }
      }
    }

    if (Time.time - lastItemSpawnTime > itemSpawnCoolDown)
    {
      lastItemSpawnTime = Time.time;
      SpawnItem();
    }

    if (player.isDead && robotFriend.isDead && ! failed) {
      Fail();
      failed = true;
    }

  }

 

  private void SpawnEnemy()
  {
    var spawnPoint = enemySpawnLocations[UnityEngine.Random.Range(0, enemySpawnLocations.Count)];
    var enemy = enemies[UnityEngine.Random.Range(0, enemies.Count)];
    spawnedWaveEnemy++;
    lastSpawn = Time.time;

    var e = Instantiate(enemy.gameObject, (Vector3)UnityEngine.Random.insideUnitCircle * 2 + spawnPoint.position, Quaternion.identity).GetComponent<Enemy>();
    e.playerTarget = player;
    e.robotTarget = robotFriend;
  }

  private void SpawnItem(Vector3? position = null)
  {
    if (UnityEngine.Random.value < itemSpawnChance)
    {
      var spawnPoint = position == null ? itemSpawnLocations[UnityEngine.Random.Range(0, itemSpawnLocations.Count)].position : position;
      Instantiate(itemChanceArray[UnityEngine.Random.Range(0, itemChanceArray.Length)], (Vector3)UnityEngine.Random.insideUnitCircle * 1 + spawnPoint.Value, Quaternion.identity);
    }
  }

  private void EndWave()
  {
    waveRunning = false;
    killedWaveEnemy = 0;
    spawnedWaveEnemy = 0;
    currentWaveCooldownTime = waveCooldownTime;
    waveNum++;
    enemyWaveNum = waveNum * 2 + 7;
    lastSpawn = float.MinValue;
    
    if (waveNum > 10)
    {
      Win();
      win = true;
    }
    else {
      waveNumText.text = $"WAVE {waveNum}/10";
    }
  }

  private void Win()
  {
    SceneManager.LoadScene(3, LoadSceneMode.Additive);
  }
  private void Fail()
  {
    SceneManager.LoadScene(2, LoadSceneMode.Additive);
  }

  private void StartWave()
  {
    waveRunning = true;
  }

  public void OnEnemyKilled(Vector3 position)
  {
    SpawnItem(position);

    PlaySound(enemyDeadSound1);
    PlaySound(enemyDeadSound2);

    killedWaveEnemy++;
  }

  public void PlaySound(AudioClip sound) {
    sharedAudioSource.PlayOneShot(sound);
  }

}
