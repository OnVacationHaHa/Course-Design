using System.Collections.Generic;

namespace 结课作业
{
    class AdjacencyList
    {
        private Vehicle V;
        public Vertex[] Vertexs;
        public Vertex[] TopSortVertexs;
        public double NowPathValue;
        public List<int> NowPath;
        public AdjacencyList(List<SpaceTimeNetwork.Node2> Nodes, List<SpaceTimeNetwork.Link2> Links, Vehicle V)
        {
            this.V = V;
            V.Adj = this;
            Vertexs = new Vertex[Nodes.Count];
            ReadNodes(Nodes);
            ReadLink(Links);
            TopSortVertexs = new Vertex[Nodes.Count];
            TopologicalSort();
        }
        private void ReadNodes(List<SpaceTimeNetwork.Node2> Nodes)
        {
            foreach (var node in Nodes)
            {
                if (node.PointID < 0)
                {
                    Vertex v = new Vertex(node.ID, null, node.Time, node.Flag);
                    Vertexs[node.ID] = v;
                }
                else
                {
                    Vertex v = new Vertex(node.ID, ReadData.Points[node.PointID], node.Time, node.Flag);
                    Vertexs[node.ID] = v;
                }
            }
        }
        private void ReadLink(List<SpaceTimeNetwork.Link2> Links)
        {
            foreach (SpaceTimeNetwork.Link2 link in Links)
            {
                Vertex FromVer = Vertexs[link.FromNode.ID];
                Vertex ToVer = Vertexs[link.ToNode.ID];
                AddDirectedEdge(FromVer, ToVer, link.Value);
                Vertexs[ToVer.ID].InDegree++;
            }
        }
        private void AddDirectedEdge(Vertex FromVer, Vertex ToVer, double Value)
        {

            if (FromVer.FirstEdge == null)
            {
                FromVer.FirstEdge = new Link(Value, ToVer);
            }
            else
            {
                Link tmp, node = FromVer.FirstEdge;
                do
                {
                    tmp = node;
                    node = node.Next;
                } while (node != null);
                tmp.Next = new Link(Value, ToVer);
            }
        }
        private List<int> ShortestPath(int StartNodeId, int EndNodeId, out double pathValue)
        {
            double[] d = new double[Vertexs.Length];
            int[] pre = new int[Vertexs.Length];
            Stack<Vertex> s = new Stack<Vertex>();
            for (int i = TopSortVertexs.Length - 1; i >= 0; i--)
            {
                s.Push(TopSortVertexs[i]);
            }
            for (int i = 0; i < Vertexs.Length; i++)
            {
                d[i] = double.PositiveInfinity;
            }
            d[StartNodeId] = 0;
            while (s.Count != 0)
            {
                Vertex u = s.Pop();
                if (u.FirstEdge != null)
                {
                    Link tmp, node = u.FirstEdge;
                    do
                    {
                        tmp = node;
                        node = node.Next;
                        if (d[tmp.AdjVer.ID] > d[u.ID] + tmp.Value)
                        {
                            d[tmp.AdjVer.ID] = d[u.ID] + tmp.Value;
                            pre[tmp.AdjVer.ID] = u.ID;
                        }
                    } while (node != null);
                }
            }
            List<int> path = new List<int>();
            if (d[EndNodeId] != double.PositiveInfinity)
            {
                path.Add(EndNodeId);
                int nowNode = EndNodeId;
                while (nowNode != StartNodeId)
                {
                    int preNode = pre[nowNode];
                    path.Add(preNode);
                    nowNode = preNode;
                }
            }
            path.Reverse();
            pathValue = d[EndNodeId];
            return path;
        }
        //List<int> Pre=new List<int>();       
        public void ShortestPath()
        {
            double pathValue;
            List<int> NowPath = ShortestPath(Vertexs[0].ID, Vertexs[Vertexs.Length - 1].ID, out pathValue);
            Vertex Start = Vertexs[NowPath[1]];
            Start.Point.Count[Start.Point.TimeIndex(Start.Time)] += 1;
            for (int i = 2; i < NowPath.Count - 2; i += 2)
            {
                Vertex ArrNode = Vertexs[NowPath[i]];
                Vertex DepNode = Vertexs[NowPath[i + 1]];
                Point NowPoint = ArrNode.Point;
                lock (NowPoint.Count)
                {
                    for (int ii = ArrNode.Time; ii <= DepNode.Time; ii += ReadData.TimeLen)
                    {
                        NowPoint.Count[NowPoint.TimeIndex(ii)] += 1;
                    }
                }
            }
            Vertex End = Vertexs[NowPath[NowPath.Count - 2]];
            End.Point.Count[End.Point.TimeIndex(End.Time)] += 1;
            NowPathValue = pathValue;
            this.NowPath = NowPath;
        }
        private void TopologicalSort()
        {
            Stack<Vertex> s = new Stack<Vertex>();
            foreach (var vertex in Vertexs)
            {
                if (vertex.InDegree == 0)
                {
                    s.Push(vertex);
                }
            }
            int num = 0;
            while (s.Count != 0)
            {
                Vertex v = s.Pop();
                TopSortVertexs[num++] = v;
                if (v.FirstEdge != null)
                {
                    Link tmp, node = v.FirstEdge;
                    do
                    {
                        tmp = node;
                        node = node.Next;
                        tmp.AdjVer.InDegree -= 1;
                        if (tmp.AdjVer.InDegree == 0)
                        {
                            s.Push(tmp.AdjVer);
                        }
                    } while (node != null);
                }
            }
        }
        public void UpdateLinkValue()
        {
            foreach (var Ver in Vertexs)
            {                
                Link temp, node = Ver.FirstEdge;
                while (node != null)
                {
                    temp = node;
                    node = node.Next;
                    UpdateOneLink(temp);
                }
            }
        }
        private void UpdateOneLink(Link L)
        {
            Point NowPoint = L.AdjVer.Point;
            if (NowPoint is null)
            {
                return;
            }
            L.Value = L.Value0 + this.V.Alpha*NowPoint.Values[NowPoint.TimeIndex(L.AdjVer.Time)];

        }
        public class Vertex
        {
            public int ID;
            public int Time;
            public Point Point;
            public Link FirstEdge;
            public int InDegree;
            public int Flag;
            public Vertex(int ID, Point Point, int Time, int Flag)
            {
                this.Point = Point;
                this.ID = ID;
                this.Time = Time;
                this.Flag = Flag;
            }

        }
        public class Link
        {
            public double Value0;
            public double Value;
            public Vertex AdjVer;
            public Link Next;
            public Link(double Value, Vertex AdjVer)
            {
                Value0 = Value;
                this.Value = Value;
                this.AdjVer = AdjVer;
            }
        }
    }
}
