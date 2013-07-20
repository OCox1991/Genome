using System;

namespace Genome
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Simulation sim = new Simulation())
            {
                sim.Run();
            }
        }
    }
#endif
}

