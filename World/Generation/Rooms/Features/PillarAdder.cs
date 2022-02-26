using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DLD.Symmetry;


namespace DLD
{

    public class PillarAdder : FeatureAdder
    {
        public PillarAdder(Degree chance) : base(chance) { }


		public override void buildFeature(Level dungeon, Room room)
		{
			int pillarx1 = dungeon.random.NextInt(room.endX - room.beginX - 2) + 1;
			int pillarz1 = dungeon.random.NextInt(room.endZ - room.beginZ - 2) + 1;
			if (room.symdat.halfX) pillarx1 = ((pillarx1 - 1) / 2) + 1;
			if (room.symdat.halfZ) pillarz1 = ((pillarz1 - 1) / 2) + 1;
			int pillarx2 = room.endX - pillarx1;
			int pillarz2 = room.endZ - pillarz1;
			pillarx1 += room.beginX;
			pillarz1 += room.beginZ;
			switch (room.sym)
			{
				case NONE: break;
				case TR1:
					dungeon.map.SetPillar(pillarx1, pillarz1);
					dungeon.map.SetPillar(pillarz1, pillarx1);
					break;
				case TR2:
					dungeon.map.SetPillar(pillarx1, pillarz1);
					dungeon.map.SetPillar(pillarz2, pillarx2);
					break;
				case X:
					dungeon.map.SetPillar(pillarx1, pillarz1);
					dungeon.map.SetPillar(pillarx2, pillarz1);
					break;
				case Z:
					dungeon.map.SetPillar(pillarx1, pillarz1);
					dungeon.map.SetPillar(pillarx1, pillarz2);
					break;
				case XZ:
					dungeon.map.SetPillar(pillarx1, pillarz1);
					dungeon.map.SetPillar(pillarx1, pillarz2);
					dungeon.map.SetPillar(pillarx2, pillarz1);
					dungeon.map.SetPillar(pillarx2, pillarz2);
					break;
				case R:
					dungeon.map.SetPillar(pillarx1, pillarz1);
					dungeon.map.SetPillar(pillarx2, pillarz2);
					break;
				case SW:
					dungeon.map.SetPillar(pillarx1, pillarz1);
					dungeon.map.SetPillar(pillarz1, pillarx2);
					dungeon.map.SetPillar(pillarz2, pillarx1);
					dungeon.map.SetPillar(pillarx2, pillarz2);
					break;
			}
		}


	}

}