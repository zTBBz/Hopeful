using Friflo.Engine.ECS;
using Microsoft.Xna.Framework.Input;

namespace Hopeful.Input;

public struct InputCache : IComponent
{
    public MouseState CurrentMouseState;
    public MouseState PreviousMouseState;
    public KeyboardState CurrentKeyboardState;
    public KeyboardState PreviousKeyboardState;
}
