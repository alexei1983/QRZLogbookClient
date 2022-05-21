using System;
using System.Collections.Generic;

namespace QRZLogbookClient {

  public class QRZLogbookStatusRequest : QRZLogbookRequest {

    public string[] LogIDs { get; set; }

    public QRZLogbookStatusRequest(string key) : base(key, QRZAction.Status)
    {
    }

    public QRZLogbookStatusRequest(string key, params string[] logIDs) : this(key)
    {
      LogIDs = logIDs;
    }

    public override IQRZResponse Execute()
    {
      var parameters = new Dictionary<string, string>();

      if (LogIDs != null && LogIDs.Length > 0)
        parameters.Add(QRZFields.LogIDs, string.Join(",", LogIDs));

      var response = GetResponse(GetRequest(parameters));
      return new QRZStatusResponse(GetResponseCollection(response));
    }
  }
}
