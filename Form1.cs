using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.ComponentModel;

namespace DataConvert
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            ComboBoxDoldur(ref comboBox1, "SELECT [Object_id] as ID,[name] FROM sys.objects WHERE Type='U' order by [name]", "--Tablo Adları--", "name");
            comboBox1.Items.Add("--Tabloları Temizle--");
            comboBox1.Items.Add("--LOG Dosyası Temizle--");
            Application.DoEvents();
            comboBox1.SelectedIndex = 0;
            Cursor = Cursors.Default;
        }
        public  DataTable TabloDoldur(string TamSorgu, SqlParameter[] pCollection)
        {

            DataTable dt = new DataTable();

            SqlDataAdapter da = new SqlDataAdapter(TamSorgu, Baglanti());
            da.SelectCommand.Parameters.AddRange(pCollection);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            try
            {
                da.Fill(dt);
            }
            catch (Exception e)
            {
                // dt = null;
                // DigerIslemler.HataYaz("AKBclass-TabloDoldur-2", e.Message, "");
            }
            da.SelectCommand.Parameters.Clear();
            System.GC.SuppressFinalize(da);
            return dt;

        }
        
        
        
        
        
        public DataTable TabloDoldur(string TamSorgu)
        {

            DataTable dt = new DataTable();

            SqlDataAdapter da = new SqlDataAdapter(TamSorgu, Baglanti());

            try
            {
                da.Fill(dt);
            }
            catch (Exception e)
            {
             
            }

            System.GC.SuppressFinalize(da);
            return dt;

        }

        public  void ComboBoxDoldur(ref ComboBox C, string Sorgu, string IlkDeger, string ColumAd)
        {
            C.Items.Clear();
            C.Items.Add(IlkDeger);

            try
            {
                DataTable dt =TabloDoldur(Sorgu);

                for (int i = 0; i < dt.Rows.Count; i++)
                    C.Items.Add(dt.Rows[i][ColumAd].ToString());
            }
            catch (Exception e)
            {
            
            }
        }
        public SqlConnection Baglanti()
        {

            return new SqlConnection(Txt1.Text + Txt2.Text + Txt3.Text + Txt4.Text + Txt5.Text + Txt6.Text + Txt7.Text + Txt8.Text);

        }
        public string parametre(string columnName, string ColumnType, string ColumnSize)
        {
            return String.Format("new SqlParameter(\"@{0}\",{1}{2}),", columnName, ColumnType, (string.IsNullOrEmpty(ColumnSize) ? "" : "," + ColumnSize));
        
        }

     
        
        void TabloClass()
        {

            textBox5.Text += "\r\n\r\n------------------------------- Tablo Class ------------------------------- \r\n\r\n    ";

            textBox5.Text += "public class " + comboBox1.SelectedItem.ToString() + "\r\n    {";
            string str = "\r\n\r\n     private {0} _{1}; \r\n      public {0} {1} \r\n      { \r\n      get { return _{1}; } \r\n      set { _{1} = value; } \r\n      }";
       
        
            try
            {
                DataTable dt = TabloDoldur("select * from " + comboBox1.SelectedItem.ToString());
                string dd;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    dd = CevirCS(dt.Columns[i]);
                    textBox5.Text += "\r\n\r\n         private " + dd.Split('|')[0].ToString() + " _" + dd.Split('|')[1].ToString() + "; \r\n         public " + dd.Split('|')[0].ToString() + " " + dd.Split('|')[1].ToString() + " \r\n         { \r\n           get { return _" + dd.Split('|')[1].ToString() + "; } \r\n           set { _" + dd.Split('|')[1].ToString() + " = value; } \r\n         }";

                }

                textBox5.Text += "\r\n\r\n    public "+comboBox1.SelectedItem.ToString()+"()	\r\n    {  } \r\n\r\n    } \r\n    ";
            }
            catch (Exception e)
            {

            }
            textBox5.Focus();
        }

        public string CevirCS(DataColumn cc)
        {


            string columnName = cc.Caption.ToString(), ColumnType = "";
            string exception = String.Format("{0} bunda hata var yaa ", cc.Caption.ToString());

            switch (cc.DataType.FullName)
            {
                case "System.Int64":
                    
                    ColumnType = "long?";
                    
                    break;
                case "System.Boolean":
                    ColumnType = "bool?";
                    break;
                case "System.String":
                    ColumnType = "string";
                    break;
                case "System.DateTime":
                    ColumnType = "DateTime?";
                    break;
                case "System.Decimal":
                    ColumnType = "decimal?";
                    break;
                case "System.Double":
                    ColumnType = "double?";
                    break;
                case "System.Byte[]":
                    ColumnType = "byte?[]";
                    break;
                case "System.Byte":
                    ColumnType = "byte?";
                    break;
                case "System.Int32":
                    ColumnType = "int?";
                    break;
                case "System.Single":
                    ColumnType = "Single";
                    break;
                case "System.Int16":
                    ColumnType = "short?";
                    break;
                case "System.Guid":
                    ColumnType = "Guid";
                    break;
                case "System.Object":
                    ColumnType = "object";
                    break;
                default:
                    throw new ArgumentException(exception);
            }

            return ColumnType + "|" + columnName;

        }

        public string Cevir(DataColumn cc)
        {


            string columnName = cc.Caption.ToString(), ColumnType = "", ColumnSize = "";
            string exception = String.Format("{0} bunda hata var yaa ", cc.Caption.ToString());

            switch (cc.DataType.FullName)
            {
                case "System.Int64":

                    ColumnType = "SqlDbType.BigInt";

                    break;
                case "System.Boolean":
                    ColumnType = "SqlDbType.Bit";
                    break;
                case "System.String":
                    if (TabloDoldur("sp_columns", new SqlParameter[] { new SqlParameter("@table_name", comboBox1.SelectedItem.ToString()), new SqlParameter("@column_name", cc.Caption.ToString()) }).Rows[0]["TYPE_NAME"].ToString() == "text")
                        ColumnType = "SqlDbType.Text";
                    else if (TabloDoldur("sp_columns", new SqlParameter[] { new SqlParameter("@table_name", comboBox1.SelectedItem.ToString()), new SqlParameter("@column_name", cc.Caption.ToString()) }).Rows[0]["TYPE_NAME"].ToString() == "ntext")
                        ColumnType = "SqlDbType.NText";
                    else if (TabloDoldur("sp_columns", new SqlParameter[] { new SqlParameter("@table_name", comboBox1.SelectedItem.ToString()), new SqlParameter("@column_name", cc.Caption.ToString()) }).Rows[0]["TYPE_NAME"].ToString() == "nvarchar")
                        ColumnType = "SqlDbType.NVarChar (" + TabloDoldur("sp_columns", new SqlParameter[] { new SqlParameter("@table_name", comboBox1.SelectedItem.ToString()), new SqlParameter("@column_name", cc.Caption.ToString()) }).Rows[0]["PRECISION"].ToString() + ") ";
                    else if (TabloDoldur("sp_columns", new SqlParameter[] { new SqlParameter("@table_name", comboBox1.SelectedItem.ToString()), new SqlParameter("@column_name", cc.Caption.ToString()) }).Rows[0]["TYPE_NAME"].ToString() == "varchar")
                        ColumnType = "SqlDbType.VarChar (" + TabloDoldur("sp_columns", new SqlParameter[] { new SqlParameter("@table_name", comboBox1.SelectedItem.ToString()), new SqlParameter("@column_name", cc.Caption.ToString()) }).Rows[0]["PRECISION"].ToString() + ") ";
                    break;
                case "System.DateTime":
                    if (TabloDoldur("sp_columns", new SqlParameter[] { new SqlParameter("@table_name", comboBox1.SelectedItem.ToString()), new SqlParameter("@column_name", cc.Caption.ToString()) }).Rows[0]["TYPE_NAME"].ToString() == "smalldatetime")
                        ColumnType = "SqlDbType.SmallDateTime";
                    else
                        ColumnType = "SqlDbType.DateTime";
                    break;
                case "System.Decimal":
                    ColumnType = "SqlDbType.Decimal";
                    break;
                case "System.Double":
                    ColumnType = "SqlDbType.Float";
                    break;
                case "System.Byte[]":
                    ColumnType = "SqlDbType.VarBinary";
                    break;
                case "System.Byte":
                    ColumnType = "SqlDbType.TinyInt";
                    break;
                case "System.Int32":
                    ColumnType = "SqlDbType.Int";
                    break;
                case "System.Single":
                    ColumnType = "SqlDbType.Real";
                    break;
                case "System.Int16":
                    ColumnType = "SqlDbType.SmallInt";
                    break;
                case "System.Guid":
                    ColumnType = "SqlDbType.UniqueIdentifier";
                    break;
                case "System.Object":
                    ColumnType = "SqlDbType.Variant";
                    break;
                default:
                    throw new ArgumentException(exception);
            }

            return parametre(columnName, ColumnType, ColumnSize);

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            switch (comboBox1.SelectedItem.ToString())
            {
                case "--Tabloları Temizle--":
                    TablolariTemizle();
                    break;
                case "--LOG Dosyası Temizle--":
                    LogDosyasiTemizle();
                    break;
                default:
                    TabloSecildi();
                    break;
            }
            Cursor = Cursors.Default;
        }
        void LogDosyasiTemizle()
        {
            textBox5.Text = "";

            string str = "\r\n\r\n     USE {0} ; \r\n\r\n      GO \r\n\r\n     ALTER DATABASE {0} \r\n\r\n     SET RECOVERY SIMPLE; \r\n\r\n     GO \r\n\r\n     DBCC SHRINKFILE (2, 1); \r\n\r\n     GO \r\n\r\n     ALTER DATABASE {0} \r\n\r\n     SET RECOVERY FULL; \r\n\r\n      GO";


            textBox5.Text += string.Format(str, Txt2.Text);

            textBox5.Focus();

        }
        void TablolariTemizle()
        {
            textBox5.Text = "";

            string str = "\r\n\r\n     DELETE FROM {0};\r\n     DBCC CHECKIDENT('{0}', RESEED, 0);  ";

            try
            {
                DataTable dt = TabloDoldur("SELECT [Object_id] as ID,[name] FROM sys.objects WHERE Type='U' order by [name]");

                for (int i = 0; i < dt.Rows.Count; i++)
                    textBox5.Text += string.Format(str, dt.Rows[i]["name"].ToString());

              
            }
            catch (Exception e)
            {

            }
            textBox5.Focus();
        }
        void TabloSecildi()
        {

            DataTable dt = TabloDoldur("select * from " + comboBox1.SelectedItem.ToString());
            string InsertSql = "     INSERT INTO " + comboBox1.SelectedItem.ToString() + "(";
            string UpdateSql = "     UPDATE " + comboBox1.SelectedItem.ToString() + " SET ";
            string DeleteSql = "     DELETE FROM " + comboBox1.SelectedItem.ToString() + " WHERE ID = @ID";
            string InsertCreateSP = "     CREATE PROCEDURE " + (comboBox1.SelectedItem.ToString().Split('_'))[0] + "_Kaydet ";
            string UpdateCreateSP = "     CREATE PROCEDURE " + (comboBox1.SelectedItem.ToString().Split('_'))[0] + "_Guncelle ";
            string ColumnName = "";
            string ParametreName = "";
            string UpdateColumnParamater = "";
            string pCollectionValues = "\r\n     foreach (SqlParameter item in pCollection)     \r\n     {     \r\n         item.Value = DBNull.Value;     \r\n     }     \r\n\r\n";

            
           
            


            textBox5.Text = "\r\n     \r\n     \r\n     \r\n     SqlParameter[] pCollection = new SqlParameter[]" + "\r\n"
                           + "     {" + "\r\n";
            InsertCreateSP += "\r\n     (\r\n";
            UpdateCreateSP += "\r\n     (\r\n";
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (!string.IsNullOrEmpty(ColumnName))
                {
                    ColumnName += ",";
                    ParametreName += ",";
                    UpdateColumnParamater += ",";

                }
                if (dt.Columns[i].Caption.ToString() != "ID")
                {
                    UpdateColumnParamater += dt.Columns[i].Caption.ToString() + " = @" + dt.Columns[i].Caption.ToString();
                    ColumnName += dt.Columns[i].Caption.ToString();
                    ParametreName += "@" + dt.Columns[i].Caption.ToString();
                }
                textBox5.Text += "          /*" + i.ToString() + "*/" + Cevir(dt.Columns[i]).Replace(" (", ", ").Replace(") ", "") + "\r\n";
                pCollectionValues += "     /*" + dt.Columns[i].Caption.ToString() + "*/" + "  pCollection[" + i.ToString() + "].Value = DBNull.Value; " + "\r\n";
                if (i + 1 < dt.Columns.Count)
                {
                    if (dt.Columns[i].Caption.ToString() != "ID")
                        InsertCreateSP += "          @" + dt.Columns[i].Caption.ToString() + " " + Cevir(dt.Columns[i]).Split('.')[1].Replace("),", "") + ",\r\n";

                    UpdateCreateSP += "          @" + dt.Columns[i].Caption.ToString() + " " + Cevir(dt.Columns[i]).Split('.')[1].Replace("),", "") + ",\r\n";
                }
                else
                {
                    if (dt.Columns[i].Caption.ToString() != "ID")
                        InsertCreateSP += "          @" + dt.Columns[i].Caption.ToString() + " " + Cevir(dt.Columns[i]).Split('.')[1].Replace("),", "") + "\r\n";

                    UpdateCreateSP += "          @" + dt.Columns[i].Caption.ToString() + " " + Cevir(dt.Columns[i]).Split('.')[1].Replace("),", "") + "\r\n";
                }
            }
            InsertCreateSP += "     )\r\n     AS \r\n     \r\n";
            UpdateCreateSP += "     )\r\n     AS \r\n     \r\n";

            textBox5.Text += "     };" + "\r\n     ";
            textBox5.Text += pCollectionValues;





            textBox5.Text += "\r\n     ";
            textBox5.Text += "\r\n     ";
            textBox5.Text += "\r\n     ";

            InsertSql += ColumnName + ") VALUES (" + ParametreName + ")";
            UpdateSql += UpdateColumnParamater + " WHERE ID = @ID";

            InsertCreateSP += InsertSql + "\r\n     \r\n     SELECT @@IDENTITY AS ID ";
            UpdateCreateSP += UpdateSql + "\r\n     \r\n     SELECT 'X'  ";

            textBox5.Text += "\r\n\r\n------------------------------- Ekle Sql ------------------------------- \r\n\r\n     " + InsertSql + "\r\n     " + "\r\n     ";
            textBox5.Text += "\r\n\r\n------------------------------- Güncelle Sql ------------------------------- \r\n\r\n     " + UpdateSql + "\r\n     " + "\r\n     ";
            textBox5.Text += "\r\n\r\n------------------------------- Sil Sql ------------------------------- \r\n\r\n     " + DeleteSql + "\r\n     " + "\r\n     ";
            textBox5.Text += "\r\n\r\n\r\n\r\n ------------------------------- Ekle SP -------------------------------\r\n\r\n";
            textBox5.Text += InsertCreateSP + "\r\n     " + "\r\n     ";
            textBox5.Text += "\r\n\r\n\r\n\r\n ------------------------------- Güncelle SP -------------------------------\r\n\r\n";
            textBox5.Text += UpdateCreateSP + "\r\n     " + "\r\n     \r\n     \r\n     \r\n     \r\n     \r\n     ";

            TabloClass();
        
        }

        private void splitContainer1_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            textBox5.Focus();
        }

        
    }
}
