using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            this.Size = new Size(FormSize.Width/2, FormSize.Height/2);
        }
        private System.Windows.Forms.ListView ScoreListView;
        private PictureBox NameSort;
        private PictureBox DuckSort;
        private PictureBox LevelSort;
        private void ScoreTable_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);
            this.BackColor = Color.Blue;
            ScoreListView = new System.Windows.Forms.ListView();
            ScoreListView.Size = new Size(this.Width / 2, this.Height / 2);
            ScoreListView.Location = new Point(0, 0);
            ScoreListView.View = View.Details;
            ScoreListView.FullRowSelect = true;

            ScoreListView.Columns.Add("Name");
            ScoreListView.Columns.Add("Coins");
            ScoreListView.Columns.Add("Levels");
            ResizeColumns(ScoreListView);
            string[] row1 = { "Ahoj", "Jak se Mas" };
            ListViewItem item1 = new ListViewItem(row1);
            ScoreListView.Items.Add(item1);

            // Create second row
            string[] row2 = { "Ahoj", "Mam se dobre" };
            ListViewItem item2 = new ListViewItem(row2);
            ScoreListView.Items.Add(item2);
            this.Controls.Add(ScoreListView);
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
