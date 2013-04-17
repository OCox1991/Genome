﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    /// <summary>
    /// The initialisation state handles the initial creation of however many creatures are needed in a way that
    /// allows progress to be displayed and prevents the game from just hanging at the start with no feedback
    /// </summary>
    class InitialisationState : SimulationState
    {
        private Random r; //The random number generator used by the state
        private int seed; //The seed for the generator
        private List<Creature> creatures; //The list of random creatures generated by the state
        private SingleStringDrawer drawer; //The drawing class for this state

        /// <summary>
        /// Constructor for the state, initialises the random number generator, creature list and drawer
        /// </summary>
        public InitialisationState()
        {
            seed = new Random().Next(1000000);
            r = new Random(seed);
            creatures = new List<Creature>();
            drawer = new SingleStringDrawer(this);
        }

        /// <summary>
        /// The update method used by the Xna game to perform any update logic
        /// </summary>
        /// <param name="gameTime">The time since update was last called as a TimeSpan</param>
        public override void update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (creatures.Count < Simulation.getPopulation())
            {
                creatures.Add(new Creature(r));
            }
            else
            {
                Simulation.begin(creatures, seed);
            }
        }

        /// <summary>
        /// The draw method used by XNA to draw the object, calls draw on the drawing class associated with the state
        /// </summary>
        public override void draw()
        {
            drawer.draw();
        }

        /// <summary>
        /// Overriding the toString method to give a more useful result, outputs the current progress the state has made in creating creatures
        /// </summary>
        /// <returns>A string representing the current progress of the state</returns>
        public override string ToString()
        {
            return ("Generating Creatures: " + creatures.Count + "/" + Simulation.getPopulation());
        }
    }
}
