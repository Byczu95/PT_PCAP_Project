using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphX.PCL.Common.Enums;
using GraphX.PCL.Logic.Algorithms.LayoutAlgorithms;
using GraphX.Controls;
using PT_MAPACKET.Models;
using QuickGraph;

namespace PT_MAPACKET
{
    /// <summary>
    /// Interaction logic for GraphTabControl.xaml
    /// </summary>
    public partial class GraphTabControl : UserControl
    {
        private static List<MyPacket> packets;
        private static List<NetworkInterface> interafaces;

        private Random Rand;
        private GraphPcap graph;

        public GraphTabControl(List<MyPacket> data)
        {
            InitializeComponent();
            packets = data;
            FindAllNetworkInterfaces();
            FindAllNetworkConnections();
            graph = new GraphPcap();
            Rand = new Random();
            GraphAreaPcap_Setup();
            randomGraph();
            RelayoutGraph();
        }

        private void FindAllNetworkInterfaces()
        {
            interafaces = new List<NetworkInterface>();
            foreach(MyPacket p in packets)
            {
                if (IsInterfaceUnique(p.SourceMac))
                {
                    int newIndex = interafaces.Count;
                    interafaces.Add(new NetworkInterface(p.SourceMac, newIndex));
                    interafaces[newIndex].AddNewIPAdress(p.SourceMac, p.SourceIP);
                }
                else
                {
                    int index = FindNetworkInteface(p.SourceMac);
                    if (interafaces[index].IPs.Contains(p.SourceIP)) { }
                    else { interafaces[index].IPs.Add(p.SourceIP); }
                }

                if (IsInterfaceUnique(p.DestMac))
                {
                    int newIndex = interafaces.Count;
                    interafaces.Add(new NetworkInterface(p.DestMac, newIndex));
                    interafaces[newIndex].AddNewIPAdress(p.DestMac, p.DestIP);
                }
                else
                {
                    int index = FindNetworkInteface(p.DestMac);
                    if (interafaces[index].IPs.Contains(p.DestIP)) { }
                    else { interafaces[index].IPs.Add(p.DestIP); }
                }
            }
        }

        private int FindNetworkInteface(string mac)
        {
            foreach(NetworkInterface n in interafaces)
            {
                if (n.MAC == mac)
                {
                    return n.pos;
                }
            }
            return -1;
        }

        private bool IsInterfaceUnique(string mac)
        {
            foreach(NetworkInterface n in interafaces)
            {
                if (n.MAC == mac)
                {
                    return false;
                }
            }
            return true;
        }

        private void FindAllNetworkConnections()
        {
            foreach(MyPacket p in packets)
            {
                int source = FindNetworkInteface(p.SourceMac);
                if(!(interafaces[source].connections.Contains(p.DestIP))) interafaces[source].connections.Add(p.DestIP);
            }
        }

        private bool IsConnectionUnique(string sourceIP,string destIP)
        {
            foreach(NetworkInterface ni in interafaces)
            {
                return false;
            }
            return true;
        }

        private void RelayoutGraph()
        {
            Area.RelayoutGraph();
            zoomctrl.ZoomToFill();
        }

        private void randomGraph()
        {
            //Lets generate configured graph using pre-created data graph assigned to LogicCore object.
            //Optionaly we set first method param to True (True by default) so this method will automatically generate edges
            //  If you want to increase performance in cases where edges don't need to be drawn at first you can set it to False.
            //  You can also handle edge generation by calling manually Area.GenerateAllEdges() method.
            //Optionaly we set second param to True (True by default) so this method will automaticaly checks and assigns missing unique data ids
            //for edges and vertices in _dataGraph.
            //Note! Area.Graph property will be replaced by supplied _dataGraph object (if any).
            Area.GenerateGraph(true, true);

            /* 
             * After graph generation is finished you can apply some additional settings for newly created visual vertex and edge controls
             * (VertexControl and EdgeControl classes).
             * 
             */

            //This method sets the dash style for edges. It is applied to all edges in Area.EdgesList. You can also set dash property for
            //each edge individually using EdgeControl.DashStyle property.
            //For ex.: Area.EdgesList[0].DashStyle = GraphX.EdgeDashStyle.Dash;
            Area.SetEdgesDashStyle(EdgeDashStyle.Dash);

            //This method sets edges arrows visibility. It is also applied to all edges in Area.EdgesList. You can also set property for
            //each edge individually using property, for ex: Area.EdgesList[0].ShowArrows = true;
            Area.ShowAllEdgesArrows(true);

            //This method sets edges labels visibility. It is also applied to all edges in Area.EdgesList. You can also set property for
            //each edge individually using property, for ex: Area.EdgesList[0].ShowLabel = true;
            Area.ShowAllEdgesLabels(true);
        }

