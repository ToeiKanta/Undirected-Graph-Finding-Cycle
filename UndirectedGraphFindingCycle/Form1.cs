using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
namespace UndirectedGraphFindingCycle
{
    public partial class Form1 : Form
    {
        //program version
        private string version = "0.1.0";
        //Create Pen to draw Line Graph
        Pen myPen = new Pen(Color.Red);
        Graphics g = null;

        //instances
        int selectedRowCycles = 0; //from datagridviewcycles
        private int NodeCount = 0; // node count
        List<Point> Lines = new List<Point>();
        private bool drawByHand = false;

        //linked node
        private string[,] linkedNode = new string[100, 2]; //all linked node
        private int totalLinkedNode = 0; // size

        //default
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //set text on comboBox 
            int t = this.NodeCount + 1;
            textBoxNodeName.Text = t.ToString();

            //disable datagrid sortable
            foreach (DataGridViewColumn column in dataGridViewCycles.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }


        }

        //load data to combobox - nodeStart to nodeEnd
        private void loadComboBox()
        {
            this.comboBoxStart.Items.Clear();
            this.comboBoxEnd.Items.Clear();
            var nodes = new object[NodeCount];
            for (int i = 1; i <= NodeCount; i++)
            {
                var nodeText = this.Controls.Find("label" + i, true)[0].Text;
                nodes[i - 1] = nodeText;
            }
            this.comboBoxStart.Items.AddRange(nodes);
            this.comboBoxEnd.Items.AddRange(nodes);
        }

        //get controls from name
        private Control getControlFromName(string name)
        {
            try
            {
                var control = this.Controls.Find(name, true)[0];
                return control;
            } catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }

        // Find Cycles And Circuits From Undirected Graph 

