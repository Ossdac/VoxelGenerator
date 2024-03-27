using UnityEngine;

public class GameOfLife3D : GameOfLife
{
    private bool[,,] stateA;
    private bool[,,] stateB;
    private bool useA = true;

    [SerializeField ] private bool bottomFill;
    [SerializeField ] private bool topFill;
    [SerializeField ] private bool diagonalsFill;

 

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

        useA = !useA; // Swap which array is active
    }

    override
    public void InitializePattern()
    {
        stateA = new bool[size, size, size];
        stateB = new bool[size, size, size];
        useA = true;
        stateA = new bool[size, size, size];
        bool totalFill = true;
        if (bottomFill)
        {
            totalFill = false;
            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    stateA[x, 0, z] = Random.value < saturationValue;
                    
                }
            }
        }
        if (topFill)
        {
            totalFill = false;
            for (int x = 0; x < size; x++)
            {
                for (int z = 0; z < size; z++)
                {
                    stateA[x, size-1, z] = Random.value < saturationValue;  
                }
            }
        }
        if (diagonalsFill)
        {
            totalFill = false;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        if (x-(2*y)+z == 0)
                        stateA[x, y, z] = Random.value < saturationValue;
                    }
                }
            }
        }
        if (totalFill)
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        stateA[x, y, z] = Random.value < saturationValue;
                    }
                }
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


    private void UpdateState(bool[,,] current, bool[,,] next)
    {

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    int neighbors = CountNeighbors(x, y, z, current);

                    if (!current[x, y, z] && neighbors >= lowBirth && neighbors <= highBirth)
                        next[x, y, z] = true;
                    else if (current[x, y, z] && (neighbors <= lowDeath || neighbors >= highDeath))
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
