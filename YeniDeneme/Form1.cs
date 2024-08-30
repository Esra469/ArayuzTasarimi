using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO.Ports;
using System.Drawing;
using System.Runtime.CompilerServices;
using AForge.Video;
using AForge.Video.DirectShow;
using GMap.NET.MapProviders;
using GMap.NET;
using AForge.Video;
using AForge.Video.DirectShow;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.MapProviders;
using GMap.NET;
using System.Windows.Forms.DataVisualization.Charting;
using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO.Ports;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace YeniDeneme
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;
       // private SerialPort serialPort2;

        private GMapOverlay markersOverlay;
        float pitch, roll, yaw;
        double takimNo = 4229795;
        double roll1, yaw1, pitch1;
     

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        public Form1()
        {
            InitializeComponent();
            InitializeChart();
            //LoadAvailablePorts2();
            LoadAvailablePorts();
            InitializeDataGridView();
            LoadDevicesToComboBox();
            InitializeGmap();
            timer2.Interval = 1000; // 1 saniyelik veri çekme aralığı
            timer2.Start();
            textBox4.Text = takimNo.ToString();
            //arayüz alarm sistemi için 
            panel1.BackColor = Color.Red;
            panel2.BackColor = Color.Red;
            panel3.BackColor = Color.Red;
            panel4.BackColor = Color.Red;
            panel5.BackColor = Color.Red;
            // GL.ClearColor(Color.Pink);//Color.FromArgb(143, 212, 150)

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.Pink);
            map.DragButton = MouseButtons.Left;
            map.MapProvider = GMapProviders.GoogleMap;

        }
        private void InitializeGmap()
        {
            map.MapProvider = GMapProviders.GoogleMap;
            map.Position = new PointLatLng(0, 0);//Başlangıç konumu
            map.MinZoom = 10;
            map.MaxZoom = 10;
            map.Zoom = 100;
            map.Manager.Mode = AccessMode.ServerAndCache;

            markersOverlay = new GMapOverlay("markers");
            map.Overlays.Add(markersOverlay);
        }
        private void LoadAvailablePorts()
        {
            string[] ports = SerialPort.GetPortNames();
            comboBoxPorts.Items.AddRange(ports);
        }
        //private void LoadAvailablePorts2()
        //{
        //    string[] ports = SerialPort.GetPortNames();
        //    comboBox2.Items.AddRange(ports);
        //}

        private void InitializeChart()
        {
            // Basınç grafiği
            chart1.Series.Clear();
            Series pressureSeries = new Series("Basinc");
            pressureSeries.ChartType = SeriesChartType.Line;
            chart1.Series.Add(pressureSeries);
            chart1.ChartAreas[0].AxisX.Title = "Time";
            chart1.ChartAreas[0].AxisY.Title = "Basınc (hPa)";

            //sicaklık
            chart2.Series.Clear();
            Series temperatureSeries = new Series("Sicaklik");
            temperatureSeries.ChartType = SeriesChartType.Line;
            chart2.Series.Add(temperatureSeries);
            chart2.ChartAreas[0].AxisX.Title = "Zaman";
            chart2.ChartAreas[0].AxisY.Title = "Sicaklik (°C)";

            //otomatik ölçeklendirme
            chart2.ChartAreas[0].AxisY.IsStartedFromZero = true;
            chart2.ChartAreas[0].AxisX.Interval = 0;

            //irtifa
            chart3.Series.Clear();
            Series irtifa = new Series("irtifa");
            irtifa.ChartType = SeriesChartType.Line;
            chart3.Series.Add(irtifa);
            chart3.ChartAreas[0].AxisX.Title = "Zaman";
            chart3.ChartAreas[0].AxisY.Title = "Yükseklik (m)";

            //hız
            chart4.Series.Clear();
            Series Hiz = new Series("Hiz");
            Hiz.ChartType = SeriesChartType.Line;
            chart4.Series.Add(Hiz);
            chart4.ChartAreas[0].AxisX.Title = "Zaman";
            chart4.ChartAreas[0].AxisY.Title = "Hiz (km/h)";

            //pil gerilimi
            chart5.Series.Clear();
            Series voltaj = new Series("voltaj");
            voltaj.ChartType = SeriesChartType.Line;
            chart5.Series.Add(voltaj);
            chart5.ChartAreas[0].AxisX.Title = "Zaman";
            chart5.ChartAreas[0].AxisY.Title = "Pil Gerilimi (V)";

        }

        private void InitializeDataGridView()
        {
            // DataGridView için sütunlar ekleme
            dataGridView1.Columns.Add("Guncel saat", "Guncel saat");
            dataGridView1.Columns.Add("Paket Numarası   ", "Paket Numarası");
            //dataGridView1.Columns.Add("Uydu Statusu", "Uydu statusu");
            //dataGridView1.Columns.Add("Hata Kodu", "Hata kodu");
            //dataGridView1.Columns.Add("Zaman", "Time");
            dataGridView1.Columns.Add("Basınç", "Basınç (hPa)");
            dataGridView1.Columns.Add("Taşıyıcı Basınç", "Taşıyıcı Basınç (hPa)");
            dataGridView1.Columns.Add("İrtifa", "İrtifa (m)");
            dataGridView1.Columns.Add("Taşıyıcı İrtifa", "Taşıyıcı İrtifa (m)");
            dataGridView1.Columns.Add("Irtifa Farkı", "İrtifa Farkı (m)");
            //dataGridView1.Columns.Add("İniş Hızı", "Speed (km/h)");
            dataGridView1.Columns.Add("Sıcaklık", "Sıcaklık (°C)");
            dataGridView1.Columns.Add("Pil Gerilimi", "Pil Gerilimi");
           // dataGridView1.Columns.Add("Enlem", "Latitude");
            //dataGridView1.Columns.Add("Boylam", "Longitude");
            //dataGridView1.Columns.Add("gps alt?", "gps alt?");
            dataGridView1.Columns.Add("Pitch", "Pitch");
            dataGridView1.Columns.Add("Roll", "Roll");
            dataGridView1.Columns.Add("Yaw", "Yaw");
            //dataGridView1.Columns.Add("RHRH", "Komut");
            dataGridView1.Columns.Add("Takım No", "Takım No");
        }
        private void InitializeSerialPort(string portName)
        {
            try
            {
                serialPort = new SerialPort(portName, 9600);
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    MessageBox.Show("Bağlanıldı: " + portName);
                    timer2.Start(); // Verileri almaya başlamak için timer'ı başlat
                    buttonConnect.Enabled = false; // Bağlanma butonunu devre dışı bırak
                    buttonStop.Enabled = true; // Durdur butonunu etkinleştir
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlanılamadı: " + portName + "\nHata: " + ex.Message);
            }
        }

        private void InitializeSerialPort2(string portName)
        {
            try
            {
                serialPort = new SerialPort(portName, 9600);
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    MessageBox.Show("Bağlanıldı: " + portName);
                    timer2.Start(); // Verileri almaya başlamak için timer'ı başlat
                    button2.Enabled = false; // Bağlanma butonunu devre dışı bırak
                    button3.Enabled = true; // Durdur butonunu etkinleştir
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlanılamadı: " + portName + "\nHata: " + ex.Message);
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadLine();
            this.Invoke(new MethodInvoker(delegate
            {
                string[] values = data.Split(',');

                if (values.Length == 11 )
                {
                    double sicaklik, basinc, irtifa, fark, pitch, roll, yaw;
                    double voltaj, basinc2, irtifa2;
                    double sayac;
                    double hiz = 0;

                    if (double.TryParse(values[0], out basinc))
                    {
                        basinc = basinc / 10000;
                        ProcessBasinc(basinc);
                    }
                    if (double.TryParse(values[1], out sicaklik))
                    {
                        sicaklik = sicaklik / 100;
                        ProcessSicaklik(sicaklik);
                    }
                    if (double.TryParse(values[2], out irtifa))
                    {
                        irtifa = irtifa / 100;
                        ProcessIrtifa(irtifa);
                    }
                    
                    if (double.TryParse(values[3], out voltaj))
                    {

                        voltaj = voltaj / 100;
                        ProcessVolaj(voltaj);
                    }
                   // if (double.TryParse(values[4], out hiz))
                    //{
                        ProcessHiz(hiz);
                    //}
                    if (double.TryParse(values[4], out pitch) &&
                       double.TryParse(values[5], out roll) &&
                       double.TryParse(values[6], out yaw))
                    {   pitch = pitch / 100;
                        roll = roll / 100;
                        yaw = yaw / 100;
                        ProcessOrientation(pitch, roll, yaw);
                    }
                    if (double.TryParse(values[7], out basinc2))
                    {

                        basinc2 = basinc2 / 10000;
                        textBox3.Text = basinc2.ToString();
                        if (basinc2 > 0)
                        {
                            
                            panel1.BackColor = Color.Green;
                        }
                        else
                        {
                            panel1.BackColor = Color.Red;
                        }
                    }
                    if (double.TryParse(values[8],out sayac))
                    {
                        textBox5.Text = sayac.ToString();
                    }
                    if (double.TryParse(values[9],out irtifa2))
                    {
                        irtifa2 = irtifa2 / 100;
                        textBox2.Text= irtifa2.ToString();
                    }
                    if (double.TryParse(values[10], out fark))
                    {
                        fark = fark / 100;
                        textBox1.Text = fark.ToString();
                    }

                    //PointLatLng point = new PointLatLng(enlem, boylam);
                    //map.Position = point;
                    //markersOverlay.Markers.Clear();
                    //GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.red_dot);
                    //markersOverlay.Markers.Add(marker);

                    string time = DateTime.Now.ToString("HH:mm:ss");
                    double.TryParse(values[4], out pitch1);
                    double.TryParse(values[5], out roll1);
                    double.TryParse(values[6], out yaw1);

                    roll1 = roll1 / 100;
                    yaw1 = yaw1 / 100;
                    pitch1 = pitch1 / 100;

                    AddDataToDataGridView(time, sayac, basinc, basinc2, irtifa, irtifa2, fark, sicaklik, voltaj, pitch, roll1, yaw1, takimNo);

                }
            }));
        }
        private void ProcessBasinc(double basinc)
        {
            // Basınç verisini grafiğe ekle
            chart1.Series["Basinc"].Points.AddY(basinc);
        }
        private void ProcessSicaklik(double sicaklik)
        {
            // Sıcaklık verisini grafiğe ekle
            chart2.Series["Sicaklik"].Points.AddY(sicaklik);
        }
        private void ProcessIrtifa(double irtifa)
        {
            chart3.Series["irtifa"].Points.AddY(irtifa);
        }
        private void ProcessHiz(double hiz)
        {
            chart4.Series["Hiz"].Points.AddY(hiz);
        }
        private void ProcessVolaj(double voltaj)
        {
            chart5.Series["voltaj"].Points.AddY(voltaj);
        }



        private void ProcessOrientation(double pitch, double roll, double yaw)
        {
            // Pitch, roll, yaw değerlerini label'larda göster
            labelPitch.Text = "Pitch: " + pitch.ToString();
            labelRoll.Text = "Roll: " + roll.ToString();
            labelYaw.Text = "Yaw: " + yaw.ToString();

            // GLControl'ü yeniden çizmek için
            glControl1.Invalidate();
        }

        //gelen verileri buraya at
        private void AddDataToDataGridView(string time, double sayac, double basinc, double basinc2, double irtifa, double irtifa2, double fark, double sicaklik, double voltaj, double pitch, double roll1, double yaw1, double takimNo)
        {
            dataGridView1.Rows.Add(time, sayac, basinc, basinc2, irtifa, irtifa2, fark, sicaklik, voltaj, pitch, roll1, yaw1, takimNo);
        }

      
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (comboBoxPorts.SelectedItem != null)
            {
                string selectedPort = comboBoxPorts.SelectedItem.ToString();
                InitializeSerialPort(selectedPort);
            }
            else
            {
                MessageBox.Show("Lütfen bir COM port seçin.");
            }
        }
        private void buttonStop_Click(object sender, EventArgs e)
        {
            StopConnection();
        }
        private void StopConnection()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
                timer2.Stop();
                MessageBox.Show("Bağlantı kesildi.");
                buttonConnect.Enabled = true; // Bağlanma butonunu yeniden etkinleştir
                buttonStop.Enabled = false; // Durdur butonunu devre dışı bırak
            }
        }
 
        private void diskKontrol_clik(object sender, EventArgs e)
        {
            string command = commandTextBox.Text.Trim(); // Komutu Label veya TextBox'tan al
            if (IsValidCommand(command))
            {
                SendCommandToArduino(command);
               // panelStatus.BackColor = Color.Green;
            }
            else
            {
                MessageBox.Show("Geçersiz komut! Geçerli bir komut girin (örneğin: R4G6).");
              //  panelStatus.BackColor = Color.Red;
            }
        }
        private bool IsValidCommand(string command)
        {
            // Komutun geçerliliğini kontrol et (örneğin, R4G6 gibi bir formatta olmalı)
            if (command.Length == 4 &&
                (command[1] == 'R' || command[1] == 'G' || command[1] == 'B' || command[1] == 'W') &&
                char.IsDigit(command[0]) &&
                (command[3] == 'R' || command[3] == 'G' || command[3] == 'B' || command[3] == 'W') &&
                char.IsDigit(command[2]))
            {
                return true;
            }
            return false;
        }
        private void SendCommandToArduino(string command)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine(command); // Komutu Arduino'ya gönder
                MessageBox.Show("Komut gönderildi: " + command);
            }
            else
            {
                MessageBox.Show("Seri port açık değil. Lütfen bağlantıyı kontrol edin.");
            }
        }

        private void LoadDevicesToComboBox()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            // Cihazları comboBox1'e ekle
            foreach (FilterInfo device in videoDevices)
            {
                comboBox1.Items.Add(device.Name);
            }

            // İkinci cihazı seç (Eğer varsa)
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Video giriş cihazı bulunamadı.");
            }
        }

        private void kamera_Click(object sender, EventArgs e)
        {
            if (videoDevices.Count > 0)
            {
                // Seçilen cihazı al
                videoSource = new VideoCaptureDevice(videoDevices[comboBox1.SelectedIndex].MonikerString);
                videoSource.NewFrame += FinalFrame_newFrame;

                // Video kaynağını başlat
                videoSource.Start();
            }
            else
            {
                MessageBox.Show("Video giriş cihazı seçilmedi.");
            }
        }

        private void FinalFrame_newFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void manuelAyrılma(object sender, EventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.WriteLine("AYRILMA");
                MessageBox.Show("Ayrıl komutu gönderildi.");
            }
            else
            {
                MessageBox.Show("Seri port açık değil.");
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }

            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }


        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);//sonradan yazdık
        }

   

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)
            {
                string selectedPort2 = comboBox2.SelectedItem.ToString();
                InitializeSerialPort2(selectedPort2);
            }
            else
            {
                MessageBox.Show("Lütfen bir COM port seçin.");
            }
        }

       

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void silindir(float step, float topla, float radius, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(BeginMode.Quads);//Y EKSEN CIZIM DAİRENİN
            while (step <= 360)
            {
                if (step < 45)
                    GL.Color3(Color.FromArgb(128, 128, 128));
                else if (step < 90)
                    GL.Color3(Color.FromArgb(255, 255, 255));
                else if (step < 135)
                    GL.Color3(Color.FromArgb(128, 128, 128));
                else if (step < 180)
                    GL.Color3(Color.FromArgb(255, 255, 255));
                else if (step < 225)
                    GL.Color3(Color.FromArgb(128, 128, 128));
                else if (step < 270)
                    GL.Color3(Color.FromArgb(255, 255, 255));
                else if (step < 315)
                    GL.Color3(Color.FromArgb(128, 128, 128));
                else if (step < 360)
                    GL.Color3(Color.FromArgb(255, 255, 255));


                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 2) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 2) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();
            GL.Begin(BeginMode.Lines);
            step = eski_step;
            topla = step;
            while (step <= 180)// UST KAPAK
            {
                if (step < 45)
                    GL.Color3(Color.FromArgb(128, 128, 128));
                else if (step < 90)
                    GL.Color3(Color.FromArgb(250, 255, 255));
                else if (step < 135)
                    GL.Color3(Color.FromArgb(128, 128, 128));
                else if (step < 180)
                    GL.Color3(Color.FromArgb(255, 255, 255));
                else if (step < 225)
                    GL.Color3(Color.FromArgb(128, 128, 128));
                else if (step < 270)
                    GL.Color3(Color.FromArgb(250, 255, 255));
                else if (step < 315)
                    GL.Color3(Color.FromArgb(128, 128, 128));
                else if (step < 360)
                    GL.Color3(Color.FromArgb(255, 255, 255));

                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);

                GL.Vertex3(ciz1_x, dikey1, ciz1_y);
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);
                step += topla;
            }
            step = eski_step;
            topla = step;
            while (step <= 180)//ALT KAPAK
            {
                if (step < 45)
                    GL.Color3(Color.FromArgb(128, 128, 128));
                else if (step < 90)
                    GL.Color3(Color.FromArgb(255, 255, 255));
                else if (step < 135)
                    GL.Color3(Color.FromArgb(128, 128, 128));
                else if (step < 180)
                    GL.Color3(Color.FromArgb(255, 255, 255));
                else if (step < 225)
                    GL.Color3(Color.FromArgb(128, 128, 128));
                else if (step < 270)
                    GL.Color3(Color.FromArgb(255, 255, 255));
                else if (step < 315)
                    GL.Color3(Color.FromArgb(128, 128, 128));
                else if (step < 360)
                    GL.Color3(Color.FromArgb(255, 255, 255));

                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey2, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();
        }
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            float step = 1.0f;
            float topla = step;
            float radius = 5.0f;
            float dikey1 = radius, dikey2 = -radius;
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.04f, 4 / 3, 1, 10000);
            Matrix4 lookat = Matrix4.LookAt(25, 0, 0, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Rotate(pitch1, 1.0, 0.0, 0.0);//ÖNEMLİ
            GL.Rotate(roll1, 0.0, 1.0, 0.0);
            GL.Rotate(yaw1, 0.0, 0.0, 1.0);

            silindir(step, topla, radius, 6, -5);
            //silindir(0.01f, topla, 0.5f, 5, 9.7f);
            // silindir(0.01f, topla, 0.1f, 5, dikey1 + 5);
            // koni(0.01f, 0.01f, radius, 3.0f, 3, 5);
            // koni(0.01f, 0.01f, radius, 2.0f, -5.0f, -10.0f);
            // Pervane(9.0f, 11.0f, 0.2f, 0.5f);

            GL.Begin(BeginMode.Lines);

            GL.Color3(Color.FromArgb(250, 0, 0));
            GL.Vertex3(-30.0, 0.0, 0.0);
            GL.Vertex3(30.0, 0.0, 0.0);


            GL.Color3(Color.FromArgb(0, 0, 0));
            GL.Vertex3(0.0, 30.0, 0.0);
            GL.Vertex3(0.0, -30.0, 0.0);

            GL.Color3(Color.FromArgb(0, 0, 250));
            GL.Vertex3(0.0, 0.0, 30.0);
            GL.Vertex3(0.0, 0.0, -30.0);

            GL.End();
            //GraphicsContext.CurrentContext.VSync = true;
            glControl1.SwapBuffers();
        }
    }
}