        // เก็บกราฟในรูปแบบเส้นเชื่อม หรือ edges
        private int[,] graph;  
        // เก็บวัฏจักรที่ได้ หรือ cycles (นำมาต่อเป็นวงจรในภายหลัง)
        private List<int[]> cycles = new List<int[]>(); 
        //ฟังก์ชั่น หาวงจร จากกราฟ
        private void loadCycles()
        {
            Console.WriteLine("=================================");
            //วนลูปเพื่อนำเข้าข้อมูลจากเส้นเชื่อม หรือ edges ทั้งหมด แล้วเข้าฟังก์ชันเพื่อหาวัฏจักร หรือ cycles
            for (int i = 0; i < graph.GetLength(0); i++)
                for (int j = 0; j < graph.GetLength(1); j++)
                    //เรียกใช้ฟังก์ชั่นหาวัฏจักร
                    findCyclesFromPath(new int[] { graph[i, j] });


            //เริ่มรวมวัฏจักรที่ได้จากตัวแปร cycles แล้วประกอบเป็นวงจร หรือ Circuit
            //สร้างลูปสำหรับเช็คข้อมูลทุกวัฏจักร
            for (int rowMain = 0; rowMain < cycles.Count; rowMain++)
            {
                //สร้างตัวแปรชื่อ nodeComb สำหรับเก็บแนวเดินใหม่ที่เกิดจากการรวม 2 วัฏจักรเข้าด้วยกัน
                List<int[]> nodeComb = new List<int[]>();
                //สร้างลูปสำหรับการตรวจสอบจุดยอดในแต่ละวัฏจักร
                for (int colMain = 0; colMain < cycles[rowMain].Length; colMain++)
                {
                    //เคลียร์ข้อมูลใน nodeComb เพื่อเริ่มเก็บข้อมูลใหม่
                    nodeComb.Clear();
                    //ตัวแปร nodeCombTemp เป็นตัวแปรที่เก็บแนวเดินที่ยังไม่สมบูรณ์
                    List<int> nodeCombTemp = new List<int>();
                    //สร้างลูปเพื่อตรวจสอบค่าวัฏจักรถัดไป
                    for (int rowComp = rowMain+1; rowComp < cycles.Count; rowComp++)
                    {
                        //ทำการตรวจสอบแต่ละจุดยอดในแนวเดิน ของวัฏจักรที่เลือกมาตรวจสอบ
                        for (int colComp = 0; colComp < cycles[rowComp].Length; colComp++)
                        {
                            int intMain = cycles[rowMain][colMain];
                            int intComp = cycles[rowComp][colComp];
                            // หากจุดยอดของแนวเดินของวัฏจักรปัจจุบันตรงกับจุดยอดในวัฏจักรเปรียบเทียบ
                            // จะทำการรวบเข้าด้วยกัน
                            // เช่น 
                            // 1 3 5 1
                            // 6 1 3 6
                            // เมื่อรวมกันแล้วจะได้ 
                            // 6 1 3 5 1 3 6
                            if (intMain == intComp)
                            {
                                // เคลียร์ข้อมูลในตัวแปรเก็บแนวเดิน เพื่อเริ่มต้นการควบวัฏจักร
                                nodeCombTemp.Clear(); 
                                // นำเข้าจุดยอดในวัฏจักรปัจจุบันทั้งหมดจนถึง จุดยอดในวัฏจักรใหม่ที่นำมาเปรียบเทียบ
                                for (int n = 0; n < colMain; n++)
                                    nodeCombTemp.Add(cycles[rowMain][n]); 
                                // นำเข้าจุดยอดที่เปรียบเทียบทั้งหมด
                                for(int n = colComp;n<cycles[rowComp].Length;n++)
                                    nodeCombTemp.Add(cycles[rowComp][n]); 
                                for (int n = 0; n <colComp; n++)
                                    nodeCombTemp.Add(cycles[rowComp][n]);
                                // นำเข้าจุดยอดจากวัฏจักรปัจจุบันที่เหลือ
                                for (int i = colMain; i < cycles[rowMain].Length; i++)
                                    nodeCombTemp.Add(cycles[rowMain][i]);
                                //นำวงจรที่ผ่านการควบรวมระหว่าง 2 วัฏจักร 
                                //มาใส่ในตัวแปร nodeComb ซึ่งเก็บแนวเดินใหม่
                                nodeComb.Add(nodeCombTemp.ToArray());
                                //เมือทำการควบรวมวัฏจักรเสร็จแล้ว ให้ออกจากลูป แล้วตรวจสอบวัฏจักรถัดไป
                                break;
                            }
                        }
                        
                       
                    }
                    //บันทึกแนวเดินที่ได้จากการรวม 2 วัฏจักรเข้าด้วยกัน
                    foreach (int[] n in nodeComb)
                    {
                        //หมุนแนวเดินให้เริ่มที่โหนดน้อยที่สุด
                        int[] p = sortPath(n);
                        //กลับทางแนวเดินเพื่อใช้ตรวจสอบภายหลัง
                        int[] inv = invertPath(n);
                        //ฟังก์ชั่น isNew(int[]) ใช้ตรวจสอบว่าแนวเดินที่ได้มีการบันทึกข้อมูลไว้แล้วหรือไม่ 
                        //ฟังก์ชั่น isOverlapedFromArrInt(int[]) ใช้ตรวจสอบว่าแนวเดินมีการเดินทับตำแหน่งเดิมหรือไม่
                        if (isPathNew(p) && isPathNew(inv) && !isOverlapedFromArrInt(p))
                            //บันทึกวงจรใหม่ที่ได้ในตัวแปร cycles
                            cycles.Add(p);
                    }
                }
            }

            //cycles add to datagridView
            foreach (int[] cy in cycles)
            {
                var nodeStart = getControlFromName("label" + cy[0]);

                string s = "" + nodeStart.Text;
                string s_name = "" + cy[0];

                for (int i = 1; i < cy.Length; i++)
                {
                    var node = getControlFromName("label" + cy[i]);
                    s += "," + node.Text;
                    s_name += "," + cy[i];
                }
                s += "," + nodeStart.Text;
                s_name += "," + cy[0];
                
                this.dataGridViewCycles.Rows.Add(s);
                this.dataGridViewCyclesOnlyName.Rows.Add(s_name);
                
                Console.WriteLine(s);
            }
        }

