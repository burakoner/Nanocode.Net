using Nanocode.Net.SMS.Providers;

namespace Nanocode.Net.SMS
{
    public enum SmsProvider
    {
        IletiMerkezi = 1,
        NetGsm = 2,
    }

    public class SmsSender
    {
        // Private Properties
        private IletiMerkezi IletiMerkezi { get;  set; }
        private NetGsm NetGsm { get;  set; }

        // Public Properties
        public SmsProvider Provider { get; private set; }
        public string Username
        {
            get
            {
                if (this.Provider == SmsProvider.IletiMerkezi)
                    return this.IletiMerkezi.Username;
                if (this.Provider == SmsProvider.NetGsm)
                    return this.NetGsm.Username;

                return string.Empty;
            }
            set
            {
                if (this.Provider == SmsProvider.IletiMerkezi)
                    this.IletiMerkezi.Username = value;
                if (this.Provider == SmsProvider.NetGsm)
                    this.NetGsm.Username = value;
            }
        }
        public string Password
        {
            get
            {
                if (this.Provider == SmsProvider.IletiMerkezi)
                    return this.IletiMerkezi.Password;
                if (this.Provider == SmsProvider.NetGsm)
                    return this.NetGsm.Password;

                return string.Empty;
            }
            set
            {
                if (this.Provider == SmsProvider.IletiMerkezi)
                    this.IletiMerkezi.Password = value;
                if (this.Provider == SmsProvider.NetGsm)
                    this.NetGsm.Password = value;
            }
        }
        public string Originator
        {
            get
            {
                if (this.Provider == SmsProvider.IletiMerkezi)
                    return this.IletiMerkezi.Originator;
                if (this.Provider == SmsProvider.NetGsm)
                    return this.NetGsm.Originator;

                return string.Empty;
            }
            set
            {
                if (this.Provider == SmsProvider.IletiMerkezi)
                    this.IletiMerkezi.Originator = value;
                if (this.Provider == SmsProvider.NetGsm)
                    this.NetGsm.Originator = value;
            }
        }
        public string ServerResponse
        {
            get
            {
                if (this.Provider == SmsProvider.IletiMerkezi)
                    return this.IletiMerkezi.ServerResponse;
                if (this.Provider == SmsProvider.NetGsm)
                    return this.NetGsm.ServerResponse;

                return string.Empty;
            }
        }
        public int StatusCode
        {
            get
            {
                if (this.Provider == SmsProvider.IletiMerkezi)
                    return this.IletiMerkezi.StatusCode;
                if (this.Provider == SmsProvider.NetGsm)
                    return this.NetGsm.StatusCode;

                return 0;
            }
        }
        public string StatusDescription
        {
            get
            {
                if (this.Provider == SmsProvider.IletiMerkezi)
                    return this.IletiMerkezi.StatusDescription;
                if (this.Provider == SmsProvider.NetGsm)
                    return this.NetGsm.StatusDescription;

                return string.Empty;
            }
        }
        public string SmsBody
        {
            get
            {
                if (this.Provider == SmsProvider.IletiMerkezi)
                    return this.IletiMerkezi.SmsBody;
                if (this.Provider == SmsProvider.NetGsm)
                    return this.NetGsm.SmsBody;

                return string.Empty;
            }
            set
            {
                if (this.Provider == SmsProvider.IletiMerkezi)
                    this.IletiMerkezi.SmsBody = value;
                if (this.Provider == SmsProvider.NetGsm)
                    this.NetGsm.SmsBody = value;
            }
        }

        public SmsSender(SmsProvider provider): this(provider,"","","")
        {
        }
        public SmsSender(SmsProvider provider, string username, string password, string originator)
        {
            this.IletiMerkezi = new IletiMerkezi(username, password, originator);
            this.NetGsm = new NetGsm(username, password, originator);

            this.Provider = provider;
            this.Username = username;
            this.Password = password;
            this.Originator = originator;
        }

        public bool SendOTP(string recipient, string sms)
        {
            if (this.Provider == SmsProvider.IletiMerkezi)
                return this.IletiMerkezi.SendSms(new string[] { recipient }, sms);
            if (this.Provider == SmsProvider.NetGsm)
                return this.NetGsm.SendOTP(recipient, sms);

            return false;
        }
        public bool SendOTP(string[] recipients, string sms)
        {
            if (this.Provider == SmsProvider.IletiMerkezi)
                return this.IletiMerkezi.SendSms(recipients, sms);
            if (this.Provider == SmsProvider.NetGsm)
            {
                var res = false;
                foreach(var recipient in recipients)
                    res = res & this.NetGsm.SendOTP(recipient, sms);

                return res;
            }

            return false;
        }
        public bool SendSms(string recipient, string sms)
        {
            if (this.Provider == SmsProvider.IletiMerkezi)
                return this.IletiMerkezi.SendSms(new string[] { recipient }, sms);
            if (this.Provider == SmsProvider.NetGsm)
                return this.NetGsm.SendSms(recipient, sms);

            return false;
        }
        public bool SendSms(string[] recipients, string sms)
        {
            if (this.Provider == SmsProvider.IletiMerkezi)
                return this.IletiMerkezi.SendSms(recipients, sms);
            if (this.Provider == SmsProvider.NetGsm)
            {
                var res = false;
                foreach(var recipient in recipients)
                    res = res & this.NetGsm.SendSms(recipient, sms);

                return res;
            }

            return false;
        }
    }

    public static class StringExtensions
    {
        public static string Between(this string @this, string firstString, string lastString, bool includeFirst = false, bool includeLast = false)
        {
            int posA = @this.IndexOf(firstString) + firstString.Length;
            if (posA > @this.Length) return "";
            string temp = @this.Substring(posA);
            int posB = posA + temp.IndexOf(lastString);

            if (posA == -1) return "";
            if (posB == -1) return "";
            if (posA >= posB) return "";

            string FinalString = @this.Substring(posA, posB - posA);
            if (includeFirst) FinalString = firstString + FinalString;
            if (includeLast) FinalString += lastString;
            return FinalString;
        }
    }
}