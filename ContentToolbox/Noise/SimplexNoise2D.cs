using ContentToolbox.Noise.External;

namespace ContentToolbox.Noise;

public class SimplexNoise2D : FastNoiseLiteNoise2D
{
    public SimplexNoise2D(int seed, float frequency) : base(FastNoiseLite.NoiseType.OpenSimplex2, seed, frequency)
    {
    }
}
