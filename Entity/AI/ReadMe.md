This whole AI system is badly designed.  In particular, the
behavior objects, because they are scripatable objects, cannot
hold runtime information.  As a result, the mobs which use them
must hold such data, meaning they mobs needs to be designed to
work with specific behavior objects which must inturn be
designed to work with specific mobs, tying the two together.
Given that, there is no real advantage over using pure C#
classes.  If anything, it is actually worse, as all it does is
add the extra overhead of creating and editing the scripatable
object separately and adding it to the mob.

For more on this, see the discussion under in the folder for
"StateMachine" where more detail about this and other options
that could have been used is discussed.

