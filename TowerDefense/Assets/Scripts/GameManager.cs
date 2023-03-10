using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateSpawner
{
    Start,
    Spawn,
    Stop,
    Break
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get { return ins; } }
    private static GameManager ins;

    public StateSpawner stateSpawner { get; private set; }

    [Header("Properties")]
    public List<Transform> pathManager;
    public Transform[] boxPlane;
    public GameObject[] enemies;
    public float startTime;
    public float coolTime;
    public float breakTime;
    public int maxEnemy;

    public int indexBox;
    public bool holdCard = false;
    public bool showModel = false;

    [Space(8)]
    [Header("My-Status")]
    public int hp = 20;
    public int money = 1000;

    private string enemyTypeMessage;
    private int currentWave = 1;
    private int randomIndex;
    private int currentEnemy;
    private float currentTime;
    private float multipleStatus = 1f;

    private void Awake()
    {
        ins = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentEnemy = maxEnemy;
        currentTime = startTime;
        randomIndex = Random.Range(0, enemies.Length);
        UIManager.Instance.SetActiveText(UIManager.Instance.myCommand, false);
        UIManager.Instance.SetActiveText(UIManager.Instance.enemyTypeText, false);
        UIManager.Instance.SetActiveText(UIManager.Instance.gameOverPanel, false);
        UIManager.Instance.SetText(UIManager.Instance.waveText, $"Wave : {currentWave}", Color.white);
        stateSpawner = StateSpawner.Start;
    }

    // Update is called once per frame
    void Update()
    {
        if(UIManager.Instance.enemyTypeText.activeInHierarchy) UIManager.Instance.SetText(UIManager.Instance.enemyTypeText, $"Enemy : {enemyTypeMessage}", Color.white);
        UIManager.Instance.SetText(UIManager.Instance.hpText, $"{hp}", Color.white);
        UIManager.Instance.SetText(UIManager.Instance.moneyText, $"{money}", Color.white);

        if (hp <= 0)
        {
            UIManager.Instance.SetActiveText(UIManager.Instance.gameOverPanel, true);
            UIManager.Instance.SetText(UIManager.Instance.scoreText, $"WIN\n{currentWave} Wave", Color.red);
            Time.timeScale = 0;
        }

        switch (stateSpawner)
        {
            case StateSpawner.Start:
                {
                    if (currentTime > 0) currentTime -= Time.deltaTime;
                    else
                    {
                        UIManager.Instance.SetActiveText(UIManager.Instance.timeText, false);
                        UIManager.Instance.SetActiveText(UIManager.Instance.enemyTypeText, true);
                        stateSpawner = StateSpawner.Spawn;
                    }

                    UIManager.Instance.SetText(UIManager.Instance.timeText, $"Prepare : {currentTime.ToString("F0")}", Color.white);
                    break;
                }
            case StateSpawner.Spawn:
                {
                    //Debug.Log("Spawn");
                    if (currentEnemy > 0)
                    {
                        var obj = Instantiate(enemies[randomIndex], transform.position, Quaternion.Euler(0, 90, 0));
                        obj.GetComponent<EnemyBase>().hp *= multipleStatus;
                        obj.GetComponent<EnemyBase>().speed *= multipleStatus;
                        enemyTypeMessage = obj.GetComponent<TypeManager>().baseType.ToString();
                        currentTime = coolTime;
                        if (randomIndex == 0) currentTime = coolTime + 0.75f;
                        else if (randomIndex == 1) currentTime = coolTime + 1.5f;
                        else if (randomIndex == 2) currentTime = coolTime + 3f;
                        stateSpawner = StateSpawner.Stop;
                    }
                    else
                    {
                        currentTime = breakTime - currentWave;
                        if (currentTime < 10) currentTime = 10;
                        UIManager.Instance.SetActiveText(UIManager.Instance.timeText, true);
                        UIManager.Instance.SetText(UIManager.Instance.timeText, $"Next Wave : {currentTime.ToString("F0")}", Color.red);
                        stateSpawner = StateSpawner.Break;
                    }
                    break;
                }
            case StateSpawner.Stop:
                {
                    //Debug.Log("Stop");
                    if (currentTime > 0) currentTime -= Time.deltaTime;
                    else
                    {
                        currentEnemy--;
                        stateSpawner = StateSpawner.Spawn;
                    }
                    break;
                }
            case StateSpawner.Break:
                {
                    //Debug.Log("Break");
                    UIManager.Instance.SetText(UIManager.Instance.timeText, $"Next Wave : {currentTime.ToString("F0")}", Color.red);
                    if (currentTime > 0) currentTime -= Time.deltaTime;
                    else
                    {
                        randomIndex = Random.Range(0, enemies.Length);
                        currentTime = coolTime;
                        currentEnemy = maxEnemy + currentWave;
                        money += 100 + (currentWave * 2);
                        hp += 3;
                        if (hp > 20) hp = 20;
                        currentWave++;
                        multipleStatus += 0.005f;
                        UIManager.Instance.SetText(UIManager.Instance.waveText, $"Wave : {currentWave}", Color.white);
                        UIManager.Instance.SetActiveText(UIManager.Instance.timeText, false);
                        GetComponent<MyCardDeck>().DrawCard();
                        stateSpawner = StateSpawner.Spawn;
                    }
                    break;
                }
        }
    }
}
