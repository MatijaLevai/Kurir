using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KurirServer.Helpers
{
    public class Attachment
    {
        public object Content { get; set; }
        public string FileName { get; set; }
        public AttachmentType Type { get; set; }
        public enum AttachmentType { Json, Text }
        public Attachment(string fileName,object content){
            Content = content;
            FileName = fileName;
            Type = AttachmentType.Text;

        }
        public async Task<MemoryStream> ContentTopStreamAsync()
        {
            string text;

            switch (Type)
            {
                case AttachmentType.Json:
                    text = JsonConvert.SerializeObject(Content);
                    break;
                case AttachmentType.Text:
                    text = Content.ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
                    //break;
            }
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
            await writer.WriteAsync(text);
            await writer.FlushAsync();
            stream.Position = 0;

            return stream;
        }
    }
}
