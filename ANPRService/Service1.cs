using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ANPRService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();



        }

        protected override void OnStart(string[] args)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = ConfigurationManager.AppSettings["Root"];

            watcher.NotifyFilter = NotifyFilters.LastAccess
            | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
            watcher.Filter = "*.txt";
            

            watcher.Created += new FileSystemEventHandler(OnCreated);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
        }

        protected override void OnStop()
        {
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {

            string[] FileName = e.Name.Split(',', '\\');

            string Camera = FileName[0];

            DateTime tempDate = DateTime.ParseExact(FileName[1] + FileName[2], "yyyyMMddHHmm", CultureInfo.InvariantCulture);
            string Timeseen = tempDate.ToString("yyyy-MM-dd HH:mm");

            DateTime tempDate2 = DateTime.ParseExact(FileName[3].Substring(1) + FileName[4], "ddMMyyyyHHmmssff", CultureInfo.InvariantCulture);
            string Timeprocessed = tempDate2.ToString("yyyy-MM-dd HH:mm:ss:ff");

            string License = FileName[5];
            
            SqlCommand cmd;
            SqlConnection con;
            con = new SqlConnection(@"Server=localhost\SQLEXPRESS02; Database=ANPR;User Id=admin1;Password=admin123");
            
            con.Open();

            cmd = new SqlCommand("INSERT INTO dbo.ANPR (Camera,Timeseen,Timeprocessed,License) VALUES (@Camera,@Timeseen,@Timeprocessed,@License)", con);
            cmd.Parameters.AddWithValue("@Camera",Camera);
            cmd.Parameters.AddWithValue("@Timeseen", Timeseen);
            cmd.Parameters.AddWithValue("@Timeprocessed", Timeprocessed);
            cmd.Parameters.AddWithValue("@License", License);
            cmd.ExecuteNonQuery();
            con.Close();
        }

    }
}
