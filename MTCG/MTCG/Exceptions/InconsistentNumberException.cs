using System;

namespace MTCG.Exceptions {
    public class InconsistentNumberException : Exception {
        public InconsistentNumberException() {}
        public InconsistentNumberException(string message): base(message) {}
        public InconsistentNumberException(string message, Exception inner) : base(message, inner) { }
    }
}
