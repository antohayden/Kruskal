//Student No: C11733511
//Name: Anthony Hayden


// Kruskal's Minimum Spanning Tree Algorithm
// Union-find implemented using disjoint set trees

using System;
using System.IO;

// convert vertex into char for pretty printing

class Edge
{

    public int wgt; //edge weight
    public int u, v;    //connecting vertices

    public Edge(int u, int v, int wgt)
    {
        this.u = u;
        this.v = v;
        this.wgt = wgt;
    }

    public void show()
    {
        Console.Write("Edge {0}-{1}-{2}\n", toChar(u), wgt, toChar(v));
    }

    // convert vertex into char for pretty printing
    private char toChar(int u)
    {
        return (char)(u + 64);
    }
}


class Heap
{
    public int[] h;	//heap array
    int N, Nmax;		//heap size, max size
    Edge[] edge;		//array of edges


    // Bottom up heap constructor
    public Heap(int _N, Edge[] _edge)
    {
        int i;
        Nmax = N = _N;
        h = new int[N + 1];
        edge = _edge;

        // initially just fill heap array with 
        // indices of edge[] array.
        for (i = 0; i <= N; ++i)
            h[i] = i;

        // Then convert h[] into a heap
        // from the bottom up.
        for (i = N /2; i > 0; i--)
            siftDown(i);
    }


    private void siftDown(int k)
    {
        while (k <= N/2)
        {
            int leftchild = k * 2;
            int rightchild = (k * 2) + 1;
            int smallerchild;

            //determine smaller child
            if ((rightchild < (N)) && (edge[h[leftchild]].wgt > edge[h[rightchild]].wgt))
            {
                smallerchild = rightchild;
            }
            else
            {
                smallerchild = leftchild;
            }

            //the indices in the heap represent indices to edge array. The top of 
            //the heap will be the indice of the edge with the lowest weight.
            //comparing parent with smallest we swap if greater

            if (edge[h[k]].wgt >= edge[h[smallerchild]].wgt)
            {   //swap indices
                int temp = h[smallerchild];
                h[smallerchild] = h[k];
                h[k] = temp;
            }
            k = smallerchild;
        }
        
    }


    public int remove()
    {
        //move top indice to unused element and replace top with
        //last indice in heap. Sift down and return removed indice
        h[0] = h[1];
        h[1] = h[N--];
        siftDown(1);
        return h[0];
    }
}

/****************************************************
*
*       UnionFind partition to support union-find operations
*       Implemented using Discrete Set Trees
*
*****************************************************/

class UnionFindSets
{
    //array to store parent indices
    private int[] treeParent;
    private int N;

    public UnionFindSets(int V)
    {
        treeParent = new int[V + 1];

        //initially each set is a parent unto itself
        for (int i = 1; i <= V; i++)
            treeParent[i] = i;

        N = V;
    }

    public int findSet(int vertex)
    {

        //continue to search through parents until root, 
        //which would be itself
        while ( vertex != treeParent[vertex] )
        {
            vertex = treeParent[vertex];
        }

        return vertex;
    }

    public void union(int set1, int set2)
    {
       //finding the parent of set1, we find it's root using findSet
       //then we assign it to what was the root of set 2
       treeParent[findSet(set2)] = findSet(treeParent[set1]);
    }

    public void showTrees()
    {
        int i;
        for (i = 1; i <= N; ++i)
            Console.Write("{0}->{1}  ", toChar(i), toChar(treeParent[i]));
        Console.Write("\n");
    }

    public void showSets()
    {
        int i, j;
        bool newSet = true; //added for better printing to screen
        int setCount = 1;   //same as above

        for (i = 1; i <= N; i++)
        {
            for (j = 1; j <= N; j++)
            {
                if (findSet(j) == i)
                {
                    if (newSet)
                    {
                        Console.Write("\n");
                        Console.Write("Set {0}: ",setCount);
                        setCount++;
                    }
                    
                    Console.Write("{0}, ", toChar(j));
                    newSet = false;
                }
            }
            newSet = true;
        }
    }

    private void showSet(int root)
    {
        Console.Write("Set " + toChar(root) + ":");

        while (root != treeParent[root])
        {
            Console.Write("{0} , ", toChar(treeParent[root]));
            root = treeParent[root];
        }
    }

    private char toChar(int u)
    {
        return (char)(u + 64);
    }
}

class Graph
{
    private int V, E;
    private Edge[] edge;
    private Edge[] mst;

    public Graph(string graphFile)
    {
        int u, v;
        int w, e;

        StreamReader reader = new StreamReader(graphFile);

        char[] splits = new char[] { ' ', ',', '\t' };
        string line = reader.ReadLine();
        string[] parts = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);

        // find out number of vertices and edges
        V = int.Parse(parts[0]);
        E = int.Parse(parts[1]);


        // create edge array 
        edge = new Edge[E + 1];

        // read the edges
        Console.WriteLine("Reading edges from text file");
        for (e = 1; e <= E; ++e)
        {
            line = reader.ReadLine();
            parts = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);
            u = int.Parse(parts[0]);
            v = int.Parse(parts[1]);
            w = int.Parse(parts[2]);

            Console.WriteLine("Edge {0}--({1})--{2}", toChar(u), w, toChar(v));

            //add edge to array
            edge[e] = new Edge(u, v, w);
        }
        Console.Write("\n");

    }


    /**********************************************************
    *
    *       Kruskal's minimum spanning tree algorithm
    *
    **********************************************************/
    public Edge[] MST_Kruskal()
    {
        int ei, i = 0;
        int uSet, vSet;
        UnionFindSets partition;
        Heap heap;

        // create edge array to store MST
        // Initially it has no edges.
        mst = new Edge[V - 1];

        // priority queue for indices of array of edges
        heap = new Heap(E, edge);

        // create partition of singleton sets for the vertices
        partition = new UnionFindSets(V);

        //show initial singleton sets
        partition.showSets();
        Console.Write("\n");


        while (i < V - 1)
        {
            //Assign vertices from edge at the top of the heap ( lowest weight )
            uSet = edge[heap.h[1]].u; 
            vSet = edge[heap.h[1]].v;

            //If they don't belong to the same set ( have the same root ), then union
            if (partition.findSet(uSet) != partition.findSet(vSet))
            {
                partition.union(uSet, vSet);
                //remove the indice for the edge from the heap
                ei = heap.remove();
                //add removed edge to minimum spanning tree
                mst[i] = edge[ei];
                //print sets
                partition.showSets();
                Console.Write("\n");

                i++;
            }

            //if they do belong to the same set, then just remove from heap
            else
            {
                heap.remove();
            }
            
        }

        return mst;
    }


    // convert vertex into char for pretty printing
    private char toChar(int u)
    {
        return (char)(u + 64);
    }

    public void showMST()
    {
        Console.Write("\nMinimum spanning tree build from following edges:\n");
        for (int e = 0; e < V - 1; ++e)
        {
            mst[e].show();
        }
        Console.WriteLine();

    }

    // test code
    public static int Main()
    {
        string fname = "wGraph3.txt";
        Console.Write("\nInput name of file with graph definition: ");
        fname = Console.ReadLine();

        Graph g = new Graph(fname);

        g.MST_Kruskal();

        g.showMST();

        return 0;
    }

} // end of Graph class