namespace ContentToolbox.Noise;

public class PerlinNoise2D : Noise2D
{
    readonly FastNoiseLite _noise;

    public PerlinNoise2D(float frequency)
    {
        _noise = new FastNoiseLite();
        _noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        _noise.SetFrequency(frequency);
    }

    public override double Get(int x, int y) => (_noise.GetNoise(x, y) + 1) / 2;
}
