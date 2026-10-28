using CapitalBraineAndSpine.Services.Infrastructures.Enums;

namespace CapitalBraineAndSpine.Services.Infrastructures.Helpers
{
    public static class CommonHelper
    {

        public static string GetRelationshipToInsuredText(string Abbr)
        {
            string name;

            switch (Abbr)
            {
                case "C":
                    name = RelationshipToInsured.Child.ToString();
                    break;
                case "O":
                    name = RelationshipToInsured.Other.ToString();
                    break;
                case "S":
                    name = RelationshipToInsured.Self.ToString();
                    break; ;
                case "U":
                    name = RelationshipToInsured.Spouse.ToString();
                    break;
                default:
                    name = "Child";
                    break;
            }

            return name;
        }
        public static string GetGenderName(string genderAbbr)
        {
            string genderName;

            switch (genderAbbr)
            {
                case "M":
                    genderName = Gender.Male.ToString();
                    break;
                case "F":
                    genderName = Gender.Female.ToString();
                    break;
                case "U":
                    genderName = Gender.Unknown.ToString();
                    break;
                default:
                    genderName = "Male";
                    break;
            }

            return genderName;
        }
        public static string GetMaritalStatus(string genderAbbr)
        {
            string maritalStatus;

            switch (genderAbbr)
            {
                case "A":
                    maritalStatus = MaritalStatus.Annulled.ToString();
                    break;
                case "D":
                    maritalStatus = MaritalStatus.Divorced.ToString();
                    break;
                case "I":
                    maritalStatus = MaritalStatus.Interlocutory.ToString();
                    break;
                case "L":
                    maritalStatus = "Legally Separated";
                    break;
                case "M":
                    maritalStatus = MaritalStatus.Married.ToString();
                    break;
                case "P":
                    maritalStatus = MaritalStatus.Polygamous.ToString();
                    break;
                case "S":
                    maritalStatus = "Single/Never Married";
                    break;
                case "T":
                    maritalStatus = "Domestic Partner ";
                    break;
                case "W":
                    maritalStatus = MaritalStatus.Widowed.ToString();
                    break;
                default:
                    maritalStatus = "";
                    break;
            }

            return maritalStatus;
        }
    }
}
