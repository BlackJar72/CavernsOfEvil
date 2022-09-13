# Caverns Of Evil
The Scripts (and only the scripts) for the game Caverns of Evil (and only those specific to Caverns of Evil).

Caverns of Evil is an action-roguelite that plays like a '90s first-person shooter, though with a few innovations 
and quirks of its own.  Fight through ever-increasing hordes of monsters while the gathering ever more powerful 
weapons you'll need to survive.  Explore entirely new procedural levels every time, adapting to ever changing level 
design.  This is a simple game, designed for some good old-fashioned violent fun.

* https://store.steampowered.com/app/1929380/Caverns_of_Evil/
* https://jaredbgreat.itch.io/caverns-of-evil

This is my first Unity game and only my fourth game, counting arcade games.  Unity turned out to be so easy that it 
produced a lot of over confidence as half-way through there seemed to be no bugs and everything seemed to be heading 
toward a great final form and my first commercial game -- tha was a mistake.  Bugs showed up that I have no clue how 
to fix. This should never been released on Steam, but simply treated as a learning project.  Unfortunately, Steam 
does not seem to allow a game to be changed from paid to free once published, so I have made it as cheap as is allowed.

**This has been abandoned, the files for the game deleted in a hard drive malfunction, so there will be no further updates.**

## About the Code

The code is of varying quality.  I'm generally proud of the level generation, both that ported from Doomlike Dungeons 
(though some of it was broken in the process) and that which was added.  I suppose a better programmer could have 
made the meshing a bit more clever so as to have not use so many near cut-n-paste methods, but it works pretty well. 
This is over half the code base.

The coding for mobs is so-so.  The player model is terrible, suffering a lot from not understanding how Unity worked 
early in the project, and connect the player, items, and UI through hard-coding in a way that is extremely inflexible 
prone to breaking.  Further, because of some scene settings, player data is made persistent between levels through 
an overly complex system of caching into static variable from which the real data is restored at the start of the 
new level -- a system the spreads accross several classes, including all items due to the hard-coded connection to 
the player model.  In short, the player model is probably not a good thing to immitate.

One other thing I would do differently now is implementing the AI states as a simpler and more consistent state machine, 
similar to that of Doom (1993).  Here I have a list of starting states in order of preference, and from there each state 
just includes references to whatever others it can transistion to.  I imagined this would be more flexible, since any 
state could be set to transition to any other free form.  In reality it was less flexible, while a class containing 
standard states (possibly with a "special" field containing included) would have made re-used of AI states much simpler. 
Such state fileds would likely include: idle, wander, chase, melee, missle, pain, die, and special (a list).  This would 
not technically prevent the use substates referenced directly from states if desired, though this would probably have 
been niether necessary nor generally desirable in term sof consistency or decoupling.  As it is the state machine is 
disorganized with states that are typically hard to reused.

These scripts contain referrence to third party code, most notably FastPriorityQueue by BlueRaja, but also the paid 
assets Procedutal Lightening and FinalIK.  These entirely separate from this project and not include, but are required 
to use this code as-is.

## Graphical and Sound Assets

None of these are included with the scripts or covered by the Licenses below.  Wall and floor textures about 2/3 my 
own creation, 1/3 are scrunged from various free and non-free assets from the Unity Asset store.  Models and skins 
are from purchased assets.  Sound effects were created from pre-made assets but edited by me as the game creator. 
Music was all written, synthesized, mixed, and mastered by me as well, though not for this game but as a hobby project 
in the 00's. 

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

