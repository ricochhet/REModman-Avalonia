# REModman-Avalonia
Rewrite of [REModman](https://github.com/ricochhet/REModman) using AvaloniaUI

### TODO
- Re-add seperate views / screens (Collection, Settings, Main).
- Refactoring to fit non WPF ui elements.
- Add some sort of code-formatting / conventions.
- Make REMod.Core agnostic of games.
    - We should have a seperate item in our settings to specify a root folder for the mod. (Require error handling)
        - MonsterHunterWorld is an example of an agnostic game.
        - MonsterHunterRise is partially agnostic; supports natives but also supports root level pak files.