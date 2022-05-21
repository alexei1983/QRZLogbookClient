using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;

namespace QRZLogbookClient {

  public static class QRZAction {
    public const string Fetch = "FETCH";
    public const string Status = "STATUS";
    public const string Insert = "INSERT";
    public const string Delete = "DELETE";
  }

  public static class QRZFields {

    public const string Action = "ACTION";
    public const string Option = "OPTION";
    public const string Result = "RESULT";
    public const string Count = "COUNT";
    public const string ADIF = "ADIF";
    public const string LogIDs = "LOGIDS";
    public const string LogID = "LOGID";
    public const string Data = "DATA";
    public const string Reason = "REASON";
    public const string Key = "KEY";
    public const string Status = "STATUS";
  }

  public static class QRZResult {
    public const string OK = "OK";
    public const string Fail = "FAIL";
    public const string Partial = "PARTIAL";
    public const string Replace = "REPLACE";
    public const string Auth = "AUTH";
  }

  public static class QRZOption {
    public const string All = "ALL";
    public const string DXCC = "DXCC";
    public const string Between = "BETWEEN";
    public const string Band = "BAND";
    public const string Mode = "MODE";
    public const string Call = "CALL";
    public const string Max = "MAX";
    public const string Type = "TYPE";
    public const string Status = "STATUS";
    public const string Replace = "REPLACE";
    public const string LogIDs = "LOGIDS";
  }

  public interface IQRZLogbookRequest {
    string Option { get; }
    string UserAgent { get; set; }
    string Action { get; }
    string Key { get; }

    void SetOption(string optionKey, string optionValue);

    void SetOption(string option);

    IQRZResponse Execute();
  }

  public abstract class QRZLogbookRequest : IQRZLogbookRequest {

    public string Option { get; private set; }
    public string UserAgent { get; set; }
    public string Action { get; }
    public string Key { get; }

    const string URL = "https://logbook.qrz.com/api";

    public QRZLogbookRequest(string key, string action)
    {
      Action = action;
      Key = key;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="optionKey"></param>
    /// <param name="optionValue"></param>
    public virtual void SetOption(string optionKey, string optionValue)
    {
      SetOption($"{optionKey}:{optionValue}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="option"></param>
    public virtual void SetOption(string option)
    {
      if (string.IsNullOrEmpty(Option))
        Option = $"{option}";
      else
        Option += $";{option}";
    }

    protected virtual HttpWebRequest GetRequest(IDictionary<string, string> parameters = null)
    {
      var request = (HttpWebRequest)WebRequest.Create(URL);

      if (!string.IsNullOrEmpty(UserAgent))
        request.UserAgent = UserAgent;

      request.Method = "POST";
      request.ContentType = "application/x-www-form-urlencoded";
      string body = $"{QRZFields.Key}={Key}&{QRZFields.Action}={Action}";

      if (!string.IsNullOrEmpty(Option))
        body += $"&{QRZFields.Option}={Option}";

      if (parameters != null)
      {
        foreach (var key in parameters.Keys)
           body += $"&{key}={parameters[key] ?? string.Empty}";
      }

      using (var writer = new StreamWriter(request.GetRequestStream()))
        writer.Write(body);

      // enable TLS 1.2 protocol
      request.ServicePoint.Expect100Continue = false;
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

      return request;
    }

    protected virtual string GetResponse(HttpWebRequest request)
    {
      var response = (HttpWebResponse)request.GetResponse();

      //
      var responseStream = response.GetResponseStream();
      if (responseStream == null)
        throw new Exception("Error reading response stream");

      var responseBody = string.Empty;

      using (var reader = new StreamReader(responseStream))
        responseBody = reader.ReadToEnd();

      return responseBody;
    }

    private Dictionary<string, string> ParseQuery(string query)
    {
      var dict = new Dictionary<string, string>();
      var regex = new Regex("(?:[?&]|^)([^&]+)=([^&]*)");
      var matches = regex.Matches(query);
      foreach (Match match in matches)
      {
        dict[match.Groups[1].Value] = Uri.UnescapeDataString(match.Groups[2].Value);
      }
      return dict;
    }

    protected virtual IDictionary<string, string> GetResponseCollection(string response)
    {
      response = HttpUtility.HtmlDecode(response);
      return ParseQuery(response);
    }

    public abstract IQRZResponse Execute();
  }
}
