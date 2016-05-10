using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Data;
using System.Windows.Forms;
using CreateFileOrFolder;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;



namespace Database
{
    public partial class fDataBank : Form
    {
        bool Readed = false;
        bool Restart = false;
        bool Stop = false;
        bool SaveRefresh = false;
        System.Data.DataTable dtb_String;
        Folder folder = new Folder();
        string firsttime;
        bool canscroll = false;

        public fDataBank()
        {
            InitializeComponent();
            folder.Create();
            InitializeCheckedBox();
        }

        public void btnCriarDataTable_Click(object sender, EventArgs e)
        {

            System.Data.DataTable tabela = CriarDataTable();
            dgvDados.DataSource = tabela;

        }

        void InitializeCheckedBox()
        {
            SaveRefresh = Datatable.Properties.Settings.Default.SSaveRefresh;
            Stop = Datatable.Properties.Settings.Default.SStop;
            tbTimerTick.Text = (Datatable.Properties.Settings.Default.STimerTick).ToString();
            chbIncludeData.Checked = Datatable.Properties.Settings.Default.SIncludeData;
            chbAllGraphs.Checked = Datatable.Properties.Settings.Default.SAllGraphs;
            chbAirTemp.Checked = Datatable.Properties.Settings.Default.SAirTemp;
            chbOilTemp.Checked = Datatable.Properties.Settings.Default.SOilTemp;
            chbGear.Checked = Datatable.Properties.Settings.Default.SGear;
            chbSpeed.Checked = Datatable.Properties.Settings.Default.SSpeed;
            chbRPM.Checked = Datatable.Properties.Settings.Default.SRPM;
            chbBrakePos.Checked = Datatable.Properties.Settings.Default.SBrakePos;
            chbAcceleratorPos.Checked = Datatable.Properties.Settings.Default.SAcceleratorPos;
            chbSteeringWheelAng.Checked = Datatable.Properties.Settings.Default.SSteeringWheelAng;
            chbBrakeLight.Checked = Datatable.Properties.Settings.Default.SBrakeLight;
            chbDeadPoint.Checked = Datatable.Properties.Settings.Default.SDeadPoint;
            chbOilLevel.Checked = Datatable.Properties.Settings.Default.SOilLevel;
            chbGasLevel.Checked = Datatable.Properties.Settings.Default.SGasLevel;
            chbPowerGood.Checked = Datatable.Properties.Settings.Default.SPowerGood;


        }

        private DataTable CriarDataTable()
        {

            dtb_String = new System.Data.DataTable();

            dtb_String.Columns.Add("Id", typeof(int));
            dtb_String.Columns.Add("Air Temperature", typeof(string));
            dtb_String.Columns.Add("Oil Temperature", typeof(string));
            dtb_String.Columns.Add("Gear", typeof(string));
            dtb_String.Columns.Add("Speed", typeof(string));
            dtb_String.Columns.Add("RPM", typeof(string));
            dtb_String.Columns.Add("Brake Position", typeof(string));
            dtb_String.Columns.Add("Accelerator Position", typeof(string));
            dtb_String.Columns.Add("Steering Wheel Angle", typeof(int));
            dtb_String.Columns.Add("Brake Light", typeof(string));
            dtb_String.Columns.Add("Dead Point", typeof(string));
            dtb_String.Columns.Add("Oil Level", typeof(string));
            dtb_String.Columns.Add("Gas Level", typeof(string));
            dtb_String.Columns.Add("Power Good", typeof(string));

            return dtb_String;
        }

