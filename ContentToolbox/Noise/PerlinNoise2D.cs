namespace ContentToolbox.Noise;

public class PerlinNoise2D : FastNoiseLiteNoise2D
{
    public PerlinNoise2D(float frequency) : base(FastNoiseLite.NoiseType.Perlin, frequency)
    {
    }
}
