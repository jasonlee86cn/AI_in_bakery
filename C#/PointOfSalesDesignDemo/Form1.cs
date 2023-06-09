using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


using AForge.Video;
using AForge.Video.DirectShow;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Management;
using System.Net;

using System.Web;
using Json.Net;
using System.Security.Principal;
using Google.Protobuf.Collections;
using System.Drawing.Printing;

using MsgBox;
using pppp;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Security.Cryptography.X509Certificates;

namespace PointOfSalesDesignDemo
{

    public partial class Form1 : Form
    {
        public int islemdurumu = 0; //CAMERA STATUS
        FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        VideoCaptureDevice videoSource = null;
        public static int durdur = 0;
        public static int gondermesayisi = 0;
        public int kamerabaslat = 0;
        public int selected = 0;

        



        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();

            B_pay.Enabled = false;

            try
            {

                var System_HDID = GetHardSerial();
                richTextBox1.Text = richTextBox1.Text + "Asset serial ID: " + System_HDID + "\r\n";
                register_asset_id.Text = System_HDID;



                string checking_url = @"http://register.p20220916.lipanlong.com/register.php?s=register&asset_id=" + System_HDID;
                var checking_result = Get(checking_url);


                j1 checking_result_2 = JsonConvert.DeserializeObject<j1>(checking_result);



                if (checking_result_2.status == "ERR")
                {
                    richTextBox1.Text = richTextBox1.Text + checking_result_2.error_reason;
                    imgVideo.ImageLocation = checking_result_2.qr_code;
                    B_ai_scan.Enabled = false;
                    B_ai_scan_2.Enabled = false;

                }
                else if (checking_result_2.status == "OK")
                {
                    L_info.Text = checking_result_2.register_asset_merchant_name;
                    register_asset_merchant_code.Text = checking_result_2.register_asset_merchant_code;
                    register_asset_merchant_name.Text = checking_result_2.register_asset_merchant_name;
                    register_asset_merchant_matching_url.Text = checking_result_2.register_asset_merchant_matching_url;
                    register_asset_merchant_ai_url.Text = checking_result_2.register_asset_merchant_ai_url;
                }




                //Enumerate all video input devices
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (videoDevices.Count ==0)
                {
                    richTextBox1.Text = richTextBox1.Text + "No local capture devices.\n";
                    string message = "No local capture devices found.";
                    string caption = "No capture devices";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    // Displays the MessageBox.
                    result = MessageBox.Show(message, caption, buttons);
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        // Closes the parent form.
                        //this.Close();
                        System.Windows.Forms.Application.Exit();
                    }

                }
                else {
                    foreach (FilterInfo device in videoDevices)
                    {
                        int i2 = 1;
                        comboBox1.Items.Add(device.Name);
                        //label8.Text = ("camera" + i + "initialization completed..." + "\n");
                        richTextBox1.Text = richTextBox1.Text + ("Camera " + i2 + " [" + device.Name + "] " + " initialization completed..." + "\n");
                        i2++;
                    }
                    comboBox1.SelectedIndex = 0;

                    //1

                    videoSource = new VideoCaptureDevice(videoDevices[selected].MonikerString);
                    //
                    int i;
                    if (videoSource.VideoCapabilities.Length > 0)
                    {
                        for (i = 0; i < videoSource.VideoCapabilities.Length; i++)
                        {
                            if (videoSource.VideoCapabilities[i].FrameSize.Width == 640)
                                break;
                        }
                        videoSource.VideoResolution = videoSource.VideoCapabilities[i];
                    }
                    //


                    videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                    videoSource.Start(); kamerabaslat = 1; //CAMERA STARTRED

                    //1

                }


                richTextBox1.Text = richTextBox1.Text + "Number of cams" + videoDevices.Count+"\n";
                //comboBox1.SelectedIndex = 0;

                string screenWidth = Screen.PrimaryScreen.Bounds.Width.ToString();
                string screenHeight = Screen.PrimaryScreen.Bounds.Height.ToString();
                richTextBox1.Text = richTextBox1.Text + screenWidth + "*" + screenHeight;
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;

