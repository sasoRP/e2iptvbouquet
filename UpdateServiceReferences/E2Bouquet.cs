using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace UpdateServiceReferences
{
    public class E2Bouquet
    {
        private IDictionary<string, TVService> services { get; set; }
        private IDictionary<string, string> mapServiceUrlEpgCode { get; }
        private string name { get; set; }

        public E2Bouquet()
        {
            this.services = new Dictionary<string, TVService>();
            this.mapServiceUrlEpgCode = new Dictionary<string, string>();
        }

        public E2Bouquet(string bouquetFilename) :this()
        {
            this.loadBouquetFile(bouquetFilename);
        }

        public E2Bouquet(string bouquetFilename, string epgReferencesFileName) : this()
        {
            this.loadBouquetFile(bouquetFilename);
            this.loadEpgReferencesFile(epgReferencesFileName);
        }

        public void CreateEpgXmlFile(string filename)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<channels>");

            foreach (TVService tvService in this.services.Values)
            {
                string fullServiceReference = String.Format("{0}:{1}", tvService.ServiceReference, tvService.Url);
                string epgCode = this.mapServiceUrlEpgCode.ContainsKey(tvService.Url)? this.mapServiceUrlEpgCode[tvService.Url] : "REPLACEME";

                sb.AppendLine(String.Format("<channel id=\"{0}\">{1}</channel><!-- {2} -->", epgCode, fullServiceReference, tvService.Description));
            }

            sb.AppendLine("</channels>");

            using (System.IO.StreamWriter fileWriter = new System.IO.StreamWriter(filename, true))
            {
                fileWriter.Write(sb.ToString());
            }
        }

        public void CreateBouquetFile(string filename)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("NAME {0}", this.name));

            foreach (TVService tvService in this.services.Values)
            {
                sb.AppendLine(String.Format("#SERVICE {0}:{1}", tvService.ServiceReference, tvService.Url));
                sb.AppendLine(String.Format("#DESCRIPTION {0}", tvService.Description));
            }

            using (System.IO.StreamWriter fileWriter = new System.IO.StreamWriter(filename, true))
            {
                fileWriter.Write(sb.ToString());
            }
        }

        public void CreateUniqueReferences()
        {
            int counter = 1;
            IDictionary<string, TVService> servicesWithUniqueReferences = new Dictionary<string, TVService>();

            foreach (TVService originalTvService in this.services.Values)
            {
                TVService uniqueReferenceTvService = new TVService();
                uniqueReferenceTvService.Url = originalTvService.Url;
                uniqueReferenceTvService.Description = originalTvService.Description;
                uniqueReferenceTvService.ServiceReference = String.Format("1:0:1:{0}:0:0:0:0:0:0", counter.ToString("X"));

                servicesWithUniqueReferences.Add(originalTvService.Url, uniqueReferenceTvService);
                counter++;
            }

            this.services = servicesWithUniqueReferences;
        }


        private TVService parseService(string line, string descriptionLine)
        {
            TVService tvService = new TVService();

            tvService.ServiceReference = line.Substring(8, line.IndexOf(":http") - 8);
            tvService.Url = line.Substring(line.IndexOf("http"));
            tvService.Description = descriptionLine.Substring(13);

            return tvService;
        }

        private string parseBouquetName(string line)
        {
            return line.Substring(line.IndexOf("#NAME ") + 6);
        }

        private string parseDirective(string line)
        {
            string directiveName = String.Empty;

            if (line.StartsWith("#"))
            {
                directiveName = line.Substring(line.IndexOf('#') + 1, line.IndexOf(' ') - 1);
            }

            return directiveName;
        }

        private void loadBouquetFile(string filename)
        {
            using (StreamReader reader = new StreamReader(File.OpenRead(filename)))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string directiveName = this.parseDirective(line);

                    switch (directiveName)
                    {
                        case "NAME":
                            this.name = this.parseBouquetName(line);
                            break;
                        case "SERVICE":
                            string descriptionLine = reader.ReadLine();
                            TVService tvService = this.parseService(line, descriptionLine);

                            this.services.Add(tvService.Url, tvService);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void loadEpgReferencesFile(string filename)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filename);

            foreach (XmlNode node in xmlDocument.DocumentElement.SelectNodes("channel"))
            {
                string serviceUrl = node.InnerText.Substring(node.InnerText.IndexOf("http"));
                string epgCode = node.Attributes["id"].Value;

                this.mapServiceUrlEpgCode.Add(serviceUrl, epgCode);
            }
        }
    }
}