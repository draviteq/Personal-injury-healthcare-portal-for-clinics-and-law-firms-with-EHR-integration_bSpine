using System;

namespace CapitalBraineAndSpine.Service.Models.ApiResponse
{
    [Serializable]
    public class ApiResponse
    {
        public bool Status { get; set; } = false;

        public object Data { get; set; } = null;

        public string Message { get; set; } = string.Empty;

    }
}
