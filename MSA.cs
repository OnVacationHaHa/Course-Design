using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace 结课作业
{
    static class MSA
    {
        public static void Init()
        {
            for (int i = 0; i < ReadData.Points.Count; i++)
            {
                Point Point = ReadData.Points[i];
                int Ear = int.MaxValue; int Lat = 0;
                ReadData.Vehicles.ForEach((a) =>
                {
                    if (a.EarliestTime.ContainsKey(Point))
                    {
                        if (a.EarliestTime[Point] < Ear)
                        {
                            Ear = a.EarliestTime[Point];
                        }
                        if (a.LatestTime[Point] > Lat)
                        {
                            Lat = a.LatestTime[Point];
                        }
                    }
                });
                Point.EarliestTime = Ear;
                Ear = Ear / ReadData.TimeLen;
                Lat = Lat / ReadData.TimeLen;
                Point.Values = new double[Lat - Ear   + 1];
                Point.Count = new int[Lat - Ear + 1];
                Point.CountNow = new double[Lat - Ear + 1];
            }
            Iteration = 1;
        }
        public static double Alpha;
        public static int Iteration;
        public static void UpdateLinkValue()
        {
            Parallel.ForEach(ReadData.Points, (Point, ParallelLoopState) =>
             {
                 for (int i = 0; i < Point.Values.Length; i++)
                 {
                     if (Point.CountNow[i] < Point.MaxVehicleNum)
                     {
                         Point.Values[i] = 0;
                     }
                     else
                     {
                         Point.Values[i] = Point.CountNow[i] - Point.MaxVehicleNum;
                     }
                 }
             });
            //foreach (var Point in ReadData.Points)
            //{
            //    for (int i = 0; i < Point.Values.Length; i++)
            //    {
            //        if (Point.CountNow[i] < Point.MaxVehicleNum)
            //        {
            //            Point.Values[i] = 0;
            //        }
            //        else
            //        {
            //            Point.Values[i] = Point.Beta * (Point.CountNow[i] - Point.MaxVehicleNum);
            //        }
            //    }
            //}
            ReadData.Vehicles.ForEach((Vehicle) => { Vehicle.Adj.UpdateLinkValue(); });
            //foreach (var Vehicle in ReadData.Vehicles)
            //{
            //    Vehicle.Adj.UpdateLinkValue();
            //}
        }
        public static void Start()
        {
            Init();
            while (Iteration < 100)
            {
                foreach (var Point in ReadData.Points)
                {
                    Point.Count = new int[Point.CountNow.Length];
                }
                Parallel.ForEach(ReadData.Vehicles, (Vehicle, ParallelLoopState) =>
                {
                    Vehicle.Adj.ShortestPath();
                });
                //foreach (var Vehicle in ReadData.Vehicles)
                //{
                //    Vehicle.Adj.ShortestPath();
                //}
                Alpha = 1.0 / Iteration++;
                bool IsHugeGap = false;
                Parallel.ForEach(ReadData.Points, (Point, ParallelLoopState) =>
                 {
                     for (int i = 0; i < Point.Count.Length; i++)
                     {
                         double PreCount = Point.CountNow[i];
                         Point.CountNow[i] = (1 - Alpha) * PreCount + Alpha * Point.Count[i];
                         if (Math.Abs((PreCount - Point.CountNow[i]) / Point.CountNow[i]) > 0.05)
                         {
                             IsHugeGap = true;
                         }
                     }
                 });
                //foreach (var Point in ReadData.Points)
                //{
                //    for (int i = 0; i < Point.Count.Length; i++)
                //    {
                //        double PreCount = Point.CountNow[i];
                //        Point.CountNow[i] = (1 - Alpha) * PreCount + Alpha * Point.Count[i];
                //        if (Math.Abs((PreCount - Point.CountNow[i]) / Point.CountNow[i]) > 0.05)
                //        {
                //            IsHugeGap = true;
                //        }
                //    }
                //}
                if (!IsHugeGap)
                {
                    break;
                }
                UpdateLinkValue();
            }            
            OutPut();
            Console.WriteLine("配流完成，输出结果见程序目录下 Output.csv 文件中！");
            Console.ReadLine();
        }
        public static void OutPut()
        {
            FileStream fs = new FileStream(Environment.CurrentDirectory + "\\Output.csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.WriteLine("配送中心或需求点ID,时间,对接车辆数");
            ReadData.Points.ForEach((P)=>
            {
                for (int i = 0; i < P.CountNow.Length; i++)
                {
                    var C = P.CountNow[i];
                    int NowTime = i * ReadData.TimeLen + P.EarliestTime;
                    if (Math.Round(C)>0)
                    {
                        sw.WriteLine(P.ID + "," + GetHHMMSS(NowTime) + "," + Math.Round(C));
                    }
                }
            });
            sw.Close();fs.Close();
        }
        private static string GetHHMMSS(int Time)
        {
            int hour = Time / 3600;
            int min = (Time - 3600 * hour) / 60;
            int sec = Time - hour * 3600 - min * 60;
            return hour + ":" + min + ":" + sec;
        }
    }

}
