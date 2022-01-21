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
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace CalcX {
    public partial class CalcX : Form {

        public CalcX() {
            InitializeComponent();
            mainHandler();
        }

        private void CalcX_Load(object sender, EventArgs e) {
            comBox_Meny.SelectedIndex = 4;
            comBox_Rezerva.SelectedIndex = 0;
        }

        public string folderDir = @"C:\CalcX";
        public string fileDir = @"C:\CalcX\kurzy.xml";
        Dictionary<string, double> exchange = new Dictionary<string, double>();
        Dictionary<string, int> exchangeQty = new Dictionary<string, int>();

        public bool folderExists() {
            string dir = folderDir;
            // If directory does not exist, create it
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            return true;
        }

        public void downloadXml(string URL) {
            using (WebClient wc = new WebClient()) {
                //wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFile(
                    // Param1 = Link of file
                    new System.Uri(URL),
                    // Param2 = Path to save
                    fileDir
                );
            }
        }

        public void xmlParser() {
            XmlDocument doc = new XmlDocument();

            doc.Load(fileDir);

            XmlNodeList nodeList = doc.GetElementsByTagName("kurzy");
            string date = nodeList[0].Attributes[1].Value;
            setUpdateDate(date);

            nodeList = doc.GetElementsByTagName("radek");

            foreach (XmlNode node in nodeList){
                exchange.Add(node.Attributes[0].Value, Convert.ToDouble(node.Attributes[3].Value)); //mena | hodnota
                exchangeQty.Add(node.Attributes[0].Value, Convert.ToInt16(node.Attributes[2].Value)); //mena | pocet
            }
        }

        public void setUpdateDate(string date) {
            tBox_Kurzy_Update.Text = tBox_Kurzy_Marze_Update.Text += "Aktualizace: " + date.Substring(0, 10);
            //tBox_Kurzy_Update.Text = tBox_Kurzy_Marze_Update.Text += "Aktualizace:  " + DateTime.Parse(date);
        }

        public void mainHandler() {
            //kontrola, zda existuje misto pro ulozeni souboru XML - pokud neexistuje, vytvori jej
            folderExists();
            
            string URL = "https://www.cnb.cz/cs/financni_trhy/devizovy_trh/kurzy_devizoveho_trhu/denni_kurz.xml";
            downloadXml(URL);
            xmlParser();
            foreach (var mena in exchange) {
                tBox_Kurzy_Kurzy.Text += exchangeQty[mena.Key] + " " + mena.Key + "  =  " + mena.Value + " CZK" + Environment.NewLine;
                tBox_Kurzy_Marze_Kurzy.Text += exchangeQty[mena.Key] + " " + mena.Key + "  =  " + Convert.ToString(Math.Round(Convert.ToDouble(mena.Value) + 0.2, 4)) + " CZK" + Environment.NewLine;
            }
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
                        tBox_Prava_Mena.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leva_Mena.Text) * exchange["USD"], 4));
                        break;
                    case 1:
                        tBox_Prava_Mena.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leva_Mena.Text) / exchange["USD"], 4));
                        break;
                    case 2:
                        tBox_Prava_Mena.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leva_Mena.Text) * exchange["EUR"], 4));
                        break;
                    case 3:
                        tBox_Prava_Mena.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leva_Mena.Text) / exchange["EUR"], 4));
                        break;
                    case 4:
                        tBox_Prava_Mena.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leva_Mena.Text) * exchange["CNY"], 4));
                        break;
                    case 5:
                        tBox_Prava_Mena.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leva_Mena.Text) / exchange["CNY"], 4));
                        break;
                    case 6:
                        tBox_Prava_Mena.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leva_Mena.Text) * (exchange["CNY"] / exchange["USD"]), 4));
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
            if (tBox_Prava_Mena.TextLength != 0 && tBox_Prava_Mena.TextLength != 0) {
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
                tBox_Procenta.Text = Convert.ToString(Convert.ToDouble(tBox_Procenta.Text));
                double procento = Convert.ToDouble(tBox_Procenta.Text) / 100.0;
                double puvodniHodnota = Convert.ToDouble(tBox_Prava_Mena.Text);
                tBox_Marze.Text = Convert.ToString(Math.Round(puvodniHodnota * (procento + 1), 4));
            }
        }

        private void tBox_Leo_CN_CNY_KeyPress(object sender, KeyPressEventArgs e) {
            if (kontrolaVstupu(sender, e.KeyChar)) e.Handled = true;
            if (e.KeyChar == ',') if (kontrolaDesetinnychMist(sender, e.KeyChar) == true) e.Handled = true;
        }

        private void tBox_Leo_CN_CNY_TextChanged(object sender, EventArgs e) {
            if (tBox_Leo_CN_CNY.TextLength != 0) {
                tBox_Leo_CN_CZK.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leo_CN_CNY.Text) * 1.05 * exchange["CNY"], 4));
            }
        }

        private void tBox_Leo_HK_USD_KeyPress(object sender, KeyPressEventArgs e) {
            if (kontrolaVstupu(sender, e.KeyChar)) e.Handled = true;
            if (e.KeyChar == ',') if (kontrolaDesetinnychMist(sender, e.KeyChar) == true) e.Handled = true;
        }

        private void tBox_Leo_HK_USD_TextChanged(object sender, EventArgs e) {
            if (tBox_Leo_HK_USD.TextLength != 0) {
                tBox_Leo_HK_CZK.Text = Convert.ToString(Math.Round(Convert.ToDouble(tBox_Leo_HK_USD.Text) * exchange["USD"] * 1.13 * 1.05, 4));
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

        private void btn_Copy_CN_Click(object sender, EventArgs e) {
            if (tBox_Leo_CN_CZK.TextLength != 0) Clipboard.SetText(tBox_Leo_CN_CZK.Text);
        }

        private void btn_Copy_HK_Click(object sender, EventArgs e) {
            if (tBox_Leo_HK_CZK.TextLength != 0) Clipboard.SetText(tBox_Leo_HK_CZK.Text);
        }
    }
}