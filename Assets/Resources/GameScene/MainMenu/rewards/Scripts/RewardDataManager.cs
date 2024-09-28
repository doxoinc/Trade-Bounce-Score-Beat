using UnityEngine;
using System;

public class RewardDataManager : MonoBehaviour
{
    public static RewardDataManager Instance;

    [Header("Reward Settings")]
    public int initialRewardAmount = 500;
    public int regularRewardAmount = 100;
    public int rewardIntervalInHours = 1; // Интервал между наградами в часах
    private TimeSpan rewardInterval;

    [HideInInspector]
    public DateTime lastRewardTime;
    [HideInInspector]
    public bool hasCollectedFirstReward;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rewardInterval = TimeSpan.FromHours(rewardIntervalInHours);
    }

    public void SaveData()
    {
        PlayerPrefs.SetString("LastRewardTime", lastRewardTime.ToBinary().ToString());
        PlayerPrefs.SetInt("HasCollectedFirstReward", hasCollectedFirstReward ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("LastRewardTime"))
        {
            long binary = Convert.ToInt64(PlayerPrefs.GetString("LastRewardTime"));
            lastRewardTime = DateTime.FromBinary(binary);
        }
        else
        {
            lastRewardTime = DateTime.MinValue;
        }

        if (PlayerPrefs.HasKey("HasCollectedFirstReward"))
        {
            hasCollectedFirstReward = PlayerPrefs.GetInt("HasCollectedFirstReward") == 1;
        }
        else
        {
            hasCollectedFirstReward = false;
        }
    }

    public void ResetReward()
    {
        lastRewardTime = DateTime.UtcNow;
        hasCollectedFirstReward = false;
        SaveData();
        Debug.Log("Reward data reset.");
    }

    public TimeSpan GetRewardInterval()
    {
        return rewardInterval;
    }
}
