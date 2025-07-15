using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("DNA Settings")]
    public int DnaSize;
    public int GeneSize;
    public int MutationSize;

    [Header("Population Settings")]
    public int PlayerCount;
    public int ResetCount; //Playerがこれ以下になるとReset

    [Header("Player Settings")]
    public float ActionInterval;
    public float PlayerSpeed;

    [Header("Object References")]
    public GameObject PlayerPrefab;
    public GameObject PlayerParent;
    
    private GeneticAlgorithm _geneticAlgorithm;
    private readonly List<PlayerController> _playerObjects = new();
    
    public const int TargetScore = 19;
    
    private float _gameStartTime;
    private int _generationCount = 1;

    //確認用
    public int CurrentLivePlayerCount;
    
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Time.timeScale = 24;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        _geneticAlgorithm = new GeneticAlgorithm(DnaSize, GeneSize, MutationSize);
    }

    private void Start()
    {
        _gameStartTime = Time.time;
        CurrentLivePlayerCount = PlayerCount;
        CreateInitialPopulation();
    }

    private void Update()
    {
        if (CurrentLivePlayerCount > 0 && CurrentLivePlayerCount <= ResetCount)
        {
            EvolveToNextGeneration();
        }
    }
    
    public void Goal()
    {
        var elapsedTime = Time.time - _gameStartTime;
        Debug.Log($"Game Win! | Final Generation: {_generationCount} | Elapsed Time: {elapsedTime:F2} seconds");
        Time.timeScale = 0f; 
    }
    
    public void OnPlayerDeath()
    {
        CurrentLivePlayerCount--;
    }
    
    private void CreateInitialPopulation()
    {
        for (var i = 0; i < PlayerCount; i++)
        {
            var newPlayerObject = Instantiate(PlayerPrefab, PlayerParent.transform);
            var playerController = newPlayerObject.GetComponent<PlayerController>();
            
            _playerObjects.Add(playerController);
            
            if (playerController != null)
            {
                playerController.DNA = _geneticAlgorithm.CreateRandomDna();
            }
            ResetPlayerState(playerController);
        }
    }

    private void EvolveToNextGeneration()
    {
        var currentPopulation = new List<GaAgent>();
        foreach (var playerObject in _playerObjects)
        {
            if (playerObject.gameObject.activeSelf)
            {
                currentPopulation.Add(new GaAgent
                {
                    Dna = playerObject.DNA,
                    Fitness = playerObject.Score
                });
            }
        }
        
        var nextGenerationDna = _geneticAlgorithm.Evolve(currentPopulation, PlayerCount);
        
        if (nextGenerationDna == null) //生存者がない時
        {
            foreach (var playerObject in _playerObjects)
            {
                playerObject.DNA = _geneticAlgorithm.CreateRandomDna();
                ResetPlayerState(playerObject);
            }
        }
        else
        {
            for (var i = 0; i < PlayerCount; i++)
            {
                _playerObjects[i].DNA = nextGenerationDna[i];
                ResetPlayerState(_playerObjects[i]);
            }
        }
        
        _generationCount++;
        CurrentLivePlayerCount = PlayerCount;
    }

    private void ResetPlayerState(PlayerController player)
    {
        if (player != null)
        {
            player.StartTime = Time.time;
        }

        player.transform.position = PlayerParent.transform.position;
        player.transform.eulerAngles = Vector3.zero;

        player.gameObject.SetActive(true);
    }
}