        //ฟังก์ชั่น สร้างวัฏจักร (Cycle)
        private void findCyclesFromPath(int[] path)
        {
            //กำหนดให้ตัวแปร startNode เป็นค่าจุดยอดเริ่มต้น เพื่อใช้เช็คภายหลังเมื่อเส้นเชื่อมกลับมาที่จุดยอดเดิม
            int firstVertex = path[0];
            //ใช้เก็บจุดยอดถัดไปที่ต่อจากจุดยอดเริ่มต้น
            int targetVertex;
            //ใช้เก็บเส้นเชื่อมถัดไป
            int[] extendPath = new int[path.Length + 1];
            //วนลูปรับค่าเส้นเชื่อมทั้งหมดจากกราฟที่รับเข้ามาในขั้นตอนแรก
            for (int i = 0; i < graph.GetLength(0); i++)
                for (int y = 0; y <= 1; y++)
                    //กรณีที่เจอเส้นเชื่อมที่ต่อจากจุดยอดเดิมให้เข้าเงื่อนไข if
                    if (graph[i, y] == firstVertex) 
                    {
                        //เซ็ตค่าจุดยอดถัดไปจากจุดยอดเริ่มต้น
                        targetVertex = graph[i, (y + 1) % 2];

                        //ตรวจสอบว่าจุดยอดถัดไปเคยมีการเดินผ่านมาแล้วหรือไม่
                        //ถ้าไม่เคยเดินผ่าน ให้เข้าเงื่อนไข
                        if (!isPassed(targetVertex, path))
                        //if(targetVertex != path[path.Length - 1])
                        {
                            //เพิ่มจุดยอดถัดไปเข้ากับแนวเดินใหม่ โดยเข้าที่ตำแหน่ง 0
                            //แล้วต่อแนวเดินใหม่ด้วยแนวเดินเก่า 
                            //(ทำให้จุดยอดใหม่ที่เข้ามาจะดันไปอยู่ที่ตำแหน่งแรกของแนวเดินใหม่เสมอ)
                            extendPath[0] = targetVertex;
                            Array.Copy(path, 0, extendPath, 1, path.Length);
                            //เรียกใช้ฟังก์ชั่นเดิมโดยส่งแนวเดินใหม่ที่ได้ เพื่อหาแนวเดินถัดไป
                            findCyclesFromPath(extendPath);
                        }
                        //ถ้าแนวเดินมีขนาดมากกว่า 2 (มีโอกาสกลับมาที่จุดยอดเดิมได้)
                        //และจุดยอดถัดไปคือจุดยอดตัวแรกของแนวเดิน 
                        //แสดงว่าแนวเดินวนมาที่จุดยอดเดิมแล้ว
                        else if ((path.Length > 2) && (targetVertex == path[path.Length - 1]))
                        //if ((path.Length > 2) && (targetVertex == path[path.Length - 1]))
                        {

                            //เรียกใช้ฟังก์ชั่นเพื่อหมุนวัฏจักรให้เริ่มที่จุดยอดที่มีค่าน้อยที่สุดเสมอ
                            int[] p = sortPath(path);
                            //เรียกใช้ฟังก์ชั่นเพื่อย้อนแนวเดินจากหลังไปหน้า
                            int[] inv = invertPath(path);
                            //ตรวจสอบว่าวัฏจักรที่ได้ซ้ำกับวัฏรเดิมหรือไม่
                            if (isPathNew(p) && isPathNew(inv))
                            //หากไม่ซ้ำกับวัฏจักรเดิมให้เพิ่มวัฏจักรใหม่
                                cycles.Add(p);
                        }
                    }
        }

        //ส่วนของการตรวจสอบแนวเดินซ้ำ

        //เก็บแนวเดิน
        private int[,] pathMemories = new int[100, 100];

        //ฟังก์ชั่น เคลียร์ข้อมูลแนวเดินทั้งหมดใน pathMemories
        private void resetPathMemories()
        {
            for (int i = 0; i < pathMemories.GetLength(0); i++) {
                for (int j = 0; j < pathMemories.GetLength(1); j++)
                    pathMemories[i, j] = 0;
            }
            Array.Clear(pathMemories, 0, 10000);
        }

        //ฟังก์ชั่นเพิ่มแนวเดินลงใน pathMemories
        private void addToPathMemories(int a, int b)
        {
            pathMemories[a, b]++;
            pathMemories[b, a]++;
        }

