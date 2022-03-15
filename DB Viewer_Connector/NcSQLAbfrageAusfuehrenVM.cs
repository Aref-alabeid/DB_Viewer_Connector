using System;
using System.Reflection;
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
using System.Linq;

namespace DB_Viewer_Connector
{
    public class NcSQLAbfrageAusfuehrenVM : NcBindableBase
    {

        #region Member

        string mAbfrageSchablone = "Dat >= '01.03.2022' and Art=70 or Art=71 or Art=90";
        AutoDataSetInfos tmpTabellenNamen = null;
        Application mMicrotechApplication = null;
        DataView mDBData = null;
        NcMicrotechCompany mSelectedCompany = null;
        string mSelectedTabelle = "Vorgang";
        Dispatcher mDispatcherObject = null;
        string mAbfrage = "Dat >= 'dd.MM.yyyy' and Art=70 or Art=71 or Art=90";
        private DateTime mAbrufDatum = DateTime.Now;
        private string mAbfrageText = null;
        private string mSelectedAnzahl = "50";

        #endregion

        #region Properties

        public ObservableCollection<NcMicrotechCompany> Companies { get; set; }
        public ObservableCollection<string> AbfrageSchablone { get; set; }
        public List<string> EintraegeAnzahl { get; set; }
        public List<string> TabellenNamen { get; set; }
        public List<OrdersTyps> OrdersTyps { get; set; }
        public string AbfrageText
        {
            get { return mAbfrageText; }
            set { mAbfrageText = value; OnPropertyChanged("SQLAbfrageText"); }
        }
        public DateTime AbrufDatum
        {
            get { return mAbrufDatum; }
            set { mAbrufDatum = value; OnPropertyChanged("AbrufDatum"); }
        }
        public string Abfrage
        {
            get { return mAbfrage; }
            set { mAbfrage = value; OnPropertyChanged("SQLAbfrage"); }
        }
        public DataView DBData
        {
            get { return mDBData; }
            set
            {
                mDBData = value;
                OnPropertyChanged("DBData");
            }
        }
        public string SelectedTabelle
        {
            get
            {
                return mSelectedTabelle;
            }
            set
            {
                SetProperty(ref mSelectedTabelle, value);
            }
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
        public string SelectedAnzahl
        {
            get { return mSelectedAnzahl; }
            set { mSelectedAnzahl = value; OnPropertyChanged("SelectedAnzahl"); }
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

        #endregion

        #region Costructor

        public NcSQLAbfrageAusfuehrenVM(string pFirmaName, string pBenutzer, string pPasswort)
        {

            EintraegeAnzahl = new List<string>();
            EintraegeAnzahl.Add("Alle");EintraegeAnzahl.Add("500"); EintraegeAnzahl.Add("50");
            OrdersTyps = new List<OrdersTyps>();
            DBData = new DataView();
            mDispatcherObject = Dispatcher.CurrentDispatcher;
            Companies = new ObservableCollection<NcMicrotechCompany>();
            AbfrageSchablone = new ObservableCollection<string>();
            AbfrageSchablone.Add("Dat >= '01.03.2022'"); AbfrageSchablone.Add("Art=70 or Art=71 or Art=90");
            AbfrageSchablone.Add("Dat >= '01.03.2022' and Art=70 or Art=71 or Art=90");
            AbfrageSchablone.Add("Art = '10' OR Art = '15' OR Art = '20' OR Art = '30' OR Art = '35' OR Art = '36' OR Art = '50' OR Art = '70' " +
                "OR Art = '71' OR Art = '60' OR Art = '90' OR Art = '72' OR Art = '81' OR Art = '79' OR Art = '80' OR Art = '85' OR Art = '91' " +
                "OR Art = '40' OR Art = '41' OR Art = '92' OR Art = '95' OR Art = '101' OR Art = '102' OR Art = '103' OR Art = '104' OR Art = '105' " +
                "OR Art = '106' OR Art = '107' OR Art = '108' OR Art = '109' OR Art = '110' ");
           
            mMicrotechApplication = new Application();
            GetCompanies(pFirmaName, pBenutzer, pPasswort);
            
            AbfrageAusfuehrenCommand = new NcCommand(async () => { GetDataFromMicrotechDB(); },
                () =>
                {
                    return true;
                });

            var MandantenListe = GetMicrotechCompanies();
            MicrotechApplication.SelectMand(MandantenListe[0].MandantenNr);
            GetOrdersTyps();
            tmpTabellenNamen = MicrotechApplication.DataSetInfos;
            TabellenNamen = new List<string>();
            for (int i = 0; i < tmpTabellenNamen.Count; i++)
            {
                TabellenNamen.Add(tmpTabellenNamen[i].Name);
            }
            System.Windows.Application.Current.MainWindow.Visibility = Visibility.Hidden;
        }

        #endregion

        public ICommand AbfrageAusfuehrenCommand { get; set; }

        #region Methods

        void GetDatFromDatabase()
        {
            try
            {
                String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TESTDataBase;Integrated Security=True;Connect Timeout=30;Encrypt=False;" +
                        "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                SqlConnection con = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand(Abfrage, con);
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
                Abfrage = "Bitte eine gültige Abfrage ausführen";

            }
        }

        public List<OrdersTyps> GetOrdersTyps()
        {
            AutoDataSet tmpDataSet = MicrotechApplication.DataSetInfos["VorgangArten"].CreateDataSet();
            List<OrdersTyps> tmpOrdersTyps = new List<OrdersTyps>();
            OrdersTyps.Clear();
            tmpDataSet.First();
            while (tmpDataSet.Eof == false)
            {
                OrdersTyps tmpAuftragsTyp = new OrdersTyps() { Nr = tmpDataSet.Fields["Nr"].AsString, Bezeichnung = tmpDataSet.Fields["Bez"].AsString };
                tmpOrdersTyps.Add(tmpAuftragsTyp);
                tmpDataSet.Next();
            }
            OrdersTyps.AddRange(tmpOrdersTyps);
            foreach (var item in tmpOrdersTyps)
            {
                if (item.Nr == "70" || item.Nr == "71" || item.Nr == "90")
                    item.IsChecked = true;
            }
            return tmpOrdersTyps;
        }
        
        async void GetDataFromMicrotechDB()
        {
           
            IAutoDataSet12 tmpVorgangADS = MicrotechApplication.DataSetInfos[SelectedTabelle].CreateDataSet();

            try
            {
                if (AbfrageText != "" && AbfrageText != null)
                {
                    tmpVorgangADS.Filter = AbfrageText;
                    tmpVorgangADS.Filtered = true; 
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Bitte eine richtige Abfrage eingeben");
                return;
            }

            DataTable dt = new DataTable();
            List<string> FelderNamenList = new List<string>();
            List<object> FelderWerteList = new List<object>();

            for(int i = 0; i < tmpVorgangADS.Fields.Count; i++)
            {
                var FeldName = tmpVorgangADS.Fields[i].Name;
                FelderNamenList.Add(FeldName);
                dt.Columns.Add(FeldName);
            }
            int tmpSelectedAnzahl = 0;
            if (SelectedAnzahl == "50" || SelectedAnzahl =="500")
                tmpSelectedAnzahl = Convert.ToInt32(SelectedAnzahl);
           
            tmpVorgangADS.First();

            while (tmpVorgangADS.Eof == false)
            {
                var row = dt.NewRow();
                for (int i = 0; i < tmpVorgangADS.Fields.Count; i++)
                {
                    var FeldWert = tmpVorgangADS.Fields[i];
                    row[FeldWert.Name] = FeldWert.Value;
                    FelderWerteList.Add(FeldWert);
                }
                dt.Rows.Add(row);
                if (dt.Rows.Count == tmpSelectedAnzahl)
                {
                    DBData = dt.DefaultView;
                    return;
                }   
                
                tmpVorgangADS.Next();
            }

            //DataTable ListAsDataTable = BuildDataTable<NcPayJoeBeleg>(tmpBelegList);
            
            DBData = dt.DefaultView;

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


            }
            catch (Exception pEx)
            {
                throw new Exception("Fehler beim Auslesen der Firmen aus der Microtech Datenbank", pEx);
            }

           
            
            return tmpMandantenListe;

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

            //MicrotechApplication.LogOff();
        }

        #endregion
    }
}
