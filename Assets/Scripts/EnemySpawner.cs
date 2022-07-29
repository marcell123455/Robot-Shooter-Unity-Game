using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class EnemySpawn
{
    public GameObject EnemyObjectToSpawn;
    public Transform Spawnpoint;
    public int AnimationScale;
}


    
public class EnemySpawner : MonoBehaviour
{
    public bool spawnInWaves;
    bool waveIsRunning;
    bool startEventsInvoked;
    bool endEventsInvoked;

    public int waveDelay;

    public GameObject Spawnanimation;
    public float sizeUpSpeed;

    public EnemySpawn[] enemys_Wave_1;
    public EnemySpawn[] enemys_Wave_2;
    public EnemySpawn[] enemys_Wave_3;

    public UnityEvent StartWavesEvents;
    public GameObject[] Barriers;
    public UnityEvent FinishedWavesEvents;
    public Transform playerRespawn;

    int currentWave;
    List<GameObject> spawnedEnemys = new List<GameObject>();

    public void Start()
    {
        foreach (GameObject B in Barriers)
        {
            B.SetActive(false);
        }
    }

    public void Update()
    {
        if (startEventsInvoked)
        {
            if (waveIsRunning)
            {
                for (int i = 0; i < spawnedEnemys.Count; i++)
                {
                    if (spawnedEnemys[i] == null)
                    {
                        spawnedEnemys.RemoveAt(i);
                    }
                }

                if (spawnedEnemys.Count == 0)
                {
                    waveIsRunning = false;
                    if (spawnInWaves)
                    {
                        currentWave++;
                        if (currentWave <= 2)
                        {
                            StartSpawning();
                        }
                        else
                        {
                            if(!endEventsInvoked)
                            FinishedWaves();
                        }
                    }
                }
            }
            else
            {
                if (spawnInWaves)
                {
                    if (currentWave > 2)
                    {
                        if (!endEventsInvoked)
                            FinishedWaves();
                    }
                }
                else
                {
                    if (!endEventsInvoked)
                        FinishedWaves();
                }
            }
        }
    }

