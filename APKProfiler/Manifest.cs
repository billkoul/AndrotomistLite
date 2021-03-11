using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
namespace APKProfiler
{
    public class Manifest
    {
        //Manifest Fields
        private XElement xElement;
        private List<string> permissions, intents, services, activities, receivers, providers;
        private string packageName, compileSdkVersion;

        //Getters
        public XElement Element => xElement;
        public List<string> Permissions => permissions;
        public List<string> Intents => intents;
        public List<string> Services => services;
        public List<string> Activities => activities;
        public List<string> Receivers => receivers;
        public List<string> Providers => providers;
        public string PackageName => packageName;
        public string CompileSdkVersion => compileSdkVersion;

        public Manifest()
        {
            permissions = new List<string>();
            intents = new List<string>();
            services = new List<string>();
            activities = new List<string>();
            receivers = new List<string>();
            providers = new List<string>();
            packageName = null;
            compileSdkVersion = null;
        }

        public void ParseManifest(string manifestFilePath)
        {
            try
            {
                xElement = XElement.Load(manifestFilePath);
                GetAllFromFile(xElement);
            }
            catch (System.IO.FileNotFoundException) { }
            catch (System.IO.DirectoryNotFoundException) { }
        }
        //Function to extract information from AndroidManifest.xml
        private void GetAllFromFile(XElement xmlRoot)
        {
            XNamespace android = "http://schemas.android.com/apk/res/android";
            List<string> tmpList = new List<string>();

            //Get Package name, compileSdkVersion
            if (xmlRoot.Attribute("package") != null)
                packageName = xmlRoot.Attribute("package").Value;
            if (xmlRoot.Attribute(android + "compileSdkVersion") != null)
                compileSdkVersion = xmlRoot.Attribute(android + "compileSdkVersion").Value;
            //Grab all descendants of the manifest's root.
            IEnumerable<XElement> elements = from element in xmlRoot.Descendants()
                                             select element;
            //Get permissions, no duplicates.
            foreach (XElement element in elements.Where(el => el.Name.LocalName.Contains("uses-permission")))
            {
                if (!tmpList.Contains(element.Attribute(android + "name").Value))
                    tmpList.Add(element.Attribute(android + "name").Value);
            }
            permissions = tmpList.ToList();
            tmpList.Clear();
            //Get intents
            foreach (XElement element in elements.Where(el => el.Name.LocalName.Equals("intent-filter")))
            {
                IEnumerable<XElement> intentChildren = from child in element.Descendants()
                                                       where child.Name.LocalName.Equals("action")
                                                       select child;
                foreach (XElement innerElement in intentChildren)
                {
                    if (!tmpList.Contains(innerElement.Attribute(android + "name").Value))
                    {
                        tmpList.Add(innerElement.Attribute(android + "name").Value);
                    }
                }
            }
            intents = tmpList.ToList();
            tmpList.Clear();
            //Get activities
            foreach (XElement element in elements.Where(el => el.Name.LocalName.Equals("activity")))
            {
                if (!tmpList.Contains(element.Attribute(android + "name").Value))
                {
                    tmpList.Add(element.Attribute(android + "name").Value);
                }
            }
            activities = tmpList.ToList();
            tmpList.Clear();
            //Get services
            foreach (XElement element in elements.Where(el => el.Name.LocalName.Equals("service")))
            {
                if (!tmpList.Contains(element.Attribute(android + "name").Value))
                {
                    tmpList.Add(element.Attribute(android + "name").Value);
                }
            }
            services = tmpList.ToList();
            tmpList.Clear();
            //Get receivers
            foreach (XElement element in elements.Where(el => el.Name.LocalName.Equals("receiver")))
            {
                if (!tmpList.Contains(element.Attribute(android + "name").Value))
                {
                    tmpList.Add(element.Attribute(android + "name").Value);
                }
            }
            receivers = tmpList.ToList();
            tmpList.Clear();
            //Get providers
            foreach (XElement element in elements.Where(el => el.Name.LocalName.Equals("provider")))
            {
                if (!tmpList.Contains(element.Attribute(android + "authorities").Value))
                {
                    tmpList.Add(element.Attribute(android + "authorities").Value);
                }
            }
            providers = tmpList.ToList();
            tmpList.Clear();
        }
    }
}
