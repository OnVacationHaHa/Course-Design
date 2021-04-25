using System;
using System.Collections.Generic;
using System.Text;

namespace 结课作业
{
    class Point
    {
        public Point(int ID,int MaxVehicleNum)
        {
            this.ID = ID;
            this.MaxVehicleNum = MaxVehicleNum;
        }
        public Point(int ID) { this.ID = ID; }
        public int ID;
        public int MaxVehicleNum;
        public double[] Values;
        public int EarliestTime;
        public int[] Count;
        public double[] CountNow;
        public int TimeIndex(int Time)
        {
            return (Time - EarliestTime) /ReadData.TimeLen;
        }
        
    }
}
