using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Searcher {
    class Utils {
        /// <summary>
        /// 计算匹配度
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static double MatchValue(string str1_src, string str2_src, bool isCn) {
            double matchValue = 0;
            string str1 = DealStringForMatch(str1_src);
            string str2 = DealStringForMatch(str2_src);

            List<string> strArray1 = ToList(str1, isCn);
            List<string> strArray2 = ToList(str2, isCn);

            foreach (var item in strArray1) {
                if (strArray2.Contains(item)) {
                    strArray2.Remove(item);
                    matchValue++;
                    continue;
                }
            }

            strArray1 = ToList(str1, isCn);
            strArray2 = ToList(str2, isCn);
            foreach (var item in strArray2) {
                if (strArray1.Contains(item)) {
                    strArray1.Remove(item);
                    matchValue++;
                    continue;
                }
            }

            strArray1 = ToList(str1, isCn);
            strArray2 = ToList(str2, isCn);
            matchValue = matchValue / (strArray1.Count + strArray2.Count);

            return Math.Round(matchValue, 2);
        }

        /// <summary>
        /// 将字符串的所有特殊符号处理为空格
        /// </summary>
        /// <param name="str_src"></param>
        /// <returns></returns>
        private static string DealStringForMatch(string str_src) {
            string regex = @"[^\w]+";
            string str = ToDBC(str_src);
            str = Regex.Replace(str, regex, " ");
            return str;
        }


        /// <summary>
        /// 将字符串拆分为单词列表（汉字列表）
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="isCn">是否是中文</param>
        /// <returns></returns>
        private static List<string> ToList(string str1, bool isCn) {
            List<string> lReturn;
            if (isCn) {
                char[] chars = UnicodeToString(str1).ToCharArray();
                lReturn = new List<string>();
                for (int i = 0; i < chars.Length; i++) {
                    lReturn.Add(chars[i].ToString());
                }
            } else {
                lReturn = str1.ToLower().Split(' ').ToList();
            }
            int count = lReturn.Count;
            for (int i = 0; i < count; i++) {
                lReturn.Remove("");
            }
            return lReturn;
        }

        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToDBC(string input) {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++) {
                if (c[i] == 12288) {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }


        /// <summary>
        /// 把unicode码转换为汉字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UnicodeToString(string str) {
            Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
            return reg.Replace(str, delegate(Match m) { return ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
        }
    }
}
