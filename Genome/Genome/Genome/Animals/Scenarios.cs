using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    public enum Scenario
    {
        NOTHING, //USED WHEN KEEPING TRACK OF SCENARIOS IN CREATURE

        IN_COMBAT,
        IN_COMBAT_CREATURE,
        IN_COMBAT_WOUNDED,
        CREATURE,
        FOOD,
        STARVING_FOOD,
        CREATURE_FOOD,
        STARVING_CREATURE_FOOD,
        MULT_CREATURE,
        MULT_CREATURE_FOOD,
        STARVING_MULT_CREATURE_FOOD,
        CREATURE_MULT_FOOD,
        MULT_CREATURE_MULT_FOOD,
        MULT_FOOD,
        DEPLETED_PLANT,
    }

    public enum Response
    {
        //SEEING CREATURE(S) + FOOD
        IGNORE_FOOD_NON_PREFERRED,
        IGNORE_FOOD,
        IGNORE_CREATURE,
        IGNORE_BOTH,

        //MULTIPLE CREATURES
        FOCUS_WEAKEST, //MIN STR
        FOCUS_SLOWEST, //MIN SPD
        FOCUS_WOUNDED, //MIN HLTH
        FOCUS_LEAST_DEFENDED, //MIN DEF
        FOCUS_CLOSEST, //CLOSEST DISTANCE
        FOCUS_MOST_HUNGRY, //MIN ENERGY

        //MULTIPLE FOOD SOURCES
        FOCUS_MOST_NOURISHING, //THE FOOD THAT IS BEST FOR THE CREATURE REGARDLESS OF DISTANCE
        FOCUS_MOST_REMAINING, //FOOD W/ MOST UNITS REMAINING
        FOCUS_LEAST_DANGEROUS, //FOOD FURTHEST FROM ANY CREATURES VISIBLE, OR CLOSEST TO THE CURRENT CREATURE
        FOCUS_CLOSEST_PREFERRED, //CLOSEST PREFERRED FOOD
        FOCUS_LEAST_REMAINING, //FOOD W/ LEAST UNITS REMAINING
        FOCUS_MOST_EFFICIENT, //BEST FOOD VALUE FOR DISTANCE

        //SINGLE CREATURE
        ATTACK, //IMMEDIATELY CHARGE
        IGNORE, //IGNORE, ACT AS IF IT ISNT THERE
        HIDE, //STAY STILL AND HIDE
        AMBUSH, //HIDE AND TRY TO SNEAK UP ON IT
        DEFEND, //FREEZE AND DEFEND AGAINST ANY ATTACKS IT MAKES
        EVADE, //RUN AWAY!
        STALK, //TRY TO FOLLOW IT OUTSIDE ITS VIEWING RADIUS

        //SINGLE FOOD
        EAT, //eat regardless
        EAT_PREFERRED, //eat only if its the preferred food source of this creature

        //FOOD DEPLETED
        WAIT,

        //NOTHING
        RANDOM
    }

    public static class ScenarioExtensions
    {
        public static Response[] PossibleResponses(this Scenario self)
        {
            Response[] responses = new Response[7];

            switch (self) //want to make this eventually settable by the Simulation, but atm I am doing this as preset.
            {
                case Scenario.IN_COMBAT: responses = new Response[] { Response.ATTACK, Response.DEFEND, Response.EVADE, Response.ATTACK, Response.DEFEND, Response.EVADE, Response.ATTACK }; 
                    break;
                case Scenario.IN_COMBAT_CREATURE: responses = new Response[] { Response.ATTACK, Response.EVADE, Response.FOCUS_CLOSEST, Response.FOCUS_LEAST_DANGEROUS, Response.DEFEND, Response.ATTACK, Response.DEFEND };
                    break;
                case Scenario.IN_COMBAT_WOUNDED: responses = new Response[] { Response.ATTACK, Response.DEFEND, Response.EVADE, Response.ATTACK, Response.EVADE, Response.EVADE, Response.ATTACK }; 
                    break;
                case Scenario.CREATURE: responses = new Response[] { Response.ATTACK, Response.IGNORE, Response.HIDE, Response.AMBUSH, Response.DEFEND, Response.EVADE, Response.STALK }; 
                    break;
                case Scenario.FOOD: responses = new Response[] { Response.EAT, Response.EAT, Response.EAT, Response.EAT_PREFERRED, Response.EAT_PREFERRED, Response.EAT_PREFERRED, Response.EAT_PREFERRED }; 
                    break;
                case Scenario.STARVING_FOOD: responses = new Response[] { Response.EAT, Response.EAT, Response.EAT, Response.EAT, Response.EAT, Response.EAT_PREFERRED, Response.EAT_PREFERRED }; 
                    break;
                case Scenario.CREATURE_FOOD: responses = new Response[] { Response.ATTACK, Response.EVADE, Response.HIDE, Response.IGNORE_FOOD_NON_PREFERRED, Response.IGNORE_CREATURE, Response.IGNORE_FOOD, Response.IGNORE_FOOD_NON_PREFERRED }; 
                    break;
                case Scenario.STARVING_CREATURE_FOOD: responses = new Response[] { Response.ATTACK, Response.IGNORE_CREATURE, Response.IGNORE_CREATURE, Response.IGNORE_FOOD, Response.IGNORE_FOOD_NON_PREFERRED, Response.HIDE, Response.IGNORE_CREATURE }; 
                    break;
                case Scenario.MULT_CREATURE: responses = new Response[] { Response.FOCUS_WEAKEST, Response.FOCUS_SLOWEST, Response.FOCUS_WOUNDED, Response.FOCUS_LEAST_DEFENDED, Response.FOCUS_CLOSEST, Response.FOCUS_MOST_HUNGRY, Response.EVADE };
                    break;
                case Scenario.MULT_CREATURE_FOOD: responses = new Response[] { Response.IGNORE_FOOD_NON_PREFERRED, Response.IGNORE_FOOD, Response.IGNORE_CREATURE, Response.IGNORE_FOOD, Response.IGNORE_CREATURE, Response.IGNORE_CREATURE, Response.IGNORE_FOOD };
                    break;
                case Scenario.STARVING_MULT_CREATURE_FOOD: responses = new Response[] { Response.IGNORE_FOOD_NON_PREFERRED, Response.IGNORE_FOOD, Response.IGNORE_CREATURE, Response.IGNORE_FOOD, Response.IGNORE_CREATURE, Response.IGNORE_CREATURE, Response.IGNORE_FOOD };
                    break;
                case Scenario.CREATURE_MULT_FOOD: responses = new Response[] { Response.IGNORE_FOOD, Response.IGNORE_CREATURE, Response.IGNORE_FOOD_NON_PREFERRED, Response.IGNORE_FOOD, Response.IGNORE_CREATURE, Response.IGNORE_CREATURE, Response.IGNORE_CREATURE };
                    break;
                case Scenario.MULT_CREATURE_MULT_FOOD: responses = new Response[] { Response.IGNORE_FOOD, Response.IGNORE_CREATURE, Response.IGNORE_FOOD_NON_PREFERRED, Response.IGNORE_FOOD, Response.IGNORE_FOOD_NON_PREFERRED, Response.IGNORE_CREATURE, Response.IGNORE_CREATURE };
                    break;
                case Scenario.MULT_FOOD: responses = new Response[] { Response.FOCUS_CLOSEST, Response.FOCUS_CLOSEST_PREFERRED, Response.FOCUS_LEAST_DANGEROUS, Response.FOCUS_MOST_EFFICIENT, Response.FOCUS_MOST_REMAINING, Response.FOCUS_MOST_NOURISHING, Response.FOCUS_CLOSEST_PREFERRED };
                    break;
                case Scenario.DEPLETED_PLANT: responses = new Response[] { Response.WAIT, Response.IGNORE, Response.IGNORE, Response.IGNORE, Response.IGNORE, Response.IGNORE, Response.IGNORE };
                    break;
                case Scenario.NOTHING: responses = new Response[] { Response.RANDOM, Response.RANDOM, Response.RANDOM, Response.RANDOM, Response.RANDOM, Response.RANDOM, Response.RANDOM };
                    break;
            }
            return responses;
        }

        public static Scenario[] AllScenarios(this Scenario self)
        {
            return new Scenario[] { Scenario.CREATURE, Scenario.CREATURE_FOOD, Scenario.CREATURE_MULT_FOOD, Scenario.DEPLETED_PLANT, Scenario.FOOD, Scenario.IN_COMBAT, Scenario.IN_COMBAT_CREATURE, 
                                    Scenario.IN_COMBAT_WOUNDED, Scenario.MULT_CREATURE, Scenario.MULT_CREATURE_FOOD, Scenario.MULT_CREATURE_MULT_FOOD, Scenario.MULT_FOOD, Scenario.STARVING_CREATURE_FOOD, 
                                    Scenario.STARVING_FOOD, Scenario.STARVING_MULT_CREATURE_FOOD };
        }
    }
}
