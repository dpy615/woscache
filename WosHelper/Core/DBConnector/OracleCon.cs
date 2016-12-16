using System;
using System.Data.OracleClient;
using System.Data;
using Core.Entity;

namespace Core.DBConnector
{
    public class OracleCon
    {
        /// <summary>
        /// 检索库中的文章
        /// </summary>
        /// <param name="idStart">起始条目号</param>
        /// <param name="idEnd">截止条目号</param>
        /// <returns></returns>
        public static DataTable SelectArticles(int idStart, int idEnd)
        {
            DataTable dt = new DataTable();
            string sql = string.Format("select * from de_reference where INTERNAL_ID>={0} and INTERNAL_ID<{1} order by INTERNAL_ID", idStart, idEnd);
            dt = ExecuteSelect(sql);
            return dt;
        }


        #region oracle执行
        /// <summary>
        /// 执行查询语句结果
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private static DataTable ExecuteSelect(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                using (OracleConnection connection = new OracleConnection(SearcherTool.ConnectString))
                {
                    connection.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(sql, connection))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception e)
            {
                Logs.WriteLog(string.Format("{0}\r\n{1}", e.Message, sql));
                return null;
            }
            return dt;
        }

        /// <summary>
        /// 插入sql语句
        /// </summary>
        /// <param name="sql">insert语句</param>
        /// <returns></returns>
        public static bool ExecuteInsert(string sql)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(SearcherTool.ConnectString))
                {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                Logs.WriteLog(string.Format("{0}\r\n{1}", e.Message, sql));
                return false;
            }

        }

        internal static void SaveMatchData(WosData wosData, string title, string id)
        {
            string sql = string.Empty;
            try
            {
                sql = string.Format("insert into titlematch values({5},'{0}','{1}','{2}','{3}')",
               title,
               wosData.UT,
               wosData.getDataArray()[wosData.getDataArray().Length - 1],
               "",
               id);
                ExecuteInsert(sql);
            }
            catch (Exception e)
            {
                Logs.WriteLog(string.Format("{0}\r\n{1}", e.Message, sql));
            }
        }

        internal static void SaveWosData(WosData wosData)
        {
            string sql = string.Empty;
            try
            {
                sql = "insert into wosdata(PT,AU,BA,BE,GP,AF,BF,CA,TI,SO,SE,BS,LA,DT,CT,CY,CL,SP,HO,DE,ID,AB,C1,RP,EM,RI,OI,FU,FX,CR,NR,TC,Z9,U1,U2,PU,PI,PA,SN,EI,BN,J9,JI,PD,PY,VL,\"IS\",PN,SU,SI,MA,BP,EP,AR,DI,D2,PG,WC,SC,GA,UT,PM) values(";
                sql += GetInsertStr(wosData);
                sql += ")";
                ExecuteInsert(sql);
            }
            catch (Exception e)
            {
                Logs.WriteLog(string.Format("{0}\r\n{1}", e.Message, sql));
            }
        }

        public static string GetInsertStr(WosData wosData)
        {
            string[] datas = wosData.getDataArray();
            string sReturn = "'";
            for (int i = 0; i < 62; i++)
            {
                if (i != 61)
                {
                    sReturn += datas[i].Replace("'", "\\'").Replace("\"", "\\\"") + "','";
                }
                else
                {
                    sReturn += datas[i].Replace("'", "\\'").Replace("\"", "\\\"") + "'";
                }
            }
            return sReturn;
        }

        /// <summary>
        /// 将DataSet数据表插入到oracle库中
        /// </summary>
        /// <param name="ds"></param>
        public static bool ExecuteInsert(DataSet ds)
        {
            if (ds.Tables.Count > 0)
            {
                OracleConnection connection = new OracleConnection(SearcherTool.ConnectString);
                connection.Open();
                var trans = connection.BeginTransaction();
                string dataSetName = ds.DataSetName;
                try
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        string tableName = dt.TableName;
                        OracleCommand cmd = new OracleCommand(string.Format("select * from {0} where rownum < 1", tableName), connection);
                        cmd.Transaction = trans;
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        OracleCommandBuilder cb = new OracleCommandBuilder(adapter);
                        adapter.Update(dt);
                    }
                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    Logs.WriteLog(string.Format("插入数据失败：{0} :{1}", dataSetName, e.Message));
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion
    }
}
