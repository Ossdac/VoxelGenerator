using UnityEngine;

public class GameOfLifeSurvivor : GameOfLife
{


    private int[,] stateA;
    private int[,] stateB;
    private bool useA = true;



    override
    public void CustomUpdateState()
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

        useA = !useA;
    }

    override
    public void InitializePattern()
    {
        stateA = new int[size, size];
        stateB = new int[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                stateA[x, y] = Random.value < saturationValue ? 1 : 0;
            }
        }
        
    }

    override
    public void RenderWorld()
    {
        if (useA)
            world.GenerateWorld(stateA);
        else
            world.GenerateWorld(stateB);
    }

    public void UpdateState(int[,] current, int[,] next)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int neighbors = CountNeighbors(x, y, current);

                if (neighbors >= lowBirth && neighbors <= highBirth)
                    next[x, y] += 1;
                else if (current[x, y] > 0 && (neighbors <= lowDeath || neighbors >= highDeath))
                    next[x, y] -= 1;
                else
                    next[x, y] = current[x, y];
            }
        }
    }

    int CountNeighbors(int x, int y, int[,] state)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                int nx = x + i;
                int ny = y + j;

                if (nx >= 0 && ny >= 0 && nx < size && ny < size)
                    count += state[nx, ny];
            }
        }
        return count;
    }
}
