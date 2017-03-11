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
    // MAIN class
    public partial class Form1 : Form
    {
        // Sudoku Grid
        int[,] grid = new int[9,9];
        // Invalid indices 
        HashSet<Tuple<int,int>> problems = new HashSet<Tuple<int,int>>();

        public Form1()
        {
            InitializeComponent();
            this.CenterToScreen();
            dataGridView1.Hide();
            update_label.Hide();
            label5.Hide();
            label6.Hide();
        }

        // Init program
        private void goBtn_Click(object sender, EventArgs e)
        {
            readFile();

            if (readFile())
            {
                fileNametb.Hide();
                goBtn.Hide();
                filename_label.Hide();

                printGrid();
                init();

                MyThread.setGrid(grid);
            }
        }

        bool readFile()
        {
            string line;
            var lines = new List<string[]>();
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(fileNametb.Text);
                while ((line = file.ReadLine()) != null)
                {
                    string[] Line = line.Split(',');
                    lines.Add(Line);
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
                return true;

            }
            catch (Exception e)
            {
                MessageBox.Show("File not found.");
            }
            return false;
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
        void init()
        {
            Thread[] testThread = new Thread[11];

            // Explicitly create threads
            Thread tid1 = new Thread(new ThreadStart(MyThread.Thread1));
            Thread tid2 = new Thread(new ThreadStart(MyThread.Thread2));

            testThread[0] = tid1;
            testThread[1] = tid2;

            for (int i = 2; i < 11; ++i)
            {
                Thread tid = new Thread(new ThreadStart(MyThread.Thread3));
                testThread[i] = tid;
               
            }
            // Start threads
            foreach (Thread myThread in testThread)
            {
                this.toolStripStatusLabel1.Text= "Starting Thread " + (Array.IndexOf(testThread, myThread) + 1);
                this.Refresh();
                update_label.Text = "Now validating " + MyThread.getCurrentThreadString() + "...";
                update_label.Refresh();
                Thread.Sleep(1500);
                myThread.Start();
            }
            // Join threads
            foreach (Thread myThread in testThread)
            {
                myThread.Join();
                this.toolStripStatusLabel1.Text = "Threads joined";
                this.Refresh();
            }

            label5.Show();
            label6.Show();

            // Calculate errors
            if (MyThread.getProblemRows().Count != 0 && MyThread.getProblemColumns().Count != 0)
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
                        dataGridView1.Rows[r - 1].DefaultCellStyle.BackColor = Color.MistyRose;
                        dataGridView1.Columns[c - 1].DefaultCellStyle.BackColor = Color.MistyRose;
                        dataGridView1.Rows[r-1].Cells[c-1].Style.BackColor = Color.Red;
                        
                    }
                }

                label3.Text = "Fix: \n" + string.Join(", ", problems);
                label3.Refresh();

                var missingSquareList = new List<IEnumerable<int>>();
                foreach (Square s in Square.getSquares())
                {
                    List<int> list = s.getSquare().Cast<int>().ToList();
                    var missing = Enumerable.Range(1, 9).Except(list);
                    missingSquareList.Add(missing);
                }

                var missingRowList = new List<IEnumerable<int>>();
                HashSet<Tuple<int,int[]>> rowTuples = new HashSet<Tuple<int, int[]>>();

                foreach(int rowM in MyThread.getProblemRows())
                {
                    List<int> list2 = new List<int>();
                    for(int g = 0; g < 9; g++)
                    {
                        list2.Add(grid[rowM-1, g]);
                    }
                    var missing = Enumerable.Range(1, 9).Except(list2);
                    missingRowList.Add(missing);

                    Tuple<int, int[]> rowValue = new Tuple<int, int[]>(rowM, missing.ToArray());
                    rowTuples.Add(rowValue);
                }

                HashSet<Tuple<int, int[]>> colTuples = new HashSet<Tuple<int, int[]>>();

                var missingColList = new List<IEnumerable<int>>();
                foreach(int colM in MyThread.getProblemColumns())
                {
                    List<int> list3 = new List<int>();
                    for(int g = 0; g < 9; g++)
                    {
                        list3.Add(grid[g, colM-1]);
                    }
                    var missing = Enumerable.Range(1, 9).Except(list3);
                    missingColList.Add(missing);

                    Tuple<int, int[]> colValue = new Tuple<int, int[]>(colM, missing.ToArray());
                    colTuples.Add(colValue);

                }

                update_label.Hide();
                label4.Text = "SOLUTION: \n";
               
                foreach(Tuple<int,int> t in problems)
                {
                    foreach(Tuple<int, int[]> tc in colTuples)
                    {
                        if(tc.Item1 == t.Item2){
                            
                            label4.Text += t + " => " + tc.Item2[0] + "\n";
                            label4.Refresh();
                            
                        }
                    }
                }
            }
            else
            {
                label3.Text = "Valid solution";
                label3.Refresh();
            }
        }
       

        private void fileNametb_MouseClick(object sender, MouseEventArgs e)
        {
            fileNametb.Text = " ";
            fileNametb.ForeColor = Color.Black;
        }
    }
    public class MyThread
    {
        public static int[,] g = new int[9, 9]; //copy of input grid
        public static HashSet<int> problemRows = new HashSet<int>();
        public static HashSet<int> problemColumns = new HashSet<int>();

        public static int rowNum = 0;
        public static int colNum = 0;

        public static string currentThread = "";

        public static void Thread1()
        {
            currentThread = "rows";

            var nums = new HashSet<int>();
            for (int i = 0; i < 9; i++)
            {
                var row = Enumerable.Range(0, 9).Select(x => g[i, x]);

                if (row.Distinct().Count() != 9)
                {
                    problemRows.Add(i+1);
                }
            }
        }
        public static void Thread2()
        {
            currentThread = "columns";

            var nums = new HashSet<int>();
             for (int j = 0; j < 9; j++)
             {
                 var column = Enumerable.Range(0, 9).Select(x => g[x, j]);

                 if (column.Distinct().Count() != 9)
                 {
                     problemColumns.Add(j+1);
                 }
             }

        }
        public static void Thread3()
        {
            currentThread = "squares";

            int[,] miniArray = new int[3,3];
            int row_Index = 0;
            int col_Index = 0;
           
            Square s = new Square(rowNum, colNum);

            for(int i = rowNum; i < rowNum + 3; i++)
            {
                for(int j = colNum; j < colNum + 3; j++)
                {
                    miniArray[row_Index, col_Index] = g[i, j];
                    col_Index++;
                }
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
        public static string getCurrentThreadString()
        {
            return currentThread;
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
