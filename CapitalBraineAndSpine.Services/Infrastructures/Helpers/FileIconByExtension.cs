namespace CapitalBraineAndSpine.Services.Infrastructures.Helpers
{
    public class FileIconByExtension
    {
        public static string FileIconByExt(string fileExtension)
        {

            switch (fileExtension)
            {
                case "pdf":
                    return "fa-file-pdf-o";

                case "xls":
                case "xlsx":
                case "csv":
                    return "fa-file-excel-o";

                case "doc":
                case "docx":
                    return "fa-file-word-o";

                case "jpg":
                case "jpeg":
                case "png":
                    return "fa-file-image-o";

                default:
                    return "fa-file-text";

            }
        }

    }
}
