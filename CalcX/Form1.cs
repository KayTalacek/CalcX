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

        public CalcX()
        {
            InitializeComponent();

            string URL = "https://api.kb.cz/openapi/v1/exchange-rates";
            string response = webGetMethod(URL);
            var objects = JArray.Parse(response);
            string datum = "";

            foreach (JProperty root in objects[0])
            {
                //Console.WriteLine(root[0]);   //test log
                foreach (JToken token in root)
                {
                    //Console.WriteLine(token);   //test log
                    //string output = JsonConvert.SerializeObject(token[0]);
                    //Currency deserializedProduct = JsonConvert.DeserializeObject<Currency>(output);
                    //Console.WriteLine(deserializedProduct.Country);
                    //------------------------------------------------ hore funguje

                    for (int i = 0; i < 18; i++)
                    {
                        string output1 = JsonConvert.SerializeObject(token[i]);
                        Currency deserializedProduct1 = JsonConvert.DeserializeObject<Currency>(output1);
                        if (datum == "")
                        {
                            datum = deserializedProduct1.RatesValidityDate;
                        }
                        //Console.WriteLine(deserializedProduct1.CurrencyISO + " -> " + deserializedProduct1.Middle + "Kč");
                        textBox1.Text += Environment.NewLine + "1 " + deserializedProduct1.CurrencyISO + " -> " + deserializedProduct1.Middle + " CZK";
                    }
                }
            }
            //Console.WriteLine("\nAktualizace: " + datum.Substring(0, 10));
            textBox1.Text += Environment.NewLine + Environment.NewLine + "Aktualizace:  " + datum.Substring(0, 10);
            //Console.ReadLine();
        }

        private void CalcX_Load(object sender, EventArgs e) { }

        private void textBox1_TextChanged(object sender, EventArgs e) { }
    }
}
