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

namespace CalcX {
    public partial class CalcX : Form {

        public CalcX() {
            InitializeComponent();
            mainHandler();
        }

        private void CalcX_Load(object sender, EventArgs e) {
            comboBox1.SelectedIndex = 4;
            comboBox3.SelectedIndex = 1;
        }

/* TODO:
 * natahnout vsechny meny
 * rozdelit combobox na dve (meny) a pak jen delit z leva do prava
 */
        double usd = 0.0;
        double cny = 0.0;
        double eur = 0.0;

        public string webGetMethod(string URL) {
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
        public void mainHandler() {
            string URL = "https://api.kb.cz/openapi/v1/exchange-rates";
            string response = webGetMethod(URL);
            var objects = JArray.Parse(response);
            string datum = "";

            foreach (JProperty root in objects[0]) {
                foreach (JToken token in root) {
/* !!!
 *  Potencialni problem v budoucnu - pevne dany pocet prvku -> (i < 18)
 */
                    for (int i = 0; i < 18; i++) { 
                        string output1 = JsonConvert.SerializeObject(token[i]);
                        Currency deserializedProduct1 = JsonConvert.DeserializeObject<Currency>(output1);

                        if (datum == "") datum = deserializedProduct1.RatesValidityDate;

                        if (deserializedProduct1.CurrencyISO == "USD") {
                            usd = Convert.ToDouble(deserializedProduct1.Middle);
                        }
                        else if (deserializedProduct1.CurrencyISO == "CNY") {
                            cny = Convert.ToDouble(deserializedProduct1.Middle);
                        }
                        else if (deserializedProduct1.CurrencyISO == "EUR") {
                            eur = Convert.ToDouble(deserializedProduct1.Middle);
                        }
                        else { }

                        textBox1.Text += "1 " + deserializedProduct1.CurrencyISO + "  =  " + deserializedProduct1.Middle + " CZK" + Environment.NewLine;
                        textBox12.Text += "1 " + deserializedProduct1.CurrencyISO + "  =  " + Convert.ToString(Math.Round(Convert.ToDouble(deserializedProduct1.Middle) + 0.2, 4)) + " CZK" + Environment.NewLine;
                    }
                }
            }
            textBox9.Text += "Aktualizace:  " + datum.Substring(0, 10);
            textBox11.Text += "Aktualizace:  " + datum.Substring(0, 10);
        }

        public bool kontrolaVstupu(object sender, char znak) {
            if (!char.IsControl(znak) && !char.IsDigit(znak) && (znak != ',')) return true;
            else return false;
        }

        public bool kontrolaDesetinnychMist(object sender, char znak) {
            if ((znak == ',') && ((sender as TextBox).Text.IndexOf(',') > -1)) return true;
            else return false;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e) {
            if (kontrolaVstupu(sender, e.KeyChar)) e.Handled = true;
            if (e.KeyChar == ',') if (kontrolaDesetinnychMist(sender, e.KeyChar) == true) e.Handled = true;
        }

/* TODO:
 * automaticky prevod
 */
        private void textBox2_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && textBox2.TextLength != 0) {
                switch (comboBox1.SelectedIndex) {
                    case 0:
                        textBox3.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox2.Text) * usd, 4));
                        break;
                    case 1:
                        textBox3.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox2.Text) / usd, 4));
                        break;
                    case 2:
                        textBox3.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox2.Text) * eur, 4));
                        break;
                    case 3:
                        textBox3.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox2.Text) / eur, 4));
                        break;
                    case 4:
                        textBox3.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox2.Text) * cny, 4));
                        break;
                    case 5:
                        textBox3.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox2.Text) / cny, 4));
                        break;
                    default:
                        MessageBox.Show("Špatný vstup...", "Chyba!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e) {
            if (textBox2.TextLength != 0) {
                double procenta = 1.0;
                if (textBox10.TextLength != 0) {
                    procenta = (Convert.ToDouble(textBox10.Text) / 100) + 1;
                }

                switch (comboBox1.SelectedIndex) {
                    case 0:
                        textBox3.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox2.Text) * usd, 4));
                        break;
                    case 1:
                        textBox3.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox2.Text) / usd, 4));
                        break;
                    case 2:
                        textBox3.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox2.Text) * eur, 4));
                        break;
                    case 3:
                        textBox3.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox2.Text) / eur, 4));
                        break;
                    case 4:
                        textBox3.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox2.Text) * cny, 4));
                        break;
                    case 5:
                        textBox3.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox2.Text) / cny, 4));
                        break;
                    default:
                        MessageBox.Show("Špatný vstup...", "Chyba!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
                textBox8.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox3.Text) * procenta, 4));
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            if (checkBox1.Checked){
                textBox8.Visible = true;
                textBox10.Visible = true;
                button1.Visible = true;
                copyBTN.Visible = true;
            }
            else {
                textBox8.Visible = false;
                textBox10.Visible = false;
                button1.Visible = false;
                copyBTN.Visible = false;
            }
        }

        private void copyBTN_Click(object sender, EventArgs e) {
            if (textBox8.TextLength != 0) Clipboard.SetText(textBox8.Text);
        }

        private void button1_Click(object sender, EventArgs e) {
            if (textBox3.TextLength != 0) {
                double procento = Convert.ToDouble(textBox10.Text) / 100.0;
                double puvodniHodnota = Convert.ToDouble(textBox3.Text);
                textBox8.Text = Convert.ToString(Math.Round(puvodniHodnota * (procento + 1), 4));
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e) {
            if (kontrolaVstupu(sender, e.KeyChar)) e.Handled = true;
            if (e.KeyChar == ',') if (kontrolaDesetinnychMist(sender, e.KeyChar) == true) e.Handled = true;
        }

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e) {
            if (kontrolaVstupu(sender, e.KeyChar)) e.Handled = true;
            if (e.KeyChar == ',') if (kontrolaDesetinnychMist(sender, e.KeyChar) == true) e.Handled = true;
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e) {
            if (kontrolaVstupu(sender, e.KeyChar)) e.Handled = true;
            if (e.KeyChar == ',') if (kontrolaDesetinnychMist(sender, e.KeyChar) == true) e.Handled = true;
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && textBox5.TextLength != 0) {
                textBox4.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox5.Text) * 1.05 * cny, 4));
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e) {
            if (textBox5.TextLength != 0) {
                textBox4.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox5.Text) * 1.05 * cny, 4));
            }
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e) {
            if (kontrolaVstupu(sender, e.KeyChar)) e.Handled = true;
            if (e.KeyChar == ',') if (kontrolaDesetinnychMist(sender, e.KeyChar) == true) e.Handled = true;
        }

        private void textBox7_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && textBox7.TextLength != 0) {
                textBox6.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox7.Text) * 1.13 * 1.05 * usd, 4));
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e) {
            if (textBox7.TextLength != 0) {
                textBox6.Text = Convert.ToString(Math.Round(Convert.ToDouble(textBox7.Text) * usd, 4));
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e) {
            if (checkBox2.Checked)
            {
                textBox13.Visible = true;
                comboBox3.Visible = true;
            }
            else
            {
                textBox13.Visible = false;
                comboBox3.Visible = false;
            }
        }
    }
}