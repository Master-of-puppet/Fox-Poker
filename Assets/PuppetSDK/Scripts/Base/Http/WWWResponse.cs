using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Puppet.Core.Network.Http
{
    public class WWWResponse : IHttpResponse
    {
        HttpStatusCode _state = HttpStatusCode.Unused;
        string _error;
        string _data;
        public WWW www;
        byte[] _bytes;

        public HttpStatusCode State
        {
            get { return _state; }
            set { _state = value; }
        }

        public string Error
        {
            get { return _error; }
            set { _error = value; }
        }

        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public byte[] Bytes 
        { 
            get { return _bytes; }
            set { _bytes = value; }
        }
    }
}
