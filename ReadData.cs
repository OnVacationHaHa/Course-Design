using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace 结课作业
{
    static class ReadData
    {
        public static int TimeLen = 60;
        public static int[,] RunningTime;
        public static List<Vehicle> Vehicles;
        public static List<Point> Points;
        public static void Read()
        {
            Points = new List<Point>();
            FileStream fs = new FileStream(Environment.CurrentDirectory + "\\Points.csv",FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string line = sr.ReadLine();
            while ((line=sr.ReadLine())!=null)
            {
                string[] cells=line.Split(',');
                int ID = int.Parse(cells[0]);
                int MaxV = int.Parse(cells[1]);
                Point NewPoi = new Point(ID, MaxV);
                Points.Add(NewPoi);
            }
            Vehicles = new List<Vehicle>();
            fs = new FileStream(Environment.CurrentDirectory + "\\Vehicles.csv", FileMode.Open);
            sr = new StreamReader(fs);
            line = sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                string[] cells = line.Split(',');
                int ID = int.Parse(cells[0]);
                string[] Paths = cells[1].Split(';');
                List<Point> Path = new List<Point>();
                foreach (var P in Paths)
                {
                    int _ID = int.Parse(P);
                    Path.Add(Points[_ID]);
                }
                int EarDepTime = (int)(double.Parse(cells[2]) * 3600);
                Dictionary<Point, int> OpeTime = new Dictionary<Point, int>();
                int _OpeTime = (int)(double.Parse(cells[3]) * 3600);
                Points.ForEach((p) => { OpeTime.Add(p, _OpeTime); });
                int Alpha = int.Parse(cells[4]);
                Vehicles.Add(new Vehicle(ID, Path, Alpha, EarDepTime, OpeTime));
            }
            sr.Close();fs.Close();
            RunningTime=new int[Points.Count,Points.Count];
            fs = new FileStream(Environment.CurrentDirectory + "\\RunningTime.csv", FileMode.Open);
            sr = new StreamReader(fs);
            int ii = 0;
            while ((line=sr.ReadLine())!=null)
            {               
                string[] cells = line.Split(',');
                for (int i = 0; i < cells.Length; i++)
                {                   
                    RunningTime[ii, i] = (int)(double.Parse(cells[i]) * 60);
                }
                ii++;
            }
            sr.Close();fs.Close();
        }
    }
}
