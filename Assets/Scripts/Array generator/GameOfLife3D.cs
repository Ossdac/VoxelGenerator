using UnityEngine;

public class GameOfLife3D : MonoBehaviour
{
    [SerializeField] private int size = 10;
    [SerializeField] private Vector2Int birthRange = new Vector2Int(9, 10);
    [SerializeField] private Vector2Int deathRange = new Vector2Int(6, 11);
    [SerializeField] private bool manualUpdateClick = false;
    [SerializeField] private bool manualUpdateHold = false;
    [SerializeField] private float updateInterval = 1.0f;
    [SerializeField][Range(0f, 1f)] private float saturationValue = 0.5f;

    [SerializeField] private World world;


    private float timer = 0f; // Timer to track update intervals


    private bool[,,] stateA;
    private bool[,,] stateB;
    private bool useA = true; // Determines which state (A or B) is currently active

    [ContextMenu("Reset")]
    void Start()
    {
        stateA = new bool[size, size, size];
        stateB = new bool[size, size, size];

        // Initialize stateA with some pattern
        InitializePattern();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Start();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            saturationValue -= .05f;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            saturationValue += .05f;
        }
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            manualUpdateClick = !manualUpdateClick;
            manualUpdateHold = !manualUpdateClick;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            manualUpdateClick = false; manualUpdateHold = false;
        }

        if (manualUpdateClick)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PerformUpdate();
            }
        }
        else if (manualUpdateHold)
        {
            timer += Time.deltaTime;
            if (Input.GetKey(KeyCode.Space) && timer >= updateInterval)
            {
                PerformUpdate();
                timer = 0f;
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= updateInterval)
            {
                PerformUpdate();
                timer = 0f; // Reset timer after an update
            }
        }
    }


    void PerformUpdate()
    {
        if (useA)
        {
            UpdateState(stateA, stateB);
            world.GenerateWorld(stateA);
        }
        else
        {
            UpdateState(stateB, stateA);
            world.GenerateWorld(stateB);
        }

        useA = !useA; // Swap which array is active
    }

    void InitializePattern()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    // Randomly set each cell to be alive or dead
                    stateA[x, y, z] = Random.value < saturationValue;
                }
            }
        }
        world.GenerateWorld(stateA);
    }


    void UpdateState(bool[,,] current, bool[,,] next)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    int neighbors = CountNeighbors(x, y, z, current);

                    // Apply birth and death ranges
                    if (!current[x, y, z] && neighbors >= birthRange.x && neighbors <= birthRange.y)
                        next[x, y, z] = true;
                    else if (current[x, y, z] && (neighbors <= deathRange.x || neighbors >= deathRange.y))
                        next[x, y, z] = false;
                    else
                        next[x, y, z] = current[x, y, z];
                }
            }
        }
    }

    int CountNeighbors(int x, int y, int z, bool[,,] state)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    if (i == 0 && j == 0 && k == 0) continue; // Skip the cell itself

                    int nx = x + i;
                    int ny = y + j;
                    int nz = z + k;

                    
                    if (nx < 0 || ny < 0 || nz < 0 || nx >= size || ny >= size || nz >= size) { 
                        //count += 1;
                    }
                    else if (state[nx, ny, nz])
                        count += 1;
                }
            }
        }
        return count;
    }
}
