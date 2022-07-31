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

        public void updateProfile(string address)
        {


        }
        public void applyProfile()
        {

        }
    }
    public class ManageXML_Apps
    {




    }
}