    public void StartSpawning()
    {
        if (!startEventsInvoked)
        {
            foreach (GameObject B in Barriers)
            {
                B.SetActive(true);
            }

            StartWavesEvents.Invoke();



            startEventsInvoked = true;
            print("invoked!");

            GameObject.Find("Player").GetComponent<Player>().SetPlayerCurrentRespawn(playerRespawn);
        }
            

        if (currentWave == 0)
        {
            for (int i = 0; i < enemys_Wave_1.Length; i++)
            {
                StartCoroutine(spawnEnemy(i));
            }

            StartCoroutine(SetWaveRunning());
        }

        if (currentWave == 1)
        {
            for (int i = 0; i < enemys_Wave_2.Length; i++)
            {
                StartCoroutine(spawnEnemy(i));
            }

            StartCoroutine(SetWaveRunning());
        }

        if (currentWave == 2)
        {
            for (int i = 0; i < enemys_Wave_3.Length; i++)
            {
                StartCoroutine(spawnEnemy(i));
            }

            StartCoroutine(SetWaveRunning());
        }




    }
    private IEnumerator SetWaveRunning()
    {
        yield return new WaitForSecondsRealtime(1);
        waveIsRunning = true;
    }
        private IEnumerator spawnEnemy( int selectedEnemy)
    {
        if (currentWave == 0) {
            //Do spawn Animation
            GameObject Animation = Instantiate(Spawnanimation, enemys_Wave_1[selectedEnemy].Spawnpoint.position,Quaternion.identity);
            Animation.transform.localScale = new Vector3(0, 0, 0);
            MeshRenderer MR = Animation.GetComponent<MeshRenderer>();

            while (Animation.transform.localScale.x < enemys_Wave_1[selectedEnemy].AnimationScale)
            {
                Animation.transform.localScale = new Vector3(Animation.transform.localScale.x + 0.1f, Animation.transform.localScale.y +0.1f, Animation.transform.localScale.z + 0.1f);
                MR.material.SetFloat("_Cutoff", 1 - (Animation.transform.localScale.x / enemys_Wave_1[selectedEnemy].AnimationScale)); 
                yield return new WaitForSecondsRealtime(sizeUpSpeed);
            }
            yield return new WaitForSecondsRealtime(0.5f);
            
            //SpawnEnemyHere!!!
            GameObject Enemy = Instantiate(enemys_Wave_1[selectedEnemy].EnemyObjectToSpawn, enemys_Wave_1[selectedEnemy].Spawnpoint.position, enemys_Wave_1[selectedEnemy].Spawnpoint.rotation);
            spawnedEnemys.Add(Enemy);

            while (Animation.transform.localScale.x > 0)
            {
                Animation.transform.localScale = new Vector3(Animation.transform.localScale.x - 0.1f, Animation.transform.localScale.y - 0.1f, Animation.transform.localScale.z - 0.1f);
                MR.material.SetFloat("_Cutoff", 1 - (Animation.transform.localScale.x / enemys_Wave_1[selectedEnemy].AnimationScale));
                yield return new WaitForSecondsRealtime(sizeUpSpeed);
            }

            Destroy(Animation.gameObject);
        }

        if (currentWave == 1)
        {
            //Do spawn Animation
            GameObject Animation = Instantiate(Spawnanimation, enemys_Wave_2[selectedEnemy].Spawnpoint.position, Quaternion.identity);
            Animation.transform.localScale = new Vector3(0, 0, 0);
            MeshRenderer MR = Animation.GetComponent<MeshRenderer>();

            while (Animation.transform.localScale.x < enemys_Wave_2[selectedEnemy].AnimationScale)
            {
                Animation.transform.localScale = new Vector3(Animation.transform.localScale.x + 0.1f, Animation.transform.localScale.y + 0.1f, Animation.transform.localScale.z + 0.1f);
                MR.material.SetFloat("_Cutoff", 1 - (Animation.transform.localScale.x / enemys_Wave_2[selectedEnemy].AnimationScale));
                yield return new WaitForSecondsRealtime(sizeUpSpeed);
            }
            yield return new WaitForSecondsRealtime(0.5f);

            //SpawnEnemyHere!!!
            GameObject Enemy = Instantiate(enemys_Wave_2[selectedEnemy].EnemyObjectToSpawn, enemys_Wave_2[selectedEnemy].Spawnpoint.position, enemys_Wave_2[selectedEnemy].Spawnpoint.rotation);
            spawnedEnemys.Add(Enemy);

            while (Animation.transform.localScale.x > 0)
            {
                Animation.transform.localScale = new Vector3(Animation.transform.localScale.x - 0.1f, Animation.transform.localScale.y - 0.1f, Animation.transform.localScale.z - 0.1f);
                MR.material.SetFloat("_Cutoff", 1 - (Animation.transform.localScale.x / enemys_Wave_2[selectedEnemy].AnimationScale));
                yield return new WaitForSecondsRealtime(sizeUpSpeed);
            }
        }

        if (currentWave == 2)
        {
            //Do spawn Animation
            GameObject Animation = Instantiate(Spawnanimation, enemys_Wave_3[selectedEnemy].Spawnpoint.position, Quaternion.identity);
            Animation.transform.localScale = new Vector3(0, 0, 0);
            MeshRenderer MR = Animation.GetComponent<MeshRenderer>();

            while (Animation.transform.localScale.x < enemys_Wave_3[selectedEnemy].AnimationScale)
            {
                Animation.transform.localScale = new Vector3(Animation.transform.localScale.x + 0.1f, Animation.transform.localScale.y + 0.1f, Animation.transform.localScale.z + 0.1f);
                MR.material.SetFloat("_Cutoff", 1 - (Animation.transform.localScale.x / enemys_Wave_3[selectedEnemy].AnimationScale));
                yield return new WaitForSecondsRealtime(sizeUpSpeed);
            }
            yield return new WaitForSecondsRealtime(0.5f);

            //SpawnEnemyHere!!!
            GameObject Enemy = Instantiate(enemys_Wave_3[selectedEnemy].EnemyObjectToSpawn, enemys_Wave_3[selectedEnemy].Spawnpoint.position, enemys_Wave_3[selectedEnemy].Spawnpoint.rotation);
            spawnedEnemys.Add(Enemy);

            while (Animation.transform.localScale.x > 0)
            {
                Animation.transform.localScale = new Vector3(Animation.transform.localScale.x - 0.1f, Animation.transform.localScale.y - 0.1f, Animation.transform.localScale.z - 0.1f);
                MR.material.SetFloat("_Cutoff", 1 - (Animation.transform.localScale.x / enemys_Wave_3[selectedEnemy].AnimationScale));
                yield return new WaitForSecondsRealtime(sizeUpSpeed);
            }
        }
        //while animation is running Spawn Enemy
    }

    public void FinishedWaves()
    {
        endEventsInvoked = true;
        FinishedWavesEvents.Invoke();
        foreach (GameObject B in Barriers)
        {
            B.SetActive(false);
        }

    }
}