                //
                B_ai_scan.Visible = false;
                B_staff.Visible = false;
                print_b_test.Visible = false;
                register_asset_id.Visible = false;
                register_asset_merchant_ai_url.Visible = false;
                register_asset_merchant_code.Visible = false;
                register_asset_merchant_name.Visible = false;
                register_asset_merchant_matching_url.Visible = false;
                order_id.Visible = false;
                print_tag.Visible = false;
                paid_tag.Visible = false;
                webBrowser1.Visible = false;

                START.Visible = false;
                CAPTURE.Visible = false;
                RESET.Visible = false;
                PAUSE.Visible = false;
                SAVE.Visible = false;

                //

            }
            catch (ApplicationException)
            {
                //this.label8.Text = "No local capture devices";
                richTextBox1.Text = richTextBox1.Text + "No local capture devices";
                videoDevices = null;
            }
        }


        private void guna2Button13_Click(object sender, EventArgs e)
        {
            B_ai_scan.Enabled = false;
            //System.Windows.Forms.Application.Exit();
            var psi = new ProcessStartInfo();
            psi.FileName = @"python.exe";
            psi.WorkingDirectory = @"C:\Python\yolov5";
            var script = @"ai_v1.py";
            //var endfix = @" data/temp/temp.jpg";
            var endfix = @"";
            psi.Arguments = $"\"{script}\" \"{endfix}\"";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            var errors = "";
            var results = "";
            using (Process process = Process.Start(psi)) {
                results = process.StandardOutput.ReadToEnd();
                errors = process.StandardError.ReadToEnd();
                

            }

            
            List<od_data> od_data = JsonConvert.DeserializeObject<List<od_data>>(results);

            if (od_data != null && od_data.Count > 0)
            {
                int c1 = 1;
                foreach (var t2 in od_data)
                {
                    string checking_url = @"http://www.google.com/?" + t2.name;
                    var checking_result = Get(checking_url);

                    richTextBox1.Text = richTextBox1.Text + "\r\n" + "Itme detected>"+c1+">" + t2.name;
                    c1++;
                }
            }
            else {
                richTextBox1.Text = richTextBox1.Text + "\r\n" + "No itmes detected.";
            }



            //richTextBox1.Text = richTextBox1.Text + results;
            //richTextBox1.Text = richTextBox1.Text + errors;
            B_ai_scan.Enabled = true;
        }

        private void guna2Button14_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();

        }

        private void panelItems_Paint(object sender, PaintEventArgs e)
        {

        }


        public class od_data
        {
            /*
    "xmin":779.8990478516,
    "ymin":408.0350646973,
    "xmax":1200.0,
    "ymax":819.892578125,
    "confidence":0.89830935,
    "class":55,
    "name":"cake"
             */
            public float xmin { get; set; }
            public float ymin { get; set; }
            public float xmax { get; set; }
            public float ymax { get; set; }
            public float confidence { get; set; }
            public int class2 { get; set; }
            public string name { get; set; }
        }






        public class HardDrive
        {
            public string Model { get; set; }
            public string InterfaceType { get; set; }
            public string Caption { get; set; }
            public string SerialNo { get; set; }
        }

        public string GetHardSerial()
        {
            ManagementObjectSearcher Finder = new ManagementObjectSearcher("Select * from Win32_OperatingSystem");
            string Name = "";
            string SerialNumber = "";
            foreach (ManagementObject OS in Finder.Get()) Name = OS["Name"].ToString();

            //Name = "Microsoft Windows XP Professional|C:\WINDOWS|\Device\Harddisk0\Partition1"

            int ind = Name.IndexOf("Harddisk") + 8;
            int HardIndex = Convert.ToInt16(Name.Substring(ind, 1));
            Finder = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive WHERE Index=" + HardIndex);
            foreach (ManagementObject HardDisks in Finder.Get())
                foreach (ManagementObject HardDisk in HardDisks.GetRelated("Win32_PhysicalMedia"))
                    SerialNumber = HardDisk["SerialNumber"].ToString();
            // TextBox1.Text = Convert.ToString(Name);
            // TextBox2.Text = Convert.ToString(SerialNumber);
            return SerialNumber;

        }

        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public string Post(string uri, string data, string contentType, string method = "POST")
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ContentLength = dataBytes.Length;
            request.ContentType = contentType;
            request.Method = method;

            using (Stream requestBody = request.GetRequestStream())
            {
                requestBody.Write(dataBytes, 0, dataBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }


        public async Task<string> PostAsync(string uri, string data, string contentType, string method = "POST")
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ContentLength = dataBytes.Length;
            request.ContentType = contentType;
            request.Method = method;

            using (Stream requestBody = request.GetRequestStream())
            {
                await requestBody.WriteAsync(dataBytes, 0, dataBytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }


        public class j1
        {
            public string status { get; set; }
            public string info { get; set; }
            public string error_reason { get; set; }
            public string qr_code { get; set; }
            public string register_asset_merchant_time_updated { get; set; }
            public string data { get; set; }
            public string register_id { get; set; }
            public string register_asset_id { get; set; }
            public string register_asset_merchant_code { get; set; }
            public string register_asset_merchant_name { get; set; }
            public string register_asset_merchant_matching_url { get; set; }
            public string register_asset_merchant_ai_url { get; set; }
            public string register_asset_merchant_status { get; set; }
            public string register_asset_merchant_timezone { get; set; }
            public string register_asset_merchant_time_added { get; set; }
            public string product_barcode { get; set; }
            public string product_name { get; set; }
            public float product_price { get; set; }
            public string product_image_id { get; set; }
            public string order_id { get; set; }

        }

       

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }



        private void guna2Button3_Click(object sender, EventArgs e)
        {
            //Set buttons language Czech/English/German/Slovakian/Spanish (default English)
            InputBox.SetLanguage(InputBox.Language.English);
            //Save the DialogResult as res
            DialogResult res = InputBox.ShowDialog("Please sacan / input staff password. ", "Staff password",   //Text message (mandatory), Title (optional)
                InputBox.Icon.Question,                                                                         //Set icon type Error/Exclamation/Question/Warning (default info)
                InputBox.Buttons.OkCancel,                                                                      //Set buttons set OK/OKcancel/YesNo/YesNoCancel (default ok)
                InputBox.Type.TextBox,                                                                         //Set type ComboBox/TextBox/Nothing (default nothing)
                null,                                                        //Set string field as ComboBox items (default null)
                true,                                                                                           //Set visible in taskbar (default false)
                new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Bold));                        //Set font (default by system)
            //Check InputBox result
            if (res == System.Windows.Forms.DialogResult.OK || res == System.Windows.Forms.DialogResult.Yes)
            {
                if (InputBox.ResultValue=="112233") {
                    //1
                    if (!(videoSource == null))
                        if (videoSource.IsRunning)
                        {
                            videoSource.SignalToStop();
                            videoSource = null;
                        }

                    kamerabaslat = 0;
                    imgVideo.Image = null;

                    //label1.Text = "CAMERA TURN OFF";
                    //1

                    System.Windows.Forms.Application.Exit();
                }
                
            }

            
        }

        private void guna2GradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap img = (Bitmap)eventArgs.Frame.Clone();
            imgVideo.Image = img;
        }

        private void RESET_Click(object sender, EventArgs e)
        {
            if (!(videoSource == null))
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                }

            kamerabaslat = 0;
            imgVideo.Image = null;

            //label1.Text = "CAMERA TURN OFF";
        }

        private void START_Click(object sender, EventArgs e)
        {
            selected = comboBox1.SelectedIndex;

            if (islemdurumu == 0)
            {


                if (kamerabaslat > 0) return;
                try
                {
                    videoSource = new VideoCaptureDevice(videoDevices[selected].MonikerString);
                    //
                    int i;
                    if (videoSource.VideoCapabilities.Length > 0)
                    {
                        for (i = 0; i < videoSource.VideoCapabilities.Length; i++)
                        {
                            if (videoSource.VideoCapabilities[i].FrameSize.Width == 640)
                                break;
                        }
                        videoSource.VideoResolution = videoSource.VideoCapabilities[i];
                    }
                    //

                    
                    videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                    videoSource.Start(); kamerabaslat = 1; //CAMERA STARTRED

                }
                catch
                {
                    MessageBox.Show("RESTART THE PROGRAM");

                    if (!(videoSource == null))
                        if (videoSource.IsRunning)
                        {
                            videoSource.SignalToStop();
                            videoSource = null;
                        }
                }//catch
            }
        }

        private void CAPTURE_Click(object sender, EventArgs e)
        {
            if (imgVideo != null) { imgCapture.Image = imgVideo.Image; }
            imgCapture.Image.Save(@"C:\Python\yolov5\data\temp\temp.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

            /*
            imgVideo.Location = new System.Drawing.Point(19, 83);
            imgCapture.Location = new System.Drawing.Point(19, 83);


            imgVideo.Size = new Size(10, 10);
            imgCapture.Size = new Size(1280, 778);
            */
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {

        }

        private void imgVideo_Click(object sender, EventArgs e)
        {

        }


        

        private void B_ai_scan_2_Click(object sender, EventArgs e)
        {

            

            Guid guid = Guid.NewGuid();
            string temp = guid.ToString();
            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            temp = Timestamp.ToString();
            temp = "temp";
            //1
            if (imgVideo != null) { imgCapture.Image = imgVideo.Image; }

            File.WriteAllText(@"C:\Python\yolov5\data\temp\" + temp + ".jpg", "");
            imgCapture.Image.Save(@"C:\Python\yolov5\data\temp\"+ temp + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);


            //imgVideo.Location = new System.Drawing.Point(19, 83);
            //imgCapture.Location = new System.Drawing.Point(19, 83);


            //imgVideo.Size = new Size(10, 10);
            //imgCapture.Size = new Size(1280, 778);

            //imgCapture.Load = "C:\\Python\\yolov5\\data\\output\\save\\temp.jpg";
            //imgCapture.Image = Image.FromFile(@"C:\Python\yolov5\data\output\save\temp.jpg");
            //1


            imgCapture.Image = null;
            listView1.Clear();
            listView1.Columns.Add("#", 35);
            listView1.Columns.Add("Barcode / Item", 200);
            listView1.Columns.Add("$", 55);
            listView1.View = View.Details;
            //listView1.Items.Clear();

            string checking_url_cart_del = register_asset_merchant_matching_url.Text + "?register_asset_id=" + register_asset_id.Text + "&merchant_code=" + register_asset_merchant_code.Text + "&s=del&guid="+ Guid.NewGuid();
            var checking_url_cart_del_result = Get(checking_url_cart_del);


            B_ai_scan_2.Text = "Just a moment please ...";
            B_ai_scan_2.Enabled = false;
            
            //System.Windows.Forms.Application.Exit();
            var psi = new ProcessStartInfo();
            psi.FileName = @"python.exe";
            psi.WorkingDirectory = @"C:\Python\yolov5";
            var script = @"ai_v1.py";
            //var endfix = @" data/temp/temp.jpg";
            var endfix = @"";
            psi.Arguments = $"\"{script}\" \"{endfix}\"";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            var errors = "";
            var results = "";
            using (Process process = Process.Start(psi))
            {
                results = process.StandardOutput.ReadToEnd();
                errors = process.StandardError.ReadToEnd();


            }
            richTextBox1.Text = richTextBox1.Text + results;

            results.Replace(@"Error: data/output\save\temp.jpg - The process cannot access the file because it is being used by another process.", "");

            imgCapture.Image = Image.FromFile(@"C:\Python\yolov5\data\output\save\"+ temp + ".jpg");
            if (results.ToString().Length < 5) {

            }
            else
            {
                List<od_data> od_data = JsonConvert.DeserializeObject<List<od_data>>(results);
                if (od_data != null && od_data.Count > 0)
                {

                    int c1 = 1;
                    int count_1 = 1;
                    float total_price = 0;
                    foreach (var t2 in od_data)
                    {
                        //string checking_url = @"http://www.google.com/?" + t2.name;
                        string checking_url = register_asset_merchant_matching_url.Text + "?register_asset_id=" + register_asset_id.Text + "&merchant_code=" + register_asset_merchant_code.Text + "&keywords=" + t2.name;
                        //richTextBox1.Text = richTextBox1.Text + checking_url;
                        var checking_result = Get(checking_url);
                        j1 checking_result_2 = JsonConvert.DeserializeObject<j1>(checking_result);


                        var opts = new Dictionary<string, string>();
                        if (checking_result_2.status == "OK")
                        {
                            //richTextBox1.Text = richTextBox1.Text + checking_result_2.product_barcode;

                            string checking_url_cart = register_asset_merchant_matching_url.Text + "?register_asset_id=" + register_asset_id.Text + "&merchant_code=" + register_asset_merchant_code.Text + "&s=add&shopping_cart_product_barcode=" + checking_result_2.product_barcode + "&guid=" + Guid.NewGuid();
                            var checking_url_cart_result = Get(checking_url_cart);
                            richTextBox1.Text = richTextBox1.Text + checking_url_cart;
                            j1 checking_url_cart_result_2 = JsonConvert.DeserializeObject<j1>(checking_url_cart_result);

                            if (checking_url_cart_result_2.status == "OK")
                            {
                                richTextBox1.Text = richTextBox1.Text + checking_result_2.product_barcode + " Succ";
                                //string[] row = { count_1.ToString(), checking_result_2.product_barcode , "" };
                                string[] row = { count_1.ToString(), checking_result_2.product_name, checking_result_2.product_price.ToString("#0.00") };
                                var listViewItem = new ListViewItem(row);
                                listView1.Items.Add(listViewItem);
                                /*
                                string[] row2 = { "",  checking_result_2.product_name, checking_result_2.product_price.ToString("#0.00") };
                                var listViewItem2 = new ListViewItem(row2);
                                listView1.Items.Add(listViewItem2);
                                */

                                count_1++;
                                total_price = total_price + checking_result_2.product_price;

                            }
                            else
                            {
                                richTextBox1.Text = richTextBox1.Text + checking_result_2.product_barcode + " Err";
                            }



                            B_pay.Enabled = true;







                            //string url = "http://matching.p20220916.lipanlong.com/test/2.php.htm";
                            //webBrowser1.Navigate(new Uri(url));
                            /*
                            opts.Add("product_barcode", "Un");
                            opts.Add("product_name", "Deux");
                            opts.Add("product_price", "Trois");
                            opts.Add("product_image_id", "Trois");

                            //var list = opts.Select(p => new Dictionary<string, string>() { { p.Key, p.Value } });

                            foreach (var list2 in opts)
                            {

                                richTextBox1.Text = richTextBox1.Text + list2.Value;



                            }
                            */
                        }
                        else
                        {
                            string message = "Item <" + t2.name + "> cannot be matched in system, please re-scan or contact staff. ";
                            string caption = "One or more time(s) cannot be matched";
                            MessageBoxButtons buttons = MessageBoxButtons.OK;
                            DialogResult result;

                            // Displays the MessageBox.
                            result = MessageBox.Show(message, caption, buttons);
                            if (result == System.Windows.Forms.DialogResult.OK)
                            {
                                // Closes the parent form.
                                //this.Close();
                            }
                            break;
                        }





                        richTextBox1.Text = richTextBox1.Text + "\r\n" + "Itme detected>" + c1 + ">" + t2.name;
                        c1++;
                    }
                    //string[] row2 = { "", (count_1-1).ToString() + "Items, Total Amount $", total_price.ToString("#0.00") };
                    //var listViewItem2 = new ListViewItem(row2);
                    //listView1.Items.Add(listViewItem2);
                    display_amount_total.Text = total_price.ToString("#0.00");
                }
                else
                {
                    richTextBox1.Text = richTextBox1.Text + "\r\n" + "No itmes detected.";
                }
            }
            
            





            //richTextBox1.Text = richTextBox1.Text + results;
            //richTextBox1.Text = richTextBox1.Text + errors;
            B_ai_scan_2.Enabled = true;
            B_ai_scan_2.Text = "AI Scan";
        }


        



        private void register_asset_id_Click(object sender, EventArgs e)
        {

        }

        private void print_b_test_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string tou = "Testing Merchant Name";
            string address = "88888 Queen Street, Auckland Central, Auckland";
            string saleID = "2022110834234234";
            string item = "Item(s)";
            decimal price = 25.00M;
            int count = 5;
            decimal total = 0.00M;
            decimal fukuan = 500.00M;
            sb.Append("            " + tou + "     \r\n");
            sb.Append("-----------------------------------------------------------------\r\n");
            sb.Append("Date:" + DateTime.Now.ToShortDateString() + "  " + "单号:" + saleID + "\r\n");
            sb.Append("-----------------------------------------------------------------\r\n");
            sb.Append("项目" + "\t\t" + "数量" + "\t" + "单价" + "\t" + "小计" + "\r\n");
            for (int i = 0; i < count; i++)
            {
                decimal xiaoji = (i + 1) * price;
                sb.Append(item + (i + 1) + "\t\t" + (i + 1) + "\t" + price + "\t" + xiaoji);
                total += xiaoji;
                if (i != (count))
                    sb.Append("\r\n");
            }
            sb.Append("-----------------------------------------------------------------\r\n");
            sb.Append("数量: " + count + " 合计:   " + total + "\r\n");
            sb.Append("付款: 现金" + "    " + fukuan);
            sb.Append("         现金找零:" + "   " + (fukuan - total) + "\r\n");
            sb.Append("-----------------------------------------------------------------\r\n");
            sb.Append("地址：" + address + "\r\n");
            sb.Append("电话：123456789   123456789\r\n");
            sb.Append("                 谢谢惠顾欢迎下次光临                    ");

            PrintService ps = new PrintService();
            ps.StartPrint(sb.ToString(), "txt");


        }

        private void B_pay_Click(object sender, EventArgs e)
        {
            B_pay.Enabled = false;

            string made_order_url = register_asset_merchant_matching_url.Text + "/?register_asset_id=" + register_asset_id.Text + "&merchant_code=" + register_asset_merchant_code.Text + "&s=made_order";
            var made_order = Get(made_order_url);
            j1 made_order2 = JsonConvert.DeserializeObject<j1>(made_order);
            order_id.Text = made_order2.order_id;
            paid_tag.Text = "N";
            print_tag.Text = "N";
            B_pay.Enabled = true;
        }




        private void timer1_Tick(object sender, EventArgs e)
        {
            L_time.Text = DateTime.Now.ToString();

            if (order_id.Text != "" && print_tag.Text == "N" && paid_tag.Text == "N")
            {
                timer2.Enabled = true;
                timer2.Start();
            }

        }


        private void timer2_Tick(object sender, EventArgs e)
        {
            //http://matching.p20220916.lipanlong.com/?register_asset_id=VB2d666d47-e3bd2d4f&merchant_code=TEST0001&s=order_status_detail&order_id=20221107220315-169940

            string order_status_detail_url = register_asset_merchant_matching_url.Text + "/?register_asset_id=" + register_asset_id.Text + "&merchant_code=" + register_asset_merchant_code.Text + "&s=order_status_detail&order_id="+ order_id.Text;
            var order_status_detail = Get(order_status_detail_url);
            order_status_detail order_status_detail2 = JsonConvert.DeserializeObject<order_status_detail>(order_status_detail);

            print_tag.Text = order_status_detail2.shopping_order_print_tag;
            paid_tag.Text = order_status_detail2.shopping_order_paid_tag;


            if (order_status_detail2.status == "OK" && paid_tag.Text == "N")
            {
                listView1.Clear();
                listView1.Columns.Add("#", 35);
                listView1.Columns.Add("Item", 130);
                listView1.Columns.Add("Qty", 35);
                listView1.Columns.Add("Price", 45);
                listView1.Columns.Add("Sub-total", 45);
                listView1.View = View.Details;

                


                int count_0 = 1;
                foreach (var order_details in order_status_detail2.order_details)
                {
                    //order_details.shopping_orders_detail_product_name;
                    string[] row = { count_0.ToString(), order_details.shopping_orders_detail_product_name, order_details.shopping_orders_detail_product_quantity, order_details.shopping_orders_detail_product_price, order_details.shopping_orders_detail_product_price_subtotal};
                    var listViewItem = new ListViewItem(row);
                    listView1.Items.Add(listViewItem);
                    


                   

                    count_0++;
                }


                

            }


            if (order_status_detail2.status == "OK" && paid_tag.Text == "Y")
            {
                timer2.Interval = 10000;

                string tag_url = register_asset_merchant_matching_url.Text + "/?register_asset_id=" + register_asset_id.Text + "&merchant_code=" + register_asset_merchant_code.Text + "&s=order_update&order_id=" + order_id.Text+ "&paid_tag="+ paid_tag.Text+"&print_tag="+ print_tag.Text;
                var tag2 = Get(tag_url);

                StringBuilder sb = new StringBuilder();

                sb.Append("              " + order_status_detail2.merchant_name+ "\r\n");
                sb.Append("\r\n");
                sb.Append("       GST No. : " + order_status_detail2.merchant_gst+ "\r\n");
                sb.Append("\r\n");
                sb.Append("       Tax Invoice/Credit Note\r\n");
                sb.Append("\r\n");
                sb.Append("Invoice No.: "+order_id.Text+"\r\n");
                sb.Append("Date : "+ order_status_detail2.shopping_order_time+ "\r\n");
                sb.Append("Address : "+ order_status_detail2.merchant_address+ "\r\n");
                sb.Append("Email : "+ order_status_detail2.merchant_email+ "\r\n");
                sb.Append("Phone : "+ order_status_detail2.merchant_phone+ "\r\n");
                sb.Append("\r\n");
                sb.Append("\r\n");
                sb.Append("#\tItems\t\tQty\tPrice\tSub-total\r\n");
                int count_0 = 1;

                foreach (var order_details in order_status_detail2.order_details)
                {
                   



                    sb.Append(count_0.ToString() + "\t" + order_details.shopping_orders_detail_product_name + "\t" + order_details.shopping_orders_detail_product_quantity + "\t" + order_details.shopping_orders_detail_product_price + "\t" + order_details.shopping_orders_detail_product_price_subtotal + "\r\n");


                    count_0++;
                }
                sb.Append("\r\n");
                sb.Append("              Total: $" + order_status_detail2.shopping_order_amount+ "       Paid by "+ order_status_detail2.shopping_order_paid_detail+ "\r\n");


                sb.Append("\r\n");
                sb.Append("\r\n");
                sb.Append("\r\n");
                sb.Append("Thank you for shopping with us!\r\n");
                sb.Append("\r\n");
                sb.Append(order_status_detail2.merchant_add_info+"\r\n");
                sb.Append("\r\n");
                sb.Append("\r\n");
                sb.Append("\r\n");
                sb.Append(register_asset_merchant_code.Text+" | "+register_asset_id.Text+"\r\n");
                sb.Append("\r\n");
                sb.Append("\r\n");

                PrintService ps = new PrintService();
                ps.StartPrint(sb.ToString(), "txt");
                print_tag.Text = "Y";

                string tag_url2 = register_asset_merchant_matching_url.Text + "/?register_asset_id=" + register_asset_id.Text + "&merchant_code=" + register_asset_merchant_code.Text + "&s=order_update&order_id=" + order_id.Text + "&paid_tag=" + paid_tag.Text + "&print_tag=" + print_tag.Text;
                var tag2b = Get(tag_url2);
                listView1.Clear();
                order_id.Text = "";
                paid_tag.Text = "";
                print_tag.Text = "";
                B_pay.Enabled = false;

                timer2.Enabled = false;
            }



        }

        // 
        public class OrderDetail
        {
            public string shopping_orders_detail_snid { get; set; }
            public string shopping_orders_detail_order_id { get; set; }
            public string shopping_orders_detail_time { get; set; }
            public string shopping_orders_detail_mcode { get; set; }
            public string shopping_orders_detail_acode { get; set; }
            public string shopping_orders_detail_product_barcode { get; set; }
            public string shopping_orders_detail_product_name { get; set; }
            public string shopping_orders_detail_product_quantity { get; set; }
            public string shopping_orders_detail_product_price { get; set; }
            public string shopping_orders_detail_product_price_subtotal { get; set; }
        }

        public class order_status_detail
        {
            public string status { get; set; }
            public string info { get; set; }
            public List<OrderDetail> order_details { get; set; }
            public string shopping_order_snid { get; set; }
            public string shopping_order_id { get; set; }
            public string shopping_order_time { get; set; }
            public string shopping_order_mcode { get; set; }
            public string shopping_order_acode { get; set; }
            public string shopping_order_status { get; set; }
            public string shopping_order_amount { get; set; }
            public string shopping_order_items { get; set; }
            public string shopping_order_paid_tag { get; set; }
            public string shopping_order_print_tag { get; set; }
            public string shopping_order_member_code { get; set; }
            public object shopping_order_paid_detail { get; set; }
            public string merchant_snid { get; set; }
            public string merchant_number { get; set; }
            public string merchant_name { get; set; }
            public string merchant_address { get; set; }
            public string merchant_phone { get; set; }
            public string merchant_email { get; set; }
            public string merchant_gst { get; set; }
            public string merchant_add_info { get; set; }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
