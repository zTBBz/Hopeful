using System;

namespace Hopeful.Injection;

[Flags]
public enum VaultExportSettings
{
    Services = 1 << 0,
    Decorators = 1 << 1,
}