using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Tracing;
using System.Xml;

namespace MvcApplication
{
    public class WebApiTracer : ITraceWriter
    {
        public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            TraceRecord rec = new TraceRecord(request, category, level);
            traceAction(rec);

            using (Stream xmlFile = new FileStream(@"C:\Path\log.xml", FileMode.Append))
            {
                using (XmlTextWriter writer = new XmlTextWriter(xmlFile, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.WriteStartElement("trace");
                    writer.WriteElementString("timestamp", rec.Timestamp.ToString());
                    writer.WriteElementString("operation", rec.Operation);
                    writer.WriteElementString("user", rec.Operator);
                    writer.WriteElementString("message", rec.Message);
                    writer.WriteElementString("category", rec.Category);
                    writer.WriteEndElement();
                    writer.WriteString(Environment.NewLine);
                }
            }
        }
    }

}