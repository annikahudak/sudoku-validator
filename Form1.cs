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
        HashSet<Tuple<int,int>> problems = new HashSet<Tuple<int,int>>();

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
            
            /* Cross check all problem rows with all problem columns, then use square
                to determine what numbers are missing and compare with those rows and columns
                to not make duplicates */

            if(MyThread.getProblemRows().Count != 0 && MyThread.getProblemColumns().Count != 0)
            {
                label1.Text = "Problem rows: \n" + string.Join(", ", MyThread.getProblemRows());
                label1.Refresh();

                label2.Text = "Problem columns: \n" + string.Join(", ", MyThread.getProblemColumns());
                label2.Refresh();

                foreach(int r in MyThread.getProblemRows())
                {
                    foreach(int c in MyThread.getProblemColumns())
                    {
                        Tuple<int, int> tuple = new Tuple<int, int>(r, c);
                        problems.Add(tuple);
                    }
                }

                label3.Text = "Fix: \n" + string.Join(", ", problems);
                label3.Refresh();

                //dataGridView1.Rows[rowIndex].Cells[columnIndex].Style.BackColor = Color.Red;

                //var list = new List<int>(new[] { 1, 2, 4, 7, 9 });
                // var result = Enumerable.Range(0, 10).Except(list);

                var missingSquareList = new List<IEnumerable<int>>();
                foreach (Square s in Square.getSquares())
                {
                    List<int> list = s.getSquare().Cast<int>().ToList();
                    var missing = Enumerable.Range(1, 9).Except(list);
                    missingSquareList.Add(missing);
                    //MessageBox.Show(string.Join(", ", result));
                }

                var missingRowList = new List<IEnumerable<int>>();
                foreach(int rowM in MyThread.getProblemRows())
                {
                    List<int> list2 = new List<int>();
                    for(int g = 0; g < 9; g++)
                    {
                        list2.Add(grid[rowM, g]);
                    }
                    var missing = Enumerable.Range(1, 9).Except(list2);
                    missingRowList.Add(missing);
                }

                var missingColList = new List<IEnumerable<int>>();
                foreach(int colM in MyThread.getProblemColumns())
                {
                    List<int> list3 = new List<int>();
                    for(int g = 0; g < 9; g++)
                    {
                        list3.Add(grid[g, colM]);
                    }
                    var missing = Enumerable.Range(1, 9).Except(list3);
                    missingColList.Add(missing);
                }



                foreach(Tuple<int,int> t in problems)
                {
                    
                }
            }
            else
            {
                label3.Text = "Valid solution";
                label3.Refresh();
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

        public static int rowNum = 0;
        public static int colNum = 0;


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
                    problemRows.Add(i+1);
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
                     problemColumns.Add(j+1);
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

            int[,] miniArray = new int[3,3];
            int row_Index = 0;
            int col_Index = 0;
           
            Square s = new Square(rowNum, colNum);

            for(int i = rowNum; i < rowNum + 3; i++)
            {
                for(int j = colNum; j < colNum + 3; j++)
                {
                    // Console.WriteLine("COL INDEX " + colIndex);
                  // Console.WriteLine("IM NOT OUT OF BOUNDS " + row_Index + " " + col_Index);
                  //  Console.WriteLine("BUT AM I OUT OF BOUNDS ?? " + rowNum + " " + colNum);
                   // Console.WriteLine("i - " + i + " j - " + j);
                    miniArray[row_Index, col_Index] = g[i, j];
                    
                    col_Index++;
                    
                }
               // Console.WriteLine("ROW INDEX " + row_Index);
                row_Index++;
                col_Index = 0;
            }
            s.setSquare(miniArray);
            s.checkSquare();
            
            if(rowNum == 6)
            {
                rowNum = 0;
                colNum += 3;
            }
            else
            {
                rowNum += 3;
            }
        }
        public static void setGrid(int[,] grid)
        {
            g = grid;
        }
        public static int[,] getGrid()
        {
            return g;
        }
        public static HashSet<int> getProblemRows()
        {
            return problemRows;
        }
        public static HashSet<int> getProblemColumns()
        {
            return problemColumns;
        }
    }
}
public class Square
{
    int rowIndex;
    int colIndex;
    int[,] square = new int[3,3];
    HashSet<int> squareList = new HashSet<int>();
    public static List<Square> problemSquares = new List<Square>();

    public Square(int r, int c)
    {
        this.rowIndex = r;
        this.colIndex = c;
    }
    public void setSquare(int[,] s)
    {
        this.square = s;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                squareList.Add(square[i, j]);
            }
        }
    }
    public void checkSquare()
    {
      if(squareList.Count() != 9)
        {
            problemSquares.Add(this);
           // Console.WriteLine("hello i am square -- " + this.rowIndex + " " + this.colIndex);
        }
    }
    public static List<Square> getSquares()
    {
        return problemSquares;
    }
    public int getRow()
    {
        return this.rowIndex;
    }
    public int getCol()
    {
        return this.colIndex;
    }
    public int[,] getSquare()
    {
        return this.square;
    }

}
