using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Genome
{
    class Simulation
    {
        private static ArrayList recognisedShapes = new ArrayList();
        private SimulationState state;

        public Simulation()
        {
            parseShapes("shapes.txt");
            state = new WorldState();
        }

        public static ArrayList getShapes()
        {
            return recognisedShapes;
        }

        private void parseShapes(String fileLocation)
        {

        }
    }
}
