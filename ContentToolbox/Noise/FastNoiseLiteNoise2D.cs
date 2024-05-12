namespace ContentToolbox.Noise;

public class FastNoiseLiteNoise2D : Noise2D
{
    readonly FastNoiseLite _noise;

    public FastNoiseLiteNoise2D(FastNoiseLite.NoiseType noiseType, int seed, float frequency)
    {
        _noise = new FastNoiseLite();
        _noise.SetNoiseType(noiseType);
        _noise.SetFrequency(frequency);
        _noise.SetSeed(seed);
    }

    public double HighCutoff { get; init; } = 1;
    public double LowCutoff { get; init; } = 0;

    public override double Get(int x, int y)
    {
        float result = (_noise.GetNoise(x, y) + 1) / 2;

        if (result < LowCutoff)
        {
            return 0;
        }

        if (result > HighCutoff)
        {
            return 0;
        }

        return result;
    }
}
