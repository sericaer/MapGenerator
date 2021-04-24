using System.Collections;
using UnityEngine;


namespace MapGenerator
{
    class PerlinGeneration
    {
        public int[] Gen(int size, int seed = 0, float scale = 25f, int octaves = 30, float persistance = 0.286f, float lacunarity = 2.9f)
        {
            float[] noiseMap = new float[size * size];

            var random = new System.Random(seed);

            // We need atleast one octave
            if (octaves < 1)
            {
                octaves = 1;
            }

            Vector2[] octaveOffsets = new Vector2[octaves];
            for (int i = 0; i < octaves; i++)
            {
                float offsetX = random.Next(-100000, 100000);
                float offsetY = random.Next(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (scale <= 0f)
            {
                scale = 0.0001f;
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            // When changing noise scale, it zooms from top-right corner
            // This will make it zoom from the center
            float halfWidth = size;
            float halfHeight = size;

            for (int x = 0, y; x < size; x++)
            {
                for (y = 0; y < size; y++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;
                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                        // Use unity's implementation of perlin noise
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        noiseHeight += perlinValue * amplitude;
                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;

                    noiseMap[y * size + x] = noiseHeight;
                }
            } 

            int[] rslt = new int[size * size];
            for (int x = 0, y; x < size; x++)
            {
                for (y = 0; y < size; y++)
                {
                    // Returns a value between 0f and 1f based on noiseMap value
                    // minNoiseHeight being 0f, and maxNoiseHeight being 1f
                    rslt[y * size + x] = (int)(Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[y * size + x])*100);
                }
            }
            return rslt;
        }
    }
}