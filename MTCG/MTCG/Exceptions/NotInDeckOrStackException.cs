using System;

namespace MTCG.Exceptions {
    public class NotInDeckOrStackException : Exception {
        public NotInDeckOrStackException() {}
        public NotInDeckOrStackException(string message): base(message) {}
        public NotInDeckOrStackException(string message, Exception inner) : base(message, inner) { }
    }
}
