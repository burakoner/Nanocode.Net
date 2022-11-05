using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace Nanocode.Net.SMS.Providers
{
    internal class NetGsm
    {
        // Adresler
        public string UrlSmsGet = "https://api.netgsm.com.tr/sms/send/get/";
        public string UrlSmsPost = "https://api.netgsm.com.tr/sms/send/xml";
        public string UrlOtpPost = "https://api.netgsm.com.tr/sms/send/otp";

        // Ayarlar
        public string Username = "";
        public string Password = "";
        public string Originator = "";

        // Sunucu Cevabı
        public string ServerResponse = "";

        // Durum Kodları
        public int StatusCode = 0;
        public string StatusDescription = "";

        // Bakiye
        public int BalanceCount = 0;
        public double BalanceAmount = 0.0;

        // Order
        public int OrderId = 0;

        // Sms
        public string SmsBody = "";

        public NetGsm(string user, string pass, string originator)
        {
            this.Username = user;
            this.Password = pass;
            this.Originator = originator;
        }

        public bool SendSms(string recipient, string SmsText)
        {
            try
            {
                // Prepare
                this.SmsBody = SmsText;

                // Send Via Get-Works
                SendSmsViaGet(recipient, this.SmsBody);

                // Send Via Post-Not Working
                // SendSmsViaPost(Recipents, this.SmsBody);

                // Return
                return this.StatusCode == 0 || this.StatusCode == 200;
            }
            catch
            {
                return false;
            }
        }

        public bool SendOTP(string recipient, string SmsText)
        {
            try
            {
                // Prepare
                this.SmsBody = SmsText;

                // Send OTP-Works
                // SendOTPViaGet(Recipents, this.SmsBody);

                // Send OTP Post-Works
                SendOTPViaPost(recipient, this.SmsBody);

                // Return
                return this.StatusCode == 0 || this.StatusCode == 200;
            }
            catch
            {
                return false;
            }
        }

        private void SendSmsViaGet(string recipient, string SmsText)
        {
            // Reset Status
            this.StatusCode = 0;
            this.SetStatusDescription();

            // Basic GET Request
            this.DoRequest(UrlSmsGet, "GET", Uri.EscapeUriString("usercode=" + Username + "&password=" + Password + "&message=" + SmsText + "&gsmno=" + recipient + "&msgheader=" + this.Originator + "&dil=TR"));

            // Console
            Console.WriteLine(this.ServerResponse);
        }

        private void SendOTPViaGet(string recipient, string SmsText)
        {
            // Reset Status
            this.StatusCode = 0;
            this.SetStatusDescription();

            // Basic GET Request
            this.DoRequest(UrlOtpPost + Uri.EscapeUriString("?usercode=" + Username + "&password=" + Password + "&msg=" + SmsText + "&no=" + recipient + "&msgheader=" + this.Originator), "GET","");

            // Console
            Console.WriteLine(this.ServerResponse);
        }

        private void SendSmsViaPost(string recipient, string SmsText)
        {
            // Reset Status
            this.StatusCode = 0;
            this.SetStatusDescription();

            // Prepare POST Request Xml
            string xmlRequest = "<?xml version='1.0' encoding='UTF-8'?>";
            xmlRequest += "<mainbody>";
            xmlRequest += "<header>";
            xmlRequest += "<company dil='TR'>Netgsm</company>";
            xmlRequest += "<usercode>" + this.Username + "</usercode>";
            xmlRequest += "<password>" + this.Password + "</password>";
            xmlRequest += "<type>1:n<</type>";
            xmlRequest += "<msgheader>" + this.Originator + "</msgheader>";
            xmlRequest += "</header>";
            xmlRequest += "<body>";
            xmlRequest += "<msg><![CDATA[" + SmsText + "]]></msg>";
            xmlRequest += "<no>" + recipient + "</no>";
            xmlRequest += "</body>";
            xmlRequest += "</mainbody>";

            // Do POST Request
            this.DoRequest(UrlSmsPost, "POST", xmlRequest);

            // Parse Response
            this.StatusCode = Convert.ToInt32(this.ServerResponse.Trim(' ').FirstOrDefault());
            this.SetStatusDescription();

            // Console
            Console.WriteLine(this.ServerResponse);
        }

        private void SendOTPViaPost(string recipient, string SmsText)
        {
            // Reset Status
            this.StatusCode = 0;
            this.SetStatusDescription();

            // Prepare POST Request Xml
            string xmlRequest = "<?xml version='1.0' encoding='UTF-8'?>";
            xmlRequest += "<mainbody>";
            xmlRequest += "<header>";
            xmlRequest += "<usercode>" + this.Username + "</usercode>";
            xmlRequest += "<password>" + this.Password + "</password>";
            xmlRequest += "<msgheader>" + this.Originator + "</msgheader>";
            xmlRequest += "</header>";
            xmlRequest += "<body>";
            xmlRequest += "<msg><![CDATA[" + SmsText + "]]></msg>";
            xmlRequest += "<no>" + recipient + "</no>";
            xmlRequest += "</body>";
            xmlRequest += "</mainbody>";

            // Do POST Request
            this.DoRequest(UrlOtpPost, "POST", xmlRequest);

            // Parse Xml Response
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(this.ServerResponse);
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("main/code");
            foreach (XmlNode node in nodeList)
            {
                this.StatusCode = int.Parse(node.InnerText);
            this.SetStatusDescription();
            }

            // Console
            Console.WriteLine(this.ServerResponse);
        }

        private void SetStatusDescription()
        {
            switch (this.StatusCode)
            {
                case 0:
                    this.StatusDescription = "";
                    break;
                case 1:
                    this.StatusDescription = "Mesaj gönderim baslangıç tarihinde hata var. Sistem tarihi ile değiştirilip işleme alındı.";
                    break;
                case 2:
                    this.StatusDescription = "Mesaj gönderim sonlandırılma tarihinde hata var.Sistem tarihi ile değiştirilip işleme alındı.Bitiş tarihi başlangıç tarihinden küçük girilmiş ise, sistem bitiş tarihine içinde bulunduğu tarihe 24 saat ekler.";
                    break;
                case 20:
                    this.StatusDescription = "Mesaj metninde ki problemden dolayı gönderilemediğini veya standart maksimum mesaj karakter sayısını geçtiğini ifade eder. (Standart maksimum karakter sayısı 917 dir. Eğer mesajınız türkçe karakter içeriyorsa Türkçe Karakter Hesaplama menüsunden karakter sayılarının hesaplanış şeklini görebilirsiniz.)";
                    break;
                case 30:
                    this.StatusDescription = "Geçersiz kullanıcı adı , şifre veya kullanıcınızın API erişim izninin olmadığını gösterir.Ayrıca eğer API erişiminizde IP sınırlaması yaptıysanız ve sınırladığınız ip dışında gönderim sağlıyorsanız 30 hata kodunu alırsınız. API erişim izninizi veya IP sınırlamanızı , web arayüzden; sağ üst köşede bulunan ayarlar> API işlemleri menüsunden kontrol edebilirsiniz.";
                    break;
                case 40:
                    this.StatusDescription = "Mesaj başlığınızın (gönderici adınızın) sistemde tanımlı olmadığını ifade eder. Gönderici adlarınızı API ile sorgulayarak kontrol edebilirsiniz.";
                    break;
                case 50:
                    this.StatusDescription = "Abone hesabınız ile İYS kontrollü gönderimler yapılamamaktadır.";
                    break;
                case 51:
                    this.StatusDescription = "Aboneliğinize tanımlı İYS Marka bilgisi bulunamadığını ifade eder.";
                    break;
                case 70:
                    this.StatusDescription = "Hatalı sorgulama. Gönderdiğiniz parametrelerden birisi hatalı veya zorunlu alanlardan birinin eksik olduğunu ifade eder.";
                    break;
                case 200:
                    this.StatusDescription = "";
                    break;
                default:
                    this.StatusDescription = "Bilinmeyen Durum Kodu";
                    break;
            }
        }

        private void DoRequest(string requestUrl, string requestMethod, string requestData)
        {
            if (requestMethod == "GET")
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.IsNullOrEmpty(requestData) ? requestUrl : requestUrl + "?" + requestData);
                    request.AutomaticDecompression = DecompressionMethods.GZip;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var html = reader.ReadToEnd();
                        this.StatusCode = html.Contains("<code>0</code>") ? 0 : Convert.ToInt32(html.Trim(' ').FirstOrDefault());
                    }
                }
                catch (WebException e)
                {
                    StatusCode = Convert.ToInt32(e.Message.Trim(' ').FirstOrDefault());
                }
                catch
                {
                    StatusCode = 0;
                }
                finally
                {
                    SetStatusDescription();
                }
            }
            else if (requestMethod == "POST")
            {
                try
                {
                    WebClient wUpload = new WebClient();
                    HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    Byte[] bPostArray = Encoding.UTF8.GetBytes(requestData);
                    Byte[] bResponse = wUpload.UploadData(requestUrl, "POST", bPostArray);
                    Char[] sReturnChars = Encoding.UTF8.GetChars(bResponse);
                    this.ServerResponse = new string(sReturnChars);
                        this.StatusCode = this.ServerResponse.Contains("<code>0</code>") ? 0 : Convert.ToInt32(this.ServerResponse.Trim(' ').FirstOrDefault());
                }
                catch (WebException e)
                {
                    StatusCode = Convert.ToInt32(e.Message.Between("(", ")"));
                }
                catch
                {
                    StatusCode = 0;
                }
                finally
                {
                    SetStatusDescription();
                }
            }

        }

    }
}