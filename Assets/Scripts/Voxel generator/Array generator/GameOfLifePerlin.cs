using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameOfLifePerlin : GameOfLife
{
    private float[,] stateA;
    private float[,] stateB;
    private bool useA = true;

    [SerializeField] private float noiseScale = 0.03f;
    [SerializeField] private float waveXSpeed = .05f;
    [SerializeField] private float waveYSpeed = .05f;

    private float offsetX = 0;
    private float offsetY = 0;



    override
    public void CustomUpdateState()
    {
        offsetX += waveXSpeed;
        offsetY += waveYSpeed;
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
        stateA = new float[size, size];
        stateB = new float[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                stateA[x, y] = Random.value < saturationValue? Mathf.PerlinNoise(x * noiseScale, y * noiseScale) : 0f;
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

    public void UpdateState(float[,] current, float[,] next)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int neighbors = CountNeighbors(x, y, current);

                if (neighbors >= lowBirth && neighbors <= highBirth)
                    next[x, y] = Mathf.PerlinNoise((x * noiseScale) + offsetX, (y * noiseScale) + offsetY);
                else if (neighbors <= lowDeath || neighbors >= highDeath)
                    next[x, y] = 0f;
                else if (current[x, y] > 0f)
                    next[x, y] = Mathf.PerlinNoise((x * noiseScale) + offsetX, (y * noiseScale) + offsetY);
                else
                    next[x, y] = 0;

                
            }
        }
    }

    int CountNeighbors(int x, int y, float[,] state)
    {
        int count = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;

                int nx = x + i;
                int ny = y + j;

                if (nx >= 0 && ny >= 0 && nx < size && ny < size && state[nx, ny] > 0f)
                    count++;
            }
        }
        return count;
    }
}
