using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMyMood
{
    class fiche
    {
        private int ID;
        private int DATE;
        private int HUMEUR;
        private int PHOTO;
        private string MESSAGE;
        private int ENBASE;

        public fiche(int maDATE, int monHUMEUR, int maPHOTO, string monMESSAGE, int monENBASE)
        {
            //ctor
            DATE = maDATE;
            HUMEUR = monHUMEUR;
            PHOTO = maPHOTO;
            MESSAGE = monMESSAGE;
            ENBASE = monENBASE;//présent dans la base
        }
        public fiche (int maDATE)
        {
            //ctor
            DATE = maDATE;
            HUMEUR = 5;
            PHOTO = 0;
            MESSAGE = "";
            ENBASE = 0;//non présent dans la base
        }

        public void setID(int monID)
        {
            ID = monID;
        }

        public int getHumeur()
        {
            return HUMEUR;
        }

        public string getMessage()
        {
            return MESSAGE;
        }

        public int getPhoto()
        {
            return PHOTO;
        }

        public int getEnbase()
        {
            return ENBASE;
        }

        public int getDate()
        {
            return DATE;
        }

    }
}
