﻿using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace MKV_Chapterizer
{
    public partial class AutoUpdate : Form
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private string[] pUpdateInfo;
        private string pMainExe;
        private string[] pApiURLs;

        public delegate void DownloadProgressDelegate(int percProgress);

        BackgroundWorker bwUpdateProgram = new BackgroundWorker();
        BackgroundWorker bwCheckUpdates = new BackgroundWorker();

        public string[] UpdateInfo
        {
            get { return pUpdateInfo; }
            set { pUpdateInfo = value; }
        }

        public string MainExe
        {
            get { return pMainExe; }
            set { pMainExe = value; }
        }

        public string[] ApiURLs
        {
            get { return pApiURLs; }
            set { pApiURLs = value; }
        }

        public AutoUpdate()
        {
            InitializeComponent();

            //Get command line args and save into the data

            string[] args = Environment.GetCommandLineArgs();

            for (int i = 0; i <= args.Count() - 1; i++)
            {
                switch (args[i])
                {
                    case "-exe":
                        MainExe = args[i + 1];
                        break;

                    case "-apiurls":
                        ApiURLs = args[i + 1].Split('|');
                        break;
                }
            }

            if (ApiURLs.Length == null || string.IsNullOrEmpty(MainExe))
            {
                //Wrong arguments, notify and quit
                MessageBox.Show("Wrong arguments specified!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            bwUpdateProgram.RunWorkerCompleted +=new RunWorkerCompletedEventHandler(bwUpdateProgram_RunWorkerCompleted);
            bwUpdateProgram.DoWork +=new DoWorkEventHandler(bwUpdateProgram_DoWork);
            bwUpdateProgram.ProgressChanged +=new ProgressChangedEventHandler(bwUpdateProgram_ProgressChanged);
            bwUpdateProgram.WorkerReportsProgress = true;
            bwUpdateProgram.WorkerSupportsCancellation = true;

            btnUpdate.Select();

            if (UpdateAvailable())
            {
                lblNewUpdate.Text = string.Format("Version {0} is available for download", UpdateInfo[1]);
                txtChangelog.Text = UpdateInfo[3];
                btnUpdate.Text = "Update";
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private bool UpdateAvailable()
        {
            FileVersionInfo fi;

            if (File.Exists(MainExe))
            {
                fi = FileVersionInfo.GetVersionInfo(MainExe);
            }
            else
            {
                return false;
            }

            UpdateInfo = GetUpdateInfo(ApiURLs);

            Version curVersion = Version.Parse(fi.FileVersion);
            Version newVersion;

            if (UpdateInfo != null)
            {
                newVersion = new Version(UpdateInfo[1]);
            }
            else
            {
                MessageBox.Show("Failed to check for updates, maybe the update server is down?", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (newVersion > curVersion)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ExitMainProgram(string exe)
        {
            Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(exe));

            foreach (Process p in processes)
            {
                uint WM_CLOSE = 0x10;
                IntPtr pFoundWindow = p.MainWindowHandle;
                SendMessage(pFoundWindow, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private string[] GetUpdateInfo(string[] APIUrls)
        {
            WebClient client = new WebClient();
            string xml = string.Empty;

            try
            {
                xml = client.DownloadString(new Uri(APIUrls[0]));
            }
            catch (WebException)
            {
                //Failed to connect, try the other URL
                try
                {
                    xml = client.DownloadString(new Uri(APIUrls[1]));
                }
                catch (WebException)
                {
                    return null;
                }
            }
            XDocument xdoc = XDocument.Parse(xml);
            XElement xe = xdoc.Root.Element("program");

            string[] data = new string[7];

            data[0] = xe.Element("name").Value;
            data[1] = xe.Element("version").Value;
            data[2] = xe.Element("beta").Value;
            data[3] = xe.Element("changelog").Value;
            data[4] = xe.Element("downloadURL").Value;
            data[5] = xe.Element("downloadURLType").Value;
            data[6] = xe.Element("downloadPath").Value;

            return data;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            bwUpdateProgram.RunWorkerAsync();
            btnUpdate.Enabled = false;
        }

        private void bwUpdateProgram_DoWork(object sender, DoWorkEventArgs e)
        {
            //Create workdir
            try
            {
                Directory.CreateDirectory("update");
            }
            catch (Exception)
            {
                return;
            }

            //Download the updatefile

            DownloadProgressDelegate progressUpdate = new DownloadProgressDelegate(downloadFileProgressChanged);
            bool result = false;

            //Check if the updateurl is any specific type
            //Not needed anymore...
            /*
            switch (UpdateInfo[5])
            {
                case "freedns.afraid.org":

                    //The real URL needs to be fetched
                    UpdateInfo[4] = FetchFreeDNSURL(UpdateInfo[4]);
                    break;
            }
            */

            try
            {
                string url = UpdateInfo[4] + UpdateInfo[6] + UpdateInfo[1] + ".exe";
                result = Download(url, "update\\" + UpdateInfo[1] + ".exe", progressUpdate);
            }
            catch (Exception ex)
            {
                e.Result = ex;
                return;
            }

            if (result == true)
            {
                //Download succeeded, notify the user and start the setup
                e.Result = UpdateInfo[1] + ".exe";
            }
                
        }

        private void bwUpdateProgram_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbarDownloadProgress.Value = e.ProgressPercentage;
        }

        private void bwUpdateProgram_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result.GetType().Name == "String")
            {
                MessageBox.Show("MKV Chapterizer will now be closed to prepare for the update, \r\nso please save all your work before proceeding!", "Update Downloaded Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ExitMainProgram(MainExe);
                Process.Start("update\\" + e.Result);
                Application.Exit();
            }
            else if (e.Result.GetType().Name == "Exception")
            {
                MessageBox.Show("Error Occured: \r\n" + (Exception)e.Result, "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        //This function is apparently not needed anymore
        private string FetchFreeDNSURL(string url)
        {
            WebClient client = new WebClient();
            string html = client.DownloadString(url);

            //Find the frame-tag that holds the redirect URL
            Regex regex = new Regex("<frame target=\"random_name_not_taken2\" name=\"random_name_not_taken2\" src=\".+\" border=\"0\" noresize>", RegexOptions.IgnoreCase);
            Match match = regex.Match(html);

            regex = new Regex("src=\".*?\"");

            string realURL = regex.Match(match.Value).Value.Substring(5).Replace("\"", "");

            return realURL;
        }

        private void downloadFileProgressChanged(int percentage)
        {
            bwUpdateProgram.ReportProgress(percentage);
        }

        /// <summary>
        /// Downloads the specified URL. Returns true if success.
        /// </summary>
        /// <param name="uri">The URL.</param>
        /// <param name="localPath">The local path.</param>
        /// <param name="progressDelegate">The progress delegate.</param>
        /// <returns></returns>
        public static bool Download(string url, string localPath, DownloadProgressDelegate progressDelegate)
        {
            long remoteSize;
            string fullLocalPath; // Full local path including file name if only directory was provided.

            try
            {
                /// Get the name of the remote file.
                Uri remoteUri = new Uri(url);
                string fileName = Path.GetFileName(remoteUri.LocalPath);

                if (Path.GetFileName(localPath).Length == 0)
                    fullLocalPath = Path.Combine(localPath, fileName);
                else
                    fullLocalPath = localPath;

                /// Have to get size of remote object through the webrequest as not available on remote files,
                /// although it does work on local files.
                using (WebResponse response = WebRequest.Create(url).GetResponse())
                using (Stream stream = response.GetResponseStream())
                    remoteSize = response.ContentLength;

            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Error connecting to URI (Exception={0})", ex.Message), ex);
            }

            int bytesRead = 0, bytesReadTotal = 0;

            try
            {
                using (WebClient client = new WebClient())
                using (Stream streamRemote = client.OpenRead(new Uri(url)))
                using (Stream streamLocal = new FileStream(fullLocalPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    byte[] byteBuffer = new byte[1024 * 1024 * 2]; // 2 meg buffer although in testing only got to 10k max usage.
                    int perc = 0;
                    while ((bytesRead = streamRemote.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
                    {
                        bytesReadTotal += bytesRead;
                        streamLocal.Write(byteBuffer, 0, bytesRead);
                        int newPerc = (int)((double)bytesReadTotal / (double)remoteSize * 100);
                        if (newPerc > perc)
                        {
                            perc = newPerc;
                            if (progressDelegate != null)
                                progressDelegate(perc);
                        }
                    }
                }

                progressDelegate(100);
                return true; //File succeeded downloading
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Error downloading file (Exception={0})", ex.Message), ex);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //Clean-up
            if (CleanUp() == false)
            {
                if (MessageBox.Show("Failed to clean up update directory!", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Retry)
                {
                    CleanUp();
                }
            }

            this.Close();
        }

        private bool CleanUp()
        {
            if (Directory.Exists("update\\"))
            {
                try
                {
                    Directory.Delete("update\\", true);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            //Clean-up succeeded
            return true;
        }
    }
}


