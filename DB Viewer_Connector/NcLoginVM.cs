using Microsoft.Win32;
using NetConnections.PayJoe.Connector.Client.General.ViewModel;
using NetConnections.PayJoe.Connector.Client.GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Application = BpNT.Application;


namespace DB_Viewer_Connector
{
    public class NcLoginVM : NcBindableBase
    {

        #region Member

        Application mMicrotechApplication = null;
        string mFirmenname = null;
        string mBenutzer = null;
        string mPasswort = null;

        #endregion

        #region Properties




        public Application MicrotechApplication
        {
            get
            {
                return mMicrotechApplication;
            }
            set
            {
                mMicrotechApplication = value;
            }
        }

        public string Firmenname
        {
            get
            {
                return mFirmenname;
            }
            set
            {
                SetProperty(ref mFirmenname, value);
                (SetUserLoginDataCommand as NcCommand).ChangeCanExecute();
            }
        }

        public string Benutzer
        {
            get
            {
                return mBenutzer;
            }
            set
            {
                SetProperty(ref mBenutzer, value);
                (SetUserLoginDataCommand as NcCommand).ChangeCanExecute();
            }
        }

        public string Passwort
        {
            get
            {
                return mPasswort;
            }
            set
            {
                SetProperty(ref mPasswort, value);
                (SetUserLoginDataCommand as NcCommand).ChangeCanExecute();
            }
        }

        #endregion

        #region Commands

        public ICommand SetUserLoginDataCommand { get; private set; }
        public ICommand NcLoginFensterClose { get; set; }
        #endregion

        #region Constructors

        public NcLoginVM()
        {
            mMicrotechApplication = new Application();
            SetUserLoginDataCommand = new NcCommand(async () => { await SetUserLoginData(); },
                () =>
                {
                    if (string.IsNullOrWhiteSpace(Firmenname) == true
                    || string.IsNullOrWhiteSpace(Benutzer) == true
                    || string.IsNullOrWhiteSpace(Passwort) == true)
                    {
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                });
            
        }

        private async Task SetUserLoginData()
        {
            try
            {
                mMicrotechApplication.Init(Firmenname, "",  Benutzer, Passwort);
                
                NcSQLAbfrageAusfuehrenVM ncSQLAbfrageAusfuehrenVM = new NcSQLAbfrageAusfuehrenVM(Firmenname, Benutzer, Passwort);
                NcSQLAbfrageAusfuehrenV ncSQLAbfrageAusfuehrenV = new NcSQLAbfrageAusfuehrenV();
                ncSQLAbfrageAusfuehrenV.DataContext = ncSQLAbfrageAusfuehrenVM;
                ncSQLAbfrageAusfuehrenV.Show();
                
            }
            catch (Exception pEx)
            {
                MessageBox.Show(pEx.Message);
                
            }

        }
        internal void SaveParameter(string pParameterKey, string pParameterValue)
        {
            Dictionary <string, string> ModulParameter = new Dictionary<string, string>();
            if (ModulParameter.ContainsKey(pParameterKey) == false)
                ModulParameter.Add(pParameterKey, pParameterValue);
            else
                ModulParameter[pParameterKey] = pParameterValue;
        }

        public void VerbindungBeenden()
        {
            try
            {
                mMicrotechApplication.LogOff();
            }
            catch (Exception pEx)
            {
                throw pEx;
            }
        }

        //internal void SaveModulesToDB()
        //{
        //    RegistryKey tmpValidBaseKey = Registry.LocalMachine;
        //    string tmpValidBaseSubKey = @"Software\NetConnections\PayJoe\Connector";
        //    RegistryKey tmpBaseKey = NcRegistryHelper.GetKey(tmpValidBaseKey, "\\", true);
        //    RegistryKey tmpValidSubKey = NcRegistryHelper.CreateKey(tmpBaseKey, tmpValidBaseSubKey);

        //    NcRegistryKeyValueStorageProvider tmpRegistryKeyValueStorageProvider =
        //        new NcRegistryKeyValueStorageProvider(tmpValidSubKey);

        //    NcDPAPICryptographyProvider tmpDPAPICryptographyProvider =
        //        new NcDPAPICryptographyProvider(DataProtectionScope.CurrentUser, null);


        //    string tmpActiveModules = string.Join(";", "Microtech");

        //    string tmpWertVerschluesselt = tmpDPAPICryptographyProvider.Encrypt(tmpActiveModules);
        //    tmpRegistryKeyValueStorageProvider.SaveEntry("ActiveModules", tmpWertVerschluesselt);

        //    foreach (NcModule tmpModule in mModules)
        //    {
        //        RegistryKey tmpModuleSubKey =
        //            NcRegistryHelper.CreateKey(tmpValidSubKey, tmpModule.ModuleID.ToString());

        //        tmpRegistryKeyValueStorageProvider =
        //            new NcRegistryKeyValueStorageProvider(tmpModuleSubKey);

        //        Dictionary<string, string> tmpProperties = new Dictionary<string, string>();

        //        tmpProperties.Add("Type", ((int)tmpModule.Type).ToString());
        //        tmpProperties.Add("ModuleID", tmpModule.ModuleID.ToString());
        //        tmpProperties.Add("ConnectionSetID", tmpModule.ConnectionSetID.ToString());
        //        tmpProperties.Add("MandantID", tmpModule.MandantID);
        //        tmpProperties.Add("SourceID", tmpModule.SourceID.ToString());
        //        tmpProperties.Add("LastOrderCall", tmpModule.LetzterAuftragsAbruf.ToString());
        //        if (tmpModule.BelegeAbrufenAb != DateTime.MinValue)
        //            tmpProperties.Add("TransferInvoicesSince", tmpModule.BelegeAbrufenAb.ToString());

        //        if (tmpModule.InvoiceTypes != null)
        //        {
        //            List<string> tmpActiveInvoiceNubers = tmpModule.InvoiceTypes.Where(i => i.IsChecked == true).Select(i => i.Nr).ToList();
        //            string tmpJoinedInvoiceNumbers = string.Join(";", tmpActiveInvoiceNubers);
        //            tmpProperties.Add("InvoiceTypes", tmpJoinedInvoiceNumbers);
        //        }


        //        foreach (KeyValuePair<string, string> tmpProperty in tmpProperties)
        //        {
        //            string tmpEncryptedValue = tmpDPAPICryptographyProvider.Encrypt(tmpProperty.Value);
        //            tmpRegistryKeyValueStorageProvider.SaveEntry(tmpProperty.Key, tmpEncryptedValue);
        //        }
        //    }
        //}

        #endregion


    }
}
