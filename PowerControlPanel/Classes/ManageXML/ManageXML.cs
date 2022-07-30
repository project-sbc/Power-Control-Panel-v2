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
                dt.Rows.Add(node.Name);
            }
            return dt;


        }
        public void createProfile()
        {


            //Dim xlmdoc As New Xml.XmlDocument
            //xlmdoc.Load(My.Settings.strXML)
        }
        public void deleteProfile()
        {

        }
        public string loadProfile(string powerStatus, string parameter, string profileName)
        {
            string result = "";
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles/" + profileName + "/" + powerStatus + "/" + parameter);

            if (xmlNode != null)
            {
                result = xmlNode.InnerText; 
            }

            xmlDocument = null;
            return result;
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
