using System;
using System.Data.OracleClient;
using System.Data;
using Core.Entity;

namespace Core.DBConnector {
    public class OracleCon {
        /// <summary>
        /// 检索库中的文章
        /// </summary>
        /// <param name="idStart">起始条目号</param>
        /// <param name="idEnd">截止条目号</param>
        /// <returns></returns>
        public static DataTable SelectArticles(int idStart, int idEnd) {
            DataTable dt = new DataTable();
            string sql = string.Format("select * from de_reference where INTERNAL_ID>={0} and INTERNAL_ID<{1}   and citation_type='J' order by INTERNAL_ID", idStart, idEnd);
            dt = ExecuteSelect(sql);
            return dt;
        }


        #region oracle执行
        /// <summary>
        /// 执行查询语句结果
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable ExecuteSelect(string sql) {
            DataTable dt = new DataTable();
            try {
                using (OracleConnection connection = new OracleConnection(SearcherTool.ConnectString)) {
                    connection.Open();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(sql, connection)) {
                        adapter.Fill(dt);
                    }
                }
            } catch (Exception e) {
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
        public static bool ExecuteInsert(string sql) {
            try {
                using (OracleConnection connection = new OracleConnection(SearcherTool.ConnectString)) {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(sql, connection)) {
                        command.ExecuteNonQuery();
                    }
                    return true;
                }
            } catch (Exception e) {
                Logs.WriteLog(string.Format("{0}\r\n{1}", e.Message, sql));
                return false;
            }

        }

        internal static void SaveMatchData(WosData wosData, string title, string id) {
            string sql = string.Empty;
            try {
                sql = string.Format("insert into wos_match(title_id,title,ut,titlematchvalue,yearmatch) values({0},'{1}','{2}','{3}','{4}')",
               id, title.Replace("'","''"),
               wosData.UT,
               wosData.getDataArray()[wosData.getDataArray().Length - 1],
               "-1");
                ExecuteInsert(sql);
            } catch (Exception e) {
                Logs.WriteLog(string.Format("{0}\r\n{1}", e.Message, sql));
                throw e;
            }
        }

        internal static void SaveMatchData(string UT, string title, string id,string matchValue)
        {
            string sql = string.Empty;
            try
            {
                sql = string.Format("insert into wos_match(title_id,title,ut,titlematchvalue,yearmatch) values({0},'{1}','{2}','{3}','{4}')",
               id, title.Replace("'", "''"),
               UT,
               matchValue,
               "-1");
                ExecuteInsert(sql);
            }
            catch (Exception e)
            {
                Logs.WriteLog(string.Format("{0}\r\n{1}", e.Message, sql));
                throw e;
            }
        }

        internal static void SaveMatchDataError(string title, string id, string errormessage) {
            string sql = string.Empty;
            try {
                sql = string.Format("insert into wos_matcherror(title_id,title,errormessage) values({0},'{1}','{2}')",
               id, title.Replace("'", "''"), errormessage);
                ExecuteInsert(sql);
            } catch (Exception e) {
                Logs.WriteLog(string.Format("{0}\r\n{1}", e.Message, sql));
            }
        }


        internal static void SaveWosData(WosData wosData) {
            string sql = string.Empty;
            try {
                sql = "insert into wos_data(PT,AU,BA,BE,GP,AF,BF,CA,TI,SO,SE,BS,LA,DT,CT,CY,CL,SP,HO,DE,ID,AB,C1,RP,EM,RI,OI,FU,FX,CR,NR,TC,Z9,U1,U2,PU,PI,PA,SN,EI,BN,J9,JI,PD,PY,VL,ISS,PN,SU,SI,MA,BP,EP,AR,DI,D2,PG,WC,SC,GA,UT,PM) values(";
                sql += GetInsertStr(wosData);
                sql += ")";
                ExecuteInsert(sql);
            } catch (Exception e) {
                Logs.WriteLog(string.Format("{0}\r\n{1}", e.Message, sql));
            }
        }



