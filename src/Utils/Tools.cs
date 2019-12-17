using System;
using System.Collections.Generic;
using System.IO;
using GoRogue.DiceNotation;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace StarsHollow.Utils
{
    public static class Tools
    {

        public static class RandomNumber
        {
            static Random random = new Random();
            public static int GetRandomInt(int min, int max)
            {
                return random.Next(min, max);
            }
        }

        public static string LoadJson(string file)
        {
            using (StreamReader r = new StreamReader(@"../../../res/json/" + file))
            {
                string json = r.ReadToEnd();
                return json;
            }
        }


        public static class Dirs
        {
            public static Point NW = new Point(-1, -1);
            public static Point SW = new Point(-1, 1);
            public static Point W = new Point(-1, 0);
            public static Point NE = new Point(1, -1);
            public static Point SE = new Point(1, 1);
            public static Point E = new Point(1, 0);
            public static Point S = new Point(0, 1);
            public static Point N = new Point(0, -1);
        }

        public static Point GetRandomDir()
        {
            Point dir = new Point(0, 0);
            switch (Dice.Roll("1d9"))
            {
                case 1:
                    dir = Dirs.NW;
                    break;
                case 2:
                    dir = Dirs.SW;
                    break;
                case 3:
                    dir = Dirs.W;
                    break;
                case 4:
                    dir = Dirs.NE;
                    break;
                case 5:
                    dir = Dirs.SE;
                    break;
                case 6:
                    dir = Dirs.E;
                    break;
                case 7:
                    dir = Dirs.S;
                    break;
                case 8:
                    dir = Dirs.N;
                    break;
                case 9:
                    dir = new Point(0, 0);
                    break;
            }
            return dir;
        }
        public static string NameGenerator()
        {
            List<string> names;
            int nameCount;
            names = new List<string>(new string[] {"Ahti", "Aikamieli", "Aikio", "Airikka", "Ampuja", "Ano", "Arijoutsi", "Armas", "Arpia",
            "Asikka", "Auvo", "Hellikki", "Himottu", "Hirvas", "Hirvi", "Hopea", "Hyvälempi", "Hyväneuvo",
            "Hyväpaulo", "Hyväri", "Hyvätty", "Ihalempi", "Ihamuoto", "Ikitiera", "Ikopäivä", "Ikäheimo",
            "Ikävalko", "Ilakka", "Ilma", "Ilmari", "Ilmatoivia", "Janakka", "Jouko", "Jousia", "Joutsi",
            "Joutsimies", "Jurva", "Jutikka", "Kainu", "Kaivas", "Kaivattu", "Kaipia", "Kalamies", "Kalervo",
            "Kallas", "Kalpio", "Kare", "Kauko", "Kaukomieli", "Kaukovalta", "Keiho", "Keihäri", "Kekko", "Kokko",
            "Koira", "Kontio", "Kostia", "Kotarikko", "Koveri", "Kullervo", "Kulta", "Kultamies", "Kultimo",
            "Kukurtaja", "Kupias", "Kurikka", "Kuutamo", "Kylli", "Kyllikki", "Lalli", "Laso", "Laulaja",
            "Leinikka", "Leino", "Lemmikki", "Lemmitty", "Lemmäs", "Lempiä", "Lempo", "Liekko", "Lyylikki",
            "Löyliä", "Maanavilja", "Mainikki", "Meri", "Meripäivä", "Metso", "Meurakas", "Mieho", "Miekka",
            "Miela", "Mielenpito", "Mielikki", "Mielipäivä", "Mielitty", "Mielivalta", "Mielo", "Miemo",
            "Mietti", "Montaja", "Neuvo", "Nousia", "Nuolia", "Ora", "Osma", "Osmo", "Otava", "Otra", "Paasia",
            "Paaso", "Pekka", "Pekko", "Pellervo", "Peuro", "Puukko", "Päiviä", "Päivälapsi", "Päivö", "Rahikka",
            "Raita", "Rasantaja", "Rautia", "Repo", "Salme", "Sarijoutsi", "Satatieto", "Sauvo", "Seppo", "Soini",
            "Sotamieli", "Sotia", "Sotijalo", "Susi", "Suvi", "Säisä", "Talvikki", "Tammi", "Tapatora", "Tapavaino",
            "Tapo", "Tenho", "Terhi", "Terhikki", "Tiera", "Tietävä", "Toiva", "Toivas", "Toivelempi", "Toivettu",
            "Toivikki", "Toivio", "Toivottu", "Torio", "Tornia", "Tornio", "Tuiretuinen", "Tulo", "Tuntia", "Tuomikki",
            "Turo", "Tuuli", "Tuulikki", "Ukko", "Unaja", "Untamo", "Unti", "Unto", "Urho", "Urja", "Uro", "Usma",
            "Utujoutsi", "Vaania", "Vaino", "Vaito", "Valo", "Valta", "Valtari", "Vartia", "Vasara", "Venemies",
            "Vesa", "Vesi", "Vesivalo", "Vihas", "Vihavaino", "Vilja", "Viljakka", "Viljari", "Viljo", "Viti",
            "Väinä", "Väinö", "Väkimieli", "Väkiä", "Yijä", "Yö", "Äijäpäivä", "Äijö", "Äiniö"});

            nameCount = names.Count;

            //Draw a name from names[]
            string rollName()
            {
                int nameIndex;
                string roll = "1d" + (nameCount - 1);
                nameIndex = Dice.Roll(roll);
                return names[nameIndex];
            }

            string GenerateName()
            {
                string name = "";
                name = rollName();
                return name;
            }
            return GenerateName();
        }
    }
}





