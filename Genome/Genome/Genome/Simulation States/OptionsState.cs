using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genome
{
    class OptionsState : Menu
    {
        public OptionsState() : base()
        {
            BranchOption initialState = new BranchOption("Options", "", this);
            BranchOption simulationOption = new BranchOption("Simulation options", "Options related to how the interface looks and functions", this);
            simulationOption.addOption(new BoolLeafOption("Follow selected world objects: ", "If true the viewing window will follow any selected creature plant or remains", this, Simulation.getFollowOnClick, Simulation.setFollowOnClick));
            simulationOption.addOption(new IntLeafOption("Generation to stop at: ", "Sets the generation to stop running at, useful if you need to check progress at a specific state.", this, Simulation.getTargetGeneration, Simulation.setTargetGeneration, int.MaxValue, -1));
            simulationOption.addOption(new IntLeafOption("Number of ticks per generation: ", "Sets how many ticks it takes before a generation ends, minimum value: 1", this, Simulation.getRoundLength, Simulation.setRoundLength, int.MaxValue, 1));
            simulationOption.addOption(new BoolLeafOption("Judge on death: ", "If true the simulation will automatically jump to judging the creatures when all of them have died, recommended to be kept as true", this, Simulation.judgeIfAllDead, Simulation.setJudgeIfAllDead));
            initialState.addOption(simulationOption);

            BranchOption worldOption = new BranchOption("World options", "Options related to plants, remains and world populations", this);
            worldOption.addOption(new IntLeafOption("Remains food value", "The energy the creature will get back from eating remains", this, Simulation.getRemainsFoodValue, Simulation.setRemainsFoodValue));
            worldOption.addOption(new IntLeafOption("Food units per remains", "The amount of times a remains object can be eaten before being depleted", this, Simulation.getRemainsFoodAmount, Simulation.setRemainsFoodAmount));
            worldOption.addOption(new IntLeafOption("Remains decay time", "The amount of ticks it takes for a single food unit to decay from the remains", this, Simulation.getNumTicksToDecayRemains, Simulation.setNumTicksToDecayRemains));
            worldOption.addOption(new IntLeafOption("Plant food value", "The energy a creature will get back from eating plants", this, Simulation.getPlantFoodValue, Simulation.setPlantFoodValue));
            worldOption.addOption(new IntLeafOption("Food units per plant", "The amount of times a plant can be eaten before becoming depleted", this, Simulation.getPlantFoodMax, Simulation.setPlantFoodMax));
            worldOption.addOption(new IntLeafOption("Plant regrow time", "The amount of time a plant must be unmolested in order to regrow a single food unit", this, Simulation.getNumTicksToRegrowPlant, Simulation.setNumTicksToRegrowPlant));
            worldOption.addOption(new IntLeafOption("Plant population", "The amount of plants in the world", this, Simulation.getPlantPopulation, Simulation.setPlantPopulation));
            worldOption.addOption(new IntLeafOption("Creature population", "The number of creatures put in the world at the start of each round", this, Simulation.getPopulation, Simulation.setPopulation));
            worldOption.addOption(new IntLeafOption("Number of obstacles", "The number of obstacles in the world", this, Simulation.getNumObstacles, Simulation.setNumObstacles));
            initialState.addOption(worldOption);

            BranchOption creatureOption = new BranchOption("Creature options", "Options related to how creatures live and behave", this);
            //TODO: add allow move if costs health bool
            //TODO: add converting energy into stamina per tick
            creatureOption.addOption(new IntLeafOption("Starving energy level", "The level of energy below which creatures will see themselves as starving, changing their behaviour", this, Simulation.getStarvingEnergyLevel, Simulation.setStarvingEnergyLevel));
            creatureOption.addOption(new IntLeafOption("Wounded energy level", "The level of health below which creatures will see themselves as wounded, changing their behaviour", this, Simulation.getWoundedHealthPercent, Simulation.setWoundedHealthPercent));
            creatureOption.addOption(new IntLeafOption("Energy drain per tick", "The amount of energy a creature will lost per tick, just by continuing to exist", this, Simulation.getEnergyDrainPerTick, Simulation.setEnergyDrainPerTick));
            creatureOption.addOption(new IntLeafOption("Stamina regen percent per tick", "The percentage of stamina that is regenerated per turn", this, Simulation.getStaminaRegenSpeed, Simulation.setStaminaRegenSpeed, 100, 1));
            creatureOption.addOption(new IntLeafOption("Health regen percent per tick", "The percentage of health that is regenerated per turn", this, Simulation.getHealthRegenSpeed, Simulation.setHealthRegenSpeed, 100, 1));
            initialState.addOption(creatureOption);

            BranchOption judgingOption = new BranchOption("Judging options", "Options related to how the creatures are judged by the program", this);
            //TODO: add ignore dead creatures bool
            judgingOption.addOption(new IntLeafOption("Energy weight", "How heavily the judging state values energy in creatures", this, Simulation.getEnergyWeight, Simulation.setEnergyWeight));
            judgingOption.addOption(new IntLeafOption("Health weight", "How heavily the judging state values health in creatures", this, Simulation.getHealthWeight, Simulation.setHealthWeight));
            judgingOption.addOption(new IntLeafOption("Top percentage", "What percentage of the creatures is considered high performing and thus has its children make up 50% of the next generation", this, Simulation.getTopPercentage, Simulation.setTopPercentage, 99, 1));
            judgingOption.addOption(new IntLeafOption("Elimination percentage", "What percentage of creatures are eliminated when judging", this, Simulation.getElimPercentage, Simulation.setElimPercentage, 99, 1));
            initialState.addOption(judgingOption);

            select(initialState);
        }
    }
}
