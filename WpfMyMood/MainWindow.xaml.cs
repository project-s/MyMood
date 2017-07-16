using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Sql;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using Microsoft.Win32;
using iTextSharp.text.pdf;

namespace WpfMyMood
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int codeDate;
        public MainWindow()
        {
            InitializeComponent();
            lblInfo.Foreground = Brushes.Red;
            lblInfo.Content = "Salut baby!";
            codeDate = int.Parse(DateTime.Now.Year.ToString("00") + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00"));
            DAL maDal = new DAL();
            fiche maFiche;
            maDal.initBase();
            maFiche = maDal.initilisationFiche(codeDate);

            //on rempli le formulaire
            Curseur.Value = maFiche.getHumeur();
            textBox.Text = maFiche.getMessage();
            if (maFiche.getPhoto()==1)
            {
                checkBox.IsChecked = true;
            }
            else
            {
                checkBox.IsChecked = false;
            }

            //debug
            lblcode.Content = codeDate;
            lblCurseur.Content = maFiche.getHumeur();

        }


    private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        //debug
        lblCurseur.Content = (int)Curseur.Value;
    }

    private void Calendar_SelectedDatesChanged(object sender,
        SelectionChangedEventArgs e)
    {
        // ... Get reference.
        var calendar = sender as Calendar;

        // ... See if a date is selected.
        if (calendar.SelectedDate.HasValue)
        {
            // ... Display SelectedDate in Title.
            DateTime date = calendar.SelectedDate.Value;
            lblDate.Content = date.ToShortDateString();
            lblcode.Content = date.Year.ToString("00") + date.Month.ToString("00") + date.Day.ToString("00");
            //lblcode.Content = codeDate;

            //appeler AfficherInfoDate();
        }
            lblInfo.Content = "";
            codeDate = int.Parse(lblcode.Content.ToString());
        //int codeDate = int.Parse(date.Year.ToString("00") + date.Month.ToString("00") + date.Day.ToString("00"));

        DAL maDal = new DAL();
        fiche maFiche;
        maFiche = maDal.initilisationFiche(codeDate);


        //on rempli le formulaire
        Curseur.Value = maFiche.getHumeur();
        textBox.Text = maFiche.getMessage();
        if (maFiche.getPhoto() == 1)
        {
            checkBox.IsChecked = true;
        }
        else
        {
            checkBox.IsChecked = false;
        }
        }

    private void btAuj_Click(object sender, RoutedEventArgs e)
    {
        calendarJour.DisplayDate = DateTime.Today;
        calendarJour.SelectedDate = DateTime.Today;
        calendarJour.DisplayMode = CalendarMode.Month;
    }

    private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
    {
        if (textBox.Text != "")
        { 
            DAL maDal = new DAL();
            codeDate = int.Parse(lblcode.Content.ToString());
            fiche maFiche;
            //initialisation de la fiche
            maFiche = maDal.initilisationFiche(codeDate);

            int photo = 0;
            if (checkBox.IsChecked == true)
            {
                photo = 1;
            }
            int enbase = maFiche.getEnbase();
               // int.Parse(Curseur.Value.ToString())
            maFiche = new fiche(codeDate, Convert.ToInt32(Curseur.Value), photo, textBox.Text, enbase);
            maDal.ajouterFiche(maFiche);
                lblInfo.Content = "Sauvegarde réussie";
            }
        else
        {
            lblInfo.Content = "Pas de texte rempli";
        }
    }



        private void AfficherInfoDate()
        {
            //ouvrir la connexion à la base

            //vérifier si la date existe en base
            //si oui, on affiche les infos
        }

        private void btDoc_Click_1(object sender, RoutedEventArgs e)
        {

            //debug
            lblInfo.Content = "DOC cliqué";
            DAL maDal = new DAL();          
            //finDebug

            //gestion de la date sélectionnée
            string DateCourte = lblcode.Content.ToString();
            DateCourte = DateCourte.Substring(0, 6);
            //maDal.CreationPDF(DateCourte);
            maDal.CheckTXT(DateCourte);
            maDal.CreationTXT(DateCourte);

        }
    }
}
