// Copyright (c) Mesut Talebi (mesut.talebi@gmail.com)
// Not: This File is based on Elmah library's log.cs file.


using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MT.Extensions.Logging.MsSql.Model
{
    /// <summary>
    /// Represents Log Instance 
    /// </summary>
    public sealed class Log 
    {
        private string _application;
        private string _category;
        private string _typeName;
        private string _source;
        private string _fileName;
        private string _message;
        private string _detail;
        private string _user;
        private string _stackTrace;

        /// <summary>
        /// Gets or sets the name of application in which this log occurred.
        /// </summary>   
        public string Application
        {
            get { return _application ?? string.Empty; }
            set { _application = value; }
        }

        /// <summary>
        /// Gets or sets the name of application in which this log occurred.
        /// </summary>   
        public string Category
        {
            get { return _category ?? string.Empty; }
            set { _category = value; }
        }        

        /// <summary>
        /// Gets or sets the type, class or category of the log.
        /// </summary>
        public string Type
        {
            get { return _typeName ?? string.Empty; }
            set { _typeName = value; }
        }

        /// <summary>
        /// Gets or sets the source that is the cause of the log.
        /// </summary>
        public string Source
        {
            get { return _source ?? string.Empty; }
            set { _source = value; }
        }

        /// <summary>
        /// Gets or sets the filename that the exception thrown.
        /// </summary>
        public string FileName
        {
            get { return _fileName ?? string.Empty; }
            set { _fileName = value; }
        }

        /// <summary>
        /// Gets or sets a brief text describing the log.
        /// </summary>
        public string Message
        {
            get { return _message ?? string.Empty; }
            set { _message = value; }
        }

        /// <summary>
        /// Gets or sets a detailed text describing the log, such as a
        /// stack trace.
        /// </summary>
        public string Detail
        {
            get { return _detail ?? string.Empty; }
            set { _detail = value; }
        }

        /// <summary>
        /// Gets or sets the user logged into the application at the time 
        /// of the log.
        /// </summary>
        public string User
        {
            get { return _user ?? string.Empty; }
            set { _user = value; }
        }

        public string StackTrace
        {
            get { return _stackTrace ?? string.Empty; }
            set { _stackTrace = value; }
        }

        /// <summary>
        /// Gets or sets the date and time (in local time) at which the 
        /// log occurred.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code of the output returned to the 
        /// client for the log.
        /// </summary>
        /// <remarks>
        /// For cases where this value cannot always be reliably determined, 
        /// the value may be reported as zero.
        /// </remarks>
        public int StatusCode { get; set; } = 500;
       
        /// <summary>
        /// Gets a collection representing the Header Collection
        /// captured as part of diagnostic data for the log.
        /// </summary>
        public IHeaderDictionary HeaderDictionary { get; }

        /// <summary>
        /// Gets a collection representing the Web query string variables
        /// captured as part of diagnostic data for the log.
        /// </summary>
        public IQueryCollection QueryString { get; }

        /// <summary>
        /// Gets a collection representing the form variables captured as 
        /// part of diagnostic data for the log.
        /// </summary>
        public IFormCollection Form { get; }

        /// <summary>
        /// Gets a collection representing the client cookies
        /// captured as part of diagnostic data for the log.
        /// </summary>
        public IRequestCookieCollection Cookies { get; }

        /// <summary>
        /// The unique Identifer of the current request.
        /// </summary>
        public string RequestId { get; set; }

        //[JsonIgnore]
        //public Exception Exception { get; }      

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class
        /// from a given <see cref="Exception"/> instance and 
        /// <see cref="HttpContext"/> instance representing the HTTP 
        /// context during the exception.
        /// </summary>
        public Log(Exception e, HttpContext context)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            var baseException = e.GetBaseException();
            
            // Load the basic information.                      
            _typeName = baseException.GetType().FullName;
            _message = baseException.Message;
            _source = baseException.Source;
            _detail = e.ToString();            
            Time = DateTime.UtcNow;

            _stackTrace = baseException.StackTrace;
            StackTrace st = new StackTrace(baseException, true);
            var frames = st.GetFrames();
            if (frames != null && frames.Any())
            {
                StackFrame frame = frames.First();
                var exMethodName = frame.GetMethod().Name;
                var exFileLineNumber = frame.GetFileLineNumber().ToString();

                _fileName = frame.GetFileName() + $"(MethodName: {exMethodName}, LineNumber: {exFileLineNumber})";
            }

            // If this is an HTTP exception, then get the status code               
            //StatusCode = context?.Response.StatusCode ?? 0;

            // If the HTTP context is available, then capture the
            // collections that represent the state request as well as
            // the user.            
            if (context != null)
            {
                var webUser = context.User;
                if (webUser != null 
                    && (webUser.Identity.Name ?? string.Empty).Length > 0)
                {
                    _user = webUser.Identity.Name;
                }

                var request = context.Request;

                HeaderDictionary = request.Headers;
                QueryString = request.Query;
                try { Form = request.Form; }
                catch { 
                    //ignore 
                }                
                Cookies = request.Cookies;
                RequestId = context.TraceIdentifier;
            }
        }

        /// <summary>
        /// Returns the value of the <see cref="Message"/> property.
        /// </summary>
        public override string ToString()
        {
            return this.Message;
        }

        /// <summary>
        /// Serializes Log Class
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);            
        }
    }
}
