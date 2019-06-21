
namespace YL.Utils.Pub
{
    public class MessageItem
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public MessageItem(int code, string message)
        {
            this.Code = code;
            this.Message = message;
        }
    }
}