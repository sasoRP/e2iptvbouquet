using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateServiceReferences
{
    class Program
    {
        static void Main(string[] args)
        {
            int counter = 1;
            string line;

            // Read the file and display it line by line.  
            System.IO.StreamReader file =
                new System.IO.StreamReader(@"C:\Users\prosa\Desktop\test-iptv-references\channels.tv");

            StringBuilder sb = new StringBuilder();

            StringBuilder customChannels = new StringBuilder();
            customChannels.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            customChannels.AppendLine("<channels>");

            while ((line = file.ReadLine()) != null)
            {
                if (line.StartsWith("#SERVICE"))
                {
                    string service = line.Replace("4097:0:1:0:0:0:0:0:0:0:", String.Format("4097:0:1:{0}:0:0:0:0:0:0:", counter.ToString("X")));
                    sb.AppendLine(service);

                    string description = file.ReadLine();
                    sb.AppendLine(description);

                    string epgLine = String.Format("<channel id=\"REPLACEME\">{0}</channel><!-- {1} -->", service.Substring(9), description.Substring(13));
                    customChannels.AppendLine(epgLine);

                    counter++;
                }
            }

            file.Close();

            using (System.IO.StreamWriter newFile =
            new System.IO.StreamWriter(@"C:\Users\prosa\Desktop\test-iptv-references\channels2.tv", true))
            {
                newFile.Write(sb.ToString());
            }

            customChannels.AppendLine("</channels>");

            using (System.IO.StreamWriter epgFile =
            new System.IO.StreamWriter(@"C:\Users\prosa\Desktop\test-iptv-references\custom.channels.xml", true))
            {
                epgFile.Write(customChannels.ToString());
            }
        }
    }
}
