using Hopeful.Scripts;

namespace Hopeful.UI;

public class VisualScript(Vault vault, Visual visual) : InjectScript(vault)
{
    public readonly Visual Visual = visual;
}
