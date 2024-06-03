﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DB_AngoraLib.Services.ValidationService;

namespace DB_AngoraLib.Models
{
    public enum ForSale
    {
        Nej,
        Ja
    }   

    public enum Gender
    {
        Hun,    // NiceToHave: ♂    0.1
        Han,    // NiceToHave: ♀    1.0
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
        Rødbrungråblå_Lux,
        Gulrød_Bourgogne,
        Orange,
        Ræverød_NewZealandRed,
        Lutino_Vildtgul,
        Lutino_Shadow,
        Chinchilla,
        Schwarzgrannen,

        // Ensfarvede
        Sort_Alaska,
        Blå,
        LyseBlå_BlåBeveren,
        Gråblå_LilleEgern,
        Gråblå_MarburgerEgern,
        LysGråblå_Gouwenaar,
        Brun_Havana,
        Beige,
        Rødorange_Sachsengold,
        Hvid_Albino,            
        Hvid_Blåøjet,

        // Ensfarvede m. slør
        Rødbrun_Madagascar,
        Kanel,
        Gulbrun_Isabella,
        Sallander,


        // Ensfarvede m. stikkelhår
        Sølv,
        Stikkelhår_Trønder,

        //Special farver
        Elfenben = Hvid_Albino
    }

    public enum Pattern
    {
        Hotot,
        TyskSchecke_TKS_LTS,
        EngelskSchecke,
        RhinskSchecke,
        Dalmatiner,
        Kappe,
        Japaner,
        Hollænder,
        Tan_White_Otter,
        Russer,
        Zobel_Siameser,
    }

    public class Rabbit
    {
        //public int Id { get; set; }
        public string EarCombId { get; set; }

        public string RightEarId { get; set; }
        public string LeftEarId { get; set; }


        public string? OwnerId { get; set; }
        public User? User { get; set; } // public virtual -> lazy loading (færre DB requests)

        public string? NickName { get; set; }
        public Race Race { get; set; }
        public Color Color { get; set; }

        public bool ApprovedRaceColorCombination
        {
            get
            {
                if (RabbitValidator.NotApprovedColorsByRace.TryGetValue(Race, out var notApprovedColors))
                {
                    return !notApprovedColors.Contains(Color);
                }
                else
                {
                    return false;
                }
            }
        }

        public DateOnly DateOfBirth { get; set; }
        public bool IsJuvenile
        {
            get
            {
                var ageInWeeks = (DateTime.Now.Date - DateOfBirth.ToDateTime(TimeOnly.MinValue)).TotalDays / 7;
                return ageInWeeks >= 8 && ageInWeeks <= 14;
            }
        }
        public DateOnly? DateOfDeath { get; set; }
        
        public Gender Gender { get; set; }
        public ForSale? ForSale { get; set; }

        public string? Father_EarCombId { get; set; }   // TODO: Bør være en reference til en Rabbit?
        public virtual Rabbit? Father { get; set; }

        public string? Mother_EarCombId { get; set; }
        public virtual Rabbit? Mother { get; set; }


        //public virtual ICollection<Rating> Ratings { get; set; }
        //public virtual ICollection<RabbitParents> Parents { get; set; }
        //public virtual ICollection<RabbitParents> MotheredChildren { get; set; } // Er altid null hvis, Rabbit er far/Han
        //public virtual ICollection<RabbitParents> FatheredChildren { get; set; } // Er altid null hvis, Rabbit er mor/Hun
        public virtual ICollection<Photo> Photos { get; set; }


        public Rabbit(string rightEarId, string leftEarId, string? ownerId, string? nickName, Race race, Color color, DateOnly dateOfBirth, DateOnly? dateOfDeath, Gender gender, ForSale? forSale, string father_EarCombId, string mother_EarCombId/*, string? fatherRightEarId, string? fatherLeftEarId, string? motherRightEarId, string? motherLeftEarId*/)
        {
            //Id = id;
            EarCombId = $"{rightEarId}-{leftEarId}";

            RightEarId = rightEarId;
            LeftEarId = leftEarId;
            OwnerId = ownerId;
            NickName = nickName;
            Race = race;
            Color = color;
            DateOfBirth = dateOfBirth;
            DateOfDeath = dateOfDeath;
            Gender = gender;

            ForSale = forSale;

            Father_EarCombId = father_EarCombId;
            Mother_EarCombId = mother_EarCombId;


            //FatherRightEarId = fatherRightEarId;
            //FatherLeftEarId = fatherLeftEarId;
            //MotherRightEarId = motherRightEarId;
            //MotherLeftEarId = motherLeftEarId;
            //FatherId = fatherId;
            //MotherId = motherId;

        }

        public Rabbit() { }

        
    }
}
