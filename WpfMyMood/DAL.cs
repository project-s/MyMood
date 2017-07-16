using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfMyMood
{
    class DAL
    {
        static string connecBase = @"Data Source=Mood.db;Version=3;New=False;Compress=True;";

        public void initBase()
        {
            //connexion à la BD, si elle n'exite pas, elle sera créée
            SQLiteConnection sql_con = new SQLiteConnection(connecBase);
            sql_con.Open();

            //vérifier si la table existe //sinon on la crée
            string myRequest = "create table if not exists T_MOOD(ID INTEGER PRIMARY KEY AUTOINCREMENT, DATE INT, HUMEUR INT, PHOTO INT, MESSAGE TEXT);";
            SQLiteCommand cmd = new SQLiteCommand(myRequest, sql_con);

            cmd.ExecuteNonQuery();
            sql_con.Close();
        }
        public fiche initilisationFiche(int codeDate)
        {

            //connexion à la BD, si elle n'exite pas, elle sera créée
            SQLiteConnection sql_con = new SQLiteConnection(connecBase);
            sql_con.Open();

            //on vérifie si des données sont présentes à la date du jour
            string myRequest = "SELECT * FROM T_MOOD WHERE DATE = " + codeDate + ";";
            SQLiteCommand cmd = new SQLiteCommand(myRequest, sql_con);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows) //on vérifie sur le reader a retourné un résultat
            {//si oui, on crée une fiche avec les infos de la base

                rdr.Read();
                int photo;
                if (rdr.GetInt32(3) == 1)
                {
                    photo = 1;
                }
                else
                {
                    photo = 0;
                }

                fiche maFiche = new fiche(codeDate, rdr.GetInt32(2), photo, rdr.GetString(4),1);
                sql_con.Close();
                return maFiche;

            }//sinon le formulaire est déjà initialisé
            else
            {
                //si le codeDate n'est pas présent en base, on initialise une nouvelle fiche "vierge"
                fiche maFiche = new fiche(codeDate);
                sql_con.Close();
                return maFiche;

            }//finIf

        }

        public void ajouterFiche(fiche maFiche)
        {
            //connexion à la BD, si elle n'exite pas, elle sera créée
            SQLiteConnection sql_con = new SQLiteConnection(connecBase);
            sql_con.Open();

            if (maFiche.getEnbase()==1)
            {
                //les données sont en base, on fait un update
                string myRequest = "UPDATE T_MOOD SET HUMEUR = "+ maFiche.getHumeur() + ", PHOTO = "+maFiche.getPhoto() +", MESSAGE = @Param WHERE DATE = " + maFiche.getDate() + ";";
                SQLiteCommand cmd = new SQLiteCommand(myRequest, sql_con);
                //création/ajout du paramètre SQL                
                cmd.Parameters.Add(new SQLiteParameter("@Param", maFiche.getMessage()));
                //éxécution de la requete
                cmd.ExecuteNonQuery();
            }
            else
            {
                //les données ne sont pas en base, on fait un insert
                string myRequest = "INSERT INTO T_MOOD(DATE, HUMEUR, PHOTO, MESSAGE) VALUES "
                        + "(" + maFiche.getDate() + "," + maFiche.getHumeur() + "," + maFiche.getPhoto() + ",@Param);";
                SQLiteCommand cmd = new SQLiteCommand(myRequest, sql_con);
                //création/ajout du paramètre SQL                
                cmd.Parameters.Add(new SQLiteParameter("@Param", maFiche.getMessage()));
                //éxécution de la requete
                cmd.ExecuteNonQuery();
            }

            sql_con.Close();
        }

        public void CreationPDF(string DateCourte)
        {
            
            SQLiteConnection sql_con = new SQLiteConnection(connecBase);
            sql_con.Open();

            int dateMin = int.Parse(DateCourte + "01");
            int dateMax = int.Parse(DateCourte + "31");
            string myRequest = "SELECT * FROM T_MOOD WHERE DATE BETWEEN " + dateMin + " AND " + dateMax + " ORDER BY DATE;";

            //exécuter la requete
            SQLiteCommand cmd = new SQLiteCommand(myRequest, sql_con);

            //utilisation d'un reader
            SQLiteDataReader rdr = cmd.ExecuteReader();

            //PDF            
            string fichier = DateCourte + ".pdf";
            FileStream fs = new FileStream(fichier, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = new Document();
            PdfWriter.GetInstance(doc, fs);
            
                //on vérifie que le reader n'est pas vide
                if (rdr.HasRows)
                {
                    //init du pdf
                    doc.Open();

                    //iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4.Rotate());
                    string ParagraphePdf = "";
                    while (rdr.Read())
                    {
                        //début de l'écriture dans le PDF
                        //PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                        //doc.Open();
                        //ajout des lignes
                        ParagraphePdf = "";
                        ParagraphePdf = rdr.GetString(1);
                        ParagraphePdf = ParagraphePdf + " : humeur " + rdr.GetString(2) + " : \n";
                        ParagraphePdf = ParagraphePdf + rdr.GetString(4) + "\n\n";
                        ParagraphePdf = ParagraphePdf + "********************************************************************** \n\n";

                        doc.Add(new iTextSharp.text.Paragraph(ParagraphePdf));

                        //old
                        //doc.Add(new iTextSharp.text.Paragraph(richTextBox1.Text));
                        //string test;
                        //test = "****************************************************************";
                        //doc.Add(new iTextSharp.text.Paragraph(test));
                        //test = "17/03/2017 : ";
                        //test = test + "n°5";
                        //test = test + "\n" + "résumé : ";
                        //test = test + "blablablabla";
                        //doc.Add(new iTextSharp.text.Paragraph(test));
                        //fin old               

                        //fin de la partie PDF

                    } //finwhile

                    doc.Close();
                    rdr.Close();
                    sql_con.Close();
            } //finIfHasRows
        }

        public void CheckTXT(string DateCourte)
        {
            //TXT création/initiation du fichier texte
            string fichier = DateCourte + ".doc";

            FileInfo monFichier = new FileInfo(@"C:\Mood\" + fichier);
            // on efface l'ancien s'il existe déjà
            if (monFichier.Exists)
            {
                monFichier.Delete();
            }

        }

        private static void AddText(FileStream fs, string value)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }

        public void CreationTXT(string DateCourte)
        {

            SQLiteConnection sql_con = new SQLiteConnection(connecBase);
            sql_con.Open();

            int dateMin = int.Parse(DateCourte + "01");
            int dateMax = int.Parse(DateCourte + "31");
            string myRequest = "SELECT * FROM T_MOOD WHERE DATE BETWEEN " + dateMin + " AND " + dateMax + " ORDER BY DATE;";

            //exécuter la requete
            SQLiteCommand cmd = new SQLiteCommand(myRequest, sql_con);

            //utilisation d'un reader
            SQLiteDataReader rdr = cmd.ExecuteReader();

            //TXT création/initiation du fichier texte
            string fichier = DateCourte + ".doc";
            
            /*
            FileInfo monFichier = new FileInfo(@"C:\Mood\" + fichier);
            // on efface l'ancien s'il existe déjà
            if (monFichier.Exists)
            { 
                monFichier.Delete();
            }*/
                            
            
            FileStream wFile = new FileStream(@"C:\Mood\" + fichier, FileMode.Append);

            //StreamWriter monStreamWriter = new StreamWriter(@"C:\Mood\" + fichier);
            //on vérifie que le reader n'est pas vide
            if (rdr.HasRows)
            {

                while (rdr.Read())
                {
                    //début de l'écriture dans le DOC
                    //ajout des lignes

                    //Première ligne
                    string tempTxt = "" + rdr.GetInt32(1) + " : humeur " + rdr.GetInt32(2) + " : \n";
                    AddText(wFile, tempTxt);

                    //Première ligne: souligné
                    tempTxt = "---------------------\n";
                    AddText(wFile, tempTxt);

                    //ligne avec le résumé de la journée
                    tempTxt = rdr.GetString(4);
                    AddText(wFile, tempTxt);

                    //fin de paragraphe
                    tempTxt = "\n**********************************************************************\n\n\n";
                    AddText(wFile, tempTxt);           

                    //fin de la partie DOC

                } //finwhile

                // Fermeture du StreamWriter (Très important) 
                wFile.Close();
                //Fermeture du reader
                rdr.Close();
                //Fermeture de la connc SQL
                sql_con.Close();
            } //finIfHasRows
        }
    }
}
