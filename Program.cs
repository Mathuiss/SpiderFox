using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace InternetExplorer
{
    class Program
    {
        static bool _isFirst;
        static long _scrapes;
        static long _discoveries;
        static int _maxRequests;
        static string _entryPoint;
        static HttpClient _client;
        static DiscoveryContext _db;
        static Discovery[] _urls;

        static void Main(string[] args)
        {
            // Initialize prerequisites
            _client = new HttpClient();
            _db = new DiscoveryContext();
            _isFirst = true;
            _scrapes = 0;
            _discoveries = 0;
            ReadInput();

            try
            {
                // Entry point of scraper
                RunScraper();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Environment.Exit(0);
        }

        #region Setup

        // Method sets the startup parameters of the application
        static void ReadInput()
        {
            if (_db.Discoverys.Any())
            {
                bool discoveriesMade = true;

                while (discoveriesMade)
                {
                    Console.WriteLine("Existing records have been found in the database");
                    Console.Write("Do you want to delete them and start fresh? (y/n): ");
                    string input = Console.ReadLine().ToLower();

                    switch (input)
                    {
                        case "y":
                            _db.Discoverys.RemoveRange(_db.Discoverys.AsEnumerable());
                            _db.SaveChangesAsync();
                            discoveriesMade = false;
                            break;
                        case "n":
                            discoveriesMade = false;
                            _isFirst = false;

                            while (true)
                            {
                                Console.Write($"Enter the scrape number you want to continue from: ");

                                try
                                {
                                    _scrapes = int.Parse(Console.ReadLine());
                                    _entryPoint = GetCurrentRecord(_scrapes);
                                    Console.WriteLine(GetHighestRecord().ToString());
                                    _discoveries = GetHighestRecord().Id;
                                    break;
                                }
                                catch
                                {
                                    Console.WriteLine("Please enter an integer");
                                }
                            }
                            break;
                        default: break;
                    }
                }
            }

            while (true)
            {
                if (_entryPoint == null)
                {
                    Console.Write("Please enter the starting URL: ");
                    string input = Console.ReadLine();

                    //Check if request is returned
                    if (!string.IsNullOrEmpty(SendRequest(input)))
                    {
                        _entryPoint = input;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            while (true)
            {
                Console.WriteLine("Please enter the maximum amount of scrapes you want to perform.");
                Console.Write("Enter 0 for infinite requests: ");

                string input = Console.ReadLine();

                try
                {
                    _maxRequests = int.Parse(input);
                    break;
                }
                catch
                {
                    Console.WriteLine("Input must be an integer.");
                }
            }
        }

        #endregion Setup
        #region MainLoop

        static void RunScraper()
        {
            if (_maxRequests != 0)
            {
                while (_scrapes < _maxRequests)
                {
                    try
                    {
                        Scrape();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        _scrapes++;
                    }
                }
            }
            else
            {
                while (true)
                {
                    try
                    {
                        Scrape();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        _scrapes++;
                    }
                }
            }
        }

        static void Scrape()
        {
            // Special case because it get's the highest record from the database
            if (_isFirst)
            {
                Discovery highestRecord = GetHighestRecord();
                _discoveries = highestRecord.Id;
                string highestBody = SendRequest(highestRecord.Url);
                _urls = FindUrls(highestBody, highestRecord.Url);
                SaveUrls(_urls);
                _scrapes++;
                _isFirst = false;
            }

            string currentUrl = GetCurrentRecord(_scrapes);
            string body = SendRequest(currentUrl);
            _urls = FindUrls(body, currentUrl);
            SaveUrls(_urls);
            _scrapes++;
        }

        #endregion MainLoop
        #region DatabaseControl

        // Gets the highest record from database on startup
        static Discovery GetHighestRecord()
        {
            if (_db.Discoverys.Any())
            {
                var d = _db.Discoverys.Last();
                return d;
            }
            else
            {
                Console.WriteLine("Attempting to create first record...");

                _db.Discoverys.Add(new Discovery(_discoveries, _entryPoint));
                _db.SaveChangesAsync();
                return new Discovery(_discoveries, _entryPoint);
            }
        }

        // Gets the record the scraper is on after one cycle
        static string GetCurrentRecord(long id)
        {
            try
            {
                return _db.Discoverys.Find(id).Url;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(0);
                throw ex;
            }
        }

        // Saves the discoveries that were found
        static void SaveUrls(Discovery[] urls)
        {
            for (int i = 0; i < urls.Length; i++)
            {
                _db.Discoverys.Add(urls[i]);
            }

            _db.SaveChangesAsync();
        }

        #endregion DatabaseControl
        #region ScrapeLogic

        // Sends request to url to retrieve web page
        static string SendRequest(string url)
        {
            Console.WriteLine($"{_scrapes} request : {url}");
            return _client.GetStringAsync(url).Result;
        }

        // Crawls the page and discovers urls (Discoveries)
        static Discovery[] FindUrls(string body, string currentUrl)
        {
            var regex = new Regex("<a href=\"(.*?)\"");
            MatchCollection matches = regex.Matches(body);
            string currentSite = Regex.Match(currentUrl, @"(http|https)\:\/\/[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,4}").ToString();

            var resList = new List<Discovery>();

            foreach (Match match in matches)
            {
                try
                {
                    string res = match.ToString();
                    res = res.Replace("<a href=\"", "")
                        .Replace("\"", "");
                    // Also clean up the remaining values in the <a> tag
                    // Irregularities uccur at: <a href="blieblabloe.com" x="y">

                    if (res.StartsWith("//www"))
                    {
                        res = res.Replace("//www", "www");
                    }

                    if (!res.StartsWith("http") && !res.StartsWith("www"))
                    {
                        if (!res.StartsWith("/"))
                        {
                            res = res.Replace(res, $"/{res}");
                        }

                        res = res.Replace(res, $"{currentSite}{res}");
                    }

                    // Check if result is duplicate and discovery is invalid
                    // Then continue to next discovery
                    // Else add discovery to the list
                    try
                    {
                        if (_db.Discoverys.Any(d => d.Url == res))
                        {
                            continue;
                        }
                        else
                        {
                            Console.WriteLine($"Discovered {_discoveries} {res}");
                            long id = _discoveries + 1;
                            _discoveries++;
                            resList.Add(new Discovery(id, res));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }

            return resList.ToArray();
        }

        #endregion ScrapeLogic
    }
}