        internal static void SaveWosDataLong(WosData wosData) {
            string sql = string.Empty;
            try {
                sql = "insert into wos_data(PT,AU,BA,BE,GP,AF,BF,CA,TI,SO,SE,BS,LA,DT,CT,CY,CL,SP,HO,DE,ID,AB,C1,RP,EM,RI,OI,FU,FX,CR,NR,TC,Z9,U1,U2,PU,PI,PA,SN,EI,BN,J9,JI,PD,PY,VL,ISS,PN,SU,SI,MA,BP,EP,AR,DI,D2,PG,WC,SC,GA,UT,PM) values(:PT,:AU,:BA,:BE,:GP,:AF,:BF,:CA,:TI,:SO,:SE,:BS,:LA,:DT,:CT,:CY,:CL,:SP,:HO,:DE,:ID,:AB,:C1,:RP,:EM,:RI,:OI,:FU,:FX,:CR,:NR,:TC,:Z9,:U1,:U2,:PU,:PI,:PA,:SN,:EI,:BN,:J9,:JI,:PD,:PY,:VL,:ISS,:PN,:SU,:SI,:MA,:BP,:EP,:AR,:DI,:D2,:PG,:WC,:SC,:GA,:UT,:PM)";

                using (OracleConnection connection = new OracleConnection(SearcherTool.ConnectString)) {
                    connection.Open();
                    using (OracleCommand command = new OracleCommand(sql, connection)) {
                        if (string.IsNullOrEmpty(wosData.PT)) { wosData.PT = " "; }
                        if (string.IsNullOrEmpty(wosData.AU)) { wosData.AU = " "; }
                        if (string.IsNullOrEmpty(wosData.BA)) { wosData.BA = " "; }
                        if (string.IsNullOrEmpty(wosData.BE)) { wosData.BE = " "; }
                        if (string.IsNullOrEmpty(wosData.GP)) { wosData.GP = " "; }
                        if (string.IsNullOrEmpty(wosData.AF)) { wosData.AF = " "; }
                        if (string.IsNullOrEmpty(wosData.BF)) { wosData.BF = " "; }
                        if (string.IsNullOrEmpty(wosData.CA)) { wosData.CA = " "; }
                        if (string.IsNullOrEmpty(wosData.TI)) { wosData.TI = " "; }
                        if (string.IsNullOrEmpty(wosData.SO)) { wosData.SO = " "; }
                        if (string.IsNullOrEmpty(wosData.SE)) { wosData.SE = " "; }
                        if (string.IsNullOrEmpty(wosData.BS)) { wosData.BS = " "; }
                        if (string.IsNullOrEmpty(wosData.LA)) { wosData.LA = " "; }
                        if (string.IsNullOrEmpty(wosData.DT)) { wosData.DT = " "; }
                        if (string.IsNullOrEmpty(wosData.CT)) { wosData.CT = " "; }
                        if (string.IsNullOrEmpty(wosData.CY)) { wosData.CY = " "; }
                        if (string.IsNullOrEmpty(wosData.CL)) { wosData.CL = " "; }
                        if (string.IsNullOrEmpty(wosData.SP)) { wosData.SP = " "; }
                        if (string.IsNullOrEmpty(wosData.HO)) { wosData.HO = " "; }
                        if (string.IsNullOrEmpty(wosData.DE)) { wosData.DE = " "; }
                        if (string.IsNullOrEmpty(wosData.ID)) { wosData.ID = " "; }
                        if (string.IsNullOrEmpty(wosData.AB)) { wosData.AB = " "; }
                        if (string.IsNullOrEmpty(wosData.C1)) { wosData.C1 = " "; }
                        if (string.IsNullOrEmpty(wosData.RP)) { wosData.RP = " "; }
                        if (string.IsNullOrEmpty(wosData.EM)) { wosData.EM = " "; }
                        if (string.IsNullOrEmpty(wosData.RI)) { wosData.RI = " "; }
                        if (string.IsNullOrEmpty(wosData.OI)) { wosData.OI = " "; }
                        if (string.IsNullOrEmpty(wosData.FU)) { wosData.FU = " "; }
                        if (string.IsNullOrEmpty(wosData.FX)) { wosData.FX = " "; }
                        if (string.IsNullOrEmpty(wosData.CR)) { wosData.CR = " "; }
                        if (string.IsNullOrEmpty(wosData.NR)) { wosData.NR = " "; }
                        if (string.IsNullOrEmpty(wosData.TC)) { wosData.TC = " "; }
                        if (string.IsNullOrEmpty(wosData.Z9)) { wosData.Z9 = " "; }
                        if (string.IsNullOrEmpty(wosData.U1)) { wosData.U1 = " "; }
                        if (string.IsNullOrEmpty(wosData.U2)) { wosData.U2 = " "; }
                        if (string.IsNullOrEmpty(wosData.PU)) { wosData.PU = " "; }
                        if (string.IsNullOrEmpty(wosData.PI)) { wosData.PI = " "; }
                        if (string.IsNullOrEmpty(wosData.PA)) { wosData.PA = " "; }
                        if (string.IsNullOrEmpty(wosData.SN)) { wosData.SN = " "; }
                        if (string.IsNullOrEmpty(wosData.EI)) { wosData.EI = " "; }
                        if (string.IsNullOrEmpty(wosData.BN)) { wosData.BN = " "; }
                        if (string.IsNullOrEmpty(wosData.J9)) { wosData.J9 = " "; }
                        if (string.IsNullOrEmpty(wosData.JI)) { wosData.JI = " "; }
                        if (string.IsNullOrEmpty(wosData.PD)) { wosData.PD = " "; }
                        if (string.IsNullOrEmpty(wosData.PY)) { wosData.PY = " "; }
                        if (string.IsNullOrEmpty(wosData.VL)) { wosData.VL = " "; }
                        if (string.IsNullOrEmpty(wosData.IS)) { wosData.IS = " "; }
                        if (string.IsNullOrEmpty(wosData.PN)) { wosData.PN = " "; }
                        if (string.IsNullOrEmpty(wosData.SU)) { wosData.SU = " "; }
                        if (string.IsNullOrEmpty(wosData.SI)) { wosData.SI = " "; }
                        if (string.IsNullOrEmpty(wosData.MA)) { wosData.MA = " "; }
                        if (string.IsNullOrEmpty(wosData.BP)) { wosData.BP = " "; }
                        if (string.IsNullOrEmpty(wosData.EP)) { wosData.EP = " "; }
                        if (string.IsNullOrEmpty(wosData.AR)) { wosData.AR = " "; }
                        if (string.IsNullOrEmpty(wosData.DI)) { wosData.DI = " "; }
                        if (string.IsNullOrEmpty(wosData.D2)) { wosData.D2 = " "; }
                        if (string.IsNullOrEmpty(wosData.PG)) { wosData.PG = " "; }
                        if (string.IsNullOrEmpty(wosData.WC)) { wosData.WC = " "; }
                        if (string.IsNullOrEmpty(wosData.SC)) { wosData.SC = " "; }
                        if (string.IsNullOrEmpty(wosData.GA)) { wosData.GA = " "; }
                        if (string.IsNullOrEmpty(wosData.UT)) { wosData.UT = " "; }
                        if (string.IsNullOrEmpty(wosData.PM)) { wosData.PM = " "; }

                        OracleParameter parm = new OracleParameter("PT", wosData.PT.Replace("'", "''"));
                        command.Parameters.Add(parm);
                        OracleParameter parm1 = new OracleParameter("AU", OracleType.Clob);
                        parm1.Value = wosData.AU.Replace("'", "''");
                        command.Parameters.Add(parm1);
                        OracleParameter parm2 = new OracleParameter("BA", wosData.BA.Replace("'", "''"));
                        command.Parameters.Add(parm2);
                        OracleParameter parm3 = new OracleParameter("BE", wosData.BE.Replace("'", "''"));
                        command.Parameters.Add(parm3);
                        OracleParameter parm4 = new OracleParameter("GP", wosData.GP.Replace("'", "''"));
                        command.Parameters.Add(parm4);
                        OracleParameter parm5 = new OracleParameter("AF", OracleType.Clob);
                        parm5.Value = wosData.AF.Replace("'", "''");
                        command.Parameters.Add(parm5);
                        OracleParameter parm6 = new OracleParameter("BF", wosData.BF.Replace("'", "''"));
                        command.Parameters.Add(parm6);
                        OracleParameter parm7 = new OracleParameter("CA", wosData.CA.Replace("'", "''"));
                        command.Parameters.Add(parm7);
                        OracleParameter parm8 = new OracleParameter("TI", OracleType.Clob);
                        parm8.Value = wosData.TI.Replace("'", "''");
                        command.Parameters.Add(parm8);
                        OracleParameter parm9 = new OracleParameter("SO", wosData.SO.Replace("'", "''"));
                        command.Parameters.Add(parm9);
                        OracleParameter parm10 = new OracleParameter("SE", wosData.SE.Replace("'", "''"));
                        command.Parameters.Add(parm10);
                        OracleParameter parm11 = new OracleParameter("BS", wosData.BS.Replace("'", "''"));
                        command.Parameters.Add(parm11);
                        OracleParameter parm12 = new OracleParameter("LA", wosData.LA.Replace("'", "''"));
                        command.Parameters.Add(parm12);
                        OracleParameter parm13 = new OracleParameter("DT", wosData.DT.Replace("'", "''"));
                        command.Parameters.Add(parm13);
                        OracleParameter parm14 = new OracleParameter("CT", wosData.CT.Replace("'", "''"));
                        command.Parameters.Add(parm14);
                        OracleParameter parm15 = new OracleParameter("CY", wosData.CY.Replace("'", "''"));
                        command.Parameters.Add(parm15);
                        OracleParameter parm16 = new OracleParameter("CL", wosData.CL.Replace("'", "''"));
                        command.Parameters.Add(parm16);
                        OracleParameter parm17 = new OracleParameter("SP", wosData.SP.Replace("'", "''"));
                        command.Parameters.Add(parm17);
                        OracleParameter parm18 = new OracleParameter("HO", wosData.HO.Replace("'", "''"));
                        command.Parameters.Add(parm18);
                        OracleParameter parm19 = new OracleParameter("DE", wosData.DE.Replace("'", "''"));
                        command.Parameters.Add(parm19);
                        OracleParameter parm20 = new OracleParameter("ID", wosData.ID.Replace("'", "''"));
                        command.Parameters.Add(parm20);
                        OracleParameter parm21 = new OracleParameter("AB", OracleType.Clob);
                        parm21.Value = wosData.AB.Replace("'", "''");
                        command.Parameters.Add(parm21);
                        OracleParameter parm22 = new OracleParameter("C1", OracleType.Clob);
                        parm22.Value = wosData.C1.Replace("'", "''");
                        command.Parameters.Add(parm22);
                        OracleParameter parm23 = new OracleParameter("RP", OracleType.Clob);
                        parm23.Value = wosData.RP.Replace("'", "''");
                        command.Parameters.Add(parm23);
                        OracleParameter parm24 = new OracleParameter("EM", wosData.EM.Replace("'", "''"));
                        command.Parameters.Add(parm24);
                        OracleParameter parm25 = new OracleParameter("RI", wosData.RI.Replace("'", "''"));
                        command.Parameters.Add(parm25);
                        OracleParameter parm26 = new OracleParameter("OI", wosData.OI.Replace("'", "''"));
                        command.Parameters.Add(parm26);
                        OracleParameter parm27 = new OracleParameter("FU", wosData.FU.Replace("'", "''"));
                        command.Parameters.Add(parm27);
                        OracleParameter parm28 = new OracleParameter("FX", OracleType.Clob);
                        parm28.Value = wosData.FX.Replace("'", "''");
                        command.Parameters.Add(parm28);
                        OracleParameter parm29 = new OracleParameter("CR", OracleType.Clob);
                        parm29.Value = wosData.CR.Replace("'", "''");
                        command.Parameters.Add(parm29);
                        OracleParameter parm30 = new OracleParameter("NR", wosData.NR.Replace("'", "''"));
                        command.Parameters.Add(parm30);
                        OracleParameter parm31 = new OracleParameter("TC", wosData.TC.Replace("'", "''"));
                        command.Parameters.Add(parm31);
                        OracleParameter parm32 = new OracleParameter("Z9", wosData.Z9.Replace("'", "''"));
                        command.Parameters.Add(parm32);
                        OracleParameter parm33 = new OracleParameter("U1", wosData.U1.Replace("'", "''"));
                        command.Parameters.Add(parm33);
                        OracleParameter parm34 = new OracleParameter("U2", wosData.U2.Replace("'", "''"));
                        command.Parameters.Add(parm34);
                        OracleParameter parm35 = new OracleParameter("PU", wosData.PU.Replace("'", "''"));
                        command.Parameters.Add(parm35);
                        OracleParameter parm36 = new OracleParameter("PI", wosData.PI.Replace("'", "''"));
                        command.Parameters.Add(parm36);
                        OracleParameter parm37 = new OracleParameter("PA", wosData.PA.Replace("'", "''"));
                        command.Parameters.Add(parm37);
                        OracleParameter parm38 = new OracleParameter("SN", wosData.SN.Replace("'", "''"));
                        command.Parameters.Add(parm38);
                        OracleParameter parm39 = new OracleParameter("EI", wosData.EI.Replace("'", "''"));
                        command.Parameters.Add(parm39);
                        OracleParameter parm40 = new OracleParameter("BN", wosData.BN.Replace("'", "''"));
                        command.Parameters.Add(parm40);
                        OracleParameter parm41 = new OracleParameter("J9", wosData.J9.Replace("'", "''"));
                        command.Parameters.Add(parm41);
                        OracleParameter parm42 = new OracleParameter("JI", wosData.JI.Replace("'", "''"));
                        command.Parameters.Add(parm42);
                        OracleParameter parm43 = new OracleParameter("PD", wosData.PD.Replace("'", "''"));
                        command.Parameters.Add(parm43);
                        OracleParameter parm44 = new OracleParameter("PY", wosData.PY.Replace("'", "''"));
                        command.Parameters.Add(parm44);
                        OracleParameter parm45 = new OracleParameter("VL", wosData.VL.Replace("'", "''"));
                        command.Parameters.Add(parm45);
                        OracleParameter parm46 = new OracleParameter("ISS", wosData.IS.Replace("'", "''"));
                        command.Parameters.Add(parm46);
                        OracleParameter parm47 = new OracleParameter("PN", wosData.PN.Replace("'", "''"));
                        command.Parameters.Add(parm47);
                        OracleParameter parm48 = new OracleParameter("SU", wosData.SU.Replace("'", "''"));
                        command.Parameters.Add(parm48);
                        OracleParameter parm49 = new OracleParameter("SI", wosData.SI.Replace("'", "''"));
                        command.Parameters.Add(parm49);
                        OracleParameter parm50 = new OracleParameter("MA", wosData.MA.Replace("'", "''"));
                        command.Parameters.Add(parm50);
                        OracleParameter parm51 = new OracleParameter("BP", wosData.BP.Replace("'", "''"));
                        command.Parameters.Add(parm51);
                        OracleParameter parm52 = new OracleParameter("EP", wosData.EP.Replace("'", "''"));
                        command.Parameters.Add(parm52);
                        OracleParameter parm53 = new OracleParameter("AR", wosData.AR.Replace("'", "''"));
                        command.Parameters.Add(parm53);
                        OracleParameter parm54 = new OracleParameter("DI", wosData.DI.Replace("'", "''"));
                        command.Parameters.Add(parm54);
                        OracleParameter parm55 = new OracleParameter("D2", wosData.D2.Replace("'", "''"));
                        command.Parameters.Add(parm55);
                        OracleParameter parm56 = new OracleParameter("PG", wosData.PG.Replace("'", "''"));
                        command.Parameters.Add(parm56);
                        OracleParameter parm57 = new OracleParameter("WC", wosData.WC.Replace("'", "''"));
                        command.Parameters.Add(parm57);
                        OracleParameter parm58 = new OracleParameter("SC", wosData.SC.Replace("'", "''"));
                        command.Parameters.Add(parm58);
                        OracleParameter parm59 = new OracleParameter("GA", wosData.GA.Replace("'", "''"));
                        command.Parameters.Add(parm59);
                        OracleParameter parm60 = new OracleParameter("UT", wosData.UT);
                        command.Parameters.Add(parm60);
                        OracleParameter parm61 = new OracleParameter("PM", wosData.PM.Replace("'", "''"));
                        command.Parameters.Add(parm61);
                        command.ExecuteNonQuery();
                    }
                }
            } catch (Exception e) {
                Logs.WriteLog(string.Format("{0}\r\n{1}", e.Message, sql));
                throw e;
            }
        }


