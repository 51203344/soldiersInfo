using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SoldiersInfo.Controllers
{
    public class Import
    {
        static public string ImportData(string filepath, int countOnSolider)
        {
            string mess = "init";
            string destinyTableName = "Soldiers";
            //Create connection
            string fileExcelConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source =" + filepath + "; Extended Properties=" + "'Excel 12.0 Xml;HDR=YES;'";
            OleDbConnection oledbConnection = new OleDbConnection(fileExcelConnectionString);
            oledbConnection.Open();
            
            DataTable currentTable = oledbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (currentTable != null)
            {
                //take sheet name.

                string sheetName = currentTable.Rows[0]["TABLE_NAME"].ToString(); // lấy sheet đầu tiên theo thứ tự tên

                if (sheetName.Contains('\''))
                {
                    sheetName = sheetName.Replace('\'', ' ').Trim();
                }
                string queryFromExcel = "select lastname,middleName,firstName,birthday,company,servingDate,pointDate,note,annouce,isDisplay from [" + sheetName + "]";
                OleDbCommand oledbCommand = new OleDbCommand(queryFromExcel, oledbConnection);
                OleDbDataAdapter oledbAdapter = new OleDbDataAdapter(oledbCommand);
                DataTable dataTable = new DataTable();
               // DataSet objDataset1 = new DataSet();
                oledbAdapter.Fill(dataTable);

                String dbConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SoldiersInfoContext"].ConnectionString;
                SqlConnection sqlConnection = new SqlConnection(dbConnectionString);

                SqlBulkCopy bulkcopy = new SqlBulkCopy(sqlConnection.ConnectionString);
                bulkcopy.DestinationTableName = destinyTableName;
                bulkcopy.BatchSize = dataTable.Rows.Count;

                DataTable dataTableToBeFilled = modifySource(dataTable, countOnSolider);
                try
                {
                    bulkcopy.WriteToServer(dataTableToBeFilled);
                }
                catch (Exception ex)
                {
                    mess += " " + ex.ToString();
                }
                
                oledbConnection.Close();
                sqlConnection.Close();
                mess += " copy done";
            }
            return mess;
        }
        static private DataTable modifySource(DataTable dataTable, int countOnSoldier)
        {
            DataTable result = new DataTable();
            DataColumn dc;
            DataRow dr;

            // cột ID
            dc = new DataColumn();
            dc.DataType = Type.GetType("System.Int32");
            dc.ColumnName = "ID";
            result.Columns.Add(dc);
            // cột lastName
            dc = new DataColumn();
            dc.DataType = Type.GetType("System.String");
            dc.ColumnName = "lastName";
            result.Columns.Add(dc);

            // cột middlename
            dc = new DataColumn();
            dc.DataType = Type.GetType("System.String");
            dc.ColumnName = "middleName";
            result.Columns.Add(dc);

            // cột firstname
            dc = new DataColumn();
            dc.DataType = Type.GetType("System.String");
            dc.ColumnName = "firstName";
            result.Columns.Add(dc);

            // cột birthday 
            dc = new DataColumn();
            dc.DataType = Type.GetType("System.DateTime");
            dc.ColumnName = "birthday";
            result.Columns.Add(dc);

            // cột company
            dc = new DataColumn();
            dc.DataType = Type.GetType("System.String");
            dc.ColumnName = "company";
            result.Columns.Add(dc);

            // cột servingDate
            dc = new DataColumn();
            dc.DataType = Type.GetType("System.DateTime");
            dc.ColumnName = "servingDate";
            result.Columns.Add(dc);

            // cột pointDate
            dc = new DataColumn();
            dc.DataType = Type.GetType("System.DateTime");
            dc.ColumnName = "pointDate";
            result.Columns.Add(dc);

            // cột note
            dc = new DataColumn();
            dc.DataType = Type.GetType("System.String");
            dc.ColumnName = "note";
            result.Columns.Add(dc);
            // cột annouce
            dc = new DataColumn();
            dc.DataType = Type.GetType("System.Int32");
            dc.ColumnName = "annouce";
            result.Columns.Add(dc);

            // cột isDisplay
            dc = new DataColumn();
            dc.DataType = Type.GetType("System.Boolean");
            dc.ColumnName = "isDisplay";
            result.Columns.Add(dc);
            foreach (DataRow row in dataTable.Rows)
            {
                dr = result.NewRow();
                dr["ID"] = countOnSoldier++;
                dr["lastName"] = row.ItemArray[0].ToString();
                dr["middleName"] = row.ItemArray[1].ToString();
                dr["firstName"] = row.ItemArray[2].ToString();
                dr["birthday"] = row.Field<DateTime>("birthday");
                dr["company"] = row.ItemArray[4].ToString();
                dr["servingDate"] = row.Field<DateTime>("servingDate");
                dr["pointDate"] = row.Field<DateTime>("pointDate");
                dr["note"] = row.ItemArray[7].ToString();
                dr["annouce"] = (row.ItemArray[8].ToString() == "0") ? 0 : 1;
                dr["isDisplay"] = (row.ItemArray[9].ToString().ToUpper() == "TRUE") ? true : false;
                result.Rows.Add(dr);
            }
            return result;
        }
    }
}