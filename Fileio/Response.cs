using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fileio;
public class Response
{
    public bool Success { get; set; }
    public HttpStatusCode Status { get; set; }
}

public class ListResponse<T> : Response
{
    public List<T> Nodes { get; set; }
    public int Count { get; set; }
    public int Size { get; set; }
    public string ScreeningStatus { get; set; }
}