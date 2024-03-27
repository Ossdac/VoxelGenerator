using UnityEngine;

public class GameOfLifeController : MonoBehaviour
{

    [SerializeField] private GameOfLife[] gamesOfLife;
    [SerializeField] private GameOfLife currentGame;

    [SerializeField] private float updateInterval = 1.0f;

    [SerializeField]private bool manualUpdateClick = false;
    [SerializeField]private bool manualUpdateHold = false;

    private float timer = 0f;
    [SerializeField] private int gameIndex;

    private void Start()
    {
        gameIndex %= gamesOfLife.Length;
        currentGame = gamesOfLife[gameIndex];

        foreach (GameOfLife game in gamesOfLife)
        {
            game.InitializePattern();
        }
        currentGame.RenderWorld();
    }

    void Update()
    {
        HandleInput();
        if (!manualUpdateClick && !manualUpdateHold)
        {
            UpdateLifeCycle();
        }
    }


    [ContextMenu("Reset")]
    private void Reset()
    {
        gameIndex = gameIndex  % gamesOfLife.Length;
        currentGame = gamesOfLife[gameIndex];
        currentGame.InitializePattern();
        currentGame.RenderWorld();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentGame.InitializePattern();
            currentGame.RenderWorld();
        }

        if (Input.GetKeyDown(KeyCode.Space) && (manualUpdateClick || manualUpdateHold))
        {
            UpdateLifeCycle();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            manualUpdateClick = !manualUpdateClick; 
            manualUpdateHold = !manualUpdateClick;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            manualUpdateClick = false; manualUpdateHold = false;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            NextGame();
        }

    }

    private void UpdateLifeCycle()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            currentGame.PerformUpdate();
            timer = 0f;
        }
    }

    private void NextGame()
    {
        gameIndex = (gameIndex + 1) % gamesOfLife.Length;
        currentGame = gamesOfLife[gameIndex];
    }
}

