using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.ComponentModel;
using System.IO;
using Database;

namespace Forms
{
    public partial class fCockpick : Form
    {

        fDataBank store = new fDataBank();
        int id = 0;

        // this timer calls bgWorker again and again after regular intervals
        System.Windows.Forms.Timer tmrCallBgWorker;

        // this is our worker
        BackgroundWorker bgWorker;

        // this is the timer to make sure that worker gets called
        System.Threading.Timer tmrEnsureWorkerGetsCalled;

        // object used for safe access
        object lockObject = new object();

        public fCockpick()
        {
            InitializeComponent();
            getAvailablePort();
            pnSerial.Location = new Point(12, 61);
            tbTimeofSamples.Text = Properties.Settings.Default.STimeOfSamples.ToString();


            // this timer calls bgWorker again and again after regular intervals
            tmrCallBgWorker = new System.Windows.Forms.Timer();
            tmrCallBgWorker.Tick += new EventHandler(tmrCallBgWorker_Tick);
            tmrCallBgWorker.Interval = 1;

            // this is our worker
            bgWorker = new BackgroundWorker();
            // work happens in this method
            bgWorker.DoWork += new DoWorkEventHandler(bg_DoWork);
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);

            store.Show();
            store.SetStop();
            store.Visibility();
        }
        int TimerTick = 0;
        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (store.GetStop())
            {
                bStopAcquire_Click(sender, e);
                store.bShowScroll_Click(sender, e);
                store.SetStop();
            }

