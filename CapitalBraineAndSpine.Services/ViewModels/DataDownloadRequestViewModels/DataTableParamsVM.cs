namespace CapitalBraineAndSpine.Services.ViewModels.DataDownloadRequestViewModels
{
    public class DataTableParamsVM
    {
        public string? userId { get; set; }
        public string draw { get; set; }
        public string start { get; set; }
        public string length { get; set; }
        public string sortColumn { get; set; }
        public string sortColumnDirection { get; set; }
        public string searchValue { get; set; }
        public int pageSize { get; set; }
        public int skip { get; set; }
        public int recordsTotal { get; set; }
        public string userRole { get; set; }
        public string extraParam { get; set; }
        //public bool isinternal { get; set; }

    }
}
