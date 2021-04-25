using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace 结课作业
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadData.Read();
            Parallel.ForEach(ReadData.Vehicles, (V, ParallelLoopState) =>
             {
                 SpaceTimeNetwork STN = new SpaceTimeNetwork(V);
             });
            //foreach (var V in ReadData.Vehicles)
            //{
            //    SpaceTimeNetwork STN = new SpaceTimeNetwork(V);
            //}
            MSA.Start();
        }
    }
}