        public static string GetInsertStr(WosData wosData) {
            string[] datas = wosData.getDataArray();
            string sReturn = "'";
            for (int i = 0; i < 62; i++) {
                if (i != 61) {
                    sReturn += datas[i].Replace("'", "''").Replace("\"", "\"\"") + "','";
                } else {
                    sReturn += datas[i].Replace("'", "''").Replace("\"", "\"\"") + "'";
                }
            }
            return sReturn;
        }

        /// <summary>
        /// 将DataSet数据表插入到oracle库中
        /// </summary>
        /// <param name="ds"></param>
        public static bool ExecuteInsert(DataSet ds) {
            if (ds.Tables.Count > 0) {
                OracleConnection connection = new OracleConnection(SearcherTool.ConnectString);
                connection.Open();
                var trans = connection.BeginTransaction();
                string dataSetName = ds.DataSetName;
                try {
                    foreach (DataTable dt in ds.Tables) {
                        string tableName = dt.TableName;
                        OracleCommand cmd = new OracleCommand(string.Format("select * from {0} where rownum < 1", tableName), connection);
                        cmd.Transaction = trans;
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        OracleCommandBuilder cb = new OracleCommandBuilder(adapter);
                        adapter.Update(dt);
                    }
                    trans.Commit();
                } catch (Exception e) {
                    trans.Rollback();
                    Logs.WriteLog(string.Format("插入数据失败：{0} :{1}", dataSetName, e.Message));
                    return false;
                }
                return true;
            } else {
                return false;
            }

        }
        #endregion
    }
}
