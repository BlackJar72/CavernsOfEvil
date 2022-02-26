using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    public abstract class FeatureAdder
	{
		/**
		 * The chance of placing the feature on a given use.
		 */
		protected Degree chance;


		public FeatureAdder(Degree chance)
		{
			this.chance = chance;
		}


		/**
		 * This will try to add the feature to the room, based on the features
		 * pre-defined degree of chance.  The actual feature is built by calling 
		 * a method back on the room.  It will return true if the room was 
		 * instructed to add the feature, and false otherwise; whether or not the 
		 * feature was actually added successfully is another matter.
		 * 
		 * @param dungeon
		 * @param room
		 * @return the result of chance.use()
		 */
		public virtual bool addFeature(Level dungeon, Room room)
		{
			bool built = dungeon.UseDegree(chance);
			if (built) buildFeature(dungeon, room);
			return built;
		}


		/**
		 * This will actually build the feature into the room.
		 * 
		 * @param dungeon
		 * @param room
		 */
		public abstract void buildFeature(Level dungeon, Room room);

	}

}