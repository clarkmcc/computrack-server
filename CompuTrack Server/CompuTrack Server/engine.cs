using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;
using System.Reflection;
using static CompuTrack_Server.consoleWriter;
using System.IO;
using System.Timers;

namespace CompuTrack_Server
{
    class engine
    {
        public static class global
        {
            public static int cpuUsage = 90;
            public static int ramUsage = 80;
            public static int freeSpace = 5;
        }

        public static void startTimer(int orgKey)
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(runLiveData);
            aTimer.Interval = 5000;
            aTimer.Enabled = true;
        }

        private static void runLiveData(object source, ElapsedEventArgs e)
        {
            alertEngine.engine(323000);

        }

        public class alertEngine
        {
            

            public static void engine(int orgKey)
            {
                checkAssets(orgKey);
            }

            public static string checkAssets(int orgKey)
            {
                List<string> assetValues = new List<string>();
                List<string> getValues = new List<string>();

                getValues.Add("ramUsage");
                getValues.Add("cpuUsage");
                getValues.Add("freeSpace");

                Console.WriteLine("Added List Values");

                if (orgKey != 0)
                {
                    using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.DB_98559_compConnectionString))
                    {
                        Console.WriteLine("Selecting assets from org");
                        conn.Open();
                        string getAssetsQuery = "SELECT compKey FROM tComputers WHERE orgKey='" + orgKey + "'";
                        SqlCommand getAssets = new SqlCommand(getAssetsQuery, conn);

                        getAssets.ExecuteNonQuery();
                        SqlDataReader dr = getAssets.ExecuteReader();

                        List<int> orgAssetKeys = new List<int>();

                        int count = 0;
                        while (dr.Read())
                        {

                            count++;
                            int compKey = Convert.ToInt32(dr["compKey"]);
                            orgAssetKeys.Add(compKey);
                            Console.WriteLine("Reading Assets " + compKey);
                        }

                        foreach (int asset in orgAssetKeys)
                        {
                            Console.WriteLine("foreach asset in orgAssetKeys: " + asset);
                            foreach (string cmd in getValues)
                            {
                                Console.WriteLine("foreach cmd in getValues: " + cmd);
                                using (SqlConnection conn2 = new SqlConnection(Properties.Settings.Default.DB_98559_compConnectionString))
                                {
                                    Console.WriteLine("Connection Active");
                                    conn2.Open();
                                    string getAssetInfoQuery = "CREATE TABLE #temp(liveDataHistoryKey int, compKey int, " + cmd + " decimal(18,2),)INSERT INTO #temp SELECT a.* FROM (SELECT TOP(20) liveDataHistoryKey, compKey," + cmd + " FROM tLiveDataHistory WHERE compKey = " + asset + " ORDER BY liveDataHistoryKey desc) a ORDER BY a.liveDataHistoryKey asc SELECT AVG(" + cmd + ") " + cmd + " FROM #temp DROP TABLE #temp";
                                    SqlCommand getAssetInfo = new SqlCommand(getAssetInfoQuery, conn2);
                                    getAssetInfo.ExecuteNonQuery();

                                    SqlDataReader dr2 = getAssetInfo.ExecuteReader();
                                    while (dr2.Read())
                                    {
                                        Console.WriteLine("Datareader Active");
                                        decimal tempValue = Convert.ToDecimal(dr2[@cmd]);
                                        assetValues.Add(asset + "," + cmd + "," + tempValue);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    return "ERROR";
                }
                foreach (string response in assetValues)
                {
                    assetAlertEngine(response, orgKey);
                }
                return "";
            }

            public static void assetAlertEngine(string response, int orgKey)
            {
                string[] assetAlert = response.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                string[] assetThreshold = dynamicMethod("global", assetAlert[1]).Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); ;

                Console.WriteLine("ENGINE: for " + assetAlert[0] + ": " + assetAlert[1] + " if (" + assetAlert[2] + " " + assetThreshold[1] + " " + assetThreshold[0] + ") {}");

                if ( assetThreshold[1] == ">" )
                {
                    if (Convert.ToDecimal(assetAlert[2]) > Convert.ToInt32(assetThreshold[0]))
                    {
                        createAssetAlert(Convert.ToInt32(assetAlert[0]), orgKey, "Assets " + assetAlert[1] + " has been over " + assetThreshold[0] + "% for an extended period of time.");
                    } 
                } else if ( assetThreshold[1] == "<" )
                {
                    if (assetAlert[1] == "freeSpace")
                    {
                        if (Convert.ToDecimal(assetAlert[2]) < Convert.ToInt32(assetThreshold[0]))
                        {
                            createAssetAlert(Convert.ToInt32(assetAlert[0]), orgKey, "Assets " + assetAlert[1] + " has been under " + assetThreshold[0] + "GB for an extended period of time.");
                        }
                    } else
                    {
                        Console.WriteLine("Unsupported");
                    }
                } 
            }

            private static void createAssetAlert(int compKey, int orgKey, string alert)
            {
                console("ALERT FOR ASSET " + compKey + ": " + alert);
                string alertTitle = "Alert for Asset";

                using (SqlConnection conn2 = new SqlConnection(Properties.Settings.Default.DB_98559_compConnectionString))
                {
                    conn2.Open();
                    string checkExistingAlertQuery = "SELECT * FROM tAlert WHERE ( alertAssetKey = '" + compKey + "' ) AND ( alertName = '" + alertTitle + "' ) AND ( alertStatus = 0 ) AND ( alertDesc = '" + alert + "' )";
                    SqlCommand checkExistingAlert = new SqlCommand(checkExistingAlertQuery, conn2);
                    checkExistingAlert.ExecuteNonQuery();

                    SqlDataReader dr = checkExistingAlert.ExecuteReader();
                    while(dr.Read())
                    {
                        console("|----------------- Alert exists ---------------|");
                        return;
                    }
                    
                }

                using (SqlConnection conn2 = new SqlConnection(Properties.Settings.Default.DB_98559_compConnectionString))
                {
                    conn2.Open();
                    string createAlertQuery = "INSERT INTO tAlert (orgKey, alertName, alertDesc, alertCreateDate, alertAssetKey, alertStatus) VALUES ('" + orgKey + "', '" + alertTitle + "', '" + alert + "', CURRENT_TIMESTAMP, '" + compKey + "', 0)";
                    SqlCommand createAlert = new SqlCommand(createAlertQuery, conn2);
                    createAlert.ExecuteNonQuery();
                }
            }

            private static string dynamicMethod(string classCall, string methodCall)
            {

                if (classCall == "global")
                {
                    string[] operation = new string[2];
                    if (methodCall == "cpuUsage")
                    {
                        operation[0] = global.cpuUsage.ToString();
                        operation[1] = ">";
                        return operation[0] + "," + operation[1];
                    }
                    else if (methodCall == "ramUsage")
                    {
                        operation[0] = global.ramUsage.ToString();
                        operation[1] = ">";
                        return operation[0] + "," + operation[1];
                    }
                    else if (methodCall == "freeSpace")
                    {
                        operation[0] = global.freeSpace.ToString();
                        operation[1] = "<";
                        return operation[0] + "," + operation[1];
                    }
                    else
                    {
                        return "ex";
                    }
                } else
                {
                    return "ex";
                }
            }

            public static void console(string write)
            {
                Console.WriteLine(write);
            }
        }
    }
}
