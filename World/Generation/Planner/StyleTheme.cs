using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{
    public class StyleTheme
    {
        public Degree pools;
        public Degree subrooms; // Really doors in rooms
        public Degree islands;  // Actual subrooms
        public Degree pillars;
        public Degree symmetry;
        public Degree variability;
        public Degree complexity;
        public Degree vertical;
        public Degree naturals;


        public StyleTheme(DungeonTheme theme, Xorshift random)
        {
            pools = theme.Pools.Select(random);
            subrooms = theme.Doors.Select(random); // Really doors in rooms
            islands = theme.Subrooms.Select(random);   // Actual subrooms
            pillars = theme.Pillars.Select(random);
            symmetry = theme.Symmetry.Select(random);
            variability = theme.Variability.Select(random);
            complexity = theme.Complexity.Select(random);
            vertical = theme.Verticality.Select(random);
            naturals = theme.Naturals.Select(random);
        }


        public StyleTheme(DungeonTheme theme)
        {
            pools = theme.Pools.Select();
            subrooms = theme.Doors.Select(); // Really doors in rooms
            islands = theme.Subrooms.Select();   // Actual subrooms
            pillars = theme.Pillars.Select();
            symmetry = theme.Symmetry.Select();
            variability = theme.Variability.Select();
            complexity = theme.Complexity.Select();
            vertical = theme.Verticality.Select();
            naturals = theme.Naturals.Select();
        }

    }
}