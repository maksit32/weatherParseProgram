using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;


namespace weatherParse
{
    public partial class Form1 : Form
    {
        //код ICAO аэропорта -- изначально Шереметьево
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

        //сама функция проверки погоды + парсинга
        private async void CheckWeather(string ICAOCode)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    //скачиваем сайт
                    string website = "https://ru.allmetsat.com/metar-taf/russia.php?icao=" + ICAOCode;
                    client.DefaultRequestHeaders.Add("user-agent", "Mozilla / 5.0 (Windows NT 10.0; Win64; x64) AppleWebKit / 537.36 (KHTML, like Gecko) Chrome / 112.0.0.0 Safari / 537.36");
                    HttpResponseMessage response = await client.GetAsync(website);
                    response.EnsureSuccessStatusCode();
                    ////заполняем все в 1 строку
                    string body = await response.Content.ReadAsStringAsync();
                    //парсим нужные данные
                    string[] tmp = body.Split("<div class=\"mt\">");
                    string temperatureString = "";
                    string pressureString = "";
                    string weatherString = "";
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        //ищу температуру и давление относительно моря
                        if (tmp[i].Contains("Температура"))
                        {
                            temperatureString = tmp[i];
                            continue;
                        }
                        if (tmp[i].Contains("Давление"))
                        {
                            pressureString = tmp[i];
                            continue;
                        }
                        //смотрю погоду //облака //меняю картинки
                        if (tmp[i].Contains("гроза"))
                        {
                            weatherString = tmp[i];
                            this.pictureBox1.ImageLocation = "..\\..\\..\\images\\storm.jpg";
                            this.statusWeatherLabel.Text = "Статус погоды:  " + "гроза";
                            break;
                        }
                        if (tmp[i].Contains("дождь") || tmp[i].Contains("небольшие ливневые дожди"))
                        {
                            weatherString = tmp[i];
                            this.pictureBox1.ImageLocation = "..\\..\\..\\images\\rainy.jpg";
                            this.statusWeatherLabel.Text = "Статус погоды:  " + "небольшие ливневые дожди";
                            break;
                        }
                        if (tmp[i].Contains("снег"))
                        {
                            weatherString = tmp[i];
                            this.pictureBox1.ImageLocation = "..\\..\\..\\images\\snowy.jpg";
                            this.statusWeatherLabel.Text = "Статус погоды:  " + "снег";
                            break;
                        }
                        if (tmp[i].Contains("Сплошная облачность"))
                        {
                            weatherString = tmp[i];
                            this.pictureBox1.ImageLocation = "..\\..\\..\\images\\cloudy.jpg";
                            this.statusWeatherLabel.Text = "Статус погоды:  " + "сплошная облачность";
                            break;
                        }
                        if (tmp[i].Contains("Отдельные облака"))
                        {
                            weatherString = tmp[i];
                            this.pictureBox1.ImageLocation = "..\\..\\..\\images\\mainlycloudy.jpg";
                            this.statusWeatherLabel.Text = "Статус погоды:  " + "отдельные облака";
                            break;
                        }
                        if (tmp[i].Contains("Нет облачности") || tmp[i].Contains("нет кучево-дождевой облачности"))
                        {
                            weatherString = tmp[i];
                            this.pictureBox1.ImageLocation = "..\\..\\..\\images\\sunny.jpg";
                            this.statusWeatherLabel.Text = "Статус погоды:  " + "нет облачности";
                            break;
                        }
                    }
                    //после того как забрал строку вытаскиваю значения
                    //удаляю ненужное из строки(замена)
                    temperatureString = temperatureString.Replace("Температура <b>", "");
                    temperatureString = temperatureString.Replace("</b>°C</div>", "");
                    this.tempLabel.Text = "Температура (°C):  " + temperatureString;


                    pressureString = pressureString.Replace("Давление <b>", "");
                    pressureString = pressureString.Replace("</b> hPa</div>", "");
                    this.QNHLabel.Text = "Давление, относительно уровня моря (QNH):  " + pressureString;


                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show("Ошибка получения данных с сайта!");
                }
            }
        }
        //таймер
        private void timer1_Tick(object sender, EventArgs e)
        {
            //обновляет каждые 5 минут погоду
            CheckWeather(ICAOCode);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //проверка ввода (это пропускаем)
            if (!((e.KeyChar >= 'A' && e.KeyChar <= 'Z') || e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Enter))
            {
                e.Handled = true;
                MessageBox.Show("Вводите только ЗАГЛАВНЫЕ английские буквы!");
            }
            //ловим enter
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
            //при изменении (выборе нового аэропорта)
            this.textBox1.Text = this.comboBox1.Text;
            this.ICAOCode = this.textBox1.Text;
            //обновляем аэропорт автоматически!
            CheckWeather(ICAOCode);
        }
    }
}