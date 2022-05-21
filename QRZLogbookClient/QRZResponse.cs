using System;
using System.Collections.Generic;
using ADIF.NET;

namespace QRZLogbookClient {

  public interface IQRZResponse {
    string Result { get; set; }
    int? Count { get; set; }
    string Reason { get; set; }
  }

  public class QRZResponse : IQRZResponse {
    public string Result { get; set; }
    public int? Count { get; set; }
    public string Reason { get; set; }

    public QRZResponse(IDictionary<string, string> dict)
    {
      if (dict != null)
      {
        if (dict.ContainsKey(QRZFields.Result))
          Result = dict[QRZFields.Result];
        else if (dict.ContainsKey(QRZFields.Status))
          Result = dict[QRZFields.Status];

        if (dict.ContainsKey(QRZFields.Reason))
          Reason = dict[QRZFields.Reason];

        if (dict.ContainsKey(QRZFields.Count))
        {
          if (int.TryParse(dict[QRZFields.Count], out int count))
            Count = count;
        }
      }
    }
  }

  public class QRZInsertResponse : QRZResponse, IQRZResponse {
    public string LogID { get; set; }

    public QRZInsertResponse(IDictionary<string, string> dict) : base(dict)
    {
      if (dict.ContainsKey(QRZFields.LogID))
        LogID = dict[QRZFields.LogID];
    }
  }

  public class QRZDeleteResponse : QRZResponse, IQRZResponse {
    public string[] LogIDs { get; set; }

    public QRZDeleteResponse(IDictionary<string, string> dict) : base(dict)
    {
      if (dict.ContainsKey(QRZFields.LogIDs))
        LogIDs = dict[QRZFields.LogIDs].Split(',');
    }
  }

  public class QRZStatusResponse : QRZResponse, IQRZResponse {
    public IDictionary<string, string> Data { get; set; }

    public DateTime? StartDate { get; }
    public DateTime? EndDate { get; }
    public int? ConfirmedCount { get; }
    public string BookName { get; }
    public string Callsign { get; }
    public string Owner { get; }
    public string BookID { get; }
    public int? DXCCCount { get; }

    public QRZStatusResponse(IDictionary<string, string> dict) : base(dict)
    {
      Data = dict;

      if (dict.ContainsKey("CALLSIGN"))
        Callsign = dict["CALLSIGN"];

      if (dict.ContainsKey("OWNER"))
        Owner = dict["OWNER"];

      if (dict.ContainsKey("BOOKID"))
        BookID = dict["BOOKID"];

      if (dict.ContainsKey("BOOK_NAME"))
        BookName = dict["BOOK_NAME"];

      if (dict.ContainsKey("CONFIRMED"))
      {
        if (int.TryParse(dict["CONFIRMED"], out int confirmedCount))
          ConfirmedCount = confirmedCount;
      }

      if (dict.ContainsKey("DXCC_COUNT"))
      {
        if (int.TryParse(dict["DXCC_COUNT"], out int dxccCount))
          DXCCCount = dxccCount;
      }

      if (dict.ContainsKey("START_DATE"))
      {
        if (DateTime.TryParse(dict["START_DATE"], out DateTime startDate))
          StartDate = startDate;
      }

      if (dict.ContainsKey("END_DATE"))
      {
        if (DateTime.TryParse(dict["END_DATE"], out DateTime endDate))
          EndDate = endDate;
      }
    }
  }

  public class QRZFetchResponse : QRZResponse, IQRZResponse {
    public string[] LogIDs { get; set; }
    public ADIFQSOCollection ADIF { get; set; }

    public QRZFetchResponse(IDictionary<string, string> dict) : base(dict)
    {
      if (dict.ContainsKey(QRZFields.LogIDs))
        LogIDs = dict[QRZFields.LogIDs].Split(',');

      if (dict.ContainsKey(QRZFields.ADIF) && !string.IsNullOrEmpty(dict[QRZFields.ADIF]))
      {
        var parser = new ADIFParser();
        parser.Load(dict[QRZFields.ADIF]);
        var adifResult = parser.Parse();
        if (adifResult != null && adifResult.TotalQSOs > 0)
          ADIF = adifResult.QSOs;
      }
    }
  }
}
