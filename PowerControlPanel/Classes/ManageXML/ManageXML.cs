using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using System.ComponentModel;
using System.Windows;

namespace Power_Control_Panel.PowerControlPanel.Classes.ManageXML
{
    public class ManageXML_Profiles
    {
        public DataTable profileList()
        {
            DataTable dt = new DataTable();
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");

            dt.Columns.Add("ProfileName");
            
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
           
                dt.Rows.Add(node.SelectSingleNode("ProfileName").InnerText);
            }
            xmlDocument = null;
            return dt;

            
        }
        public void createProfile()
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNodeTemplate = xmlDocument.SelectSingleNode("//Configuration/ProfileTemplate/Profile");
            XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/Profiles");
          
      
            string newProfileName = "NewProfile";
                        

            XmlNode newNode = xmlDocument.CreateNode(XmlNodeType.Element,"Profile", "");
            newNode.InnerXml = xmlNodeTemplate.InnerXml;
            newNode.SelectSingleNode("ProfileName").InnerText = newProfileName;
            xmlNodeProfiles.AppendChild(newNode);



            xmlDocument.Save(GlobalVariables.xmlFile);

            xmlDocument = null;

        }
        public void deleteProfile(string profileName)
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/Profiles");

            foreach (XmlNode node in xmlNodeProfiles.ChildNodes)
            {
                if (node.SelectSingleNode("ProfileName").InnerText == profileName)
                {
                    xmlNodeProfiles.RemoveChild(node);
                    break;
                }
                
            }

            xmlDocument.Save(GlobalVariables.xmlFile);
            xmlDocument = null;
        }


        public void saveProfileArray(string[] result, string profileName)
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ProfileName[text()='" + profileName + "']");
            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;

                if (parentNode != null)
                {
                    XmlNode onlineNode = parentNode.SelectSingleNode("Online");
                    foreach (XmlNode node in onlineNode.ChildNodes)
                    {
                        if (node.Name == "TDP1") { node.InnerText = result[0]; }
                        if (node.Name == "TDP2") { node.InnerText = result[1]; }
                        if (node.Name == "GPUCLK") { node.InnerText = result[2]; }
                    }



                    XmlNode offlineNode = parentNode.SelectSingleNode("Offline");
                    foreach (XmlNode node in offlineNode.ChildNodes)
                    {
                        if (node.Name == "TDP1") { node.InnerText = result[3];  }
                        if (node.Name == "TDP2") { node.InnerText = result[4]; }
                        if (node.Name == "GPUCLK") { node.InnerText = result[5]; }
                    }
                }


            }
            xmlDocument.Save(GlobalVariables.xmlFile);

            xmlDocument = null;
     

        }
        public string[] loadProfileArray(string profileName)
        {
            string[] result = new string [6];
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ProfileName[text()='" + profileName + "']");
            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;

                if (parentNode != null)
                {
                    XmlNode onlineNode = parentNode.SelectSingleNode("Online");
                    foreach (XmlNode node in onlineNode.ChildNodes)
                    {
                        if (node.Name == "TDP1") { result[0] = node.InnerText; }
                        if (node.Name == "TDP2") { result[1] = node.InnerText; }
                        if (node.Name == "GPUCLK") { result[2] = node.InnerText; }
                    }



                    XmlNode offlineNode = parentNode.SelectSingleNode("Offline");
                    foreach (XmlNode node in offlineNode.ChildNodes)
                    {
                        if (node.Name == "TDP1") { result[3] = node.InnerText; }
                        if (node.Name == "TDP2") { result[4] = node.InnerText; }
                        if (node.Name == "GPUCLK") { result[5] = node.InnerText; }
                    }
                }

           
            }

            xmlDocument = null;
            return result;
        }

        public string loadProfileParameter(string powerStatus, string parameter, string profileName)
        {
            string result = "";
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ProfileName[text()='" + profileName + "']");
            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;
                XmlNode parameterNode = parentNode.SelectSingleNode(powerStatus + "/" + parameter);
                result = parameterNode.InnerText;
            }

            xmlDocument = null;
            return result;
        }
        public void changeProfileParameter(string powerStatus, string parameter, string profileName, string newValue)
        {
            string result = "";
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ProfileName[text()='" + profileName + "']");
            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;
                XmlNode parameterNode = parentNode.SelectSingleNode(powerStatus + "/" + parameter);
                parameterNode.InnerText =newValue;
            }

            xmlDocument = null;
           
        }

        public void changeProfileName(string oldName, string newName)
        {
            
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNodeList xmlSelectedNodes = xmlNode.SelectNodes("Profile/ProfileName[text()='" + oldName + "']");
            if (xmlSelectedNodes != null)
            {
                foreach (XmlNode xmlNode1 in xmlSelectedNodes)
                {
                    XmlNode nameNode = xmlNode1.SelectSingleNode("ProfileName");
                    if (nameNode != null)
                    {
                        if (nameNode.InnerText == oldName) { nameNode.InnerText = newName; }

                    }

                }


            }

            xmlDocument.Save(GlobalVariables.xmlFile);

            xmlDocument = null;

        }


        public void applyProfile()
        {

        }
    }
    public class ManageXML_Apps
    {
        public DataTable appList()
        {
            DataTable dt = new DataTable();
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");

            dt.Columns.Add("DisplayName");

            foreach (XmlNode node in xmlNode.ChildNodes)
            {

                dt.Rows.Add(node.SelectSingleNode("DisplayName").InnerText);
            }
            xmlDocument = null;
            return dt;


        }

        public void changeProfileNameInApps(string oldName, string newName)
        {

            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");
            XmlNodeList xmlSelectedNodes = xmlNode.SelectNodes("App/Profile[text()='" + oldName + "']");
            if (xmlSelectedNodes != null)
            {
                foreach (XmlNode xmlNode1 in xmlSelectedNodes)
                {
                    XmlNode nameNode = xmlNode1.SelectSingleNode("Profile");
                    if (nameNode != null)
                    {
                        if (nameNode.InnerText == oldName) { nameNode.InnerText = newName; }

                    }

                }


            }

            xmlDocument.Save(GlobalVariables.xmlFile);

            xmlDocument = null;

        }

        public void createApp()
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNodeTemplate = xmlDocument.SelectSingleNode("//Configuration/AppTemplate/App");
            XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/Applications");


            string newAppName = "NewApp";


            XmlNode newNode = xmlDocument.CreateNode(XmlNodeType.Element, "App", "");
            newNode.InnerXml = xmlNodeTemplate.InnerXml;
            newNode.SelectSingleNode("DisplayName").InnerText = newAppName;
            xmlNodeProfiles.AppendChild(newNode);



            xmlDocument.Save(GlobalVariables.xmlFile);

            xmlDocument = null;

        }
        public void deleteApp(string appName)
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/Applications");

            foreach (XmlNode node in xmlNodeProfiles.ChildNodes)
            {
                if (node.SelectSingleNode("DisplayName").InnerText == appName)
                {
                    xmlNodeProfiles.RemoveChild(node);
                    break;
                }

            }

            xmlDocument.Save(GlobalVariables.xmlFile);
            xmlDocument = null;
        }


        public void saveProfileArray(string[] result, string appName)
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("App/DisplayName[text()='" + appName + "']");
            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;

                if (parentNode != null)
                {
                    
                    foreach (XmlNode node in parentNode.ChildNodes)
                    {
                        if (node.Name == "DisplayName") { node.InnerText = result[0]; }
                        if (node.Name == "Exe") { node.InnerText = result[1]; }
                        if (node.Name == "Path") { node.InnerText = result[2]; }
                        if (node.Name == "AppType") { node.InnerText = result[3]; }
                        if (node.Name == "GameType") { node.InnerText = result[4]; }
                        if (node.Name == "Image") { node.InnerText = result[5]; }
                        if (node.Name == "Profile") { node.InnerText = result[6]; }
                        if (node.Name == "Order") { node.InnerText = result[7]; }
                    }

          
                }


            }
            xmlDocument.Save(GlobalVariables.xmlFile);

            xmlDocument = null;


        }
        public string[] loadAppArray(string appName)
        {
            string[] result = new string[8];
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("App/DisplayName[text()='" + appName + "']");
            if (xmlSelectedNode != null)
            {
                XmlNode parentNode = xmlSelectedNode.ParentNode;

                if (parentNode != null)
                {
                   
                    foreach (XmlNode node in parentNode.ChildNodes)
                    {
                        if (node.Name == "DisplayName") { result[0] = node.InnerText; }
                        if (node.Name == "Exe") { result[1] = node.InnerText; }
                        if (node.Name == "Path") { result[2] = node.InnerText; }
                        if (node.Name == "AppType") { result[3] = node.InnerText; }
                        if (node.Name == "GameType") { result[4] = node.InnerText; }
                        if (node.Name == "Image") { result[5] = node.InnerText; }
                        if (node.Name == "Profile") { result[6] = node.InnerText; }
                        if (node.Name == "Order") { result[7] = node.InnerText; }
                     
                    }



                }


            }

            xmlDocument = null;
            return result;
        }

        public string loadAppParameter(string parameter, string appName)
        {
            string result = "";
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("App/DisplayName[text()='" + appName + "']");
            if (xmlSelectedNode != null)
            {
          
                XmlNode parameterNode = xmlSelectedNode.SelectSingleNode(parameter);
                result = parameterNode.InnerText;
            }

            xmlDocument = null;
            return result;
        }
        public void changeAppParameter(string parameter, string appName, string newValue)
        {
            string result = "";
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("App/DisplayName[text()='" + appName + "']");
            if (xmlSelectedNode != null)
            {
             
                XmlNode parameterNode = xmlSelectedNode.SelectSingleNode(parameter);
                parameterNode.InnerText = newValue;
            }

            xmlDocument = null;

        }



    }
}
