# Caverns Of Evil

The Scripts (and only the scripts) for the game Caverns of Evil (and only those specific to Caverns of Evil).

Caverns of Evil is an action roguelike that plays like a '90s first-person shooter, though with a few innovations. 
and quirks of its own.  Fight through ever-increasing hordes of monsters while the gathering ever more powerful 
weapons you'll need to survive.  Explore entirely new procedural levels every time, adapting to ever-changing levels.
design.  This is a simple game, designed for some good old-fashioned violent fun.

https://store.steampowered.com/app/1929380/Caverns_of_Evil

## About the Code

The code is of varying quality.  I'm generally proud of the level generation, both that ported from Doomlike Dungeons 
(though some of it was broken in the process) and that which was added.  I suppose a better programmer could have 
made the meshing a bit more clever so as not to use so many near cut-n-paste methods, but it works pretty well. 
This is over half the code base.

The player model is terrible, suffering a lot from not understanding how Unity worked early in the project, and 
connect the player, items, and UI through hard-coding in a way that is extremely inflexible and prone to breaking. 
Further, because of the scene setup, player data is made persistent between levels through an overly complex system 
of caching into static variables from which the real data is restored at the start of the  new level -- a system 
the spreads across several classes, including all items due to the hard-coded connection to the player model. 
(That was eventually fixed.)  In short, the player model is probably not a good thing to imitate.

The coding for mobs is so-so.  Monster AI was not done well, and should have used a finite state machine similar to 
Doom instead of the poorly Implemented (though small) hierarchies of preferred states.  A working version of a good FSM 
was created for my unfinished RPG project, and the (unused) foundation of an even better system can be found here in 
the v2.0 branch.

Monsters have only one collider, acting both a combat hit box and for colliding with solid objects such as level 
geometry.  This was a mistake; the physics collisions and hit box functionality should have been separate colliders. 
on separate game objects on different layers, allowing each to interact only with relevant parts of the game.  As it 
is, monsters sometimes get stuck because their collider leans to stay over their visible body (without this they lean 
Visibly outside of their hit box).  This was done correctly in my later RPG code, but fixing it here could be a challenge. 
as the simplest, most obvious fix causes game breaking bugs.

These scripts contain reference to third party code, most notably the paid assets Procedural Lightning, Easy Save 3, 
Quantum Console, and FinalIK.  These are entirely separate from this project and are not included, but are required to use this 
code as-is.

Also, the game uses art, sound, and other assets, both of my own creation and third party.  These are not included here 
and not covered by the below license, but would need to replaced to in order to make a working game.  Likewise, the 
The setup of the game (prefabs, data, preferences, configurations, etc.) is not included and would be needed to clone the game. 

## License

These scripts are the creation and copyright (C) of Jared Blackburn, 2021-2022.  These scripts, and only these scripts, 
are covered under the MIT License.  This license does not extend to any other part of the game, so art assets, sound, 
music, and third party scripts referenced from (but not included with) these are excluded from said license.  Anything not 
actually included in this repository is not covered by the given license, regardless of what it is. This also means that 
*THE GAME AS A WHOLE IS **NOT** OPEN SOURCE.*

#### Copyright (C) 2021-2026 Jared Blackburn

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files 
(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 
