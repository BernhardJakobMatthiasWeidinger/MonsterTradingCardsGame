using System;

namespace MTCG.Exceptions {
    public class FriendException : Exception {
        public FriendException() {}
        public FriendException(string message): base(message) {}
        public FriendException(string message, Exception inner) : base(message, inner) { }
    }
}
