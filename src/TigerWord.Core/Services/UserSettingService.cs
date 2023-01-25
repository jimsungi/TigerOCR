using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

//#pragma warning disable CS8600 // null 리터럴 또는 가능한 null값을 null을 허용하지 않는 형식으로 변환하는 중입니다
//#pragma warning disable CS8601 // 가능한 null 참조 할당입니다
//#pragma warning disable CS8602 // null 가능 참조에 대한 역참조입니다.
//#pragma warning disable CS8603 // 가능한 null 참조 반환입니다
//#pragma warning disable CS8604 // 가능한 null 참조 인수입니다가능한 null 참조 반환입니다
//#pragma warning disable CS8618 // 생성자를 종료할 때 null을 허용하지 않는 속성 ㅌㅌㅌ에 nul이 아닌 값을 포함해야 합니다.

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
