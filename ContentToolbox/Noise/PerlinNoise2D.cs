namespace ContentToolbox.Noise;

public class PerlinNoise2D : FastNoiseLiteNoise2D
{
    public PerlinNoise2D(int seed, float frequency) : base(FastNoiseLite.NoiseType.Perlin, seed, frequency)
    {
    }
}
