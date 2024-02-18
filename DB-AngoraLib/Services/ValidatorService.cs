﻿using DB_AngoraLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Services
{
    
    public class ValidatorService
    {
        private readonly Dictionary<Race, List<Color>> NotApprovedColorsByRace;

        public ValidatorService()
        {
            NotApprovedColorsByRace = new Dictionary<Race, List<Color>>();

            // Tilføj farver, der ikke er godkendt for hver race
            NotApprovedColorsByRace[Race.Angora] = new List<Color> { Color.Chinchilla, Color.Gouwenaar, Color.LyseBlå_BlåBeveren, Color.Ræverød_NewZealandRed, Color.Sallander, };
            NotApprovedColorsByRace[Race.Satinangora] = new List<Color> { Color.Chinchilla, Color.Gouwenaar, Color.LyseBlå_BlåBeveren, Color.Ræverød_NewZealandRed, Color.Sallander, };
            NotApprovedColorsByRace[Race.Satin] = new List<Color> { Color.Hvid, };
            NotApprovedColorsByRace[Race.Rex] = new List<Color> { Color.Vildtrød_Harefarvet, Color.Gulrød_Bourgogne, Color.Ræverød_NewZealandRed, Color.LyseBlå_BlåBeveren, Color.LilleEgern_Gråblå, Color.MarburgerEgern_Gråblå, Color.Gouwenaar, Color.Jerngrå, };
            NotApprovedColorsByRace[Race.Lille_Rex] = new List<Color> { Color.Vildtrød_Harefarvet, Color.Gulrød_Bourgogne, Color.Ræverød_NewZealandRed, Color.LyseBlå_BlåBeveren, Color.LilleEgern_Gråblå, Color.MarburgerEgern_Gråblå, Color.Gouwenaar, Color.Jerngrå, };
        }

        public bool IsColorValidForRace(Race race, Color color)
        {
            // Hent farver, der ikke er godkendt for den givne race
            if (NotApprovedColorsByRace.TryGetValue(race, out var notApprovedColors))
            {
                // Kontroller om den valgte farve er godkendt for racen
                return !notApprovedColors.Contains(color);
            }
            // Hvis racen ikke findes i NotApprovedColorsByRace, betragt farven som godkendt
            return true;
        }

        public bool ValidateLeftEarId(string leftEarId)
        {
            if (string.IsNullOrEmpty(leftEarId))
            {
                throw new ArgumentNullException("Kanin.Id: Skal have en værdi");
            }

            if (!int.TryParse(leftEarId, out int _))
            {
                throw new ArgumentException("Kanin.Id, skal være numerisk");
            }

            if (leftEarId.Length < 3 || leftEarId.Length > 4)
            {
                throw new ArgumentException($"Kanin.Id: Skal være imellem 3-4 numrer langt. Du har angivet {leftEarId.Length} cifre");
            }

            return true;
        }

        public bool ValidateRace(string race)
        {
            if (!Enum.TryParse(race, true, out Race _)) // true (gør små og store bogstaver ignoreres)
            {
                throw new ArgumentException($"Ugyldig race! Vælg fra listen");
            }

            return true;
        }

    }
}
