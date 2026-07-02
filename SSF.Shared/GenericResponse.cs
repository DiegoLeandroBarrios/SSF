using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSF.Shared
{
    public enum StatusCodeType
    {
        Success,
        ValidationError,
        UnexpectedError,
        NotFound
    }
    public class GenericResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int Status { get; set; }
        public T? Data { get; set; }
    }
}
