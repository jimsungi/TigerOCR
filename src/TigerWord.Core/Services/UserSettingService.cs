using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TigerWord.Core.Services
{
    public interface IUserSettingService
    {
        string GetSetting(string key);
        string GetSetting(string key, string defaultValue);
        void SetSetting(string key, string value);

        string GetAppSetting(string app, string key);
        string GetAppSetting(string app, string key, string defaultValue);
        void SetAppSetting(string app, string key, string value);

    }

    public class UserSettingService : IUserSettingService
    {
        string regstring = @"tigerword\setting";

        public string GetAppSetting(string app, string key_value)
        {
            string regstring = string.Format(@"tigerword\{0}\setting", app);
            string value = "";
            {
                try
                {
                    RegistryKey key = Registry.CurrentUser.CreateSubKey(regstring);
                    key.Close();
                }
                catch { }
                finally { }
            }

            {
                try
                {

                    RegistryKey key = Registry.CurrentUser.OpenSubKey(regstring, true);
                    value = key.GetValue(key_value).ToString();
                    key.Close();
                }
                catch { }
                finally { }
            }
            return value;
        }

        public string GetAppSetting(string app, string key, string defaultValue)
        {           
            string value = GetSetting(app,key);
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;
            return value;
        }

        public string GetSetting(string key_value)
        {
            string value = "";
            {
                try
                {
                    RegistryKey key = Registry.CurrentUser.CreateSubKey(regstring);
                    key.Close();
                }
                catch { }
                finally { }
            }

            {
                try
                {

                    RegistryKey key = Registry.CurrentUser.OpenSubKey(regstring, true);
                    value = key.GetValue(key_value).ToString();
                    key.Close();
                }
                catch { }
                finally { }
            }
            return value;
        }

        public string GetSetting(string key, string defaultValue)
        {
            string value = GetSetting(key);
            if(string.IsNullOrWhiteSpace(value))
                return defaultValue;
            return value;
        }

        public void SetAppSetting(string app, string key_value, string value)
        {
            string regstring = string.Format(@"tigerword\{0}\setting", app);
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(regstring, true);
                key.SetValue(key_value, value);
                key.Close();
            }
            catch { }
            finally { }
        }

        public void SetSetting(string key_value, string value)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(regstring, true);
                key.SetValue(key_value, value);
                key.Close();
            }
            catch { }
            finally { }
        }
    }
}