        public DataTable AddRow(int id, string air_temp, string oil_temp, string gear, string speed, string rpm, string brake_pos, string accelerator_pos, int steering_ang, string brake_light, string dead, string oil_lvl, string gas_lvl, string power)
        {
            dtb_String.Rows.Add(id, air_temp, oil_temp, gear, speed, rpm, brake_pos, accelerator_pos, steering_ang, brake_light, dead, oil_lvl, gas_lvl, power);

            return dtb_String;
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            pnlShowScroll.Visible = false;
            string aux;
            try
            {
                if (tbNameTable.Text != "" && chbIncludeData.Checked == true)
                {
                    dtb_String.TableName = tbNameTable.Text;
                    aux = folder.folderName + tbNameTable.Text + " - " + DataTime() + ".xml";
                    dtb_String.WriteXml(aux);
                }
                else if (tbNameTable.Text != "" && chbIncludeData.Checked == false)
                {
                    dtb_String.TableName = tbNameTable.Text;
                    aux = folder.folderName + tbNameTable.Text + ".xml";
                    dtb_String.WriteXml(aux);
                }
                else
                {
                    dtb_String.TableName = DataTime();
                    aux = folder.folderName + DataTime() + ".xml";
                    dtb_String.WriteXml(aux);
                }
                pnlShowScroll.Visible = true;
                lShowScroll.Text = " XML File Was Generated With Sucess";
                if(!SaveRefresh)
                {
                    System.Diagnostics.Process.Start(aux);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro : " + ex.Message);
            }
        }

        private void btnList_DataTable_Click(object sender, EventArgs e)
        {
            // Examplo de lista
            List<string[]> oLista = new List<string[]>();
            oLista.Add(new string[] { "Brasil", "Peru", "Venezuela", "Colômbia" });
            oLista.Add(new string[] { "Argentina", "Paraguai", "Uruguai", "Jamaica" });
            oLista.Add(new string[] { "Chile", "Bolívia", "México", "Equador" });

            // Converte para DataTable
            System.Data.DataTable oDataTable = ConverteLista_DataTable(oLista);
            dgvDados.DataSource = oDataTable;
        }

        private DataTable ConverteLista_DataTable(List<string[]> oLista)
        {
            // Nova tabela
            System.Data.DataTable oDataTable = new System.Data.DataTable();

            // no maximo colunas
            int colunas = 0;
            foreach (var array in oLista)
            {
                if (array.Length > colunas)
                {
                    colunas = array.Length;
                }
            }

            // Adiciona colunas
            for (int i = 0; i < colunas; i++)
            {
                oDataTable.Columns.Add();
            }

            // Adiciona linhas
            foreach (var array in oLista)
            {
                oDataTable.Rows.Add(array);
            }

            return oDataTable;
        }

        string DataTime()
        {
            return "Day " +
                    DateTime.Now.ToShortDateString().ToString()[0] +
                    DateTime.Now.ToShortDateString().ToString()[1] + "-" +
                    DateTime.Now.ToShortDateString().ToString()[3] +
                    DateTime.Now.ToShortDateString().ToString()[4] + "-" +
                    DateTime.Now.ToShortDateString().ToString()[8] +
                    DateTime.Now.ToShortDateString().ToString()[9] +
                    firsttime +
                    " at " +
                    DateTime.Now.ToLongTimeString().ToString()[0] +
                    DateTime.Now.ToLongTimeString().ToString()[1] + "h " +
                    DateTime.Now.ToLongTimeString().ToString()[3] +
                    DateTime.Now.ToLongTimeString().ToString()[4] + "m " +
                    DateTime.Now.ToLongTimeString().ToString()[6] +
                    DateTime.Now.ToLongTimeString().ToString()[7] + "s";
        }

        public void FirstTime()
        {
            firsttime = " Time " +
                         DateTime.Now.ToLongTimeString().ToString()[0] +
                         DateTime.Now.ToLongTimeString().ToString()[1] + "h " +
                         DateTime.Now.ToLongTimeString().ToString()[3] +
                         DateTime.Now.ToLongTimeString().ToString()[4] + "m " +
                         DateTime.Now.ToLongTimeString().ToString()[6] +
                         DateTime.Now.ToLongTimeString().ToString()[7] + "s";
        }

        private void fDataBank_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void chbIncludeData_CheckStateChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SIncludeData = chbIncludeData.Checked;
            Datatable.Properties.Settings.Default.Save();
        }

