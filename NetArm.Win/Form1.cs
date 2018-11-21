using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetArm.Core;

namespace NetArm.Win
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            int tps = Convert.ToInt32(txt_tps.Text);
            string url = txt_Url.Text;
            var headers = rtxt_header.Text;

            var splitHeaders = headers.Split('\n');

            int[] input = Enumerable.Range(0, tps).ToArray();
            ConcurrentBag<NetTraceRecord> results = new ConcurrentBag<NetTraceRecord>();
            try
            {
                var tasks = input.Select(async item =>
                {
                    NetTraceRecord ntr = new NetTraceRecord();
                    HttpClient client = new HttpClient();
                    Stopwatch sw = Stopwatch.StartNew();
                    foreach (var header in splitHeaders)
                    {
                        var k_v = header.Split(':');
                        client.DefaultRequestHeaders.TryAddWithoutValidation(k_v[0], k_v[1]);
                    }

                    //client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhYWEzM3MzM3YiLCJqdGkiOiIzZWVjODUxMi1kNDE0LTQyNzItYTY0Yi00OTcxODI0Y2Q3ODkiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImVkMjRmNzM2LWQzYjAtNDk1OS1iODUzLWFiYjU1ZDYyN2YyNCIsImV4cCI6MTU0NDg1NDM4NiwiaXNzIjoicHdjIiwiYXVkIjoicHdjIn0.FvN7sGw5Us7nuKoOCSzi8JNp8CoTcVJudT5xfd9uR3o");
                    try
                    {
                        ntr.StartTime = DateTime.Now;
                        var response = await client.GetAsync(url);
                        ntr.EndTime = DateTime.Now;
                        ntr.StatusCode = response.StatusCode;
                        ntr.Message = await response.Content.ReadAsStringAsync();
                        if (response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var aaa = 0;
                        }
                    }
                    catch (Exception exception)
                    {
                        sw.Stop();
                        ntr.EndTime = DateTime.Now;
                        ntr.StatusCode = HttpStatusCode.ExpectationFailed;
                        ntr.Message = exception.Message;
                    }
                    finally
                    {
                        results.Add(ntr);
                    }
                });
                await Task.WhenAll(tasks);
                var count = results.Count;
                var failResult = results.Where(bbb => bbb.StatusCode != HttpStatusCode.OK).ToList();
                var minDuration = results.Min(item => item.Duration);
                var maxDuration = results.Max(item => item.Duration);
                var minTime = results.Min(item => item.StartTime);
                var maxTime = results.Max(item => item.EndTime);

            }

            catch (AggregateException ex)
            {
                Console.WriteLine("Parallel.ForEach has thrown an exception. THIS WAS NOT EXPECTED.\n{0}", e);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }
    }
}
