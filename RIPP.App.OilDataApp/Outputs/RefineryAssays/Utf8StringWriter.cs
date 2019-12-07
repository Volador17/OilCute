using System.IO;
using System.Text;

namespace RIPP.App.OilDataApp.Outputs.RefineryAssays
{
    internal class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}