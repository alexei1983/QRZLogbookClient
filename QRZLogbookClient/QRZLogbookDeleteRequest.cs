using System;
using System.Collections.Generic;

namespace QRZLogbookClient {

  public class QRZLogbookDeleteRequest : QRZLogbookRequest {

    public string[] LogIDs { get; set; }

    public QRZLogbookDeleteRequest(string key) : base(key, QRZAction.Delete)
    {
    }

    public QRZLogbookDeleteRequest(string key, params string[] logIDs) : this(key)
    {
      LogIDs = logIDs;
    }

    public override IQRZResponse Execute()
    {
      if (LogIDs == null || LogIDs.Length < 1)
        throw new InvalidOperationException("At least one log ID is required.");

      var response = GetResponse(GetRequest(new Dictionary<string, string>() { { QRZFields.LogIDs, string.Join(",", LogIDs) } }));
      return new QRZDeleteResponse(GetResponseCollection(response));
    }
  }
}
