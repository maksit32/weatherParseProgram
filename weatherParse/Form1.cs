using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;


namespace weatherParse
{
    public partial class Form1 : Form
    {
        //��� ICAO ��������� -- ���������� �����������
        string ICAOCode = "UUEE";
        public Form1()
        {
            InitializeComponent();
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.textBox1.Text = "UUEE";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            CheckWeather(ICAOCode);
            timer1.Start();
        }

        //���� ������� �������� ������ + ��������
        private async void CheckWeather(string ICAOCode)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    //��������� ����
                    string website = "https://ru.allmetsat.com/metar-taf/russia.php?icao=" + ICAOCode;
                    client.DefaultRequestHeaders.Add("user-agent", "Mozilla / 5.0 (Windows NT 10.0; Win64; x64) AppleWebKit / 537.36 (KHTML, like Gecko) Chrome / 112.0.0.0 Safari / 537.36");
                    HttpResponseMessage response = await client.GetAsync(website);
                    response.EnsureSuccessStatusCode();
                    ////��������� ��� � 1 ������
                    string body = await response.Content.ReadAsStringAsync();
                    //������ ������ ������
                    string[] tmp = body.Split("<div class=\"mt\">");
                    string temperatureString = "";
                    string pressureString = "";
                    string weatherString = "";
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        //��� ����������� � �������� ������������ ����
                        if (tmp[i].Contains("�����������"))
                        {
                            temperatureString = tmp[i];
                            continue;
                        }
                        if (tmp[i].Contains("��������"))
                        {
                            pressureString = tmp[i];
                            continue;
                        }
                        //������ ������ //������ //����� ��������
                        if (tmp[i].Contains("�����"))
                        {
                            weatherString = tmp[i];
                            this.pictureBox1.ImageLocation = "..\\..\\..\\images\\storm.jpg";
                            this.statusWeatherLabel.Text = "������ ������:  " + "�����";
                            break;
                        }
                        if (tmp[i].Contains("�����") || tmp[i].Contains("��������� �������� �����"))
                        {
                            weatherString = tmp[i];
                            this.pictureBox1.ImageLocation = "..\\..\\..\\images\\rainy.jpg";
                            this.statusWeatherLabel.Text = "������ ������:  " + "��������� �������� �����";
                            break;
                        }
                        if (tmp[i].Contains("����"))
                        {
                            weatherString = tmp[i];
                            this.pictureBox1.ImageLocation = "..\\..\\..\\images\\snowy.jpg";
                            this.statusWeatherLabel.Text = "������ ������:  " + "����";
                            break;
                        }
                        if (tmp[i].Contains("�������� ����������"))
                        {
                            weatherString = tmp[i];
                            this.pictureBox1.ImageLocation = "..\\..\\..\\images\\cloudy.jpg";
                            this.statusWeatherLabel.Text = "������ ������:  " + "�������� ����������";
                            break;
                        }
                        if (tmp[i].Contains("��������� ������"))
                        {
                            weatherString = tmp[i];
                            this.pictureBox1.ImageLocation = "..\\..\\..\\images\\mainlycloudy.jpg";
                            this.statusWeatherLabel.Text = "������ ������:  " + "��������� ������";
                            break;
                        }
                        if (tmp[i].Contains("��� ����������") || tmp[i].Contains("��� ������-�������� ����������"))
                        {
                            weatherString = tmp[i];
                            this.pictureBox1.ImageLocation = "..\\..\\..\\images\\sunny.jpg";
                            this.statusWeatherLabel.Text = "������ ������:  " + "��� ����������";
                            break;
                        }
                    }
                    //����� ���� ��� ������ ������ ���������� ��������
                    //������ �������� �� ������(������)
                    temperatureString = temperatureString.Replace("����������� <b>", "");
                    temperatureString = temperatureString.Replace("</b>�C</div>", "");
                    this.tempLabel.Text = "����������� (�C):  " + temperatureString;


                    pressureString = pressureString.Replace("�������� <b>", "");
                    pressureString = pressureString.Replace("</b> hPa</div>", "");
                    this.QNHLabel.Text = "��������, ������������ ������ ���� (QNH):  " + pressureString;


                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show("������ ��������� ������ � �����!");
                }
            }
        }
        //������
        private void timer1_Tick(object sender, EventArgs e)
        {
            //��������� ������ 5 ����� ������
            CheckWeather(ICAOCode);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //�������� ����� (��� ����������)
            if (!((e.KeyChar >= 'A' && e.KeyChar <= 'Z') || e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Enter))
            {
                e.Handled = true;
                MessageBox.Show("������� ������ ��������� ���������� �����!");
            }
            //����� enter
            if(e.KeyChar == (char)Keys.Enter)
            {
                this.ICAOCode = this.textBox1.Text;
                CheckWeather(ICAOCode);
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            this.ICAOCode = this.textBox1.Text;
            CheckWeather(ICAOCode);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //��� ��������� (������ ������ ���������)
            this.textBox1.Text = this.comboBox1.Text;
            this.ICAOCode = this.textBox1.Text;
            //��������� �������� �������������!
            CheckWeather(ICAOCode);
        }
    }
}