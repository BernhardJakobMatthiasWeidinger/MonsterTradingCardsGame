using System;

namespace MTCG.Exceptions {
    public class InDeckException : Exception {
        public InDeckException() {}
        public InDeckException(string message): base(message) {}
        public InDeckException(string message, Exception inner) : base(message, inner) { }
    }
}
