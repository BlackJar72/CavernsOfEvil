# Caverns Of Evil
The Scripts (and only the scripts) for the game Caverns of Evil (and only those specific to Caverns of Evil).

Caverns of Evil is an action roguelike that plays like a '90s first-person shooter, though with a few innovations 
and quirks of its own.  Fight through ever-increasing hordes of monsters while the gathering ever more powerful 
weapons you'll need to survive.  Explore entirely new procedural levels every time, adapting to ever changing level 
design.  This is a simple game, designed for some good old-fashioned violent fun.

https://jaredbgreat.itch.io/caverns-of-evil

## Fate of the Project

This game was only originally intended as a short-term learning project with the side goal of creating a fun action 
game that I could play somewhat casually in short burst when I don't really to be sucked into a big game.  At one 
point I did begin to have fantasies of making it my first commercial game, perhaps piggy-backing on Doomlike Dungeons 
(with it millions of downloads) for publicity.

Unfortunately it fell victim to some bad early planning and ignorance of how Unity is designed to be used, leading to 
some inflexible code at its core.  After stripping down plans for further development to adding more monsters, a few 
minore tweaks to mob and ammo placement, and some aesthetic and qaulity of life addition (intro, help, and perhaps 
options screens), it fell to what seems to be an actual engine or editor/compilation bug the broke animations (apparently 
removing the animator on death, which is not in the code).  This bug is not resolved by reverting project changes to 
a previous working state, and so this has been abandoned and the project itself deleted.

These scripts make specific referenc to code not include here, including Fast Priority Queue by BlueRaja, and the 
the paid code assets Procedural Lightening and Final IK, all of which are separate from this project.

## About the Code

The code is of varying quality.  I'm generally proud of the level generation, both that ported from Doomlike Dungeons 
(though some of it was broken in the process) and that which was added.  I suppose a better programmer could have 
made the meshing a bit more clever so as to have not use so many near cut-n-paste methods, but it works pretty well. 
This is over half the code base.

The coding for mobs is so-so.  The player model is terrible, suffering a lot from not understanding how Unity worked 
early in the project, and connect the player, items, and UI through hard-coding in a way that is extremely inflexible 
prone to breaking.  Further, because of some scene settings, player data is made persistent between levels through 
an overly complex system of cachy into static variable from which the real data is restored -- a system the spreads 
accross several classes, including all items due to the hard-coded connection to the player model.  In short, the player 
model is probably not a good thing to immitate.

## License

These scripts are the creation and copyright (C) of Jared Blackburn, 2021-2022.  These scripts, and only these scripts, 
are covered under the MIT License.  This license does not extend to any other part of the game, so art assets and 
third party scripts referrenced from (but not included with) these are excluded from said license.

Copyright (C) 2021-2022 Jared Blackburn

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files 
(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

