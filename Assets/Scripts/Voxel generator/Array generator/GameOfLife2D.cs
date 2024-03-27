using UnityEngine;

public class GameOfLife2D : GameOfLife 
{
    private bool[,] stateA;
    private bool[,] stateB;
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
        stateA = new bool[size, size];
        stateB = new bool[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                stateA[x, y] = Random.value < saturationValue;
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

    public void UpdateState(bool[,] current, bool[,] next)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int neighbors = CountNeighbors(x, y, current);

                if (!current[x, y] && neighbors >= lowBirth && neighbors <= highBirth)
                    next[x, y] = true;
                else if (current[x, y] && (neighbors <= lowDeath || neighbors >= highDeath))
                    next[x, y] = false;
                else
                    next[x, y] = current[x, y];
            }
        }
    }

    int CountNeighbors(int x, int y, bool[,] state)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                int nx = x + i;
                int ny = y + j;

                if (nx >= 0 && ny >= 0 && nx < size && ny < size && state[nx, ny])
                    count++;
            }
        }
        return count;
    }
}