        private void GraphAreaPcap_Setup()
        {
            //Lets create logic core and filled data graph with edges and vertices
            var logicCore = new GXLogicCoreExample() { Graph = GraphPcap_Setup() };
            //This property sets layout algorithm that will be used to calculate vertices positions
            //Different algorithms uses different values and some of them uses edge Weight property.
            logicCore.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.KK;
            //Now we can set parameters for selected algorithm using AlgorithmFactory property. This property provides methods for
            //creating all available algorithms and algo parameters.
            logicCore.DefaultLayoutAlgorithmParams = logicCore.AlgorithmFactory.CreateLayoutParameters(LayoutAlgorithmTypeEnum.KK);
            //Unfortunately to change algo parameters you need to specify params type which is different for every algorithm.
            ((KKLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).MaxIterations = 100;

            //This property sets vertex overlap removal algorithm.
            //Such algorithms help to arrange vertices in the layout so no one overlaps each other.
            logicCore.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            //Default parameters are created automaticaly when new default algorithm is set and previous params were NULL
            logicCore.DefaultOverlapRemovalAlgorithmParams.HorizontalGap = 50;
            logicCore.DefaultOverlapRemovalAlgorithmParams.VerticalGap = 50;

            //This property sets edge routing algorithm that is used to build route paths according to algorithm logic.
            //For ex., SimpleER algorithm will try to set edge paths around vertices so no edge will intersect any vertex.
            //Bundling algorithm will try to tie different edges that follows same direction to a single channel making complex graphs more appealing.
            logicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;

            //This property sets async algorithms computation so methods like: Area.RelayoutGraph() and Area.GenerateGraph()
            //will run async with the UI thread. Completion of the specified methods can be catched by corresponding events:
            //Area.RelayoutFinished and Area.GenerateGraphFinished.
            logicCore.AsyncAlgorithmCompute = false;

            //Finally assign logic core to GraphArea object
            Area.LogicCore = logicCore;
        }

        private BidirectionalGraph<DataVertex, DataEdge> GraphPcap_Setup()
        {
            //Lets make new data graph instance
            var dataGraph = new GraphPcap();
            //Now we need to create edges and vertices to fill data graph
            //This edges and vertices will represent graph structure and connections
            //Lets make some vertices
            foreach(NetworkInterface n in interafaces)
            {
                //Create new vertex with specified Text. Also we will assign custom unique ID.
                //This ID is needed for several features such as serialization and edge routing algorithms.
                //If you don't need any custom IDs and you are using automatic Area.GenerateGraph() method then you can skip ID assignment
                //because specified method automaticaly assigns missing data ids (this behavior is controlled by method param).
                var dataVertex = new DataVertex("MAC : " + n.MAC);
                //Add vertex to data graph
                dataGraph.AddVertex(dataVertex);
            }

            //Now lets make some edges that will connect our vertices
            //get the indexed list of graph vertices we have already added
            var vlist = dataGraph.Vertices.ToList();
            //Then create two edges optionaly defining Text property to show who are connected
            foreach(NetworkInterface n in interafaces)
            {
                foreach(string connection in n.connections)
                {
                    var dataEdge = new DataEdge(vlist[n.pos], vlist[FindNetworkInteface(FindMacByIP(connection))]) { Text = string.Format("{0} - {1}",n.MAC, FindMacByIP(connection)) };
                    dataGraph.AddEdge(dataEdge);
                }
            }
            //var dataEdge = new DataEdge(vlist[0], vlist[1]) { Text = string.Format("{0} -> {1}", vlist[0], vlist[1]) };
            //dataGraph.AddEdge(dataEdge);
            //dataEdge = new DataEdge(vlist[2], vlist[3]) { Text = string.Format("{0} -> {1}", vlist[2], vlist[3]) };
            //dataGraph.AddEdge(dataEdge);

            return dataGraph;
        }

        public string FindMacByIP(string IP)
        {
            foreach(NetworkInterface i in interafaces)
            {
                foreach(string adress in i.connections)
                {
                    if (IP == adress) return i.MAC;
                }
            }
            return "";
        }

        public void Dispose()
        {
            //If you plan dynamicaly create and destroy GraphArea it is wise to use Dispose() method
            //that ensures that all potential memory-holding objects will be released.
            Area.Dispose();
        }
    }

    
}
