using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using BpNT;
using NetConnections.PayJoe.Connector.Client.General.ViewModel;
using NetConnections.PayJoe.Connector.Client.GUI.ViewModels;
using Application = BpNT.Application;

namespace DB_Viewer_Connector
{
    public class NcSQLAbfrageAusfuehrenVM : NcBindableBase
    {

        #region Member

        Application mMicrotechApplication = null;
        DataView mDBData = null;
        NcMicrotechCompany mSelectedCompany = null;
        Dispatcher mDispatcherObject = null;
        public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        #endregion

        private string mSQLAbfrage = "";

        public NcSQLAbfrageAusfuehrenVM( string pFirmaName, string pBenutzer, string pPasswort)
        {
            DBData = new DataView();
            mDispatcherObject = Dispatcher.CurrentDispatcher;
            Companies = new ObservableCollection<NcMicrotechCompany>();
            
            mMicrotechApplication = new Application();
            GetCompanies(pFirmaName, pBenutzer, pPasswort);

            SQLAbfrageAusfuehrenCommand = new NcCommand(async () => { GetDataFromMicrotechDB(); },
                () =>
                {
                    return true;
                });

            var MandantenListe = GetMicrotechCompanies();
            MicrotechApplication.SelectMand(MandantenListe[0].MandantenNr);


            //GetDataFromMicrotechDB();
        }
        public string SQLAbfrage
        {
            get { return mSQLAbfrage; }
            set { mSQLAbfrage = value; OnPropertyChanged("SQLAbfrage"); }
        }

        public DataView DBData
        {
            get { return mDBData; }
            set { mDBData = value; 
                OnPropertyChanged("DBData"); }
        }

        public NcMicrotechCompany SelectedCompany
        {
            get
            {
                return mSelectedCompany;
            }
            set
            {
                SetProperty(ref mSelectedCompany, value);
            }
        }
        public ObservableCollection<NcMicrotechCompany> Companies { get; set; }
        void GetDatFromDatabase()
        {
            try
            {
                String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TESTDataBase;Integrated Security=True;Connect Timeout=30;Encrypt=False;" +
                        "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                SqlConnection con = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(SQLAbfrage, con);
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                DBData = dt.DefaultView;
                cmd.Dispose();
                con.Close();
            }
            catch (Exception)
            {
                SQLAbfrage = "Bitte eine gültige SQL-Abfrage ausführen";

            }
        }

        

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
        public ICommand SQLAbfrageAusfuehrenCommand { get; set; }

