using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.ComponentModel;
using static System.Net.Mime.MediaTypeNames;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Net.Http.Headers;
using System.IO.Packaging;
using System.Reflection.Metadata.Ecma335;

namespace NEAwpf
{

    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateDraggableRectangle(0, 0, 0);
        }

        private void Canvas_Drop(object sender, DragEventArgs e)
        {

        }
        public List<System.Windows.Controls.Image> rectangles = new List<System.Windows.Controls.Image>();
        


        private void canvas1_DragOver(object sender, DragEventArgs e)
        {
            
        }



        private void btn1_Click(object sender, RoutedEventArgs e)
        {

            CreateDraggableRectangle(12, 12,1);


        }
        
        public static List<LineAttributes> LA_list = new List<LineAttributes>();
        public class LineAttributes
        {
            public UIElement r1;
            public UIElement r2;
            public bool r1_isLeft;
            public bool r2_isLeft;


            public LineAttributes(UIElement rect1, UIElement rect2, bool r1_isLeft, bool r2_isLeft)
            {

                r1 = rect1;
                r2 = rect2;
                this.r1_isLeft = r1_isLeft;
                this.r2_isLeft = r2_isLeft;
            }
        }
        public static int nodecount;
       public class componentProperty
        {
            public int type;
            public int nodeindex;
            public System.Windows.Controls.Image component;
            public List<UIElement> C1;
            public List<UIElement> C2;
            public List<bool> C1polarity;
            public List<bool> C2polarity;
            
            public componentProperty(System.Windows.Controls.Image component, int type)
            {
                this.type = type;
                this.component = component;
                 if(type == 2)
                {
                    nodecount++;
                    nodeindex = nodecount;
                }
                 C1 = new List<UIElement>();
                C1polarity = new List<bool>();
                C2 = new List<UIElement>();
                C2polarity = new List<bool>();
            }
        }
        public List<componentProperty> compProps = new List<componentProperty>();
        public List<UIElement> elmnts = new List<UIElement>();
        private void CreateDraggableRectangle(double x, double y,int type)
        {
            // Create a Rectangle
            Rectangle rectangle1    = new Rectangle();
           // rectangle1.Fill.image
            System.Windows.Controls.Image draggableRectangle = new System.Windows.Controls.Image();
            draggableRectangle.Width = 150;
            draggableRectangle.Height = 100;
           // draggableRectangle.Source = new Uri(@"\Images\Vsource.png");
            //   draggableRectangle.Fill = Brushes.Red;
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            if(type == 0)
            {  
                myBitmapImage.UriSource = new Uri(@"C:\Users\pasca\OneDrive - Notre Dame Catholic Sixth Form College\Computer science year 2\NEA\NEAwpf\NEAwpf\Vsource.png");
                draggableRectangle.Width = 150;
                draggableRectangle.Height = 50;
                 nodelist.Add(new Node(false)); //srcnode
              //create vsoucrce


            }
            else if(type == 1)
            {
                draggableRectangle.Width = 150;
                draggableRectangle.Height = 100;
                myBitmapImage.UriSource = new Uri(@"C:\Users\pasca\OneDrive - Notre Dame Catholic Sixth Form College\Computer science year 2\NEA\NEAwpf\NEAwpf\bin\Debug\net6.0-windows\Images\Symbol-of-Resistor.jpg"); 
              
               
            }
       
            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            //set image source
            draggableRectangle.Source = myBitmapImage;
            //RotateTransform myRotateTransform = new RotateTransform(90);
            //draggedElementR.RenderTransform = myRotateTransform;
   

            // Set up event handlers for dragging
            draggableRectangle.MouseLeftButtonDown += DraggableRectangle_MouseLeftButtonDown;
            draggableRectangle.MouseMove += DraggableRectangle_MouseMove;
            draggableRectangle.MouseLeftButtonUp += DraggableRectangle_MouseLeftButtonUp;

            // Add the Rectangle to the Canvas
            canvas1.Children.Add(draggableRectangle);

            // Set initial position
            Canvas.SetLeft(draggableRectangle, x);
            Canvas.SetTop(draggableRectangle, y);
            compProps.Add(new componentProperty(draggableRectangle, type)); //nodelist[nodelist.Count()-2]));
      
        }
      private void DrawWire(Point P1 , Point P2)
       {
          
            Line WireLine = new Line();
            WireLine.Width = 10;
          WireLine.X1 = P1.X;
           WireLine.Y1 = P1.Y+50;
           WireLine.X2 = P2.X;
            WireLine.Y2 = P2.Y+50;
            WireLine.Stroke = Brushes.Black;
           WireLine.StrokeThickness = 1;
           canvas1.Children.Add(WireLine); 
         

            Lines.Add(WireLine);
        }
     

    public static List<Line> Lines = new List<Line>();

        private bool isDraggingR = false;
        private Point lastMousePositionR;
        private UIElement draggedElementR;
        private UIElement temp;
        private bool isLeft;
        private bool LineCreated;
        private int dragElemType;
        private bool temp_polarity;
       
        

        private void DraggableRectangle_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            // Start dragging
             
            isDraggingR = true; // when clicked isdragging = true
            lastMousePositionR = e.GetPosition(canvas1);
            draggedElementR = (UIElement)sender;
            if (elmnts.Contains(draggedElementR)) { }
            else
            {
                elmnts.Add(draggedElementR);
            }
            System.Windows.Controls.Image img = (System.Windows.Controls.Image)draggedElementR;
            Mouse.Capture(draggedElementR);
            Trace.WriteLine(elmnts.Count());
            Trace.WriteLine(compProps.Count());
            
        }

        private void DraggableRectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingR)
            {
               int xoffset = 0;
               int yoffset= 0;
                if (compProps[elmnts.IndexOf(draggedElementR)].type == 0)
                {
                    xoffset = 50;
                   yoffset = 100;
                }
                else if (compProps[elmnts.IndexOf(draggedElementR)].type == 1)
                {
                    xoffset = 50;
                    yoffset = 100;
                }
            


                    if (e.GetPosition(canvas1).X >= Canvas.GetLeft(draggedElementR) + xoffset && e.GetPosition(canvas1).X <= Canvas.GetLeft(draggedElementR) + yoffset)
                    {
                        // if its in the center draggable sectiom
                        // Move the Rectangle with the mouse
                        Point currentPosition = e.GetPosition(canvas1);// get mouse position

                        double X = currentPosition.X - lastMousePositionR.X;
                        double Y = currentPosition.Y - lastMousePositionR.Y;

                        Canvas.SetLeft(draggedElementR, Canvas.GetLeft(draggedElementR) + X); // sets position of image
                        Canvas.SetTop(draggedElementR, Canvas.GetTop(draggedElementR) + Y);

                        lastMousePositionR = currentPosition;
                        Render();
                    }
                    else
                    {


                        if (temp == null)
                        {
                            temp = draggedElementR; //sets temp to first component
                            Trace.WriteLine("set to temp");
                            if (e.GetPosition(canvas1).X > Canvas.GetLeft(draggedElementR) + 100)// if its right box
                            {
                                isLeft = false;
                            Trace.WriteLine("added first rectangle RIGHT");
                            temp_polarity = true;
                            }
                            else 
                              {
                            isLeft = true; 
                            Trace.WriteLine("added first rectangle LEFT");
                            temp_polarity = false;
                              }// if its left
                            

                        }


                        else if (temp != null && temp != draggedElementR)
                        {
                            bool S = false;


                        if (e.GetPosition(canvas1).X > Canvas.GetLeft(draggedElementR) + yoffset) // checks if its on the right
                        {
                            LA_list.Add(new LineAttributes(draggedElementR, temp, false, isLeft));
                            compProps[elmnts.IndexOf(draggedElementR)].C2.Add(temp);
                            compProps[elmnts.IndexOf(draggedElementR)].C2polarity.Add(temp_polarity);

                            if (temp_polarity == true)
                            {
                                compProps[elmnts.IndexOf(temp)].C2.Add(draggedElementR);
                                compProps[elmnts.IndexOf(temp)].C2polarity.Add(true);

                            }
                               
                            else {
                                compProps[elmnts.IndexOf(temp)].C1.Add(draggedElementR);
                                compProps[elmnts.IndexOf(temp)].C1polarity.Add(true);
                            }


                            Trace.WriteLine("element has  components connected on the left" + compProps[elmnts.IndexOf(draggedElementR)].C2.Count());

                        
                            //   if ((temp == elmnts[0] && isLeft == false ) || (draggedElementR == elmnts[0])   ) // if the component is a source
                          
                                Trace.WriteLine("added second rectangle RIGHT");
                        }
                            else// on the left
                            {
                                LA_list.Add(new LineAttributes(draggedElementR, temp, true, isLeft));
                                Trace.WriteLine("added second rectangle LEFT");
                                compProps[elmnts.IndexOf(draggedElementR)].C1.Add( temp);
                                compProps[elmnts.IndexOf(draggedElementR)].C1polarity.Add(temp_polarity);


                            if (temp_polarity == true)
                            {
                                compProps[elmnts.IndexOf(temp)].C2.Add(draggedElementR);
                                compProps[elmnts.IndexOf(temp)].C2polarity.Add(false);

                            }
                               
                            else { compProps[elmnts.IndexOf(temp)].C1.Add(draggedElementR);
                                compProps[elmnts.IndexOf(temp)].C1polarity.Add(false);
                            }

                            Trace.WriteLine("");
                        }

                        LineCreated = true;
                            temp = null;
                            Trace.WriteLine("LA list " + LA_list.Count());

                            Trace.WriteLine("added to list");


                        }

                        Render();

                    }
             

                
                

            }
        }
        public void Render()
        {
           
            foreach(Line L in Lines)
            {
                canvas1.Children.Remove(L);
            }
            foreach(LineAttributes L in LA_list)
            {
                Point P1 = new Point(Canvas.GetLeft(L.r1), Canvas.GetTop(L.r1));
                Point P2 = new Point(Canvas.GetLeft(L.r2), Canvas.GetTop(L.r2));
                if (L.r1_isLeft == false)
                {
                    if (compProps[elmnts.IndexOf(L.r1)].type == 0)
                    {
                        P1.X += 100;
                        P1.Y -= 25;
                       
                    }
                    else
                    {
                        P1.X += 150;
                    }
             
                    // Trace.WriteLine("Rendered right1");
                }
                else
                {

                    if (compProps[elmnts.IndexOf(L.r1)].type == 0)
                    {
                       
                        P1.Y -= 25;
                        P1.X += 30;
                    }
                }
                if (L.r2_isLeft == false)
                {
                    if (compProps[elmnts.IndexOf(L.r2)].type == 0)
                    {
                       
                        P2.Y -= 25;
                        P2.X += 150;
                    }
                    else
                    {
                        P2.X += 150;
                    }
                 
                   //Trace.WriteLine("rendered left1");
                }
                else
                {
                    if (compProps[elmnts.IndexOf(L.r2)].type == 0)
                    {

                        P2.Y -= 25;
                        P2.X += 30;
                    }
                }

                DrawWire(P1,P2);
            }
        }

        private void DraggableRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Stop dragging
            isDraggingR = false;
            Mouse.Capture(null);
            if(LineCreated == true) { temp = null;  LineCreated = false; }

        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LA_list.Remove(LA_list.Last());
           

                temp = null; 
                Render();
            }
            catch { }
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            canvas1.Children.Remove(rectangles.Last());
            rectangles.Remove(rectangles.Last());   
        }



        //nodelist.Add(new Node(false)); //srcnode
            
        //    nodelist.Add(new Node(false));
        //    nodelist.Add(new Node(true));//rootnode
        //    vsource vsource = new vsource(5, nodelist.Last(), nodelist.First()); //create vsoucrce
        //resistors.Add(new resistor(100, nodelist.First(), nodelist[1]));
         





           // nodesolve();//use nodesolve algorthm 

        public static List<Node> nodelist = new List<Node>(); // nodelist
        public static List<resistor> resistors = new List<resistor>(); //overall resistor list
        static void nodesolve()
        {

            int s = nodelist.Count - 2;
            Console.WriteLine(s);
            Matrix<double> A = Matrix<double>.Build.Dense(s, s);//build matrix of size -> length of the list minus the refnode and srcnode
            Vector<double> B = Vector<double>.Build.Dense(s);//build matching vector
            int matrixY = 0;  //Y coord of matrix array
            int matrixX = 0;  //X coord of matric array


            foreach (Node n in nodelist)  //itterates through the list of nodes
            {
                if (n.v != nodelist.First().v)//excludes srcnode
                {
                    foreach (resistor R in n.resistorsB) //itterates through list of resistors after node
                    {

                        A[matrixX, matrixY] += (1 / R.getR()); //add 1/resistance values for the current node
                        if (R.Getn2() != nodelist.Last()) //exclude the rootnode
                        {
                            Console.WriteLine((R.getR()));
                            A[nodelist.IndexOf(R.Getn2()) - 1, matrixY] += -(1 / R.getR()); //add -1/resitance vals for previous node
                            Console.WriteLine(A + "d");//report data 

                        }
                    }
                    foreach (resistor R in n.resistorsA) //itterates through list of resistors before node
                    {

                        A[matrixX, matrixY] += (1 / (R.getR()));//add 1/resistance values for the current node
                        Console.WriteLine(A + "a");//report data
                        if (R.Getn1() != nodelist.First())//exclude srcnode 
                        {
                            Console.WriteLine((R.getR()));
                            A[nodelist.IndexOf(R.Getn1()) - 1, matrixY] += -(1 / R.getR()); //add -1/resitamce vals for previous nodes
                            Console.WriteLine(A + "c");//report data

                        }

                    }
                    if (matrixX == s - 1) { break; }//end if it reaches the end 
                    matrixX++; //itterate coords
                    matrixY++;
                }
            }

            foreach (resistor R in (nodelist.First().resistorsB)) //itterates through all the resistances after the src
            {

                bool single = true;
                float vcval = 0;
                bool AddR = true;
                Node tempnode = null;

                foreach (resistor N in (nodelist.First().resistorsB)) //loops through all resistors after srcnode
                {
                    if ((R.Getn2() == N.Getn2()) && (N != R)) // compares the resistors to find matches
                    {
                        if (AddR == true) { vcval += (nodelist.First().v / R.getR()); AddR = false; } // go means that it wont add the value of R each time
                        vcval += (nodelist.First().v / N.getR()); // adds the N value
                        single = false; // confirms there is multiplw connected to a node
                        tempnode = N.Getn2(); // assigns the node to a temporary value
                        Console.WriteLine("called" + vcval + "," + R.getR() + N.getR());
                    }
                }
                Console.WriteLine((nodelist.IndexOf(R.Getn2()) - 1));
                if (single == true)
                { //if it is single
                    if ((nodelist.IndexOf(R.Getn2()) - 1) < s) // if the index is possible
                    {
                        B[(nodelist.IndexOf(R.Getn2()) - 1)] = (nodelist.First().v / R.getR());// adds single val to vector
                        Console.WriteLine("single");
                    }
                    else { Console.WriteLine("break"); break; }

                }
                else { B[nodelist.IndexOf(tempnode) - 1] = vcval; Console.WriteLine("assign"); }// uses vcval if there was multiple

            }


            Vector<double> C = A.Solve(B); //solve the matrix
            Console.WriteLine(A);
            Console.WriteLine(B);
            Console.WriteLine(C);//output vector
            for (int i = 0; i < nodelist.Count - 2; i++) { nodelist[i + 1].v = ((float)(C[i])); }//enters voltages into nodes from vector

        }



        public class Node
        {
            public float v;

            public Node(bool Rn) { if (Rn == true) { v = 0; } rootnode = Rn; }//create a node

            public bool rootnode;//is it a rootnode
            public float I;
            public List<resistor> resistorsA = new List<resistor>(); //list of resitors in front of the node
            public List<resistor> resistorsB = new List<resistor>();//list of resistors after the node
            public List<UIElement> Nodecomps = new List<UIElement>();// list of connected nodes

        }



        public class resistor
        {
            private float R;
            private float I;
            private float V;
            private Node n1;
            private Node n2;
            public float getR() { return R; }
            public float getI()
            { I = V / R; return I; }//returns current after solve
            public float getV()
            {
                V = n1.v - n2.v;   //finds P.D only works after solve
                return V;
            }
            public Node Getn1() { return n1; }
            public Node Getn2() { return n2; }
            public resistor(float R, Node n1, Node n2)//create resistor 
            {
                this.R = R;
                n1.resistorsB.Add(this); //add to the first nodes B list (after list)
                n2.resistorsA.Add(this);//add to seconds nodes A list (before list)
                this.n1 = n1;
                this.n2 = n2;

                if (n2.v != 0 && n2.rootnode != true) //decides if root node
                {
                    V = (n1.v) - (n2.v);
                }
                else
                {
                    V = n1.v;
                }

            }

        }
        public class vsource // create source
        {
            float srcv;
            public vsource(float srcv, Node n1, Node n2)
            {
                this.srcv = srcv;
                n2.v = n1.v + srcv;//creates P.D
            }
        }

        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            temp = null;
        }
        public int count;
        public List<List<UIElement>> nodelistslist = new List<List<UIElement>> ();
        public List<List<bool>> polListsList = new List<List<bool>> ();
        public List<UIElement> checklists(UIElement R, bool Rpol)//checks if input is in  a list already if so returns that list
        {
         foreach(List<UIElement> List in nodelistslist) 
            {
                foreach (UIElement U in List)
                {
                    if (R == U && polListsList[nodelistslist.IndexOf(List)][List.IndexOf(U)] == Rpol )//if it does match one found in a list
                    {
                        return List;//return the current list 
                    }

                }
            }
            return null;// if it is never found return nothing
        }
           
            

       
        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            
            nodelistslist.Add(compProps[0].C1);
            polListsList.Add(compProps[0].C1polarity);//set up first lists
            nodelistslist[0].Add(elmnts[0]);
            polListsList[0].Add(false);
            foreach (componentProperty C in compProps)//loop through every component 
            {
                List<UIElement> tempList = new List<UIElement>();// make a temporary list to check for repeats

                tempList = checklists(elmnts[0], false);
             
                if (checklists(elmnts[compProps.IndexOf(C)], false) != null)// check current component negative side
                {
                    tempList = checklists(elmnts[compProps.IndexOf(C)], false); // if it is in  a list already make that the temp list
                    Trace.WriteLine("happen");
                }
              
                int l = C.C1.Count() - 1;
                for(int i = 0; i< l; i++)//loop through all related components on the negative side
                {
                    UIElement U = C.C1[i];
                    if (checklists(U, C.C1polarity[C.C1.IndexOf(U)]) != null && checklists(U, C.C1polarity[C.C1.IndexOf(U)]) != tempList)//check if there is a item from a different list
                    {
                        checklists(U, C.C1polarity[C.C1.IndexOf(U)]).AddRange(tempList);//if so combine the lists
                        nodelistslist.Remove(tempList);
                    }
                }
                if(tempList.Count == 0)
                {
                    nodelistslist.Add(C.C1);
                    polListsList.Add(C.C1polarity);
                    nodelistslist[nodelistslist.Count - 1].Add(elmnts[compProps.IndexOf(C)]);
                    polListsList[polListsList.Count - 1].Add(false);
                }
            }
    
                Trace.WriteLine("check");

            




        }

        private void btn6_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }   
}

