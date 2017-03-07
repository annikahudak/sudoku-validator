using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        int[,] grid = new int[9,9];

        public Form1()
        {
            InitializeComponent();
            this.CenterToScreen();
            dataGridView1.Hide();
            update_label.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        void init()
        {
            Console.WriteLine("Before start thread");

            Thread[] testThread = new Thread[11];
            Thread tid1 = new Thread(new ThreadStart(MyThread.Thread1));
            Thread tid2 = new Thread(new ThreadStart(MyThread.Thread2));

            testThread[0] = tid1;
            testThread[1] = tid2;

            for (int i = 2; i < 11; ++i)
            {
                Thread tid = new Thread(new ThreadStart(MyThread.Thread3));
                testThread[i] = tid;
            }
            foreach (Thread myThread in testThread)
            {
                update_label.Text = "Starting Thread " + (Array.IndexOf(testThread, myThread) + 1);
                update_label.Refresh();
                Thread.Sleep(1000);
                myThread.Start();
            }
            foreach (Thread myThread in testThread)
            {
                myThread.Join();
                update_label.Text = "Threads joined.";
                update_label.Refresh();
            }

        }
        void readFile()
        {
            //int[,] grid = new int[9, 9];
            string line;
            var lines = new List<string[]>();

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(fileNametb.Text);
            while ((line = file.ReadLine()) != null)
            {
                string[] Line = line.Split(',');
                lines.Add(Line);
                //Console.WriteLine(line);

            }

            file.Close();
            var data = lines.ToArray();
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    grid[i, j] = Int32.Parse(data[i][j]);
                }

            }
            MyThread.setGrid(grid);
        }
        void printGrid()
        {

            this.dataGridView1.ColumnCount = 9;

            for (int x = 0; x < 9; x++)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(this.dataGridView1);

                for (int y = 0; y < 9; y++)
                {
                    row.Cells[y].Value = grid[x, y];
                }
                this.dataGridView1.Rows.Add(row);
            }
            this.dataGridView1.ColumnHeadersVisible = false;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
           

            dataGridView1.Show();
            update_label.Show();
            dataGridView1.Refresh();
       }

        //Read file
        private void goBtn_Click(object sender, EventArgs e)
        {
            readFile();

            fileNametb.Hide();
            goBtn.Hide();
            filename_label.Hide();
            
            printGrid();
            init();

            Console.WriteLine("GRID");

            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    Console.WriteLine(grid[i, j]);
                }
            }
            Console.WriteLine("END GRID");

                    MyThread.setGrid(grid);
        }

        private void fileNametb_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void fileNametb_MouseClick(object sender, MouseEventArgs e)
        {
            fileNametb.Text = " ";
            fileNametb.ForeColor = Color.Black;
        }
    }
    public class MyThread
    {
        public static int[,] g = new int[9, 9];
        public static HashSet<int> problemRows = new HashSet<int>();
        public static HashSet<int> problemColumns = new HashSet<int>();
        public static HashSet<int[]> problemSquares = new HashSet<int[]>();
        public static HashSet<int> correct = new HashSet<int>();

        public static int rowNum = -3;
        public static int colNum = -3;


        public static void Thread1()
        {
            
            Console.WriteLine("Hello I am Row Thread");
            var nums = new HashSet<int>();
            for (int i = 0; i < 9; i++)
            {
                var row = Enumerable.Range(0, 9).Select(x => g[i, x]);
                //Console.WriteLine("{0}", string.Join(",", column));

                if (row.Distinct().Count() != 9)
                {
                    problemRows.Add(i);
                }
            }

            Console.WriteLine("PROB ROWS");
            foreach(int n in problemRows){
                Console.WriteLine(n);
            }
            Console.WriteLine("END PROB ROWS");

        }
        public static void Thread2()
        {
            Console.WriteLine("Hello I am Column Thread");

            var nums = new HashSet<int>();
             for (int j = 0; j < 9; j++)
             {
                 var column = Enumerable.Range(0, 9).Select(x => g[x, j]);
                 //Console.WriteLine("{0}", string.Join(",", column));

                 if (column.Distinct().Count() != 9)
                 {
                     problemColumns.Add(j);
                 }
             }
            Console.WriteLine("PROB COLUMNS");

           foreach (int n in problemColumns)
            {
                Console.WriteLine(n);
            }
            Console.WriteLine("END PROB COLUMNS");

        }
        public static void Thread3()
        {
            Console.WriteLine("Hello I am Square Thread");

            int[] miniArray = new int[9];
            int index = 0;
            for(int i = rowNum + 3; i < i + 3; i++)
            {
                for(int j = colNum + 3; j < j + 3; j++)
                {
                    miniArray[index] = g[i, j];
                    index++;
                }
            }

            for(int k = 0; k < 9; k++)
            {
                if (!miniArray.Contains(k))
                {
                    problemSquares.Add(miniArray);
                }
            }


        }
        public static void setGrid(int[,] grid)
        {
            g = grid;
            for(int i = 0; i < 9; ++i)
            {
                correct.Add(i);
            } 
        }
        public static int[,] getGrid()
        {
            return g;
        }
    }
}
public class Square
{
    int rowIndex;
    int colIndex;
    int[,] square;

    public Square(int r, int c)
    {
        this.rowIndex = r;
        this.colIndex = c;
    }
}
