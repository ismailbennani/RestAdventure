using RestAdventure.Kernel;

namespace RestAdventure.Core.Players;

public record PlayerId(Guid Guid) : Id(Guid);
