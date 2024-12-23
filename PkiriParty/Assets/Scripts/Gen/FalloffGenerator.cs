using UnityEngine;

public class FalloffGenerator
{
    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = Mathf.Abs(i / (float)(size - 1) * 2 - 1);
                float y = Mathf.Abs(j / (float)(size - 1) * 2 - 1);

                // Linear falloff instead of radial
                float value = Mathf.Max(x, y);
                map[i, j] = Evaluate(value); // Use the Evaluate function to control the curve
            }
        }

        return map;
    }


    static float Evaluate(float value)
    {
        float a = 2;   // Lower value makes the falloff less steep
        float b = 2f;  // Adjust this to change the curve's shape

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
