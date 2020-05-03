
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

        public override string ToString()
        {
            string result = "";
            if(Code < 0)
            {
                result += "E-";
            }
            else
            {
                result += "I-";
            }
            result += Code + "," + Message;
            return result;
        }
    }
}