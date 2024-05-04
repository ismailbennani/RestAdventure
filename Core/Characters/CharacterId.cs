using RestAdventure.Kernel;

namespace RestAdventure.Core.Characters;

public record CharacterId(Guid Guid) : Id(Guid);
