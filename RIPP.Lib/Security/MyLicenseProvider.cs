using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Management;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Net;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using RIPP.Lib;

namespace RIPP.Lib.Security
{
    [ReflectionPermission(SecurityAction.Deny, MemberAccess=false, ReflectionEmit=false)]
    public class MyLicenseProvider : LicenseProvider
    {
        #region Define MyLicense
        private class MyLicense : License
        {
            private String mLicenseKey = null;
            private MyLicenseProvider mProvider = null;

            public MyLicense(MyLicenseProvider provider, String key)
            {
                this.mProvider = provider;
                this.mLicenseKey = key;
            }

            public override string LicenseKey
            {
                get { return this.mLicenseKey; }
            }

            public override void Dispose()
            {

                this.mProvider = null;
                this.mLicenseKey = null;
            }
        }
        #endregion

        public MyLicenseProvider()
        { }
       

        public static string GetCpuID()
        {
            try
            {
                string id = "";
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    id = mo.Properties["ProcessorId"].Value.ToString();
                }
                return id;
            }
            catch
            {
                return "unknow";
            }
        }

        private static string _licenseFullPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "MyLicense.lic");
        public static  string LicenseFullPath{
            get{
                return _licenseFullPath;
            }
        }

        public static bool Validation(string cpuid = null, string licFile=null)
        {
            if (string.IsNullOrWhiteSpace(cpuid))
            {
                cpuid = GetCpuID();
                cpuid = DESLicense.Encrypt(cpuid);//加密
            }
            if (string.IsNullOrWhiteSpace(licFile))
                licFile = LicenseFullPath;
            if (File.Exists(licFile))
            {
                var f = Serialize.Read<ChemLicense>(licFile);
                if (f != null && cpuid.Equals(f.CpuID))
                    return true;
            }
            return false;
        }

        public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
        {
            MyLicense license = null;

            string cpuid = GetCpuID();
            cpuid = DESLicense.Encrypt(cpuid);//加密
          
            if (context != null)
            {
                if (context.UsageMode == LicenseUsageMode.Runtime)
                {
                    String savedLicenseKey = context.GetSavedLicenseKey(type, null);
                    if (cpuid.Equals(savedLicenseKey))
                    {
                        return new MyLicense(this, cpuid);
                    }
                }
                if (license != null)
                {
                    return license;
                }
                
                // 打开License文件 'MyLicense.dat'
                if(Validation(cpuid))
                    license = new MyLicense(this, cpuid);
                if (license != null)
                {
                    context.SetSavedLicenseKey(type, cpuid);
                }
            }

            if (license == null)
            {
                //System.Windows.Forms.MessageBox.Show("!!!尚未注册!!!");
                throw new LicenseException(type, instance, "Your license is invalid");;
                //return new MyLicense(this,"no");
                
            }

            return license;
        }
    }
}
