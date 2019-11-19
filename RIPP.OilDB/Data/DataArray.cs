using System;
using System.Data;
using RIPP.OilDB.Model;

namespace RIPP.OilDB.Data
{
    public class DataArray
    {
        public static DataSet ToDataSet(object[] sourceArray)
        {
            Type baseType = sourceArray.GetType().GetElementType();

            DataColumn dc = new DataColumn();
            dc.DataType = baseType;

            DataTable dt = new DataTable();
            dt.Columns.Add(dc);

            for (int currentIndex = 0; currentIndex < sourceArray.Length; currentIndex++)
                dt.Rows.Add(new object[] { sourceArray[currentIndex] });

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;

        }

        public static DataSet ToDataSet(object[,] sourceArray)
        {
            Type baseType = sourceArray.GetType().GetElementType();
            int rowCount = sourceArray.GetLength(0);
            int columnCount = sourceArray.GetLength(1);

            DataTable dt = new DataTable();
            DataColumn[] dca = new DataColumn[columnCount];

            for (int currentColumnIndex = 0; currentColumnIndex < columnCount; currentColumnIndex++)
            {
                dca[currentColumnIndex] = new DataColumn();
                dca[currentColumnIndex].DataType = baseType;
            }
            dt.Columns.AddRange(dca);

            for (int currentRowIndex = 0; currentRowIndex < rowCount; currentRowIndex++)
            {
                object[] rowArr = new object[columnCount];
                for (int currentColumnIndex = 0; currentColumnIndex < columnCount; currentColumnIndex++)
                    rowArr[currentColumnIndex] = sourceArray.GetValue(currentRowIndex, currentColumnIndex);
                dt.Rows.Add(rowArr);
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;

        }

        public static DataSet ToDataSet(object[, ,] sourceArray)
        {
            Type baseType = sourceArray.GetType().GetElementType();
            int tableCount = sourceArray.GetLength(0);
            int rowCount = sourceArray.GetLength(1);
            int columnCount = sourceArray.GetLength(2);
            DataSet ds = new DataSet();

            for (int currentTableIndex = 0; currentTableIndex < tableCount; currentTableIndex++)
            {
                DataTable dt = new DataTable();
                DataColumn[] dca = new DataColumn[columnCount];

                for (int currentColumnIndex = 0; currentColumnIndex < columnCount; currentColumnIndex++)
                {
                    dca[currentColumnIndex] = new DataColumn();
                    dca[currentColumnIndex].DataType = baseType;
                }
                dt.Columns.AddRange(dca);

                for (int currentRowIndex = 0; currentRowIndex < rowCount; currentRowIndex++)
                {
                    object[] rowArr = new object[columnCount];
                    for (int currentColumnIndex = 0; currentColumnIndex < columnCount; currentColumnIndex++)
                        rowArr[currentColumnIndex] = sourceArray.GetValue(currentTableIndex, currentRowIndex, currentColumnIndex);
                    dt.Rows.Add(rowArr);
                }
                ds.Tables.Add(dt);
            }
            return ds;

        }
    }


}
