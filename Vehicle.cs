using System;
using System.Collections.Generic;
using System.Text;

namespace 结课作业
{
    class Vehicle
    {
        public Vehicle(int VehicleID,List<Point> Path,int Alpha,int EarDepTime,Dictionary<Point,int> MinOperationTime)
        {
            this.VehicleID = VehicleID;
            this.Path = Path;
            this.Alpha = Alpha;
            this.EarDepTime = EarDepTime;
            this.MinOperationTime = MinOperationTime;
            EarliestTime = new Dictionary<Point, int>();
            LatestTime = new Dictionary<Point, int>();
            
        }
        public int Alpha;
        public int VehicleID;
        public List<Point> Path;
        public double Level;
        public int EarDepTime;
        public Dictionary<Point, int> MinOperationTime;
        public int MaxDelayTime = 3600;
        public Dictionary<Point, int> EarliestTime;
        public Dictionary<Point, int> LatestTime;
        public AdjacencyList Adj;
    }
}
