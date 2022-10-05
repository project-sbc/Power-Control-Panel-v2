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
        public static DataTable profileList()
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
        public static List<string> profileListForHomePage()
        {
            List<string> cboAppProfile = new List<string>();
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");

            cboAppProfile.Add("None");

            foreach (XmlNode node in xmlNode.ChildNodes)
            {

                cboAppProfile.Add(node.SelectSingleNode("ProfileName").InnerText);
            }
            xmlDocument = null;
            return cboAppProfile;


        }
        public static List<string> profileListForAppCBO()
        {
            List<string> cboAppProfile = new List<string>();
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");

          

            foreach (XmlNode node in xmlNode.ChildNodes)
            {

                cboAppProfile.Add(node.SelectSingleNode("ProfileName").InnerText);
            }
            xmlDocument = null;
            return cboAppProfile;


        }

        public static void createProfile()
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNodeTemplate = xmlDocument.SelectSingleNode("//Configuration/ProfileTemplate/Profile");
            XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/Profiles");
          
      
            string newProfileName = "NewProfile";
            int countProfile = 0;
            XmlNodeList xmlNodesByName = xmlNodeProfiles.SelectNodes("Profile/ProfileName[text()='" + newProfileName + "']");

            if (xmlNodesByName.Count > 0)
            {
                while (xmlNodesByName.Count > 0)
                {
                    countProfile++;
                    xmlNodesByName = xmlNodeProfiles.SelectNodes("Profile/ProfileName[text()='" + newProfileName + countProfile.ToString() + "']");

                }
                newProfileName = newProfileName + countProfile.ToString();
            }

   
            XmlNode newNode = xmlDocument.CreateNode(XmlNodeType.Element,"Profile", "");
            newNode.InnerXml = xmlNodeTemplate.InnerXml;
            newNode.SelectSingleNode("ProfileName").InnerText = newProfileName;
            xmlNodeProfiles.AppendChild(newNode);



            xmlDocument.Save(GlobalVariables.xmlFile);

            xmlDocument = null;

        }
        public static void deleteProfile(string profileName)
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


        public static void saveProfileArray(string[] result, string profileName)
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
                        if (node.Name == "MAXCPU") { node.InnerText = result[6]; }
                        if (node.Name == "ActiveCores") { node.InnerText = result[7]; }
                    }



                    XmlNode offlineNode = parentNode.SelectSingleNode("Offline");
                    foreach (XmlNode node in offlineNode.ChildNodes)
                    {
                        if (node.Name == "TDP1") { node.InnerText = result[3];  }
                        if (node.Name == "TDP2") { node.InnerText = result[4]; }
                        if (node.Name == "GPUCLK") { node.InnerText = result[5]; }
                        if (node.Name == "MAXCPU") { node.InnerText = result[8]; }
                        if (node.Name == "ActiveCores") { node.InnerText = result[9]; }
                    }
                }


            }
            xmlDocument.Save(GlobalVariables.xmlFile);

            xmlDocument = null;
     

        }
        public static string[] loadProfileArray(string profileName)
        {
            string[] result = new string [10];
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
                        if (node.Name == "MAXCPU") { result[6] = node.InnerText; }
                        if (node.Name == "ActiveCores") { result[7] = node.InnerText; }
                    }



                    XmlNode offlineNode = parentNode.SelectSingleNode("Offline");
                    foreach (XmlNode node in offlineNode.ChildNodes)
                    {
                        if (node.Name == "TDP1") { result[3] = node.InnerText; }
                        if (node.Name == "TDP2") { result[4] = node.InnerText; }
                        if (node.Name == "GPUCLK") { result[5] = node.InnerText; }
                        if (node.Name == "MAXCPU") { result[8] = node.InnerText; }
                        if (node.Name == "ActiveCores") { result[9] = node.InnerText; }
                    }
                }

           
            }

            xmlDocument = null;
            return result;
        }

        public static void applyProfile(string profileName)
        {
            if (profileName != "None")
            {
                string powerStatus = SystemParameters.PowerLineStatus.ToString();
                System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                xmlDocument.Load(GlobalVariables.xmlFile);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
                XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("Profile/ProfileName[text()='" + profileName + "']");
                if (xmlSelectedNode != null)
                {
                    XmlNode parentNode = xmlSelectedNode.ParentNode;

                    if (parentNode != null)
                    {
                        GlobalVariables.ActiveProfile = profileName;


                        XmlNode powerNode;
                        if (powerStatus == "Online")
                        {
                            powerNode = parentNode.SelectSingleNode("Online");
                        }
                        else
                        {
                            powerNode = parentNode.SelectSingleNode("Offline");
                        }
                        string tdp1 = "";
                        string tdp2 = "";
                        foreach (XmlNode node in powerNode.ChildNodes)
                        {
                            if (node.Name == "TDP1")
                            {
                                if (node.InnerText != "")
                                {
                                    tdp1 = node.InnerText;
                                    if (tdp2 != "")
                                    {
                                        Classes.TaskScheduler.TaskScheduler.runTask(() => GlobalVariables.tdp.changeTDP(Convert.ToInt32(tdp1), Convert.ToInt32(tdp2)));
                                    }
                                    //
                                }

                            }
                            if (node.Name == "TDP2")
                            {
                                if (node.InnerText != "")
                                {
                                    tdp2 = node.InnerText;
                                    if (tdp1 != "")
                                    {
                                        Classes.TaskScheduler.TaskScheduler.runTask(() => GlobalVariables.tdp.changeTDP(Convert.ToInt32(tdp1), Convert.ToInt32(tdp2)));
                                    }
                                }

                            }
                            if (node.Name == "GPUCLK")
                            {
                                if (node.InnerText != "")
                                {
                                    Classes.TaskScheduler.TaskScheduler.runTask(() => ChangeGPUCLK.ChangeGPUCLK.changeAMDGPUClock(Convert.ToInt32(node.InnerText)));
                                }

                            }
                            if (node.Name == "MAXCPU")
                            {
                                if (node.InnerText != "")
                                {
                                    Classes.TaskScheduler.TaskScheduler.runTask(() => changeCPU.ChangeCPU.changeCPUMaxFrequency(Convert.ToInt32(node.InnerText)));
                                }

                            }
                            if (node.Name == "ActiveCores")
                            {
                                if (node.InnerText != "")
                                {
                                    Classes.TaskScheduler.TaskScheduler.runTask(() => changeCPU.ChangeCPU.changeActiveCores(Convert.ToInt32(node.InnerText)));
                                }

                            }

                        }

                    }


                }
                else
                {
                    //if profile is default and no profile was detected make activeprofile none
                    GlobalVariables.ActiveProfile = "None";
                    GlobalVariables.ActiveApp = "None";
                }
                xmlDocument = null;
            }
            else
            {
                GlobalVariables.ActiveProfile = profileName;
                GlobalVariables.ActiveApp = "None";
            }
 

        }

        public static string loadProfileParameter(string powerStatus, string parameter, string profileName)
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
        public static void changeProfileParameter(string powerStatus, string parameter, string profileName, string newValue)
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

        public static void changeProfileName(string oldName, string newName)
        {
            
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Profiles");
            XmlNodeList xmlSelectedNodes = xmlNode.SelectNodes("Profile/ProfileName[text()='" + oldName + "']");
            if (xmlSelectedNodes != null)
            {
                foreach (XmlNode xmlNode1 in xmlSelectedNodes)
                {
                    
                    if (xmlNode1.Name == "ProfileName")
                    {
                        if (xmlNode1.InnerText == oldName) { xmlNode1.InnerText = newName; }

                    }

                }


            }

            xmlDocument.Save(GlobalVariables.xmlFile);

            xmlDocument = null;

        }


        public static void applyProfile()
        {

        }
    }
    public static class ManageXML_Apps
    {
        public static DataTable appList()
        {
            DataTable dt = new DataTable();
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");

            dt.Columns.Add("DisplayName");

            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.SelectSingleNode("DisplayName") != null)
                {
                    dt.Rows.Add(node.SelectSingleNode("DisplayName").InnerText);
                }
                
            }
            xmlDocument = null;
            return dt;


        }

        public static DataTable appListProfileExe()
        {
            DataTable dt = new DataTable();
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");

            dt.Columns.Add("Profile");
            dt.Columns.Add("Exe");
            if (xmlNode.ChildNodes.Count > 0)
            {
                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    if (node.SelectSingleNode("Profile") != null)
                    {
                        if (node.SelectSingleNode("Profile").InnerText != "")
                        {
                            dt.Rows.Add(node.SelectSingleNode("Profile").InnerText, node.SelectSingleNode("Exe").InnerText);
                        }
                    }

                }
            }

            xmlDocument = null;
            return dt;


        }

        public static string lookupProfileByAppExe(string exe)
        {
           
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNodeList xmlNodes = xmlDocument.SelectSingleNode("//Configuration/Applications").SelectNodes("App/Exe[text()='" + exe + "']");
            string profile = "";
    

            foreach (XmlNode node in xmlNodes)
            {
                profile = node.ParentNode.SelectSingleNode("Profile").InnerText;
            }
            xmlDocument = null;
            return profile;


        }
        public static DataTable appListByProfile(string profileName)
        {
            DataTable dt = new DataTable();
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNodeList xmlNodes = xmlDocument.SelectSingleNode("//Configuration/Applications").SelectNodes("App/Profile[text()='" + profileName + "']");

            dt.Columns.Add("DisplayName");

            foreach (XmlNode node in xmlNodes)
            {
      
                dt.Rows.Add(node.ParentNode.SelectSingleNode("DisplayName").InnerText);
            }
            xmlDocument = null;
            return dt;


        }
        public static void changeProfileNameInApps(string oldName, string newName)
        {

            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");
            XmlNodeList xmlSelectedNodes = xmlNode.SelectNodes("App/Profile[text()='" + oldName + "']");
            if (xmlSelectedNodes != null)
            {
                foreach (XmlNode xmlNode1 in xmlSelectedNodes)
                {
                    
                    if (xmlNode1.Name == "Profile")
                    {
                        if (xmlNode1.InnerText == oldName) { xmlNode1.InnerText = newName; }

                    }

                }


            }

            xmlDocument.Save(GlobalVariables.xmlFile);

            xmlDocument = null;

        }

        public static void createApp(string ProfileName = "")
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNodeTemplate = xmlDocument.SelectSingleNode("//Configuration/AppTemplate/App");
            XmlNode xmlNodeProfiles = xmlDocument.SelectSingleNode("//Configuration/Applications");

            string newAppName = "NewApp";
            if (ProfileName != "")
            {
                newAppName = ProfileName + "_App";
            }
            
            int countApp = 0;
            XmlNodeList xmlNodesByName = xmlNodeProfiles.SelectNodes("App/DisplayName[text()='" + newAppName + "']");

            if (xmlNodesByName.Count > 0)
            {
                while (xmlNodesByName.Count > 0)
                {
                    countApp++;
                    xmlNodesByName = xmlNodeProfiles.SelectNodes("App/DisplayName[text()='" + newAppName + countApp.ToString() + "']");

                }
                newAppName = newAppName + countApp.ToString();
            }

            XmlNode newNode = xmlDocument.CreateNode(XmlNodeType.Element, "App", "");
            newNode.InnerXml = xmlNodeTemplate.InnerXml;
            newNode.SelectSingleNode("DisplayName").InnerText = newAppName;
            newNode.SelectSingleNode("Profile").InnerText = ProfileName;
            xmlNodeProfiles.AppendChild(newNode);

            xmlDocument.Save(GlobalVariables.xmlFile);

            xmlDocument = null;

        }
        public static void deleteApp(string appName)
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


        public static void saveAppArray(string[] result, string appName)
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
                        
                        if (node.Name == "Exe") { node.InnerText = result[0]; }
                        if (node.Name == "Path") { node.InnerText = result[1]; }
                        if (node.Name == "AppType") { node.InnerText = result[2]; }
                        if (node.Name == "GameType") { node.InnerText = result[3]; }
                        if (node.Name == "Image") { node.InnerText = result[4]; }
                        if (node.Name == "Profile") { node.InnerText = result[5]; }
  
                    }

          
                }


            }
            xmlDocument.Save(GlobalVariables.xmlFile);

            xmlDocument = null;


        }
        public static string[] loadAppArray(string appName)
        {
            string[] result = new string[6];
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
                        // not needed, already gotten if (node.Name == "DisplayName") { result[0] = node.InnerText; }
                        if (node.Name == "Exe") { result[0] = node.InnerText; }
                        if (node.Name == "Path") { result[1] = node.InnerText; }
                        if (node.Name == "AppType") { result[2] = node.InnerText; }
                        if (node.Name == "GameType") { result[3] = node.InnerText; }
                        if (node.Name == "Image") { result[4] = node.InnerText; }
                        if (node.Name == "Profile") { result[5] = node.InnerText; }
                        
                     
                    }



                }


            }

            xmlDocument = null;
            return result;
        }

        public static string loadAppParameter(string parameter, string appName)
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
        public static void changeAppParameter(string parameter, string appName, string newValue)
        {
    
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(GlobalVariables.xmlFile);
            XmlNode xmlNode = xmlDocument.SelectSingleNode("//Configuration/Applications");
            XmlNode xmlSelectedNode = xmlNode.SelectSingleNode("App/DisplayName[text()='" + appName + "']");
            if (xmlSelectedNode != null)
            {

                XmlNode parameterNode = xmlSelectedNode.ParentNode.SelectSingleNode(parameter);
          
                parameterNode.InnerText = newValue;
            }
            xmlDocument.Save(GlobalVariables.xmlFile);
            xmlDocument = null;

        }



    }
}
