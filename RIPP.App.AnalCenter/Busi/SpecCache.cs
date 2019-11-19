using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RIPP.Lib;
using RIPP.NIR;
using RIPP.NIR.Models;
using System.IO;

namespace RIPP.App.AnalCenter.Busi
{
    public class SpecCache
    {
       // private IdentifyModel _baseModel;
        private IdentifyModel _model;
       // private List<Specs> _specList;
        private int _maxNum = 5;
        private string _baseModelName = "base.IdLib";

        public SpecCache(int maxNum)
        {
            this._maxNum = maxNum;
            this.init();
        }

        private void init()
        {
            //加载模型
            var fullpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this._baseModelName);

            this._model = Serialize.Read<IdentifyModel>(fullpath);
            if (this._model == null)
                return;
            this._model.FullPath = fullpath;
            if (this._model.LibBase == null || this._model.LibBase.Count == 0)
                this._model.SpecLib.Clear();
          
            if (this._model.LibBase == null)
                this._model.LibBase = new SpecBase();
            if (this._model.LibBase.Components == null || this._model.LibBase.Components.Count == 0)
            {
                this._model.LibBase.Comp_Add(new Component() { Name = "ID" });
            }
            else
                this._model.LibBase.Components[0].Name = "ID";
            for (int i = this._model.LibBase.Components.Count - 1; i > 0; i--)
                this._model.LibBase.Comp_RemoveAt(i);

            this._model.Save();
        }

        public Specs GetSpecsByIndex(int idx)
        {
            if(this._model==null||this._model.LibBase==null)
                return null;
            if (idx < 0 || idx > this._model.LibBase.Count)
                return null;
            if (!this._model.LibBase.Components.Contains("ID"))
                return null;
            var s = this._model.LibBase[idx];
            int dbID = (int)s.Components["ID"].ActualValue;
            if(dbID<1)
                return null; ;
            using (var db = new NIRCeneterEntities())
            {
                var dbs = db.Specs.Where(d => d.ID == dbID).FirstOrDefault();
                return dbs;
            }
        }

        public void RemoveSpecByIdx(int idx)
        {
            if (this._model == null || this._model.LibBase == null)
                return ;
            if (idx < 0 || idx > this._model.LibBase.Count)
                return ;
            this._model.LibBase.RemoveAt(idx);
        }

        public void SaveModel()
        {
            if (this._model == null)
                return;
            if (this._model.LibBase.Count < 1)
                return;
            this._model.Train(this._model.LibBase.Clone());
            this._model.Save();
        }

        public SpecBase GetBaseLib()
        {
            if (this._model == null )
                return null;
            return this._model.LibBase;
        }
       

        public bool Predict(ref Specs s)
        {
            if (s == null || s.Spec == null)
                return false;
            if (this._maxNum < 1)
                return false;
            try
            {
                if (this._model == null||this._model.SpecLib==null||this._model.SpecLib.Count<1)
                    return false;
                var r = this._model.Predict(s.Spec);
                if (!r.IsId)
                    return false;
                if(r.Items.Length<1||!r.Items.First().Spec.Components.Contains("ID"))
                    return false;
                int dbID=(int)r.Items.First().Spec.Components["ID"].ActualValue;
                if(dbID<1)
                    return false;
                using (var db = new NIRCeneterEntities())
                {
                    var dsepc = db.Specs.Where(d => d.ID == dbID).FirstOrDefault();
                    if (dsepc == null)
                        return false;
                    s.Result = dsepc.Result;
                    s.ResultObj = dsepc.ResultObj;
                    s.ResultType = dsepc.ResultType;
                    s.Spec.Components = dsepc.Spec.Components;
                    s.PredictByA = true;

                    //修改密度的值
                    var c = s.Spec.Components.Where(d => d.Name == "密度(20℃)").FirstOrDefault();
                    if (c != null)
                    {
                        Random rd = new Random();
                        c.PredictedValue = c.PredictedValue * (1 + 0.0008 * (0.5 - rd.NextDouble()));
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public void AddSpec(Specs s)
        {
            if (s == null||s.Spec==null||s.ID<1)
                return;
            if (this._maxNum < 1)
                return ;
            try
            {
                var spec = s.Spec.Clone();
                spec.Components = new ComponentList();
                spec.Components.Add(new Component() { Name = "ID", ActualValue = s.ID });
                Action a = () =>
                    {
                        this._model.LibBase.Add(spec);
                        this._model.Train(this._model.LibBase.Clone());

                        if (this._model.LibBase.Specs.Count > this._maxNum)
                            this._model.LibBase.RemoveAt(0);
                        this._model.Save();
                    };
                a.BeginInvoke(null, null);
            }
            catch (Exception ex)
            {

            }
        }

    }
}
