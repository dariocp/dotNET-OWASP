using System;
using System.Net;

namespace A7.Attacker
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:50666/hack/");
                listener.Start();

                Console.WriteLine("Listening...");

                for (; ; )
                {
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    Console.WriteLine(request.Url);

                    using (HttpListenerResponse response = context.Response)
                    {
                        var responseString = "<HTML><BODY>Hacked!</BODY></HTML>";
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                        response.ContentLength64 = buffer.Length;

                        using (var output = response.OutputStream)
                        {
                            output.Write(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
        }
    }
}