using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using BeeGraph.Core;
using BeeGraph.Core.Domain;
using BeeGraph.Data;
using BeeGraph.IoC;

namespace ReportDrawer
{
    class Program
    {
        static void Main(string[] args)
        {
            var nodeProvider = IoC.Container.GetInstance<INodeProvider>();
            var edgeProvider = IoC.Container.GetInstance<INodeRelationRepository>();

            var bitmap = Draw(nodeProvider.GetAll());
            bitmap.Save("test.png");
        }

        public static Bitmap Draw(IEnumerable<Node> nodes)
        {
            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("MyGraph");

            foreach (var node in nodes)
            {
                graph.AddNode(node.Body );

                foreach (var edge in node.OutEdges)
                {
                    graph.AddEdge(node.Body, edge.Key, edge.To.Body);
                }

            }
            //graph.AddEdge("A", "B");
            //graph.AddEdge("A", "B");
            //graph.AddEdge("B", "C");
            //graph.AddNode("He");
            //graph.AddNode("He");


            //graph.FindNode("A").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;
            //graph.FindNode("B").Attr.FillColor = Microsoft.Msagl.Drawing.Color.Blue;
            Microsoft.Msagl.GraphViewerGdi.GraphRenderer renderer = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
            renderer.CalculateLayout();
            int k = 150;
            int width = 50;
            Bitmap bitmap = new Bitmap(width * k, (int)(graph.Height * (width / graph.Width)) * k, PixelFormat.Format32bppPArgb);
            renderer.Render(bitmap);
            return bitmap;
        }
    }
}
