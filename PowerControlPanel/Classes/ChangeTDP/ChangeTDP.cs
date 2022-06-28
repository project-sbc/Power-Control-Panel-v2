using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Power_Control_Panel;
using System.Threading;
using System.Diagnostics;

namespace Power_Control_Panel.PowerControlPanel.Classes.ChangeTDP
{
    public class ChangeTDP
    {
        static string cpuType = "";
        static string MCHBAR = "";
        static string RWDelay = Properties.Settings.Default.RWDelay;
        private static Object objLock = new Object();

        public static string BaseDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);


 
        public static void readTDP2()
        {

            try
            {
                //add small delay to prevent write and read operations from interfering
                Thread.Sleep(100);
                string tdp = null;
                determineCPU();
                if (cpuType == "Intel") {tdp = runIntelReadTDPMSR(); }
                else { if (cpuType == "AMD") {  } }

                double dblPL1 = Convert.ToDouble(tdp.Substring(0, tdp.IndexOf(";")));
                GlobalVariables.PL1 = dblPL1;
               
                double dblPL2 = Convert.ToDouble(tdp.Substring(tdp.IndexOf(";") + 1, tdp.Length - tdp.IndexOf(";") - 1));
                GlobalVariables.PL2 = dblPL2;
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Reading TDP: " + ex.Message;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);

                //return "Error";
            }

        }
        public static async Task<string> readTDP()
        {

            try
            {
                //add small delay to prevent write and read operations from interfering
                Thread.Sleep(100);
                string tdp = null;
                determineCPU();
                if (cpuType == "Intel") { tdp = runIntelReadTDPMSR(); }
                else { if (cpuType == "AMD") { } }

                return tdp;
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Reading TDP: " + ex.Message;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);

                return "Error";
            }

        }
        public static async Task<string> changeTDP(int pl1TDP, int pl2TDP)
        {
            //Return Success as default value, otherwise alert calling routine to error
            try
            {
                determineCPU();
     
                if (cpuType == "Intel") {await Task.Run(() => runIntelTDPChange(pl1TDP, pl2TDP)); }
                else { if (cpuType == "AMD") { await Task.Run(() => runAMDTDPChange(pl1TDP, pl2TDP)); } }
              
                return "Success";
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Changing TDP: " + ex.Message;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);
                return "Error";
            }

        }


