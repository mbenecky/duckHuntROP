using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace duckHuntROP
{
    public partial class ScoreTable : UserControl
    {
        public ScoreTable()
        {
            InitializeComponent();
        }
        public ScoreTable(Size FormSize)
        {
            InitializeComponent();
            this.Size = new Size(FormSize.Width / 2, FormSize.Height / 2);
        }
        public static System.Windows.Forms.ListView ScoreListView;

        private void ScoreTable_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);
            this.BackColor = Color.Transparent;
            ScoreListView = new System.Windows.Forms.ListView();
            ScoreListView.Size = new Size(this.Width / 2, this.Height / 2+this.Height/6);
            ScoreListView.Location = new Point(this.Width/32, this.Height/4);
            ScoreListView.View = View.Details;
            ScoreListView.FullRowSelect = true;

            ScoreListView.Columns.Add("Name");
            ScoreListView.Columns.Add("Coins");
            ScoreListView.Columns.Add("Levels");
            ResizeColumns(ScoreListView);

            this.Controls.Add(ScoreListView);

        }
        
        public void UpdateListView()
        {
            if(!File.Exists("save.dat"))
            {
                using(File.Create("save.dat")) { }
            }
            Game.Show("save.dat");
            try
            {
                using (StreamReader sr = new StreamReader("save.txt"))
                {
                   ScoreListView.Items.Clear();
                    string[] splitPath = File.ReadAllLines("save.txt");
                    foreach (string a in splitPath)
                    {
                        string[] splitGameData = a.Split(';');
                        string[] row = { splitGameData[0], splitGameData[1], splitGameData[2] };
                        ListViewItem item = new ListViewItem(row);
                        ScoreListView.Items.Add(item);
                    }
                }

                Game.Hide("save.txt");
            }
            catch(Exception ex)
            {
                MessageBox.Show("UpdateListView went wrong " + ex.Message);
            }
        }
        private void ResizeColumns(System.Windows.Forms.ListView listView)
        {
            int columnCount = listView.Columns.Count;
            int availableWidth = listView.ClientSize.Width;

            int columnWidth = availableWidth / columnCount;

            foreach (ColumnHeader column in listView.Columns)
            {
                column.Width = columnWidth;
            }
        }
    }
}