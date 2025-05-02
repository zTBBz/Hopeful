using DryIoc;
using Friflo.Engine.ECS.Systems;
using Hopeful.Asset;
using System;

namespace Hopeful;

public interface IGameServices
{
    Container Container { get; }
    Lazy<AssetManager>? Assets { get; }
    Lazy<SystemRoot>? SystemRoot { get; }
}
