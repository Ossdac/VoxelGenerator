using System.Collections.Generic;

public class FillSpaces
{
    public static bool[,,] Bool3D(bool[,,] spaces)
    {
        int sideX = 0;
        int sideY = 0;
        int sideZ = 0;
        int width = spaces.GetLength(0);
        int height = spaces.GetLength(1);
        int depth = spaces.GetLength(2);
        bool[,,] doNotFill = new bool[width, height, depth];
        for (int i = 0; i < 2; i++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    ReverseFloodFill(x, y, sideZ, spaces, doNotFill);
                }
            }
            for (int x = 0; x < width; x++)
            {
                for (int z = 1; z < depth; z++)
                {

                    ReverseFloodFill(x, sideY, z, spaces, doNotFill);
                }
            }
            for (int y = 1; y < height; y++)
            {
                for (int z = 1; z < depth; z++)
                {

                    ReverseFloodFill(sideX, y, z, spaces, doNotFill);
                }

            }
            sideX = width - 1;
            sideY = height - 1;
            sideZ = depth - 1;
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (!doNotFill[x, y, z] && !spaces[x, y, z])
                    {
                        spaces[x, y, z] = true;
                    }
                }
            }
        }

        return spaces;
    }

    private static void ReverseFloodFill(int x, int y, int z, 
        bool[,,] spaces, bool[,,] doNotFill)
    {
        if (spaces[x, y, z] && !doNotFill[x, y, z])
        {
            return;
        }
        Stack<(int x, int y, int z)> stack = new Stack<(int, int, int)>();
        stack.Push((x, y, z));
        doNotFill[x, y, z] = true;

        while (stack.Count > 0)
        {
            (x, y, z) = stack.Pop();

            TryPush(x - 1, y, z, spaces, doNotFill, stack);
            TryPush(x + 1, y, z, spaces, doNotFill, stack);
            TryPush(x, y - 1, z, spaces, doNotFill, stack);
            TryPush(x, y + 1, z, spaces, doNotFill, stack);
            TryPush(x, y, z - 1, spaces, doNotFill, stack);
            TryPush(x, y, z + 1, spaces, doNotFill, stack);
        }
    }

    private static void TryPush(int x, int y, int z, 
        bool[,,] spaces, bool[,,] doNotFill, Stack<(int, int, int)> stack)
    {
        if (x >= 0 && x < spaces.GetLength(0) &&
            y >= 0 && y < spaces.GetLength(1) &&
            z >= 0 && z < spaces.GetLength(2) &&
            !spaces[x, y, z] && !doNotFill[x, y, z])
        {
            stack.Push((x, y, z));
            doNotFill[x, y, z] = true;
        }
    }
}
