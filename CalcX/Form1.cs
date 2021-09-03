using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalcX
{
    public partial class CalcX : Form
    {

        public static string webGetMethod(string URL)
        {
            string jsonString = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            request.Credentials = CredentialCache.DefaultCredentials;
            ((HttpWebRequest)request).UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 7.1; Trident/5.0)";
            request.Accept = "/";
            request.UseDefaultCredentials = true;
            request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            request.ContentType = "application/x-www-form-urlencoded";

            WebResponse response = request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            jsonString = sr.ReadToEnd();
            sr.Close();
            return jsonString;
        }

        double usd = 0.0;
        double cny = 0.0;
        double eur = 0.0;

        public CalcX()
        {
            InitializeComponent();

            string URL = "https://api.kb.cz/openapi/v1/exchange-rates";
            string response = webGetMethod(URL);
            var objects = JArray.Parse(response);
            string datum = "";
            

            foreach (JProperty root in objects[0])
            {
                foreach (JToken token in root)
                {

                    for (int i = 0; i < 18; i++)
                    {
                        string output1 = JsonConvert.SerializeObject(token[i]);
                        Currency deserializedProduct1 = JsonConvert.DeserializeObject<Currency>(output1);
                        if (datum == "")
                        {
                            datum = deserializedProduct1.RatesValidityDate;
                        }
                        textBox1.Text += Environment.NewLine + "1 " + deserializedProduct1.CurrencyISO + " -> " + deserializedProduct1.Middle + " CZK";
                        if (deserializedProduct1.CurrencyISO == "USD")
                        {
                            usd = Convert.ToDouble(deserializedProduct1.Middle);
                        }
                        if (deserializedProduct1.CurrencyISO == "CNY")
                        {
                            cny = Convert.ToDouble(deserializedProduct1.Middle);
                        }
                        if (deserializedProduct1.CurrencyISO == "EUR")
                        {
                            eur = Convert.ToDouble(deserializedProduct1.Middle);
                        }
                    }
                }
            }
            textBox1.Text += Environment.NewLine + Environment.NewLine + "Aktualizace:  " + datum.Substring(0, 10);
        }

        private void CalcX_Load(object sender, EventArgs e) {
            comboBox1.SelectedIndex = 4;
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            //textBox3.Text = Convert.ToString(e.KeyCode);
            if (e.KeyCode == Keys.Enter)
            {
                textBox3.Text = Convert.ToString(Convert.ToDouble(textBox2.Text) * usd);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}


// numbers -> 96 - 105