## Hopeful Philosophy

# Principles

1. **Hard Decomposition**
ECS systems exclusively manage their own components. They are not responsible for verifying the presence of other components on an Entity (soft ensure). This validation falls under Top-level responsibilities.

2. **Top-Low-Level Separation**
Top-level: Entities are constructed via validated, specialized builders.
Low-level: Direct component manipulation is permitted, but developers assume full responsibility for functional integrity.

3. **Naked Data**
Component data fields are always public. Manual intervention is allowed but carries inherent risks.

4. **Future-Shot**
Code must prioritize maximal extensibility during implementation.

5. **AStatic**
Static classes/data are prohibited. Use services instead (without Service Locator). Derived from Future-Shot.

# Architectural Patterns

1. **ECS-Centric Logic**
All game logic follows a strict component-system pipeline. Not EventBus Events and non-ECS operations ("gray operations") are forbidden.

2. **Reactive UI**
UI operates under constrained reactivity:
May modify component data.
Cannot directly create/delete Entities (deferred execution via CommandBuffer only).

3. **Clear IoC**
Dependencies are loosely coupled, easily replaceable, and avoid IL, code-generation "magic" injection patterns.

4. **Proxy Services**
Non-game-logic systems (e.g., assets loading, networking) are implemented as swappable, runtime-replaceable services. These:
* Exist outside the ECS pipeline.
* Never interact with Entities directly.

# Code Style

1. **Eco-Line**
Minimize lines while preserving readability:
* Ternary operations
* if/else/for/while/foreach without {}
* Lambda expressions
* Discard redundant syntax.

2. **File-Scoped Namespaces**
Required using of file-scoped namespaces. Derived from Eco-Line.

3. **DRY Files**
File content must strictly match its declared purpose. Extraneous logic is split into separate files.