        static void determineCPU()
        {
            try
            {
                if (cpuType != "Intel" && cpuType != "AMD")
                {
                    //Get the processor name to determine intel vs AMD
                    object processorNameRegistry = Registry.GetValue("HKEY_LOCAL_MACHINE\\hardware\\description\\system\\centralprocessor\\0", "ProcessorNameString", null);
                    string processorName = null;
                    if (processorNameRegistry != null)
                    {
                        //If not null, find intel or AMD string and clarify type. For Intel determine MCHBAR for rw.exe
                        processorName = processorNameRegistry.ToString();
                        if (processorName.IndexOf("Intel") >= 0) { cpuType = "Intel"; }
                        if (processorName.IndexOf("AMD") >= 0) { cpuType = "AMD"; }
                    }
                }
                if (cpuType == "Intel" && MCHBAR == "")
                {
                    determineIntelMCHBAR();
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs: Determining CPU type: " + ex.Message;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);
            }
        }

        static void determineIntelMCHBAR()
        {
            try
            {
                //Get the processor model to determine MCHBAR, INTEL ONLY
                object processorModelRegistry = Registry.GetValue("HKEY_LOCAL_MACHINE\\hardware\\description\\system\\centralprocessor\\0", "Identifier", null);
                string processorModel = null;
                if (processorModelRegistry != null)
                {
                    //If not null, convert to string and determine MCHBAR for rw.exe
                    processorModel = processorModelRegistry.ToString();
                    if (processorModel.IndexOf("Model 140") >= 0) { MCHBAR = "0xFEDC59"; } else { MCHBAR = "0xFED159"; };
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs: Determining MCHBAR: " + ex.Message;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);
            }

        }

       
        static string convertTDPToHex(int tdp)
        {
            //Convert integer TDP value to Hex for rw.exe
            //Must use formula (TDP in watt   *1000/125) +32768 and convert to hex
            try
            {
                int newTDP = (tdp * 1000 / 125) + 32768;
                return newTDP.ToString("X");

            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  convert TDP To Hex: " + ex.Message;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);
                return "Error";
            }
        }
        static string runIntelTDPChange(int pl1TDP, int pl2TDP)
        {
            try
            {
                string processRW = BaseDir + "\\Resources\\Intel\\RW\\Rw.exe";
                string hexPL1 = convertTDPToHex(pl1TDP);
                string hexPL2 = convertTDPToHex(pl2TDP);
                if (hexPL1 != "Error" && hexPL2 != "Error" && MCHBAR != null)
                {
                    lock (objLock)
                    {
                        string commandArguments = " /nologo /stdout /command=" + '\u0022' + "Delay " + RWDelay + "; w16 " + MCHBAR + "a0 0x" + hexPL1 + "; Delay " + RWDelay + "; w16 " + MCHBAR + "a4 0x" + hexPL2 + "; Delay " + RWDelay + ";" + '\u0022';

                        RunCLI.RunCommand(commandArguments, false, processRW);
                        Thread.Sleep(100);
                        return "Success";

                    }

                }




                else { return "Error"; }


            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Run Intel TDP Change: " + ex.Message;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);
                return "Error";
            }


        }

        static string runIntelReadTDPMMIO()
        {
            string tdp = null;
            tdp = "Error";
           
            try
            {
                string processRW = BaseDir + "\\Resources\\Intel\\RW\\Rw.exe";
                if (MCHBAR != null)
                {
                    lock (objLock)
                    {
                        string commandArguments = " /nologo /stdout /command=" + '\u0022' + "Delay " + RWDelay + "; r16 " + MCHBAR + "a0; Delay " + RWDelay + "; r16 " + MCHBAR + "a4; Delay " + RWDelay + ";" + '\u0022';

                        string result = RunCLI.RunCommand(commandArguments, true, processRW);
                        if (result != null)
                        {
                            tdp = parseHexFromResultMMIOConvertToTDP(result,true);
                            tdp = tdp + ";" + parseHexFromResultMMIOConvertToTDP(result, false);

                            return tdp;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Unable to get MCHBAR for intel CPU");
                    tdp = "Error";
                   
                    return tdp;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs: Reading intel tdp: " + ex.Message;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);
                tdp = "Error";
            
                return tdp;
            }
            return tdp;
        }
        static string runIntelReadTDPMSR()
        {
            string tdp = null;
            tdp = "Error";

            try
            {
                string processRW = BaseDir + "\\Resources\\Intel\\RW\\Rw.exe";
                if (MCHBAR != null)
                {
                    lock (objLock)
                    {
                        string commandArguments = " /nologo /stdout /command=" + '\u0022' + "RDMSR 0x610 0x0 0x00000000 0;" + '\u0022';

                        string result = RunCLI.RunCommand(commandArguments, true, processRW);
                        if (result != null)
                        {
                            tdp = parseHexFromResultMSRConvertToTDP(result, true);
                            tdp = tdp + ";" + parseHexFromResultMSRConvertToTDP(result, false);

                            return tdp;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Unable to get MCHBAR for intel CPU");
                    tdp = "Error";

                    return tdp;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs: Reading intel tdp: " + ex.Message;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);
                tdp = "Error";

                return tdp;
            }
            return tdp;
        }
        static string parseHexFromResultMMIOConvertToTDP(string result, bool isPL1)
        {
            try
            {
                int FindString;
                string hexResult;
                float intResult;
                if (isPL1)
                {
                    FindString = result.IndexOf(MCHBAR + "A0 =") + MCHBAR.Length + 4;
                    hexResult = result.Substring(FindString,7).Trim();
                    intResult = (Convert.ToInt32(hexResult, 16) - 32768) / 8;
                    return intResult.ToString(); 
                   
                }
                else
                {
                    FindString = result.IndexOf(MCHBAR + "A4 =") + MCHBAR.Length + 4;
                    hexResult = result.Substring(FindString, 7).Trim();
                    intResult = (Convert.ToInt32(hexResult, 16) - 32768) / 8;
                    return intResult.ToString();
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Parse intel tdp from result: " + ex.Message;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);
                return "Error";
            }



        }

        static string parseHexFromResultMSRConvertToTDP(string result, bool isPL1)
        {
            try
            {
                int FindString;
                string hexResult;
                float intResult;
                if (isPL1)
                {
                    FindString = result.IndexOf("0x00DD8") + 7;
                    hexResult = result.Substring(FindString, 3).Trim();
                    intResult = (Convert.ToInt32(hexResult, 16)) / 8;
                    return intResult.ToString();

                }
                else
                {
                    FindString = result.IndexOf("0x00428") + 7;
                    hexResult = result.Substring(FindString, 3).Trim();
                    intResult = (Convert.ToInt32(hexResult, 16)) / 8;
                    return intResult.ToString();
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Parse intel tdp from result: " + ex.Message;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);
                return "Error";
            }



        }
        static string runAMDTDPChange(int pl1TDP, int pl2TDP)
        {
            
            //NOT COMPLETE
            try
            {
                
                return "Success";
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeTDP.cs:  Run AMD TDP Change: " + ex.Message;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);
                return "Error";
            }


        }
       
    }
}