        async void GetDataFromMicrotechDB()
        {
            

            List<NcPayJoeBeleg> tmpBelegList = new List<NcPayJoeBeleg>();

            IAutoDataSet12 tmpVorgangADS = MicrotechApplication.DataSetInfos["Vorgang"].CreateDataSet();
            IAutoDataSet12 tmpVorgangArchivADS = MicrotechApplication.DataSetInfos["VorgangArchiv"].CreateDataSet();

            try
            {
                tmpVorgangADS.Filter = SQLAbfrage;
                tmpVorgangADS.Filtered = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Bitte eine richtige Abfrage eingeben");
                return;
            }

            tmpVorgangADS.First();
            while (tmpVorgangADS.Eof == false)
            {
                NcPayJoeBeleg tmpBeleg = new NcPayJoeBeleg();
                string tmpVorgangsart = tmpVorgangADS.Fields["Art"].AsString;
                string tmpBelegNr = tmpVorgangADS.Fields["BelegNr"].AsString;
                double tmpBruttoBetrag = tmpVorgangADS.Fields["GPreisBt"].AsFloat;
                string tmpWaehrung = tmpVorgangADS.Fields["Waehr"].AsString;
                DateTime tmpBelegDatum = tmpVorgangADS.Fields["Dat"].AsDateTime;
                string tmpZahlungsart = tmpVorgangADS.Fields["ZahlArt"].AsString;
                string tmpEmail = tmpVorgangADS.Fields["ReEMail1"].AsString;
                string tmpAuftragsNr = tmpVorgangADS.Fields["AuftrNr"].AsString;
                string tmpKundennummer = tmpVorgangADS.Fields["AdrNr"].AsString;
                string tmpDebitorenNr = tmpVorgangADS.Fields["BKtoNr"].AsString;
                string tmpFirma = tmpVorgangADS.Fields["ReNa2"].AsString;
                string tmpName = tmpVorgangADS.Fields["ReNa3"].AsString;

                tmpBeleg.BelegBetrag = tmpBruttoBetrag;
                if (tmpBeleg.BelegBetrag < 0)
                    tmpBeleg.Belegtyp = "Gutschrift";
                else
                    tmpBeleg.Belegtyp = "Rechnung";
                tmpBeleg.BelegNummer = tmpBelegNr;
                tmpBeleg.BelegWaehrung = "EUR";
                tmpBeleg.Belegdatum = tmpBelegDatum;
                tmpBeleg.ZahlungsArt = tmpZahlungsart;
                tmpBeleg.BelegEmail = tmpEmail;
                tmpBeleg.BelegFirma = tmpFirma;
                tmpBeleg.BelegExterneBestellNr = tmpAuftragsNr;
                tmpBeleg.BelegKundenNr = tmpKundennummer;
                tmpBeleg.BelegDebitorenNr = tmpDebitorenNr;

                tmpBelegList.Add(tmpBeleg);

                tmpVorgangADS.Next();
            }

            // populate list
            DataTable ListAsDataTable = BuildDataTable<NcPayJoeBeleg>(tmpBelegList);
            
            DBData = ListAsDataTable.DefaultView;

            //tmpVorgangArchivADS.First();
            //while (tmpVorgangArchivADS.Eof == false)
            //{
            //}

            //VerbindungBeenden();
        }
        public static DataTable BuildDataTable<T>(IList<T> lst)
        {
            DataTable tbl = CreateTable<T>();
            Type entType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entType);
            foreach (T item in lst)
            {
                DataRow row = tbl.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item);
                }
                tbl.Rows.Add(row);
            }
            return tbl;
        }

        private static DataTable CreateTable<T>()
        {
            Type entType = typeof(T);
            DataTable tbl = new DataTable(entType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entType);
            foreach (PropertyDescriptor prop in properties)
            {
                tbl.Columns.Add(prop.Name, prop.PropertyType);
            }
            return tbl;
        }

        public List<NcMicrotechCompany> GetMicrotechCompanies()
        {

            AutoDataSet tmpMandanten = MicrotechApplication.MandList;
            List<NcMicrotechCompany> tmpMandantenListe = new List<NcMicrotechCompany>();

            try
            {
                tmpMandanten.First();

                while (tmpMandanten.Eof == false)
                {
                    NcMicrotechCompany tmpMandant = new NcMicrotechCompany();

                    tmpMandant.MandantenNr = tmpMandanten.Fields["MandNr"].AsString;
                    tmpMandant.Name = tmpMandanten.Fields["Namen"].AsString;

                    tmpMandantenListe.Add(tmpMandant);

                    tmpMandanten.Next();
                }

                return tmpMandantenListe;

            }
            catch (Exception pEx)
            {
                throw new Exception("Fehler beim Auslesen der Firmen aus der Microtech Datenbank", pEx);
            }

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



        private async Task GetCompanies(string pFirmaName, string pBenutzer, string pPasswort)
        {

            if (mDispatcherObject.Thread != Thread.CurrentThread)
            {
                await mDispatcherObject.InvokeAsync(async () => { await GetCompanies(pFirmaName, pBenutzer, pPasswort); });
                return;
            }
            mMicrotechApplication.Init(pFirmaName, "", pBenutzer, pPasswort);
            List<NcMicrotechCompany> tmpCompanies = GetMicrotechCompanies();
            Companies.Clear();

            foreach (NcMicrotechCompany tmpCompany in tmpCompanies)
                Companies.Add(tmpCompany);

            if (tmpCompanies != null && tmpCompanies.Count > 0)
                SelectedCompany = tmpCompanies[0];

        }

    }
}
