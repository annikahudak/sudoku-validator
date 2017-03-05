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
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        void init()
        {
            Console.WriteLine("Before start thread");

            Thread[] testThread = new Thread[10];
            Thread tid1 = new Thread(new ThreadStart(MyThread.Thread1));
            Thread tid2 = new Thread(new ThreadStart(MyThread.Thread2));

            testThread[0] = tid1;
            testThread[1] = tid2;

            for (int i = 0; i < 9; ++i)
            {
                Thread tid = new Thread(new ThreadStart(MyThread.Thread3));
                testThread[i + 2] = tid;
            }
            foreach (Thread myThread in testThread)
            {
                myThread.Start();
            }

        }
        void readFile()
        {
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
        public static void Thread1()
        {
            Console.WriteLine("Hello I am Row Thread");
        }
        public static void Thread2()
        {
            Console.WriteLine("Hello I am Column Thread");
        }
        public static void Thread3()
        {
            Console.WriteLine("Hello I am Square Thread");
        }
    }
}
