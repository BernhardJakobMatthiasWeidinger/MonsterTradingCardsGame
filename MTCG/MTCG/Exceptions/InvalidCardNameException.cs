using System;

namespace MTCG.Exceptions {
    public class InvalidCardNameException : Exception {
        public InvalidCardNameException() {}
        public InvalidCardNameException(string message): base(message) {}
        public InvalidCardNameException(string message, Exception inner) : base(message, inner) { }
    }
}