            if (mySerialPort.IsOpen)
            {
                if (!store.GetSaveRefresh())
                {
                    bStopSaveRefresh.Visible = false;
                }

                if (store.GetRestart())
                {
                    bStopAcquire_Click(sender, e);
                    if(!store.GetSaveRefresh())
                    {
                        store.SetStop();
                    }
                }

                if(store.GetSaveRefresh())
                {
                    bStopSaveRefresh.Location = new Point(12, 96);
                    bStopSaveRefresh.Visible = true;
                    if (TimerTick == store.SetTimerTick())
                    {
                        store.varSaveRefresh_Click(sender, e);
                        TimerTick = 0;
                    }
                    TimerTick++;
                }

                System.Diagnostics.Debug.WriteLine("Complete");
                tbSend.Text = "1";
                bSendTest_Click(sender, e);
                bClosePort.Enabled = true;
            }
            bClosePort.Enabled = true;
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            // does a job like writing to serial communication, webservices etc
            System.Threading.Thread.Sleep(Properties.Settings.Default.STimeOfSamples);
        }

        void tmrCallBgWorker_Tick(object sender, EventArgs e)
        {
            if (Monitor.TryEnter(lockObject))
            {
                try
                {
                    // if bgworker is not busy the call the worker
                    if (!bgWorker.IsBusy)
                        bgWorker.RunWorkerAsync();
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }

            }
            else
            {

                // as the bgworker is busy we will start a timer that will try to call the bgworker again after some time
                tmrEnsureWorkerGetsCalled = new System.Threading.Timer(new TimerCallback(tmrEnsureWorkerGetsCalled_Callback), null, 0, 10);

            }

        }

        void tmrEnsureWorkerGetsCalled_Callback(object obj)
        {
            // this timer was started as the bgworker was busy before now it will try to call the bgworker again
            if (Monitor.TryEnter(lockObject))
            {
                try
                {
                    if (!bgWorker.IsBusy)
                        bgWorker.RunWorkerAsync();
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
                tmrEnsureWorkerGetsCalled = null;
            }
        }

        void Brake(string brake_light)
        {
            int on_off = Convert.ToInt16(brake_light[3] - 48);

            if (on_off == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedBrake);
                pbLedBrake.Location = new Point(213, 479);
                pbLedBrake.BackColor = Color.Transparent;
                pbLedBrake.Visible = true;
            }
        }

        void DeadPoint(string dead_point)
        {
            int on_off = Convert.ToInt16(dead_point[3] - 48);
            if (on_off == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedDeadPoint);
                pbLedDeadPoint.Location = new Point(243 + 7, 532 - 11);
                pbLedDeadPoint.BackColor = Color.Transparent;
                pbLedDeadPoint.Visible = true;
            }
        }

        void LevelOil(string oil_lvl)
        {
            int on_off = Convert.ToInt16(oil_lvl[3] - 48);
            if (on_off == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedOilLevel);
                pbLedOilLevel.Location = new Point(312 + 6, 560 - 11);
                pbLedOilLevel.BackColor = Color.Transparent;
                pbLedOilLevel.Visible = true;
            }
        }

        void LevelGas(string gas_lvl)
        {
            int on_off = Convert.ToInt16(gas_lvl[3] - 48);
            if (on_off == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedGasLevel);
                pbLedGasLevel.Location = new Point(378 + 6, 532 - 11);
                pbLedGasLevel.BackColor = Color.Transparent;
                pbLedGasLevel.Visible = true;
            }
        }

        void PowerGood(string power_good)
        {
            int on_off = Convert.ToInt16(power_good[3] - 48);
            if (on_off == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedPowerGood);
                pbLedPowerGood.Location = new Point(416 + 6, 489 - 11);
                pbLedPowerGood.BackColor = Color.Transparent;
                pbLedPowerGood.Visible = true;
            }
        }

        void OilTemp(string oiltemp)
        {
            int oil_temp = Convert.ToInt32(oiltemp);
            tbOilTemp.Text = (oil_temp / 10).ToString() + "," + (oil_temp - (oil_temp / 10) * 10).ToString()+" ºC";
        }

        void AirTemp(string airtemp)
        {
            int air_temp = Convert.ToInt32(airtemp);
            tbAirTemp.Text = (air_temp / 10).ToString() + "," + (air_temp - (air_temp / 10) * 10).ToString() + " ºC";
        }

        void Rotation(int angle)
        {
            Graphics g;
            g = CreateGraphics();

            Bitmap b = new Bitmap(pbSteeringWheel.Width, pbSteeringWheel.Height);

            //now we set the rotation point to the center of our image
            g.TranslateTransform((float)720, (float)340);

            g.RotateTransform((float)angle);

            //now we set the rotation point to back of the corner of image, for plot the image in the same local where she lives
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);

            //set the InterpolationMode to HighQualityBicubic so to ensure a high
            //quality image once it is transformed to the specified size
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //turn the Bitmap into a Graphics object


            pbSteeringWheel.DrawToBitmap(b, new Rectangle(0, 0, b.Width, b.Height));
            
            

            g.DrawImage(b, new Point(0, 0));

            g.Dispose();

            tbSteeringWheel.Text = angle.ToString() + "º";

        }

        void Speed(int digit_1, int digit_2, int digit_3, int digit_4)
        {
            if (digit_1 == 0)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit1_0);
                pbSpeedDigit1_0.Location = new Point(355, 356);
                pbSpeedDigit1_0.BackColor = Color.Transparent;
                pbSpeedDigit1_0.Visible = true;
            }
            if (digit_1 == 1)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit1_1);
                pbSpeedDigit1_1.Location = new Point(355, 356);
                pbSpeedDigit1_1.BackColor = Color.Transparent;
                pbSpeedDigit1_0.Visible = false;
                pbSpeedDigit1_1.Visible = true;
            }
            if (digit_1 == 2)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit1_2);
                pbSpeedDigit1_2.Location = new Point(355, 356);
                pbSpeedDigit1_2.BackColor = Color.Transparent;
                pbSpeedDigit1_2.Visible = true;
            }
            if (digit_1 == 3)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit1_3);
                pbSpeedDigit1_3.Location = new Point(355, 356);
                pbSpeedDigit1_3.BackColor = Color.Transparent;
                pbSpeedDigit1_3.Visible = true;
            }
            if (digit_1 == 4)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit1_4);
                pbSpeedDigit1_4.Location = new Point(355, 356);
                pbSpeedDigit1_4.BackColor = Color.Transparent;
                pbSpeedDigit1_4.Visible = true;
            }
            if (digit_1 == 5)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit1_5);
                pbSpeedDigit1_5.Location = new Point(355, 356);
                pbSpeedDigit1_5.BackColor = Color.Transparent;
                pbSpeedDigit1_5.Visible = true;
            }
            if (digit_1 == 6)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit1_6);
                pbSpeedDigit1_6.Location = new Point(355, 356);
                pbSpeedDigit1_6.BackColor = Color.Transparent;
                pbSpeedDigit1_6.Visible = true;
            }
            if (digit_1 == 7)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit1_7);
                pbSpeedDigit1_7.Location = new Point(355, 356);
                pbSpeedDigit1_7.BackColor = Color.Transparent;
                pbSpeedDigit1_7.Visible = true;
            }
            if (digit_1 == 8)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit1_8);
                pbSpeedDigit1_8.Location = new Point(355, 356);
                pbSpeedDigit1_8.BackColor = Color.Transparent;
                pbSpeedDigit1_8.Visible = true;
            }
            if (digit_1 == 9)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit1_9);
                pbSpeedDigit1_9.Location = new Point(355, 356);
                pbSpeedDigit1_9.BackColor = Color.Transparent;
                pbSpeedDigit1_9.Visible = true;
            }
            if (digit_2 == 0)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit2_0);
                pbSpeedDigit2_0.Location = new Point(322, 356);
                pbSpeedDigit2_0.BackColor = Color.Transparent;
                pbSpeedDigit2_0.Visible = true;
            }
            if (digit_2 == 1)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit2_1);
                pbSpeedDigit2_1.Location = new Point(322, 356);
                pbSpeedDigit2_1.BackColor = Color.Transparent;
                pbSpeedDigit2_1.Visible = true;
            }
            if (digit_2 == 2)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit2_2);
                pbSpeedDigit2_2.Location = new Point(322, 356);
                pbSpeedDigit2_2.BackColor = Color.Transparent;
                pbSpeedDigit2_2.Visible = true;
            }
            if (digit_2 == 3)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit2_3);
                pbSpeedDigit2_3.Location = new Point(322, 356);
                pbSpeedDigit2_3.BackColor = Color.Transparent;
                pbSpeedDigit2_3.Visible = true;
            }
            if (digit_2 == 4)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit2_4);
                pbSpeedDigit2_4.Location = new Point(322, 356);
                pbSpeedDigit2_4.BackColor = Color.Transparent;
                pbSpeedDigit2_4.Visible = true;
            }
            if (digit_2 == 5)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit2_5);
                pbSpeedDigit2_5.Location = new Point(322, 356);
                pbSpeedDigit2_5.BackColor = Color.Transparent;
                pbSpeedDigit2_5.Visible = true;
            }
            if (digit_2 == 6)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit2_6);
                pbSpeedDigit2_6.Location = new Point(322, 356);
                pbSpeedDigit2_6.BackColor = Color.Transparent;
                pbSpeedDigit2_6.Visible = true;
            }
            if (digit_2 == 7)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit2_7);
                pbSpeedDigit2_7.Location = new Point(322, 356);
                pbSpeedDigit2_7.BackColor = Color.Transparent;
                pbSpeedDigit2_7.Visible = true;
            }
            if (digit_2 == 8)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit2_8);
                pbSpeedDigit2_8.Location = new Point(322, 356);
                pbSpeedDigit2_8.BackColor = Color.Transparent;
                pbSpeedDigit2_8.Visible = true;
            }
            if (digit_2 == 9)
            {
                pbSteeringWheel.Controls.Add(pbSpeedDigit2_9);
                pbSpeedDigit2_9.Location = new Point(322, 356);
                pbSpeedDigit2_9.BackColor = Color.Transparent;
                pbSpeedDigit2_9.Visible = true;
            }
            if (digit_3 >= 1 || digit_4 == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedSpeed1);
                pbLedSpeed1.Location = new Point(242, 337);
                pbLedSpeed1.BackColor = Color.Transparent;
                pbLedSpeed1.Visible = true;
                if (digit_3 == 1)
                {
                    pbSteeringWheel.Controls.Add(pbSpeedDigit3_1);
                    pbSpeedDigit3_1.Location = new Point(291, 356);
                    pbSpeedDigit3_1.BackColor = Color.Transparent;
                    pbSpeedDigit3_1.Visible = true;
                }
            }
            if (digit_3 >= 2 || digit_4 == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedSpeed2);
                pbLedSpeed2.Location = new Point(247, 324);
                pbLedSpeed2.BackColor = Color.Transparent;
                pbLedSpeed2.Visible = true;
                if (digit_3 == 2)
                {
                    pbSteeringWheel.Controls.Add(pbSpeedDigit3_2);
                    pbSpeedDigit3_2.Location = new Point(291, 356);
                    pbSpeedDigit3_2.BackColor = Color.Transparent;
                    pbSpeedDigit3_2.Visible = true;
                }
            }
            if (digit_3 == 0 && digit_4 == 1)
            {

                pbSteeringWheel.Controls.Add(pbSpeedDigit3_0);
                pbSpeedDigit3_0.Location = new Point(291, 356);
                pbSpeedDigit3_0.BackColor = Color.Transparent;
                pbSpeedDigit3_0.Visible = true;
            }
            if (digit_3 >= 3 || digit_4 == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedSpeed3);
                pbLedSpeed3.Location = new Point(254, 312);
                pbLedSpeed3.BackColor = Color.Transparent;
                pbLedSpeed3.Visible = true;
                if (digit_3 == 3)
                {
                    pbSteeringWheel.Controls.Add(pbSpeedDigit3_3);
                    pbSpeedDigit3_3.Location = new Point(291, 356);
                    pbSpeedDigit3_3.BackColor = Color.Transparent;
                    pbSpeedDigit3_3.Visible = true;
                }
            }
            if (digit_3 >= 4 || digit_4 == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedSpeed4);
                pbLedSpeed4.Location = new Point(263, 302);
                pbLedSpeed4.BackColor = Color.Transparent;
                pbLedSpeed4.Visible = true;
                if (digit_3 == 4)
                {
                    pbSteeringWheel.Controls.Add(pbSpeedDigit3_4);
                    pbSpeedDigit3_4.Location = new Point(291, 356);
                    pbSpeedDigit3_4.BackColor = Color.Transparent;
                    pbSpeedDigit3_4.Visible = true;
                }
            }
            if (digit_3 >= 5 || digit_4 == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedSpeed5);
                pbLedSpeed5.Location = new Point(273, 295);
                pbLedSpeed5.BackColor = Color.Transparent;
                pbLedSpeed5.Visible = true;
                if (digit_3 == 5)
                {
                    pbSteeringWheel.Controls.Add(pbSpeedDigit3_5);
                    pbSpeedDigit3_5.Location = new Point(291, 356);
                    pbSpeedDigit3_5.BackColor = Color.Transparent;
                    pbSpeedDigit3_5.Visible = true;
                }
            }
            if (digit_3 >= 6 || digit_4 == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedSpeed6);
                pbLedSpeed6.Location = new Point(285, 288);
                pbLedSpeed6.BackColor = Color.Transparent;
                pbLedSpeed6.Visible = true;
                if (digit_3 == 6)
                {
                    pbSteeringWheel.Controls.Add(pbSpeedDigit3_6);
                    pbSpeedDigit3_6.Location = new Point(291, 356);
                    pbSpeedDigit3_6.BackColor = Color.Transparent;
                    pbSpeedDigit3_6.Visible = true;
                }
            }
            if (digit_3 >= 7 || digit_4 == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedSpeed7);
                pbLedSpeed7.Location = new Point(298, 284);
                pbLedSpeed7.BackColor = Color.Transparent;
                pbLedSpeed7.Visible = true;
                if (digit_3 == 7)
                {
                    pbSteeringWheel.Controls.Add(pbSpeedDigit3_7);
                    pbSpeedDigit3_7.Location = new Point(291, 356);
                    pbSpeedDigit3_7.BackColor = Color.Transparent;
                    pbSpeedDigit3_7.Visible = true;
                }
            }
            if (digit_3 >= 8 || digit_4 == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedSpeed8);
                pbLedSpeed8.Location = new Point(311, 281);
                pbLedSpeed8.BackColor = Color.Transparent;
                pbLedSpeed8.Visible = true;
                if (digit_3 == 8)
                {
                    pbSteeringWheel.Controls.Add(pbSpeedDigit3_8);
                    pbSpeedDigit3_8.Location = new Point(291, 356);
                    pbSpeedDigit3_8.BackColor = Color.Transparent;
                    pbSpeedDigit3_8.Visible = true;
                }
            }
            if (digit_3 >= 9 || digit_4 == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedSpeed9);
                pbLedSpeed9.Location = new Point(325, 281);
                pbLedSpeed9.BackColor = Color.Transparent;
                pbLedSpeed9.Visible = true;
                if (digit_3 == 9)
                {
                    pbSteeringWheel.Controls.Add(pbSpeedDigit3_9);
                    pbSpeedDigit3_9.Location = new Point(291, 356);
                    pbSpeedDigit3_9.BackColor = Color.Transparent;
                    pbSpeedDigit3_9.Visible = true;
                }
            }
            if (digit_4 == 1)
            {
                pbSteeringWheel.Controls.Add(pbLedSpeed10);
                pbLedSpeed10.Location = new Point(416 + 6, 289 - 11);
                pbLedSpeed10.BackColor = Color.Transparent;
                pbLedSpeed10.Visible = true;

                pbSteeringWheel.Controls.Add(pbSpeedDigit4_1);
                pbSpeedDigit4_1.Location = new Point(259, 356);
                pbSpeedDigit4_1.BackColor = Color.Transparent;
                pbSpeedDigit4_1.Visible = true;

                if (digit_3 >= 0)
                {
                    pbSteeringWheel.Controls.Add(pbLedSpeed10);
                    pbLedSpeed10.Location = new Point(338, 283);
                    pbLedSpeed10.BackColor = Color.Transparent;
                    pbLedSpeed10.Visible = true;
                }
                if (digit_3 >= 1)
                {
                    pbSteeringWheel.Controls.Add(pbLedSpeed11);
                    pbLedSpeed11.Location = new Point(351, 288);
                    pbLedSpeed11.BackColor = Color.Transparent;
                    pbLedSpeed11.Visible = true;
                }
                if (digit_3 >= 2)
                {
                    pbSteeringWheel.Controls.Add(pbLedSpeed12);
                    pbLedSpeed12.Location = new Point(363, 295);
                    pbLedSpeed12.BackColor = Color.Transparent;
                    pbLedSpeed12.Visible = true;
                }
                if (digit_3 >= 3)
                {
                    pbSteeringWheel.Controls.Add(pbLedSpeed13);
                    pbLedSpeed13.Location = new Point(374, 303);
                    pbLedSpeed13.BackColor = Color.Transparent;
                    pbLedSpeed13.Visible = true;
                }
                if (digit_3 >= 4)
                {
                    pbSteeringWheel.Controls.Add(pbLedSpeed14);
                    pbLedSpeed14.Location = new Point(383, 314);
                    pbLedSpeed14.BackColor = Color.Transparent;
                    pbLedSpeed14.Visible = true;
                }
                if (digit_3 >= 5)
                {
                    pbSteeringWheel.Controls.Add(pbLedSpeed15);
                    pbLedSpeed15.Location = new Point(390, 326);
                    pbLedSpeed15.BackColor = Color.Transparent;
                    pbLedSpeed15.Visible = true;
                }
                if (digit_3 >= 6)
                {
                    pbSteeringWheel.Controls.Add(pbLedSpeed16);
                    pbLedSpeed16.Location = new Point(395, 338);
                    pbLedSpeed16.BackColor = Color.Transparent;
                    pbLedSpeed16.Visible = true;
                }
            }
            if (digit_4 == 0)
            {
                tbSpeed.Text = digit_3.ToString() + digit_2.ToString() + "," + digit_1.ToString() + " km/h";
            }
            else
            {
                tbSpeed.Text = digit_4.ToString() + digit_3.ToString() + digit_2.ToString() + "," + digit_1.ToString() + " km/h";
            }
        }

        void Unvisible()
        {
            pbSpeedDigit1_0.Visible = false;
            pbSpeedDigit1_1.Visible = false;
            pbSpeedDigit1_2.Visible = false;
            pbSpeedDigit1_3.Visible = false;
            pbSpeedDigit1_4.Visible = false;
            pbSpeedDigit1_5.Visible = false;
            pbSpeedDigit1_6.Visible = false;
            pbSpeedDigit1_7.Visible = false;
            pbSpeedDigit1_8.Visible = false;
            pbSpeedDigit1_9.Visible = false;
            pbSpeedDigit2_0.Visible = false;
            pbSpeedDigit2_1.Visible = false;
            pbSpeedDigit2_2.Visible = false;
            pbSpeedDigit2_3.Visible = false;
            pbSpeedDigit2_4.Visible = false;
            pbSpeedDigit2_5.Visible = false;
            pbSpeedDigit2_6.Visible = false;
            pbSpeedDigit2_7.Visible = false;
            pbSpeedDigit2_8.Visible = false;
            pbSpeedDigit2_9.Visible = false;
            pbSpeedDigit3_0.Visible = false;
            pbSpeedDigit3_1.Visible = false;
            pbSpeedDigit3_2.Visible = false;
            pbSpeedDigit3_3.Visible = false;
            pbSpeedDigit3_4.Visible = false;
            pbSpeedDigit3_5.Visible = false;
            pbSpeedDigit3_6.Visible = false;
            pbSpeedDigit3_7.Visible = false;
            pbSpeedDigit3_8.Visible = false;
            pbSpeedDigit3_9.Visible = false;
            pbSpeedDigit4_1.Visible = false;
            pbLedSpeed1.Visible = false;
            pbLedSpeed2.Visible = false;
            pbLedSpeed3.Visible = false;
            pbLedSpeed4.Visible = false;
            pbLedSpeed5.Visible = false;
            pbLedSpeed6.Visible = false;
            pbLedSpeed7.Visible = false;
            pbLedSpeed8.Visible = false;
            pbLedSpeed9.Visible = false;
            pbLedSpeed10.Visible = false;
            pbLedSpeed11.Visible = false;
            pbLedSpeed12.Visible = false;
            pbLedSpeed13.Visible = false;
            pbLedSpeed14.Visible = false;
            pbLedSpeed15.Visible = false;
            pbLedSpeed16.Visible = false;
            pbLedRPM1.Visible = false;
            pbLedRPM2.Visible = false;
            pbLedRPM3.Visible = false;
            pbLedRPM4.Visible = false;
            pbLedRPM5.Visible = false;
            pbLedRPM6.Visible = false;
            pbLedRPM7.Visible = false;
            pbLedRPM8.Visible = false;
            pbBarBrake1.Visible = false;
            pbBarBrake2.Visible = false;
            pbBarBrake3.Visible = false;
            pbBarBrake4.Visible = false;
            pbBarBrake7.Visible = false;
            pbBarBrake8.Visible = false;
            pbBarBrake5.Visible = false;
            pbBarBrake6.Visible = false;
            pbBarAccelerator1.Visible = false;
            pbBarAccelerator2.Visible = false;
            pbBarAccelerator3.Visible = false;
            pbBarAccelerator4.Visible = false;
            pbBarAccelerator7.Visible = false;
            pbBarAccelerator8.Visible = false;
            pbBarAccelerator5.Visible = false;
            pbBarAccelerator6.Visible = false;
            pbGearDigit0.Visible = false;
            pbGearDigit1.Visible = false;
            pbGearDigit2.Visible = false;
            pbGearDigit3.Visible = false;
            pbGearDigit4.Visible = false;
            pbLedBrake.Visible = false;
            pbLedGasLevel.Visible = false;
            pbLedOilLevel.Visible = false;
            pbLedPowerGood.Visible = false;
            pbLedDeadPoint.Visible = false;
        }

        void RPM(string rpm)
        {

            int digit = Convert.ToInt32(rpm)/1000;
            if (digit >= 1)
            {
                pbSteeringWheel.Controls.Add(pbLedRPM1);
                pbLedRPM1.Location = new Point(273, 419);
                pbLedRPM1.BackColor = Color.Transparent;
                pbLedRPM1.Visible = true;
            }
            if (digit >= 2)
            {
                pbSteeringWheel.Controls.Add(pbLedRPM2);
                pbLedRPM2.Location = new Point(286, 419);
                pbLedRPM2.BackColor = Color.Transparent;
                pbLedRPM2.Visible = true;
            }
            if (digit >= 3)
            {
                pbSteeringWheel.Controls.Add(pbLedRPM3);
                pbLedRPM3.Location = new Point(298, 419);
                pbLedRPM3.BackColor = Color.Transparent;
                pbLedRPM3.Visible = true;
            }
            if (digit >= 4)
            {
                pbSteeringWheel.Controls.Add(pbLedRPM4);
                pbLedRPM4.Location = new Point(311, 419);
                pbLedRPM4.BackColor = Color.Transparent;
                pbLedRPM4.Visible = true;
            }
            if (digit >= 5)
            {
                pbSteeringWheel.Controls.Add(pbLedRPM5);
                pbLedRPM5.Location = new Point(324, 419);
                pbLedRPM5.BackColor = Color.Transparent;
                pbLedRPM5.Visible = true;
            }
            if (digit >= 6)
            {
                pbSteeringWheel.Controls.Add(pbLedRPM6);
                pbLedRPM6.Location = new Point(337, 419);
                pbLedRPM6.BackColor = Color.Transparent;
                pbLedRPM6.Visible = true;
            }
            if (digit >= 7)
            {
                pbSteeringWheel.Controls.Add(pbLedRPM7);
                pbLedRPM7.Location = new Point(351, 419);
                pbLedRPM7.BackColor = Color.Transparent;
                pbLedRPM7.Visible = true;
            }
            if (digit >= 8)
            {
                pbSteeringWheel.Controls.Add(pbLedRPM8);
                pbLedRPM8.Location = new Point(364, 419);
                pbLedRPM8.BackColor = Color.Transparent;
                pbLedRPM8.Visible = true;
            }

        }

        void Gear(string gear)
        {
            int digit = Convert.ToInt32(gear);

            if (digit >= 0)
            {
                pbSteeringWheel.Controls.Add(pbGearDigit0);
                pbGearDigit0.Location = new Point(307, 473);
                pbGearDigit0.BackColor = Color.Transparent;
                pbGearDigit0.Visible = true;
            }
            if (digit >= 1)
            {
                pbSteeringWheel.Controls.Add(pbGearDigit1);
                pbGearDigit1.Location = new Point(307, 473);
                pbGearDigit1.BackColor = Color.Transparent;
                pbGearDigit1.Visible = true;
            }
            if (digit >= 2)
            {
                pbSteeringWheel.Controls.Add(pbGearDigit2);
                pbGearDigit2.Location = new Point(307, 473);
                pbGearDigit2.BackColor = Color.Transparent;
                pbGearDigit2.Visible = true;
            }
            if (digit >= 3)
            {
                pbSteeringWheel.Controls.Add(pbGearDigit3);
                pbGearDigit3.Location = new Point(307, 473);
                pbGearDigit3.BackColor = Color.Transparent;
                pbGearDigit3.Visible = true;
            }
            if (digit >= 4)
            {
                pbSteeringWheel.Controls.Add(pbGearDigit4);
                pbGearDigit4.Location = new Point(307, 473);
                pbGearDigit4.BackColor = Color.Transparent;
                pbGearDigit4.Visible = true;
            }
        }

        int adjust_curse_x = 80, adjust_curse_y = 10;
        void Curse_Brake(string brakeposition)
        {

            int digit = Convert.ToInt32(Convert.ToDouble(brakeposition) /12.5);
            int brake_position = Convert.ToInt32(brakeposition)*10;
            tbBrake.Text = (brake_position / 10).ToString() + "," + (brake_position - (brake_position / 10) * 10).ToString() + "%";

            if (digit >= 1)
            {
                pbBarBrake1.Location = new Point(485 + adjust_curse_x, 918-29*1 + adjust_curse_y);
                pbBarBrake1.BackColor = Color.Transparent;
                pbBarBrake1.Visible = true;
            }
            if (digit >= 2)
            {
                pbBarBrake2.Location = new Point(485 + adjust_curse_x, (918 - 29 * 2) + adjust_curse_y);
                pbBarBrake2.BackColor = Color.Transparent;
                pbBarBrake2.Visible = true;
            }
            if (digit >= 3)
            {
                pbBarBrake3.Location = new Point(485 + adjust_curse_x, (918 - 29 * 3) + adjust_curse_y);
                pbBarBrake3.BackColor = Color.Transparent;
                pbBarBrake3.Visible = true;
            }
            if (digit >= 4)
            {
                pbBarBrake4.Location = new Point(485 + adjust_curse_x, (918 - 29 * 4) + adjust_curse_y);
                pbBarBrake4.BackColor = Color.Transparent;
                pbBarBrake4.Visible = true;
            }
            if (digit >= 5)
            {
                pbBarBrake5.Location = new Point(485 + adjust_curse_x, (918 - 29 * 5) + adjust_curse_y);
                pbBarBrake5.BackColor = Color.Transparent;
                pbBarBrake5.Visible = true;
            }
            if (digit >= 6)
            {
                pbBarBrake6.Location = new Point(485 + adjust_curse_x, (918 - 29 * 6) + adjust_curse_y);
                pbBarBrake6.BackColor = Color.Transparent;
                pbBarBrake6.Visible = true;
            }
            if (digit >= 7)
            {
                pbBarBrake7.Location = new Point(485 + adjust_curse_x, (918 - 29 * 7) + adjust_curse_y);
                pbBarBrake7.BackColor = Color.Transparent;
                pbBarBrake7.Visible = true;
            }
            if (digit >= 8)
            {
                pbBarBrake8.Location = new Point(485 + adjust_curse_x, (918 - 29 * 8) + adjust_curse_y);
                pbBarBrake8.BackColor = Color.Transparent;
                pbBarBrake8.Visible = true;
            }

        }

        void Curse_Accelerator(string acelleratorposition)
        {
            int digit = Convert.ToInt32(Convert.ToDouble(acelleratorposition) / 12.5);
            int acellerator_position = Convert.ToInt32(acelleratorposition)*10;
            tbAccelerator.Text = (acellerator_position / 10).ToString() + "," + (acellerator_position - (acellerator_position / 10) * 10).ToString() + "%";

            if (digit >= 1)
            {
                pbBarAccelerator1.Location = new Point(753 + adjust_curse_x, 918 - 29 * 1 + adjust_curse_y);
                pbBarAccelerator1.BackColor = Color.Transparent;
                pbBarAccelerator1.Visible = true;
            }
            if (digit >= 2)
            {
                pbBarAccelerator2.Location = new Point(753 + adjust_curse_x, (918 - 29 * 2) + adjust_curse_y);
                pbBarAccelerator2.BackColor = Color.Transparent;
                pbBarAccelerator2.Visible = true;
            }
            if (digit >= 3)
            {
                pbBarAccelerator3.Location = new Point(753 + adjust_curse_x, (918 - 29 * 3) + adjust_curse_y);
                pbBarAccelerator3.BackColor = Color.Transparent;
                pbBarAccelerator3.Visible = true;
            }
            if (digit >= 4)
            {
                pbBarAccelerator4.Location = new Point(753 + adjust_curse_x, (918 - 29 * 4) + adjust_curse_y);
                pbBarAccelerator4.BackColor = Color.Transparent;
                pbBarAccelerator4.Visible = true;
            }
            if (digit >= 5)
            {
                pbBarAccelerator5.Location = new Point(753 + adjust_curse_x, (918 - 29 * 5) + adjust_curse_y);
                pbBarAccelerator5.BackColor = Color.Transparent;
                pbBarAccelerator5.Visible = true;
            }
            if (digit >= 6)
            {
                pbBarAccelerator6.Location = new Point(753 + adjust_curse_x, (918 - 29 * 6) + adjust_curse_y);
                pbBarAccelerator6.BackColor = Color.Transparent;
                pbBarAccelerator6.Visible = true;
            }
            if (digit >= 7)
            {
                pbBarAccelerator7.Location = new Point(753 + adjust_curse_x, (918 - 29 * 7) + adjust_curse_y);
                pbBarAccelerator7.BackColor = Color.Transparent;
                pbBarAccelerator7.Visible = true;
            }
            if (digit >= 8)
            {
                pbBarAccelerator8.Location = new Point(753 + adjust_curse_x, (918 - 29 * 8) + adjust_curse_y);
                pbBarAccelerator8.BackColor = Color.Transparent;
                pbBarAccelerator8.Visible = true;
            }

        }

        private void fCockpick_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you really want to go? \n", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                // you don't need that, it's already closing
                bClosePort_Click(sender, e);
            }
            else
            {
                // to don't close form is user change his mind
                e.Cancel = true;
            }
            bClosePort_Click(sender, e);
        }
        
        void getAvailablePort()
        {
            String[] ports = SerialPort.GetPortNames();
            cbPortNames.Items.AddRange(ports);
            cbBaudRate.SelectedIndex = 8;
        }

        private void bOpenPort_Click(object sender, EventArgs e)
        {
            if (store.GetReaded() == true)
            {
                store.Dispose();
                store = new fDataBank();
                store.Show();
                store.Visibility();
                store.Visibility();
                store.Visibility();
                store.SetReaded();
            }
            try
            {
                if (cbPortNames.Text == "")
                {
                    lbInformer.Text = "Please select the port.";
                }
                else
                {
                    mySerialPort.PortName = cbPortNames.Text;
                    mySerialPort.BaudRate = Convert.ToInt32(cbBaudRate.Text);
                    mySerialPort.Open();
                    if (mySerialPort.IsOpen)
                    {
                        bSendTest.Enabled = true;
                        tbSend.Enabled = true;
                        pbStatusOn.Visible = true;
                        pbStatusOff.Visible = false;
                        bOpenPort.Enabled = false;
                        pnSerial.Visible = true;
                        tmrCallBgWorker.Start();
                        bStopAcquire.Visible = true;
                    }
                    lbInformer.Text = "";
                    tbTimeofSamples.Enabled = true;
                    pnSerial.Visible = false;
                    pnParameters.Visible = false;
                    pnTime.Visible = false;
                    id = 0;
                    store.btnCriarDataTable_Click(sender, e);
                    store.FirstTime();
                    store.CanScroll(false);
                    store.ZeroA();
                    store.SetReaded();

                }
            }
            catch (UnauthorizedAccessException)
            {
                lbInformer.Text = "Unauthorized Access";
            }
        }

        private void bClosePort_Click(object sender, EventArgs e)
        {
            mySerialPort.Close();
            tbSend.Enabled = false;
            bOpenPort.Enabled = true;
            bClosePort.Enabled = false;
            pbStatusOn.Visible = false;
            pbStatusOff.Visible = true;
            bgWorker.CancelAsync();
            bgWorker.Dispose();
            tmrCallBgWorker.Dispose();
            store.CanScroll(true);
        }

        private void bOpenHideSerialComunication_Click(object sender, EventArgs e)
        {
            if (pnSerial.Visible == false)
            {
                pnSerial.Location = new Point(12, 61);
                pnSerial.Visible = true;
                pnParameters.Visible = true;
                pnTime.Visible = true;
                bStopAcquire.Visible = false;
            }
            else
            {
                pnSerial.Visible = false;
                pnParameters.Visible = false;
                pnTime.Visible = false;
                bStopAcquire.Visible = true;
            }
        }

        private void bSendTest_Click(object sender, EventArgs e)
        {
            mySerialPort.WriteLine(tbSend.Text);
            tbSend.Text = "";

            try
            {
                tbDataAcquire.Text = mySerialPort.ReadLine();
            }
            catch (IOException)
            {
                lbInformer.Text = "Timout Exception";
            }
        }

        private void tbDataAcquire_Changed(object sender, EventArgs e)
        {
            char[] digits = tbDataAcquire.Text.ToCharArray();
            int i = 0;
            while (i < 4)
            {   if (i == 0)
                {
                    tbNAirTemperature.Text = digits[1 + i].ToString();
                    tbNOilTemperature.Text = digits[1 + 1 * 5 + i].ToString();
                    tbNGear.Text = digits[1 + 2 * 5 + i].ToString();
                    tbNSpeed.Text = digits[1 + 3 * 5 + i].ToString();
                    tbNRPM.Text = digits[1 + 4 * 5 + i].ToString();
                    tbNBrakePosition.Text = digits[1 + 5 * 5 + i].ToString();
                    tbNAcceleratorPosition.Text = digits[1 + 6 * 5 + i].ToString();
                    tbNSteeringWheelAngle.Text = digits[1 + 7 * 5 + i].ToString();
                    tbNBrakeLight.Text = digits[1 + 8 * 5 + i].ToString();
                    tbNDeadPoint.Text = digits[1 + 9 * 5 + i].ToString();
                    tbNOilLevel.Text = digits[1 + 10 * 5 + i].ToString();
                    tbNGasLevel.Text = digits[1 + 11 * 5 + i].ToString();
                    tbNPowerGood.Text = digits[1 + 12 * 5 + i].ToString();
                }
                else
                {
                    tbNAirTemperature.Text += digits[1 + i];
                    tbNOilTemperature.Text += digits[1 + 1 * 5 + i];
                    tbNGear.Text += digits[1 + 2 * 5 + i];
                    tbNSpeed.Text += digits[1 + 3 * 5 + i];
                    tbNRPM.Text += digits[1 + 4 * 5 + i];
                    tbNBrakePosition.Text += digits[1 + 5 * 5 + i];
                    tbNAcceleratorPosition.Text += digits[1 + 6 * 5 + i];
                    tbNSteeringWheelAngle.Text += digits[1 + 7 * 5 + i];
                    tbNBrakeLight.Text += digits[1 + 8 * 5 + i];
                    tbNDeadPoint.Text += digits[1 + 9 * 5 + i];
                    tbNOilLevel.Text += digits[1 + 10 * 5 + i];
                    tbNGasLevel.Text += digits[1 + 11 * 5 + i];
                    tbNPowerGood.Text += digits[1 + 12 * 5 + i];
                }
                i++;
            }

            int varAux;
            if (tbNSteeringWheelAngle.Text[1] == '-')
            {
                varAux = ((tbNSteeringWheelAngle.Text[3] - 48) + (tbNSteeringWheelAngle.Text[2]-48) * 10) * (-1);
            }
            else
            {
                varAux = (tbNSteeringWheelAngle.Text[3] - 48) + (tbNSteeringWheelAngle.Text[2] - 48) * 10;
            }

            store.AddRow(id++, tbNAirTemperature.Text, tbNOilTemperature.Text, tbNGear.Text, tbNSpeed.Text, tbNRPM.Text, tbNBrakePosition.Text, tbNAcceleratorPosition.Text, varAux, tbNBrakeLight.Text, tbNDeadPoint.Text, tbNOilLevel.Text, tbNGasLevel.Text, tbNPowerGood.Text);
            store.GSUnvisible();
            store.LoadGraph(sender, e);
            
            Rotation(varAux);

            Unvisible();
            Speed(tbNSpeed.Text[3]-48, tbNSpeed.Text[2] - 48, tbNSpeed.Text[1]-48, tbNSpeed.Text[0]-48);
            Gear(tbNGear.Text);
            RPM(tbNRPM.Text);
            Curse_Brake(tbNBrakePosition.Text);
            Curse_Accelerator(tbNAcceleratorPosition.Text);
            OilTemp(tbNOilTemperature.Text);
            AirTemp(tbNAirTemperature.Text);
            Brake(tbNBrakeLight.Text);
            DeadPoint(tbNDeadPoint.Text);
            LevelOil(tbNOilLevel.Text);
            LevelGas(tbNGasLevel.Text);
            PowerGood(tbNPowerGood.Text);
            if (id == 5)
            {
                store.Visibility();
            }
        }
        private void tbNumberofSamples_TextChanged(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(Convert.ToInt32(tbTimeofSamples.Text));
        }

        bool stopped = false;
        private void bStopAcquire_Click(object sender, EventArgs e)
        {
            if (stopped == true)
            {
                bOpenPort_Click(sender, e);
                stopped = false;
                bStopAcquire.Text = "Stop";
                store.WhatGraph(stopped);
                store.Update();
            }
            else
            {
                if (store.GetRestart())
                {
                    bClosePort_Click(sender, e);
                    bOpenPort_Click(sender, e);

                }
                else
                {
                    bClosePort_Click(sender, e);
                    stopped = true;
                    bStopAcquire.Text = "Start";
                    store.WhatGraph(stopped);
                }
            }
        }

        private void bGraph_Click(object sender, EventArgs e)
        {
            Screen myScreen = Screen.PrimaryScreen;
            if (store.Visible != true)
            {
                store.Show();
            }

            if (store.Location.X == myScreen.WorkingArea.Width)
            {
                bGraph.Text =  "Graph at 2º Monitor";
                store.Location = new Point((myScreen.WorkingArea.Width-store.Size.Width)/2,0);
            }
            else
            {
                bGraph.Text = "Graph at 1º Monitor";
                store.Location = new Point(myScreen.WorkingArea.Width, 0);
            }

            if (store.Visible != true)
            {
                store.Show();
            }
        }

        private void tbTimeofSamples_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.STimeOfSamples = Convert.ToInt32(tbTimeofSamples.Text);
            Properties.Settings.Default.Save();
            tbTimeofSamples.Text = Properties.Settings.Default.STimeOfSamples.ToString();

        }

        private void bStopSaveRefresh_Click(object sender, EventArgs e)
        {
            if (store.GetSaveRefresh())
            {
                store.SetSaveRefresh();
            }
        }

        private void fCockpick_DoubleClick(object sender, EventArgs e)
        {
            if (tbSpeed.Visible == false)
            {
                tbSpeed.Visible = true;
                tbBrake.Visible = true;
                tbAccelerator.Visible = true;
                tbSteeringWheel.Visible = true;
            }
            else
            {
                tbSpeed.Visible = false;
                tbBrake.Visible = false;
                tbAccelerator.Visible = false;
                tbSteeringWheel.Visible = false;
            }
        }
    }
}


    

