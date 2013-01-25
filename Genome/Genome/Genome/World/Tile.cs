using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    class Tile
    {
        private Creature creature;
        private Plant plant;
        private Remains remains;
        private Obstacle obstacle;

        private Boolean isBlocked;

        public Tile()
        {
            clearTile();
        }

        public Boolean isImpassible()
        {
            Boolean b = false;
            if (isBlocked)
            {
                b = true;
            }
            else
            {
                b = !isEmpty();
            }
            isBlocked = b;
            return b;
        }

        public void addCreature(Creature c)
        {
            creature = c;
            isBlocked = true;
        }

        public Creature getCreature()
        {
            return creature;
        }

        public void addPlant(Plant p)
        {
            plant = p;
            isBlocked = true;
        }

        public Plant getPlant()
        {
            return plant;
        }

        public void addRemains(Remains r)
        {
            remains = r;
            isBlocked = true;
        }

        //...

        public bool isEmpty()
        {
            return plant == null && remains == null && creature == null && obstacle == null;
        }

        public bool plantPresent()
        {
            return plant != null;
        }

        public bool remainsPresent()
        {
            return remains != null;
        }

        public bool creaturePresent()
        {
            return creature != null;
        }

        public bool obstaclePresent()
        {
            return obstacle != null;
        }

        public void clearTile()
        {
            creature = null;
            plant = null;
            remains = null;
            obstacle = null;
            isBlocked = false;
        }
    }
}
