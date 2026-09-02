## New Mob AI (W.I.P.)

This might be a waste of time.  While this is probably how I should have done this originally (and would in a new
project), and would have simplified the development of a lot of other things, the old system is stable and sufficient.
All the challenges and problems a system like this could have spared me have already been solved in the old system.

I may or may not bother to finish this, as its only real purpose would to learn and develope the system for future use.
(Well, maybe, maybe, it would have some benefit in adding new monsters that might never be added anyway).

### History and Ideas for State Repressentation

1. The original Caverns of Evil system.  States are Scriptable Objects inheriting from a common abstaract base class 
defining a common interface. All methods must take the creatures (some derivative of EntityMob) using the AI as a 
parameter, so that the effects can be applied to them.  Likewise, any variables needed must be stored on the creatures 
object instances; the the state class must therefore also be of a required type that has the required data fields. This, 
along the clunky state hierachy, makes this a very clunky stystem.

2. Use in my recent (2025) RPG project.  States are once again Scriptable Objects with a common abstract base class. 
However, all variables needed by the AI code are fields of the states own class, the creature to which the AI belongs 
is also held a field.  When a class using one of these classes is initialized in its Awake() or Start() method, a new 
copy of the state instantiated from the that provided to the prototype and the new copy is assigned to the variable as 
a replacement for the original.  This allows the state to hold there own values without altering the original. however 
this is heavy weights, and the instantiated state objects would need to be destroyed explicitly allong with the creature 
holding them. It seems to work well, in short tests with few characters, but this might have broken down due memory 
links as no explicit destruction was included int he original implementation (perhaps the streaming of scenes in and 
out of memory would have prevented this).

3. Suggestion: It might be a better idea to use pure C# classes, and have sciptable objects with factores to create them. 
(See 5, below.)

4. The System Setup in this Folder: States are pure C# classes, identified through an enum with instances generated and 
retrieved from a static Dictionary based on their corestponding enum.  The enum and static initializer from the dictionary 
would be edited as states are created to add them.  This is light weight, and no worse than created a scripable object 
for each state.  However, it makes including any non-script assets with a state a problem, as there is no way to add 
prefabs and other assets to the pure C# class.  This can likely be done through resource loading or addressable, but this 
would make thing far harder, more complicated, and more time consuming that is needs to be, as it if effectively fighting 
the engine and its intended/expected usage. 

5. A Better System (See 3 Above): Use pure C# classes as with 4.  However, these would be packaged with scriptable 
objects through with data fields could be set.  The actual AI state would be a *Impl, or else the Scriptable Objects would 
be a *Def name, but one name being and extension of the other.  The scriptable object would contain a factory method 
(inherited from an abstract base class) which would create the state intance, set all the predefined variables, and return 
it. The only obvious disadvange is that each AI state would need two field (one for the scriptable object, and another for 
the real AI state class). 

Perhaps for future projects (for a future RPG projects, or reboot of the previous one) I'll try approach 5, as least if the 
is going to be a state machine involved (alone or a part of a wider system).
