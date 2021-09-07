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

        Meny meny;

        public CalcX() {
            InitializeComponent();
            mainHandler();
        }

        private void CalcX_Load(object sender, EventArgs e) {
            comBox_Meny.SelectedIndex = 4;
            comBox_Rezerva.SelectedIndex = 0;
        }

/* TODO:
 * pridat automatickou opravu prazdneho tBoxu -> na nulu
 * pocitat prevod podle dat ze struktury 
 * rozdelit combobox na dve (meny) a pak jen delit z leva do prava
 * odstranit "KeyDown" u kazdeho textBoxu
 * prekopat na pouziti struktury ... nebo mozna na 3 pole? (currencyISO, original kurz, kurz s rezervou)
 */

        struct Meny {
            public double aud;
            public double bgn;
            public double cad;
            public double chf;
            public double cny;
            public double dkk;
            public double eur;
            public double gbp;
            public double hrk;
            public double huf;
            public double jpy;
            public double nok;
            public double pln;
            public double ron;
            public double rub;
            public double sek;
            public double tryy;
            public double usd;
        }

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

                        string mena = deserializedProduct1.CurrencyISO;

                        switch (mena) {
                            case "AUD":
                                meny.aud = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "BGN":
                                meny.bgn = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "CAD":
                                meny.cad = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "CHF":
                                meny.chf = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "CNY":
                                meny.cny = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "DKK":
                                meny.dkk = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "EUR":
                                meny.eur = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "GBP":
                                meny.gbp = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "HRK":
                                meny.hrk = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "HUF":
                                meny.huf = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "JPY":
                                meny.jpy = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "NOK":
                                meny.nok = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "PLN":
                                meny.pln = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "RON":
                                meny.ron = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "RUB":
                                meny.rub = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "SEK":
                                meny.sek = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "TRY":
                                meny.tryy = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            case "USD":
                                meny.usd = Convert.ToDouble(deserializedProduct1.Middle);
                                break;
                            default:
                                break;
                        }
                        
                        tBox_Kurzy_Kurzy.Text += "1 " + deserializedProduct1.CurrencyISO + "  =  " + deserializedProduct1.Middle + " CZK" + Environment.NewLine;
                        tBox_Kurzy_Marze_Kurzy.Text += "1 " + deserializedProduct1.CurrencyISO + "  =  " + Convert.ToString(Math.Round(Convert.ToDouble(deserializedProduct1.Middle) + 0.2, 4)) + " CZK" + Environment.NewLine;
                    }
                }
            }
            tBox_Kurzy_Update.Text += "Aktualizace:  " + datum.Substring(0, 10);
            tBox_Kurzy_Marze_Update.Text += "Aktualizace:  " + datum.Substring(0, 10);
        }

        public bool kontrolaVstupu(object sender, char znak) {
            if (!char.IsControl(znak) && !char.IsDigit(znak) && (znak != ',')) return true;
            else return false;
        }

        public bool kontrolaDesetinnychMist(object sender, char znak) {
            if ((znak == ',') && ((sender as TextBox).Text.IndexOf(',') > -1)) return true;
            else return false;
        }

        public void kontrolaPrazdnehoBoxu(object sender) { }

        private void tBox_Leva_Mena_KeyPress(object sender, KeyPressEventArgs e) {
            if (kontrolaVstupu(sender, e.KeyChar)) e.Handled = true;
            if (e.KeyChar == ',') if (kontrolaDesetinnychMist(sender, e.KeyChar) == true) e.Handled = true;
        }

        private void tBox_Leva_Mena_TextChanged(object sender, EventArgs e) {
            if (tBox_Leva_Mena.TextLength != 0) {
                double procenta = 1.0;
                if (tBox_Procenta.TextLength != 0) {
                    procenta = (Convert.ToDouble(tBox_Procenta.Text) / 100) + 1;
                }

                switch (comBox_Meny.SelectedIndex) {
                    case 0:
                        tBox_Prava_Mena.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leva_Mena.Text) * meny.usd, 4));
                        break;
                    case 1:
                        tBox_Prava_Mena.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leva_Mena.Text) / meny.usd, 4));
                        break;
                    case 2:
                        tBox_Prava_Mena.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leva_Mena.Text) * meny.eur, 4));
                        break;
                    case 3:
                        tBox_Prava_Mena.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leva_Mena.Text) / meny.eur, 4));
                        break;
                    case 4:
                        tBox_Prava_Mena.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leva_Mena.Text) * meny.cny, 4));
                        break;
                    case 5:
                        tBox_Prava_Mena.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leva_Mena.Text) / meny.cny, 4));
                        break;
                    case 6:
                        tBox_Prava_Mena.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leva_Mena.Text) * (meny.cny / meny.usd), 4));
                        break;
                    default:
                        MessageBox.Show("Špatný vstup...", "Chyba!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
                tBox_Marze.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Prava_Mena.Text) * procenta, 4));
            }
        }

        private void comBox_Meny_SelectedIndexChanged(object sender, EventArgs e) { }

        private void chBox_Marze_CheckedChanged(object sender, EventArgs e) {
            if (chBox_Marze.Checked){
                tBox_Marze.Visible = true;
                tBox_Procenta.Visible = true;
                btn_Hidden.Visible = true;
                btn_Copy.Visible = true;
            }
            else {
                tBox_Marze.Visible = false;
                tBox_Procenta.Visible = false;
                btn_Hidden.Visible = false;
                btn_Copy.Visible = false;
            }
        }

        private void btn_Copy_Click(object sender, EventArgs e) {
            if (tBox_Marze.TextLength != 0) Clipboard.SetText(tBox_Marze.Text);
        }

        private void btn_Hidden_Click(object sender, EventArgs e) {
            if (tBox_Prava_Mena.TextLength != 0) {
                double procento = Convert.ToDouble(tBox_Procenta.Text) / 100.0;
                double puvodniHodnota = Convert.ToDouble(tBox_Prava_Mena.Text);
                tBox_Marze.Text = Convert.ToString(Math.Round(puvodniHodnota * (procento + 1), 4));
            }
        }

        private void tBox_Prava_Mena_KeyPress(object sender, KeyPressEventArgs e) {
            if (kontrolaVstupu(sender, e.KeyChar)) e.Handled = true;
            if (e.KeyChar == ',') if (kontrolaDesetinnychMist(sender, e.KeyChar) == true) e.Handled = true;
        }

        private void tBox_Procenta_KeyPress(object sender, KeyPressEventArgs e) {
            if (kontrolaVstupu(sender, e.KeyChar)) e.Handled = true;
            if (e.KeyChar == ',') if (kontrolaDesetinnychMist(sender, e.KeyChar) == true) e.Handled = true;
        }
        private void tBox_Procenta_TextChanged(object sender, EventArgs e) {
            if (tBox_Procenta.TextLength != 0 && tBox_Prava_Mena.TextLength != 0){
                tBox_Marze.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Prava_Mena.Text) * 1.05 * meny.cny, 4));
            }
        }

        private void tBox_Leo_CN_CNY_KeyPress(object sender, KeyPressEventArgs e) {
            if (kontrolaVstupu(sender, e.KeyChar)) e.Handled = true;
            if (e.KeyChar == ',') if (kontrolaDesetinnychMist(sender, e.KeyChar) == true) e.Handled = true;
        }

        private void tBox_Leo_CN_CNY_TextChanged(object sender, EventArgs e) {
            if (tBox_Leo_CN_CNY.TextLength != 0) {
                tBox_Leo_CN_CZK.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leo_CN_CNY.Text) * 1.05 * meny.cny, 4));
            }
        }

        private void tBox_Leo_HK_USD_KeyPress(object sender, KeyPressEventArgs e) {
            if (kontrolaVstupu(sender, e.KeyChar)) e.Handled = true;
            if (e.KeyChar == ',') if (kontrolaDesetinnychMist(sender, e.KeyChar) == true) e.Handled = true;
        }

        private void tBox_Leo_HK_USD_TextChanged(object sender, EventArgs e) {
            if (tBox_Leo_HK_USD.TextLength != 0) {
                tBox_Leo_HK_CZK.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leo_HK_USD.Text) * meny.usd * 1.13 * 1.05, 4));
            }
        }

        private void chBox_Rezerva_CheckedChanged(object sender, EventArgs e) {
            if (chBox_Rezerva.Checked) {
                tBox_Rezerva.Visible = true;
                comBox_Rezerva.Visible = true;
            }
            else {
                tBox_Rezerva.Visible = false;
                comBox_Rezerva.Visible = false;
            }
        }

        private void tBox_Rezerva_TextChanged(object sender, EventArgs e) {
            if (tBox_Rezerva.TextLength != 0) {
                tBox_Rezerva.Text = Convert.ToString(Convert.ToDouble(tBox_Rezerva.Text));
                tabPage_Kurzy_Marze.Text = "Kurzy + " + tBox_Rezerva.Text + comBox_Rezerva.SelectedItem;
                tBox_Kurzy_Marze_Kurzy.Refresh();
            }
        }

        private void tBox_Rezerva_KeyPress(object sender, KeyPressEventArgs e) {
            if (kontrolaVstupu(sender, e.KeyChar)) e.Handled = true;
            if (e.KeyChar == ',') if (kontrolaDesetinnychMist(sender, e.KeyChar) == true) e.Handled = true;
        }

        private void comBox_Rezerva_SelectedIndexChanged(object sender, EventArgs e) {
            if (tBox_Rezerva.TextLength != 0) {
                tBox_Rezerva.Text = Convert.ToString(Convert.ToDouble(tBox_Rezerva.Text));
                tabPage_Kurzy_Marze.Text = "Kurzy + " + tBox_Rezerva.Text + comBox_Rezerva.SelectedItem;
                tBox_Kurzy_Marze_Kurzy.Refresh();
            }
        }
    }
}