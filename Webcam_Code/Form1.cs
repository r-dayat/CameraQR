using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;

namespace Webcam_Code
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection infoCollection;
        private int webcam1 = -1;
        private Boolean isQuerying = false;
        private string lastDecode = String.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webcamList();
            timerTaskQR();
        }

        private void webcamList()
        {
            infoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            int counter = 0;

            foreach (FilterInfo videoCapture in infoCollection)
            {
                counter = counter + 1;
                CListWebcam.Items.Add(videoCapture.Name + " " + counter.ToString());
            }
        }

        private void bConnect_Click(object sender, EventArgs e)
        {
            connectWebcam();
        }

        private void connectWebcam()
        {
            webcam1 = CListWebcam.SelectedIndex;
            try
            {
                if (webcam1 != -1)
                {
                    int index = webcam1;
                    videoSourcePlayer1.VideoSource = (IVideoSource)new VideoCaptureDevice(infoCollection[index].MonikerString);
                    videoSourcePlayer1.Start();
                }
                else
                {
                    MessageBox.Show("Please, select Webcam !");
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            closeForm();
            System.Windows.Forms.Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            closeForm();
        }

        private void closeForm()
        {
            try
            {
                if (videoSourcePlayer1.IsRunning)
                {
                    videoSourcePlayer1.SignalToStop();
                    videoSourcePlayer1.WaitForStop();
                    videoSourcePlayer1.Stop();
                }
            }
            catch (Exception e)
            {

            }
            this.Dispose();
        }

        private void timerTaskQR()
        {
            timerTask.Interval = 1000;
            timerTask.Tick += new EventHandler(timerTask_Tick);
            timerTask.Start();
        }

        private void timerTask_Tick(object sender, EventArgs e)
        {
            if (!isQuerying)
            {
                isQuerying = true;
                txtValue.Text = "";
                if (videoSourcePlayer1.IsRunning)
                {
                    Bitmap b = null;
                    b = videoSourcePlayer1.GetCurrentVideoFrame();
                    
                    if (b != null)
                    {
                        BarcodeReader reader = new BarcodeReader();
                        Result result = reader.Decode(b);
                        try
                        {
                            string decode = result.ToString().Trim();
                            if (decode != "" && decode != lastDecode)
                            {
                                txtValue.Text = decode;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                        isQuerying = false;
                    }
                    
                }
                isQuerying = false;
            }
        }
    }
}
