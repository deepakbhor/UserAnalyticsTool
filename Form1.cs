using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
//Added New Namespaces....
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;
using System.IO;
using System.Drawing.Imaging;
using Gma.System.MouseKeyHook;
using Gma.System.MouseKeyHook.Implementation;
using System.Security.Principal;
using CSUACSelfElevation;
using WinRobotCoreLib;



namespace WinApplWatcher
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Form1 : System.Windows.Forms.Form
    {

        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.DataGrid dataGrid1;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;


        #region Variables Declaration
        public static string appName, prevvalue = "";
        public static Stack applnames;
        public static Hashtable applhash;
        public static DateTime applfocustime;
        public static string appltitle;
        public static Form1 form1;
        public static string tempstr;
        public TimeSpan applfocusinterval;
        public DateTime logintime;
        public string appFileName = "D:\\appldetails_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xml";
        public bool isKeyPress = false;
        private DateTime lastAppKeyPressed = DateTime.Now;
        private DateTime prevKeyPressed = DateTime.Now;
        double lapsedSeconds;
        private String userRights;
        private WindowsIdentity winID;
        private List<GenericWatcherList> watchFiels = new List<GenericWatcherList>();


        //This Function is used to get Active process ID...
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr GetWindowRect(IntPtr hWnd, out RECTANGLE lprect);

        //Added by Deepak Bhor
        public IKeyboardMouseEvents m_Events;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECTANGLE
        {
            public int Left;        // x position of upper-left corner  
            public int Top;         // y position of upper-left corner  
            public int Right;       // x position of lower-right corner  
            public int Bottom;      // y position of lower-right corner  
        }

        #endregion

        [Guid("43CF023A-99C4-4867-AA3F-EA490CACE693"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IWinRobotService
        {
            void GetActiveConsoleSession([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);
            void RegSession([Out, MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);            
        }


        public Form1()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.Click += new System.EventHandler(this.notifyIcon1_Click);
            // 
            // dataGrid1
            // 
            this.dataGrid1.DataMember = "";
            this.dataGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid1.GridLineColor = System.Drawing.SystemColors.Highlight;
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Location = new System.Drawing.Point(0, 0);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.PreferredColumnWidth = 164;
            this.dataGrid1.Size = new System.Drawing.Size(536, 398);
            this.dataGrid1.TabIndex = 0;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2,
            this.menuItem3});
            this.menuItem1.Text = "&Main";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 0;
            this.menuItem2.Shortcut = System.Windows.Forms.Shortcut.F5;
            this.menuItem2.Text = "&Refresh";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 1;
            this.menuItem3.Text = "E&xit";
            this.menuItem3.Click += new System.EventHandler(this.menuItem3_Click);
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(536, 398);
            this.Controls.Add(this.dataGrid1);
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.Text = "Win Appl Watcher";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Closed += new System.EventHandler(this.Form1_Closed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.ResumeLayout(false);

        }

        private void SubscribeApplication()
        {
            Unsubscribe();
            Subscribe(Hook.AppEvents());
        }

        public void SubscribeGlobal()
        {
            Unsubscribe();
            Subscribe(Hook.GlobalEvents());
        }

        
        //Image will return memory stream of the image.....
        public Stream GetStream(Image img, ImageFormat format)
        {
            var ms = new MemoryStream();
            img.Save(ms, format);
            return ms;
        }

        private void Subscribe(IKeyboardMouseEvents events)
        {
            m_Events = events;
            m_Events.KeyDown += OnKeyDown;
            m_Events.KeyUp += OnKeyUp;
            m_Events.KeyPress += HookManager_KeyPress;

            m_Events.MouseUp += OnMouseUp;
            m_Events.MouseClick += OnMouseClick;
            m_Events.MouseDoubleClick += OnMouseDoubleClick;

            m_Events.MouseMove += HookManager_MouseMove;
            m_Events.MouseDragStarted += OnMouseDragStarted;
            m_Events.MouseDragFinished += OnMouseDragFinished;
        }

        //subscribe to the keyboard alerts....
        private void Unsubscribe()
        {
            if (m_Events == null) return;
            m_Events.KeyDown -= OnKeyDown;
            m_Events.KeyUp -= OnKeyUp;
            m_Events.KeyPress -= HookManager_KeyPress;

            m_Events.MouseUp -= OnMouseUp;
            m_Events.MouseClick -= OnMouseClick;
            m_Events.MouseDoubleClick -= OnMouseDoubleClick;

            m_Events.MouseMove -= HookManager_MouseMove;

            m_Events.MouseDragStarted -= OnMouseDragStarted;
            m_Events.MouseDragFinished -= OnMouseDragFinished;

            m_Events.Dispose();
            m_Events = null;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            lastAppKeyPressed = DateTime.Now;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            lastAppKeyPressed = DateTime.Now;
        }

        private void HookManager_KeyPress(object sender, KeyPressEventArgs e)
        {
            lastAppKeyPressed = DateTime.Now;
        }

        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {

        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {

        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {

        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void OnMouseDragStarted(object sender, MouseEventArgs e)
        {

        }

        private void OnMouseDragFinished(object sender, MouseEventArgs e)
        {

        }

        private void checkBoxSupressMouseWheel_CheckedChanged(object sender, EventArgs e)
        {
            if (m_Events == null) return;

            if (((CheckBox)sender).Checked)
            {
                m_Events.MouseWheel -= HookManager_MouseWheel;
                m_Events.MouseWheelExt += HookManager_MouseWheelExt;
            }
            else
            {
                m_Events.MouseWheelExt -= HookManager_MouseWheelExt;
                m_Events.MouseWheel += HookManager_MouseWheel;
            }
        }


        private void HookManager_MouseWheel(object sender, MouseEventArgs e)
        {

        }

        private void HookManager_MouseWheelExt(object sender, MouseEventExtArgs e)
        {
            e.Handled = true;
        }

        #endregion

        [STAThread]
        static void Main()
        {
            Guid CLSID_CTask = new Guid("{D08018BD-6958-4A2E-95EA-FEC13211DA0F}");
            Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");            
            const uint CLSCTX_INPROC_SERVER = 1;
            windowsService OSched = new windowsService();
            IntPtr pIUnk = Marshal.GetIUnknownForObject(OSched);
            Int32 usecount1 = Marshal.AddRef(pIUnk);
            Int32 usecount2 = Marshal.Release(pIUnk);
            IntPtr pISched;
            Guid IidSched = new Guid("43CF023A-99C4-4867-AA3F-EA490CACE693");
            Int32 result = Marshal.QueryInterface(pIUnk, ref IidSched, out pISched);
            IWinRobotService ITaskSchd = OSched as IWinRobotService;
            object test = new windowsSession();
            //ITaskSchd.GetActiveConsoleSession(out test);
            applnames = new Stack();
            applhash = new Hashtable();
            form1 = new Form1();
            Application.Run(form1);
        }

        //Timer event to control the various audit options.....
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            //This is used to monitor and save active application's  details in Hashtable for future saving in xml file...
            try
            {
                bool isNewAppl = false;
                GenericWatcherList genWatchList = new GenericWatcherList();
                form1.SubscribeGlobal();
                //IntPtr hwnd = APIFuncs.getforegroundWindow(); -- commented by Deepak
                var hwnd = APIFuncs.getforegroundWindow();
                Int32 pid = APIFuncs.GetWindowProcessID(hwnd);
                Process p = Process.GetProcessById(pid);
                appName = p.ProcessName;
                appltitle = APIFuncs.ActiveApplTitle().Trim().Replace("\0", "");
                winID = WindowsIdentity.GetCurrent();
                //genWatchList.seconds = 0;
                WindowsPrincipal winPrincipal = new WindowsPrincipal(winID);
                Boolean blnIsAdmin = winPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
                Boolean isUserAdminInGroup = IsUserInAdminGroup();
                if (appName.Equals("mmc", StringComparison.InvariantCultureIgnoreCase))
                {
                    p.Kill();
                    MessageBox.Show("You do not have sufficient privileges to run this process, Please contact administrator");
                }
                if (isUserAdminInGroup)
                {
                    genWatchList.isUserInAdminGroup = "YES";
                }

                if (blnIsAdmin)
                {
                    userRights = "Administrator";
                }

                if (!applnames.Contains(appltitle + "$$$!!!" + appName))
                {
                    applnames.Push(appltitle + "$$$!!!" + appName);
                    //applhash.Add(appltitle+"$$$!!!"+appName, 0);
                    genWatchList.appTitle = APIFuncs.ActiveApplTitle().Trim().Replace("\0", "");
                    genWatchList.windowsId = WindowsIdentity.GetCurrent().Name.ToString().Trim();
                    genWatchList.processName = p.ProcessName;
                    watchFiels.Add(genWatchList);
                    isNewAppl = true;
                    lapsedSeconds = 0;
                }

                if (prevvalue.Equals(appltitle + "$$$!!!" + appName))
                {
                    if (prevKeyPressed != lastAppKeyPressed)
                    {
                        TimeSpan appLastKeyPressInterval = DateTime.Now.Subtract(lastAppKeyPressed);
                        lapsedSeconds = lapsedSeconds + appLastKeyPressInterval.TotalSeconds;
                        prevKeyPressed = lastAppKeyPressed;
                    }
                }
                else
                {
                    IDictionaryEnumerator en = applhash.GetEnumerator();
                    applfocusinterval = DateTime.Now.Subtract(applfocustime);

                    foreach (var item in watchFiels)
                    {
                        string prevItem = item.appTitle + "$$$!!!" + item.processName;
                        if (prevItem.Equals(prevvalue))
                        {
                            double prevseconds = Convert.ToDouble(item.seconds);
                            item.seconds = (applfocusinterval.TotalSeconds + prevseconds) - lapsedSeconds;
                            //applhash.Add(prevvalue, (applfocusinterval.TotalSeconds + prevseconds) - lapsedSeconds);
                            break;
                        }
                    }
                    prevvalue = appltitle + "$$$!!!" + appName;
                    applfocustime = DateTime.Now;
                }
                // Get and display the process integrity level.
                int IL = GetProcessIntegrityLevel();
                switch (IL)
                {
                    case NativeMethods.SECURITY_MANDATORY_UNTRUSTED_RID:
                        genWatchList.integratedSecurityLevel = "Untrusted"; break;
                    case NativeMethods.SECURITY_MANDATORY_LOW_RID:
                        genWatchList.integratedSecurityLevel = "Low"; break;
                    case NativeMethods.SECURITY_MANDATORY_MEDIUM_RID:
                        genWatchList.integratedSecurityLevel = "Medium"; break;
                    case NativeMethods.SECURITY_MANDATORY_HIGH_RID:
                        genWatchList.integratedSecurityLevel = "High"; break;
                    case NativeMethods.SECURITY_MANDATORY_SYSTEM_RID:
                        genWatchList.integratedSecurityLevel = "System"; break;
                    default:
                        genWatchList.integratedSecurityLevel = "Unknown"; break;
                }
                //Added by Deepak Bhor
                /*if (!appName.Equals("chrome", StringComparison.InvariantCultureIgnoreCase))
                {
                    var rect = new RECTANGLE();                    

                    GetWindowRect(hwnd, out rect);

                    Rectangle bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

                    Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);

                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                    }

                        bmp.Save("D:\\test.png", ImageFormat.Png);
                }*/
                    if (isNewAppl)
                        {
                            applfocustime = DateTime.Now;
                        }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ":" + ex.StackTrace);
            }
        }
        private void Form1_Closed(object sender, System.EventArgs e)
        {
            //This is activated on click on Exit menu item and to show actual and calculated time spent on all applications
            //opened so far and to open IE to show xml contents....
            try
            {
                SaveandShowDetails();
                TimeSpan timeinterval = DateTime.Now.Subtract(logintime);
                System.Diagnostics.EventLog.WriteEntry("Application Watcher Total Time Details", timeinterval.Hours + " Hrs " + timeinterval.Minutes + " Mins", System.Diagnostics.EventLogEntryType.Information);
                MessageBox.Show("Actual Time Spent :" + timeinterval.Hours + " Hrs " + timeinterval.Minutes + " Mins", "Application Watcher Total Time Details");

                //commented by Deepak
                //StreamReader freader = new StreamReader(@"c:\appldetails.xml");
                //Added by Deepak
                StreamReader freader = new StreamReader(appFileName);
                XmlTextReader xmlreader = new XmlTextReader(freader);
                string tottime = "";
                while (xmlreader.Read())
                {
                    if (xmlreader.NodeType == XmlNodeType.Element && xmlreader.Name == "TotalSeconds")
                    {
                        tottime += ";" + xmlreader.ReadInnerXml().ToString();
                    }
                }
                string[] tottimes = tottime.Split(';');
                long totsecs = 0;
                foreach (string str in tottimes)
                {
                    if (str != string.Empty)
                    {
                        if (str.IndexOf("Seconds") != -1)
                        {
                            totsecs += Convert.ToInt64(str.Substring(0, str.Length - 8));
                        }
                        else
                        {
                            totsecs += Convert.ToInt64(str.Substring(0, str.Length - 8)) * 60;
                        }
                    }
                }
                Unsubscribe();
                MessageBox.Show((totsecs / 60) + " Minutes");
                showdetailsinIE();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #region User-defined Methods...
        private void showdetailsinIE()
        {
            //To create XSL file,if it is not existing....
            //if(!File.Exists(@"c:\appl_xsl.xsl")) -- commented by Deepak Bhor
            if (!File.Exists(appFileName))
            {
                //File.Create(@"c:\appl_xsl.xsl").Close(); -- commented by Deepak Bhor
                File.Create(appFileName).Close();
                string xslcontents = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><xsl:stylesheet version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\"><xsl:template match=\"/\"> <html> <body>  <h2>My Applications Details</h2>  <table border=\"1\"> <tr bgcolor=\"#9acd32\">  <th>Window Title</th>  <th>Process Name</th>  <th>Total Time</th> <th>Current Windows Identity</th> </tr> </tr> <xsl:for-each select=\"ApplDetails/Application_Info\"><xsl:sort select=\"ApplicationName\"/> <tr>  <td><xsl:value-of select=\"ProcessName\"/></td>  <td><xsl:value-of select=\"ApplicationName\"/></td>  <td><xsl:value-of select=\"TotalSeconds\"/></td> <td><xsl:value-of select=\"currentWindowsIdentity\"/></td></tr></xsl:for-each></table></body></html></xsl:template></xsl:stylesheet>";
                StreamWriter xslwriter = new StreamWriter(@"c:\appl_xsl.xsl");
                xslwriter.Write(xslcontents);
                xslwriter.Flush();
                xslwriter.Close();
            }
            //TO show the contents of xml file in IE with a proper xsl....
            System.Diagnostics.Process ie = new Process();
            //System.Diagnostics.ProcessStartInfo ieinfo = new ProcessStartInfo(@"C:\Program Files\Internet Explorer\iexplore.exe",@"c:\appldetails.xml"); changed by Deepak Bhor
            System.Diagnostics.ProcessStartInfo ieinfo = new ProcessStartInfo(@"C:\Program Files\Internet Explorer\iexplore.exe", appFileName);
            ie.StartInfo = ieinfo;
            bool started = ie.Start();
            Application.Exit();
        }
        private void TestFocusedChanged()
        {
            //This is used to handle hashtable,if its length is 1.It means number of active applications is only one....
            try
            {
                if (applhash.Count == 1)
                {
                    IDictionaryEnumerator en = applhash.GetEnumerator();
                    applfocusinterval = DateTime.Now.Subtract(applfocustime);

                    foreach (var item in watchFiels)
                    {
                        string appWatchItem = item.appTitle + "$$$!!!" + item.processName;
                        if (appWatchItem.Equals(appltitle + "$$$!!!" + appName))
                        {
                            item.seconds = applfocusinterval.TotalSeconds;
                        }
                    }
                    while (en.MoveNext())
                    {
                        if (en.Key.ToString().Equals(appltitle + "$$$!!!" + appName))
                        {
                            applhash.Remove(appltitle + "$$$!!!" + appName);
                            applhash.Add(appltitle + "$$$!!!" + appName, applfocusinterval.TotalSeconds);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void BindGrid()
        {
            //This is used to bind grid with update contents of xml file....
            SaveandShowDetails();
            DataSet ds = new DataSet();
            //ds.ReadXml(@"c:\appldetails.xml");
            ds.ReadXml(appFileName);
            dataGrid1.DataSource = ds;
        }
        private void SaveandShowDetails()
        {
            //This is used to save contents of hashtable in a xml file....
            try
            {
                TestFocusedChanged();
                //System.IO.StreamWriter writer = new System.IO.StreamWriter(@"c:\appldetails.xml",false); -- commented by Deepak Bhor
                System.IO.StreamWriter writer = new System.IO.StreamWriter(appFileName, false);
                IDictionaryEnumerator en = applhash.GetEnumerator();
                writer.Write("<?xml version=\"1.0\"?>");
                writer.WriteLine("");
                writer.Write("<?xml-stylesheet type=\"text/xsl\" href=\"appl_xsl.xsl\"?>");
                writer.WriteLine("");
                writer.Write("<ApplDetails>");
                foreach (var item in watchFiels)
                {
                    if (!item.seconds.ToString().Trim().StartsWith("0"))
                    {
                        writer.Write("<Application_Info>");
                        writer.Write("<ProcessName>");
                        string processname = "";
                        processname = "<![CDATA[" + item.processName.ToString().Trim() + "]]>";
                        processname = processname.Replace("\0", "");
                        writer.Write(processname);
                        writer.Write("</ProcessName>");
                        writer.Write("<ApplicationName>");
                        string applname = "";
                        applname = "<![CDATA[" + item.appTitle.ToString().Trim() + "]]>";
                        writer.Write(applname);
                        writer.Write("</ApplicationName>");
                        writer.Write("<TotalSeconds>");
                        if ((item.seconds / 60) < 1)
                        {
                            writer.Write(Convert.ToInt32(item.seconds) + " Seconds");
                        }
                        else
                        {
                            writer.Write(Convert.ToInt32(item.seconds / 60) + " Minutes");
                        }
                        writer.Write("</TotalSeconds>");
                        writer.Write("<currentWindowsIdentity>");
                        writer.Write(item.windowsId.ToString().Trim());
                        writer.Write("</currentWindowsIdentity>");
                        writer.Write("<isUserPartOfAdminGroup>");
                        writer.Write(item.isUserInAdminGroup.ToString().Trim());
                        writer.Write("</isUserPartOfAdminGroup>");
                        writer.Write("<IntegrityLevel>");
                        writer.Write(item.integratedSecurityLevel.ToString().Trim());
                        writer.Write("</IntegrityLevel>");
                        writer.Write("<Recordings>");
                        //writer.Write(item.integratedSecurityLevel.ToString().Trim());
                        writer.Write("</Recordings>");
                        writer.Write("</Application_Info>");
                    }
                }
                writer.Write("</ApplDetails>");
                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        private void notifyIcon1_Click(object sender, System.EventArgs e)
        {
            //This is used to show and hide Form....
            try
            {
                if (form1.Visible == true)
                {
                    form1.Visible = false;
                    notifyIcon1.Text = "Application Watcher is in Invisible Mode";
                }
                else
                {
                    form1.Visible = true;
                    form1.Focus();
                    form1.WindowState = FormWindowState.Normal;
                    notifyIcon1.Text = "Application Watcher is in Visible Mode";
                    BindGrid();
                }
            }
            catch { }
        }
        private void Form1_Load(object sender, System.EventArgs e)
        {
            try
            {

                form1.Visible = false;
                notifyIcon1.Text = "Application Watcher is in Invisible Mode";
                logintime = DateTime.Now;
                form1.Text = "Login Time is at :" + DateTime.Now.ToLongTimeString();

                /*	if(!System.IO.File.Exists(@"c:\appldetails.xml"))
                    {
                        System.IO.File.Create(@"c:\appldetails.xml").Close();
                    } */

                using (SamServer server = new SamServer(null, SamServer.SERVER_ACCESS_MASK.SAM_SERVER_ENUMERATE_DOMAINS | SamServer.SERVER_ACCESS_MASK.SAM_SERVER_LOOKUP_DOMAIN))
                {
                    foreach (string domain in server.EnumerateDomains())
                    {
                        //MessageBox.Show(domain);
                        var sid = server.GetDomainSid(domain);
                        //MessageBox.Show(sid.ToString());
                        var pi = server.GetDomainPasswordInformation(sid);
                        //MessageBox.Show(pi.MaxPasswordAge.ToString());
                        //MessageBox.Show(pi.MinPasswordAge.ToString());
                        //MessageBox.Show(pi.MinPasswordLength.ToString());
                        //MessageBox.Show(pi.PasswordHistoryLength.ToString());
                        //Console.WriteLine(" PasswordProperties: " + pi.PasswordProperties);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void menuItem2_Click(object sender, System.EventArgs e)
        {
            //This is activated on click on Refresh menu item to load grid with present contents of xml file....
            BindGrid();
        }
        private void menuItem3_Click(object sender, System.EventArgs e)
        {
            //This is activated on click on Exit menu item and to show actual and calculated time spent on all applications
            //opened so far and to open IE to show xml contents....
            SaveandShowDetails();
            TimeSpan timeinterval = DateTime.Now.Subtract(logintime);
            System.Diagnostics.EventLog.WriteEntry("Application Watcher Total Time Details", timeinterval.Hours + " Hrs " + timeinterval.Minutes + " Mins", System.Diagnostics.EventLogEntryType.Information);
            MessageBox.Show("Actual Time Spent :" + timeinterval.Hours + " Hrs " + timeinterval.Minutes + " Mins", "Application Watcher Total Time Details");
            //StreamReader freader = new StreamReader(@"c:\appldetails.xml"); -- commented by Deepak
            StreamReader freader = new StreamReader(appFileName);
            XmlTextReader xmlreader = new XmlTextReader(freader);
            string tottime = "";

            while (xmlreader.Read())
            {
                if (xmlreader.NodeType == XmlNodeType.Element && xmlreader.Name == "TotalSeconds")
                {
                    tottime += ";" + xmlreader.ReadInnerXml().ToString();
                }
            }

            string[] tottimes = tottime.Split(';');
            long totsecs = 0;

            foreach (string str in tottimes)
            {
                if (str != string.Empty)
                {
                    if (str.IndexOf("Seconds") != -1)
                    {
                        totsecs += Convert.ToInt64(str.Substring(0, str.Length - 8));
                    }
                    else
                    {
                        totsecs += Convert.ToInt64(str.Substring(0, str.Length - 8)) * 60;
                    }
                }
            }

            MessageBox.Show((totsecs / 60) + " Minutes");
            showdetailsinIE();
        }

        #region Helper Functions for Admin Privileges and Elevation Status

        /// <summary>
        /// The function checks whether the primary access token of the process belongs 
        /// to user account that is a member of the local Administrators group, even if 
        /// it currently is not elevated.
        /// </summary>
        /// <returns>
        /// Returns true if the primary access token of the process belongs to user 
        /// account that is a member of the local Administrators group. Returns false 
        /// if the token does not.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// When any native Windows API call fails, the function throws a Win32Exception 
        /// with the last error code.
        /// </exception>
        internal bool IsUserInAdminGroup()
        {
            bool fInAdminGroup = false;
            SafeTokenHandle hToken = null;
            SafeTokenHandle hTokenToCheck = null;
            IntPtr pElevationType = IntPtr.Zero;
            IntPtr pLinkedToken = IntPtr.Zero;
            int cbSize = 0;

            try
            {
                // Open the access token of the current process for query and duplicate.
                if (!NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    NativeMethods.TOKEN_QUERY | NativeMethods.TOKEN_DUPLICATE, out hToken))
                {
                    throw new Win32Exception();
                }

                // Determine whether system is running Windows Vista or later operating 
                // systems (major version >= 6) because they support linked tokens, but 
                // previous versions (major version < 6) do not.
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    // Running Windows Vista or later (major version >= 6). 
                    // Determine token type: limited, elevated, or default. 

                    // Allocate a buffer for the elevation type information.
                    cbSize = sizeof(TOKEN_ELEVATION_TYPE);
                    pElevationType = Marshal.AllocHGlobal(cbSize);
                    if (pElevationType == IntPtr.Zero)
                    {
                        throw new Win32Exception();
                    }

                    // Retrieve token elevation type information.
                    if (!NativeMethods.GetTokenInformation(hToken,
                        TOKEN_INFORMATION_CLASS.TokenElevationType, pElevationType,
                        cbSize, out cbSize))
                    {
                        throw new Win32Exception();
                    }

                    // Marshal the TOKEN_ELEVATION_TYPE enum from native to .NET.
                    TOKEN_ELEVATION_TYPE elevType = (TOKEN_ELEVATION_TYPE)
                        Marshal.ReadInt32(pElevationType);

                    // If limited, get the linked elevated token for further check.
                    if (elevType == TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
                    {
                        // Allocate a buffer for the linked token.
                        cbSize = IntPtr.Size;
                        pLinkedToken = Marshal.AllocHGlobal(cbSize);
                        if (pLinkedToken == IntPtr.Zero)
                        {
                            throw new Win32Exception();
                        }

                        // Get the linked token.
                        if (!NativeMethods.GetTokenInformation(hToken,
                            TOKEN_INFORMATION_CLASS.TokenLinkedToken, pLinkedToken,
                            cbSize, out cbSize))
                        {
                            throw new Win32Exception();
                        }

                        // Marshal the linked token value from native to .NET.
                        IntPtr hLinkedToken = Marshal.ReadIntPtr(pLinkedToken);
                        hTokenToCheck = new SafeTokenHandle(hLinkedToken);
                    }
                }

                // CheckTokenMembership requires an impersonation token. If we just got 
                // a linked token, it already is an impersonation token.  If we did not 
                // get a linked token, duplicate the original into an impersonation 
                // token for CheckTokenMembership.
                if (hTokenToCheck == null)
                {
                    if (!NativeMethods.DuplicateToken(hToken,
                        SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                        out hTokenToCheck))
                    {
                        throw new Win32Exception();
                    }
                }

                // Check if the token to be checked contains admin SID.
                WindowsIdentity id = new WindowsIdentity(hTokenToCheck.DangerousGetHandle());
                WindowsPrincipal principal = new WindowsPrincipal(id);
                fInAdminGroup = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            finally
            {
                // Centralized cleanup for all allocated resources. 
                if (hToken != null)
                {
                    hToken.Close();
                    hToken = null;
                }
                if (hTokenToCheck != null)
                {
                    hTokenToCheck.Close();
                    hTokenToCheck = null;
                }
                if (pElevationType != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pElevationType);
                    pElevationType = IntPtr.Zero;
                }
                if (pLinkedToken != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pLinkedToken);
                    pLinkedToken = IntPtr.Zero;
                }
            }
            return fInAdminGroup;
        }

        /// <summary>
        /// The function checks whether the current process is run as administrator.
        /// In other words, it dictates whether the primary access token of the 
        /// process belongs to user account that is a member of the local 
        /// Administrators group and it is elevated.
        /// </summary>
        /// <returns>
        /// Returns true if the primary access token of the process belongs to user 
        /// account that is a member of the local Administrators group and it is 
        /// elevated. Returns false if the token does not.
        /// </returns>
        internal bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        /// <summary>
        /// The function gets the integrity level of the current process. Integrity 
        /// level is only available on Windows Vista and newer operating systems, thus 
        /// GetProcessIntegrityLevel throws a C++ exception if it is called on systems 
        /// prior to Windows Vista.
        /// </summary>
        /// <returns>
        /// Returns the integrity level of the current process. It is usually one of 
        /// these values:
        /// 
        ///    SECURITY_MANDATORY_UNTRUSTED_RID - means untrusted level. It is used 
        ///    by processes started by the Anonymous group. Blocks most write access.
        ///    (SID: S-1-16-0x0)
        ///    
        ///    SECURITY_MANDATORY_LOW_RID - means low integrity level. It is used by
        ///    Protected Mode Internet Explorer. Blocks write acess to most objects 
        ///    (such as files and registry keys) on the system. (SID: S-1-16-0x1000)
        /// 
        ///    SECURITY_MANDATORY_MEDIUM_RID - means medium integrity level. It is 
        ///    used by normal applications being launched while UAC is enabled. 
        ///    (SID: S-1-16-0x2000)
        ///    
        ///    SECURITY_MANDATORY_HIGH_RID - means high integrity level. It is used 
        ///    by administrative applications launched through elevation when UAC is 
        ///    enabled, or normal applications if UAC is disabled and the user is an 
        ///    administrator. (SID: S-1-16-0x3000)
        ///    
        ///    SECURITY_MANDATORY_SYSTEM_RID - means system integrity level. It is 
        ///    used by services and other system-level applications (such as Wininit, 
        ///    Winlogon, Smss, etc.)  (SID: S-1-16-0x4000)
        /// 
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// When any native Windows API call fails, the function throws a Win32Exception 
        /// with the last error code.
        /// </exception>
        public int GetProcessIntegrityLevel()
        {
            int IL = -1;
            CSUACSelfElevation.SafeTokenHandle hToken = null;
            int cbTokenIL = 0;
            IntPtr pTokenIL = IntPtr.Zero;

            try
            {
                // Open the access token of the current process with TOKEN_QUERY.
                if (!NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    NativeMethods.TOKEN_QUERY, out hToken))
                {
                    throw new Win32Exception();
                }

                // Then we must query the size of the integrity level information 
                // associated with the token. Note that we expect GetTokenInformation 
                // to return false with the ERROR_INSUFFICIENT_BUFFER error code 
                // because we've given it a null buffer. On exit cbTokenIL will tell 
                // the size of the group information.

                if (!NativeMethods.GetTokenInformation(hToken,TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, IntPtr.Zero, 0, out cbTokenIL))
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != CSUACSelfElevation.NativeMethods.ERROR_INSUFFICIENT_BUFFER)
                    {
                        // When the process is run on operating systems prior to 
                        // Windows Vista, GetTokenInformation returns false with the 
                        // ERROR_INVALID_PARAMETER error code because 
                        // TokenIntegrityLevel is not supported on those OS's.
                        throw new Win32Exception(error);
                    }
                }

                // Now we allocate a buffer for the integrity level information.
                pTokenIL = Marshal.AllocHGlobal(cbTokenIL);
                if (pTokenIL == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                // Now we ask for the integrity level information again. This may fail 
                // if an administrator has added this account to an additional group 
                // between our first call to GetTokenInformation and this one.
                if (!NativeMethods.GetTokenInformation(hToken,
                    TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, pTokenIL, cbTokenIL,
                    out cbTokenIL))
                {
                    throw new Win32Exception();
                }

                // Marshal the TOKEN_MANDATORY_LABEL struct from native to .NET object.
                TOKEN_MANDATORY_LABEL tokenIL = (TOKEN_MANDATORY_LABEL)
                    Marshal.PtrToStructure(pTokenIL, typeof(TOKEN_MANDATORY_LABEL));

                // Integrity Level SIDs are in the form of S-1-16-0xXXXX. (e.g. 
                // S-1-16-0x1000 stands for low integrity level SID). There is one 
                // and only one subauthority.
                IntPtr pIL = NativeMethods.GetSidSubAuthority(tokenIL.Label.Sid, 0);
                IL = Marshal.ReadInt32(pIL);
            }
            finally
            {
                // Centralized cleanup for all allocated resources. 
                if (hToken != null)
                {
                    hToken.Close();
                    hToken = null;
                }
                if (pTokenIL != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pTokenIL);
                    pTokenIL = IntPtr.Zero;
                    cbTokenIL = 0;
                }
            }
            return IL;
        }

        /// <summary>
        /// The function gets the elevation information of the current process. It 
        /// dictates whether the process is elevated or not. Token elevation is only 
        /// available on Windows Vista and newer operating systems, thus 
        /// IsProcessElevated throws a C++ exception if it is called on systems prior 
        /// to Windows Vista. It is not appropriate to use this function to determine 
        /// whether a process is run as administartor.
        /// </summary>
        /// <returns>
        /// Returns true if the process is elevated. Returns false if it is not.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// When any native Windows API call fails, the function throws a Win32Exception 
        /// with the last error code.
        /// </exception>
        /// <remarks>
        /// TOKEN_INFORMATION_CLASS provides TokenElevationType to check the elevation 
        /// type (TokenElevationTypeDefault / TokenElevationTypeLimited / 
        /// TokenElevationTypeFull) of the process. It is different from TokenElevation 
        /// in that, when UAC is turned off, elevation type always returns 
        /// TokenElevationTypeDefault even though the process is elevated (Integrity 
        /// Level == High). In other words, it is not safe to say if the process is 
        /// elevated based on elevation type. Instead, we should use TokenElevation. 
        /// </remarks>
        internal bool IsProcessElevated()
        {
            bool fIsElevated = false;
            SafeTokenHandle hToken = null;
            int cbTokenElevation = 0;
            IntPtr pTokenElevation = IntPtr.Zero;

            try
            {
                // Open the access token of the current process with TOKEN_QUERY.
                if (!NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    NativeMethods.TOKEN_QUERY, out hToken))
                {
                    throw new Win32Exception();
                }

                // Allocate a buffer for the elevation information.
                cbTokenElevation = Marshal.SizeOf(typeof(TOKEN_ELEVATION));
                pTokenElevation = Marshal.AllocHGlobal(cbTokenElevation);
                if (pTokenElevation == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                // Retrieve token elevation information.
                if (!NativeMethods.GetTokenInformation(hToken,
                    TOKEN_INFORMATION_CLASS.TokenElevation, pTokenElevation,
                    cbTokenElevation, out cbTokenElevation))
                {
                    // When the process is run on operating systems prior to Windows 
                    // Vista, GetTokenInformation returns false with the error code 
                    // ERROR_INVALID_PARAMETER because TokenElevation is not supported 
                    // on those operating systems.
                    throw new Win32Exception();
                }

                // Marshal the TOKEN_ELEVATION struct from native to .NET object.
                TOKEN_ELEVATION elevation = (TOKEN_ELEVATION)Marshal.PtrToStructure(
                    pTokenElevation, typeof(TOKEN_ELEVATION));

                // TOKEN_ELEVATION.TokenIsElevated is a non-zero value if the token 
                // has elevated privileges; otherwise, a zero value.
                fIsElevated = (elevation.TokenIsElevated != 0);
            }
            finally
            {
                // Centralized cleanup for all allocated resources. 
                if (hToken != null)
                {
                    hToken.Close();
                    hToken = null;
                }
                if (pTokenElevation != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pTokenElevation);
                    pTokenElevation = IntPtr.Zero;
                    cbTokenElevation = 0;
                }
            }
            return fIsElevated;
        }
        #endregion
    }

    public class GenericWatcherList
    {
        public string appTitle { get; set; }
        public double seconds { get; set; }
        public string processName { get; set; }
        public string windowsId { get; set; }
        public string processRights { get; set; }
        public string isUserAdmin { get; set; }
        public string isUserInAdminGroup { get; set; }
        public string isProcessElevated { get; set; }
        public string integratedSecurityLevel { get; set; }
    }

    [ComImport, Guid("80AD2243-BBC8-442B-AA55-14633E6FE17B")]
    public class windowsSession
    {

    }

    [ComImport, Guid("d08018bd-6958-4a2e-95ea-fec13211da0f")]
    public class windowsService
    {
        
    }

    public sealed class SamServer : IDisposable
    {
        private IntPtr _handle;

        public SamServer(string name, SERVER_ACCESS_MASK access)
        {
            Name = name;
            Check(SamConnect(new UNICODE_STRING(name), out _handle, access, IntPtr.Zero));
        }

        public string Name { get; private set; }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                SamCloseHandle(_handle);
                _handle = IntPtr.Zero;
            }
        }

        public void SetDomainPasswordInformation(SecurityIdentifier domainSid, DOMAIN_PASSWORD_INFORMATION passwordInformation)
        {
            if (domainSid == null)
                throw new ArgumentNullException("domainSid");

            byte[] sid = new byte[domainSid.BinaryLength];
            domainSid.GetBinaryForm(sid, 0);

            IntPtr domain;
            Check(SamOpenDomain(_handle, DOMAIN_ACCESS_MASK.DOMAIN_WRITE_PASSWORD_PARAMS, sid, out domain));
            IntPtr info = Marshal.AllocHGlobal(Marshal.SizeOf(passwordInformation));
            Marshal.StructureToPtr(passwordInformation, info, false);
            try
            {
                Check(SamSetInformationDomain(domain, DOMAIN_INFORMATION_CLASS.DomainPasswordInformation, info));
            }
            finally
            {
                Marshal.FreeHGlobal(info);
                SamCloseHandle(domain);
            }
        }

        public DOMAIN_PASSWORD_INFORMATION GetDomainPasswordInformation(SecurityIdentifier domainSid)
        {
            if (domainSid == null)
                throw new ArgumentNullException("domainSid");

            byte[] sid = new byte[domainSid.BinaryLength];
            domainSid.GetBinaryForm(sid, 0);

            IntPtr domain;
            Check(SamOpenDomain(_handle, DOMAIN_ACCESS_MASK.DOMAIN_READ_PASSWORD_PARAMETERS, sid, out domain));
            IntPtr info = IntPtr.Zero;
            try
            {
                Check(SamQueryInformationDomain(domain, DOMAIN_INFORMATION_CLASS.DomainPasswordInformation, out info));
                return (DOMAIN_PASSWORD_INFORMATION)Marshal.PtrToStructure(info, typeof(DOMAIN_PASSWORD_INFORMATION));
            }
            finally
            {
                SamFreeMemory(info);
                SamCloseHandle(domain);
            }
        }

        public SecurityIdentifier GetDomainSid(string domain)
        {
            if (domain == null)
                throw new ArgumentNullException("domain");

            IntPtr sid;
            Check(SamLookupDomainInSamServer(_handle, new UNICODE_STRING(domain), out sid));
            return new SecurityIdentifier(sid);
        }

        public IEnumerable<string> EnumerateDomains()
        {
            int cookie = 0;
            while (true)
            {
                IntPtr info;
                int count;
                var status = SamEnumerateDomainsInSamServer(_handle, ref cookie, out info, 1, out count);
                if (status != NTSTATUS.STATUS_SUCCESS && status != NTSTATUS.STATUS_MORE_ENTRIES)
                    Check(status);

                if (count == 0)
                    break;

                UNICODE_STRING us = (UNICODE_STRING)Marshal.PtrToStructure(info + 8, typeof(UNICODE_STRING));
                SamFreeMemory(info);
                yield return us.ToString();
            }
        }

        private enum DOMAIN_INFORMATION_CLASS
        {
            DomainPasswordInformation = 1,
        }

        [Flags]
        public enum PASSWORD_PROPERTIES
        {
            DOMAIN_PASSWORD_COMPLEX = 0x00000001,
            DOMAIN_PASSWORD_NO_ANON_CHANGE = 0x00000002,
            DOMAIN_PASSWORD_NO_CLEAR_CHANGE = 0x00000004,
            DOMAIN_LOCKOUT_ADMINS = 0x00000008,
            DOMAIN_PASSWORD_STORE_CLEARTEXT = 0x00000010,
            DOMAIN_REFUSE_PASSWORD_CHANGE = 0x00000020,
        }

        [Flags]
        private enum DOMAIN_ACCESS_MASK
        {
            DOMAIN_READ_PASSWORD_PARAMETERS = 0x00000001,
            DOMAIN_WRITE_PASSWORD_PARAMS = 0x00000002,
            DOMAIN_READ_OTHER_PARAMETERS = 0x00000004,
            DOMAIN_WRITE_OTHER_PARAMETERS = 0x00000008,
            DOMAIN_CREATE_USER = 0x00000010,
            DOMAIN_CREATE_GROUP = 0x00000020,
            DOMAIN_CREATE_ALIAS = 0x00000040,
            DOMAIN_GET_ALIAS_MEMBERSHIP = 0x00000080,
            DOMAIN_LIST_ACCOUNTS = 0x00000100,
            DOMAIN_LOOKUP = 0x00000200,
            DOMAIN_ADMINISTER_SERVER = 0x00000400,
            DOMAIN_ALL_ACCESS = 0x000F07FF,
            DOMAIN_READ = 0x00020084,
            DOMAIN_WRITE = 0x0002047A,
            DOMAIN_EXECUTE = 0x00020301
        }

        [Flags]
        public enum SERVER_ACCESS_MASK
        {
            SAM_SERVER_CONNECT = 0x00000001,
            SAM_SERVER_SHUTDOWN = 0x00000002,
            SAM_SERVER_INITIALIZE = 0x00000004,
            SAM_SERVER_CREATE_DOMAIN = 0x00000008,
            SAM_SERVER_ENUMERATE_DOMAINS = 0x00000010,
            SAM_SERVER_LOOKUP_DOMAIN = 0x00000020,
            SAM_SERVER_ALL_ACCESS = 0x000F003F,
            SAM_SERVER_READ = 0x00020010,
            SAM_SERVER_WRITE = 0x0002000E,
            SAM_SERVER_EXECUTE = 0x00020021
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DOMAIN_PASSWORD_INFORMATION
        {
            public short MinPasswordLength;
            public short PasswordHistoryLength;
            public PASSWORD_PROPERTIES PasswordProperties;
            private long _maxPasswordAge;
            private long _minPasswordAge;

            public TimeSpan MaxPasswordAge
            {
                get
                {
                    return -new TimeSpan(_maxPasswordAge);
                }
                set
                {
                    _maxPasswordAge = value.Ticks;
                }
            }

            public TimeSpan MinPasswordAge
            {
                get
                {
                    return -new TimeSpan(_minPasswordAge);
                }
                set
                {
                    _minPasswordAge = value.Ticks;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct UNICODE_STRING : IDisposable
        {
            public ushort Length;
            public ushort MaximumLength;
            private IntPtr Buffer;

            public UNICODE_STRING(string s)
                : this()
            {
                if (s != null)
                {
                    Length = (ushort)(s.Length * 2);
                    MaximumLength = (ushort)(Length + 2);
                    Buffer = Marshal.StringToHGlobalUni(s);
                }
            }

            public void Dispose()
            {
                if (Buffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(Buffer);
                    Buffer = IntPtr.Zero;
                }
            }

            public override string ToString()
            {
                return Buffer != IntPtr.Zero ? Marshal.PtrToStringUni(Buffer) : null;
            }
        }

        private static void Check(NTSTATUS err)
        {
            if (err == NTSTATUS.STATUS_SUCCESS)
                return;

            throw new Win32Exception("Error " + err + " (0x" + ((int)err).ToString("X8") + ")");
        }

        private enum NTSTATUS
        {
            STATUS_SUCCESS = 0x0,
            STATUS_MORE_ENTRIES = 0x105,
            STATUS_INVALID_HANDLE = unchecked((int)0xC0000008),
            STATUS_INVALID_PARAMETER = unchecked((int)0xC000000D),
            STATUS_ACCESS_DENIED = unchecked((int)0xC0000022),
            STATUS_OBJECT_TYPE_MISMATCH = unchecked((int)0xC0000024),
            STATUS_NO_SUCH_DOMAIN = unchecked((int)0xC00000DF),
        }

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamConnect(UNICODE_STRING ServerName, out IntPtr ServerHandle, SERVER_ACCESS_MASK DesiredAccess, IntPtr ObjectAttributes);

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamCloseHandle(IntPtr ServerHandle);

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamFreeMemory(IntPtr Handle);

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamOpenDomain(IntPtr ServerHandle, DOMAIN_ACCESS_MASK DesiredAccess, byte[] DomainId, out IntPtr DomainHandle);

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamLookupDomainInSamServer(IntPtr ServerHandle, UNICODE_STRING name, out IntPtr DomainId);

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamQueryInformationDomain(IntPtr DomainHandle, DOMAIN_INFORMATION_CLASS DomainInformationClass, out IntPtr Buffer);

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamSetInformationDomain(IntPtr DomainHandle, DOMAIN_INFORMATION_CLASS DomainInformationClass, IntPtr Buffer);

        [DllImport("samlib.dll", CharSet = CharSet.Unicode)]
        private static extern NTSTATUS SamEnumerateDomainsInSamServer(IntPtr ServerHandle, ref int EnumerationContext, out IntPtr EnumerationBuffer, int PreferedMaximumLength, out int CountReturned);
    }

}
