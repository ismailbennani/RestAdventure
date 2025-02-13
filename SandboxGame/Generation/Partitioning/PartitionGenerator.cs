﻿using SandboxGame.Generation.Shaping;

namespace SandboxGame.Generation.Partitioning;

public abstract class PartitionGenerator
{
    public abstract Partition Generate(Land land);
}
