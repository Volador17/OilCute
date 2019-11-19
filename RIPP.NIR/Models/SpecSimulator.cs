using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RIPP.NIR.Models
{
    public class SpecSimulator
    {
        private List<Spectrum> specs = new List<Spectrum>();

        public List<Spectrum> Specs
        {
            get { return this.specs; }
        }

        public SpecSimulator()
        {
            string appPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "spec");
            this.specs.Add(new Spectrum(Path.Combine(appPath, "f1-1.rip"), SpecExtensionEnum.RIP) { Components = this.getCompoenent() });
            this.specs.Add(new Spectrum(Path.Combine(appPath, "f1-2.rip"), SpecExtensionEnum.RIP) { Components = this.getCompoenent() });
            this.specs.Add(new Spectrum(Path.Combine(appPath, "f1-3.rip"), SpecExtensionEnum.RIP) { Components = this.getCompoenent() });
            this.specs.Add(new Spectrum(Path.Combine(appPath, "f1-4.rip"), SpecExtensionEnum.RIP) { Components = this.getCompoenent() });
            this.specs.Add(new Spectrum(Path.Combine(appPath, "f1-5.rip"), SpecExtensionEnum.RIP) { Components = this.getCompoenent() });
            this.specs.Add(new Spectrum(Path.Combine(appPath, "f1-6.rip"), SpecExtensionEnum.RIP) { Components = this.getCompoenent() });
        }

        private ComponentList getCompoenent()
        {
            Random myRandom = new Random();
            float corr = (float)(1 + (myRandom.NextDouble() * 0.05));
            ComponentList clist = new ComponentList();
            clist.Add(new Component() { Name = "置信度", PredictedValue = 0.98 * corr });
            clist.Add(new Component() { Name = "密度", PredictedValue = 0.8515 * corr });
            clist.Add(new Component() { Name = "酸值", PredictedValue = 0.97 * corr });
            clist.Add(new Component() { Name = "残炭", PredictedValue = 2.98 * corr });
            clist.Add(new Component() { Name = "硫含量", PredictedValue = 1.46 * corr });
            clist.Add(new Component() { Name = "氮含量", PredictedValue = 0.04 * corr });
            clist.Add(new Component() { Name = "蜡含量", PredictedValue = 2.56 * corr });
            clist.Add(new Component() { Name = "胶质", PredictedValue = 6.19 * corr });
            clist.Add(new Component() { Name = "沥青质", PredictedValue = 0.20 * corr });
            clist.Add(new Component() { Name = "TBP65", PredictedValue = 5.97 * corr });
            clist.Add(new Component() { Name = "TBP80", PredictedValue = 7.35 * corr });
            clist.Add(new Component() { Name = "TBP100", PredictedValue = 10.94 * corr });
            clist.Add(new Component() { Name = "TBP120", PredictedValue = 13.33 * corr });
            clist.Add(new Component() { Name = "TBP140", PredictedValue = 16.69 * corr });
            clist.Add(new Component() { Name = "TBP165", PredictedValue = 20.90 * corr });
            clist.Add(new Component() { Name = "TBP180", PredictedValue = 22.68 * corr });
            clist.Add(new Component() { Name = "TBP200", PredictedValue = 25.85 * corr });
            clist.Add(new Component() { Name = "TBP220", PredictedValue = 28.29 * corr });
            clist.Add(new Component() { Name = "TBP240", PredictedValue = 30.36 * corr });
            clist.Add(new Component() { Name = "TBP260", PredictedValue = 33.38 * corr });
            clist.Add(new Component() { Name = "TBP280", PredictedValue = 36.83 * corr });
            clist.Add(new Component() { Name = "TBP300", PredictedValue = 38.66 * corr });
            clist.Add(new Component() { Name = "TBP320", PredictedValue = 43.04 * corr });
            clist.Add(new Component() { Name = "TBP350", PredictedValue = 45.64 * corr });
            clist.Add(new Component() { Name = "TBP380", PredictedValue = 52.35 * corr });
            clist.Add(new Component() { Name = "TBP400", PredictedValue = 55.88 * corr });
            clist.Add(new Component() { Name = "TBP425", PredictedValue = 56.53 * corr });
            clist.Add(new Component() { Name = "TBP450", PredictedValue = 57.85 * corr });
            clist.Add(new Component() { Name = "TBP470", PredictedValue = 64.03 * corr });
            clist.Add(new Component() { Name = "TBP500", PredictedValue = 68.95 * corr });
            clist.Add(new Component() { Name = "TBP520", PredictedValue = 73.03 * corr });
            clist.Add(new Component() { Name = "TBP540", PredictedValue = 75.75 * corr });

            return clist;
            
        }

    }
}