        private void FillChart(int[] a, bool type, Chart graph, Color colour, int border)
        {
            // clear the chart
            graph.Series.Clear();

            // fill the chart
            var series = graph.Series.Add("My Serie");

            // if choiced charttype is differentiable...
            if(type == true)
            {
                series.ChartType = SeriesChartType.Spline;
            }
            else
            {
                series.ChartType = SeriesChartType.StepLine;
            }
            // more caracteristics of series
            series.BorderWidth = border;
            series.Color = colour;
            series.XValueType = ChartValueType.Int32;
            series.IsVisibleInLegend = false;
            for (int i = 0; i < a.Length; i++)
                series.Points.AddXY(i, a[i]);
            var chartArea = graph.ChartAreas[series.ChartArea];

            // set view range to [0,max]
            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Maximum = a.Length;

            // enable autoscroll
            chartArea.CursorX.AutoScroll = true;

            // let's zoom to [0,blockSize] (e.g. [0,100])
            chartArea.AxisX.ScaleView.Zoomable = true;
        }

        int a = 0;
        public void LoadGraph(object sender, EventArgs e)
        {
            UnCheckedGraphs(sender, e);
            DataRow[] oDataRow = dtb_String.Select();
            foreach (DataRow dr in oDataRow)
            {
                if (chbAirTemp.Checked == true)
                {
                    if ((GAirTemp.ChartAreas[0].AxisX.Maximum - GAirTemp.ChartAreas[0].AxisX.Minimum) > -1000) { GAirTemp.ChartAreas[0].AxisX.Minimum++; }
                    double AirTemp = Convert.ToInt16(dr["Air Temperature"]);
                    this.GAirTemp.Series["My Serie"].Points.AddY(AirTemp / 10);
                }

                if (chbOilTemp.Checked == true)
                {
                    if ((GOilTemp.ChartAreas[0].AxisX.Maximum - GOilTemp.ChartAreas[0].AxisX.Minimum) > -1000) { GOilTemp.ChartAreas[0].AxisX.Minimum++; }
                    double OilTemp = Convert.ToInt16(dr["Oil Temperature"]);
                    this.GOilTemp.Series["My Serie"].Points.AddY(OilTemp / 10);
                }

                if (chbGear.Checked == true)
                {
                    if ((GGear.ChartAreas[0].AxisX.Maximum - GGear.ChartAreas[0].AxisX.Minimum) > -1000) { GGear.ChartAreas[0].AxisX.Minimum++; }
                    double Gear = Convert.ToInt16(dr["Gear"]);
                    this.GGear.Series["My Serie"].Points.AddY(Gear);
                }

                if (chbSpeed.Checked == true)
                {
                    if ((GSpeed.ChartAreas[0].AxisX.Maximum - GSpeed.ChartAreas[0].AxisX.Minimum) > -1000) { GSpeed.ChartAreas[0].AxisX.Minimum++; }
                    double Speed = Convert.ToInt16(dr["Speed"]);
                    this.GSpeed.Series["My Serie"].Points.AddY(Speed/10);
                }

                if (chbRPM.Checked == true)
                {
                    if ((GRPM.ChartAreas[0].AxisX.Maximum - GRPM.ChartAreas[0].AxisX.Minimum) > -1000) { GRPM.ChartAreas[0].AxisX.Minimum++; }
                    double RPM = Convert.ToInt16(dr["RPM"]);
                    this.GRPM.Series["My Serie"].Points.AddY(RPM);
                }

                if (chbBrakePos.Checked == true)
                {
                    if ((GBrakePosition.ChartAreas[0].AxisX.Maximum - GBrakePosition.ChartAreas[0].AxisX.Minimum) > -1000) { GBrakePosition.ChartAreas[0].AxisX.Minimum++; }
                    double BrakePosition = Convert.ToInt16(dr["Brake Position"]) / 12.5;
                    this.GBrakePosition.Series["My Serie"].Points.AddY(BrakePosition);
                    textBox1.Text = BrakePosition.ToString();
                }

                if (chbAcceleratorPos.Checked == true)
                {
                    if ((GAcceleratorPosition.ChartAreas[0].AxisX.Maximum - GAcceleratorPosition.ChartAreas[0].AxisX.Minimum) > -1000) { GAcceleratorPosition.ChartAreas[0].AxisX.Minimum++; }
                    double AcceleratorPosition = Convert.ToInt16(dr["Accelerator Position"]) / 12.5;
                    this.GAcceleratorPosition.Series["My Serie"].Points.AddY(AcceleratorPosition);
                    textBox2.Text = AcceleratorPosition.ToString();
                }

                if (chbSteeringWheelAng.Checked == true)
                {
                    if ((GSteeringWheelAngle.ChartAreas[0].AxisX.Maximum - GSteeringWheelAngle.ChartAreas[0].AxisX.Minimum) > -1000) { GSteeringWheelAngle.ChartAreas[0].AxisX.Minimum++; }
                    double SteeringWheelAngle = Convert.ToDouble(dr["Steering Wheel Angle"]);
                    this.GSteeringWheelAngle.Series["My Serie"].Points.AddY(SteeringWheelAngle);
                }

                if (chbBrakeLight.Checked == true)
                {
                    if ((GBrakeLight.ChartAreas[0].AxisX.Maximum - GBrakeLight.ChartAreas[0].AxisX.Minimum) > -1000) { GBrakeLight.ChartAreas[0].AxisX.Minimum++; }
                    double BrakeLight = Convert.ToInt16(dr["Brake Light"]);
                    this.GBrakeLight.Series["My Serie"].Points.AddY(BrakeLight);
                }
            
                if (chbDeadPoint.Checked == true)
                {
                    if ((GDeadPoint.ChartAreas[0].AxisX.Maximum - GDeadPoint.ChartAreas[0].AxisX.Minimum) > -1000) { GDeadPoint.ChartAreas[0].AxisX.Minimum++; }
                    double DeadPoint = Convert.ToInt16(dr["Dead Point"]);
                    this.GDeadPoint.Series["My Serie"].Points.AddY(DeadPoint);
                }

                if (chbOilLevel.Checked == true)
                {
                    if ((GOilLevel.ChartAreas[0].AxisX.Maximum - GOilLevel.ChartAreas[0].AxisX.Minimum) > -1000) { GOilLevel.ChartAreas[0].AxisX.Minimum++; }
                    double OilLevel = Convert.ToInt16(dr["Oil Level"]);
                    this.GOilLevel.Series["My Serie"].Points.AddY(OilLevel);
                }

                if (chbGasLevel.Checked == true)
                {
                    if ((GGasLevel.ChartAreas[0].AxisX.Maximum - GGasLevel.ChartAreas[0].AxisX.Minimum) > -1000) { GGasLevel.ChartAreas[0].AxisX.Minimum++; }
                    double GasLevel = Convert.ToInt16(dr["Gas Level"]);
                    this.GGasLevel.Series["My Serie"].Points.AddY(GasLevel);
                }

                if (chbPowerGood.Checked == true)
                {
                    if ((GPowerGood.ChartAreas[0].AxisX.Maximum - GPowerGood.ChartAreas[0].AxisX.Minimum) > -1000) { GPowerGood.ChartAreas[0].AxisX.Minimum++; }
                    double PowerGood = Convert.ToInt16(dr["Power Good"]);
                    this.GPowerGood.Series["My Serie"].Points.AddY(PowerGood);
                }

                a++;
            }
        }

