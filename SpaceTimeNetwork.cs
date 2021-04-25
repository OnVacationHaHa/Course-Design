using System.Collections.Generic;
using System.Linq;

namespace 结课作业
{
    class SpaceTimeNetwork
    {
        private Vehicle V;
        HashSet<Node> Nodes = new HashSet<Node>();
        List<Link> Links = new List<Link>();
        public SpaceTimeNetwork(Vehicle V)
        { this.V = V;
            CreatNetwork();
        }
        public void CreatNetwork()
        {            
            Node LogicStart = new Node(-1, -1,-1);
            Nodes.Add(LogicStart);
            Point NowPoint = V.Path[0];
            int DefaultStartTime = V.EarDepTime;
            V.EarliestTime.Add(NowPoint, DefaultStartTime);
            HashSet<Node> FromNodes = new HashSet<Node>();
            HashSet<Node> NextNodes = new HashSet<Node>();
            for (int i = 0; i < V.MaxDelayTime; i+=ReadData.TimeLen)
            {
                Node NewNode = new Node(NowPoint.ID, DefaultStartTime+i, 2);
                Nodes.Add(NewNode);
                Links.Add(new Link(LogicStart, NewNode, i));
                FromNodes.Add(NewNode);
            }
            V.LatestTime.Add(NowPoint, DefaultStartTime + V.MaxDelayTime);
            for (int i = 1; i < V.Path.Count-1; i++)
            {      
                Point NextPoint = V.Path[i];
                LinkBetweenPoints(FromNodes, NowPoint, NextPoint, ref NextNodes);
                NowPoint = V.Path[i];
                LinkInPoint(NextNodes, NowPoint, ref FromNodes);
            }
            LinkBetweenPoints(FromNodes, NowPoint, V.Path.Last(), ref NextNodes);
            Node LogicEndNode = new Node(-2,-2,-2);
            int Ear = int.MaxValue; int Lat = 0;
            foreach (var Node in NextNodes)
            {
                Nodes.Add(Node);
                Links.Add(new Link(Node, LogicEndNode, 0));
                if (Node.Time>Lat)
                {
                    Lat = Node.Time;
                }
                if (Node.Time<Ear)
                {
                    Ear = Node.Time;
                }
            }            
            V.LatestTime[V.Path.Last()]=Lat;
            Nodes.Add(LogicEndNode);
            AddId(out List<Node2> NewNodes, out List<Link2> Newlinks);
            new AdjacencyList(NewNodes, Newlinks, V);
        }
        private void LinkBetweenPoints(HashSet<Node> FromNodes,Point FromPoint,Point NextPoint,ref HashSet<Node> NextNodes)
        {
            NextNodes = new HashSet<Node>();
            int RunningTime = ReadData.RunningTime[FromPoint.ID, NextPoint.ID];
            foreach (var FromNode in FromNodes)
            {
                Node NextNode = new Node(NextPoint.ID, FromNode.Time + RunningTime, 1);
                Nodes.Add(NextNode);NextNodes.Add(NextNode);
                Links.Add(new Link(FromNode, NextNode, RunningTime));
            }
        }
        private void LinkInPoint(HashSet<Node> FromNodes, Point NowPoint, ref HashSet<Node> NextNodes)
        {
            NextNodes = new HashSet<Node>();
            int MinOperationTime = V.MinOperationTime[NowPoint];
            int MaxOperationTime = MinOperationTime + 1200;
            int Ear = int.MaxValue;int Lat = 0;
            foreach (var FromNode in FromNodes)
            {
                for (int i = MinOperationTime; i < MaxOperationTime; i += ReadData.TimeLen)
                {
                    Node NewNode = new Node(NowPoint.ID, FromNode.Time + i, 2);
                    if (FromNode.Time + i> Lat)
                    {
                        Lat = FromNode.Time + i;
                    }
                    if (FromNode.Time  < Ear)
                    {
                        Ear = FromNode.Time;
                    }
                    Nodes.Add(NewNode);NextNodes.Add(NewNode);
                    Links.Add(new Link(FromNode, NewNode, i));
                }
            }
            V.EarliestTime.Add(NowPoint, Ear);
            V.LatestTime.Add(NowPoint, Lat);
        }
        public struct Node
        {
            public int PointID;
            public int Time;
            public int Flag;
            public Node(int PointID, int Time, int Flag)
            {
                this.PointID = PointID;
                this.Time = Time;
                this.Flag = Flag;
            }
        }
        public class Link
        {
            public Node FromNode;
            public Node NextNode;
            public double Value;
            public Link(Node FromNode,Node NextNode,double Value)
            {
                this.FromNode = FromNode;
                this.NextNode = NextNode;
                this.Value = Value;
            }
        }
        public class Node2
        {
            public int ID;
            public int Time;
            public int PointID;
            public int Flag;
            public int outDegree;
            public Node2(int ID, int Time, int PointID, int Flag)
            {
                this.ID = ID;
                this.Time = Time;
                this.PointID = PointID;
                this.Flag = Flag;
            }
        }
        public class Link2
        {
            public Node2 FromNode;
            public Node2 ToNode;
            public double Value;
            public Link2(Node2 FromNode, Node2 ToNode, double Value)
            {
                this.FromNode = FromNode;
                this.ToNode = ToNode;
                this.Value = Value;
            }
        }
        private void AddId(out List<Node2> NewNodes, out List<Link2> Newlinks)
        {
            NewNodes = new List<Node2>();
            Newlinks = new List<Link2>();
            Dictionary<Node, int> recordId = new Dictionary<Node, int>();
            int NodeId = 0;
            foreach (var Node in Nodes)
            {
                Node2 Newnode = new Node2(NodeId, Node.Time, Node.PointID, Node.Flag);
                recordId.Add(Node, NodeId++);
                NewNodes.Add(Newnode);
            }
            foreach (var link in Links)
            {
                int fromNodeId = recordId[link.FromNode];
                int toNodeId = recordId[link.NextNode];
                Link2 Newlink = new Link2(NewNodes[fromNodeId], NewNodes[toNodeId], link.Value);
                NewNodes[fromNodeId].outDegree += 1;
                Newlinks.Add(Newlink);
            }
        }
    }
}