        //ฟังก์ชั่นตรวจสอบเส้นเชื่อมว่าทับกับแนวเดิน ใน pathMemories หรือไม่
        private bool isOverlaped(int a, int b)
        {
            if (pathMemories[a, b] > 1 || pathMemories[b, a] > 1)
                return true;
            return false;
        }

        //ฟังก์ชั่นตรวจสอบการ ทับซ้อนของแนวเดิน
        //หากแนวเดินไม่มีการทับซ้อนแนวเดินเก่า 
        //จะส่งค่าจริงกลับไป ถ้ามีการซ้อนทับกันจะส่งค่าเท็จกลับไป
        private bool isOverlapedFromArrInt(int[] n){
            int[] newN = new int[n.Length + 1];
            Array.Copy(n, 0, newN, 0, n.Length);
            newN[newN.Length - 1] = newN[0];
            for(int i=0; i < newN.Length-1; i++)
                addToPathMemories(newN[i], newN[i + 1]);
            for (int i = 0; i < newN.Length - 1; i++)
                if (isOverlaped(newN[i], newN[i + 1]))
                {
                    Console.Write("overlaped from " + newN[i] + "," + newN[i + 1] + " : ");
                    foreach (int nn in newN)
                        Console.Write(nn + ",");
                    Console.WriteLine("");
                    resetPathMemories();
                    return true;
                }
            resetPathMemories();
            return false;
        }

        

        //ฟังก์ชั่น ตรวจสอบว่าแนวเดินเหมือนกันหรือไม่
        private bool isSame(int[] a, int[] b)
        {
            bool ret = (a[0] == b[0]) && (a.Length == b.Length);

            for (int i = 1; ret && (i < a.Length); i++)
                if (a[i] != b[i])
                {
                    ret = false;
                }

            return ret;
        }

        //เรียงแนวเดินใหม่ จากหลังไปหน้า 
        //เพื่อให้สามารถตรวจสอบภายหลังได้ว่าเป็นวัฏจักรซ้ำซ้อนหรือไม่
        private int[] invertPath(int[] path)
        {
            int[] p = new int[path.Length];
            for (int i = 0; i < path.Length; i++)
                p[i] = path[path.Length - 1 - i];
            return sortPath(p);
        }

        //หมุนวัฏจักรที่ได้เพื่อให้เริ่มที่จุดยอดที่ค่าน้อยที่สุดเสมอ
        //เพื่อจัดเรียงให้เป็นแนวเดียวกัน และง่ายต่อการเปรียบเทียบในการตรวจสอบวัฏจักรซ้ำซ้อน
        private int[] sortPath(int[] path)
        {
            int min = getMin(path);
            
            int[] newPath = new int[path.Length];
            Array.Copy(path, 0, newPath, 0, path.Length);
            int length = newPath.Length;
            int temp;
            //เลื่อน path จนกว่าตำแหน่งแรกจะเป็นจุดยอดที่มีค่าน้อยที่สุด
            while (newPath[0] != min)
            {
                temp = newPath[0];
                Array.Copy(newPath, 1, newPath, 0, length - 1);
                newPath[length - 1] = temp;
            }

            return newPath;
        }

        //ตรวจสอบว่าวัฏจักรที่ได้ซ้ำกับวัฏจักรเดิมหรือไม่
        private bool isPathNew(int[] path)
        {
            foreach (int[] p in cycles)
                if (isSame(p, path))
                    return false;
            return true;
        }

        //ฟังก์ชั่นหาจุดยอดที่มีค่าน้อยที่สุดในแนวเดิน
        private int getMin(int[] path)
        {
            int min = path[0];
            foreach (int p in path)
                if (p < min)
                    min = p;
            return min;
        }

