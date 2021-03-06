﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Entity {
    public class WosData {
        /// <summary>
        /// 出版物类型
        /// </summary>
        public string PT { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string AU { get; set; }
        public string BA { get; set; }
        public string BE { get; set; }
        public string GP { get; set; }
        /// <summary>
        /// 作者全名
        /// </summary>
        public string AF { get; set; }
        public string BF { get; set; }
        public string CA { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string TI { get; set; }
        /// <summary>
        /// 来源出版物
        /// </summary>
        public string SO { get; set; }
        public string SE { get; set; }
        public string BS { get; set; }
        /// <summary>
        /// 语种
        /// </summary>
        public string LA { get; set; }
        /// <summary>
        /// 文献类型
        /// </summary>
        public string DT { get; set; }
        public string CT { get; set; }
        public string CY { get; set; }
        public string CL { get; set; }
        public string SP { get; set; }
        public string HO { get; set; }
        /// <summary>
        /// 关键词
        /// </summary>
        public string DE { get; set; }
        /// <summary>
        /// 扩展关键字
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string AB { get; set; }
        /// <summary>
        /// 作者地址
        /// </summary>
        public string C1 { get; set; }
        /// <summary>
        /// 通讯作者地址
        /// </summary>
        public string RP { get; set; }
        /// <summary>
        /// 电子邮件地址
        /// </summary>
        public string EM { get; set; }
        public string RI { get; set; }
        public string OI { get; set; }
        /// <summary>
        /// 基金资助机构和授权号
        /// </summary>
        public string FU { get; set; }
        /// <summary>
        /// 基金资助正文
        /// </summary>
        public string FX { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CR { get; set; }
        /// <summary>
        /// 引用的参考文献数
        /// </summary>
        public string NR { get; set; }
        /// <summary>
        /// 被引频次计数(WoS核心合集)
        /// </summary>
        public string TC { get; set; }
        /// <summary>
        /// 被引频次合计（WoS核心、BCI 和 CSCD）
        /// </summary>
        public string Z9 { get; set; }
        public string U1 { get; set; }
        public string U2 { get; set; }
        /// <summary>
        /// 出版商
        /// </summary>
        public string PU { get; set; }
        /// <summary>
        /// 出版商所在城市
        /// </summary>
        public string PI { get; set; }
        /// <summary>
        /// 出版商地址
        /// </summary>
        public string PA { get; set; }
        /// <summary>
        /// 国际标准期刊号 (ISSN)
        /// </summary>
        public string SN { get; set; }
        public string EI { get; set; }
        public string BN { get; set; }
        /// <summary>
        /// 来源文献名称缩写
        /// </summary>
        public string J9 { get; set; }
        /// <summary>
        /// ISO 来源文献名称缩写
        /// </summary>
        public string JI { get; set; }
        /// <summary>
        /// 出版日期
        /// </summary>
        public string PD { get; set; }
        /// <summary>
        /// 出版年
        /// </summary>
        public string PY { get; set; }
        /// <summary>
        /// 卷
        /// </summary>
        public string VL { get; set; }
        /// <summary>
        /// 期
        /// </summary>
        public string IS { get; set; }
        public string PN { get; set; }
        public string SU { get; set; }
        public string SI { get; set; }
        public string MA { get; set; }
        /// <summary>
        /// 开始页
        /// </summary>
        public string BP { get; set; }
        /// <summary>
        /// 结束页
        /// </summary>
        public string EP { get; set; }
        public string AR { get; set; }
        /// <summary>
        /// 数字对象标识符 (DOI)
        /// </summary>
        public string DI { get; set; }
        public string D2 { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        public string PG { get; set; }
        /// <summary>
        /// Web of Science 类别
        /// </summary>
        public string WC { get; set; }
        /// <summary>
        /// 研究方向
        /// </summary>
        public string SC { get; set; }
        /// <summary>
        /// 文献传递号
        /// </summary>
        public string GA { get; set; }
        /// <summary>
        /// 入藏号
        /// </summary>
        public string UT { get; set; }
        public string PM { get; set; }

        /// <summary>
        /// wos网站中导出的数据数组
        /// </summary>
        private string[] _dataArray;

        public string[] getDataArray() {
            return _dataArray;
        }

        public void setDataArray(object[] datas) {
            this.PT = datas[0].ToString();
            this.AU = datas[1].ToString();
            this.BA = datas[2].ToString();
            this.BE = datas[3].ToString();
            this.GP = datas[4].ToString();
            this.AF = datas[5].ToString();
            this.BF = datas[6].ToString();
            this.CA = datas[7].ToString();
            this.TI = datas[8].ToString();
            this.SO = datas[9].ToString();
            this.SE = datas[10].ToString();
            this.BS = datas[11].ToString();
            this.LA = datas[12].ToString();
            this.DT = datas[13].ToString();
            this.CT = datas[14].ToString();
            this.CY = datas[15].ToString();
            this.CL = datas[16].ToString();
            this.SP = datas[17].ToString();
            this.HO = datas[18].ToString();
            this.DE = datas[19].ToString();
            this.ID = datas[20].ToString();
            this.AB = datas[21].ToString();
            this.C1 = datas[22].ToString();
            this.RP = datas[23].ToString();
            this.EM = datas[24].ToString();
            this.RI = datas[25].ToString();
            this.OI = datas[26].ToString();
            this.FU = datas[27].ToString();
            this.FX = datas[28].ToString();
            this.CR = datas[29].ToString();
            this.NR = datas[30].ToString();
            this.TC = datas[31].ToString();
            this.Z9 = datas[32].ToString();
            this.U1 = datas[33].ToString();
            this.U2 = datas[34].ToString();
            this.PU = datas[35].ToString();
            this.PI = datas[36].ToString();
            this.PA = datas[37].ToString();
            this.SN = datas[38].ToString();
            this.EI = datas[39].ToString();
            this.BN = datas[40].ToString();
            this.J9 = datas[41].ToString();
            this.JI = datas[42].ToString();
            this.PD = datas[43].ToString();
            this.PY = datas[44].ToString();
            this.VL = datas[45].ToString();
            this.IS = datas[46].ToString();
            this.PN = datas[47].ToString();
            this.SU = datas[48].ToString();
            this.SI = datas[49].ToString();
            this.MA = datas[50].ToString();
            this.BP = datas[51].ToString();
            this.EP = datas[52].ToString();
            this.AR = datas[53].ToString();
            this.DI = datas[54].ToString();
            this.D2 = datas[55].ToString();
            this.PG = datas[56].ToString();
            this.WC = datas[57].ToString();
            this.SC = datas[58].ToString();
            this.GA = datas[59].ToString();
            this.UT = datas[60].ToString();
            this.PM = datas[61].ToString();

            _dataArray = new string[datas.Length];
            for (int i = 0; i < datas.Length; i++) {
                _dataArray[i] = datas[i].ToString();
            }
        }
    }
}
