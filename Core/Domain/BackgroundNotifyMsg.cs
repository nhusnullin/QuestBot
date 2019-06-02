using System;

namespace Core.Domain
{
    public class BackgroundNotifyMsg
    {
        public UserId UserId { get; set; }
        public DateTime WhenByUTC { get; set; }
        public string Msg { get; set; }
        public bool WasSend { get; set; }
    }
}