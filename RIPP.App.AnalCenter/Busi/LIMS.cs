using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.Lib;
using System.IO;

namespace RIPP.App.AnalCenter.Busi
{
    public class LIMS
    {
        public static bool WriteToFile(Specs s,List<PropertyTable> initP, string fullPath,string dataDescription)
        {
            if (s == null || s.Spec == null)
                return false;
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format("数据描述：{0}", dataDescription));
            sb.AppendLine("分析方法：原油近红外快速评价.met");
            sb.AppendLine(string.Format("采样时间：{0}", s.SampleTime.HasValue ? s.SampleTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""));
            sb.AppendLine(string.Format("采样点：{0}", s.SamplePlace));
            sb.AppendLine(string.Format("样品名称：{0}", s.Spec.Name));
            sb.AppendLine(string.Format("分析时间：{0}", s.AnalyTime.HasValue ? s.AnalyTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""));
            sb.AppendLine(string.Format("分析者：{0}", s.User != null ? s.User.realName : ""));
            sb.AppendLine(string.Format("数据文件：{0}", fullPath));
            sb.AppendLine("");
            sb.AppendLine("数据描述\t性质\t单位\t数值\t置信度");
            var dlstStr = new List<string>();
            

            foreach (var t in s.OilData)
            {
                var initTable = initP == null ? null : initP.Where(d => d.Table == t.Table).FirstOrDefault();
                IOrderedEnumerable<PropertyEntity> lst;
                if (initTable != null)
                    lst = t.Datas.Where(d => initTable.Datas.Where(b => b.Name == d.Name && b.Name2 == d.Name2 && b.ShowEngineer == true && b.ShowRIPP == true&&!double.IsNaN(d.Value)).Count() > 0).OrderBy(d => d.ColumnIdx).ThenBy(d => d.Index);
                else
                    lst = t.Datas.Where(d => !double.IsNaN(d.Value)).OrderBy(d => d.ColumnIdx).ThenBy(d => d.Index);
               
                //lst.Where(d=>d.ShowRIPP&&d.ShowEngineer)

                if (t.Table == PropertyType.NIR)
                {
                    dlstStr.AddRange(lst.Select(d => string.Format("{0}\t{1}\t{2}\t{3}\t{4}",
                          dataDescription,
                          d.Name == d.Name2 ? d.Name : (d.Name + d.Name2),
                          d.Unit,
                          d.Value.ToString(string.Format("F{0}", d.Eps)),
                          d.Confidence.ToString("F1")
                          )));
                }
                else
                {
                    dlstStr.AddRange(lst.Select(d => string.Format("{0}\t{1}\t{2}\t{3}\t{4}",
                          dataDescription,
                          t.Table.GetDescription() + (d.Name == d.Name2 ? d.Name : (d.Name + d.Name2)),
                          d.Unit,
                          d.Value.ToString(string.Format("F{0}", d.Eps)),
                          d.Confidence.ToString("F1")
                          )));
                }
            }
            foreach (var sin in dlstStr)
                sb.AppendLine(sin);

            using (StreamWriter sw = new StreamWriter(fullPath, false, Encoding.Default))
            {
                sw.Write(sb.ToString());
            }
            return true;
        }

    }
}
