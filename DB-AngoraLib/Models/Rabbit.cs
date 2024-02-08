﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DB_AngoraLib.Models
{
    
    public enum IsPublic
    {
        Yes,
        No
    }

    public enum Gender
    {
        Male,
        Female
    }

    public enum Race
    {
        Angora,
        Belgisk_Hare,
        Belgisk_Kæmpe,
        Beveren,

        Hermelin,
        Hollænder,
        Hotot,
        Jamora,

        Lille_Chinchilla,
        Lille_Havana,
        Lille_Rex,
        Lille_Satin,

        Lux,
        Løvehoved,

        Rex,
        Sallander,
        Satin,
        Satinangora,

        Stor_Chinchilla,
        Stor_Havana,
    }

    public enum Color
    {
        // Vildtanlægfarver
        Vildtgrå,
        Jerngrå,
        Vildtsort,
        Vildtgul,
        Vildtbrun,
        Vildtblå_PerleEgern,
        Vildtrød_Harefarvet,
        Rødbrun_Gråblå_Lux,
        Gulrød_Bourgogne,
        Orange,
        Ræverød_NewZealandRed,
        Lutino,
        Lutino_Shadow,
        Chinchilla,
        Schwarzgrannen,

        // Ensfarvede
        Sort_Alaska,
        Blå,
        LyseBlå_BlåBeveren,
        LilleEgern_Gråblå,
        MarburgerEgern_Gråblå,
        Gouwenaar,
        Brun_Havana,
        Beige,
        Rødorange_Sachsengold,
        Hvid,

        // Ensfarvede m. slør
        Rødbrun_Madagascar,
        Gulbrun_Isabella,
        Sallander,

        // Ensfarvede m. stikkelhår
        Sølv,
        Stikkelhår_Trønder
    }

    public enum Pattern
    {
        Hotot,
        TyskSchecke_TKS_LTS,
        EngelskSchecke,
        Dalmatiner,
        Kappe,
        RhinskSchecke,
        Japaner,
        Hollænder,
        Tan_White_Otter,
        Russer,
        Zobel_Siameser,
    }

    public class Rabbit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Owner { get; set; }

        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Kanin.Id: Min 3 tal, Max 4 tal")]
        public string LeftEarId { get; set; }

        [RegularExpression(@"^\d{4}$", ErrorMessage = "Kanin.AvlerNo: Skal bestå af 4 tal!")]
        public string RightEarId { get; set; }

        public string? NickName { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public DateOnly? DateOfDeath { get; set; }

        public Gender? Gender { get; set; }

        public Race? Race { get; set; }

        public Color? Color { get; set; }

        public bool? ApprovedRaceColorCombination { get; set; } = true;

        public IsPublic? IsPublic { get; set; }


        public Rabbit(int id, string leftEarId, string rightEarId, string? owner, string? nickName, DateOnly dateOfBirth, DateOnly? dateOfDeath, Gender? gender, Race? race, Color? color, bool? approvedRaceColor, IsPublic? isPublic)
        {
            Id = id;
            LeftEarId = leftEarId;
            RightEarId = rightEarId;
            Owner = owner;
            NickName = nickName;
            DateOfBirth = dateOfBirth;
            DateOfDeath = dateOfDeath;
            Gender = gender;
            Race = race;
            Color = color;
            ApprovedRaceColorCombination = approvedRaceColor;
            IsPublic = isPublic;
        }
        public Rabbit() { }

        public void ValidateLeftEarId()
        {
            if (string.IsNullOrEmpty(LeftEarId))
            {
                throw new ArgumentNullException("NULL: Kanin.Id, skal udfyldes");
            }

            if (!int.TryParse(LeftEarId, out int _))
            {
                throw new ArgumentException("Kanin.Id, skal være numerisk");
            }

            if (LeftEarId.Length < 3 || LeftEarId.Length > 4)
            {
                throw new ArgumentException($"Kanin.Id, skal være imellem 3-4 numrer langt. Du har angivet {LeftEarId.Length} cifre");
            }
        }
    }
}
