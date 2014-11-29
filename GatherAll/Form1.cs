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
using System.Text.RegularExpressions;

namespace GatherAll
{
    public partial class Form1 : Form
    {
        protected DYH_DB.BLL.AU_LayerNode m_bllAU_LayerNode = new DYH_DB.BLL.AU_LayerNode();
        private string m_strDBConStringPath = @"Data Source=" + Application.StartupPath + @"\WebSiteDB\";

        private string m_strDBFolder = Application.StartupPath + @"\WebSiteDB\";
        private Cls_SqliteMng m_sqliteMng = new Cls_SqliteMng();
        private BloomFilter m_bf = new BloomFilter(10485760);
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
 
        }
        private void AddBlog(BlogGather.DelegatePara dp)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new BlogGatherCnblogs.GreetingDelegate(this.AddBlog), dp);
                return;
            }

            try
            {
                string strWholeDbName = m_strDBConStringPath + this.toolStripTextBox1.Text + ".db";



                DYH_DB.Model.AU_LayerNode modelAU_LayerNode = new DYH_DB.Model.AU_LayerNode();
                modelAU_LayerNode.AU_ParentLayerNodeID = -1;
                modelAU_LayerNode.AU_LayerNodeID = m_bllAU_LayerNode.GetMaxId(strWholeDbName);
                modelAU_LayerNode.AU_UrlLayer = 0;
                modelAU_LayerNode.AU_UrlAddress = "";
                string strTitle = Regex.Replace(dp.strTitle, @"[|/\;.':*?<>-]", "").ToString();
                strTitle = Regex.Replace(strTitle, "[\"]", "").ToString();
                strTitle = Regex.Replace(strTitle, @"\s", "");
                modelAU_LayerNode.AU_UrlTitle = strTitle;
                modelAU_LayerNode.AU_UrlContent = dp.strContent; ;
                modelAU_LayerNode.AU_IsVisit = 0;
                modelAU_LayerNode.AU_RemoveSameOffset1 = 0;
                modelAU_LayerNode.AU_RemoveSameOffset2 = 0;
                modelAU_LayerNode.AU_LastUpdateDate = System.DateTime.Now.Date;

                m_bllAU_LayerNode.Add(strWholeDbName, modelAU_LayerNode);

                DataSet dsTemps = m_bllAU_LayerNode.GetList(strWholeDbName, "");

                this.dataGridView1.DataSource = dsTemps.Tables[0];
                this.dataGridView1.Columns[1].Visible = false;
                this.dataGridView1.Columns[0].Width = this.Width;
            }
            catch (Exception ex)
            {
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!Directory.Exists(m_strDBFolder))
            {
                Directory.CreateDirectory(m_strDBFolder);
            }
            if (File.Exists(m_strDBFolder + this.toolStripTextBox1.Text + ".db"))
            {
                MessageBox.Show("该博客已存在，若要重新下载请先到WebSiteDB删除!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
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
            List<string> lstSite = new List<string>();
            if (!Directory.Exists(m_strDBFolder))
            {
                Directory.CreateDirectory(m_strDBFolder);
            }
            DirectoryInfo dir = new DirectoryInfo(m_strDBFolder);
            FileInfo[] dirSubs = dir.GetFiles();

            // 为每个子目录添加一个子节点
            foreach (FileInfo dirSub in dirSubs)
            {


                this.toolStripDropDownButton1.DropDownItems.Add(dirSub.Name.Replace(".db", ""));

            }
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

        private void toolStripDropDownButton1_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string strSite = e.ClickedItem.Text;
            string strWholeDbName = m_strDBConStringPath + strSite + ".db";
            DataSet dsTemps = m_bllAU_LayerNode.GetList(strWholeDbName, "");

            this.dataGridView1.DataSource = dsTemps.Tables[0];
            this.dataGridView1.Columns[1].Visible = false;
            this.dataGridView1.Columns[0].Width = this.Width;
        }


    }
}
