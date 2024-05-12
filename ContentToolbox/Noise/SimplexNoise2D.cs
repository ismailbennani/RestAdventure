namespace ContentToolbox.Noise;

public class SimplexNoise2D : FastNoiseLiteNoise2D
{
    public SimplexNoise2D(float frequency) : base(FastNoiseLite.NoiseType.OpenSimplex2, frequency)
    {
    }
}
