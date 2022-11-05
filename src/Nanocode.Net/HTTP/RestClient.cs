using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace Nanocode.Net.HTTP
{
    public class RestClient
    {
        // Enumerations
        public enum Methods : int
        {
            GET,
            POST,
            PUT,
            PATCH,
            DELETE,
            COPY,
            HEAD,
            OPTIONS,
            LINK,
            UNLINK,
            PURGE,
            LOCK,
            UNLOCK,
            PROPFIND,
            VIEW
        }
        public enum ContentTypes : int
        {
            [Description("application/json; charset=utf-8")]
            JSON,

            [Description("application/javascript; charset=utf-8")]
            JSON_P,

            [Description("application/x-www-form-urlencoded")]
            POST,

            [Description("application/text/xml; charset=utf-8")]
            SOAP_1_1,

            [Description("application/soap+xml; charset=utf-8")]
            SOAP_1_2
        }

        // Public Variables
        public string Url { get; set; }
        public Methods Method { get; set; }
        public ContentTypes ContentType { get; set; }
        public string RequestData { get; set; }
        public string ResponseData { get; private set; }
        public int Timeout { get; set; }

        public RestClient() : this(string.Empty, Methods.GET, ContentTypes.JSON, null)
        {
        }

        public RestClient(string url) : this(url, Methods.GET, ContentTypes.JSON, null)
        {
        }

        public RestClient(string url, Methods method) : this(url, method, ContentTypes.JSON, null)
        {
        }

        public RestClient(string url, Methods method, ContentTypes contentType) : this(url, method, contentType, null)
        {
        }

        public RestClient(string url, Methods method, ContentTypes contentType, string requestData)
        {
            this.Url = url;
            this.Method = method;
            this.ContentType = contentType;
            this.RequestData = requestData;
            this.Timeout = 60000;
        }

        public string Request()
        {
            var request = (HttpWebRequest)WebRequest.Create(Url);

            request.Timeout = this.Timeout;
            request.Method = Method.ToString();
            request.ContentType = GetEnumDescription(ContentType);

            try
            {
                if (!string.IsNullOrEmpty(RequestData) && Method == Methods.POST)
                {
                    var bytes = Encoding.UTF8.GetBytes(RequestData);
                    request.ContentLength = bytes.Length;

                    using (var writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    ResponseData = string.Empty;
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new ApplicationException(String.Format("Request failed. Received HTTP {0}", response.StatusCode));
                    }

                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (var reader = new StreamReader(responseStream))
                            {
                                ResponseData = reader.ReadToEnd();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return ResponseData;
        }

        public static string GetEnumDescription(object enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());
            if (null != fi)
            {
                object[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return string.Empty;
        }

    }
}


/*
 * ÖRNEK KULLANIM
 * 
Parametresiz Gönderim
WebServiceClient.Methods.GET
http://10.10.8.128:8080/gui/services/legalProceeding/getLegalAccountList/BISY/83a34e3d936358043ab3dae00b887ace/201506785
WebServiceClient wsc = new Paratica.Data.Net.WebServiceClient(textBox1.Text, WebServiceClient.Methods.GET, WebServiceClient.ContentTypes.JSON, textBox2.Text);


Parametreli Gönderim
WebServiceClient.Methods.POST
http://10.10.8.128:8080/gui/services/legalProceeding/legalStatusUpdate/BISY/83a34e3d936358043ab3dae00b887ace/
{"data" : [{"LEGAL_PROCEEDING_CODE":201506836,"STATUS":3,"STATUS_DATE":"24/02/2017 17:34:23","DESCRIPTION":"DOSYA BISY TARAFINDAN KAPATILDI"}]}
WebServiceClient wsc = new Paratica.Data.Net.WebServiceClient(textBox1.Text, WebServiceClient.Methods.POST, WebServiceClient.ContentTypes.JSON, textBox2.Text);

 * 
 * 
 */