        private void cboCampo_Click(object sender, EventArgs e)
        {
            if (folder.ProcessFiles(folder.folderName).Length != 0)
            {
                cbListFiles.Items.Clear();
                cbListFiles.Items.AddRange(folder.ProcessFiles(folder.folderName));
                cbListFiles.SelectedIndex = 0;
            }
        }

        private void chbAirTemp_CheckStateChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SAirTemp = chbAirTemp.Checked;
            Datatable.Properties.Settings.Default.Save();
            if (chbAirTemp.Checked == true)
            {
                pnghAirTemp.Visible = true;
            }
            else
            {
                pnghAirTemp.Visible = false;
            }

        }

        private void chbOilTemp_CheckedChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SOilTemp = chbOilTemp.Checked;
            Datatable.Properties.Settings.Default.Save();
            if (chbOilTemp.Checked == true)
            {
                pnghOilTemp.Visible = true;
            }
            else
            {
                pnghOilTemp.Visible = false;
            }
        }

        private void chbGear_CheckedChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SGear = chbGear.Checked;
            Datatable.Properties.Settings.Default.Save();
            if (chbGear.Checked == true)
            {
                pnghGear.Visible = true;
            }
            else
            {
                pnghGear.Visible = false;
            }
        }

        private void chbSpeed_CheckedChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SSpeed = chbSpeed.Checked;
            Datatable.Properties.Settings.Default.Save();
            if (chbSpeed.Checked == true )
            {
                pnghSpeed.Visible = true;
            }
            else
            {
                pnghSpeed.Visible = false;
            }
        }

        private void chbRPM_CheckedChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SRPM = chbRPM.Checked;
            Datatable.Properties.Settings.Default.Save();
            if (chbRPM.Checked == true)
            {
                pnghRPM.Visible = true;
            }
            else
            {
                pnghRPM.Visible = false;
            }
        }

        private void chbBrakePos_CheckedChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SBrakePos = chbBrakePos.Checked;
            Datatable.Properties.Settings.Default.Save();
            if (chbBrakePos.Checked == true)
            {
                pnghBrakePosition.Visible = true;
            }
            else
            {
                pnghBrakePosition.Visible = false;
            }
        }

        private void chbAcceleratorPos_CheckedChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SAcceleratorPos = chbAcceleratorPos.Checked;
            Datatable.Properties.Settings.Default.Save();
            if (chbAcceleratorPos.Checked == true)
            {
                pnghAcceleratorPosition.Visible = true;
            }
            else
            {
                pnghAcceleratorPosition.Visible = false;
            }
        }

        private void chbSteeringWheelAng_CheckedChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SSteeringWheelAng = chbSteeringWheelAng.Checked;
            Datatable.Properties.Settings.Default.Save();
            if (chbSteeringWheelAng.Checked == true)
            {
                pnghSteeringWheelAngle.Visible = true;
            }
            else
            {
                pnghSteeringWheelAngle.Visible = false;
            }
        }

        private void chbBrakeLight_CheckedChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SBrakeLight = chbBrakeLight.Checked;
            Datatable.Properties.Settings.Default.Save();
            if (chbBrakeLight.Checked == true)
            {
                pnghBrakeLight.Visible = true;
            }
            else
            {
                pnghBrakeLight.Visible = false;
            }
        }

        private void chbDeadPoint_CheckedChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SDeadPoint = chbDeadPoint.Checked;
            Datatable.Properties.Settings.Default.Save();
            if (chbDeadPoint.Checked == true)
            {
                pnghDeadPoint.Visible = true;
            }
            else
            {
                pnghDeadPoint.Visible = false;
            }
        }

        private void chbOilLevel_CheckedChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SOilLevel = chbOilLevel.Checked;
            Datatable.Properties.Settings.Default.Save();
            if (chbOilLevel.Checked == true)
            {
                pnghOilLevel.Visible = true;
            }
            else
            {
                pnghOilLevel.Visible = false;
            }
        }

        private void chbGasLevel_CheckedChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SGasLevel = chbGasLevel.Checked;
            Datatable.Properties.Settings.Default.Save();
            if (chbGasLevel.Checked == true)
            {
                pnghGasLevel.Visible = true;
            }
            else
            {
                pnghGasLevel.Visible = false;
            }
        }

        private void chbPowerGood_CheckedChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SPowerGood = chbPowerGood.Checked;
            Datatable.Properties.Settings.Default.Save();
            if (chbPowerGood.Checked == true)
            {
                pnghPowerGood.Visible = true;
            }
            else
            {
                pnghPowerGood.Visible = false;
            }
        }

        void CheckAll(bool check)
        {
            chbAirTemp.Checked = check;
            chbOilTemp.Checked = check;
            chbGear.Checked = check;
            chbSpeed.Checked = check;
            chbRPM.Checked = check;
            chbBrakePos.Checked = check;
            chbAcceleratorPos.Checked = check;
            chbSteeringWheelAng.Checked = check;
            chbBrakeLight.Checked = check;
            chbDeadPoint.Checked = check;
            chbOilLevel.Checked = check;
            chbGasLevel.Checked = check;
            chbPowerGood.Checked = check;
        }

        public void bShowScroll_Click(object sender, EventArgs e)
        {
            if (canscroll == true && a > 0)
            {
                pnFill.AutoScroll = true;

                GSAirTemp.Visible = true;
                GSOilTemp.Visible = true;
                GSGear.Visible = true;
                GSSpeed.Visible = true;
                GSRPM.Visible = true;
                GSBrakePosition.Visible = true;
                GSAcceleratorPosition.Visible = true;
                GSSteeringWheelAngle.Visible = true;
                GSBrakeLight.Visible = true;
                GSDeadPoint.Visible = true;
                GSOilLevel.Visible = true;
                GSGasLevel.Visible = true;
                GSPowerGood.Visible = true;

                visio ^= true;

                FillChart(ScrollGraphs(10, "Air Temperature"),true , GSAirTemp, System.Drawing.Color.DeepSkyBlue, 3);
                FillChart(ScrollGraphs(10, "Oil Temperature"), true, GSOilTemp, System.Drawing.Color.Purple, 3);
                FillChart(ScrollGraphs(1, "Gear"), false, GSGear, System.Drawing.Color.LimeGreen, 3);
                FillChart(ScrollGraphs(10, "Speed"), true, GSSpeed, System.Drawing.Color.Red, 3);
                FillChart(ScrollGraphs(1, "RPM"), true, GSRPM, System.Drawing.Color.Tomato, 3);
                FillChart(ScrollGraphs(10, "Brake Position"), false, GSBrakePosition, System.Drawing.Color.Magenta,3);
                FillChart(ScrollGraphs(10, "Accelerator Position"), false, GSAcceleratorPosition, System.Drawing.Color.Aquamarine, 3);
                FillChart(ScrollGraphs(1, "Steering Wheel Angle"), false, GSSteeringWheelAngle, System.Drawing.Color.Gold, 3);
                FillChart(ScrollGraphs(1, "Brake Light"), false, GSBrakeLight, System.Drawing.Color.Crimson, 3);
                FillChart(ScrollGraphs(10, "Dead Point"), false, GSDeadPoint, System.Drawing.Color.DodgerBlue, 5);
                FillChart(ScrollGraphs(1, "Oil Level"), false, GSOilLevel, System.Drawing.Color.Violet, 5);
                FillChart(ScrollGraphs(10, "Gas Level"), false, GSGasLevel, System.Drawing.Color.DarkGoldenrod, 5);
                FillChart(ScrollGraphs(1, "Power Good"), false, GSPowerGood, System.Drawing.Color.GreenYellow, 5);

                sbGraphs.Visible = true;
                pnlShowScroll.Visible = false;
            }
            else
            {
                sbGraphs.Visible = false;
                pnlShowScroll.Visible = true;
                if (a > 0)
                {
                    Stop = true;
                }
                else
                {
                    lShowScroll.Text = "Please Start First";

                }
            }
        }

       int[] ScrollGraphs(int precision, string field)
        {
            //read_aquire will say if the data source is from file or from Xbee
            int count = 0;
            DataRow[] datarw;

            datarw = dtb_String.Select();
            count = Convert.ToInt16(dtb_String.Compute("Count(ID)", string.Empty));

            int[] numbers = new int[count];
            int i = 0;

            foreach (DataRow datagrd in datarw)
            {
                numbers[i] = Convert.ToInt16(datagrd[field]) / precision;
                i++;
            }

            return numbers;
        }

        bool visio = false;
        public void Visibility()
        {

            if (visio == true)
            {

                pnghAirTemp.Visible = true;
                pnghOilTemp.Visible = true;
                pnghGear.Visible = true;
                pnghSpeed.Visible = true;
                pnghRPM.Visible = true;
                pnghBrakePosition.Visible = true;
                pnghAcceleratorPosition.Visible = true;
                pnghSteeringWheelAngle.Visible = true;
                pnghBrakeLight.Visible = true;
                pnghDeadPoint.Visible = true;
                pnghOilLevel.Visible = true;
                pnghGasLevel.Visible = true;
                pnghPowerGood.Visible = true;


            }
            else
            {
                pnghAirTemp.Visible = false;
                pnghOilTemp.Visible = false;
                pnghGear.Visible = false;
                pnghSpeed.Visible = false;
                pnghRPM.Visible = false;
                pnghBrakePosition.Visible = false;
                pnghAcceleratorPosition.Visible = false;
                pnghSteeringWheelAngle.Visible = false;
                pnghBrakeLight.Visible = false;
                pnghDeadPoint.Visible = false;
                pnghOilLevel.Visible = false;
                pnghGasLevel.Visible = false;
                pnghPowerGood.Visible = false;
            }
            visio ^= true;
        }

        public void WhatGraph(bool isopen)
        {

            if (isopen == true)
            {
                GAirTemp.Visible = true;
                GOilTemp.Visible = true;
                GGear.Visible = true;
                GSpeed.Visible = true;
                GRPM.Visible = true;
                GBrakePosition.Visible = true;
                GAcceleratorPosition.Visible = true;
                GSteeringWheelAngle.Visible = true;
                GBrakeLight.Visible = true;
                GDeadPoint.Visible = true;
                GOilLevel.Visible = true;
                GGasLevel.Visible = true;
                GPowerGood.Visible = true;
            }
            else
            {
                GSUnvisible();
            }
        }

        public void GSUnvisible ()
        {
            GSAirTemp.Visible = false;
            GSOilTemp.Visible = false;
            GSGear.Visible = false;
            GSSpeed.Visible = false;
            GSRPM.Visible = false;
            GSBrakePosition.Visible = false;
            GSAcceleratorPosition.Visible = false;
            GSSteeringWheelAngle.Visible = false;
            GSBrakeLight.Visible = false;
            GSDeadPoint.Visible = false;
            GSOilLevel.Visible = false;
            GSGasLevel.Visible = false;
            GSPowerGood.Visible = false;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ScrolBar(GSAirTemp);
            ScrolBar(GSOilTemp);
            ScrolBar(GSGear);
            ScrolBar(GSSpeed);
            ScrolBar(GSRPM);
            ScrolBar(GSBrakePosition);
            ScrolBar(GSAcceleratorPosition);
            ScrolBar(GSSteeringWheelAngle);
            ScrolBar(GSBrakeLight);
            ScrolBar(GSDeadPoint);
            ScrolBar(GSOilLevel);
            ScrolBar(GSGasLevel);
            ScrolBar(GSPowerGood);
        }

        void ScrolBar(Chart graph)
        {
            var lala = graph.ChartAreas[graph.Series["My Serie"].ChartArea];
            int position = 0;
            int size = 100;
            lala.AxisX.ScaleView.Zoom(position, size);
            lala.AxisX.ScaleView.Position = sbGraphs.Value;
        }

        public void CanScroll(bool can)
        {
            canscroll = can;
            if (can == true)
            {
                sbGraphs.Enabled = true;
                pnlShowScroll.Visible = false;

            }
            else
            {
                sbGraphs.Enabled = false;
                pnlShowScroll.Visible = false;
            }
        }

        public void ZeroA()
        {
            a = 0;
        }

        private void bReadFile_Click(object sender, EventArgs e)
        {
            if (cbListFiles.Text == "")
            {
                pnlShowScroll.Visible = true;
                lShowScroll.Text = "Please Select the File";
            }
            else
            {
                try
                {
                    canscroll = true;
                    a = 1;
                    btnCriarDataTable_Click(sender, e);
                    DataSet ds = new DataSet();
                    ds.ReadXml(folder.folderName + cbListFiles.Text + ".xml");
                    dtb_String = ds.Tables[0];
                    dgvDados.DataSource = dtb_String;
                    bShowScroll_Click(sender, e);
                    canscroll = false;
                    a = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro : " + ex.Message);
                }
            }
            CheckAll(false);
            CheckAll(true);
            Readed = true;
        }

        public bool GetReaded()
        {
            return Readed;
        }

        public void SetReaded()
        {
             Readed = false;
             pnFill.AutoScroll = false;
        }

        private void chbAllGraphs_CheckStateChanged(object sender, EventArgs e)
        {
            
        }

        void UnCheckedGraphs(object sender, EventArgs e)
        {
            if (!chbAirTemp.Checked)
            {
                chbAirTemp_CheckStateChanged(sender, e);
            }
            if (!chbOilTemp.Checked)
            {
                chbOilTemp_CheckedChanged(sender, e);
            }

            if (!chbGear.Checked)
            {
                chbGear_CheckedChanged(sender, e);
            }

            if (!chbSpeed.Checked)
            {
                chbSpeed_CheckedChanged(sender, e);
            }

            if (!chbRPM.Checked)
            {
                chbRPM_CheckedChanged(sender, e);
            }

            if (!chbBrakePos.Checked)
            {
                chbBrakePos_CheckedChanged(sender, e);
            }

            if (!chbAcceleratorPos.Checked)
            {
                chbAcceleratorPos_CheckedChanged(sender, e);
            }

            if (!chbSteeringWheelAng.Checked)
            {
                chbSteeringWheelAng_CheckedChanged(sender, e);
            }

            if (!chbBrakeLight.Checked)
            {
                chbBrakeLight_CheckedChanged(sender, e);
            }

            if (!chbDeadPoint.Checked)
            {
                chbDeadPoint_CheckedChanged(sender, e);
            }

            if (!chbOilLevel.Checked)
            {
                chbOilLevel_CheckedChanged(sender, e);
            }

            if (!chbGasLevel.Checked)
            {
                chbGasLevel_CheckedChanged(sender, e);
            }

            if (!chbPowerGood.Checked)
            {
                chbPowerGood_CheckedChanged(sender, e);
            }



        }

        private void bUpdate_Click(object sender, EventArgs e)
        {
            Readed = true;
            Restart = true;
            if(!Stop)
            {
                pnlShowScroll.Visible = true;
                lShowScroll.Text = "Wait the Update";
            }

            Stop = true;
            Datatable.Properties.Settings.Default.SStop = Stop;
            Datatable.Properties.Settings.Default.Save();
        }

        private void chbAllGraphs_CheckedChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.SAllGraphs = chbAllGraphs.Checked;
            Datatable.Properties.Settings.Default.Save();
            CheckAll(chbAllGraphs.Checked);
        }

        public void bSaveRefresh_Click(object sender, EventArgs e)
        {
                SaveRefresh = true;
                Datatable.Properties.Settings.Default.SSaveRefresh = SaveRefresh;
                Datatable.Properties.Settings.Default.Save();
        }

        public int SetTimerTick()
        {
            return Datatable.Properties.Settings.Default.STimerTick;
        }

        public bool GetRestart()
        {
            return Restart;
        }

        public void SetRestart()
        {
            Restart = false;
        }

        public bool GetStop()
        {
            return Stop;
        }

        public void SetStop()
        {
            Stop = false;
        }

        public bool GetSaveRefresh()
        {
            return SaveRefresh;
        }

        public void SetSaveRefresh()
        {
            SaveRefresh = false;
            Datatable.Properties.Settings.Default.SSaveRefresh = SaveRefresh;
            Datatable.Properties.Settings.Default.Save();
        }

        

        private void tbTimerTick_TextChanged(object sender, EventArgs e)
        {
            Datatable.Properties.Settings.Default.STimerTick = Convert.ToInt16(tbTimerTick.Text);
            Datatable.Properties.Settings.Default.Save();
        }

        public void varSaveRefresh_Click(object sender, EventArgs e)
        {
            bSave_Click(sender, e);
            bUpdate_Click(sender, e);
        }

        private void fDataBank_Load(object sender, EventArgs e)
        {
            SendToBack();
        }
    }
}