        //ฟังก์ชั่นตรวจสอบจุดยอดว่าเคยเดินผ่านแล้วหรือไม่
        private bool isPassed(int n, int[] path)
        {
            foreach (int p in path)
                if (p == n)
                    return true;
            return false;
        }
        
        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            try{
                for (int i = 0; i < linkedNode.GetLength(0); i++)
                {
                    if (linkedNode[i, 0] != "" && linkedNode[i, 0] != null && linkedNode[i, 1] != "" && linkedNode[i, 1] != null)
                    {
                        var node1 = this.Controls.Find("label" + linkedNode[i, 0], true)[0];
                        var node2 = this.Controls.Find("label" + linkedNode[i, 1], true)[0];
                        drawLine(node1, node2);

                        //drawline cycles
                        if(dataGridViewCyclesOnlyName.RowCount > 1)
                        {
                            int row = selectedRowCycles;
                            DataGridViewRow selectedRow = dataGridViewCyclesOnlyName.Rows[row];
                            string str;
                            str = selectedRow.Cells[0].Value.ToString();
                            //str is cycles
                            string[] t = str.Split(',');
                            //show this is euler label
                            this.labelThisIsEuler.Visible = IsEulerPath(str) ? true : false;

                            for(int j=0; j<t.Length-1; j++)
                            {
                                node1 = getControlFromName("label" + t[j]);
                                node2 = getControlFromName("label" + t[j+1]);
                                drawLineWithcolor(node1, node2,Color.Blue);
                            }
                        }
                        
                    }
                }
            } catch (IndexOutOfRangeException ee)
            {
                Console.WriteLine(ee.ToString());
                MessageBox.Show("กรุณาตรวจสอบข้อมูลอีกครั้ง");
            }
            
            
        }

        //draw linked line
        private void drawLineWithcolor(Control ob1, Control ob2,Color color)
        {
            Pen myPen = new Pen(color);
            myPen.Width = 5;
            g = canvas.CreateGraphics();
            Point point1 = ob1.PointToScreen(Point.Empty);
            Point point2 = ob2.PointToScreen(Point.Empty);
            point1.X -= this.Left;
            point2.X -= this.Left;
            point1.Y -= this.Top + 20;
            point2.Y -= this.Top + 20;
            Point[] points =
            {
                point1,
                point2
            };
            g.DrawLines(myPen, points);
        }

        //draw linked line
        private void drawLine(Control ob1, Control ob2)
        {
            myPen.Width = 3;
            g = canvas.CreateGraphics();
            Point point1 = ob1.PointToScreen(Point.Empty);
            Point point2 = ob2.PointToScreen(Point.Empty);
            point1.X -= this.Left;
            point2.X -= this.Left;
            point1.Y -= this.Top + 20;
            point2.Y -= this.Top + 20;
            Point[] points =
            {
                point1,
                point2
            };
            g.DrawLines(myPen, points);
            
        }
        //check is euler Path from string dataGridView
        private bool IsEulerPath(string dataGridPath)
        {
            string[] t = dataGridPath.Split(',');
            //valid is Euler Circuit
            if (t.Length >= totalLinkedNode + 1)
                return true;
            else
                return false;
        }

        //refresh line on canvas 
        private void refreshWhenMove(object sender, EventArgs e)
        {
            var nodeLabel = (Label)sender;
            if (!drawByHand)
            {
                Refresh();
            }
            //case draw by hand will can not move objects
            else
            {
            }
        }
        
        //when add Node button click
        private void btnAddNode_Click(object sender, EventArgs e)
        {
            if(textBoxNodeName.Text.Trim() != "")
            {
                int t = this.NodeCount + 1;
                String nodeName = "label" + t.ToString();
                Label nodeLabel= (Label) this.Controls.Find(nodeName, true)[0];
                int col = NodeCount % 10;
                int row = NodeCount / 10;
                
                //set initial location
                int startX = 100;
                int startY = 300;
                nodeLabel.Location = new Point(startX  + (40* col), startY + (50 * row));
                nodeLabel.Text = textBoxNodeName.Text;
                nodeLabel.Visible = true;
                ControlExtension.Draggable(nodeLabel, true);
                this.NodeCount++;

                
                //add event to newNode
                nodeLabel.Move += new EventHandler(refreshWhenMove);
                nodeLabel.DoubleClick += new EventHandler(nodeLabelDoubleClick);
                nodeLabel.Click += new EventHandler(nodeLabelClick);
                //load ComboBox
                loadComboBox();

                //set text on comboBox 
                t = this.NodeCount + 1;
                textBoxNodeName.Text = t.ToString();
            }
            else
            {
                MessageBox.Show("กรุณากรอกชื่อโหนด");
            }
        }
         
        //double to change node text
        private void nodeLabelDoubleClick(object sender, EventArgs e)
        {
            var node = (Label)sender;
            var ans = Interaction.InputBox("กรอกชื่อโหนดลงในกล่องข้อความด้านล่างแล้วกด \"OK\"","ตั้งชื่อโหนด", node.Text, -1,-1);
            if(ans != null && ans.Trim() != "")
            {
                node.Text = ans;
                loadComboBox();
                loadDataGridViewLinkedNode();
                //loadDataGridViewCycles();
            }
        }
        
        //add rount node - linked node
        private void btnAddRount_Click(object sender, EventArgs e)
        {
            //first is 0
            int nodeStart = comboBoxStart.SelectedIndex + 1;
            int nodeEnd = comboBoxEnd.SelectedIndex + 1;

            linkedNode[totalLinkedNode, 0] = nodeStart.ToString();
            linkedNode[totalLinkedNode, 1] = nodeEnd.ToString();

            totalLinkedNode++;
            canvas_Paint(null, null);

            loadDataGridViewLinkedNode();
            //callcurate line cycles
            //loadDataGridViewCycles();
        }

        //load color euler path
        int eulerPathTotal = 0;
        private void loadColorEulerPathOnDataGridViewCycles()
        {
            
            eulerPathTotal = 0;
            this.labelEulerCountIs.Text = "0";
            foreach (DataGridViewRow row in dataGridViewCycles.Rows)
            {
                try
                {
                    string dataFromRow = row.Cells[0].Value.ToString();
                    if (IsEulerPath(dataFromRow))
                    {
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                        eulerPathTotal++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            this.labelEulerCountIs.Text = eulerPathTotal.ToString();
        }
        //load Data Grid View Cycles
        private void loadDataGridViewCycles()
        {
            //load default
            if(dataGridViewCycles.RowCount > 1)
            {
                dataGridViewCycles.Rows.Clear();
                dataGridViewCyclesOnlyName.Rows.Clear();
            }
                int count = 0;
                //find size
                for (int i = 0; i < totalLinkedNode; i++)
                {
                    if (linkedNode[i, 0] != null && linkedNode[i, 1] != null)
                        count++;
                }
                graph = new int[count, 2]; // malloc arrays size
                int j = 0;
                for (int i = 0; i < totalLinkedNode; i++)
                {
                    if (linkedNode[i, 0] != null && linkedNode[i, 1] != null)
                    {
                        var nodeStart = getIntFromString(linkedNode[i, 0]);
                        var nodeEnd = getIntFromString(linkedNode[i, 1]);
                        graph[j, 0] = nodeStart;
                        graph[j, 1] = nodeEnd;
                        j++;
                    }
                }
                loadCycles();
                canvas_Paint(null, null);

            loadColorEulerPathOnDataGridViewCycles();
        }
        //get integer from string value
        private int getIntFromString(string str)
        {
            return int.Parse(str);
        }


        //load data grid view linked node
        private void loadDataGridViewLinkedNode()
        {
            //load default linked node
            this.dataGridViewLinkedNode.Rows.Clear();
            for (int i = 0; i < totalLinkedNode; i++)
            {
                if (linkedNode[i, 0] != null && linkedNode[i, 1] != null)
                {
                    var node1 = getControlFromName("label" + linkedNode[i, 0]);
                    var node2 = getControlFromName("label" + linkedNode[i, 1]);
                    this.dataGridViewLinkedNode.Rows.Add("( " + node1.Text + " , " + node2.Text + " )");
                }
            }
            
        }

        //click cell to show cycles line
        private void dataGridViewCycles_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridViewCyclesLoadSelectedIndex(); 
        }

        //load selected data grid index
        private void dataGridViewCyclesLoadSelectedIndex()
        {
            try
            {
                int rowSelected = dataGridViewCycles.CurrentRow.Index;
                if (rowSelected != -1 && rowSelected != dataGridViewCycles.RowCount - 1)
                {
                    //do somethings
                    selectedRowCycles = rowSelected;
                }
                Refresh();
                canvas_Paint(null, null);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
        private void buttonGetCycles_Click(object sender, EventArgs e)
        {
            loadDataGridViewCycles();

            //load total sub Cycles
            int subTotal = cycles.Count - eulerPathTotal;
            this.labelSubCyclesTotalIs.Text = subTotal.ToString();
        }

        private void buttonDrawByHand_Click(object sender, EventArgs e)
        {
            drawByHand = drawByHand == true ? false:true;
            //open
            if (drawByHand)
            {
                var button = (Button)sender;
                button.BackColor = Color.Yellow;
                labelDrawByHand.Visible = true;
                for (int i = 1; i <= NodeCount; i++)
                {
                    var nodeLabel = this.Controls.Find("label" + i, true)[0];
                    ControlExtension.Draggable(nodeLabel, false);
                }
            }
            else
            //close
            {
                var button = (Button)sender;
                button.BackColor = Color.Gainsboro;
                for (int i = 1; i <= NodeCount; i++)
                {
                    var nodeLabel = this.Controls.Find("label" + i, true)[0];
                    ControlExtension.Draggable(nodeLabel, true);
                }
                clearSelected();
                labelDrawByHand.Visible = false;
            }
                
        }

        int[] selected = new int[2] {-1,-1};
        private void nodeLabelClick(object sender, EventArgs e)
        {
            //can click when draw by hand mode on
            if (drawByHand)
            {
                var nodeLabel = (Label)sender;
                int nodeNum = Int32.Parse(nodeLabel.Name.Replace("label", ""));
                if (nodeLabel.BackColor == Color.DodgerBlue) 
                //selected
                {
                    if (selected[0] < 1)
                    {
                        nodeLabel.BackColor = Color.Red;
                        selected[0] = nodeNum;
                        comboBoxStart.SelectedIndex = selected[0]-1;
                    }
                    else if (selected[1] < 1)
                    {
                        nodeLabel.BackColor = Color.Red;
                        selected[1] = nodeNum;
                        comboBoxEnd.SelectedIndex = selected[1]-1;
                        //add to linked node by click button
                        btnAddRount_Click(null, null);
                        clearSelected();
                    }
                    else
                    {
                        // nothing
                    }
                }
                //unselected
                else if (nodeLabel.BackColor == Color.Red)
                {
                    if (selected[0] == nodeNum)
                    {
                        nodeLabel.BackColor = Color.DodgerBlue;
                        selected[0] = -1;
                        comboBoxStart.SelectedIndex = -1;
                    }
                    else if (selected[1] == nodeNum)
                    {
                        nodeLabel.BackColor = Color.DodgerBlue;
                        selected[1] = -1;
                        comboBoxEnd.SelectedIndex = -1;
                    }
                    else
                    {
                        // nothing
                    }
                }
                   
            }
        }
        private void clearSelected()
        {
            if (selected[0] > 0)
            {
                var nodeStart = this.Controls.Find("label" + selected[0], true)[0];
                nodeStart.BackColor = Color.DodgerBlue;
            }
            if (selected[1] > 0)
            {
                var nodeEnd = this.Controls.Find("label" + selected[1], true)[0];
                nodeEnd.BackColor = Color.DodgerBlue;
            }
            selected[0] = -1;
            selected[1] = -1;
        }
        
        private void dataGridViewCycles_KeyUp(object sender, KeyEventArgs e)
        {
            dataGridViewCyclesLoadSelectedIndex();
        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "พัฒนาโดย " + 
                "\n" + "นายกันตพงศ์ พูนเกษม" +
                "\n" + "อีเมลล์ติดต่อ : ToeiKanta@gmail.com" + 
                "\n" + "อีเมลล์ติดต่อสำรอง : Toei_Kanta@hotmail.com" +
                "\n" + "มหาวิทยาลัยเทคโนโลยีสุรนารี" +
                "\n\n" + "โปรแกรมเวอร์ชั่น "+ this.version,
                "เกี่ยวกับผู้พัฒนา"
                );
        }
    }
}
