namespace ContentToolbox.Noise;

public class SimplexNoise2D : Noise2D
{
    readonly FastNoiseLite _noise;

    public SimplexNoise2D(float frequency)
    {
        _noise = new FastNoiseLite();
        _noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        _noise.SetFrequency(frequency);
    }

    public override double Get(int x, int y) => (_noise.GetNoise(x, y) + 1) / 2;
}
