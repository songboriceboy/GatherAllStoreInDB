using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BlogGather;
using System.IO;

namespace GatherAll
{
    public partial class Form1 : Form
    {
        private DataTable m_ataTable;
        private string m_strDBFolder = Application.StartupPath + @"\WebSiteDB\";
        private Cls_SqliteMng m_sqliteMng = new Cls_SqliteMng();

        string m_connStr1 = @"Data Source=" + Application.StartupPath + @"\WebSiteDB\";
        string m_connStr2 = @";Initial Catalog=sqlite;Integrated Security=True;Max Pool Size=10";
        string m_strCreatTable = @"--1-2 层节点表(AU_LayerNode)
drop table if exists [AU_LayerNode];
CREATE TABLE AU_LayerNode(
	AU_LayerNodeID         		INT NOT NULL PRIMARY KEY,
	AU_ParentLayerNodeID		INT NOT NULL DEFAULT 0,
	AU_UrlAddress		  	VARCHAR(1000) NOT NULL DEFAULT '',
	AU_UrlTitle	  		NVARCHAR(1000) NOT NULL DEFAULT '',
	AU_UrlContent			NTEXT NOT NULL DEFAULT '', 
	AU_UrlLayer	           	INT NOT NULL DEFAULT 0,	
	AU_IsVisit	           	INT NOT NULL DEFAULT 0,	
	AU_RemoveSameOffset1  		INT NOT NULL DEFAULT 0, 
	AU_RemoveSameOffset2  		INT NOT NULL DEFAULT 0, 
	AU_LastUpdateDate	   	DATETIME  NOT NULL DEFAULT '2012-01-01',


	AU_ReserveInt1			INT NOT NULL DEFAULT 0,
	AU_ReserveInt2			INT NOT NULL DEFAULT 0,
	AU_ReserveInt3			INT NOT NULL DEFAULT 0,
	AU_ReserveInt4			INT NOT NULL DEFAULT 0,
	AU_ReserveInt5			INT NOT NULL DEFAULT 0,
	AU_ReserveInt6			INT NOT NULL DEFAULT 0,
	AU_ReserveInt7			INT NOT NULL DEFAULT 0,
	AU_ReserveInt8			INT NOT NULL DEFAULT 0,


	AU_ReserveStr1		  	VARCHAR(1000) NOT NULL DEFAULT '',
	AU_ReserveStr2		  	VARCHAR(1000) NOT NULL DEFAULT '',
	AU_ReserveNStr1		  	NVARCHAR(1000) NOT NULL DEFAULT '',
	AU_ReserveNStr2		  	NVARCHAR(1000) NOT NULL DEFAULT '',

	AU_ReserveTEXT1		  	TEXT NOT NULL DEFAULT '',
	AU_ReserveTEXT2		  	TEXT NOT NULL DEFAULT '',
	AU_ReserveTEXT3		  	TEXT NOT NULL DEFAULT '',
	AU_ReserveNTEXT1		NTEXT NOT NULL DEFAULT '',
	AU_ReserveNTEXT2		NTEXT NOT NULL DEFAULT '',
	AU_ReserveNTEXT3		NTEXT NOT NULL DEFAULT '',

	AU_ReserveDateTime1	   	DATETIME  NOT NULL DEFAULT '2012-01-01',
	AU_ReserveDateTime2	   	DATETIME  NOT NULL DEFAULT '2012-01-01',
	AU_ReserveDateTime3	   	DATETIME  NOT NULL DEFAULT '2012-01-01',
	AU_ReserveDateTime4	   	DATETIME  NOT NULL DEFAULT '2012-01-01',

	AU_ReserveDecmial1		DECIMAL NOT NULL DEFAULT 0,
	AU_ReserveDecmial2		DECIMAL NOT NULL DEFAULT 0
);

";
        private string m_strInsertTaskInitData = @"insert into [AU_LayerNode] values(0, 0, '#^$BlogID$^#','', '', 0, 0, 0, 0
         , '2012-01-01', 0, 0, 0, 0, 0, 1, 1, 0,'', '','', '','', '','', '','', '', '2012-01-01', '2012-01-01', '2012-01-01', '2012-01-01', 1, 0)";
   
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.backgroundWorker1.RunWorkerAsync();
            m_ataTable = new DataTable();

            m_ataTable.Columns.Add("标题", System.Type.GetType("System.String"));
            m_ataTable.Columns.Add("内容", System.Type.GetType("System.String"));


  

            this.dataGridView1.DataSource = m_ataTable;
            this.dataGridView1.Columns[1].Visible = false;
            this.dataGridView1.Columns[0].Width = this.Width;
        }
        private void AddBlog(BlogGather.DelegatePara dp)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new BlogGatherCnblogs.GreetingDelegate(this.AddBlog), dp);
                return;
            }
            DataRow row1 = m_ataTable.NewRow();
            row1["标题"] = dp.strTitle;
            row1["内容"] = dp.strContent;
            m_ataTable.Rows.Add(row1);
            this.dataGridView1.DataSource = m_ataTable;
            this.dataGridView1.Columns[1].Visible = false;
            this.dataGridView1.Columns[0].Width = this.Width;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!Directory.Exists(m_strDBFolder))
            {
                Directory.CreateDirectory(m_strDBFolder);
            }
            m_sqliteMng.CreateDB(m_strDBFolder + this.toolStripTextBox1.Text + ".db");
            m_sqliteMng.ExecuteSql(m_strCreatTable
                , m_connStr1 + this.toolStripTextBox1.Text + ".db" + m_connStr2);

            string strInsertTaskInitData = m_strInsertTaskInitData.Replace("#^$BlogID$^#", this.toolStripTextBox1.Text);

            m_sqliteMng.ExecuteSql(strInsertTaskInitData
                , m_connStr1 + this.toolStripTextBox1.Text + ".db" + m_connStr2);
            BlogGatherCnblogs bgb = new BlogGatherCnblogs(this.toolStripTextBox1.Text);
            bgb.delAddBlog += new BlogGatherCnblogs.GreetingDelegate(this.AddBlog);
            bgb.GatherBlog(e);
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.dataGridView1.RowCount <= 0)
                return;

            if (this.dataGridView1.CurrentCell == null)
                return;
            string strContent = this.dataGridView1.Rows[this.dataGridView1.CurrentCell.RowIndex].Cells[1].Value.ToString();
            this.webBrowser1.DocumentText = strContent;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("全部博客下载完成!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.toolStripTextBox1.Text = "ice-river";
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            {
                using (SolidBrush b = new SolidBrush(dataGridView1.RowHeadersDefaultCellStyle.ForeColor))
                {
                    e.Graphics.DrawString((e.RowIndex + 1).ToString(System.Globalization.CultureInfo.CurrentUICulture), e.InheritedRowStyle.Font, b, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4);
                }
            }
        }


    }
}
