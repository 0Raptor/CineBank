using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CineBank.Classes
{
    public class API
    {
        string ApiKey { get; set; }

        private const string Server = "https://api.themoviedb.org/";
        private const string GeneralInfoEP = Server + "3/search/movie";
        private const string DetailsEP = Server + "3/movie/";
        private const string ImageEP = "https://image.tmdb.org/t/p/w500";

        public API(string apiKey)
        { 
            ApiKey = apiKey;
        }

        /// <summary>
        /// Check if the API-Server can be reached using pings.
        /// </summary>
        /// <returns>True if ping succeeded. False if something went wrong: Expect a network issue.</returns>
        public bool CheckConnection()
        {
            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(Server.Replace("https://", "").Replace("/",""));
                if (reply.Status == IPStatus.Success) return true;
                else return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Query for the TMDB-internal ID of a movie.
        /// </summary>
        /// <param name="search">Name of movie to search for</param>
        /// <param name="language">(optional) Language of the results</param>
        /// <returns>ID of the movie or 0 if no movie was found or -1 in case of an error</returns>
        public int SearchForMovieID(string search, string language = "")
        {
            try
            {
                // build request
                string request = GeneralInfoEP + "?api_key=" + ApiKey
                    + "&query=" + Uri.EscapeDataString(search);
                if (!String.IsNullOrWhiteSpace(language))
                    request += "&language=" + Uri.EscapeDataString(language);

                // get data
                string json = "";
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    json = wc.DownloadString(request);
                }

                // extract information
                JObject response = JObject.Parse(json); //parse web request
                JArray results = (JArray)response["results"]; //get results
                if (results.Count == 0)  //no movie found? --> abort
                    return 0;
                JObject data = JObject.FromObject(results[0]); //parse 1st result as single object --> access into: data["id"] etc.

                int movieID = (int)data["id"];
                return movieID;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: API: Failed to search for movie id in TMDB: " + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Query TMDB for information of a movie
        /// </summary>
        /// <param name="search">Name of movie to search for</param>
        /// <param name="language">(optional) Language of the results</param>
        /// <param name="castToSave">(optional) (default: 7) Number of cast to save locally (first x occurences). Set to -1 to include all</param>
        /// <param name="includeCastsCharacter">(optional) (default: false) Also save name of the character an actor/actress played</param>
        /// <param name="downloadPoster">(optional) (default: false) Automatically download poster to download folder if available</param>
        /// <param name="downloadPoster">(optional) Directory path to save posters into. When empty user's picture folder is used.</param>
        /// <returns>APIResult object containing inforamtion OR APIResult containing a statuscode in ID-field (0 nothing found, -1 general error) OR null in case of an error </returns>
        public APIResult SearchForMovie(string search, string language = "", int castToSave = 7, bool includeCastsCharacter = false, bool downloadPoster = false, string posterDir = "")
        {
            try
            {
                APIResult obj = new APIResult();

                // get TMDB-internal id
                int movieID = SearchForMovieID(search, language);
                obj.Id = movieID;
                // check for error during id discovery
                if (movieID <= 0)
                    return obj;

                // build request
                string request = DetailsEP + movieID.ToString()
                    + "?api_key=" + ApiKey
                    + "&append_to_response=credits";
                if (!String.IsNullOrWhiteSpace(language))
                    request += "&language=" + Uri.EscapeDataString(language);

                // get data
                string json = "";
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    json = wc.DownloadString(request);
                }

                // parse information
                JObject response = JObject.Parse(json); // all information
                JObject responseCredits = JObject.FromObject(response["credits"]);
                JArray responseCast = (JArray)responseCredits["cast"];
                JArray responseCrew = (JArray)responseCredits["crew"];
                JArray responseGenres = (JArray)response["genres"];

                // extract simple data
                obj.Title = (string)response["original_title"];
                string description = (string)response["overview"];
                string tagline = (string)response["tagline"];
                obj.Description = tagline + "\r\n" + description;
                obj.Released = (string)response["release_date"];
                string cover = (string)response["poster_path"];

                // extract cast information
                List<string> cast = new List<string>();
                int i = 0;
                int max = castToSave; // set maximum number of cast members to obtain based on parameters
                if (castToSave == -1)
                    max = responseCast.Count;
                while (i < max && i < responseCast.Count) // loop over first x members of cast and add them to list
                {
                    JObject member = JObject.FromObject(responseCast[i]);
                    if (includeCastsCharacter) // check if character name should be included
                        cast.Add((string)member["name"] + " (" + (string)member["character"] + ")");
                    else
                        cast.Add((string)member["name"]);
                    i++;
                }
                obj.Cast = cast.ToArray();

                // extract crew information
                obj.Score = ""; // init strings
                obj.Director = "";
                for (i = 0; i < responseCrew.Count; i++) // loop over crew to extract relevant personel
                {
                    JObject member = JObject.FromObject(responseCrew[i]);

                    //get data if required
                    if ((string)member["department"] == "Sound" && (string)member["job"] == "Original Music Composer") obj.Score += (string)member["name"] + ", ";
                    if ((string)member["department"] == "Directing" && (string)member["job"] == "Director") obj.Director += (string)member["name"] + ", ";
                }
                if (obj.Score.Length > 2) // remove tailing ", "
                    obj.Score = obj.Score.Substring(0, obj.Score.Length - 2);
                if (obj.Director.Length > 2)
                    obj.Director = obj.Director.Substring(0, obj.Director.Length - 2);

                // extract genre information
                List<string> g = new List<string>();
                for (i = 0; i < responseGenres.Count; i++)
                {
                    JObject genre = JObject.FromObject(responseGenres[i]);
                    g.Add((string)genre["name"]);
                }
                obj.Genre = g.ToArray();

                // download poster
                if (downloadPoster && !String.IsNullOrWhiteSpace(cover))
                {
                    // prepare path to store file
                    string path = "";
                    if (!String.IsNullOrWhiteSpace(posterDir) && Directory.Exists(posterDir))
                        path = posterDir + cover.Replace("/", "\\");
                    else
                        path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + cover.Replace("/", "\\");

                    // download image
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFile(ImageEP + cover, path);
                    }

                    // update obj
                    obj.CoverPath = path;
                }

                // return result
                return obj;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: API: Failed get movie in TMDB: " + ex.Message);
                return null;
            }
        }
    }

    public class APIResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] Genre { get; set; }
        public string Released { get; set; }
        public string[] Cast { get; set; }
        public string Director { get; set; }
        public string Score { get; set; }
        public string CoverPath { get; set; }

    }
}
